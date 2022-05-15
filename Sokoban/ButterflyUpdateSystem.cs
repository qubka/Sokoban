using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class ButterflyUpdateSystem : EntityUpdateSystem {
		private GraphicsDevice _graphicsDevice;
		private ComponentMapper<Seeking> _seekingMapper;
		private ComponentMapper<Boid> _boidMapper;
		private ComponentMapper<Transform2> _transformMapper;
		private ComponentMapper<Butterfly> _butterflyMapper;
		
		public ButterflyUpdateSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(Seeking), typeof(Boid), typeof(Transform2), typeof(Butterfly))) {
			_graphicsDevice = graphicsDevice;
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_seekingMapper = mapperService.GetMapper<Seeking>();
			_boidMapper = mapperService.GetMapper<Boid>();
			_transformMapper = mapperService.GetMapper<Transform2>();
			_butterflyMapper = mapperService.GetMapper<Butterfly>();
		}

		public override void Update(GameTime gameTime) {
			var viewport = _graphicsDevice.Viewport;
			var rnd = new Random();

			var current = gameTime.TotalGameTime;
			
			foreach (var entityId in ActiveEntities) {
				var seeking = _seekingMapper.Get(entityId);
				var boid = _boidMapper.Get(entityId);
				var transform = _transformMapper.Get(entityId);
				var butterfly = _butterflyMapper.Get(entityId);

				if (butterfly.NextThink <= current) {
					butterfly.NextThink = current + new TimeSpan(0, 0, rnd.Next(3, 10));
					butterfly.State = rnd.Next(2) != 0 ? NpcState.Idle : NpcState.Move;
				}

				seeking.Weight = butterfly.State == NpcState.Move ? 1f : 0f;

				transform.Rotation = MathF.Atan2(boid.Velocity.X, boid.Velocity.Y);

				if (Vector2.Distance(transform.Position, seeking.Target) <= 1f) {
					seeking.Target = new Vector2(rnd.Next(0, viewport.Width - 100), rnd.Next(0, viewport.Height - 100));
				}
			}
		}
	}
}