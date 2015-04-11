using System;
using Artemis.System;
using SS31.Core.Game.Component;

namespace SS31.Core.Game.System
{
	public class RenderingSystem : EntityComponentProcessingSystem<Renderable, Transformable>
	{
		public RenderingSystem () : base(Aspect.All(typeof(Renderable), typeof(Transformable))) {}

		public override void Process (Artemis.Entity entity, Renderable renderable, Transformable transformable)
		{
						
		}
	}
}

