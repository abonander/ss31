using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31
{
	public class SSClient : Game
	{
		public GraphicsDeviceManager Graphics { get; private set; }
		public SpriteBatch SpriteBatch { get; private set; }

		private Profiler _profiler;

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

			// TODO: Initialize things

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

			// TODO: Update things

			base.Update(gameTime);
			_profiler.EndBlock();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_profiler.BeginBlock("BaseDraw");

			// TODO: Draw things

			base.Draw(gameTime);
			_profiler.EndBlock();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			Logger.LogInfo("Exiting the game.");

			// TODO: Cleanup things that require cleaning

			ServiceManager.UnregisterAll();
			base.OnExiting(sender, args);
		}
	}
}