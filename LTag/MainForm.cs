using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;

namespace LTag
{
	public partial class MainForm : Form
	{
		private Capture _capture;
		private LaserTrackerResult _lastResult;
		private readonly LaserTracker _tracker = new LaserTracker();
		private Bitmap _debugImage;

		public MainForm()
		{
			InitializeComponent();
			CvInvoke.UseOpenCL = false;
			_capture = new Capture();
			_capture.ImageGrabbed += ProcessGrabbedImage;
			captureCheckbox.Checked = true;
			_capture.Start();
			trackingPropertyGrid.SelectedObject = _tracker;
			cameraPropertyGrid.SelectedObject = _capture;
		}

		private void ProcessGrabbedImage(object sender, EventArgs e)
		{
			if (_capture == null) return;
			LaserTrackerResult newResult;
			using (var frame = new Mat())
			{
				_capture.Retrieve(frame);
				newResult = _tracker.UpdateFromFrame(frame);
			}
			var oldResult = Interlocked.Exchange(ref _lastResult, newResult);
			if(oldResult != null) oldResult.Dispose();
			BeginInvoke(new Action(UpdateUI));
		}


		private void UpdateUI()
		{
			var result = _lastResult;
			if (result == null) return;
			var sb = new StringBuilder();
			var rects = result.Rectangles;
			result.ApplyDebugToImage();
			sb.AppendFormat("Contours: {0}\n", rects.Count);
			foreach (var rectangle in rects)
			{
				sb.AppendFormat("{0}\n", rectangle);
			}
			if (_debugImage == null || _debugImage.Height != debugPictureBox.Height || _debugImage.Width != debugPictureBox.Width)
			{
				Util.Dispose(ref _debugImage);
				_debugImage = new Bitmap(debugPictureBox.Width, debugPictureBox.Height, PixelFormat.Format32bppArgb);
			}
			using (var g = Graphics.FromImage(_debugImage))
			{
				var y = 0;
				var camBitmap = result.CamBitmap;
				var threshBitmap = result.ThreshBitmap;

				g.DrawImage(camBitmap, 0, y);
				g.DrawRectangle(Pens.Orange, 0, 0, camBitmap.Width, camBitmap.Height);
				y += camBitmap.Height + 2;
				
				g.DrawImage(threshBitmap, 0, y);
				g.DrawRectangle(Pens.LimeGreen, 0, y, threshBitmap.Width, threshBitmap.Height);
			}

			debugPictureBox.Image = _debugImage;

			label1.Text = sb.ToString();
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
