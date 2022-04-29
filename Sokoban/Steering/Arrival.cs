using Microsoft.Xna.Framework;

namespace Sokoban {
	public class Arrival {
		public Arrival(float weight) {
			Weight = weight;
		}

		public float Weight { get; set; }
		public Vector2 Target { get; set; }
		public float SlowRadius { get; set; }
		public float TargetRadius { get; set; }
		public float TimeToTarget { get; set; }
	}
}