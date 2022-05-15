using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sokoban {
	public class BoardUpdateSystem : EntityUpdateSystem {
		private ComponentMapper<Board> _boardMapper;
		private KeyboardState _oldState;
		
		public BoardUpdateSystem()
			: base(Aspect.All(typeof(Board))) {
		}

		public override void Initialize(IComponentMapperService mapperService) {
			_boardMapper = mapperService.GetMapper<Board>();
			_oldState = Keyboard.GetState();
		}

		public override void Update(GameTime gameTime) {
			var newState = Keyboard.GetState();
			
			foreach (var entityId in ActiveEntities) {
				var board = _boardMapper.Get(entityId);

				if (newState.IsKeyDown(Keys.Up) && !_oldState.IsKeyDown(Keys.Up))
					board.MakeMove(Move.Up);
				else if (newState.IsKeyDown(Keys.Down) && !_oldState.IsKeyDown(Keys.Down))
					board.MakeMove(Move.Down);
				else if (newState.IsKeyDown(Keys.Right) && !_oldState.IsKeyDown(Keys.Right))
					board.MakeMove(Move.Right);
				else if (newState.IsKeyDown(Keys.Left) && !_oldState.IsKeyDown(Keys.Left))
					board.MakeMove(Move.Left);

				if (board.StartedTime == TimeSpan.Zero) {
					board.StartedTime = gameTime.TotalGameTime - new TimeSpan(0, 0, board.Duration);
				}
			}
			
			_oldState = newState;
		}
	}
}