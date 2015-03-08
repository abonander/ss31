using System;
using SS31.Common;
using SS31.Common.Config;

namespace SS31.Server.Config
{
	public class ServerConfiguration : IConfiguration
	{
		public event HandleConfigValueChange OnValueChange;

		#region Members
		private string _serverName = "SS31 Server";
		public string ServerName
		{
			get { return _serverName; }
			set
			{
				onValueChange("ServerName", _serverName, value);
				_serverName = value;
			}
		}

		private uint _tickRate = 20;
		public uint ServerTickRate
		{
			get { return _tickRate; }
			set
			{
				onValueChange("ServerTickRate", _tickRate, value);
				_tickRate = value;
			}
		}
		#endregion

		public SerializeAs GetSerializeType()
		{
			return SerializeAs.Json;
		}

		private void onValueChange(string name, object oldVal, object newVal)
		{
			if (OnValueChange != null && oldVal != newVal)
				OnValueChange(name, oldVal, newVal);
		}
	}
}