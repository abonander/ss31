using System;
using SS31.Common.Config;

namespace SS31.Client.Config
{
	public struct ClientConfiguration : IConfiguration
	{
		// Width of the screen
		public ushort ScreenWidth;
		// Height of the screen
		public ushort ScreenHeight;
	}
}