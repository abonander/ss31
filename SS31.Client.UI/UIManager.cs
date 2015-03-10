using System;
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

		public void Draw()
		{
			RenderManager.Draw();
		}

		#region Dispose
		public override void Dispose(bool disposing)
		{
			// TODO: Dispose things, I suppose

			base.Dispose(disposing);
		}
		#endregion
	}
}