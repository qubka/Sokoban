using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class AnimatedRenderSystem : EntityDrawSystem {
		private GraphicsDevice _graphicsDevice;
		private SpriteBatch _spriteBatch;
		private ComponentMapper<AnimatedSprite> _spriteMapper;
		private ComponentMapper<Transform2> _transformMapper;

		public AnimatedRenderSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(AnimatedSprite), typeof(Transform2))) {
			_graphicsDevice = graphicsDevice;
			_spriteBatch = new SpriteBatch(graphicsDevice);
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_spriteMapper = mapperService.GetMapper<AnimatedSprite>();
			_transformMapper = mapperService.GetMapper<Transform2>();
		}

		public override void Draw(GameTime gameTime) {
			_spriteBatch.Begin();

			foreach (var entityId in ActiveEntities) {
				var transform = _transformMapper.Get(entityId);
				var sprite = _spriteMapper.Get(entityId);

				var texture = sprite.CurrentFrame(gameTime);
				
				var rect = new Rectangle(0, 0, texture.Width, texture.Height);
				var extent = new Vector2(texture.Width, texture.Height) / 2.0f;
				_spriteBatch.Draw(texture, transform.Position + extent, rect, Color.White, transform.Rotation, extent, 1, SpriteEffects.None, 0);
			}

			_spriteBatch.End();
		}
	}
}