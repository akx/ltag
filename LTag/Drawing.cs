using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LTag
{
	public class Drawing
	{
		public delegate void BitmapChangedDelegate();

		public event BitmapChangedDelegate OnBitmapChanged;

		private int _width = 1024;
		private int _height = 768;
		private int _drawPxDist = 5;
		private int _brushSize = 35;
		private float _brushOpacity = 0.9f;
		private float _brushBias = 1f;
		private Color _brushColor = Color.White;
		private Bitmap _bitmap;
		private Bitmap _brushBitmap;

		[Browsable(false)]
		public Bitmap Bitmap
		{
			get { return _bitmap; }
		}

		[Category("Brush")]
		public int DrawPxDist
		{
			get { return _drawPxDist; }
			set { _drawPxDist = value; }
		}

		[Category("Brush")]
		public float BrushOpacity
		{
			get { return _brushOpacity; }
			set { _brushOpacity = value; RecreateBrush(); }
		}

		[Category("Brush")]
		public float BrushBias
		{
			get { return _brushBias; }
			set { _brushBias = value; RecreateBrush(); }
		}

		[Category("Brush")]
		public Color BrushColor
		{
			get { return _brushColor; }
			set { _brushColor = value; RecreateBrush(); }
		}

		[Category("Brush")]
		public int BrushSize
		{
			get { return _brushSize; }
			set { _brushSize = value; RecreateBrush(); }
		}

		[Category("Output")]
		public int Width
		{
			get { return _width; }
			set { _width = value; RecreateBitmap(); }
		}

		[Category("Output")]
		public int Height
		{
			get { return _height; }
			set { _height = value; RecreateBitmap(); }
		}

		private void RecreateBrush()
		{
			var bitmap = new Bitmap(_brushSize, _brushSize, PixelFormat.Format32bppArgb);
			var center = new PointF(bitmap.Width / 2f, bitmap.Height / 2f);
			var sz = Math.Max(center.X, center.Y);
			var maxAlpha = (int) Math.Round(_brushOpacity*255);
			for(var y = 0; y < bitmap.Width; y++)
			{
				for (var x = 0; x < bitmap.Height; x++)
				{
					var dst = Math.Sqrt(Util.DistanceSqr(new PointF(x, y), center));
					dst = 1 - (dst/sz);
					dst = Math.Pow(dst, _brushBias);
					var alpha = Math.Min(Math.Max((int)(dst * maxAlpha), 0), maxAlpha);
					bitmap.SetPixel(x, y, Color.FromArgb(alpha, _brushColor));
				}
			}
			Util.Dispose(ref _brushBitmap);
			_brushBitmap = bitmap;
		}

		public void RecreateBitmap()
		{
			var newBitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
			using (var g = Graphics.FromImage(newBitmap))
			{
				g.Clear(Color.Black);
				if (_bitmap != null) g.DrawImage(_bitmap, _width, _height);
				
			}
			Util.Dispose(ref _bitmap);
			_bitmap = newBitmap;
			if (OnBitmapChanged != null) OnBitmapChanged();
		}

		public Drawing()
		{
			RecreateBrush();
			RecreateBitmap();
		}

		public void Draw(PointF point1, PointF point2)
		{
			if (_bitmap == null) return;
			point1 = point1.Rescale(_width, _height);
			point2 = point2.Rescale(_width, _height);
			var dst = Math.Sqrt(Util.DistanceSqr(point1, point2));
			var nSteps = (int) Math.Ceiling(dst/_drawPxDist);
			lock (_bitmap)
			{
				using (var g = Graphics.FromImage(_bitmap))
				{
					g.CompositingMode = CompositingMode.SourceOver;
					//g.DrawLine(Pens.White, point1, point2);
					for (var step = 1; step < nSteps; step++)
					{
						var alpha = step/(float) (nSteps - 1);
						var p = point1.LerpTo(point2, alpha);
						g.DrawImage(_brushBitmap, p.X - _brushBitmap.Width * 0.5f, p.Y - _brushBitmap.Height * 0.5f);
					}
				}
			}
		}

		public void Clear()
		{
			lock (_bitmap)
			{
				using (var g = Graphics.FromImage(_bitmap))
				{
					g.Clear(Color.Black);
				}
			}
		}
	}
}
