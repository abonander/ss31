using System;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Core
{
	public interface Sprite
	{

		Texture2D getTexture(Facing direction);
	}


	public enum Facing 
	{
		North = 0,
		NorthEast = 1,
		East = 2,
		SouthEast = 3,
		South = 4,
		SouthWest = 5,
		West = 6,
		NorthWest = 7
	}
}

