namespace LTag
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.trackingTabPage = new System.Windows.Forms.TabPage();
			this.trackingPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.camTabPage = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.captureCheckbox = new System.Windows.Forms.CheckBox();
			this.cameraPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.debugPictureBox = new System.Windows.Forms.PictureBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.trackingTabPage.SuspendLayout();
			this.camTabPage.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.debugPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(784, 25);
			this.toolStrip1.TabIndex = 7;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 614);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(784, 22);
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 25);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.debugPictureBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
			this.splitContainer1.Size = new System.Drawing.Size(784, 589);
			this.splitContainer1.SplitterDistance = 363;
			this.splitContainer1.TabIndex = 9;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.label1, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.tabControl1, 1, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(417, 589);
			this.tableLayoutPanel2.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(411, 294);
			this.label1.TabIndex = 7;
			this.label1.Text = "label1";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.trackingTabPage);
			this.tabControl1.Controls.Add(this.camTabPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 297);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(411, 289);
			this.tabControl1.TabIndex = 8;
			// 
			// trackingTabPage
			// 
			this.trackingTabPage.Controls.Add(this.trackingPropertyGrid);
			this.trackingTabPage.Location = new System.Drawing.Point(4, 22);
			this.trackingTabPage.Name = "trackingTabPage";
			this.trackingTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.trackingTabPage.Size = new System.Drawing.Size(403, 263);
			this.trackingTabPage.TabIndex = 0;
			this.trackingTabPage.Text = "Tracking";
			this.trackingTabPage.UseVisualStyleBackColor = true;
			// 
			// trackingPropertyGrid
			// 
			this.trackingPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trackingPropertyGrid.HelpVisible = false;
			this.trackingPropertyGrid.Location = new System.Drawing.Point(3, 3);
			this.trackingPropertyGrid.Name = "trackingPropertyGrid";
			this.trackingPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.trackingPropertyGrid.Size = new System.Drawing.Size(397, 257);
			this.trackingPropertyGrid.TabIndex = 15;
			this.trackingPropertyGrid.ToolbarVisible = false;
			// 
			// camTabPage
			// 
			this.camTabPage.Controls.Add(this.tableLayoutPanel1);
			this.camTabPage.Location = new System.Drawing.Point(4, 22);
			this.camTabPage.Name = "camTabPage";
			this.camTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.camTabPage.Size = new System.Drawing.Size(444, 301);
			this.camTabPage.TabIndex = 1;
			this.camTabPage.Text = "Camera";
			this.camTabPage.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.captureCheckbox, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.cameraPropertyGrid, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 295);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// captureCheckbox
			// 
			this.captureCheckbox.AutoSize = true;
			this.captureCheckbox.Location = new System.Drawing.Point(3, 275);
			this.captureCheckbox.Name = "captureCheckbox";
			this.captureCheckbox.Size = new System.Drawing.Size(99, 17);
			this.captureCheckbox.TabIndex = 19;
			this.captureCheckbox.Text = "Enable Capture";
			this.captureCheckbox.UseVisualStyleBackColor = true;
			// 
			// cameraPropertyGrid
			// 
			this.cameraPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cameraPropertyGrid.HelpVisible = false;
			this.cameraPropertyGrid.Location = new System.Drawing.Point(3, 3);
			this.cameraPropertyGrid.Name = "cameraPropertyGrid";
			this.cameraPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.cameraPropertyGrid.Size = new System.Drawing.Size(432, 266);
			this.cameraPropertyGrid.TabIndex = 18;
			this.cameraPropertyGrid.ToolbarVisible = false;
			// 
			// debugPictureBox
			// 
			this.debugPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.debugPictureBox.Location = new System.Drawing.Point(0, 0);
			this.debugPictureBox.Name = "debugPictureBox";
			this.debugPictureBox.Size = new System.Drawing.Size(363, 589);
			this.debugPictureBox.TabIndex = 10;
			this.debugPictureBox.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 636);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Laser Tag";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.trackingTabPage.ResumeLayout(false);
			this.camTabPage.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.debugPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PictureBox debugPictureBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage trackingTabPage;
		private System.Windows.Forms.PropertyGrid trackingPropertyGrid;
		private System.Windows.Forms.TabPage camTabPage;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox captureCheckbox;
		private System.Windows.Forms.PropertyGrid cameraPropertyGrid;


	}
}

