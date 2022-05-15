using System;
using System.Linq;
using System.Text;

namespace Sokoban {
   public class Board {
       #region Defines

       public const char WALL = '#';
       public const char PLAYER = '@';
       public const char PLAYER_ON_GOAL = '+';
       public const char BOX = '$';
       public const char BOX_ON_GOAL = '*';
       public const char GOAL = '.';
       public const char FLOOR = ' ';
       public const char COIN = '%';

       #endregion

       #region Private Members

       // The player's position
       private Position _player;

       // The moves that have been made
       private readonly StringBuilder _moveList = new StringBuilder();
       
       // The board that have been modify
       //private readonly Queue<char[][]> _boardCache = new Queue<char[][]>();

       #endregion

       #region Properties

       public char[][] Squares { get; private set; }
       public string MoveList => _moveList.ToString();
       public int Moves { get; private set; }
       public int Pushes { get; private set; }
       public int Rows { get; private set; }
       public int Columns { get; private set; }
       public TimeSpan StartedTime { get; set; }
       public int Duration { get; set; }
       public Move LastMove { get; set; }

       #endregion
       
       [Serializable]
       public class Cache {
          public char[][] Squares { get; set; }
          public int Moves { get; set; }
          public int Pushes { get; set; }
          public string MoveList { get; set; }
          public int Duration { get; set; }
       }

       #region Construction

       public Board(string board) {
          Moves = 0;
          Pushes = 0;
          _moveList.Clear();
          //_boardCache.Clear();

          board = board.Replace("\r", "");

          var rows = board.Split(new[] {'\n'});
          Squares = new char[rows.Length][];
          Rows = rows.Length;
          for (var r = 0; r < rows.Length; r++) {
             if (rows[r].Length > Columns)
                Columns = rows[r].Length;

             Squares[r] = rows[r].ToCharArray();
          }

          _player = FindPlayer();

          if (Columns == 0)
             Rows = 0;
          
          //StartedTime = TimeSpan.Zero;
          Duration = 0;
       }
       
       public Board(Cache cache) {
          Moves = cache.Moves;
          Pushes = cache.Pushes;
          _moveList = new StringBuilder(cache.MoveList);
          //_boardCache.Clear();

          Squares = cache.Squares;
          Rows = Squares.Length;
          Columns = Rows > 0 ? Squares[0].Length : 0;

          _player = FindPlayer();

          if (Columns == 0)
             Rows = 0;

          //StartedTime = TimeSpan.Zero;
          Duration = cache.Duration;
       }

       #endregion

       #region Public Methods

       public bool MakeMove(Move move) {
          if (!IsMoveValid(move))
             return false;

          if (IsSolved())
             return false;

          //_boardCache.Enqueue((char[][]) Squares.Clone());
          LastMove = move;
          
          var moveLetter = GetMoveLetter(move);
          
          Squares[_player.Row][_player.Column] = Squares[_player.Row][_player.Column] == PLAYER ? FLOOR : GOAL;

          var newPlayer = _player.MakeMove(move);
          var newSquare = Squares[newPlayer.Row][newPlayer.Column];
          switch (newSquare) {
             case FLOOR:
             case BOX:
                Squares[newPlayer.Row][newPlayer.Column] = PLAYER;
                break;
             case GOAL:
             case BOX_ON_GOAL:
                Squares[newPlayer.Row][newPlayer.Column] = PLAYER_ON_GOAL;
                break;
          }

          if (newSquare == BOX || newSquare == BOX_ON_GOAL) {
             Pushes++;
             
             moveLetter = char.ToUpperInvariant(moveLetter);
             
             var box = newPlayer.MakeMove(move);
             
             Squares[box.Row][box.Column] = Squares[box.Row][box.Column] == FLOOR ? BOX : BOX_ON_GOAL;
          } else {
             Moves++;
          }

          _player = newPlayer;
          _moveList.Append(moveLetter);
          return true;
       }

       /*public void BackMove() {
          if (_boardCache.Count <= 0) 
             return;

          var o = _boardCache.Dequeue();
          if (o.SequenceEqual(Squares)) Console.WriteLine("same");
          Squares = o;
          
          _player = FindPlayer();
          
          _moveList.Append('b');
       }*/
       
       public bool IsSolved() {
          return Squares.All(r => r.All(c => c != BOX));
       }

       #endregion

       #region Private Methods

       private bool IsMoveValid(Move move) {
          var newPlayer = _player.MakeMove(move);

          if (newPlayer.Row <= 0 ||
              newPlayer.Row >= Squares.Length - 1 ||
              newPlayer.Column <= 0 ||
              newPlayer.Column >= Squares[newPlayer.Row].Length - 1)
             return false;

          var square = Squares[newPlayer.Row][newPlayer.Column];
          switch (square) {
             case WALL:
                return false;
             case FLOOR:
             case GOAL:
                return true;
             case BOX:
             case BOX_ON_GOAL: {
                var box = newPlayer.MakeMove(move);

                if (box.Row < 0 ||
                    box.Row >= Squares.Length ||
                    box.Column < 0 ||
                    box.Column >= Squares[box.Row].Length)
                   return false;

                var boxSquare = Squares[box.Row][box.Column];
                if (boxSquare == FLOOR || boxSquare == GOAL) 
                   return true;
                break;
             }
          }

          return false;
       }

       private static char GetMoveLetter(Move move) {
          return move switch {
             Move.Down => 'd',
             Move.Left => 'l',
             Move.Right => 'r',
             Move.Up => 'u',
             _ => 'b'
          };
       }

       public Position FindPlayer() {
          for (var r = 0; r < Squares.Length; r++)
          for (var c = 0; c < Squares[r].Length; c++)
             if (Squares[r][c] == PLAYER || Squares[r][c] == PLAYER_ON_GOAL)
                return new Position(r, c);
          return new Position(-1, -1);
       }

       #endregion

       #region Overrides

       public override string ToString() {
          var builder = new StringBuilder();
          foreach (var row in Squares) builder.AppendLine(new string(row));
          if (builder.Length >= 2) builder.Remove(builder.Length - 2, 2);
          return builder.ToString();
       }

       #endregion
    }
}