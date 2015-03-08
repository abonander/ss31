using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using SS31.Common;
using SS31.Common.Service;
using SS31.Common.Network;
using SS31.Server.Config;

namespace SS31.Server
{
	// Base class for the server application. Contains all of the general server information,
	// as well as the update loop.
	public class SSServer
	{
		public bool Initialized { get; private set; } // If this server has been initialized
		public bool Running { get; private set; } // If the server is running in its main loop
		public uint TickRate { get; private set; } // The number of times the server should update every second
		public float ServerRate { get { return 1000.0f / TickRate; } } // The time each update should take (in milliseconds)
		public string ServerName { get; private set; }

		public Dictionary<NetConnection, Client> ClientList;

		private Stopwatch _timer;
		private TimeSpan _lastTime;
		private TimeSpan _lastTitleUpdate;

		private int _lastSentBytes; // Number of bytes sent in the last title update
		private int _lastRecvBytes; // NUmber of bytes recieved in the last title update

		public SSServer()
		{
			Initialized = false;
			Running = false;

			ClientList = new Dictionary<NetConnection, Client>();

			_timer = new Stopwatch();
			_lastTime = TimeSpan.Zero;
			_lastTitleUpdate = TimeSpan.Zero;

			_lastSentBytes = 0;
			_lastRecvBytes = 0;
		}

		// Initialization stuff here...
		public bool Initialize()
		{
			if (Initialized)
				return true;

			// TODO: Initialization stuff
			ServiceManager.Resolve<ServerConfigManager>().Initialize("server.cfg");
			var config = ServiceManager.Resolve<ServerConfigManager>().Configuration;
			ServerName = config.ServerName;
			TickRate = config.ServerTickRate;

			Initialized = true;
			return true;
		}

		#region Accessors
		public Client GetClient(NetConnection conn)
		{
			return ClientList[conn];
		}
		#endregion

		public void Restart()
		{
			Logger.LogWarning("RESTARTING SERVER...");
			// TODO: Let players know that we are restarting
			DisposeForRestart();
			// TODO: Restart into the lobby
		}

		public void DisposeForRestart()
		{
			// TODO: Dispose and empty all of the world and entity related managers
			GC.Collect();
		}

		// Start the server and launch into the main loop.
		// Main loop needs to be changed as it gums up the server right now.
		public void Run()
		{
			_timer.Restart();
			Running = true;

			ServiceManager.Resolve<SSNetServer>().Start();
			Logger.LogInfo(String.Format("Server running. Listening on port: {0:D}", ServiceManager.Resolve<SSNetServer>().Port));

			while (Running)
				mainLoop();
		}

		// The actual main loop stuff.
		private void mainLoop()
		{
			TimeSpan total = _timer.Elapsed;
			TimeSpan delta = total - _lastTime;
			float deltaSec = (float)delta.TotalMilliseconds;
			if (deltaSec < ServerRate && (ServerRate - deltaSec) >= 0.2f)
				return; // If still have over fifth a millisecond before we are supposed to update again

			GameTime gameTime = new GameTime(total, delta);

			updateNetwork(gameTime);
			updateTitle(gameTime);

			// TODO: Other updatey things

			_lastTime = total;
		}

		// Updates the title with general relative information
		private void updateTitle(GameTime gameTime)
		{
			if ((gameTime.TotalGameTime - _lastTitleUpdate).TotalSeconds >= 0.5f)
			{
				NetPeerStatistics stats = ServiceManager.Resolve<SSNetServer>().Statistics;
				int dSent = (_lastSentBytes - stats.SentBytes);
				int dRecv = (_lastRecvBytes - stats.ReceivedBytes);
				_lastSentBytes = stats.SentBytes;
				_lastRecvBytes = stats.ReceivedBytes;

				Console.Title = string.Format("{0} - TPS: {1:N2}({2:N2}) | Net: (S: {3:N0} KiB/s, R: {4:N0} KiB/s) | Mem: {5:N0} KiB", 
					ServerName, 1.0f / gameTime.ElapsedGameTime.TotalSeconds, TickRate, (dSent >> 10) / 2, (dRecv >> 10) / 2,
					Process.GetCurrentProcess().PrivateMemorySize64 >> 10);
				_lastTitleUpdate = gameTime.TotalGameTime;
			}
		}

		// Message pump, reads through them and dispatches them as necessary
		private void updateNetwork(GameTime gameTime)
		{
			NetIncomingMessage msg;
			SSNetServer server = ServiceManager.Resolve<SSNetServer>();
			while ((msg = server.ReadMessage()) != null)
			{
				switch (msg.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						Logger.LogInfo(msg.ReadString());
						break;
					case NetIncomingMessageType.WarningMessage:
						Logger.LogWarning(msg.ReadString());
						break;
					case NetIncomingMessageType.ErrorMessage:
						Logger.LogError(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						if (ClientList.ContainsKey(msg.SenderConnection))
							handleData(msg); 
						break;
					case NetIncomingMessageType.StatusChanged:
						handleStatusChange(msg);
						break;
					default:
						Logger.LogError("An unhandled message was recieved: " + msg.MessageType);
						break;
				}
				server.Recycle(msg);
			}
		}

		#region Packet Handling
		// Handle data packets (custom user packets)
		private void handleData(NetIncomingMessage msg)
		{
			switch ((NetMessageType)msg.ReadByte())
			{
				default:
					Logger.LogWarning("Unhandled message type.");
					break;
			}
		}

		// Handle status change packets (client connecting or disconnecting)
		private void handleStatusChange(NetIncomingMessage msg)
		{
			NetConnection conn = msg.SenderConnection;
			string sip = conn.RemoteEndpoint.Address.ToString();
			Logger.LogInfo(sip + ": STATUS CHANGE");

			switch (conn.Status)
			{
				case NetConnectionStatus.Connected:
					Logger.LogInfo(sip + ": CONNECTION REQUEST");
					if (ClientList.ContainsKey(conn))
					{
						Logger.LogWarning(sip + "(" + ClientList[conn].PlayerName + "): already connected.");
						return;
					}
					// TODO: When the ban manager is implemented, here is where we would reject a banned connection
					// TODO: Implement the player manager, and then let it know that someone connected
					ClientList.Add(conn, new Client(conn));
					Logger.LogInfo(sip + ": CONNECTED");
					break;
				case NetConnectionStatus.Disconnected:
					if (ClientList.ContainsKey(conn))
						Logger.LogInfo(sip + "(" + ClientList[conn].PlayerName + "): DISCONNECTED");
					else
						Logger.LogInfo(sip + ": DISCONNECTED");
					// TODO: Let the player manager know that the client has disconnected
					if (ClientList.ContainsKey(conn))
						ClientList.Remove(conn);
					break;
			}
		}
		#endregion
	}
}