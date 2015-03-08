using System;
using SS31.Common.Network;
using SS31.Common;
using SS31.Common.Service;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace SS31.Client.Network
{
	// Client side network manager. Contains all of the data about the connection to the server.
	// Automatically updates once per frame and polls for the events coming from the server.
	// Handles non-data events, and passes data events through the OnMessageRecieved event to
	// any and all classes that are subscribed. Its a good idea for game states to have a method that
	// subscribes to that event, and unsubscribe when the game state is being destroyed or suspended.
	public class NetManager : GameService, INetManager
	{
		#region Members
		private readonly NetPeerConfiguration _netConfig;
		public NetPeerStatistics Stats { get { return NetClient.Statistics; } }
		public NetClient NetClient { get; private set; }
		public IPEntry GlobalIp { get { return new IPEntry(); } } // TODO: Implement this by detecting the global WAN ip
		public bool IsConnected { get; private set; }
		public long Uid { get { return NetClient.UniqueIdentifier; } }

		public event EventHandler OnConnect; // Event for connect to the server
		public event EventHandler OnDisconnect; // Event for disconnect from the server
		public event EventHandler<IncomingNetMessageArgs> OnMessageRecieved; // Event for a data message being recieved
		#endregion

		public NetManager()
		{
			_netConfig = new NetPeerConfiguration(SharedConstants.SERVER_TAG);

			NetClient = new NetClient(_netConfig);
			NetClient.Start();
		}

		// Checks for a change in the state of the connection, and polls the messages and raises the events.
		public void Update(GameTime gameTime)
		{
			if (IsConnected)
			{
				NetIncomingMessage msg;
				while ((msg = NetClient.ReadMessage()) != null)
				{
					onMessageRecieved(msg);
					NetClient.Recycle(msg);
				}
			}

			if (!IsConnected && NetClient.ServerConnection != null)
			{
				onConnect();
				IsConnected = true;
			}
			if (IsConnected && NetClient.ServerConnection == null)
			{
				onDisconnect();
				IsConnected = false;
			}
		}

		public void Connect(IPEntry ip)
		{
			NetClient.Connect(ip.IPString, (int)ip.Port);
		}

		public void Disconnect()
		{
			Restart();
		}

		public NetOutgoingMessage CreateMessage()
		{
			return NetClient.CreateMessage();
		}
		public void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			NetClient.SendMessage(message, method);
		}

		public void Restart()
		{
			NetClient.Disconnect("Forced Restart");
			NetClient = new NetClient(_netConfig);
			NetClient.Start();
		}
		public void Shutdown()
		{
			NetClient.Disconnect("Shutdown");
		}

		public override void Dispose(bool disposing)
		{
			if (!Disposed)
				Shutdown();

			base.Dispose(disposing);
		}

		#region Event Functions
		private void onConnect()
		{
			if (OnConnect != null)
				OnConnect(this, null);
		}

		private void onDisconnect()
		{
			if (OnDisconnect != null)
				OnDisconnect(this, null);
		}

		private void onMessageRecieved(NetIncomingMessage m)
		{
			if (OnMessageRecieved != null)
				OnMessageRecieved(this, new IncomingNetMessageArgs(m));
		}
		#endregion
	}
}