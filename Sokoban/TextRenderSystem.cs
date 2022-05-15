using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class TextRenderSystem : EntityDrawSystem {
		private GraphicsDevice _graphicsDevice;
		private SpriteBatch _spriteBatch;
		private FontSystem _fontSystem;
		private DynamicSpriteFont _font;
		
		private ComponentMapper<Text> _textMapper;
		private ComponentMapper<Transform2> _transformMapper;
		
		public TextRenderSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(Text), typeof(Transform2))) {
			_graphicsDevice = graphicsDevice;
			_spriteBatch = new SpriteBatch(graphicsDevice);
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_textMapper = mapperService.GetMapper<Text>();
			_transformMapper = mapperService.GetMapper<Transform2>();
			
			_fontSystem = new FontSystem();
			_fontSystem.AddFont(File.ReadAllBytes("Content/fonts/Roboto-Black.ttf"));
			_font = _fontSystem.GetFont(30);
		}

		public override void Draw(GameTime gameTime) {
			var bounds = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
			
			_spriteBatch.Begin();

			foreach (var entityId in ActiveEntities) {
				var text = _textMapper.Get(entityId);
				if (string.IsNullOrEmpty(text.Str)) 
					continue;
				
				var offset = text.IsCentered ? new Vector2(bounds.Width / 2f, bounds.Height / 2f) : Vector2.Zero;
				
				var transform = _transformMapper.Get(entityId);
				_spriteBatch.DrawString(_font, text.Str, transform.Position + offset, text.Color);
			}

			_spriteBatch.End();
		}
	}
}