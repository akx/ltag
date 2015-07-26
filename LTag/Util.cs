using System;
using System.Drawing;

namespace LTag
{
	static class Util
	{
		public static void Dispose<T>(ref T disposable) where T: IDisposable
		{
			if (disposable != null)
			{
				disposable.Dispose();
				disposable = default(T);
			}
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
	}
}
