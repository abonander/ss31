using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using SS31.Common;
using SS31.Common.Service;

namespace SS31.Server
{
	public class SSServer
	{
		public bool Initialized { get; private set; } // If this server has been initialized
		public bool Running { get; private set; } // If the server is running in its main loop
		public uint TickRate { get; private set; } // The number of times the server should update every second
		public float ServerRate { get { return 1000.0f / TickRate; } } // The time each update should take (in milliseconds)

		private Stopwatch _timer;
		private TimeSpan _lastTime;
		private TimeSpan _lastTitleUpdate;

		public SSServer()
		{
			Initialized = false;

			_timer = new Stopwatch();
			_lastTime = TimeSpan.Zero;
			_lastTitleUpdate = TimeSpan.Zero;
		}

		public bool Initialize()
		{
			if (Initialized)
				return true;

			// TODO: Initialization stuff
			TickRate = 20; // TODO: This is temporary, we should eventually load it from a config manager.

			Initialized = true;
			return true;
		}

		public void Run()
		{
			_timer.Restart();
			Running = true;

			while (Running)
				mainLoop();
		}

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
				Console.Title = string.Format("TPS: {0:N2}", 1.0f / gameTime.ElapsedGameTime.TotalSeconds);
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
						handleData(msg); // TODO: Make sure this data is from a valid connection
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

		}

		// Handle status change packets (client connecting or disconnecting)
		private void handleStatusChange(NetIncomingMessage msg)
		{

		}
		#endregion
	}
}