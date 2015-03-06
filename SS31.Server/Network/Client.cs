using System;
using Lidgren.Network;

namespace SS31.Server
{
	// A record of a client currently connected to the server
	public class Client
	{
		public NetConnection Connection { get; private set; }
		public string PlayerName { get; set; }

		public Client(NetConnection conn)
		{
			Connection = conn;
			PlayerName = "";
		}
	}
}