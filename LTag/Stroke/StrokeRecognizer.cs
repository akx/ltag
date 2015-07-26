using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace LTag.Stroke
{
	internal delegate void StrokeUpdatedDelegate(Stroke stroke, bool didFinish);
	class StrokeRecognizer
	{
		public event StrokeUpdatedDelegate OnStrokeUpdated;
		private int _strokeEndFrames = 5;
		private int _strokeEndFrameCounter;
		private float _minNormDistance = 0.005f;
		private float _smoothing = 0.03f;

		private Stroke _currentStroke;

		public Stroke CurrentStroke
		{
			get { return _currentStroke; }
		}

		[Description("How many frames of no point means the stroke has ended")]
		public int StrokeEndFrames
		{
			get { return _strokeEndFrames; }
			set { _strokeEndFrames = value; }
		}

		public float MinNormDistance
		{
			get { return _minNormDistance; }
			set { _minNormDistance = value; }
		}

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
			if (_currentStroke.DistanceSqrToLastPoint(normalizedPoint) < _minNormDistance*_minNormDistance) return;
			_currentStroke.AddPoint(normalizedPoint);
			if (OnStrokeUpdated != null) OnStrokeUpdated(_currentStroke, false);
		}

		private void BeginStroke()
		{
			_strokeEndFrameCounter = 0;
			_currentStroke = new Stroke();
		}

		private void EndCurrentStroke()
		{
			_strokeEndFrameCounter = 0;
			if (_currentStroke == null) return;
			if (OnStrokeUpdated != null) OnStrokeUpdated(_currentStroke, true);
			_currentStroke = null;
		}


	}

	
}
