using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class BoardRenderSystem : EntityDrawSystem {
		private GraphicsDevice _graphicsDevice;
		private SpriteBatch _spriteBatch;
		private ComponentMapper<Board> _boardMapper;
		
		private readonly Texture2D _wall;
		private readonly Texture2D _player;
		private readonly Texture2D _box;
		private readonly Texture2D _boxOnGoal;
		private readonly Texture2D _goal;
		private readonly Texture2D _background1;
		private readonly Texture2D _background2;

		//private Vector2 _playerPosition = new Vector2(float.NaN);
		//private Vector2 _playerVelocity;

		public BoardRenderSystem(GraphicsDevice graphicsDevice)
			: base(Aspect.All(typeof(Board))) {
			_graphicsDevice = graphicsDevice;
			_spriteBatch = new SpriteBatch(graphicsDevice);

			_wall = Utils.LoadTexture(graphicsDevice, "Content/textures/Wall.png");
			_player = Utils.LoadTexture(graphicsDevice, "Content/textures/Player.png");
			_box = Utils.LoadTexture(graphicsDevice, "Content/textures/Crate.png");
			_boxOnGoal = Utils.LoadTexture(graphicsDevice, "Content/textures/CrateOnGoal.png");
			_goal = Utils.LoadTexture(graphicsDevice, "Content/textures/EndPoint.png"); // https://github.com/Reesa23/Sokoban
			_background1 = Utils.LoadTexture(graphicsDevice, "Content/textures/Background1.png");
			_background2 = Utils.LoadTexture(graphicsDevice, "Content/textures/Background2.png"); // https://opengameart.org/content/simple-seamless-tiles-of-dirt-and-sand
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_boardMapper = mapperService.GetMapper<Board>();
		}

		public override void Draw(GameTime gameTime) {
			var bounds = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
			
			_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap);
			_spriteBatch.Draw(ActiveEntities.Count > 0 ? _background1 : _background2,  Vector2.Zero, bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
			_spriteBatch.End();
			
			_spriteBatch.Begin();

			foreach (var entityId in ActiveEntities) {
				var board = _boardMapper.Get(entityId);

				var size = _wall.Width;
				var width = board.Columns * size;
				var height = board.Rows * size;
				var offset = new Vector2((bounds.Width - width) / 2 + bounds.X,  (bounds.Height - height) / 2 + bounds.Y);

				var rotation = board.LastMove switch {
					Move.Down => MathHelper.ToRadians(180),
					Move.Left => MathHelper.ToRadians(-90),
					Move.Right => MathHelper.ToRadians(90),
					_ => 0
				};
				
				var player = Vector2.Zero;

				for (var r = 0; r < board.Squares.Length; r++)
					for (var c = 0; c < board.Squares[r].Length; c++) {
						var position = new Vector2(c * size + offset.X, r * size + offset.Y);
						switch (board.Squares[r][c]) {
							case Board.WALL:
								_spriteBatch.Draw(_wall, position, Color.White);
								break;
							case Board.PLAYER:
								player = position;
								break;
							case Board.PLAYER_ON_GOAL:
								player = position;
								_spriteBatch.Draw(_goal, position, Color.White);
								break;
							case Board.BOX:
								_spriteBatch.Draw(_box, position, Color.White);
								break;
							case Board.BOX_ON_GOAL:
								_spriteBatch.Draw(_boxOnGoal, position, Color.White);
								break;
							case Board.GOAL:
								_spriteBatch.Draw(_goal, position, Color.White);
								break;
						}
					}
				
				var rect = new Rectangle(0, 0, _player.Width, _player.Height);
				var extent = new Vector2(_player.Width, _player.Height) / 2.0f;
				//_playerPosition = Utils.SmoothDamp(_playerPosition, player + extent, ref _playerVelocity, 0.02f, 10.0f, gameTime.ElapsedGameTime.Milliseconds);
				_spriteBatch.Draw(_player, player + extent, rect, Color.White, rotation, extent, 1, SpriteEffects.None, 0);
			}

			_spriteBatch.End();
		}
	}
}