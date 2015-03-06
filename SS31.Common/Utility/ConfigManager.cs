using System;
using SS31.Common.Service;

namespace SS31.Common.Utility
{
	public abstract class ConfigManager<T> : GameService 
		where T : IConfiguration, new()
	{
		public string ConfigFile { get; protected set; }
		public T Configuration { get; protected set; }
		public Type ConfigurationType { get { return typeof(T); } }

		protected ConfigManager()
		{
			ConfigFile = "config.cfg";
			Configuration = default(T);
		}

		public abstract void Initialize(string file);
		public abstract void Save();
	
		public T GetDefaultConfiguration()
		{
			return new T();
		}
		public object GetDefaultValue(string name)
		{
			return (new T()).GetSetting(name);
		}

		public object GetSetting(string name)
		{
			return Configuration.GetSetting(name);
		}
		public void SetSetting(string name, object value)
		{
			Configuration.SetSetting(name, value);
		}
	}
}