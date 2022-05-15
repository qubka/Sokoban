using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Sokoban {
	public static class Utils {
		public const float KEpsilonNormalSqrt = 1e-15F;
		
		public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, string path) => Texture2D.FromStream(graphicsDevice, new FileStream(path, FileMode.Open));

		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime,
			float maxSpeed, float deltaTime) {
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
				var mag = (float) Math.Sqrt(sqrMagnitude);
				change /= mag * maxChange;
			}

			target = current - change;

			var temp = (currentVelocity + omega * change) * deltaTime;
			currentVelocity = (currentVelocity - omega * temp) * exp;
			var output = target + (change + temp) * exp;

			// Prevent overshooting
			var origMinusCurrent = originalTo - current;
			var outMinusOrig = output - originalTo;

			if (Vector2.Dot(origMinusCurrent, outMinusOrig) > 0f) {
				currentVelocity = Vector2.Zero;
			}

			return output;
		}

		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime,
			float maxSpeed, float deltaTime) {
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
				var mag = (float) Math.Sqrt(sqrMagnitude);
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

		public static float Angle(Vector3 from, Vector3 to) {
			// sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
			var denominator = MathF.Sqrt(from.LengthSquared() * to.LengthSquared());
			if (denominator < KEpsilonNormalSqrt)
				return 0f;

			var dot = Math.Clamp(Vector3.Dot(from, to) / denominator, -1f, 1f);
			return MathHelper.ToDegrees(MathF.Acos(dot));
		}

		public static float Angle(Vector2 from, Vector2 to) {
			// sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
			var denominator = MathF.Sqrt(from.LengthSquared() * to.LengthSquared());
			if (denominator < KEpsilonNormalSqrt)
				return 0f;

			var dot = Math.Clamp(Vector2.Dot(from, to) / denominator, -1f, 1f);
			return MathHelper.ToDegrees(MathF.Acos(dot));
		}
		
		public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis) {
			var unsignedAngle = Angle(from, to);
			var crossX = from.Y * to.Z - from.Z * to.Y;
			var crossY = from.Z * to.X - from.X * to.Z;
			var crossZ = from.X * to.Y - from.Y * to.X;
			var sign = MathF.Sign(axis.X * crossX + axis.Y * crossY + axis.Z * crossZ);
			return unsignedAngle * sign;
		}

		public static float SignedAngle(Vector2 from, Vector2 to) {
			var unsignedAngle = Angle(from, to);
			float sign = MathF.Sign(from.X * to.Y - from.Y * to.X);
			return unsignedAngle * sign;
		}
	}
}