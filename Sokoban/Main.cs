using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace Sokoban {
	public class Main : Game {
		private GraphicsDeviceManager _graphics;
		private World _world;
		private Entity _board;
		
		private string[] _levelBase;
		private List<Entity> _levelMenu;
		private Dictionary<int, Board.Cache> _levelCache;
		private int _currentLevel = -1;
		
		private Dictionary<string, Entity> _mainMenu;

		public Main() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = "Sokoban";
			Window.AllowUserResizing = true;
		}

		protected override void Initialize() {
			_world = new WorldBuilder()
				.AddSystem(new BoardUpdateSystem())
				.AddSystem(new BoardRenderSystem(GraphicsDevice))
				.AddSystem(new TextRenderSystem(GraphicsDevice))
				.AddSystem(new ButtonUpdateSystem(GraphicsDevice))
				.AddSystem(new ButtonRenderSystem(GraphicsDevice))
				//.AddSystem(new RenderSystem(GraphicsDevice))
				.Build();
			base.Initialize();
		}

		protected override void LoadContent() {
			var buttonTexture = Utils.LoadTexture(GraphicsDevice, "Content/textures/Button.png");


			var files = Directory.GetFiles("Content/data/", "*.bin");
			_levelCache = new Dictionary<int, Board.Cache>(files.Length);
			foreach (var file in files) {
				var id = Path.GetFileNameWithoutExtension(file);
				_levelCache[int.Parse(id)] = Tools.Deserialize<Board.Cache>(File.Open(file, FileMode.Open));
			}
			
			_levelBase = File.ReadAllText("Content/levels.txt", Encoding.ASCII).Split("---------------");
			_levelMenu = new List<Entity>(_levelBase.Length);
			
			var position = new Vector2(0f, 0f);
			for (var i = 1; i < _levelBase.Length; i++) {
				var button = _world.CreateEntity();
				button.Attach(new Transform2(position));
				button.Attach(new Sprite(buttonTexture));
				button.Attach(new Button("Level" + i, LevelButton_Click) {
					IsActive = false,
					Color = _levelCache.ContainsKey(i - 1) ? Color.Purple : Color.Black,
					Data = new ButtonEventArgs((i - 1).ToString())
				});
				
				if (i % 10 == 0) {
					position.X = 0.0f;
					position.Y += buttonTexture.Height + 5.0f;
				} else {
					position.X += buttonTexture.Width + 5.0f;
				}

				_levelMenu.Add(button);
			}

			var playButton = _world.CreateEntity();
			playButton.Attach(new Transform2(-80f, 0f));
			playButton.Attach(new Sprite(buttonTexture));
			playButton.Attach(new Button("Play", PlayButton_Click) { IsCentered = true });
			
			var quitButton = _world.CreateEntity();
			quitButton.Attach(new Transform2(-80f, 40f));
			quitButton.Attach(new Sprite(buttonTexture));
			quitButton.Attach(new Button("Quit", QuitButton_Click) { IsCentered = true });
			
			var mainButton = _world.CreateEntity();
			mainButton.Attach(new Transform2(0f, 0f));
			mainButton.Attach(new Sprite(buttonTexture));
			mainButton.Attach(new Button("Go Back", MainButton_Click) { IsActive = false });

			var restartButton = _world.CreateEntity();
			restartButton.Attach(new Transform2(0f, 40f));
			restartButton.Attach(new Sprite(buttonTexture));
			restartButton.Attach(new Button("Restart", RestartButton_Click) { IsActive = false });
			
			var saveButton = _world.CreateEntity();
			saveButton.Attach(new Transform2(0f, 80f));
			saveButton.Attach(new Sprite(buttonTexture));
			saveButton.Attach(new Button("Save", SaveButton_Click) { IsActive = false });
			
			_mainMenu = new Dictionary<string, Entity> {
				{"play", playButton},
				{"quit", quitButton},
				{"main", mainButton},
				{"restart", restartButton},
				{"save", saveButton}
			};
		}
		
		private void PlayButton_Click(object sender, EventArgs e) {
			foreach (var entityId in _mainMenu.Values)
				entityId.Get<Button>().IsActive = false;
			foreach (var entityId in _levelMenu)
				entityId.Get<Button>().IsActive = true;
		}

		private void QuitButton_Click(object sender, EventArgs e) {
			Exit();
		}
		
		private void SaveButton_Click(object sender, EventArgs e) {
			SaveBoard();
		}
        
		private void RestartButton_Click(object sender, EventArgs e) {
			_world.DestroyEntity(_board);
			_board = null;
			CreateBoard();
		}

		private void MainButton_Click(object sender, EventArgs e) {
			_world.DestroyEntity(_board);
			_board = null;
			_currentLevel = -1;
			
			foreach (var entityId in _levelMenu)
				entityId.Get<Button>().IsActive = true;
			_mainMenu["main"].Get<Button>().IsActive = false;
			_mainMenu["restart"].Get<Button>().IsActive = false;
			_mainMenu["save"].Get<Button>().IsActive = false;
		}
        
		private void LevelButton_Click(object sender, EventArgs e) {
			_currentLevel = int.Parse(((ButtonEventArgs) e).Data);
			CreateBoard(true);
			
			foreach (var entityId in _levelMenu)
				entityId.Get<Button>().IsActive = false;
			_mainMenu["main"].Get<Button>().IsActive = true;
			_mainMenu["restart"].Get<Button>().IsActive = true;
			_mainMenu["save"].Get<Button>().IsActive = true;
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				QuitButton_Click(this, EventArgs.Empty);
			}

			if (_board != null) {
				var board = _board.Get<Board>();
				_board.Get<Text>().Str = $"Moves: {board.Moves} Pushes: {board.Pushes} Duration: {(gameTime.TotalGameTime - board.StartedTime).Seconds}";
				if (board.IsSolved()) {
					MainButton_Click(this, EventArgs.Empty);
				}
			}

			_world.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_world.Draw(gameTime);
			base.Draw(gameTime);
		}

		private void CreateBoard(bool loadFromCache = false) {
			_board = _world.CreateEntity();
			_board.Attach(loadFromCache && _levelCache.ContainsKey(_currentLevel) 
				? new Board(_levelCache[_currentLevel])
				: new Board(_levelBase[_currentLevel]));
			_board.Attach(new Transform2(200f, 0f));
			_board.Attach(new Text());
		}

		private void SaveBoard() {
			if (_currentLevel != -1 && _board != null) {
				var board = _board.Get<Board>();
				var cache = new Board.Cache{ Squares = board.Squares, Moves = board.Moves, Pushes = board.Pushes };
				_levelCache[_currentLevel] = cache;
				Tools.Serialize(cache, File.Open($"Content/data/{_currentLevel}.bin", FileMode.OpenOrCreate));
			}
		}
	}
}