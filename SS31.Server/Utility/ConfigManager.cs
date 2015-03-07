using System;
using System.IO;
using System.Xml.Serialization;
using SS31.Common.Utility;
using SS31.Common;

namespace SS31.Server.Utility
{
	public class ServerConfigManager : ConfigManager<ServerConfiguration>
	{
		public override void Initialize(string file)
		{
			if (File.Exists(file))
			{
				XmlSerializer loader = new XmlSerializer(typeof(ServerConfiguration));
				StreamReader reader = File.OpenText(file);
				ServerConfiguration config = (ServerConfiguration)loader.Deserialize(reader);
				reader.Close();
				Configuration = config;
				ConfigFile = file;
			}
			else
			{
				Logger.LogError("Could not load the configuration file: \"" + file + "\". Creating default file and using default settings.");
				Configuration = new ServerConfiguration();
				ConfigFile = file;
				Save();
			}
		}

		public override void Save()
		{
			XmlSerializer saver = new XmlSerializer(typeof(ServerConfiguration));
			StreamWriter writer = File.CreateText(ConfigFile);
			saver.Serialize(writer, Configuration);
			writer.Flush();
			writer.Close();
		}
	}
}