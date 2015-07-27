using System;
using System.Drawing;
using System.Windows.Forms;

namespace LTag
{
	public partial class DrawWindow : Form
	{
		public Bitmap Image { get; set; }
		private Timer _refreshTimer = new Timer() { Interval = 1000 / 45, Enabled = true};
		private bool _shouldRedraw;

		public DrawWindow()
		{
			InitializeComponent();
			_refreshTimer.Tick +=RefreshTick;

		}

		private void RefreshTick(object sender, EventArgs eventArgs)
		{
			if (!_shouldRedraw) return;
			_shouldRedraw = false;
			Refresh();
		}

		private void DrawWindow_KeyPress(object sender, KeyPressEventArgs e)
		{
			var key = e.KeyChar;

			if (key == 's')
			{
				var screen = Screen.FromControl(this);
				Left = screen.Bounds.Left;
				Top = screen.Bounds.Top;
				Width = screen.Bounds.Width;
				Height = screen.Bounds.Height;
			}
			if (key == 'f')
			{
				ToggleFullScreen();
			}
		}

		private void ToggleFullScreen()
		{
			FormBorderStyle = (FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.SizableToolWindow : FormBorderStyle.None);
		}

		private void DrawWindow_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			if (Image == null) return;
			lock (Image)
			{
				g.DrawImage(Image, 0, 0, Width, Height);
			}
		}

		public void RefreshSoon()
		{
			_shouldRedraw = true;
		}
	}
}
