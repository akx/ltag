using System.Drawing;

namespace LTag.Draw
{
	public class DrawParams
	{
		private float _scaleX = 1f;
		private float _scaleY = 1f;
		private float _offsetX = 0f;
		private float _offsetY = 0f;
		private float _rotation = 0f;

		public float ScaleX
		{
			get { return _scaleX; }
			set { _scaleX = value; }
		}

		public float ScaleY
		{
			get { return _scaleY; }
			set { _scaleY = value; }
		}

		public float OffsetX
		{
			get { return _offsetX; }
			set { _offsetX = value; }
		}

		public float OffsetY
		{
			get { return _offsetY; }
			set { _offsetY = value; }
		}

		public float Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}

		public void Apply(Graphics g, int width, int height)
		{
			g.ResetTransform();
			if(_rotation != 0) g.RotateTransform(_rotation);
			if(_scaleX != 1 || _scaleY != 1) g.ScaleTransform(_scaleX, _scaleY);
			g.TranslateTransform(_offsetX * width, _offsetY * height);
		}
	}
}