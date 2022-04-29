using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class ArrivalUpdateSystem : EntityUpdateSystem {
		private ComponentMapper<Arrival> _arrivalMapper;
		private ComponentMapper<Steering> _steeringMapper;
		private ComponentMapper<Boid> _boidMapper;
		private ComponentMapper<Transform2> _transformMapper;
		
		public ArrivalUpdateSystem() 
			: base(Aspect.All(typeof(Arrival), typeof(Steering), typeof(Boid), typeof(Transform2))) {
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_arrivalMapper = mapperService.GetMapper<Arrival>();
			_steeringMapper = mapperService.GetMapper<Steering>();
			_boidMapper = mapperService.GetMapper<Boid>();
			_transformMapper = mapperService.GetMapper<Transform2>();
		}

		public override void Update(GameTime gameTime) {
			foreach (var entityId in ActiveEntities) {
				var arrival = _arrivalMapper.Get(entityId);
				var steering = _steeringMapper.Get(entityId);
				var boid = _boidMapper.Get(entityId);
				var transform = _transformMapper.Get(entityId);
				
				var direction = arrival.Target - transform.Position;
				var distance = direction.Length();

				// If we close, do nothing
				if (distance < arrival.TargetRadius)
					return;

				float targetSpeed;
				if (distance > arrival.SlowRadius) { 
					targetSpeed = boid.MaxSpeed;
				} else {
					targetSpeed = boid.MaxSpeed * distance / arrival.SlowRadius;
				}

				direction *= 1f / distance; // Normalize
				direction *= targetSpeed;
				direction -= boid.Velocity;
				direction /= arrival.TimeToTarget;

				var length = direction.Length();
				if (length > boid.MaxAccel) {
					direction *= 1f / length; // Normalize
					direction *= boid.MaxAccel;
				}
                    
				steering.Linear += (arrival.Weight * direction);
			}
		}
	}
}