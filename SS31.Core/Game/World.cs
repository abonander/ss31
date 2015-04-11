using System;
using Artemis;
using Microsoft.Xna.Framework;

namespace SS31.Core.Game
{
	public class World
	{
		private EntityWorld entityWorld;


		public World ()
		{
			this.entityWorld = new EntityWorld ();
		}

		public void Update(GameTime gameTime)
		{ 
			this.entityWorld.Update(gameTime.ElapsedGameTime.Ticks);			
		}

		public void Draw(GameTime gameTime) 
		{
			this.entityWorld.Draw ();
		}
	}
}

