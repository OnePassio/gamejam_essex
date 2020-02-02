/// <summary>
/// OneP GM Console version 0.6
/// Modified by Strong D
/// </summary>
///
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Console = OnePStudio.Console.Console;
using OnePStudio.Console;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OneP.GMConsole;

public class GMEditorControl : EditorWindow
{
	internal static GMEditorControl instance;
    private Vector2 _scroll;

	private string command = "";

	//
	private static GUIStyle _righAlignMiniLabel;
	private static GUIStyle righAlignMiniLabel
	{
		get
		{
			if (_righAlignMiniLabel == null)
			{
				_righAlignMiniLabel = new GUIStyle(EditorStyles.miniLabel);
				_righAlignMiniLabel.alignment = TextAnchor.MiddleRight;
			}
			return _righAlignMiniLabel;
		}
	}

	const float LifeTime = 10, HistorySteps = 128;
	private int _autoClearTime = 180;

    [MenuItem("Tool/GMTool/OneP GM Command")]
    public static void CreateWindow()
    {
		instance = GetWindow<GMEditorControl>();

#pragma warning disable 0618
        instance.title = "OneP GM Command";
#pragma warning restore 0618
        instance.minSize = new Vector2(256, 256);
        instance.Show();
        instance.ShowUtility();
        instance.autoRepaintOnSceneChange = false;
        instance.wantsMouseMove = false;
        //Application.RegisterLogCallback(instance.LogCallback);
    }

