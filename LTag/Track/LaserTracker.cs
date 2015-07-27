using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace LTag.Track
{
	public class LaserTracker
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
		private bool _warp = true;


		private PointF _quad1 = new PointF(0, 0);
		private PointF _quad2 = new PointF(1, 0);
		private PointF _quad3 = new PointF(1, 1);
		private PointF _quad4 = new PointF(0, 1);
		private readonly Mat _homographyMat = new Mat(3, 3, DepthType.Cv32F, 1);
		private readonly Stopwatch _timer = new Stopwatch();

		public LaserTracker()
		{
			UpdateHomographyMat();
		}

		#region Properties

		[DefaultValue(0)]
		[Category("Accuracy")]
		public int Dilate
		{
			get { return _dilate; }
			set { _dilate = value; }
		}

		[DefaultValue(true)]
		[Category("Warp")]
		public bool Warp
		{
			get { return _warp; }
			set { _warp = value; }
		}


		[Category("Warp")]
		[TypeConverter(typeof(PointFConverter))]
		public PointF Quad1
		{
			get { return _quad1; }
			set { _quad1 = value; UpdateHomographyMat(); }
		}

		[Category("Warp")]
		[TypeConverter(typeof(PointFConverter))]
		public PointF Quad2
		{
			get { return _quad2; }
			set { _quad2 = value; UpdateHomographyMat(); }
		}

		[Category("Warp")]
		[TypeConverter(typeof(PointFConverter))]
		public PointF Quad3
		{
			get { return _quad3; }
			set { _quad3 = value; UpdateHomographyMat(); }
		}

		[Category("Warp")]
		[TypeConverter(typeof(PointFConverter))]
		public PointF Quad4
		{
			get { return _quad4; }
			set { _quad4 = value; UpdateHomographyMat(); }
		}

		[Category("Accuracy")]
		public int Width
		{
			get { return _width; }
			set { _width = value; UpdateHomographyMat(); }
		}

		[Category("Accuracy")]
		public int Height
		{
			get { return _height; }
			set { _height = value; UpdateHomographyMat(); }
		}

		[Category("Accuracy")]
		public int MinPixels
		{
			get { return _minPixels; }
			set { _minPixels = value; }
		}

		[Category("Color")]
		public int HueCenter
		{
			get { return _hueCenter; }
			set { _hueCenter = value; }
		}

		[Category("Color")]
		public int HueWidth
		{
			get { return _hueWidth; }
			set { _hueWidth = value; }
		}

		[Category("Color")]
		public int SatMin
		{
			get { return _satMin; }
			set { _satMin = value; }
		}

		[Category("Color")]
		public int SatMax
		{
			get { return _satMax; }
			set { _satMax = value; }
		}

		[Category("Color")]
		public int ValMin
		{
			get { return _valMin; }
			set { _valMin = value; }
		}

		[Category("Color")]
		public int ValMax
		{
			get { return _valMax; }
			set { _valMax = value; }
		}
		#endregion

		private void UpdateHomographyMat()
		{
			var origPoints = new[]
			{
				_quad1.Rescale(_width, _height),
				_quad2.Rescale(_width, _height),
				_quad3.Rescale(_width, _height),
				_quad4.Rescale(_width, _height)
			};
			var destPoints = new[]
			{
				new PointF(0, 0),
				new PointF(_width, 0),
				new PointF(_width, _height),
				new PointF(0, _height)
			};
			CvInvoke.FindHomography(origPoints, destPoints, _homographyMat, HomographyMethod.Default);
		}
		
		public LaserTrackerResult UpdateFromFrame(Mat frame)
		{
			_timer.Reset();
			_timer.Start();
			Bitmap camBitmap, threshBitmap;
			
			var rects = new List<Rectangle>();
			using (var threshFrame = new Mat())
			{
				using (var hsvFrame = new Mat())
				{
					using (var resizeFrame = new Mat())
					{
						var size = new Size(_width, _height);
						CvInvoke.Resize(frame, resizeFrame, size);
						if (_warp)
						{
							using (var warpedFrame = new Mat())
							{
								CvInvoke.WarpPerspective(resizeFrame, warpedFrame, _homographyMat, size);
								warpedFrame.CopyTo(resizeFrame);
							}
						}
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
				var s2 = r2.Width * r2.Height;
				return s1.CompareTo(s2);
			});
			return new LaserTrackerResult(camBitmap, threshBitmap, rects, _timer.Elapsed);
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
