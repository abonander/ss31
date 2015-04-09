using System;

namespace SS31.Client
{
	public class Program
	{
		static void Main()
		{
			string[] args = Environment.GetCommandLineArgs();
			processArgs(args);

			using (SSClient client = new SSClient())
				client.Run();
		}

		static void processArgs(string[] args)
		{

		}
	}
}