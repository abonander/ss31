using System;
using System.IO;
using System.Reflection;
using SS31.Core.Service;
using SS31.Core.IO;
using SS31.Core.IO.ORM;

namespace SS31.Core.Config
{
	// Base class for generating default configurations and loading and saving the configurations to the disk
	public abstract class ConfigurationManager<T> : GameService 
		where T : struct, IConfiguration
	{
		#region Members
		public string ConfigPath { get; private set; }
		public T Configuration { get; private set; }
		#endregion

		protected ConfigurationManager()
		{
			ConfigPath = null;
			Configuration = default(T);
		}

		public void Initialize(string path)
		{
			ConfigPath = path;
			Configuration = CreateDefaultConfiguration();
		}

		public void Reset()
		{
			Configuration = CreateDefaultConfiguration();
		}

		public virtual void Load()
		{
			if (String.IsNullOrEmpty(ConfigPath))
				throw new InvalidOperationException("Cannot load the configuration until the ConfigManager has been initialized.");

			if (!File.Exists(ConfigPath))
			{
				Logger.LogWarning("The client configuration file could not be found. Resetting to defaults.");
				Reset();
				Save();
			}
			else
			{
				try
				{
					Reset();
					Configuration = JsonParser.DeserializeObject<T>(File.ReadAllText(ConfigPath));
					Logger.LogInfo("Loaded configuration settings from the disk.");
				}
				catch (Exception ex)
				{
					Logger.LogError("Could not load from the configuration file. Resetting to defaults.");
					Logger.LogException(ex);
					Reset();
				}
			}
		}

		public virtual void Save()
		{
			if (String.IsNullOrEmpty(ConfigPath))
				throw new InvalidOperationException("Cannot save the configuration until the ConfigManager has been initialized.");

			if (!File.Exists(ConfigPath))
			{
				Logger.LogWarning("Writing a new config file.");
				try
				{
					File.Create(ConfigPath).Close();
				}
				catch (Exception ex)
				{
					Logger.LogError("Could not save the configuration to a file. Config settings will not carry over between instances.");
					Logger.LogException(ex);
					return;
				}
			}
			else
				Logger.LogWarning("Overwriting an existing config file.");

			try
			{
				string json = JsonParser.SerializeObject<T>(Configuration);
				File.WriteAllText(ConfigPath, json);
				Logger.LogInfo("Saved configuration settings to the disk.");
			}
			catch (Exception ex)
			{
				Logger.LogError("Could not save the configuration to a file. Config settings will not carry over between instances.");
				Logger.LogException(ex);
			}
		}

		public abstract T CreateDefaultConfiguration();
	}
}