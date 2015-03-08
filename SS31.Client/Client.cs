using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS31.Common;
using SS31.Client.Network;
using SS31.Client.Config;
using SS31.Common.Service;

namespace SS31.Client
{
	public class SSClient : Game
	{
		public GraphicsDeviceManager Graphics { get; private set; }
		public SpriteBatch SpriteBatch { get; private set; }

		private Profiler _profiler;
		private StateManager _stateManager;
		private InputManager _inputManager;

		private NetManager _netManager;

		public SSClient() :
			base()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// This will create the Logger and Profiler instances
			ServiceManager.Resolve<Logger>().Open(NetSide.Client);
			_profiler = ServiceManager.Resolve<Profiler>();
		}

		protected override void Initialize()
		{
			_profiler.BeginInterval();
			_profiler.BeginBlock("Initialization");
			Logger.LogInfo("Initializing the game.");

			ServiceManager.Resolve<ClientConfigManager>().Initialize("client.cfg");
			var config = ServiceManager.Resolve<ClientConfigManager>().Configuration;
			Graphics.PreferredBackBufferWidth = config.ScreenWidth;
			Graphics.PreferredBackBufferHeight = config.ScreenHeight;
			Graphics.ApplyChanges();

			(_stateManager = ServiceManager.Resolve<StateManager>()).Initialize(this);
			_inputManager = ServiceManager.Resolve<InputManager>();

			_netManager = ServiceManager.Resolve<NetManager>();

			// TODO: Switch this out when we get an actual main menu system and whatnot going
			_stateManager.SwitchTo<GameState>();

			base.Initialize();
			_profiler.EndBlock();
		}

		protected override void LoadContent()
		{
			_profiler.BeginBlock("Content Loading");
			Logger.LogInfo("Loading game content.");

			SpriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: Load the content things

			_profiler.EndBlock();
		}

		protected override void Update(GameTime gameTime)
		{
			_profiler.BeginFrame(); // This will also end the previous frame
			_profiler.BeginBlock("BaseUpdate");

			_netManager.Update(gameTime);
			_inputManager.Update(gameTime);
			_stateManager.Update(gameTime);

			base.Update(gameTime);
			_profiler.EndBlock();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_profiler.BeginBlock("BaseDraw");

			_stateManager.Draw(gameTime);

			base.Draw(gameTime);
			_profiler.EndBlock();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			Logger.LogInfo("Exiting the game.");

			// TODO: Cleanup things that require cleaning
			ServiceManager.UnregisterService<StateManager>(); // Clean up the scene manager first, to release objects that other managers may need released to shut down

			ServiceManager.UnregisterAll();
			base.OnExiting(sender, args);
		}
	}
}