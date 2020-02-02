using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace OneP.GMConsole
{
	public class LogInfo {
		public string logString;
		public string logTrace;
		public UnityEngine.LogType logType;

		public LogInfo (string logString, string logTrace, UnityEngine.LogType logType) {
			this.logString = logString;
			this.logTrace = logTrace;
			this.logType = logType;
		}
	}

	public enum LogInfoFilter
	{
		VERBOSE	 =0,
		INFO	 =1,
		WARNING	 =2,
		ERROR	 =3,
		EXCEPTION=4
	}
	public class GMCommandMobileConsole : MonoBehaviour
	{
		public bool render = false;// render GM tool or not
        public GameObject objUIBackground;
		#region FPS Window
		public Rect startRect = new Rect(80, 40, 75, 50); 
		public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
		public GUISkin skin;

		private float frequency = 0.5F; // The update frequency of the fps
		private float accum = 0f;
		private int frames = 0;
		private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
		private GUIStyle style;
		private string sFPS = "";
        #endregion

       

		#region GM window
		public int limitLog=20; // limit show log in each page
		private int indexPage=0; //index page render in screen
		private bool isshowGM=false;// check flat show GM or not
		//private bool isShowMinimal=false;// just show minimal window

		private bool isShowLogOption=false;// show option filter log
		private bool isShowLogFull=false;// show full log trace or not
		private string gmCommandText="help";// gm input text

		private Vector2 scrollPosition = Vector2.zero;// index scroll text
		private LogInfoFilter logInfo=LogInfoFilter.VERBOSE; // log info filter
		private static OneP.GMConsole.GMCommand cmdInstance=null; // GM Command
		private Dictionary<LogInfoFilter,List<LogInfo>> dicLogs=new Dictionary<LogInfoFilter, List<LogInfo>>(){
			{LogInfoFilter.VERBOSE,new List<LogInfo>()},
			{LogInfoFilter.INFO,new List<LogInfo>()},
			{LogInfoFilter.WARNING,new List<LogInfo>()},
			{LogInfoFilter.ERROR,new List<LogInfo>()},
			{LogInfoFilter.EXCEPTION,new List<LogInfo>()}
		};


		#endregion

		void Start()
		{
			StartCoroutine(FPS());
		}
		void Awake(){
			Initial ();
		}

        public bool IsshowGM
        {
            get
            {
                return isshowGM;
            }
            set
            {
                isshowGM = value;
                if (objUIBackground != null)
                {
                    objUIBackground.SetActive(isshowGM);
                }
            }
        }
        #region Initial
        public void Initial()
		{
#pragma warning disable 0618
            Application.RegisterLogCallback (OnLogCallback);
			//Debug.LogError ("Initial");
			GameObject.DontDestroyOnLoad (this.gameObject);
			// Initial GM Console
			if (cmdInstance == null) {
				cmdInstance = new GMCommand ();
				ConsoleCommands.AddCommandProvider (cmdInstance, null);
			}
#pragma warning restore 0618
        }
        #endregion

        #region Update FPS
        void Update()
		{
			accum += Time.timeScale / Time.deltaTime;
			++frames;
		}

		IEnumerator FPS()
		{
			// Infinite loop executed every "frenquency" secondes.
			while (true)
			{
				// Update the FPS
				float fps = accum / frames;
				sFPS = fps.ToString("f" + Mathf.Clamp(1, 0, 10));

				//Update the color
				color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);

				accum = 0.0F;
				frames = 0;

				yield return new WaitForSeconds(frequency);
			}
		}
		#endregion

		#region ONGUI
		void OnGUI()
		{
	        if (!render)
	            return;
	     	if (style == null)
			{
				style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = Color.white;
				style.alignment = TextAnchor.MiddleCenter;
			}

			if (!IsshowGM) {
				GUI.color = color;
				startRect = GUI.Window (0, startRect, ShowFPSWindow, "");
			} else {
				GUI.color = Color.white;
				GUI.Window (0, new Rect(0,0,Screen.width,Screen.height), ShowGMWindow, "GM Command Tool");
			}
		}
		#endregion

		#region ONGUI GM Tool
		void ShowGMWindow(int windowID){
			
			if (!isShowLogOption) {
				#region Show GM
				GUILayout.BeginVertical ();
				{
					GUI.Box (new Rect (20, 25, Screen.width - 40, 95), "");
					GUILayout.BeginArea (new Rect (30, 30, Screen.width - 60, 25));
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Show Log Type");
					if (GUILayout.Button (logInfo.ToString())) {
						isShowLogOption = true;
					}
					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();
					
					GUILayout.BeginArea (new Rect (30, 40, Screen.width - 60, 20));
					GUILayout.Label ("__________________________________________________________________________________________________________________________________________________________________________________________");
					GUILayout.EndArea ();

					GUI.Box (new Rect (30, 70, Screen.width - 60, 20), "");
					GUILayout.BeginArea (new Rect (30, 70, Screen.width - 60, 20));
				
					gmCommandText = GUILayout.TextArea (gmCommandText);
					GUILayout.EndArea ();
					GUILayout.BeginArea (new Rect (70, 95, Screen.width - 140, 25));
					if (GUILayout.Button ("Excecute Command")) {
						Command();
					}
					GUILayout.EndArea ();
					GUI.Box (new Rect (20, 125, Screen.width - 40, Screen.height - 200), "");
					GUILayout.BeginArea (new Rect (20, 125, Screen.width - 40, Screen.height - 200));
					{
						scrollPosition = GUILayout.BeginScrollView (scrollPosition);
						GUI.skin.textArea.normal.background = null;
						GUI.skin.textArea.active.background = null;
						GUI.skin.textArea.hover.background = null;
						GUI.color = Color.red;
						FillToScreen();
						//GUILayout.TextArea ("ahdjkhaskjdhakjshdkjashdkjahskdjhaskjdhaskjdhkjashdkjashdjhasjkdhakjshdkjaghfjahsdkjhk");
						GUILayout.EndScrollView ();
					}
					GUILayout.EndArea ();

					GUI.Box (new Rect (20, Screen.height - 70, Screen.width - 40, 55), "");
					GUILayout.BeginArea (new Rect (40, Screen.height - 60, Screen.width - 80, 55));
					{
						GUILayout.BeginHorizontal ();
						GUI.color = Color.white;
						if (GUILayout.Button ("Clear Logs")) {
							for(int i=0;i<5;i++)
							{
								dicLogs[(LogInfoFilter)i]=new List<LogInfo>();
							}
						}
						if(!isShowLogFull)
						{
							if (GUILayout.Button ("Show Logs Full  ")) {
								isShowLogFull=true;
							}
						}
						else
						{
							if (GUILayout.Button ("Show Logs Normal")) {
								isShowLogFull=false;
							}
						}
						if (GUILayout.Button ("Close GM")) {
                            IsshowGM = false;
						}
						GUILayout.EndHorizontal ();
					}
					GUILayout.EndArea ();

					GUI.Box (new Rect (20, 125, Screen.width - 40, Screen.height - 200), "");

				}
				GUILayout.EndVertical ();
				#endregion
			} else {
				ShowOptionLog ();
			}
		}

		void ShowOptionLog(){
			GUI.Box (new Rect (80, 100, Screen.width - 160, Screen.height - 200),"");
			GUILayout.BeginArea(new Rect(100,100,Screen.width-200,Screen.height-200));
			GUILayout.BeginVertical ();
			GUILayout.Label ("Show Log option:");
			if (GUILayout.Button ("Verbose")) {
				OnLogFilterChange (LogInfoFilter.VERBOSE);
			}
			if(GUILayout.Button ("Info")){
				OnLogFilterChange (LogInfoFilter.INFO);
			}
			if (GUILayout.Button ("Warning")) {
				OnLogFilterChange (LogInfoFilter.WARNING);
			}
			if (GUILayout.Button ("Error")) {
				OnLogFilterChange (LogInfoFilter.ERROR);
			}
			if (GUILayout.Button ("Exception")) {
				OnLogFilterChange (LogInfoFilter.EXCEPTION);
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea();

		}
		#endregion 

		#region ONGUI FPS
		void ShowFPSWindow(int windowID)
		{
			string content = " "+sFPS + " FPS\n";
			GUI.Label(new Rect(0, 0, startRect.width, startRect.height), content);
			if (GUI.Button (new Rect (2, startRect.height - 22, startRect.width - 4, 20), "Open GM")) {
                IsshowGM = true;
			}

			if (allowDrag) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
		}
		#endregion

		public void Command()
		{
			if (gmCommandText.Length < 1) {
				Debug.Log("Input command are empty");
			}
            if (gmCommandText.ToLower().Equals("close")) {
                this.enabled = false;
            }
            else
            {
                string command = gmCommandText;
                string ret = ConsoleCommands.ExecuteCommand(command);
                Debug.LogWarning(ret);
            }

		}
		public void OnLogCallback (string logString, string stackTrace, UnityEngine.LogType type) {
			if (type == UnityEngine.LogType.Log || type == UnityEngine.LogType.Assert) {
				dicLogs [LogInfoFilter.INFO].Add (new LogInfo (logString, stackTrace, type));
			}
			else if (type == UnityEngine.LogType.Warning) {
				dicLogs [LogInfoFilter.WARNING].Add (new LogInfo (logString, stackTrace, type));
			}
			else if (type == UnityEngine.LogType.Error) {
				dicLogs [LogInfoFilter.ERROR].Add (new LogInfo (logString, stackTrace, type));
			}
			else if (type == UnityEngine.LogType.Exception) {
				dicLogs [LogInfoFilter.EXCEPTION].Add (new LogInfo (logString, stackTrace, type));
			}
			dicLogs[LogInfoFilter.VERBOSE].Add (new LogInfo (logString, stackTrace, type));
		}
		public void OnLogCallbackNotWrite (string logString, string stackTrace, UnityEngine.LogType type) {
			//todo
		}
		public void OnLogFilterChange(LogInfoFilter value)
		{
			if (logInfo != value) {
				logInfo = value;
				indexPage = 0;
				isShowLogOption = false;
				scrollPosition = new Vector2 (0, 0);
			}
		}

		public void FillToScreen()
		{
			//float cacheHeight = 0;
			List<LogInfo> listLog=null;
			dicLogs.TryGetValue (logInfo, out listLog);
			if (listLog == null)
				return;
			int count=listLog.Count-indexPage*limitLog;
			int limit = listLog.Count - indexPage * limitLog - limitLog;
			bool isPrevious = false;
			bool isNext = false;
			if (indexPage > 0) {
				isPrevious=true;
			}
			if (limit > 0) {
				isNext = true;
			}

			for (int i = count-1;i>limit&&i > 0; --i) {
				if (i >= 0) {
					LogInfo info = listLog[i];
					string log = "";
					if (info.logType == UnityEngine.LogType.Error) {
						GUI.color = Color.red;
					}
					else if (info.logType == UnityEngine.LogType.Exception) {
						GUI.color = Color.magenta;
					}
					else if (info.logType == UnityEngine.LogType.Warning) {
						GUI.color = Color.yellow;
					}
					else {
						GUI.color = Color.white;
					}
					log += info.logString + "\n";
					if (log.Length> 1000) {
						log = log.Substring (log.Length - 1000, 999);
						log += "\n... (limit show)";
					}
					if (string.IsNullOrEmpty (info.logTrace) == false) {
						if (isShowLogFull) {
							log += "<= " + info.logTrace;
						}
					}

					log+= "\n-------------------------";
					GUILayout.TextArea (log);
				}
			}
			GUI.color = Color.white;
			GUILayout.BeginHorizontal ();
			if (isPrevious) {
				if (GUILayout.Button ("Previous Page")) {
					indexPage--;
					if (indexPage < 0) {
						indexPage = 0;
						scrollPosition = new Vector2 (0, 0);
					}
				}
			}
			if (isNext) {
				if (GUILayout.Button ("Next Page")) {
					indexPage++;
					int maxPage = 0;
					if (limitLog > 0) {
						maxPage = listLog.Count / limitLog;
					}
					if (indexPage >maxPage) {
						indexPage = maxPage;
						scrollPosition = new Vector2 (0, 0);
					}
				}
			}
			GUILayout.EndHorizontal ();
		}
	}
}