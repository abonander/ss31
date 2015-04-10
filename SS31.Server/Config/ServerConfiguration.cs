using System;
using SS31.Core.Config;

namespace SS31.Server.Config
{
	public struct ServerConfiguration : IConfiguration
	{
		// Name of the server
		public string ServerName;
		// Target TPS of the server
		public uint TargetTPS;
	}
}