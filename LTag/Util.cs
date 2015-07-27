using System;
using System.Drawing;

namespace LTag
{
	static class Util
	{
		public static void Dispose<T>(ref T disposable) where T: class, IDisposable
		{
			if (disposable == null) return;
			disposable.Dispose();
			disposable = default(T);
		}

		public static PointF Rescale(this PointF pt, float mulX, float mulY)
		{
			return new PointF(pt.X * mulX, pt.Y * mulY);
		}

		public static PointF Center(this Rectangle rect)
		{
			return new PointF(
				rect.X + rect.Width * 0.5f,
				rect.Y + rect.Height * 0.5f
			);
		}

		public static float DistanceSqr(PointF p1, PointF p2)
		{
			var dx = (p1.X - p2.X);
			var dy = (p1.Y - p2.Y);
			return dx * dx + dy * dy;
		}
		
		public static PointF LerpTo(this PointF p1, PointF p2, float val)
		{
			var iVal = 1f - val;
			return new PointF(
				p1.X * iVal + p2.X * val,
				p1.Y * iVal + p2.Y * val
			);
		}

	}
}
