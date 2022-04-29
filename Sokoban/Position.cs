namespace Sokoban {
    public struct Position {
        public int Row { get; }
        public int Column { get; }

        public Position(int row, int column) {
            Row = row;
            Column = column;
        }

        public bool IsValid() {
            return Row >= 0 && Column >= 0;
        }

        public Position MakeMove(Move move) {
            var r = 0;
            var c = 0;

            switch (move) {
                case Move.Up:
                    r = -1;
                    break;
                case Move.Down:
                    r = 1;
                    break;
                case Move.Left:
                    c = -1;
                    break;
                case Move.Right:
                    c = 1;
                    break;
            }

            return new Position(Row + r, Column + c);
        }
    }
}