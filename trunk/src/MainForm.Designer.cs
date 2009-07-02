//	(C) Copyright Ian Keough 2009
//	This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace ikeo
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
//			this.glControl1 = new OpenTK.GLControl();
			this.glControl1 = new CustomGLControl();
			
			this.label1 = new System.Windows.Forms.Label();
			this.viewport1 = new System.Windows.Forms.Panel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.lineWidth_trackBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.Red_label = new System.Windows.Forms.Label();
			this.red_slider = new System.Windows.Forms.TrackBar();
			this.label3 = new System.Windows.Forms.Label();
			this.green_slider = new System.Windows.Forms.TrackBar();
			this.blue_label = new System.Windows.Forms.Label();
			this.blue_slider = new System.Windows.Forms.TrackBar();
			this.pointLabel = new System.Windows.Forms.Label();
			this.xmlFileName_tb = new System.Windows.Forms.TextBox();
			this.loadXML_butt = new System.Windows.Forms.Button();
			this.viewport1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lineWidth_trackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.red_slider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.green_slider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.blue_slider)).BeginInit();
			this.SuspendLayout();
			// 
			// glControl1
			// 
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glControl1.Location = new System.Drawing.Point(0, 0);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(586, 531);
			this.glControl1.TabIndex = 0;
			this.glControl1.VSync = false;
			this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.GlControl1Paint);
			this.glControl1.Resize += new System.EventHandler(this.GlControl1Resize);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 373);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(182, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// viewport1
			// 
			this.viewport1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.viewport1.Controls.Add(this.glControl1);
			this.viewport1.Location = new System.Drawing.Point(200, 28);
			this.viewport1.Name = "viewport1";
			this.viewport1.Size = new System.Drawing.Size(590, 535);
			this.viewport1.TabIndex = 2;
			this.viewport1.Resize += new System.EventHandler(this.GlControl1Resize);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(792, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// lineWidth_trackBar
			// 
			this.lineWidth_trackBar.Location = new System.Drawing.Point(12, 89);
			this.lineWidth_trackBar.Maximum = 30;
			this.lineWidth_trackBar.Minimum = 1;
			this.lineWidth_trackBar.Name = "lineWidth_trackBar";
			this.lineWidth_trackBar.Size = new System.Drawing.Size(104, 42);
			this.lineWidth_trackBar.TabIndex = 4;
			this.lineWidth_trackBar.Value = 1;
			this.lineWidth_trackBar.Scroll += new System.EventHandler(this.LineWidth_trackBarScroll);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Line Width";
			// 
			// Red_label
			// 
			this.Red_label.Location = new System.Drawing.Point(20, 137);
			this.Red_label.Name = "Red_label";
			this.Red_label.Size = new System.Drawing.Size(100, 23);
			this.Red_label.TabIndex = 6;
			this.Red_label.Text = "Red";
			// 
			// red_slider
			// 
			this.red_slider.Location = new System.Drawing.Point(16, 163);
			this.red_slider.Name = "red_slider";
			this.red_slider.Size = new System.Drawing.Size(104, 42);
			this.red_slider.TabIndex = 7;
			this.red_slider.Scroll += new System.EventHandler(this.Red_sliderScroll);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(20, 211);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 8;
			this.label3.Text = "Green";
			// 
			// green_slider
			// 
			this.green_slider.Location = new System.Drawing.Point(20, 238);
			this.green_slider.Name = "green_slider";
			this.green_slider.Size = new System.Drawing.Size(104, 42);
			this.green_slider.TabIndex = 9;
			this.green_slider.Scroll += new System.EventHandler(this.Green_sliderScroll);
			// 
			// blue_label
			// 
			this.blue_label.Location = new System.Drawing.Point(20, 290);
			this.blue_label.Name = "blue_label";
			this.blue_label.Size = new System.Drawing.Size(100, 23);
			this.blue_label.TabIndex = 10;
			this.blue_label.Text = "Blue";
			// 
			// blue_slider
			// 
			this.blue_slider.Location = new System.Drawing.Point(20, 317);
			this.blue_slider.Name = "blue_slider";
			this.blue_slider.Size = new System.Drawing.Size(104, 42);
			this.blue_slider.TabIndex = 11;
			this.blue_slider.Scroll += new System.EventHandler(this.Blue_sliderScroll);
			// 
			// pointLabel
			// 
			this.pointLabel.Location = new System.Drawing.Point(12, 423);
			this.pointLabel.Name = "pointLabel";
			this.pointLabel.Size = new System.Drawing.Size(182, 23);
			this.pointLabel.TabIndex = 12;
			this.pointLabel.Text = "label4";
			// 
			// xmlFileName_tb
			// 
			this.xmlFileName_tb.Location = new System.Drawing.Point(12, 534);
			this.xmlFileName_tb.Name = "xmlFileName_tb";
			this.xmlFileName_tb.Size = new System.Drawing.Size(140, 20);
			this.xmlFileName_tb.TabIndex = 13;
			// 
			// loadXML_butt
			// 
			this.loadXML_butt.Location = new System.Drawing.Point(158, 534);
			this.loadXML_butt.Name = "loadXML_butt";
			this.loadXML_butt.Size = new System.Drawing.Size(36, 23);
			this.loadXML_butt.TabIndex = 14;
			this.loadXML_butt.Text = "...";
			this.loadXML_butt.UseVisualStyleBackColor = true;
			this.loadXML_butt.Click += new System.EventHandler(this.Load_buttClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.loadXML_butt);
			this.Controls.Add(this.xmlFileName_tb);
			this.Controls.Add(this.pointLabel);
			this.Controls.Add(this.blue_slider);
			this.Controls.Add(this.blue_label);
			this.Controls.Add(this.green_slider);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.red_slider);
			this.Controls.Add(this.Red_label);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lineWidth_trackBar);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.viewport1);
			this.Controls.Add(this.label1);
			this.Name = "MainForm";
			this.Text = "BH-VizualizationApplication";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.Resize += new System.EventHandler(this.GlControl1Resize);
			this.ResizeEnd += new System.EventHandler(this.GlControl1Resize);
			this.viewport1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lineWidth_trackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.red_slider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.green_slider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.blue_slider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button loadXML_butt;
		private System.Windows.Forms.TextBox xmlFileName_tb;
		private System.Windows.Forms.Label pointLabel;
		private System.Windows.Forms.TrackBar blue_slider;
		private System.Windows.Forms.Label blue_label;
		private System.Windows.Forms.TrackBar green_slider;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar red_slider;
		private System.Windows.Forms.Label Red_label;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar lineWidth_trackBar;
		private System.Windows.Forms.Panel viewport1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.Label label1;
		private OpenTK.GLControl glControl1;
	}
}
