using Microsoft.Xna.Framework;

namespace Sokoban {
	public class Arrival {
		public Arrival(float weight) {
			Weight = weight;
			SlowRadius = 1.0f;
			TargetRadius = 0.1f;
			TimeToTarget = 0.1f;
		}

		public float Weight { get; set; }
		public Vector2 Target { get; set; }
		public float SlowRadius { get; set; }
		public float TargetRadius { get; set; }
		public float TimeToTarget { get; set; }
	}
}