using System;
using System.IO;
using System.Reflection;

namespace SS31
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

	public class Logger : Service
	{
		private static readonly string[] s_levelText = new string[]
			{
				"Info", "Warn", "ERROR", "FATAL"
			};

		private static StreamWriter s_logStream;
		private static event MessageLoggedHandler s_logEvent;
		private static bool s_isOpen = false;
		public static bool IsOpen { get { return s_isOpen; } private set { s_isOpen = value; } }

		private static void open(NetSide side)
		{
			if (IsOpen)
			{
				Log(LogLevel.Warning, "The log was opened twice.");
				return;
			}

			try
			{
				s_logStream = new StreamWriter(File.Open(side == NetSide.Client ? "client.log" : "server.log", FileMode.Create, FileAccess.ReadWrite, FileShare.None));
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not open the log file for the client. Given reason:");
				Console.WriteLine(e.Message);
				s_logStream = null;
			}

			if (s_logStream == null)
			{
				s_isOpen = false;
				return;
			}
			s_isOpen = true;

			s_logStream.Write(getFileHeader() + "\n\n");
		}

		public static void Close()
		{
			if (!IsOpen)
				return;

			s_logStream.Close();
			s_isOpen = false;
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
			s_logStream.Write(log + "\n");
		}

		public static void SubscribeToMessageEvent(MessageLoggedHandler mlh)
		{
			s_logEvent += mlh;
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
			if (s_logEvent != null)
				s_logEvent(lvl, ts, mes);
		}

		public Logger()
		{
			if (Assembly.GetEntryAssembly().GetName().Name.Contains("client"))
				open(NetSide.Client);
			else
				open(NetSide.Server);
		}

		public override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			Close();
		}
	}
}