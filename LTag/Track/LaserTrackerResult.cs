using System;
using System.Collections.Generic;
using System.Drawing;

namespace LTag.Track
{
	public class LaserTrackerResult: IDisposable
	{
		private Bitmap _threshBitmap;
		private Bitmap _camBitmap;
		private readonly List<Rectangle> _rectangles;
		private readonly TimeSpan _processingTime;

		public Bitmap ThreshBitmap
		{
			get { return _threshBitmap; }
		}

		public List<Rectangle> Rectangles 
		{
			get { return _rectangles; }
		}

		public Bitmap CamBitmap
		{
			get { return _camBitmap; }
		}

		public TimeSpan ProcessingTime
		{
			get { return _processingTime; }
		}

		public LaserTrackerResult(Bitmap camBitmap, Bitmap threshBitmap, List<Rectangle> rectangles, TimeSpan processingTime)
		{
			_camBitmap = camBitmap;
			_threshBitmap = threshBitmap;
			_rectangles = rectangles;
			_processingTime = processingTime;
		}

		public void ApplyDebugToImage()
		{
			if (_threshBitmap == null) return;
			using (var g = Graphics.FromImage(_threshBitmap))
			{
				foreach (var rectangle in _rectangles)
				{
					var cx = (rectangle.Left + rectangle.Right) / 2;
					var cy = (rectangle.Top + rectangle.Bottom) / 2;
					g.DrawEllipse(Pens.Lime, cx - 3, cy - 3, 6, 6);
					g.DrawRectangle(Pens.Orange, rectangle);
				}
			}
		}

		public void Dispose()
		{
			Util.Dispose(ref _threshBitmap);
			Util.Dispose(ref _camBitmap);
		}
	}
}