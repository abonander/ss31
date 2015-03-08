using System;

namespace SS31.Common
{
	// Will contain all of the important constants that are shared between the client and server.
	public static class SharedConstants
	{
		public const string SERVER_TAG = "SS31_NetTag"; // The unique tag that Lidgren uses to match the server to the clients.
		public const ushort SS_DEFAULT_PORT = 3100; // The default port for the server, 31 from SS31 and 00 to put it into an acceptable port range.
	}
}