using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace SS31.Common.Network
{
	public interface INetManager
	{
		NetPeerStatistics Stats { get; }
		NetClient NetClient { get; }
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