    private void LogCallback (string condition, string stackTrace,  UnityEngine.LogType type)
    {
        Color color = Color.white;
        switch(type)
        {
			case UnityEngine.LogType.Exception:
			case UnityEngine.LogType.Error:
			case UnityEngine.LogType.Assert:
                color = Color.red;
                break;
			case UnityEngine.LogType.Warning:
                color = Color.yellow;
                break;
        }

		//string descriptive = string.Format("[{0}] {1}\n{2}", type, condition, stackTrace);
		Console.Log("UNITY DEBUG", condition, GMLogger.FormatCallstack(stackTrace), color);
    }

    
    void OnGUI()
    {
        List<object> toDelete = new List<object>();
        if (Console.entries == null)
        {
            Debug.Log("No Entries Dictionary");
            return;
        }
        DrawToolbar();
        _scroll = GUILayout.BeginScrollView(_scroll);
        {
            foreach (KeyValuePair<object, Console.DebugObject> dob in Console.entries)
            {
                double millisecs = ((dob.Value.lastTime.AddSeconds(LifeTime)) - DateTime.Now).TotalMilliseconds * .01f;

                GUI.color = ((dob.Value.lastTime.AddSeconds(LifeTime)) - DateTime.Now).Seconds > 0 ? Color.Lerp(dob.Value.color, dob.Value.color.A(.5f), (float)(LifeTime / millisecs)) : dob.Value.color.A(.5f);

                using (new Layout.Horizontal())
                {
                    switch (dob.Value.owner != null)
                    {
                        case true :
							if (GUILayout.Button(new GUIContent("", "Ping GameObject"), EditorStyles.radioButton, GUILayout.Width(16)))
							{
								EditorGUIUtility.PingObject(dob.Value.owner);
							}
                            break;
                        case false :
                            if (!dob.Value.noMonobeh) toDelete.Add(dob.Key);
                            break;
                    }

                    dob.Value.open = EditorGUILayout.Foldout(dob.Value.open, string.Format("{0}, ({1}) (Events : {2})", dob.Key, dob.Value.owner != null ? dob.Value.owner.name : ("non MonoBehaviour"), dob.Value.GetMessageHolder().Count));
                }
                if (dob.Value.GetMessageHolder().Count == 0) toDelete.Add(dob.Key);
                if(_autoClearTime > 0 && Event.current.type == EventType.Repaint)
                {
                    List<string> toDeleteMessages = null;
                    foreach (KeyValuePair<string, Console.DebugObject> mess in dob.Value.GetMessageHolder())
                    {
                        if ((DateTime.Now - (mess.Value.lastTime)).Seconds > _autoClearTime)
                        {
                            if (toDeleteMessages == null) toDeleteMessages = new List<string>();
                            toDeleteMessages.Add(mess.Key);
                        }
                    }
                    if (toDeleteMessages != null)
                        foreach (string s in toDeleteMessages)
                            dob.Value.RemoveEntry(s);
                }
                if (!dob.Value.open) continue;

                foreach (KeyValuePair<string, Console.DebugObject> mess in dob.Value.GetMessageHolder())
                {
                    GUI.color = Color.white;

					// Timestamp label
                    using (new Layout.Horizontal())
                    {
						string time = string.Format("{0}:{1}:{2}:{3}", mess.Value.lastTime.TimeOfDay.Hours, mess.Value.lastTime.TimeOfDay.Minutes,
												mess.Value.lastTime.TimeOfDay.Seconds, mess.Value.lastTime.Millisecond);
						//GUILayout.Label(time, GUILayout.Width(90));

						GUILayout.Label(string.Format("{0}", time), 
							righAlignMiniLabel,
							GUILayout.Width(90));
					
						/*GUILayout.Space(-10);
						using (new Layout.Horizontal(GUILayout.Width(90)))
						{
							GUILayout.FlexibleSpace();
							//GUILayout.Label(string.Format("History ({0})", mess.Value.history.Count), EditorStyles.miniLabel);
							//if (GUILayout.Button(new GUIContent("", "Show callstack"), "box", GUILayout.Width(16)))
							//{
							//	mess.Value.open = !mess.Value.open;
							//}

							mess.Value.open = EditorGUILayout.Foldout(mess.Value.open,
																		  new GUIContent(
																			  string.Format("({0})",
																					mess.Value.history.Count),
																		  "Show last 128 records"));
								
						}*/

						// Actual log message
						millisecs = ((mess.Value.lastTime.AddSeconds(LifeTime)) - DateTime.Now).TotalMilliseconds * .01f;
						GUI.color = ((mess.Value.lastTime.AddSeconds(LifeTime)) - DateTime.Now).Seconds > 0
										? Color.Lerp(mess.Value.color, mess.Value.color.A(.5f), (float)(LifeTime / millisecs))
										: mess.Value.color.A(.5f);

						string content = mess.Value.Value.ToString();

						// Remove callstack information if any
						int firstCallstackPos = content.IndexOf("$");
						if (firstCallstackPos >= 0) // Log item has callstack
						{
							//Debug.LogWarning("Match at " + match.Index);
							content = content.Substring(0, firstCallstackPos);
						}

						string sub = string.Empty;
						if (mess.Value.history.Count > 1)
							sub += string.Format("({0}) ", mess.Value.history.Count);
						if (!mess.Key.StartsWith("#"))
							sub += mess.Key;

						if (GUILayout.Button(
							sub.Length > 0 ? string.Format("{0}\n{1}", content.Trim(), sub) : content.Trim(), 
							EditorStyles.label))
						{
							mess.Value.open = !mess.Value.open;
						}

						//GUILayout.Label(string.Format("{0}\n{1}", mess.Key, content.Trim()));

						GUI.color = Color.white;	
                    }
					/*using (new Layout.Horizontal())
					{
						GUILayout.Space(20);
						if (mess.Value.history.Count > 1)
						{
							mess.Value.open = EditorGUILayout.Foldout(mess.Value.open,
																		  new GUIContent(
																			  string.Format("Show History (messages : {0})",
																					mess.Value.history.Count),
																		  "Show last 128 records"));
						}
					}*/
                    if (mess.Value.open)
                    {
                        //GUILayout.FlexibleSpace(); 
                        /*mess.Value.scroll = GUILayout.BeginScrollView(mess.Value.scroll, GUI.skin.textArea);
                        {
                            int step = 0;
                            foreach (Console.DebugObject d in mess.Value.history)
                            {
                                using (new Layout.Horizontal())
                                {
                                    string time = string.Format("{0}:{1}:{2}:{3}", d.lastTime.TimeOfDay.Hours, d.lastTime.TimeOfDay.Minutes,
                                                   d.lastTime.TimeOfDay.Seconds, d.lastTime.Millisecond);
                                    GUILayout.Label(time, GUILayout.Width(90));
									GUILayout.TextArea(string.Format("{0}\n{1}", mess.Key, d.Value), EditorStyles.label, GUILayout.ExpandHeight(true));
                                    if (++step == HistorySteps) break;
                                }
                            }
                        }
                        GUILayout.EndScrollView(); 
						//*/

						DrawFullMessageWithStackTrace(mess.Value.history);
                    }
                    GUI.color = Color.white;
                    GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
                }
                GUI.color = Color.white;
            }

            if (Event.current.type == EventType.Repaint && toDelete.Count > 0)
            {
                foreach (object obj in toDelete)
                    if (Console.entries.ContainsKey(obj)) Console.entries.Remove(obj);
                toDelete.Clear();
            }
        }
        GUILayout.EndScrollView();

		GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

		DrawCommandLineInput();

		GUILayout.FlexibleSpace();
        //Repaint();
    }

