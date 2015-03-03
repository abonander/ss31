using System;
using System.Windows.Forms;

namespace SS31
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
				ServiceManager.Resolve<Logger>();
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
				return;
			}

			Application.Run(new ServerForm(server));
		}

		static void processArgs(string[] args)
		{

		}
	}
}