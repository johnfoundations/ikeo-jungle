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
			this.viewport1 = new System.Windows.Forms.Panel();
			this.loadXML_butt = new System.Windows.Forms.Button();
			this.xmlFileName_tb = new System.Windows.Forms.TextBox();
//			this.glControl1 = new OpenTK.GLControl();
			this.glControl1 = new CustomGLControl();
			this.viewport1.SuspendLayout();
			this.SuspendLayout();
			// 
			// viewport1
			// 
			this.viewport1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.viewport1.Controls.Add(this.glControl1);
			this.viewport1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewport1.Location = new System.Drawing.Point(0, 0);
			this.viewport1.Name = "viewport1";
			this.viewport1.Size = new System.Drawing.Size(792, 566);
			this.viewport1.TabIndex = 2;
			this.viewport1.Resize += new System.EventHandler(this.GlControl1Resize);
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
			// xmlFileName_tb
			// 
			this.xmlFileName_tb.Location = new System.Drawing.Point(12, 534);
			this.xmlFileName_tb.Name = "xmlFileName_tb";
			this.xmlFileName_tb.Size = new System.Drawing.Size(140, 20);
			this.xmlFileName_tb.TabIndex = 13;
			// 
			// glControl1
			// 
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Location = new System.Drawing.Point(225, 290);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(150, 150);
			this.glControl1.TabIndex = 0;
			this.glControl1.VSync = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.loadXML_butt);
			this.Controls.Add(this.xmlFileName_tb);
			this.Controls.Add(this.viewport1);
			this.Name = "MainForm";
			this.Text = "BH-VizualizationApplication";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.Resize += new System.EventHandler(this.GlControl1Resize);
			this.ResizeEnd += new System.EventHandler(this.GlControl1Resize);
			this.viewport1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
//		private OpenTK.GLControl glControl1;
		private CustomGLControl glControl1;
		private System.Windows.Forms.Button loadXML_butt;
		private System.Windows.Forms.TextBox xmlFileName_tb;
		private System.Windows.Forms.Panel viewport1;
		
	
	}
}
