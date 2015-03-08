using System;
using SS31.Common;
using SS31.Common.Config;

namespace SS31.Client.Config
{
	public class ClientConfirguration : IConfiguration
	{
		public event HandleConfigValueChange OnValueChange;

		#region Members
		private ushort _screenWidth = 800;
		public ushort ScreenWidth
		{
			get { return _screenWidth; }
			set
			{
				//onValueChange("ScreenWidth", _screenWidth, value);
				_screenWidth = value;
			}
		}
		private ushort _screenHeight = 600;
		public ushort ScreenHeight
		{
			get { return _screenHeight; }
			set
			{
				//onValueChange("ScreenHeight", _screenHeight, value);
				_screenHeight = value;
			}
		}
		#endregion

		public SerializeAs GetSerializeType()
		{
			return SerializeAs.Yaml;
		}

		private void onValueChange(string name, object oldVal, object newVal)
		{
			if (OnValueChange != null && oldVal != newVal)
				OnValueChange(name, oldVal, newVal);
		}
	}
}