using System;

namespace SS31.Common.Config
{
	// Delegate for event in configurations when a value changes
	public delegate void HandleConfigValueChange(string name, object oldVal, object newVal);

	// Base class for the general configuration files for the client and server
	public interface IConfiguration
	{
		event HandleConfigValueChange OnValueChange;

		SerializeAs GetSerializeType();
	}
}