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
    partial class SpriteViewer
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
            this.timelineView1 = new Capslock.Windows.Forms.SpriteViewer.TimelineView();
            this.listView1 = new Capslock.Windows.Forms.SpriteViewer.ListView();
            this.SuspendLayout();
            // 
            // timelineView1
            // 
            this.timelineView1.AutoSize = true;
            this.timelineView1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timelineView1.Location = new System.Drawing.Point(0, 0);
            this.timelineView1.Name = "timelineView1";
            this.timelineView1.Size = new System.Drawing.Size(283, 249);
            this.timelineView1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.AutoSize = true;
            this.listView1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listView1.Location = new System.Drawing.Point(0, 255);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(283, 417);
            this.listView1.TabIndex = 1;
            // 
            // SpriteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.timelineView1);
            this.Name = "SpriteView";
            this.Size = new System.Drawing.Size(286, 675);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TimelineView timelineView1;
        private ListView listView1;
    }
}
