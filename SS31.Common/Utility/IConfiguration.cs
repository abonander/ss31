using System;

namespace SS31.Common.Utility
{
	public delegate void HandleConfigValueChanged(string name, object oldValue, object newValue);

	public interface IConfiguration
	{
		event HandleConfigValueChanged OnValueChanged;

		object GetSetting(string name);
		void SetSetting(string name, object value);
	}
}