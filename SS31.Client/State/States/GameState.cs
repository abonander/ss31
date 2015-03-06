using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SS31.Common;
using SS31.Common.Service;
using SS31.Client.Network;
using SS31.Common.Network;
using Lidgren.Network;

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
			if (args.Message.MessageType == NetIncomingMessageType.Data)
			{
				NetMessageType type = (NetMessageType)args.Message.ReadByte();
				if (type == NetMessageType.PlainText)
					Console.WriteLine("Server: \"" + args.Message.ReadString() + "\"");
			}
		}

		void handleKeyDown(Keys key, KeyboardState p, KeyboardState c)
		{
			NetOutgoingMessage ms = ServiceManager.Resolve<NetManager>().CreateMessage();
			ms.Write((byte)NetMessageType.PlainText);
			ms.Write(key.ToString());
			ServiceManager.Resolve<NetManager>().SendMessage(ms, NetDeliveryMethod.ReliableOrdered);
		}

		protected override void Initialize()
		{
			ServiceManager.Resolve<NetManager>().OnMessageRecieved += handleNetworkMessage;
			ServiceManager.Resolve<NetManager>().Connect(new IPEntry("127.0.0.1:8100"));
			ServiceManager.Resolve<InputManager>().KeyPressed += handleKeyDown;
		}

		public override void Update(GameTime gameTime)
		{

		}

		public override void Draw(GameTime gameTime)
		{

		}
	}
}