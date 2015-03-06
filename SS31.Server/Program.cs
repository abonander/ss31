using System;
using SS31.Common;
using SS31.Common.Service;

namespace SS31.Server
{
	public class Program
	{
		static void Main()
		{
			string[] args = Environment.GetCommandLineArgs();
			processArgs(args);

			SSServer server = null;
			try
			{
				ServiceManager.Resolve<Logger>(); // Start up the logger
				server = new SSServer();
				if (!server.Initialize())
				{
					Logger.LogFatal("Could not initialize the server instance.");
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.LogFatal("Could not initialize the server instance.");
				Logger.LogException(ex);
				Environment.Exit(-1);
			}

			try
			{
				server.Run();
			}
			catch(Exception e)
			{
				Logger.LogFatal("An exception went unhandled while the program was running!");
				Logger.LogException(e);
				Environment.Exit(-1);
			}

			ServiceManager.UnregisterAll();
		}

		static void processArgs(string[] args)
		{

		}
	}
}