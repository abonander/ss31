using System;
using System.IO;
using SS31.Common.Service;

namespace SS31.Common.Config
{
	public abstract class ConfigManager<T> : GameService 
		where T : struct, IConfiguration
	{
		public string ConfigPath { get; protected set; }
		public T Configuration { get; protected set; }
		public readonly SerializeAs ConfigSerialType;

		protected ConfigManager()
		{
			ConfigPath = "";
			Configuration = default(T);
			ConfigSerialType = Configuration.SerializeType;
		}

		public virtual void Initialize(string path)
		{
			if (String.IsNullOrEmpty(path))
				throw new ArgumentException("Configuration file path cannot be null or empty.");
			if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
				throw new ArgumentException("Configuration file path \"" + path + "\" was not valid.");

			ConfigPath = path;

			bool success;
			if (!File.Exists(path))
			{
				Logger.LogError("The config file " + path + " could not be found. Resorting to default settings and creating a new file.");
				Configuration = default(T);
				ObjectSerializer.SerializeToFile<T>(Configuration, ConfigPath, ConfigSerialType, out success);
			}
			else
			{
				Configuration = ObjectSerializer.DeserializeFile<T>(ConfigPath, ConfigSerialType, out success);
			}

			if (!success)
				Logger.LogError("Could not deserialize settings. Resorting to default.");
		}

		public virtual void Save()
		{
			if (String.IsNullOrEmpty(ConfigPath))
				throw new InvalidOperationException("Cannot save the config manager before it has been initilialized.");

			bool success;
			ObjectSerializer.SerializeToFile<T>(Configuration, ConfigPath, ConfigSerialType, out success);
			if (!success)
				Logger.LogError("Could not save the configuration file \"" + ConfigPath + "\".");
		}
	}
}