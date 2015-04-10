using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace SS31.Core.Network
{
	// Interface for the manager that updates and polls for network messages.
	// Also keeps the information for the current connection.
	// This looks like it moght only be used for client, which means we will probably get rid of it in the future.
	public interface INetManager
	{
		NetPeerStatistics Stats { get; }
		NetClient NetClient { get; }
		IPEntry GlobalIp { get; } // The global internet IP for this machine
		bool IsConnected { get; }
		long Uid { get; }

		event EventHandler OnConnect;
		event EventHandler OnDisconnect;
		event EventHandler<IncomingNetMessageArgs> OnMessageRecieved;

		void Update(GameTime gameTime);

		void Connect(IPEntry ip);
		void Disconnect();

		NetOutgoingMessage CreateMessage();
		void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method);
	}
}