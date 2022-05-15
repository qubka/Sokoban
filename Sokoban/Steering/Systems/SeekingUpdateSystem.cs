using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class SeekingUpdateSystem : EntityUpdateSystem {
		private ComponentMapper<Seeking> _seekingMapper;
		private ComponentMapper<Steering> _steeringMapper;
		private ComponentMapper<Boid> _boidMapper;
		private ComponentMapper<Transform2> _transformMapper;
		
		public SeekingUpdateSystem() 
			: base(Aspect.All(typeof(Seeking), typeof(Steering), typeof(Boid), typeof(Transform2))) {
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_seekingMapper = mapperService.GetMapper<Seeking>();
			_steeringMapper = mapperService.GetMapper<Steering>();
			_boidMapper = mapperService.GetMapper<Boid>();
			_transformMapper = mapperService.GetMapper<Transform2>();
		}

		public override void Update(GameTime gameTime) {
			foreach (var entityId in ActiveEntities) {
				var seeking = _seekingMapper.Get(entityId);
				var steering = _steeringMapper.Get(entityId);
				var boid = _boidMapper.Get(entityId);
				var transform = _transformMapper.Get(entityId);
				
				// If target died, do nothing
				//if (seeking.Target == Entity.Null) // Can happens only here, we use it to seek for other units which might die
				//	return;
                    
				var direction = seeking.Target - transform.Position;
				var distance = direction.Length();
                    
				// If we close, do nothing
				if (distance <= 0f)
					return;

				direction *= 1f / distance; // Normalize
				direction *= boid.MaxAccel;
				
				steering.Linear += (seeking.Weight * direction);
			}
		}
	}
}