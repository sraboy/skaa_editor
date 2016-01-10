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

namespace Capslock.Windows.Forms.SpriteViewer
{
    partial class TimelineView
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
            this._picBoxFrame = new System.Windows.Forms.PictureBox();
            this._sliderBar = new System.Windows.Forms.TrackBar();
            this._animationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._picBoxFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._sliderBar)).BeginInit();
            this.SuspendLayout();
            // 
            // _picBoxFrame
            // 
            this._picBoxFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._picBoxFrame.Location = new System.Drawing.Point(0, 1);
            this._picBoxFrame.Name = "_picBoxFrame";
            this._picBoxFrame.Size = new System.Drawing.Size(235, 235);
            this._picBoxFrame.TabIndex = 4;
            this._picBoxFrame.TabStop = false;
            this._picBoxFrame.DoubleClick += new System.EventHandler(this.picBoxFrame_DoubleClick);
            this._picBoxFrame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picBoxFrame_Click);
            // 
            // _sliderBar
            // 
            this._sliderBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._sliderBar.AutoSize = false;
            this._sliderBar.LargeChange = 4;
            this._sliderBar.Location = new System.Drawing.Point(0, 190);
            this._sliderBar.Name = "_sliderBar";
            this._sliderBar.Size = new System.Drawing.Size(235, 35);
            this._sliderBar.TabIndex = 5;
            this._sliderBar.ValueChanged += new System.EventHandler(this.frameSlider_ValueChanged);
            // 
            // TimelineView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._sliderBar);
            this.Controls.Add(this._picBoxFrame);
            this.Name = "TimelineView";
            this.Size = new System.Drawing.Size(235, 225);
            ((System.ComponentModel.ISupportInitialize)(this._picBoxFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._sliderBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private PictureBox _picBoxFrame;
        private TrackBar _sliderBar;
        private Timer _animationTimer;
    }
}
