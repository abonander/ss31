using System;
using Microsoft.Xna.Framework;
using SS31.Common;
using SS31.Client.Network;
using SS31.Common.Network;

namespace SS31.Client
{
	public class GameState : State
	{
		public override bool ShouldSuspend { get { return false; } }

		public GameState()
		{

		}

		public override void Dispose(bool disposing)
		{
			if (!IsDisposed && disposing)
			{
				if (ServiceManager.HasService<NetManager>())
					ServiceManager.Resolve<NetManager>().OnMessageRecieved -= handleNetworkMessage;
			}

			base.Dispose(disposing);
		}

		void handleNetworkMessage(object sender, IncomingNetMessageArgs args)
		{

		}

		protected override void Initialize()
		{
			ServiceManager.Resolve<NetManager>().OnMessageRecieved += handleNetworkMessage;
		}

		public override void Update(GameTime gameTime)
		{

		}

		public override void Draw(GameTime gameTime)
		{

		}
	}
}