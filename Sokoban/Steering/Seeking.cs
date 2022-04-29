using Microsoft.Xna.Framework;

namespace Sokoban {
	public class Seeking {
		public Seeking(float weight) {
			Weight = weight;
		}
		
		public float Weight;
		public Vector2 Target;
		public float TargetRadius;
	}
}