using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using LTag.Stroke;
using LTag.Track;

namespace LTag
{
	public partial class MainForm : Form
	{
		private Capture _capture;
		private CapturePropertyProxy _captureProps;
		private LaserTrackerResult _lastResult;
		private readonly LaserTracker _tracker = new LaserTracker();
		private readonly StrokeRecognizer _strokeRecognizer = new StrokeRecognizer();
		private Bitmap _debugImage;

		public MainForm()
		{
			InitializeComponent();
			CvInvoke.UseOpenCL = false;
			_capture = new Capture();
			_captureProps = new CapturePropertyProxy(_capture);
			_capture.ImageGrabbed += ProcessGrabbedImage;
			captureCheckbox.Checked = true;
			_capture.Start();
			trackingPropertyGrid.SelectedObject = _tracker;
			cameraPropertyGrid.SelectedObject = _capture;
			strokePropertyGrid.SelectedObject = _strokeRecognizer;
		}

		private void ProcessGrabbedImage(object sender, EventArgs e)
		{
			if (_capture == null) return;
			if (capturePropertyGrid.SelectedObject == null)
			{
				// Only safe to do this once the first image is captured
				capturePropertyGrid.SelectedObject = _captureProps;
			}
			LaserTrackerResult newResult;
			using (var frame = new Mat())
			{
				_capture.Retrieve(frame);
				newResult = _tracker.UpdateFromFrame(frame);
				var rects = newResult.Rectangles;
				if (rects.Count > 0)
				{
					var pt = rects[0].Center().Rescale(
						1.0f/newResult.ThreshBitmap.Width,
						1.0f/newResult.ThreshBitmap.Height
					);
					_strokeRecognizer.UpdateWithPoint(pt);
				}
				else
				{
					_strokeRecognizer.UpdateNoPoint();
				}

			}
			var oldResult = Interlocked.Exchange(ref _lastResult, newResult);
			if(oldResult != null) oldResult.Dispose();
			BeginInvoke(new Action(UpdateUI));
		}


		private void UpdateUI()
		{
			var result = _lastResult;
			if (result == null) return;
			UpdateDebugText(result);
			UpdateDebugImage(result);
		}

		private void UpdateDebugText(LaserTrackerResult result)
		{
			var sb = new StringBuilder();
			var rects = result.Rectangles;
			sb.AppendFormat("Processing time: {0}ms\n", result.ProcessingTime.TotalMilliseconds);
			sb.AppendFormat("Contours: {0}\n", rects.Count);
			foreach (var rectangle in rects)
			{
				sb.AppendFormat("{0}\n", rectangle);
			}
			label1.Text = sb.ToString();
		}

		private void UpdateDebugImage(LaserTrackerResult result)
		{
			result.ApplyDebugToImage();
			if (_debugImage == null || _debugImage.Height != debugPictureBox.Height || _debugImage.Width != debugPictureBox.Width)
			{
				Util.Dispose(ref _debugImage);
				_debugImage = new Bitmap(debugPictureBox.Width, debugPictureBox.Height, PixelFormat.Format32bppArgb);
			}
			using (var g = Graphics.FromImage(_debugImage))
			{
				var camBitmap = result.CamBitmap;
				var threshBitmap = result.ThreshBitmap;
				g.Clear(Color.Transparent);
				g.ResetTransform();
				g.DrawImage(camBitmap, 0, 0);
				g.DrawRectangle(Pens.Orange, 0, 0, camBitmap.Width, camBitmap.Height);
				if (!_tracker.Warp)
				{
					var points = (new[] {_tracker.Quad1, _tracker.Quad2, _tracker.Quad3, _tracker.Quad4, _tracker.Quad1}).Select((p) => p.Rescale(camBitmap.Width, camBitmap.Height)).ToArray();
					g.DrawLines(Pens.Silver, points);
				}
				g.TranslateTransform(0, camBitmap.Height + 2);
				g.DrawImage(threshBitmap, 0, 0);
				g.DrawRectangle(Pens.LimeGreen, 0, 0, threshBitmap.Width, threshBitmap.Height);
				var stroke = _strokeRecognizer.CurrentStroke;
				if (stroke != null)
				{
					var points = stroke.Points.Select(point => point.Rescale(threshBitmap.Width, threshBitmap.Height)).ToArray();
					if (points.Length >= 2)
					{
						using (var p = new Pen(Color.YellowGreen, 2) {DashStyle = DashStyle.Dot})
						{
							g.DrawLines(p, points);
						}
					}
				}
			}
			debugPictureBox.Image = _debugImage;
		}

		private void CaptureChanged(object sender, EventArgs e)
		{
			var flag = captureCheckbox.Checked;
			if (flag)
			{
				_capture.Start();
			}
			else
			{
				_capture.Pause();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Util.Dispose(ref _capture);
		}


	}
}
