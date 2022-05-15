using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;

namespace Sokoban {
	public class AnimatedSprite {
		public AnimatedSprite(Texture2D[] frames, float framesPerSecond) {
			Frames = frames;
			FramesPerSecond = framesPerSecond;
		}
		
		public Texture2D[] Frames { get; set; }
		public float FramesPerSecond { get; set; }

		public Texture2D CurrentFrame(GameTime gameTime) {
			var index = (int) (gameTime.TotalGameTime.Milliseconds * FramesPerSecond) % Frames.Length; 
			return Frames[index];
		}
	}
}