namespace Sokoban {
	public enum Side {
		Forward,
		Left,
		Right,
		Backward,
	}

	public static class Direction {
		public static Side AngleToSide(float angle) {
			var deg = (angle + 180f) % 360f - 180f;

			if (deg <= 67.5f && deg > -67.5f) {
				return Side.Forward;
			}

			if (deg <= -67.5f && deg > -112.5f) {
				return Side.Left;
			}

			if (deg > 67.5f && deg <= 112.5f) {
				return Side.Right;
			}

			return Side.Backward;
		}
	}
}