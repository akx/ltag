﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using LTag.Draw;
using LTag.Stroke;
using LTag.Track;
using Timer = System.Windows.Forms.Timer;

namespace LTag
{
	public partial class MainForm : Form
	{
		private Capture _capture;
		private CapturePropertyProxy _captureProps;		
		private TuioReceiver _tuioReceiver = new TuioReceiver();
		private readonly LaserTracker _tracker = new LaserTracker();
		private readonly StrokeRecognizer _strokeRecognizer = new StrokeRecognizer();
		private Bitmap _debugImage;
		private readonly DrawWindow _drawWindow;
		private readonly Drawing _drawing = new Drawing();
		private String _nextStatus;
		private readonly Queue<LaserTrackerResult> _results = new Queue<LaserTrackerResult>();
		private readonly Timer _uiUpdateTimer = new Timer {Interval = 1000 / 25, Enabled = true};

		public MainForm()
		{
			InitializeComponent();
			CvInvoke.UseOpenCL = false;
			_drawWindow = new DrawWindow();
			_drawWindow.Show(this);
			_strokeRecognizer.OnStrokeUpdated += StrokeUpdated;
			_strokeRecognizer.OnClearZoneHit += ClearZoneHit;
			_drawing.OnBitmapChanged += () => { _drawWindow.Image = _drawing.Bitmap; };
			_drawing.RecreateBitmap();
			drawWindowPropertyGrid.PropertyValueChanged += (o, args) => _drawWindow.RefreshSoon();
			UpdatePropertyGrids();
			_tuioReceiver.PointReceived += TuioReceiverOnPointReceived;
			tuioCheckbox.Checked = true;
			TuioChanged(null, null);
			StartOrStopCapture(captureCheckbox.Checked);
			_uiUpdateTimer.Tick += (sender, args) => UpdateUI();
			
		}

		private void UpdatePropertyGrids()
		{
			cameraPropertyGrid.SelectedObject = _capture;
			capturePropertyGrid.SelectedObject = _captureProps;
			
			trackingPropertyGrid.SelectedObject = _tracker;
			strokePropertyGrid.SelectedObject = _strokeRecognizer;
			drawingPropertyGrid.SelectedObject = _drawing;
			drawWindowPropertyGrid.SelectedObject = _drawWindow.DrawParams;
			cameraPropertyGrid.Enabled = (cameraPropertyGrid.SelectedObject != null);
			capturePropertyGrid.Enabled = (capturePropertyGrid.SelectedObject != null);
		}

		private void ClearZoneHit(PointF point)
		{
			_drawing.Clear();
			SetStatus("Clear zone hit.");
		}

		private void StrokeUpdated(Stroke.Stroke stroke, bool didFinish)
		{
			if (stroke.Points.Count <= 0) return;
			var lastPoint = stroke.Points[stroke.Points.Count - 1];
			var secondLastPoint = stroke.Points.Count >= 2 ? stroke.Points[stroke.Points.Count - 2] : lastPoint;
			_drawing.Draw(secondLastPoint, lastPoint);
			_drawWindow.RefreshSoon();
		}

		private void ProcessGrabbedImage(object sender, EventArgs e)
		{
			if (_capture == null) return;
			using (var frame = new Mat())
			{
				_capture.Retrieve(frame);
				ProcessFrameFromQueue(frame);
			}
		}


		private void TuioReceiverOnPointReceived(bool hasPoint, PointF coords)
		{
			if (hasPoint)
			{
				_strokeRecognizer.UpdateWithPoint(coords);
			}
			else
			{
				_strokeRecognizer.UpdateNoPoint();
			}
		}


		private void ProcessFrameFromQueue(Mat frame)
		{
			var newResult = _tracker.UpdateFromFrame(frame);
			var rects = newResult.Rectangles;
			if (rects.Count > 0)
			{
				var pt = rects[0].Center().Rescale(
					1.0f / newResult.ThreshBitmap.Width,
					1.0f / newResult.ThreshBitmap.Height
				);
				_strokeRecognizer.UpdateWithPoint(pt);
			}
			else
			{
				_strokeRecognizer.UpdateNoPoint();
			}
			_results.Enqueue(newResult);
		}

