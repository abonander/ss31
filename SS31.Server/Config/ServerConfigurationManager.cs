using System;
using SS31.Core.Config;

namespace SS31.Server.Config
{
	public class ServerConfigurationManager : ConfigurationManager<ServerConfiguration>
	{
		private static readonly ServerConfiguration _defaultConfiguration = new ServerConfiguration()
			{
				ServerName = "SS31 Server",
				TargetTPS = 20
			};

		public override ServerConfiguration CreateDefaultConfiguration()
		{
			return _defaultConfiguration;
		}
	}
}