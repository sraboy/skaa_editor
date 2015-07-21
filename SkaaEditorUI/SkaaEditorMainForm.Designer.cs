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

namespace SkaaEditor
{
    partial class SkaaEditorMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkaaEditorMainForm));
            this.btnLoadSPR = new System.Windows.Forms.Button();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wholeSpriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bmp32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbEdit = new System.Windows.Forms.CheckBox();
            this.skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
            this.multiplePictureBox1 = new MultiplePictureBox.MultiplePictureBox();
            this.skaaImageBox1 = new SkaaEditor.SkaaImageBox();
            this.skaaFrameViewer1 = new SkaaFrameViewer.SkaaFrameViewer();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadSPR
            // 
            this.btnLoadSPR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadSPR.Location = new System.Drawing.Point(893, 158);
            this.btnLoadSPR.Name = "btnLoadSPR";
            this.btnLoadSPR.Size = new System.Drawing.Size(125, 51);
            this.btnLoadSPR.TabIndex = 0;
            this.btnLoadSPR.Text = "Load SPR";
            this.btnLoadSPR.UseVisualStyleBackColor = true;
            this.btnLoadSPR.Click += new System.EventHandler(this.btnLoadSPR_Click);
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1023, 24);
            this.mainMenuStrip.TabIndex = 3;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frameToolStripMenuItem,
            this.wholeSpriteToolStripMenuItem});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveToolStripMenuItem.Text = "Save (Native SPR)";
            // 
            // frameToolStripMenuItem
            // 
            this.frameToolStripMenuItem.Name = "frameToolStripMenuItem";
            this.frameToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.frameToolStripMenuItem.Text = "Current Frame";
            this.frameToolStripMenuItem.Click += new System.EventHandler(this.saveFrameToolStripMenuItem_Click);
            // 
            // wholeSpriteToolStripMenuItem
            // 
            this.wholeSpriteToolStripMenuItem.Name = "wholeSpriteToolStripMenuItem";
            this.wholeSpriteToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.wholeSpriteToolStripMenuItem.Text = "Whole Sprite";
            // 
            // exportAsToolStripMenuItem
            // 
            this.exportAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bmp32bppToolStripMenuItem});
            this.exportAsToolStripMenuItem.Name = "exportAsToolStripMenuItem";
            this.exportAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exportAsToolStripMenuItem.Text = "Export As";
            // 
            // bmp32bppToolStripMenuItem
            // 
            this.bmp32bppToolStripMenuItem.Name = "bmp32bppToolStripMenuItem";
            this.bmp32bppToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.bmp32bppToolStripMenuItem.Text = "32-bit Bitmap";
            this.bmp32bppToolStripMenuItem.Click += new System.EventHandler(this.exportBmp32bppToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPaletteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // loadPaletteToolStripMenuItem
            // 
            this.loadPaletteToolStripMenuItem.Name = "loadPaletteToolStripMenuItem";
            this.loadPaletteToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.loadPaletteToolStripMenuItem.Text = "Load Palette";
            this.loadPaletteToolStripMenuItem.Click += new System.EventHandler(this.loadPaletteToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showGridToolStripMenuItem
            // 
            this.showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
            this.showGridToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.showGridToolStripMenuItem.Text = "Show Grid";
            this.showGridToolStripMenuItem.Click += new System.EventHandler(this.showGridToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // cbEdit
            // 
            this.cbEdit.AutoSize = true;
            this.cbEdit.Location = new System.Drawing.Point(893, 216);
            this.cbEdit.Name = "cbEdit";
            this.cbEdit.Size = new System.Drawing.Size(74, 17);
            this.cbEdit.TabIndex = 7;
            this.cbEdit.Text = "Edit Mode";
            this.cbEdit.UseVisualStyleBackColor = true;
            this.cbEdit.CheckedChanged += new System.EventHandler(this.cbEdit_CheckedChanged);
            // 
            // skaaColorChooser1
            // 
            this.skaaColorChooser1.ActiveColor = System.Drawing.Color.Empty;
            this.skaaColorChooser1.Location = new System.Drawing.Point(5, 28);
            this.skaaColorChooser1.Name = "skaaColorChooser1";
            this.skaaColorChooser1.Palette = null;
            this.skaaColorChooser1.Size = new System.Drawing.Size(186, 758);
            this.skaaColorChooser1.TabIndex = 5;
            // 
            // multiplePictureBox1
            // 
            this.multiplePictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.multiplePictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.multiplePictureBox1.Location = new System.Drawing.Point(893, 27);
            this.multiplePictureBox1.Name = "multiplePictureBox1";
            this.multiplePictureBox1.Size = new System.Drawing.Size(125, 125);
            this.multiplePictureBox1.TabIndex = 4;
            // 
            // skaaImageBox1
            // 
            this.skaaImageBox1.ActiveColor = System.Drawing.Color.Empty;
            this.skaaImageBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skaaImageBox1.GridCellSize = 12;
            this.skaaImageBox1.IsDrawing = false;
            this.skaaImageBox1.Location = new System.Drawing.Point(198, 28);
            this.skaaImageBox1.Name = "skaaImageBox1";
            this.skaaImageBox1.Size = new System.Drawing.Size(689, 758);
            this.skaaImageBox1.TabIndex = 8;
            // 
            // skaaFrameViewer1
            // 
            this.skaaFrameViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skaaFrameViewer1.Location = new System.Drawing.Point(893, 356);
            this.skaaFrameViewer1.Name = "skaaFrameViewer1";
            this.skaaFrameViewer1.Size = new System.Drawing.Size(125, 125);
            this.skaaFrameViewer1.TabIndex = 9;
            // 
            // SkaaEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 788);
            this.Controls.Add(this.skaaFrameViewer1);
            this.Controls.Add(this.skaaImageBox1);
            this.Controls.Add(this.cbEdit);
            this.Controls.Add(this.skaaColorChooser1);
            this.Controls.Add(this.multiplePictureBox1);
            this.Controls.Add(this.btnLoadSPR);
            this.Controls.Add(this.mainMenuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "SkaaEditorMainForm";
            this.Text = "Skaa Editor for 7KAA";
            this.Load += new System.EventHandler(this.skaaEditorMainForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadSPR;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private MultiplePictureBox.MultiplePictureBox multiplePictureBox1;
        private SkaaColorChooser.SkaaColorChooser skaaColorChooser1;
        private System.Windows.Forms.ToolStripMenuItem loadPaletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbEdit;
        private SkaaImageBox skaaImageBox1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bmp32bppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wholeSpriteToolStripMenuItem;
        private SkaaFrameViewer.SkaaFrameViewer skaaFrameViewer1;
    }
}