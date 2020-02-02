/// <summary>
/// OneP GM Console version 0.6
/// Strong D
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OneP.GMConsole
{
	[Serializable]
	public class LogTypeConfig : IEquatable<LogTypeConfig>
	{
		public bool debugEnabled = true;
		public bool infoEnabled = true;
		public bool warnEnabled = true;
		public bool errorEnabled = true;
		public bool fatalEnabled = true;

		public LogTypeConfig()
		{
		}

		public LogTypeConfig(LogTypeConfig config)
		{
			debugEnabled = config.debugEnabled;
			infoEnabled = config.infoEnabled;
			warnEnabled = config.warnEnabled;
			errorEnabled = config.errorEnabled;
			fatalEnabled = config.fatalEnabled;
		}


		public bool Equals(LogTypeConfig other)
		{
			if (debugEnabled != other.debugEnabled)
				return false;

			if (infoEnabled != other.infoEnabled)
				return false;

			if (warnEnabled != other.warnEnabled)
				return false;

			if (errorEnabled != other.errorEnabled)
				return false;

			if (fatalEnabled != other.fatalEnabled)
				return false;

			return true;
		}

	}

	[Serializable]
	public class LogFormatConfig : IEquatable<LogFormatConfig>
	{
		public string dateTimeFormat = "dd/MM/yy hh:mm";
		public bool sender = true;
		public bool datetime = false;
		public bool type = false;

		public LogFormatConfig()
		{
		}

		public LogFormatConfig(LogFormatConfig format)
		{
			sender = format.sender;
			datetime = format.datetime;
			dateTimeFormat = format.dateTimeFormat;
			type = format.type;
		}

		public bool Equals(LogFormatConfig other)
		{
			if (sender != other.sender)
				return false;

			if (datetime != other.datetime)
				return false;

			if (dateTimeFormat != other.dateTimeFormat)
				return false;

			if (type != other.type)
				return false;

			return true;
		}

		public LogFormat format
		{
			get
			{
				LogFormat result = 0;
				if (sender)
					result |= LogFormat.Sender;

				if (datetime)
					result |= LogFormat.DateTime;

				if (type)
					result |= LogFormat.Type;

				return result;
			}
		}
	}

	[ExecuteInEditMode]
	public class LoggerConfig : MonoBehaviour
	{

		// Enable log level
		[SerializeField]
		public LogTypeConfig logTypes;
		
		// Log format
		[SerializeField]
		public LogFormatConfig logFormat;

		// Stacktrace
		public bool stackTrace = false;

		// Log filters
		public List<string> includeFilters;
		public List<string> excludeFilters;

		void Awake()
		{
			GMLogger.logFormat = logFormat.format;
			GMLogger.dateTimeFormat = logFormat.dateTimeFormat;

			GMLogger.includeFilters = includeFilters;
			GMLogger.excludeFilters = excludeFilters;
		}

		void LateUpdate()
		{
			GMLogger.debugEnabled = logTypes.debugEnabled;
			GMLogger.infoEnabled = logTypes.infoEnabled;
			GMLogger.warnEnabled = logTypes.warnEnabled;
			GMLogger.errorEnabled = logTypes.errorEnabled;
			GMLogger.fatalEnabled = logTypes.fatalEnabled;

			GMLogger.stackTrace = stackTrace;
		}
	}
}

