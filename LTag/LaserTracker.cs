using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace LTag
{
	class LaserTracker
	{
		private int _hueCenter = 40;
		private int _hueWidth = 10;
		private int _satMin = 200;
		private int _satMax = 255;
		private int _valMin = 100;
		private int _valMax = 255;
		private int _dilate = 0;
		private int _minPixels = 30;
		private int _width = 480;
		private int _height = 320;

		#region Properties
		public int Dilate
		{
			get { return _dilate; }
			set { _dilate = value; }
		}
		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public int MinPixels
		{
			get { return _minPixels; }
			set { _minPixels = value; }
		}

		public int HueCenter
		{
			get { return _hueCenter; }
			set { _hueCenter = value; }
		}

		public int HueWidth
		{
			get { return _hueWidth; }
			set { _hueWidth = value; }
		}

		public int SatMin
		{
			get { return _satMin; }
			set { _satMin = value; }
		}

		public int SatMax
		{
			get { return _satMax; }
			set { _satMax = value; }
		}

		public int ValMin
		{
			get { return _valMin; }
			set { _valMin = value; }
		}

		public int ValMax
		{
			get { return _valMax; }
			set { _valMax = value; }
		}
		#endregion
		
		public LaserTrackerResult UpdateFromFrame(Mat frame)
		{
			Bitmap camBitmap, threshBitmap;
			var rects = new List<Rectangle>();
			using (var threshFrame = new Mat())
			{
				using (var hsvFrame = new Mat())
				{
					using (var resizeFrame = new Mat())
					{
						CvInvoke.Resize(frame, resizeFrame, new Size(_width, _height));
						CvInvoke.CvtColor(resizeFrame, hsvFrame, ColorConversion.Bgr2Hsv);
						camBitmap = resizeFrame.Bitmap.Clone(new Rectangle(0, 0, _width, _height), PixelFormat.Format32bppArgb);
					}
					float hueMin = _hueCenter - _hueWidth;
					float hueMax = _hueCenter + _hueWidth;
					HueThreshold(hueMin, hueMax, hsvFrame, threshFrame);
					if (_dilate > 0)
					{
						CvInvoke.Dilate(threshFrame, threshFrame, null, new Point(-1, -1), _dilate, BorderType.Default, new MCvScalar());
					}

				}
				threshBitmap = threshFrame.Bitmap.Clone(new Rectangle(0, 0, _width, _height), PixelFormat.Format32bppArgb);
				
				using (var dummyFrame = threshFrame.Clone())
				{
					using (var contours = new VectorOfVectorOfPoint())
					{
						CvInvoke.FindContours(dummyFrame, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
						for (var i = 0; i < contours.Size; i++)
						{
							var rect = CvInvoke.BoundingRectangle(contours[i]);
							if (rect.Width*rect.Height < _minPixels) continue;
							rects.Add(rect);
						}
					}
				}
			}
			rects.Sort((r1, r2) =>
			{
				var s1 = r1.Width * r1.Height;
				var s2 = r1.Width * r2.Height;
				if (s1 > s2) return 1;
				if (s1 < s2) return -1;
				return 0;
			});
			return new LaserTrackerResult(camBitmap, threshBitmap, rects);
		}

		private void HueThreshold(float hueMin, float hueMax, Mat hsvFrame, Mat threshFrame)
		{
			using (var minThresh1 = new VectorOfFloat(new[] {hueMin, _satMin, _valMin}))
			{
				using (var maxThresh1 = new VectorOfFloat(new[] {hueMax, _satMax, _valMax}))
				{
					CvInvoke.InRange(hsvFrame, minThresh1, maxThresh1, threshFrame);
				}
			}
		}
	}
}
