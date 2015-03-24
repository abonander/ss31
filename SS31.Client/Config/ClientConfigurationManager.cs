using System;
using SS31.Common.Config;

namespace SS31.Client.Config
{
	public class ClientConfigurationManager : ConfigurationManager<ClientConfiguration>
	{
		private static readonly ClientConfiguration _defaultConfiguration = new ClientConfiguration()
			{
				ScreenWidth = 800,
				ScreenHeight = 600
			};

		public override ClientConfiguration CreateDefaultConfiguration()
		{
			return _defaultConfiguration;
		}
	}
}