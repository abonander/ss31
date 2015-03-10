using System;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI
{
	internal class RenderManager : IDisposable
	{
		#region Members
		private bool _disposed;

		private RasterizerState _scissorState; // State for activating the scissor test

		internal GraphicsDevice GraphicsDevice { get; private set; } // The graphics device for the application
		internal SpriteBatch SpriteBatch { get; private set; } // The spritebatch used specifically by this library
		#endregion

		public RenderManager(GraphicsDevice device)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			GraphicsDevice = device;
			SpriteBatch = new SpriteBatch(device);

			_scissorState = new RasterizerState { ScissorTestEnable = true };

			_disposed = false;
		}
		~RenderManager()
		{
			Dispose(false);
		}

		public void Draw()
		{
			SpriteBatch.Begin(
				SpriteSortMode.Immediate,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,
				DepthStencilState.None,
				_scissorState
			);

			// TODO: do the rendering

			SpriteBatch.End();

			// Reset Graphics Device settings to default
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		}

		#region Dispose
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				SpriteBatch.Dispose();
				SpriteBatch = null;
				GraphicsDevice = null;
			}

			_disposed = true;
		}
		#endregion
	}
}