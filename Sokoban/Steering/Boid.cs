using Microsoft.Xna.Framework;

namespace Sokoban {
	public class Boid {
		public Boid(float maxSpeed, float maxAccel) {
			MaxSpeed = maxSpeed;
			MaxAccel = maxAccel;
			Velocity = Vector2.Zero;
		}
		
		public float MaxSpeed { get; set; }
		public float MaxAccel { get; set; }
		public Vector2 Velocity { get; set; }
		//public Vector2 Rotation { get; set; }
	}
}