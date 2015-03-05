using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using SS31.Common;

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
		}

		#region Packet Handling
		private void handleData(NetIncomingMessage msg)
		{

		}
		#endregion
	}
}