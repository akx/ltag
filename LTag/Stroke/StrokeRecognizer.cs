using System;
using System.ComponentModel;
using System.Drawing;

namespace LTag.Stroke
{
	internal delegate void StrokeUpdatedDelegate(Stroke stroke, bool didFinish);
	internal delegate void ClearZoneHitDelegate(PointF point);
	class StrokeRecognizer
	{
		public event StrokeUpdatedDelegate OnStrokeUpdated;
		public event ClearZoneHitDelegate OnClearZoneHit;
		private int _clearZoneFrames = 5;
		private int _strokeEndFrames = 5;
		
		private float _minNormDistance = 0.005f;
		private float _jumpDistance = 0.2f;
		private float _smoothing = 0.03f;
		private PointF _clearZonePoint1 = new PointF(0.95f, 0);
		private PointF _clearZonePoint2 = new PointF(1.0f, 0.05f);

		private int _clearZoneFrameCounter;
		private int _strokeEndFrameCounter;
		private Stroke _currentStroke;

		[Browsable(false)]
		public Stroke CurrentStroke
		{
			get { return _currentStroke; }
		}

		[Category("Clear Zone")]
		public int ClearZoneFrames
		{
			get { return _clearZoneFrames; }
			set { _clearZoneFrames = value; }
		}

		[Category("Clear Zone")]
		public PointF ClearZonePoint1
		{
			get { return _clearZonePoint1; }
			set { _clearZonePoint1 = value; }
		}

		[Category("Clear Zone")]
		public PointF ClearZonePoint2
		{
			get { return _clearZonePoint2; }
			set { _clearZonePoint2 = value; }
		}

		[Category("Behavior")]
		[Description("How many frames of no point means the stroke has ended")]
		public int StrokeEndFrames
		{
			get { return _strokeEndFrames; }
			set { _strokeEndFrames = value; }
		}

		[Category("Behavior")]
		public float JumpDistance
		{
			get { return _jumpDistance; }
			set { _jumpDistance = value; }
		}

		[Category("Behavior")]
		public float MinNormDistance
		{
			get { return _minNormDistance; }
			set { _minNormDistance = value; }
		}

		[Category("Behavior")]
		public float Smoothing
		{
			get { return _smoothing; }
			set { _smoothing = Math.Max(0, Math.Min(1, value)); }
		}


		public void UpdateNoPoint()
		{
			if (_currentStroke == null) return;
			_strokeEndFrameCounter ++;
			if (_strokeEndFrameCounter >= _strokeEndFrames)
			{
				EndCurrentStroke();
			}
		}

		public void UpdateWithPoint(PointF normalizedPoint)
		{
			if (CheckClearZone(normalizedPoint)) return;
			if (_currentStroke == null) BeginStroke();
			if (_smoothing > 0)
			{
				if (_currentStroke.Points.Count > 0)
				{
					var lastPoint = _currentStroke.Points[_currentStroke.Points.Count - 1];
					var iSmoothing = 1.0 - _smoothing;
					normalizedPoint = new PointF(
						(float)(normalizedPoint.X * iSmoothing + lastPoint.X * _smoothing),
						(float)(normalizedPoint.Y * iSmoothing + lastPoint.Y * _smoothing)
					);
				}
			}
			var dist = _currentStroke.DistanceSqrToLastPoint(normalizedPoint);
			if (dist < _minNormDistance*_minNormDistance) return;
			if (dist > _jumpDistance * _jumpDistance) BeginStroke();
			_currentStroke.AddPoint(normalizedPoint);
			if (OnStrokeUpdated != null) OnStrokeUpdated(_currentStroke, false);
		}

		private bool CheckClearZone(PointF normalizedPoint)
		{
			if (_clearZoneFrames <= 0) return false;
			var w = _clearZonePoint2.X - _clearZonePoint1.X;
			var h = _clearZonePoint2.Y - _clearZonePoint1.Y;
			bool czHit = false;
			if (new RectangleF(_clearZonePoint1.X, _clearZonePoint1.Y, w, h).Contains(normalizedPoint))
			{
				_clearZoneFrameCounter ++;
				czHit = true;
			}
			else
			{
				_clearZoneFrameCounter = 0;
			}
			if (_clearZoneFrameCounter >= _clearZoneFrames)
			{
				ClearZoneHit(normalizedPoint);
			}
			return czHit;
		}

		private void ClearZoneHit(PointF pointF)
		{
			EndCurrentStroke();
			if (OnClearZoneHit != null) OnClearZoneHit(pointF);
		}

		private void BeginStroke()
		{
			EndCurrentStroke();
			_currentStroke = new Stroke();
		}

		public void EndCurrentStroke()
		{
			_strokeEndFrameCounter = 0;
			_clearZoneFrameCounter = 0;
			if (_currentStroke == null) return;
			if (OnStrokeUpdated != null) OnStrokeUpdated(_currentStroke, true);
			_currentStroke = null;
		}


	}

	
}
