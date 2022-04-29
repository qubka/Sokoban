using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Sokoban {
	public static class Utils {
		public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, string path) => Texture2D.FromStream(graphicsDevice, new FileStream(path, FileMode.Open));

		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime,  float maxSpeed, float deltaTime) {
			// Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Math.Max(0.0001f, smoothTime);
            var omega = 2f / smoothTime;

            var x = omega * deltaTime;
            var exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            var change = current - target;
            var originalTo = target;

            // Clamp maximum speed
            var maxChange = maxSpeed * smoothTime;

            var maxChangeSq = maxChange * maxChange;
            var sqrMagnitude = Vector2.Dot(change, change);
            if (sqrMagnitude > maxChangeSq) {
                var mag = (float)Math.Sqrt(sqrMagnitude);
                change /= mag * maxChange;
            }

            target = current - change;

            var temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            var output = target + (change + temp) * exp;

            // Prevent overshooting
            var origMinusCurrent = originalTo - current;
            var outMinusOrig = output - originalTo;

            if (Vector2.Dot(origMinusCurrent, outMinusOrig) > 0f)  {
                currentVelocity = Vector2.Zero;
            }
            return output;
		}

		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
			// Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Math.Max(0.0001f, smoothTime);
            var omega = 2f / smoothTime;

            var x = omega * deltaTime;
            var exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            var change = current - target;
            var originalTo = target;

            // Clamp maximum speed
            var maxChange = maxSpeed * smoothTime;

            var maxChangeSq = maxChange * maxChange;
            var sqrMagnitude = Vector3.Dot(change, change);
            if (sqrMagnitude > maxChangeSq) {
                var mag = (float)Math.Sqrt(sqrMagnitude);
                change /= mag * maxChange;
            }

            target = current - change;

            var temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            var output = target + (change + temp) * exp;

            // Prevent overshooting
            var origMinusCurrent = originalTo - current;
            var outMinusOrig = output - originalTo;

            if (Vector3.Dot(origMinusCurrent, outMinusOrig) > 0f) {
	            currentVelocity = Vector3.Zero;
            }

            return output;
        }
	}
}