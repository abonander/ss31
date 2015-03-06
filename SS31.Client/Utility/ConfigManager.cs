using System;
using System.Xml.Serialization;
using System.IO;
using SS31.Common.Utility;
using SS31.Common;

namespace SS31.Client.Utility
{
	public class ClientConfigManager : ConfigManager<ClientConfiguration>
	{
		public override void Initialize(string file)
		{
			if (File.Exists(file))
			{
				XmlSerializer loader = new XmlSerializer(typeof(ClientConfiguration));
				StreamReader reader = File.OpenText(file);
				ClientConfiguration config = (ClientConfiguration)loader.Deserialize(reader);
				reader.Close();
				Configuration = config;
				ConfigFile = file;
			}
			else
			{
				Logger.LogError("Could not load the configuration file: \"" + file + "\". Creating default file and using default settings.");
				Configuration = new ClientConfiguration();
				ConfigFile = file;
				Save();
			}
		}

		public override void Save()
		{

		}
	}
}