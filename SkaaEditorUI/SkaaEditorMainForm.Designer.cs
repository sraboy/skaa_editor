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

namespace SkaaEditorUI
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGameSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSpriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSpriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFrameTobmp32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportAllFramesTo32bppBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cbEdit = new System.Windows.Forms.CheckBox();
            this.btnFeatureTest = new System.Windows.Forms.Button();
            this.cbMultiColumn = new MultiColumnComboBox.MultiColumnComboBoxControl();
            this.timelineControl = new SkaaEditorControls.TimelineControl();
            this.imageEditorBox = new SkaaEditorControls.SkaaImageBox();
            this.skaaColorChooser = new SkaaEditorControls.SkaaColorChooser();
            this.toolStripStatLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.openGameSetToolStripMenuItem,
            this.openSpriteToolStripMenuItem,
            this.saveSpriteToolStripMenuItem,
            this.saveGameSetToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Visible = false;
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.newProjectToolStripMenuItem.Text = "Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openGameSetToolStripMenuItem
            // 
            this.openGameSetToolStripMenuItem.Name = "openGameSetToolStripMenuItem";
            this.openGameSetToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.openGameSetToolStripMenuItem.Text = "Open Game Set";
            this.openGameSetToolStripMenuItem.Click += new System.EventHandler(this.openGameSetToolStripMenuItem_Click);
            // 
            // openSpriteToolStripMenuItem
            // 
            this.openSpriteToolStripMenuItem.Name = "openSpriteToolStripMenuItem";
            this.openSpriteToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.openSpriteToolStripMenuItem.Text = "Open Sprite";
            this.openSpriteToolStripMenuItem.Click += new System.EventHandler(this.openSpriteToolStripMenuItem_Click);
            // 
            // saveSpriteToolStripMenuItem
            // 
            this.saveSpriteToolStripMenuItem.Name = "saveSpriteToolStripMenuItem";
            this.saveSpriteToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveSpriteToolStripMenuItem.Text = "Save Sprite";
            this.saveSpriteToolStripMenuItem.Click += new System.EventHandler(this.saveSpriteToolStripMenuItem_Click);
            // 
            // saveGameSetToolStripMenuItem
            // 
            this.saveGameSetToolStripMenuItem.Name = "saveGameSetToolStripMenuItem";
            this.saveGameSetToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveGameSetToolStripMenuItem.Text = "Save Game Set";
            this.saveGameSetToolStripMenuItem.Click += new System.EventHandler(this.saveGameSetToolStripMenuItem_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportBmpToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // exportBmpToolStripMenuItem
            // 
            this.exportBmpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentFrameTobmp32bppToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportAllFramesTo32bppBmpToolStripMenuItem});
            this.exportBmpToolStripMenuItem.Name = "exportBmpToolStripMenuItem";
            this.exportBmpToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exportBmpToolStripMenuItem.Text = "Export 32-bit BMP";
            // 
            // currentFrameTobmp32bppToolStripMenuItem
            // 
            this.currentFrameTobmp32bppToolStripMenuItem.Name = "currentFrameTobmp32bppToolStripMenuItem";
            this.currentFrameTobmp32bppToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.currentFrameTobmp32bppToolStripMenuItem.Text = "Current Frame";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // exportAllFramesTo32bppBmpToolStripMenuItem
            // 
            this.exportAllFramesTo32bppBmpToolStripMenuItem.Name = "exportAllFramesTo32bppBmpToolStripMenuItem";
            this.exportAllFramesTo32bppBmpToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exportAllFramesTo32bppBmpToolStripMenuItem.Text = "All Frames";
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 788);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1023, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "SkaaEditor ALPHA v0.1";
            // 
            // cbEdit
            // 
            this.cbEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEdit.AutoSize = true;
            this.cbEdit.Location = new System.Drawing.Point(919, 205);
            this.cbEdit.Name = "cbEdit";
            this.cbEdit.Size = new System.Drawing.Size(74, 17);
            this.cbEdit.TabIndex = 7;
            this.cbEdit.Text = "Edit Mode";
            this.cbEdit.UseVisualStyleBackColor = true;
            this.cbEdit.CheckedChanged += new System.EventHandler(this.cbEdit_CheckedChanged);
            // 
            // btnFeatureTest
            // 
            this.btnFeatureTest.Location = new System.Drawing.Point(918, 487);
            this.btnFeatureTest.Name = "btnFeatureTest";
            this.btnFeatureTest.Size = new System.Drawing.Size(75, 23);
            this.btnFeatureTest.TabIndex = 11;
            this.btnFeatureTest.Text = "Feature Test";
            this.btnFeatureTest.UseVisualStyleBackColor = true;
            this.btnFeatureTest.Visible = false;
            this.btnFeatureTest.Click += new System.EventHandler(this.btnFeatureTest_Click);
            // 
            // cbMultiColumn
            // 
            this.cbMultiColumn.FormattingEnabled = true;
            this.cbMultiColumn.Location = new System.Drawing.Point(198, 3);
            this.cbMultiColumn.Name = "cbMultiColumn";
            this.cbMultiColumn.Size = new System.Drawing.Size(689, 21);
            this.cbMultiColumn.TabIndex = 13;
            this.cbMultiColumn.SelectionChangeCommitted += new System.EventHandler(this.cbMultiColumn_SelectionChangeCommitted);
            // 
            // timelineControl
            // 
            this.timelineControl.ActiveFrame = null;
            this.timelineControl.ActiveSprite = null;
            this.timelineControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timelineControl.Location = new System.Drawing.Point(888, 27);
            this.timelineControl.Name = "timelineControl";
            this.timelineControl.Size = new System.Drawing.Size(129, 172);
            this.timelineControl.TabIndex = 12;
            // 
            // imageEditorBox
            // 
            this.imageEditorBox.ActiveColor = System.Drawing.Color.Empty;
            this.imageEditorBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageEditorBox.Font = new System.Drawing.Font("Calibri", 12.75F);
            this.imageEditorBox.GridCellSize = 12;
            this.imageEditorBox.IsDrawing = false;
            this.imageEditorBox.Location = new System.Drawing.Point(198, 28);
            this.imageEditorBox.Name = "imageEditorBox";
            this.imageEditorBox.Size = new System.Drawing.Size(689, 780);
            this.imageEditorBox.TabIndex = 8;
            // 
            // skaaColorChooser
            // 
            this.skaaColorChooser.ActiveColor = System.Drawing.Color.Empty;
            this.skaaColorChooser.Location = new System.Drawing.Point(5, 28);
            this.skaaColorChooser.Name = "skaaColorChooser";
            this.skaaColorChooser.Palette = null;
            this.skaaColorChooser.Size = new System.Drawing.Size(186, 758);
            this.skaaColorChooser.TabIndex = 5;
            // 
            // toolStripStatLbl
            // 
            this.toolStripStatLbl.Name = "toolStripStatLbl";
            this.toolStripStatLbl.Size = new System.Drawing.Size(1008, 17);
            this.toolStripStatLbl.Spring = true;
            this.toolStripStatLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SkaaEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 810);
            this.Controls.Add(this.cbMultiColumn);
            this.Controls.Add(this.btnFeatureTest);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.timelineControl);
            this.Controls.Add(this.imageEditorBox);
            this.Controls.Add(this.cbEdit);
            this.Controls.Add(this.skaaColorChooser);
            this.Controls.Add(this.mainMenuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "SkaaEditorMainForm";
            this.Text = "Skaa Editor for 7KAA";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkaaEditorMainForm_FormClosing);
            this.Load += new System.EventHandler(this.skaaEditorMainForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private SkaaEditorControls.SkaaColorChooser skaaColorChooser;
        private System.Windows.Forms.CheckBox cbEdit;
        private SkaaEditorControls.SkaaImageBox imageEditorBox;
        private System.Windows.Forms.Button btnFeatureTest;
        private SkaaEditorControls.TimelineControl timelineControl;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private MultiColumnComboBox.MultiColumnComboBoxControl cbMultiColumn;
        private System.Windows.Forms.ToolStripMenuItem openSpriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGameSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSpriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportBmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentFrameTobmp32bppToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportAllFramesTo32bppBmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatLbl;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}