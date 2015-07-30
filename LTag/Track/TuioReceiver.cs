using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using LTag.OSC;

namespace LTag.Track
{
	public delegate void TuioPointReceived(bool hasPoint, PointF coords);

	class TuioReceiver: IDisposable
	{
		private OSCReceiver _oscReceiver = new OSCReceiver(3333);
		private BackgroundWorker _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
		private bool _enabled = false;
		private int _currentPointerId = 0;
		private PointF _lastPoint = new PointF();
		public event TuioPointReceived PointReceived;

		public TuioReceiver()
		{
			_worker.DoWork += ReceiverThread;
		}

		private void ReceiverThread(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			while (!_worker.CancellationPending)
			{
				if(!_enabled) Thread.Sleep(100);
				var packet = _oscReceiver.Receive();
				if (packet == null) continue;
				HandlePacket(packet);
			}
		}

		private void HandlePacket(OSCPacket packet)
		{
			if (packet.IsBundle())
			{
				foreach (var value in packet.Values)
				{
					HandleMessage(value as OSCMessage);
				}
			}
			else
			{
				HandleMessage(packet as OSCMessage);
			}
		}

		private void HandleMessage(OSCMessage msg)
		{
			if (msg == null) return;
			if (msg.Address != "/tuio/2Dcur") return;
			var kind = msg.Values[0] as string;
			if (string.IsNullOrEmpty(kind)) return;
			switch (kind)
			{
				case "alive":
					if (!msg.Values.ToArray().Skip(1).Cast<int>().Contains(_currentPointerId))
					{
						_currentPointerId = 0;
						DispatchCurrentPoint();
					}
					break;
				case "set":
					var pointerId = Convert.ToInt32(msg.Values[1]);
					if (_currentPointerId == 0 || _currentPointerId == pointerId)
					{
						_currentPointerId = pointerId;
						var x = (float) msg.Values[2];
						var y = (float) msg.Values[3];
						_lastPoint.X = x;
						_lastPoint.Y = y;
						DispatchCurrentPoint();
					}
					break;

				default:
					//Debug.Print("{0} {1}", msg.Address, msg);
					break;
			}
			
		}

		private void DispatchCurrentPoint()
		{
			if (PointReceived != null)
			{
				PointReceived(_currentPointerId != 0, _lastPoint);
			}
		}

		public void SetEnabled(bool enabled)
		{
			if (_oscReceiver == null) return;
			if (enabled)
			{
				_oscReceiver.Connect();
				if(!_worker.IsBusy) _worker.RunWorkerAsync();
			}
			else
			{
				_oscReceiver.Close();
				_currentPointerId = 0;
				DispatchCurrentPoint();
			}
			_enabled = enabled;
		}

		public void Dispose()
		{
			if (_worker != null)
			{
				_worker.CancelAsync();
				_worker.Dispose();
				_worker = null;
			}
			if (_oscReceiver != null)
			{
				_oscReceiver.Close();
				_oscReceiver = null;
			}
		}
	}
}
