using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SS31.Client.UI
{
	public class RenderManager : IDisposable
	{
		#region Members
		private bool _disposed;

		private int _stencilDepth; // The stencil buffer depth, used to mask ui elements
		private Texture2D _maskTexture; // The texture that is used to mask the stencil buffer

		private RenderTarget2D _renderTarget; // The target to render to GUI onto

		internal GraphicsDevice GraphicsDevice { get; private set; } // The graphics device for the application
		internal SpriteBatch SpriteBatch { get; private set; } // The spritebatch used specifically by this library

		public Rectangle Viewport { get { return GraphicsDevice.Viewport.Bounds; } } // Get the bounds of the viewport
		#endregion

		public RenderManager(GraphicsDevice device)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			GraphicsDevice = device;
			SpriteBatch = new SpriteBatch(device);

			_renderTarget = new RenderTarget2D(
				device,
				device.PresentationParameters.BackBufferWidth,
				device.PresentationParameters.BackBufferHeight,
				false,
				device.PresentationParameters.BackBufferFormat,
				DepthFormat.Depth24Stencil8
			);

			_stencilDepth = 0;
			_maskTexture = new Texture2D(device, 1, 1);
			_maskTexture.SetData(new []{ Color.Black });

			_disposed = false;
		}
		~RenderManager()
		{
			Dispose(false);
		}

		// This generates the GUI as a separate texture from a render target. Call this before you clear the
		// client screen.
		public void PreDraw(List<Widget> widgets)
		{
			GraphicsDevice.SetRenderTarget(_renderTarget);
			GraphicsDevice.Clear(Color.Transparent);

			foreach (Widget w in widgets)
			{
				SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

				w.Draw(SpriteBatch);

				SpriteBatch.End();
			}

			GraphicsDevice.SetRenderTarget(null);
		}

		// Draw the gui as a single texture over the current contents of the screen
		public void Draw()
		{
			SpriteBatch.Begin();
			SpriteBatch.Draw((Texture2D)_renderTarget, Vector2.Zero, Color.White);
			SpriteBatch.End();
		}

		#region Stencil Buffer Functions
		internal void PushStencilBuffer(Rectangle rect)
		{
			if (_stencilDepth++ == 0)
				GL.Enable(EnableCap.StencilTest);

			GL.ColorMask(false, false, false, false);
			GL.DepthMask(false);
			GL.StencilFunc(StencilFunction.Always, _stencilDepth, _stencilDepth);
			GL.StencilOp(StencilOp.Incr, StencilOp.Incr, StencilOp.Incr);

			SpriteBatch.Draw(_maskTexture, rect, Color.White);

			GL.ColorMask(true, true, true, true);
			GL.DepthMask(true);
			GL.StencilFunc(StencilFunction.Equal, _stencilDepth, _stencilDepth);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
		}

		internal void PopStencilBuffer(Rectangle rect)
		{
			GL.ColorMask(false, false, false, false);
			GL.DepthMask(false);
			GL.StencilFunc(StencilFunction.Always, _stencilDepth, _stencilDepth);
			GL.StencilOp(StencilOp.Decr, StencilOp.Decr, StencilOp.Decr);

			SpriteBatch.Draw(_maskTexture, rect, Color.White);

			GL.ColorMask(true, true, true, true);
			GL.DepthMask(true);

			if (--_stencilDepth == 0)
				GL.Disable(EnableCap.StencilTest);
		}
		#endregion

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