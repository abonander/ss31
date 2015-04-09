using System;
using Lidgren.Network;

namespace SS31.Common.Network
{
	// Event args for the OnMessageRecieved event in INetManager
	public class IncomingNetMessageArgs : EventArgs
	{
		public readonly NetIncomingMessage Message;

		public IncomingNetMessageArgs(NetIncomingMessage m)
		{
			Message = m;
		}
	}
}