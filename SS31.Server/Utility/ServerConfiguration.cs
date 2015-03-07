using System;
using SS31.Common.Utility;

namespace SS31.Server.Utility
{
	[Serializable]
	public struct ServerConfiguration : IConfiguration
	{
		public event HandleConfigValueChanged OnValueChanged;

		#region Settings // These will all be serializable by default
		#endregion

		public object GetSetting(string name)
		{
			switch (name)
			{
			default:
				return null;
			}
		}

		public void SetSetting(string name, object value)
		{
			object oldval = GetSetting(name);
			if (oldval == null)
				return;

			switch (name)
			{
			default:
				return;
			}

			if (oldval != value)
				onValueChanged(name, oldval, value);
		}

		private void onValueChanged(string name, object oldval, object newval)
		{
			if (OnValueChanged != null)
				OnValueChanged(name, oldval, newval);
		}
	}
}