using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Sokoban {
	public class ButtonUpdateSystem : EntityUpdateSystem {
		private GraphicsDevice _graphicsDevice;
		private ComponentMapper<Button> _buttonMapper;
		private ComponentMapper<Transform2> _transformMapper;
		private ComponentMapper<Sprite> _spriteMapper;
		
		public ButtonUpdateSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(Button), typeof(Transform2), typeof(Sprite))) {
			_graphicsDevice = graphicsDevice;
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_buttonMapper = mapperService.GetMapper<Button>();
			_transformMapper = mapperService.GetMapper<Transform2>();
			_spriteMapper = mapperService.GetMapper<Sprite>();
		}

		public override void Update(GameTime gameTime) {
			var bounds = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
			var mouseState = Mouse.GetState();
			
			foreach (var entityId in ActiveEntities) {
				var button = _buttonMapper.Get(entityId);
				if (!button.IsActive)
					continue;
				var transform = _transformMapper.Get(entityId);
				var sprite = _spriteMapper.Get(entityId);
				
				button.PreviousMouse = button.CurrentMouse;
				button.CurrentMouse = mouseState;
				
				var offset = button.IsCentered ? new Vector2(bounds.Width / 2f, bounds.Height / 2f) : Vector2.Zero;

				var mouseRectangle = new Rectangle(button.CurrentMouse.X, button.CurrentMouse.Y, 1, 1);
				var buttonRectangle = new Rectangle((int) (transform.Position.X + offset.X), (int) (transform.Position.Y + offset.Y), sprite.TextureRegion.Width, sprite.TextureRegion.Height);
				
				if (mouseRectangle.Intersects(buttonRectangle)) {
					button.IsHovering = true;

					if (button.CurrentMouse.LeftButton == ButtonState.Released &&
					    button.PreviousMouse.LeftButton == ButtonState.Pressed) {
						button.OnClick();
					}
				} else {
					button.IsHovering = false;
				}
			}
		}
	}
}