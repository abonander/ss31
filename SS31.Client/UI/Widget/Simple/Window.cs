using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SS31.Common;

namespace SS31.Client.UI.Widgets
{
	// The class that represents a gui window, which can
	public class Window : WidgetContainer
	{
		#region Members
		public BorderedTexture WindowTexture { get; set; }
		#endregion

		public Window()
		{
			// WindowTexture = new BorderedTexture(RenderManager.Instance.Content.Load<Texture2D>(@"GUI\window_default"), 6, 6, 12, 6);
		}

		public override void Draw(SpriteBatch batch)
		{
			if (WindowTexture != null)
				WindowTexture.Draw(batch, AbsoluteOuterArea);
		}
	}
}