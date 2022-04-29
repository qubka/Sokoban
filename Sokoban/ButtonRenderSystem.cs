using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Sokoban {
	public class ButtonRenderSystem : EntityDrawSystem {
		private GraphicsDevice _graphicsDevice;
		private SpriteBatch _spriteBatch;
		private FontSystem _fontSystem;
		private DynamicSpriteFont _font;
		
		private ComponentMapper<Button> _buttonMapper;
		private ComponentMapper<Transform2> _transformMapper;
		private ComponentMapper<Sprite> _spriteMapper;

		public ButtonRenderSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(Button), typeof(Transform2), typeof(Sprite))) {
			_graphicsDevice = graphicsDevice;
			_spriteBatch = new SpriteBatch(graphicsDevice);
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_buttonMapper = mapperService.GetMapper<Button>();
			_transformMapper = mapperService.GetMapper<Transform2>();
			_spriteMapper = mapperService.GetMapper<Sprite>();
			
			_fontSystem = new FontSystem();
			_fontSystem.AddFont(File.ReadAllBytes("Content/fonts/Roboto-Black.ttf"));
			_font = _fontSystem.GetFont(30);
		}

		public override void Draw(GameTime gameTime) {
			var bounds = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
			
			_spriteBatch.Begin();

			foreach (var entityId in ActiveEntities) {
				var button = _buttonMapper.Get(entityId);
				if (!button.IsActive)
					continue;
				
				var transform = _transformMapper.Get(entityId);
				var sprite = _spriteMapper.Get(entityId);
				
				var offset = button.IsCentered ? new Vector2(bounds.Width / 2f, bounds.Height / 2f) : Vector2.Zero;
				var rectangle = new Rectangle((int) (transform.Position.X + offset.X), (int) (transform.Position.Y + offset.Y), sprite.TextureRegion.Width, sprite.TextureRegion.Height);

				_spriteBatch.Draw(sprite.TextureRegion.Texture, rectangle, button.IsHovering ? Color.Gray : Color.White);

				if (!string.IsNullOrEmpty(button.Text)) {
					var x = (rectangle.X + (rectangle.Width / 2)) - (_font.MeasureString(button.Text).X / 2);
					var y = (rectangle.Y + (rectangle.Height / 2)) - (_font.MeasureString(button.Text).Y / 2);

					_spriteBatch.DrawString(_font, button.Text, new Vector2(x, y), button.Color);
				}
			}

			_spriteBatch.End();
		}
	}
}