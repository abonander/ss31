using System;
using Lidgren.Network;

namespace SS31.Network
{
	public class IncomingNetMessageArgs : EventArgs
	{
		public readonly NetIncomingMessage Message;

		public IncomingNetMessageArgs(NetIncomingMessage m)
		{
			Message = m;
		}
	}
}