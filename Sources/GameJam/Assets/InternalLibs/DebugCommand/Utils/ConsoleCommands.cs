/// <summary>
/// OneP GM Console version 0.6
/// Modified by Strong D
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace OneP.GMConsole
{
	/// <summary>
	/// 
	/// </summary>
	class ConsoleCommandException : Exception
    {
        public ConsoleCommandException(string Message) : base(Message)
        {
        }
    }

	/// <summary>
	/// 
	/// </summary>
	class ConsoleCommandWrapper
    {
        public string command = string.Empty;

        public ConsoleCommandWrapper(string c)
        {
            this.command = c;
        }
    }

	/// <summary>
	/// Command detail description
	/// </summary>
	public class CmdDetailAttribute : Attribute
	{
		public CmdDetailAttribute(string detail)
		{
			this.Detail = detail;
		}

		public string Detail { get; private set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public struct CmdMethodInfo
	{
		public object instance;
		public MethodInfo methodInfo;

		public CmdMethodInfo(object instance, MethodInfo methodInfo)
		{
			this.instance = instance;
			this.methodInfo = methodInfo;
		}

		public string declaration
		{
			get
			{
				var builder = new StringBuilder();
				ParameterInfo[] parameters = methodInfo.GetParameters();
				builder.Append(methodInfo.Name);
				if (parameters.Length == 0)
				{
					builder.Append("()");
				}
				else
				{
					builder.Append(" (");
					bool flag = true;
					foreach (ParameterInfo info in parameters)
					{
						if (!flag)
						{
							builder.Append(", ");
						}
						builder.Append(info.ParameterType.Name + " " + info.Name);
						flag = false;
					}
					builder.Append(")");
				}

				return builder.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string briefDescription
		{
			get
			{
				var builder = new StringBuilder();

				DescriptionAttribute[] customAttributes = (DescriptionAttribute[])methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (customAttributes.Length > 0)
				{
					builder.Append(customAttributes[0].Description);
				}

				return builder.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string detailDescription
		{
			get
			{
				var builder = new StringBuilder();

				CmdDetailAttribute[] detailAttributes = (CmdDetailAttribute[])methodInfo.GetCustomAttributes(typeof(CmdDetailAttribute), false);
				if (detailAttributes.Length > 0)
				{
					builder.Append(detailAttributes[0].Detail);
				}

				return builder.ToString();
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ConsoleCommands
	{
		/// <summary>
		/// Console methods
		/// </summary>
		public static Dictionary<string, List<CmdMethodInfo>> consoleMethods { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleCommands"/> class.
		/// </summary>
		static ConsoleCommands()
		{
			consoleMethods = new Dictionary<string, List<CmdMethodInfo>>();
		}


		/// <summary>
		/// Adds the command provider.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="type">The type.</param>
		public static void AddCommandProvider(object instance, Type type)
		{
			if (instance == null && type == null)
				return;

			BindingFlags bindFlags;
			if (instance != null)
			{
				type = instance.GetType();
				bindFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
			}
			else
			{
				bindFlags = BindingFlags.Public | BindingFlags.Static;
			}

			List<string> list = new List<string>();
			foreach (MethodInfo info in type.GetMethods(bindFlags))
			{
				if ((((info.Name != "ToString") && (info.Name != "Equals")) && (info.Name != "GetHashCode")) && (info.Name != "GetType"))
				{
					foreach (ParameterInfo info2 in info.GetParameters())
					{
						if (!ConsoleTypeConversion.CanConvertTo(info2.ParameterType))
						{
							list.Add(info.Name);
						}
					}
					List<CmdMethodInfo> list2 = null;
					if (!consoleMethods.TryGetValue(info.Name.ToLower(), out list2))
					{
						consoleMethods.Add(info.Name.ToLower(), new List<CmdMethodInfo>());
					}
					consoleMethods[info.Name.ToLower()].Add(new CmdMethodInfo(instance, info));
				}
			}
			if (list.Count > 0)
			{
				string text = "WARNING!\n\nThe following console methods contain parameters with no supported conversion:\n\n";
				foreach (string str2 in list)
				{
					text = text + str2 + "\n";
				}
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
				if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer))
				{
					MessageBox(IntPtr.Zero, text, "Warning", 0);
				}
#endif
			}
		}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		[DllImport("user32.dll")]
		private static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);
#endif

		/// <summary>
		/// Gets all commands info.
		/// </summary>
		/// <returns></returns>
		public static List<CmdMethodInfo>[] GetAllCommands()
		{
			//StringBuilder builder = new StringBuilder();
			List<CmdMethodInfo>[] array = new List<CmdMethodInfo>[consoleMethods.Count];
			consoleMethods.Values.CopyTo(array, 0);

			Array.Sort<List<CmdMethodInfo>>(array, (l, r) =>
				{
					if ((l.Count > 0) && (r.Count > 0))
						return string.Compare(l[0].methodInfo.Name, r[0].methodInfo.Name);
					return 0;
				});

			return array;
		}

		/// <summary>
		/// Gets the command.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <returns></returns>
		public static List<CmdMethodInfo> GetCommand(string cmd)
		{
			List<CmdMethodInfo> list = null;
			consoleMethods.TryGetValue(cmd.ToLower(), out list);
			return list;
		}

		/// <summary>
		/// Gets help for all commands
		/// </summary>
		/// <returns></returns>
		public static string GetAllCommandHelps()
		{
			List<CmdMethodInfo>[] array = GetAllCommands();

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("-----------------------");

			bool isFirst = true;
			foreach (List<CmdMethodInfo> list in array)
			{
				foreach (CmdMethodInfo cmdInfo in list)
				{
					if (!isFirst)
						builder.AppendLine(string.Empty);
					else
						isFirst = false;

					builder.Append("> ");
					builder.AppendLine(cmdInfo.declaration);

					builder.Append("> ");
					builder.AppendLine(cmdInfo.briefDescription);
				}
			}

			builder.AppendLine("----------------------");
			return builder.ToString();
		}

		/// <summary>
		/// Gets help for single command
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <returns></returns>
		public static string GetCommandHelp(string cmd)
		{
			List<CmdMethodInfo> list = GetCommand(cmd);
			if( list == null )
				return "Command \"" + cmd + "\" not found.";

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("-----------------------------------------------------------------------------------------");

			bool isFirst = true;
			foreach (CmdMethodInfo cmdInfo in list)
			{
				if (!isFirst)
					builder.AppendLine(string.Empty);
				else
					isFirst = false;

				builder.Append("> ");
				builder.AppendLine(cmdInfo.declaration);

				builder.Append("> ");
				builder.AppendLine(cmdInfo.briefDescription);

				string detail = cmdInfo.detailDescription;
				if (detail.Length > 0)
				{
					builder.Append("> ");
					builder.AppendLine(cmdInfo.detailDescription);
				}
			}
			builder.AppendLine("-----------------------------------------------------------------------------------------");
			return builder.ToString();
		}


		/// <summary>
		/// Executes the specified command.
		/// </summary>
		/// <param name="Command">The command.</param>
		/// <returns></returns>
		private static string Execute(string command, object instance)
		{
			string str = string.Empty;
			object[] cmdParams = SplitParams(command);
			if (cmdParams.Length == 0)
			{
				return string.Empty;
			}
			for (int i = 0; i < cmdParams.Length; i++)
			{
				if (cmdParams[i] is ConsoleCommandWrapper)
				{
					cmdParams[i] = Execute(((ConsoleCommandWrapper)cmdParams[i]).command, instance);
				}
			}
			if (!(cmdParams[0] is string))
			{
				throw new ConsoleCommandException("Unknown command \"" + cmdParams[0].ToString() + "\"");
			}
			string key = ((string)cmdParams[0]).ToLower();
			/*if ((key == "help") || (key == "?"))
			{
				if (cmdParams.Length == 1)
					return this.buildHelpString();

				if (cmdParams.Length == 2)
					return this.buildCommandHelpString((string)cmdParams[1]);
			}*/
			IList list = null;
			List<CmdMethodInfo> list2 = null;
			if (consoleMethods.TryGetValue(key, out list2))
			{
				list = consoleMethods[key];
			}
			else
			{
				char[] separator = new char[] { '.' };
				string[] strArray = key.Split(separator);
				if (strArray.Length == 2)
				{
					list = FindStaticMethods(strArray[0], strArray[1]);
				}
			}
			if ((list == null) || (list.Count <= 0))
			{
				throw new ConsoleCommandException("Unknown command \"" + cmdParams[0] + "\"");
			}
			object[] parameters = null;
			foreach(CmdMethodInfo info in list)
			{			
				if (ParametersMatch(info.methodInfo.GetParameters(), cmdParams, out parameters))
				{
					return ExecuteCmd(info, instance, parameters);
				}
			}
			
			if ((cmdParams.Length > 1) && (cmdParams[1] is object[]))
			{
				object[] destinationArray = new object[((object[])cmdParams[1]).Length + 1];
				destinationArray[0] = cmdParams[0];
				Array.Copy((object[])cmdParams[1], 0, destinationArray, 1, ((object[])cmdParams[1]).Length);
				foreach(CmdMethodInfo info in list)
				{
					if (ParametersMatch(info.methodInfo.GetParameters(), destinationArray, out parameters))
					{
						return ExecuteCmd(info, instance, parameters);
					}
				}
				
			}
			str = "Could not find matching parameter list for command \"" + cmdParams[0] + "\" with parameters [";
			for (int j = 1; j < cmdParams.Length; j++)
			{
				if (j > 1)
				{
					str = str + " ";
				}
				str = str + cmdParams[j].ToString();
			}
			throw new ConsoleCommandException(str + "].");
		}

		/// <summary>
		/// Executes the CMD.
		/// </summary>
		/// <param name="mi">The mi.</param>
		/// <param name="Parameters">The parameters.</param>
		/// <returns></returns>
		private static string ExecuteCmd(CmdMethodInfo mi, object instance, object[] parameters)
		{
			object obj2 = mi.methodInfo.Invoke(instance != null ? instance : mi.instance, parameters);
			return ((obj2 != null) ? obj2.ToString() : string.Empty);
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="Cmd">The CMD.</param>
		/// <returns></returns>
		public static string ExecuteCommand(string cmd, object instance)
		{
			try
			{
				return Execute(cmd, instance);
			}
			catch (ConsoleCommandException exception)
			{
				return ("ERROR: " + exception.Message);
			}
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <returns></returns>
		public static string ExecuteCommand(string cmd)
		{
            UnityEngine.Debug.Log("Execute:" + cmd);
			return ExecuteCommand(cmd, null);
		}

		/// <summary>
		/// Finds the static methods.
		/// </summary>
		/// <param name="ClassName">Name of the class.</param>
		/// <param name="MethodName">Name of the method.</param>
		/// <returns></returns>
		private static MethodInfo[] FindStaticMethods(string className, string methodName)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (System.Type type in assembly.GetTypes())
				{
					if (string.Compare(type.Name, className, true) == 0)
					{
						foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
						{
							if (string.Compare(info.Name, methodName, true) == 0)
							{
								list.Add(info);
							}
						}
						break;
					}
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// Parameterses the match.
		/// </summary>
		/// <param name="pi">The pi.</param>
		/// <param name="CmdParams">The CMD params.</param>
		/// <param name="Parameters">The parameters.</param>
		/// <returns></returns>
		private static bool ParametersMatch(ParameterInfo[] pi, object[] cmdParams, out object[] parameters)
		{
			parameters = null;
			List<object> list = new List<object>();
			int cmdParamIndex = 1;
			int index = 0;
			index = 0;
			while (index < pi.Length)
			{
				object param = null;
				if (!ParseParameterType(pi[index].ParameterType, cmdParams, ref cmdParamIndex, out param))
				{
					break;
				}
				list.Add(param);
				index++;
			}
			if (index < pi.Length)
			{
				return false;
			}
			if (cmdParamIndex < cmdParams.Length)
			{
				return false;
			}
			parameters = list.ToArray();
			return true;
		}

		/// <summary>
		/// Parses the command parameter.
		/// </summary>
		/// <param name="Params">The params.</param>
		/// <returns></returns>
		private static object ParseCommandParameter(ref string parms)
		{
			parms = parms.Trim();
			if (parms[0] == '"')
			{
				int num = parms.IndexOf('"', 1);
				while ((num > 0) && (parms[num - 1] == '\\'))
				{
					num = parms.IndexOf('"', num + 1);
				}
				if (num > 0)
				{
					string str = parms.Substring(1, num - 1);
					parms = parms.Substring(num + 1).Trim();
					return str.Replace("\\\"", "\"");
				}
			}
			if ((parms[0] == '[') || (parms[0] == '{'))
			{
				char ch = '[';
				char ch2 = ']';
				if (parms[0] == '{')
				{
					ch = '{';
					ch2 = '}';
				}
				int num2 = 1;
				int startIndex = 1;
				while ((num2 > 0) && (startIndex < parms.Length))
				{
					if (parms[startIndex] == ch2)
					{
						num2--;
					}
					else if (parms[startIndex] == ch)
					{
						num2++;
					}
					startIndex++;
				}
				if (num2 == 0)
				{
					object obj2 = new ConsoleCommandWrapper(parms.Substring(1, startIndex - 2).Trim());
					parms = parms.Substring(startIndex).Trim();
					return obj2;
				}
			}
			if (parms[0] == '(')
			{
				char ch3 = ')';
				int num4 = parms.IndexOf(ch3);
				if (num4 > 0)
				{
					string paramString = parms.Substring(1, num4 - 1);
					parms = parms.Substring(num4 + 1).Trim();
					return SplitParams(paramString);
				}
			}
			string str3 = parms;
			int index = parms.IndexOf(' ');
			if (index > 0)
			{
				str3 = parms.Substring(0, index);
				parms = parms.Substring(index + 1).Trim();
				return str3;
			}
			parms = string.Empty;
			return str3;
		}

		/// <summary>
		/// Parses the type of the parameter.
		/// </summary>
		/// <param name="ParamType">Type of the param.</param>
		/// <param name="CmdParams">The CMD params.</param>
		/// <param name="CmdParamIndex">Index of the CMD param.</param>
		/// <param name="Param">The param.</param>
		/// <returns></returns>
		private static bool ParseParameterType(System.Type paramType, object[] cmdParams, ref int cmdParamIndex, out object param)
		{
			//int num = CmdParamIndex;
			bool flag = ConsoleTypeConversion.ConvertToType(cmdParams, ref cmdParamIndex, paramType, out param);
			if (!flag)
			{
			}
			return flag;
		}

		/// <summary>
		/// Splits the params.
		/// </summary>
		/// <param name="ParamString">The param string.</param>
		/// <returns></returns>
		private static object[] SplitParams(string paramString)
		{
			paramString = paramString.Trim();
			List<object> list = new List<object>();
			while (paramString.Length > 0)
			{
				list.Add(ParseCommandParameter(ref paramString));
			}
			return list.ToArray();
		}
	}
}

