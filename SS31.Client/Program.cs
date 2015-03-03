using System;

namespace SS31
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