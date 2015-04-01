using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS31.Common;
using SS31.Client.Network;
using SS31.Common.Service;
using SS31.Client.UI;
using SS31.Client.Config;
using SS31.Client.Content;
using SS31.Client.State;
using SS31.Client.State.States;
using SS31.Client.Input;

namespace SS31.Client
{
	// The base class for the game. Monogame handles initialization and updating this.
	public class SSClient : Game
	{
		public GraphicsDeviceManager Graphics { get; private set; }
		public SpriteBatch SpriteBatch { get; private set; }

		private Profiler _profiler;
		private StateManager _stateManager;
		private InputManager _inputManager;

		private NetManager _netManager;
		private UIManager _uiManager;

		// Not much should ever happen here. Put the initialization stuff in initialize.
		// Methods are called as such: Initialize() -> LoadContent() -> { Update(), Draw() }(repeated) -> OnExiting()
		public SSClient() :
			base()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// This will create the Logger and Profiler instances
			ServiceManager.Resolve<Logger>().Open(NetSide.Client);
			_profiler = ServiceManager.Resolve<Profiler>();
		}

		// Do all of the initialization steps for the game. All graphics changes should be done here, as the window
		// opens for the first time right after this method returns.
		protected override void Initialize()
		{
			_profiler.BeginInterval();
			_profiler.BeginBlock("Initialization");
			Logger.LogInfo("Initializing the game.");

			ServiceManager.Resolve<ClientConfigurationManager>().Initialize("client.cfg");
			ServiceManager.Resolve<ClientConfigurationManager>().Load();
			var config = ServiceManager.Resolve<ClientConfigurationManager>().Configuration;
			this.IsMouseVisible = true;
			Graphics.PreferredBackBufferWidth = config.ScreenWidth;
			Graphics.PreferredBackBufferHeight = config.ScreenHeight;
			Graphics.ApplyChanges();

			(_stateManager = ServiceManager.Resolve<StateManager>()).Initialize(this);
			_inputManager = ServiceManager.Resolve<InputManager>();

			_netManager = ServiceManager.Resolve<NetManager>();
			(_uiManager = ServiceManager.Resolve<UIManager>()).Initialize(GraphicsDevice, Content);

			base.Initialize();
			_profiler.EndBlock();
		}

		// Not much use for this yet, as we will probably go with loading content as it is needed.
		// Maybe get the gui content loaded here in the future.
		protected override void LoadContent()
		{
			_profiler.BeginBlock("Content Loading");
			Logger.LogInfo("Loading game content.");

			SpriteBatch = new SpriteBatch(GraphicsDevice);

			ServiceManager.Resolve<ContentManagerService>().Initialize(Content);

			// TODO: Switch this out when we get an actual main menu system and whatnot going
			_stateManager.SwitchTo<GameState>();

			_profiler.EndBlock();
		}

		// Base update function, updates the network, input, and then the active state
		protected override void Update(GameTime gameTime)
		{
			_profiler.BeginFrame(); // This will also end the previous frame
			_profiler.BeginBlock("BaseUpdate");

			_netManager.Update(gameTime);
			_inputManager.Update(gameTime);
			_uiManager.Update(gameTime);
			_stateManager.Update(gameTime);

			base.Update(gameTime);
			_profiler.EndBlock();
		}

		// Base draw function, draws the active state, and soon the GUI system
		protected override void Draw(GameTime gameTime)
		{
			_uiManager.Predraw();

			GraphicsDevice.Clear(Color.CornflowerBlue);
			_profiler.BeginBlock("BaseDraw");

			_stateManager.Draw(gameTime);
			_uiManager.Draw();

			base.Draw(gameTime);
			_profiler.EndBlock();
		}

		// Called when the game exits. Right now just unregisters services.
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