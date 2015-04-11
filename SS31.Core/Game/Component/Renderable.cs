using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Core.Game.Component
{
	public class Renderable
	{
		
		private Texture2D texture;

		public Texture2D Texture 
		{
			get { return this.texture; }
			set { this.texture = value; }
		}

		public Renderable (Texture2D texture)
		{
			this.texture = texture;
		}
	}
}

