using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		private Entity _scoreTable;
		private Dictionary<string, Entity> _mainMenu;
		private SQLiteConnection _dbConnection;
		private TimeSpan _lastUpdate;

		public Main() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = "Sokoban";
			Window.AllowUserResizing = true;
		}

		protected override void Initialize() {
			_world = new WorldBuilder()
				.AddSystem(new SeekingUpdateSystem())
				.AddSystem(new ArrivalUpdateSystem())
				.AddSystem(new BoidUpdateSystem())
				.AddSystem(new BoardUpdateSystem())
				.AddSystem(new ButterflyUpdateSystem(GraphicsDevice))
				.AddSystem(new BoardRenderSystem(GraphicsDevice))
				.AddSystem(new AnimatedRenderSystem(GraphicsDevice))
				.AddSystem(new TextRenderSystem(GraphicsDevice))
				.AddSystem(new ButtonUpdateSystem(GraphicsDevice))
				.AddSystem(new ButtonRenderSystem(GraphicsDevice))
				.Build();
			base.Initialize();
		}

		protected override void LoadContent() {
			const string database = "Content/data/HighScore.sqlite";
			if (!File.Exists(database)) SQLiteConnection.CreateFile(database);
			_dbConnection = new SQLiteConnection($"Data Source=\"{database}\";Version=3;");
			_dbConnection.Open();
			
			const string sql = "CREATE TABLE IF NOT EXISTS `highscores` " +
		                       "(`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
		                       "`datetime` TEXT NOT NULL DEFAULT ''," +
		                       "`level`INTEGER NOT NULL DEFAULT -1," +
		                       "`moves` INTEGER NOT NULL DEFAULT 0," +
		                       "`pushes` INTEGER NOT NULL DEFAULT 0," +
		                       "`duration` INTEGER NOT NULL DEFAULT 0);";
			var command = new SQLiteCommand(sql, _dbConnection);
			command.ExecuteNonQuery();
			
			var files = Directory.GetFiles("Content/data/", "*.bin");
			_levelCache = new Dictionary<int, Board.Cache>(files.Length);
			foreach (var file in files) {
				var id = Path.GetFileNameWithoutExtension(file);
				_levelCache[int.Parse(id)] = Tools.Deserialize<Board.Cache>(File.Open(file, FileMode.Open));
			}
			
			_levelBase = File.ReadAllText("Content/data/Levels.txt", Encoding.ASCII).Split("---------------");
			_levelMenu = new List<Entity>(_levelBase.Length);
			
			var buttonTexture = Utils.LoadTexture(GraphicsDevice, "Content/textures/Button.png");
			
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

			var butterflySize = Directory.GetFiles("Content/textures/Butterfly", "*.png").Length;
			var butterflyTextures = new List<Texture2D>(butterflySize * 2);
			for (var i = 0; i < butterflySize; i++) {
				butterflyTextures.Add(Utils.LoadTexture(GraphicsDevice, "Content/textures/Butterfly/" + i + ".png"));
			}
			for (var i = butterflySize - 1; i != -1; i--) {
				butterflyTextures.Add(Utils.LoadTexture(GraphicsDevice, "Content/textures/Butterfly/" + i + ".png"));
			}

			var rnd = new Random();
			for (var i = 0; i < rnd.Next(8, 12); i++) {
				var butterfly = _world.CreateEntity();
				var viewport = GraphicsDevice.Viewport;
				butterfly.Attach(new Transform2(rnd.Next(0, viewport.Width - 100), rnd.Next(0, viewport.Height - 100)));
				butterfly.Attach(new Steering());
				butterfly.Attach(new Boid(rnd.Next(130, 150), 1000.0f));
				butterfly.Attach(new Seeking(1f) {
					Target = new Vector2(rnd.Next(0, viewport.Width - 100), rnd.Next(0, viewport.Height - 100))
				});
				butterfly.Attach(new AnimatedSprite(butterflyTextures.ToArray(), 0.05f));
				butterfly.Attach(new Butterfly());
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
			
			_scoreTable = _world.CreateEntity();
			_scoreTable.Attach(new Transform2(-300f, -300f));
			_scoreTable.Attach(new Text() { IsCentered = true });
		}
		
		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				QuitButton_Click(this, EventArgs.Empty);
			}
			
			if (_board != null) {
				var board = _board.Get<Board>();
				var moves = board.Moves;
				var pushes = board.Pushes;
				var duration = (gameTime.TotalGameTime - board.StartedTime).Seconds;
				
				if (board.IsSolved()) {
					SolveBoard(moves, pushes, duration);
				} else {
					_board.Get<Text>().Str = $"Moves: {moves} Pushes: {pushes} Duration: {duration}";
				}
			}

			_lastUpdate = gameTime.TotalGameTime;
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
			_board.Attach(
				loadFromCache && _levelCache.ContainsKey(_currentLevel) 
				? new Board(_levelCache[_currentLevel])
				: new Board(_levelBase[_currentLevel])
			);
			_board.Attach(new Transform2(200f, 0f));
			_board.Attach(new Text());
		}

		private void SaveBoard() {
			if (_currentLevel != -1 && _board != null) {
				var board = _board.Get<Board>();
				var cache = new Board.Cache {
					Squares = board.Squares, 
					Moves = board.Moves, 
					Pushes = board.Pushes, 
					MoveList = board.MoveList,
					Duration = (_lastUpdate - board.StartedTime).Seconds
				};
				_levelCache[_currentLevel] = cache;
				Tools.Serialize(cache, File.Open($"Content/data/{_currentLevel}.bin", FileMode.OpenOrCreate));
			}
		}

		private void SolveBoard(int moves, int pushes, int duration) {
			var now = DateTime.Now;
			var datetime = now.ToString("yyyy-MM-dd HH:mm:ss");

			var sql = $"INSERT INTO `highscores` (`datetime`, `level`, `moves`, `pushes`, `duration`) VALUES ('{datetime}', {_currentLevel}, {moves}, {pushes}, {duration});";
			var command = new SQLiteCommand(sql, _dbConnection);
			command.ExecuteNonQuery();

			sql = $"SELECT * FROM `highscores` WHERE `level` = {_currentLevel} ORDER BY `duration`";
			command = new SQLiteCommand(sql, _dbConnection);

			var data = "Score Table\n";
			var reader = command.ExecuteReader();

			var i = 0;
			var moreResults = true;
			while (moreResults && i < 9) {
				while (reader.Read()) {
					data += $"{reader["datetime"]} | Moves: {reader["moves"]} Pushes: {reader["pushes"]} Duration: {reader["duration"]}" + "\n";
				}
				moreResults = reader.NextResult();
				i++;
			}
			
			_scoreTable.Get<Text>().Str = data;
			
			_world.DestroyEntity(_board);
			_board = null;
			
			_mainMenu["save"].Get<Button>().IsActive = false;
		}
		
		private void PlayButton_Click(object sender, EventArgs e) {
			foreach (var entityId in _mainMenu.Values)
				entityId.Get<Button>().IsActive = false;
			foreach (var entityId in _levelMenu)
				entityId.Get<Button>().IsActive = true;
		}

		private void QuitButton_Click(object sender, EventArgs e) {
			_dbConnection.Close();
			Exit();
		}
		
		private void SaveButton_Click(object sender, EventArgs e) {
			SaveBoard();
		}
        
		private void RestartButton_Click(object sender, EventArgs e) {
			if (_board != null) {
				_world.DestroyEntity(_board);
				_board = null;
			}
			_scoreTable.Get<Text>().Str = null;
			CreateBoard();
		}

		private void MainButton_Click(object sender, EventArgs e) {
			if (_board != null) {
				_world.DestroyEntity(_board);
				_board = null;
				_currentLevel = -1;
			}
			_scoreTable.Get<Text>().Str = null;

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
	}
}