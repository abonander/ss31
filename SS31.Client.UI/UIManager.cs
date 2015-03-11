using System;
using System.Collections.Generic;
using SS31.Common;
using SS31.Common.Service;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI
{
	public class UIManager : GameService
	{
		#region Members
		internal RenderManager RenderManager { get; private set; }

		internal List<Widget> ManagedWidgets { get; private set; } // A list of the base widgets, as the starting point for the update and draw calls
		#endregion

		public UIManager(GraphicsDevice device)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			RenderManager = new RenderManager(device);
		}

		public void Update(GameTime gameTime)
		{
			
		}

		// Call this before clearing the main screen
		public void Predraw()
		{
			RenderManager.PreDraw(ManagedWidgets);
		}

		// Call this after everything else in the frame is done drawing
		public void Draw()
		{
			RenderManager.Draw();
		}

		#region Dispose
		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				RenderManager.Dispose();
			}

			base.Dispose(disposing);
		}
		#endregion
	}
}