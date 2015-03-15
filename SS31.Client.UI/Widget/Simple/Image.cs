using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI.Widgets
{
	public class Image : Widget
	{
		#region Members
		public Texture2D Icon { get; set; }
		#endregion

		public Image(Texture2D tex, string ident = null) : base(ident) 
		{
			Icon = tex;
		}

		public override void Update(GameTime gameTime)
		{

		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Icon, AbsoluteInnerArea, Color.White);
		}
	}
}