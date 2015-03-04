using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS31.Common;
using SS31.Client.Network;

namespace SS31.Client
{
	public class SSClient : Game
	{
		public GraphicsDeviceManager Graphics { get; private set; }
		public SpriteBatch SpriteBatch { get; private set; }

		private Profiler _profiler;
		private SceneManager _sceneManager;
		private InputManager _inputManager;

		private NetManager _netManager;

		public SSClient() :
			base()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// This will create the Logger and Profiler instances
			ServiceManager.Resolve<Logger>();
			_profiler = ServiceManager.Resolve<Profiler>();
		}

		protected override void Initialize()
		{
			_profiler.BeginInterval();
			_profiler.BeginBlock("Initialization");
			Logger.LogInfo("Initializing the game.");

			(_sceneManager = ServiceManager.Resolve<SceneManager>()).Initialize(this);
			_inputManager = ServiceManager.Resolve<InputManager>();
			_netManager = ServiceManager.Resolve<NetManager>();

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
			_sceneManager.Update(gameTime);

			base.Update(gameTime);
			_profiler.EndBlock();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_profiler.BeginBlock("BaseDraw");

			_sceneManager.Draw(gameTime);

			base.Draw(gameTime);
			_profiler.EndBlock();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			Logger.LogInfo("Exiting the game.");

			// TODO: Cleanup things that require cleaning
			ServiceManager.UnregisterService<SceneManager>(); // Clean up the scene manager first, to release objects that other managers may need released to shut down

			ServiceManager.UnregisterAll();
			base.OnExiting(sender, args);
		}
	}
}