	void DrawFullMessageWithStackTrace(List<Console.DebugObject> history)
	{
		int step = 0;
		foreach (var d in history)
		{
			using (new Layout.Horizontal())
			{
				string time = string.Format("{0}:{1}:{2}:{3}", d.lastTime.TimeOfDay.Hours, d.lastTime.TimeOfDay.Minutes,
							   d.lastTime.TimeOfDay.Seconds, d.lastTime.Millisecond);
				GUILayout.Label(time, righAlignMiniLabel, GUILayout.Width(90));

				var lines = d.Value.ToString().Split(new char[] {'$'});
				using (new Layout.Vertical("box"))
				{
					// Build regex pattern for callstack line
					for( int i = 0; i < lines.Length; i++ )
					{
						string line = lines[i].Trim();
						if (line == string.Empty)
							continue;

						if (i == 0)
						{
							int newlineCount = 1;
							foreach(char c in line)
							{
								if(c == '\n')
									newlineCount++;
							}

							EditorGUILayout.SelectableLabel(line, 
								GUILayout.ExpandWidth(true),
								GUILayout.Height(14 * newlineCount));
							continue;
						}

						using (new Layout.Horizontal())
						{

							// Callstack item (with file and line)
							if (line.StartsWith("@"))
							{
								if (GUILayout.Button(new GUIContent(">", "Go to source"),
									EditorStyles.miniButton,
									GUILayout.Width(20)))
								{
									int begin = line.IndexOf("(at ");
									if( begin < 0 )
										begin = line.IndexOf(" in ");
									if (begin >= 0)
									{
										begin += 4;

										int end = line.LastIndexOf(':');
										string scriptPath = line.Substring(begin, end - begin);

										begin = end + 1;
										end = line.LastIndexOf(')');
										if (end < 0 || end <= begin)
											end = line.Length;
										int scriptLine = Int32.Parse(line.Substring(begin, end - begin));
										Debug.Log(scriptPath + ":" + scriptLine);
										OpenScriptFileInVisualStudioIDE(scriptPath, scriptLine);
									}
								}

								EditorGUILayout.SelectableLabel(line.Substring(1), // remove @ char
									GUILayout.ExpandWidth(true),
									GUILayout.Height(14));
							}
							else // Callstack item (no file and line)
							{
								GUILayout.Button(new GUIContent(" ", "Source is not available"),
									EditorStyles.miniButton,
									GUILayout.Width(20));

								EditorGUILayout.SelectableLabel(line, // remove @ char
									GUILayout.ExpandWidth(true),
									GUILayout.Height(14));
							}

							//GUILayout.FlexibleSpace();
						}
					}
				}
				if (++step == HistorySteps) break;
			}
		}
	}

    void DrawToolbar()
    {
        if (instance == null) CreateWindow();
        GUI.Box(new Rect(0, 0, instance.position.width, 10),"", EditorStyles.toolbar);

		Rect curRect = new Rect(0, 0, 75, 16);
		EditorGUI.DropShadowLabel(curRect, "Command");

		// Expand all button
		curRect.x += curRect.width;
		curRect.width = 60;
		if (GUI.Button(curRect, new GUIContent("Expand", "Expand all log items"), EditorStyles.toolbarButton))
		{
			Dictionary<object, Console.DebugObject> objects = Console.entries;
			foreach (KeyValuePair<object, Console.DebugObject> dob in objects)
				dob.Value.open = true;
		}

		// Collapse all
		curRect.x += curRect.width;
		curRect.width = 60;
		if (GUI.Button(curRect, new GUIContent("Collapse", "Collapse all foldouts"), EditorStyles.toolbarButton))
		{
			Dictionary<object, Console.DebugObject> objects = Console.entries;
			foreach (KeyValuePair<object, Console.DebugObject> dob in objects)
				dob.Value.open = false;
		}

		// Clear button
		curRect.x += curRect.width + 10;
		curRect.width = 60;
		if (GUI.Button(curRect, new GUIContent("Clear", "Clear all items"), EditorStyles.toolbarButton))
		{
			Console.ClearAll();
		} 
		curRect.x += curRect.width + 10;
		curRect.width = 100;
		if (GUI.Button(curRect, new GUIContent("Close GM Tool", "Close GM"), EditorStyles.toolbarButton))
		{
			instance.Close ();
		} 
		// Auto clear
		string[] timeNames = new string[] { "Infinite", "30s", "60s", "180s" };
		int[] timeValues = new int[] { 0, 30, 60, 180 };

		curRect.x += curRect.width;
		curRect.width = 60;
		_autoClearTime = EditorGUI.IntPopup(curRect, _autoClearTime, timeNames, timeValues, EditorStyles.toolbarDropDown);
		//EditorGUI.Popup(curRect, 0, timeNames, EditorStyles.toolbarDropDown);

		// Filters text
		curRect.x += curRect.width + 10;
		curRect.width = position.width - curRect.x - 5;
		Rect textRect = curRect;
		textRect.y += 2;
		EditorGUI.TextField(textRect, "", EditorStyles.toolbarTextField);


		using (new Layout.Vertical())
		{
			GUILayout.Space(20);
		}
    }

