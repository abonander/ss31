using System;

namespace SS31.Common.Config
{
	// Attribute telling the config manager to ignore the member when loading and saving
	public class ConfigIgnoreAttribute : Attribute
	{

	}

	// Attribute telling the config manager to load and save it to the config file under a different name
	public class ConfigKnownAsAttribute : Attribute
	{
		public readonly String EffectiveName;

		public ConfigKnownAsAttribute(string name)
		{
			EffectiveName = name;
		}
	}
}