using Microsoft.Xna.Framework;

namespace Sokoban {
	public class Text {
		public Text(string text = null) {
			Str = text;
			Color = Color.White;
		}

		public string Str { get; set; }
		public Color Color { get; set; }
		public bool IsCentered { get; set;  }
	}
}