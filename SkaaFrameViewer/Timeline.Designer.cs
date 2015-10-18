/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/

using System.Windows.Forms;
namespace Timeline
{
    partial class TimelineControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picBoxFrame = new System.Windows.Forms.PictureBox();
            this.frameSlider = new System.Windows.Forms.TrackBar();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxFrame
            // 
            this.picBoxFrame.Location = new System.Drawing.Point(3, 3);
            this.picBoxFrame.Name = "picBoxFrame";
            this.picBoxFrame.Size = new System.Drawing.Size(121, 121);
            this.picBoxFrame.TabIndex = 4;
            this.picBoxFrame.TabStop = false;
            this.picBoxFrame.DoubleClick += new System.EventHandler(this.picBoxFrame_DoubleClick);
            this.picBoxFrame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picBoxFrame_Click);
            // 
            // frameSlider
            // 
            this.frameSlider.LargeChange = 4;
            this.frameSlider.Location = new System.Drawing.Point(4, 131);
            this.frameSlider.Name = "frameSlider";
            this.frameSlider.Size = new System.Drawing.Size(120, 45);
            this.frameSlider.TabIndex = 5;
            this.frameSlider.ValueChanged += new System.EventHandler(this.frameSlider_ValueChanged);
            // 
            // TimelineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.frameSlider);
            this.Controls.Add(this.picBoxFrame);
            this.Name = "TimelineControl";
            this.Size = new System.Drawing.Size(129, 172);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private PictureBox picBoxFrame;
        private TrackBar frameSlider;
        private Timer animationTimer;
    }
}
