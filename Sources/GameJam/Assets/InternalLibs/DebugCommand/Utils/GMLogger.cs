/// <summary>
/// OneP GM Console version 0.6
/// Strong D
/// </summary>
/// ƒ
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WII || UNITY_PS3 || UNITY_XBOX360
#define UNITY // running inside the unity runtime environment?
#endif

#if UNITY
#define LOG_UNITY // Debug.Log* active?
#define LOG_INGAME_CONSOLE // Log into ingame console
#endif


#if UNITY
#define LOG_EDITOR_CONSOLE
using UnityEngine; // you can disable this - recommended when using outside of Unity!
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OneP.GMConsole
{
	/// <summary>
	/// Log level
	/// </summary>
	public enum LogType
	{
		Debug = 0,
		Info,
		Warn,
		Error,
		Fatal,
	}

	[Flags]
	public enum LogFormat
	{
		Sender = 0x01,
		DateTime = 0x02,
		Type = 0x04,
	}

	/// <summary>
	/// Log provider interface
	/// </summary>
	public interface ILogProvider
	{
		// Log the actual message
		void Log(LogType type, object sender, string group, string message, string meta, string callStack );
	}

#if LOG_UNITY
	/// <summary>
	/// UnityDebug provider
	/// </summary>
	public class UnityLogProvider : ILogProvider
	{
		private Action<object>[] logDelegate;

		public UnityLogProvider()
		{
			logDelegate = new Action<object>[] { 
				UnityEngine.Debug.Log,
				UnityEngine.Debug.Log,
				UnityEngine.Debug.LogWarning,
				UnityEngine.Debug.LogError,
				UnityEngine.Debug.LogError };
		}

		public void Log(LogType type, object sender, string group, string message, string meta, string callStack)
		{
			if( group.Length > 0 )
				logDelegate[(int)type](string.Format("{0}:{1}\n{2}", group, message, meta));
			else
				logDelegate[(int)type](string.Format("{0}\n{1}", message, meta));
		}
	}
#endif

#if LOG_EDITOR_CONSOLE
	/// <summary>
	/// UnityDebug provider
	/// </summary>
	public class AntaresLogProvider : ILogProvider
	{
		public AntaresLogProvider()
		{
		}

		public void Log(LogType type, object sender, string group, string message, string meta, string callStack)
		{
			Color color = Color.white;
			switch( type )
			{
				case LogType.Debug:
					color = Color.white;
					break;

				case LogType.Warn:
					color = Color.yellow;
					break;

				case LogType.Error:  
				case LogType.Fatal:
					color = Color.red;
					break;

			}

			//UnityEngine.Debug.LogWarning(result.Length == 0 ? fullMessage : result.ToString());
			string fullMessage = message;

			if (!string.IsNullOrEmpty(callStack))
				fullMessage += "\n" + callStack;

			if (group.Length > 0)
				OnePStudio.Console.Console.Log(sender, group, fullMessage, color);
			else
				OnePStudio.Console.Console.Log(sender, string.Empty, fullMessage, color);
		}
	}
	
#endif

	//
	public class GMLogger
	{
		#region Class methods
		// Pattern regex for searching callstack line
		private const string _callstackPattern =
			@"(\s\s(at)\s)?((?<class>(\w|\d|\.)+):)?" +
			@"(?<func>(\w|\d|\.)+)\s?" +
			@"(?<param>\([\w\d&,\.\s\[\]]*\))?\s" +
			@"(?<adress>\[0x(\d|\w)+\]\s)?" +
			@"\(?(at|in)\s(?<file>(\w|\d|/|\.|\\|:)+):(?<line>\d+)\)?" +
			@"\s?\r?\n?";
		private static readonly Regex callStackRegex = new Regex(_callstackPattern, RegexOptions.Multiline);

		// Active log providers
		private static List<ILogProvider> logProviders = new List<ILogProvider>();

		// Default logger, each thread have difference default logger (to prevent synchronize)
		[ThreadStatic]
		private static GMLogger defaultLogger;

		static GMLogger()
		{
			debugEnabled = true;
			infoEnabled = true;
			warnEnabled = true;
			errorEnabled = true;
			fatalEnabled = true;

			stackTrace = false;

			logFormat = LogFormat.Sender | LogFormat.DateTime | LogFormat.Type;
			dateTimeFormat = "dd/MM/yy hh:mm";
			
		}

		/// <summary>
		/// Return current class logger
		/// </summary>
		public static GMLogger GetLogger(System.Type type)
		{
			return new GMLogger(type);
		}

		public static GMLogger current
		{
			get
			{
				if (defaultLogger == null)
					defaultLogger = new GMLogger();
				return defaultLogger;
			}
		}

		// Date time format
		public static string dateTimeFormat { set; get; }

		// Log format
		public static LogFormat logFormat { set; get; }

		// Stack trace
		public static bool stackTrace { set; get; }

		// Log type enabled
		public static bool debugEnabled { set; get; }
		public static bool infoEnabled { set; get; }
		public static bool warnEnabled { set; get; }
		public static bool errorEnabled { set; get; }
		public static bool fatalEnabled { set; get; }

		// Strign builder
		private static StringBuilder builder = new StringBuilder();

		// Filters
		public static List<string> includeFilters;
		public static List<string> excludeFilters;

		private static void Log(LogType type, object sender, object group, object message, params object[] args)
		{
			if (logProviders.Count == 0)
				return;

			lock (builder)
			{
				builder.Remove(0, builder.Length);

				// Sender infomation
				if ((logFormat & LogFormat.Sender) == LogFormat.Sender)
				{
					if (sender == null)
					{
						// Use callstack to get class name
						StackTrace _stackTrace = new StackTrace();
						StackFrame[] stackFrames = _stackTrace.GetFrames();

						if (stackFrames.Length > 2)
							sender = stackFrames[2].GetMethod().ReflectedType.Name;
						else
							sender = string.Empty;
					}

					builder.AppendFormat("[{0}] ", sender);
					sender = builder.ToString();
				}

				if ((logFormat & LogFormat.Type) == LogFormat.Type)
					builder.AppendFormat("{0}, ", type.ToString().ToUpper());

				if ((logFormat & LogFormat.DateTime) == LogFormat.DateTime)
					builder.AppendFormat("{0} ", DateTime.Now.ToString(dateTimeFormat));

				// Meta info
				string meta = builder.ToString();

				// Custom message
				message = string.Format(message.ToString(), args);
				//string fullMsg = builder.ToString(); 

				/*int i;
				bool isShow = true;
				// Apply filters
				if( excludeFilters != null && excludeFilters.Count > 0 )
				{
					isShow = true;
					for (i = 0; i < excludeFilters.Count && isShow; i++)
						if (fullMsg.Contains(excludeFilters[i]))
							isShow = false;
				}

				if( includeFilters != null && includeFilters.Count > 0 )
				{
					isShow = false;
					for (i = 0; i < includeFilters.Count && !isShow; i++)
						if (fullMsg.Contains(includeFilters[i]))
							isShow = true;
				}

				if (!isShow)
					return;*/

				// Build callstack
				builder.Remove(0, builder.Length);
				string callstack = string.Empty;
				if (stackTrace)
				{
					StackTrace _stackTrace = new StackTrace(true);
					StackFrame[] stackFrames = _stackTrace.GetFrames();

					if (stackFrames.Length > 2)
					{
						//builder.AppendLine();
						for (int i = 2; i < stackFrames.Length; i++)
						{
							StackFrame currFrame = stackFrames[i];
							if (currFrame == null)
								continue;

							string filename = currFrame.GetFileName();
							if (string.IsNullOrEmpty(filename))
							{
								builder.AppendLine(string.Format("${0}:{1}",
									currFrame.GetMethod().ReflectedType.Name,
									currFrame.GetMethod().Name));
							}
							else
							{
								filename = filename.Replace('\\', '/');
								int pos = filename.IndexOf("Assets/");
								if (pos >= 0)
									filename = filename.Substring(pos);

								builder.AppendLine(string.Format("$@{0}:{1} (at {2}:{3})",
									currFrame.GetMethod().ReflectedType.Name,
									currFrame.GetMethod().Name,
									filename, currFrame.GetFileLineNumber()));
							}

						}
					}

					callstack = builder.ToString();
				}

				// Group
				if (group == null)
					group = string.Empty;

				for (int i = 0; i < logProviders.Count; i++)
					logProviders[i].Log(type, sender, group.ToString(), message.ToString(), meta, callstack);
			}
		}

		/// <summary>
		/// Pre format message (with callstack)
		/// Prefix each callstack item with $@ where 
		/// - $: beginning of a line (may be a callstack without fileline or a none callstack)
		/// - @: this callstack line has file and line infomation
		/// </summary>
		public static string FormatCallstack(string callStack)
		{
			// Preformat message, extract callstack infomation
			int begin = 0;
			StringBuilder builder = new StringBuilder();
			MatchCollection matches = callStackRegex.Matches(callStack);
			if (matches.Count == 0)
				return callStack;

			for( int i = 0; i < matches.Count; i++ )
			{
				Match match = matches[i];

				// Between the match may be a unfull callstack or a literal string
				if( match.Index > begin )
					builder.AppendFormat("${0}", callStack.Substring(begin, match.Index - begin));
				
				// This is a full info callstack line
				builder.AppendFormat("$@{0}", callStack.Substring(match.Index, match.Length));
				begin = match.Index + match.Length;
			}

			return builder.ToString();
		}

		/// <summary>
		/// Break compose message with callstack (option come from an exception)
		/// The message format must be satisfied
		///		Message
		///			Callstack 1
		///			...
		///			Calstack n
		/// </summary>
		public static void BreakMessageAndFormatCallstack(string messageWithCallstack, out string message, out string callstack)
		{
			// Preformat message, extract callstack infomation
			int begin = 0;
			StringBuilder builder = new StringBuilder();
			MatchCollection matches = callStackRegex.Matches(messageWithCallstack);
			if (matches.Count == 0)
			{
				message = messageWithCallstack;
				callstack = string.Empty;
				return;
			}

			// Get message
			message = messageWithCallstack.Substring(begin, matches[0].Index - begin);
			begin = matches[0].Index;
			
			// Get callstack
			for (int i = 0; i < matches.Count; i++)
			{
				Match match = matches[i];

				// Callstack with no file and line
				if( match.Index > begin )
					builder.AppendFormat("${0}", messageWithCallstack.Substring(begin, match.Index - begin));
					
				// This is a full info callstack line
				builder.AppendFormat("$@{0}", messageWithCallstack.Substring(match.Index, match.Length));
				begin = match.Index + match.Length;
			}
			callstack = builder.ToString();
		}

		#endregion

		#region Instance methods
		// Current sender name
		private object sender = null;

		public GMLogger()
		{
		}

		public GMLogger(object sender) : base()
		{
			this.sender = sender;
		}

		public GMLogger(Type type) : base()
		{
			this.sender = type;
		}

		#region Log a message object
		public void Debug(object message)
		{
			if (debugEnabled)
			{
				Log(LogType.Debug, sender, null, message);
			}
		}

		public void Info(object message)
		{
			if (infoEnabled)
			{
				Log(LogType.Info, sender, null, message);
			}
		}

		public void Warn(object message)
		{
			if (warnEnabled)
			{
				Log(LogType.Warn, sender, null, message);
			}
		}

		public void Error(object message)
		{
			if (errorEnabled)
			{
				Log(LogType.Error, sender, null, message);
			}
		}

		public void Fatal(object message)
		{
			if (fatalEnabled)
			{
				Log(LogType.Fatal, sender, null, message);
			}
		}
		#endregion

		#region Log a source and a value
		public void Debug(object source, object value)
		{
			if (debugEnabled)
			{
				Log(LogType.Debug, sender, source, value);
			}
		}

		public void Info(object source, object value)
		{
			if (infoEnabled)
			{
				Log(LogType.Info, sender, source, value);
			}
		}
		#endregion

		#region Log a message string using the System.String.Format syntax
		public void DebugFormat(string format, params object[] args)
		{
			if (debugEnabled)
			{
				Log(LogType.Debug, sender, null, format, args);
			}
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (infoEnabled)
			{
				Log(LogType.Info, sender, null, format, args);
			}
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (warnEnabled)
			{
				Log(LogType.Warn, sender, null, format, args);
			}
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (errorEnabled)
			{
				Log(LogType.Error, sender, null, format, args);
			}
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (fatalEnabled)
			{
				Log(LogType.Fatal, sender, null, format, args);
			}
		}
		#endregion

		/* Log a message object and exception */
		public void Exception(object message, Exception t)
		{
			Log(LogType.Error, sender, null, "{0}\n{1}: {2}\n{3}", message, t.GetType().ToString(), t.Message, FormatCallstack(t.StackTrace));
		}


		/* Log a message string using the System.String.Format syntax */
		/*public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
#if LOG4NET
			log.DebugFormat(provider, format, args);
#endif
#if LOG_UNITY
			if (IsDebugEnabled)
			{
#if LOG_INGAME_CONSOLE
				GDebug.Log(string.Format(provider, format, args));
#endif
				UnityEngine.Debug.Log(string.Format(provider, format, args));
			}
#endif
		}


		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
#if LOG4NET
			log.InfoFormat(provider, format, args);
#endif
#if LOG_UNITY
			if (IsInfoEnabled)
			{
#if LOG_INGAME_CONSOLE
				GDebug.Log(string.Format(provider, format, args));
#endif
				UnityEngine.Debug.Log(string.Format(provider, format, args));
			}
#endif
		}


		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
#if LOG4NET
			log.WarnFormat(provider, format, args);
#endif
#if LOG_UNITY
			if (IsWarnEnabled)
			{
#if LOG_INGAME_CONSOLE
				GDebug.LogWarning(string.Format(provider, format, args));
#endif
				UnityEngine.Debug.LogWarning(string.Format(provider, format, args));
			}
#endif
		}


		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
#if LOG4NET
			log.ErrorFormat(provider, format, args);
#endif
#if LOG_UNITY
			if (IsErrorEnabled)
			{
#if LOG_INGAME_CONSOLE
				GDebug.LogError(string.Format(provider, format, args));
#endif
				UnityEngine.Debug.LogError(string.Format(provider, format, args));
			}
#endif
		}


		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
#if LOG4NET
			log.FatalFormat(provider, format, args);
#endif
#if LOG_UNITY
			if (IsFatalEnabled)
			{
#if LOG_INGAME_CONSOLE
				GDebug.LogError(string.Format(provider, format, args));
#endif
				UnityEngine.Debug.LogError(string.Format(provider, format, args));
			}
#endif
		}*/

		#endregion
	}
}
