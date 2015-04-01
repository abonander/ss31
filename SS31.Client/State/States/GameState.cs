using Microsoft.Xna.Framework;
using SS31.Common.Service;
using SS31.Client.Network;
using SS31.Common.Network;
using SS31.Client.UI.Widgets;

namespace SS31.Client.State.States
{
	// This is just the test state, until we get a  nice splash screen and main menu and whatnot going.
	public class GameState : State
	{
		public override bool ShouldSuspend { get { return false; } }

		private Window _menuWindow;

		public GameState()
		{
			_menuWindow = null;
		}

		public override void Dispose(bool disposing)
		{
			if (!IsDisposed && disposing)
			{
				if (ServiceManager.HasService<NetManager>())
					ServiceManager.Resolve<NetManager>().OnMessageRecieved -= handleNetworkMessage;

				if (ServiceManager.HasService<UI.UIManager>())
					ServiceManager.Resolve<UI.UIManager>().RemoveWidget(_menuWindow);
			}

			base.Dispose(disposing);
		}

		void handleNetworkMessage(object sender, IncomingNetMessageArgs args)
		{

		}

		protected override void Initialize()
		{
			ServiceManager.Resolve<NetManager>().OnMessageRecieved += handleNetworkMessage;
			// ServiceManager.Resolve<NetManager>().Connect(new IPEntry("127.0.0.1:8100"));

			_menuWindow = new Window()
			{
					OuterArea = new Rectangle(0, 0, 400, 400)
			};
			ServiceManager.Resolve<UI.UIManager>().AddWidget(_menuWindow);
		}

		public override void Update(GameTime gameTime)
		{

		}

		public override void Draw(GameTime gameTime)
		{

		}
	}
}