	void DrawCommandLineInput()
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Command", GUILayout.ExpandWidth(false)))
		{
			if (command.Trim() != string.Empty)
			{
				string sRet = ConsoleCommands.ExecuteCommand(command);
				if (sRet != string.Empty)
					GMLogger.current.Debug(sRet);
			}

			command = "";
		}


		GUI.SetNextControlName("editor_cmd_input");
		command = EditorGUILayout.TextField(command, GUILayout.ExpandWidth(true));

		if (Event.current.type == EventType.KeyUp &&
			Event.current.isKey &&
			GUI.GetNameOfFocusedControl() == "editor_cmd_input")
		{
			if (Event.current.keyCode == KeyCode.Return)
			{
				if (command.Trim() != string.Empty)
				{
					string sRet = ConsoleCommands.ExecuteCommand(command);
					if (sRet != string.Empty)
						GMLogger.current.Debug(sRet);
				}

				command = "";
			}
		}

		
		GUILayout.EndHorizontal();
	}

    void OnDisable()
    {
        Console.ClearAll();
    }

	public static void OpenComponentInVisualStudioIDE(MonoBehaviour component, int gotoLine)
	{

		string[] fileNames = Directory.GetFiles(Application.dataPath, component.GetType().ToString() + ".cs", SearchOption.AllDirectories);
		if (fileNames.Length > 0)
		{
			string finalFileName = Path.GetFullPath(fileNames[0]);
			//Debug.Log("File Found:" + fileNames[0] + ". Converting forward slashes: to backslashes" + finalFileName );

			System.Diagnostics.Process.Start("devenv", " /edit \"" + finalFileName + "\" /command \"edit.goto " + gotoLine.ToString() + " \" ");

		}
		else
		{
			Debug.Log("File Not Found:" + component.GetType().ToString() + ".cs");
		}
	}

	public void OpenComponentInVisualStudioIDE(string name, int gotoLine)
	{

		string[] fileNames = Directory.GetFiles(Application.dataPath, name + ".cs", SearchOption.AllDirectories);
		if (fileNames.Length > 0)
		{
			string finalFileName = Path.GetFullPath(fileNames[0]);
			//Debug.Log("File Found:" + fileNames[0] + ". Converting forward slashes: to backslashes" + finalFileName );

			System.Diagnostics.Process.Start("devenv", " /edit \"" + finalFileName + "\" /command \"edit.goto " + gotoLine.ToString() + " \" ");

		}
		else
		{
			//Debug.Log("File Not Found:" + component.GetType().ToString() + ".cs");
		}
	}

	public void OpenScriptFileInVisualStudioIDE(string scriptPath, int gotoLine)
	{
		/*string dir = Application.dataPath.Replace("Assets", "");
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo.FileName = dir + "../tools/bin/gotofileline.exe";
		process.StartInfo.Arguments = string.Format("\"{0}\" {1}", dir + scriptPath, gotoLine);

		//Debug.LogWarning(process.StartInfo.Arguments);

		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;

		// This ensures that you get the output from the DOS application
		process.StartInfo.RedirectStandardOutput = true;

		process.Start();
		*/

		UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, gotoLine);
	}
}
