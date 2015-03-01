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

			SSServer server = new SSServer();
			server.Initialize();
			Application.Run(new ServerForm(server));
		}

		static void processArgs(string[] args)
		{

		}
	}
}