using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class BoidUpdateSystem : EntityUpdateSystem {
		private ComponentMapper<Steering> _steeringMapper;
		private ComponentMapper<Boid> _boidMapper;
		private ComponentMapper<Transform2> _transformMapper;
		
		public BoidUpdateSystem() 
			: base(Aspect.All(typeof(Steering), typeof(Boid), typeof(Transform2))) {
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_steeringMapper = mapperService.GetMapper<Steering>();
			_boidMapper = mapperService.GetMapper<Boid>();
			_transformMapper = mapperService.GetMapper<Transform2>();
		}

		public override void Update(GameTime gameTime) {
			var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			foreach (var entityId in ActiveEntities) {
				var steering = _steeringMapper.Get(entityId);
				var boid = _boidMapper.Get(entityId);
				var transform = _transformMapper.Get(entityId);
				
				var displacement = boid.Velocity * deltaTime;
				transform.Position += displacement;
				
				boid.Velocity += steering.Linear * deltaTime;
				//boid.Rotation += steering.Angular * deltaTime;

				var length = boid.Velocity.Length();
				if (length > boid.MaxSpeed) {
					boid.Velocity *= 1f / length;
					boid.Velocity *= boid.MaxSpeed;
				}

				if (steering.Linear.LengthSquared() == 0f) {
					boid.Velocity = Vector2.Zero;
				}
        
				steering.Linear = Vector2.Zero;
				//steering.Angular = Vector2.Zero;
			}
		}
	}
}