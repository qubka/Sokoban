using System;

namespace Sokoban {
	public class Butterfly {
		public Butterfly() {
			State = NpcState.Idle;
			NextThink = TimeSpan.MinValue;
		}
		
		public NpcState State { get; set; }
		public TimeSpan NextThink { get; set; }
	}
}