using Emgu.CV;
using Emgu.CV.CvEnum;

namespace LTag
{
	partial class CapturePropertyProxy
	{
		private readonly Capture _capture;


		private double GetValue(CapProp prop)
		{
			try
			{
				return _capture.GetCaptureProperty(prop);
			}
			catch
			{
				return 0;
			}
		}

		private void SetValue(CapProp prop, double value)
		{
			try
			{
				_capture.SetCaptureProperty(prop, value);
			}
			catch
			{
				
			}
		}

		public CapturePropertyProxy(Capture capture)
		{
			_capture = capture;
		}
	}
}