		private void UpdateUI()
		{
			if (!String.IsNullOrEmpty(_nextStatus))
			{
				statusLabel.Text = _nextStatus;
			}
			LaserTrackerResult result = null;
			try
			{
				while (_results.Count > 0)
				{
					Util.Dispose(ref result);
					result = _results.Dequeue();
				}
			}
			catch
			{
				
			}
			if (result == null) return;
			if (capturePropertyGrid.SelectedObject == null)
			{
				// Only safe to do this once the first image is captured
				capturePropertyGrid.SelectedObject = _captureProps;
			}
			UpdateDebugText(result);
			UpdateDebugImage(result);
			Util.Dispose(ref result);
		}

		private void UpdateDebugText(LaserTrackerResult result)
		{
			var sb = new StringBuilder();
			var rects = result.Rectangles;
			processingTimeLabel.Text = String.Format("Process: {0:F1}ms", result.ProcessingTime.TotalMilliseconds);
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
				if (!(camBitmap != null && threshBitmap != null)) return;
				g.Clear(Color.Transparent);
				g.ResetTransform();
				g.DrawImage(camBitmap, 0, 0);
				g.DrawRectangle(Pens.Orange, 0, 0, camBitmap.Width, camBitmap.Height);
				if (!_tracker.Warp)
				{
					var points = (new[] {_tracker.Quad1, _tracker.Quad2, _tracker.Quad3, _tracker.Quad4, _tracker.Quad1}).Select(p => p.Rescale(camBitmap.Width, camBitmap.Height)).ToArray();
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
			StartOrStopCapture(captureCheckbox.Checked);
		}

		private void StartOrStopCapture(bool flag)
		{
			if (flag)
			{
				Util.Dispose(ref _capture);
				_capture = new Capture((int) cameraIndexUpDown.Value);
				_captureProps = new CapturePropertyProxy(_capture);
				_capture.ImageGrabbed += ProcessGrabbedImage;
				_capture.Start();
				SetStatus("Capture started.");
			}
			else
			{
				if(_capture != null) _capture.Stop();
				Util.Dispose(ref _capture);
				SetStatus("Capture stopped.");
			}
			UpdatePropertyGrids();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Util.Dispose(ref _tuioReceiver);
			Util.Dispose(ref _capture);
		}

		private void clearButton_Click(object sender, EventArgs e)
		{
			_strokeRecognizer.EndCurrentStroke();
			_drawing.Clear();
			_drawWindow.RefreshSoon();
			SetStatus("Drawing cleared.");
		}

		private void SetStatus(string status)
		{
			_nextStatus = status;
		}

		private static String GetIniPath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ltag.ini");
		}
		private void saveSettingsButton_Click(object sender, EventArgs e)
		{
			var iser = new IniSerializer();
			iser.WriteObject("Tracker", _tracker);
			iser.WriteObject("StrokeRecognizer", _strokeRecognizer);
			iser.WriteObject("Drawing", _drawing);
			iser.WriteObject("DrawParams", _drawWindow.DrawParams);
			var path = GetIniPath();
			using (var tw = new StreamWriter(path, false, Encoding.UTF8))
			{
				tw.Write(iser.GetValue());
			}
			SetStatus("Saved to " + path);
		}


		private void loadSettingsButton_Click(object sender, EventArgs e)
		{
			var path = GetIniPath();
			if (!File.Exists(path))
			{
				SetStatus("No INI file at " + path);
				return;
			}
			var iser = new IniSerializer();
			using (var tw = new StreamReader(path, Encoding.UTF8))
			{
				iser.Parse(tw.ReadToEnd());
				iser.UpdateObject("Tracker", _tracker);
				iser.UpdateObject("StrokeRecognizer", _strokeRecognizer);
				iser.UpdateObject("Drawing", _drawing);
				iser.UpdateObject("DrawParams", _drawWindow.DrawParams);
				UpdatePropertyGrids();
				SetStatus("Loaded from " + path);
			}
		}

		private void TuioChanged(object sender, EventArgs e)
		{
			_tuioReceiver.SetEnabled(tuioCheckbox.Checked);

		}
	}
}
