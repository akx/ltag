using System.Collections.Generic;
using System.Drawing;

namespace LTag.Stroke
{
	public class Stroke
	{
		private readonly List<PointF> _points = new List<PointF>();

		public List<PointF> Points
		{
			get { return _points; }
		}

		public void AddPoint(PointF pointF)
		{
			_points.Add(pointF);
		}

		public float DistanceSqrToLastPoint(PointF newPoint)
		{
			if (_points.Count == 0) return float.MaxValue;
			var lastPoint = _points[_points.Count - 1];
			var dx = newPoint.X - lastPoint.X;
			var dy = newPoint.Y - lastPoint.Y;
			return dx*dx + dy*dy;
		}
	}
}
