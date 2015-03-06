using System;
using System.IO;
using System.Reflection;
using SS31.Common.Service;

namespace SS31.Common
{
	public enum LogLevel : 
		byte
	{
		Info = 0,
		Warning = 1,
		Error = 2,
		Fatal = 3
	}

	// The delegate for subscribing to message logged events.
	// Passes the log level of the message, the time stamp in the format "[H:mm:ss]", and the message itself.
	public delegate void MessageLoggedHandler(LogLevel lvl, string ts, string mes);

	public class Logger : GameService
	{
		private static readonly string[] s_levelText = new string[]
			{
				"Info", "Warn", "ERROR", "FATAL"
			};

		private static StreamWriter logStream;
		private static event MessageLoggedHandler logEvent;
		private static object mutex;
		private static bool isOpen = false;
		public static bool IsOpen { get { return isOpen; } private set { isOpen = value; } }

		public void Open(NetSide side)
		{
			if (mutex == null)
				mutex = new object();

			lock (mutex)
			{
				if (IsOpen)
				{
					Log(LogLevel.Warning, "The log was opened twice.");
					return;
				}
				
				try
				{
					logStream = new StreamWriter(File.Open(side == NetSide.Client ? "client.log" : "server.log", FileMode.Create, FileAccess.ReadWrite, FileShare.None));
				}
				catch (Exception e)
				{
					Console.WriteLine("Could not open the log file for the client. Given reason: ");
					Console.WriteLine(e.Message);
					logStream = null;
				}
				
				if (logStream == null)
				{
					isOpen = false;
					return;
				}
				isOpen = true;
				
				logStream.Write(getFileHeader() + "\n\n");
			}
		}

		public void Close()
		{
			if (!IsOpen)
				return;

			lock (mutex)
			{
				logStream.Close();
				isOpen = false;
			}
			// TODO: Log the total runtime before closing.
		}

		public static void Log(LogLevel lvl, string mes)
		{
			if (!IsOpen)
				return;

			string ll = "[" + s_levelText[(int)lvl] + "]";
			string ts = "[" + getTimeStamp() + "]";

			dispatchEvent(lvl, ts, mes);

			string log = ll + ts + ":\t" + mes;
			Console.WriteLine(log);
			lock (mutex)
			{
				logStream.Write(log + "\n");
			}
		}

		public static void LogException(Exception e)
		{
			if (!IsOpen)
				return;

			string ts = "[" + getTimeStamp() + "]";
			string mes =
				"\tMessage: \"" + e.Message + "\"\n" + 
				"\tStack Trace:\n" + 
				"\t" + e.StackTrace.Replace("\n", "\n\t\t");

			dispatchEvent(LogLevel.Fatal, ts, mes);

			string log = "[EXCEPTION]" + ts + ":\n" + mes;
			Console.WriteLine(log);
			lock (mutex)
			{
				logStream.Write(log + "\n");
			}
		}

		public static void SubscribeToMessageEvent(MessageLoggedHandler mlh)
		{
			logEvent += mlh;
		}

		#region Easy Log Functions
		public static void LogInfo(string mes) { Log(LogLevel.Info, mes); }
		public static void LogWarning(string mes) { Log(LogLevel.Warning, mes); }
		public static void LogError(string mes) { Log(LogLevel.Error, mes); }
		public static void LogFatal(string mes) { Log(LogLevel.Fatal, mes); }
		#endregion

		private static string getTimeStamp()
		{
			return DateTime.Now.ToString("H:mm:ss");
		}

		private static string getFileHeader()
		{
			return DateTime.Now.ToString("H:mm:ss tt zz");
		}

		private static void dispatchEvent(LogLevel lvl, string ts, string mes)
		{
			if (logEvent != null)
				logEvent(lvl, ts, mes);
		}
	}
}