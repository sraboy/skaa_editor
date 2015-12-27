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
namespace Capslock.WinForms.SpriteViewer
{
    partial class SpriteView
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
            this.listViewer1 = new Capslock.WinForms.SpriteViewer.ListView();
            this.timeline1 = new Capslock.WinForms.SpriteViewer.TimelineView();
            this.SuspendLayout();
            // 
            // listViewer1
            // 
            this.listViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewer1.AutoSize = true;
            this.listViewer1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listViewer1.Location = new System.Drawing.Point(0, 283);
            this.listViewer1.Name = "listViewer1";
            this.listViewer1.Size = new System.Drawing.Size(283, 535);
            this.listViewer1.TabIndex = 1;
            // 
            // timeline1
            // 
            this.timeline1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeline1.AutoSize = true;
            this.timeline1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timeline1.Location = new System.Drawing.Point(0, 2);
            this.timeline1.Name = "timeline1";
            this.timeline1.Size = new System.Drawing.Size(281, 326);
            this.timeline1.TabIndex = 0;
            // 
            // SpriteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewer1);
            this.Controls.Add(this.timeline1);
            this.Name = "SpriteView";
            this.Size = new System.Drawing.Size(283, 821);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TimelineView timeline1;
        private ListView listViewer1;
    }
}
