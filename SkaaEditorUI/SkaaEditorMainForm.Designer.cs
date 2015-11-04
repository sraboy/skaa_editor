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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSPRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFrameTobmp32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.allFramesTobmp32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSPRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAllFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.skaaColorChooser = new SkaaColorChooser.SkaaColorChooser();
            this.cbEdit = new System.Windows.Forms.CheckBox();
            this.imageEditorBox = new SkaaEditor.SkaaImageBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cbMultiColumn = new MultiColumnComboBox.MultiColumnComboBoxControl();
            this.timelineControl = new Timeline.TimelineControl();
            this.mainMenuStrip.SuspendLayout();
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
            this.openToolStripMenuItem,
            this.saveExportToolStripMenuItem,
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
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.newProjectToolStripMenuItem.Text = "Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem,
            this.openSPRToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open / Import";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // openSPRToolStripMenuItem
            // 
            this.openSPRToolStripMenuItem.Name = "openSPRToolStripMenuItem";
            this.openSPRToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openSPRToolStripMenuItem.Text = "Open SPR";
            this.openSPRToolStripMenuItem.Click += new System.EventHandler(this.openSPRToolStripMenuItem_Click);
            // 
            // saveExportToolStripMenuItem
            // 
            this.saveExportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportBmpToolStripMenuItem,
            this.saveSPRToolStripMenuItem,
            this.saveProjectToolStripMenuItem});
            this.saveExportToolStripMenuItem.Name = "saveExportToolStripMenuItem";
            this.saveExportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveExportToolStripMenuItem.Text = "Save / Export";
            // 
            // exportBmpToolStripMenuItem
            // 
            this.exportBmpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentFrameTobmp32bppToolStripMenuItem,
            this.toolStripSeparator2,
            this.allFramesTobmp32bppToolStripMenuItem});
            this.exportBmpToolStripMenuItem.Name = "exportBmpToolStripMenuItem";
            this.exportBmpToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exportBmpToolStripMenuItem.Text = "Export 32-bit BMP";
            // 
            // currentFrameTobmp32bppToolStripMenuItem
            // 
            this.currentFrameTobmp32bppToolStripMenuItem.Name = "currentFrameTobmp32bppToolStripMenuItem";
            this.currentFrameTobmp32bppToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.currentFrameTobmp32bppToolStripMenuItem.Text = "Current Frame";
            this.currentFrameTobmp32bppToolStripMenuItem.Click += new System.EventHandler(this.exportCurFrameTo32bppBmpToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // allFramesTobmp32bppToolStripMenuItem
            // 
            this.allFramesTobmp32bppToolStripMenuItem.Name = "allFramesTobmp32bppToolStripMenuItem";
            this.allFramesTobmp32bppToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.allFramesTobmp32bppToolStripMenuItem.Text = "All Frames";
            this.allFramesTobmp32bppToolStripMenuItem.Click += new System.EventHandler(this.exportAllFramesTo32bppBmpToolStripMenuItem_Click);
            // 
            // saveSPRToolStripMenuItem
            // 
            this.saveSPRToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFrameToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveAllFramesToolStripMenuItem});
            this.saveSPRToolStripMenuItem.Name = "saveSPRToolStripMenuItem";
            this.saveSPRToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.saveSPRToolStripMenuItem.Text = "Save SPR";
            // 
            // saveFrameToolStripMenuItem
            // 
            this.saveFrameToolStripMenuItem.Name = "saveFrameToolStripMenuItem";
            this.saveFrameToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.saveFrameToolStripMenuItem.Text = "Current Frame";
            this.saveFrameToolStripMenuItem.Click += new System.EventHandler(this.saveSPRFrameToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // saveAllFramesToolStripMenuItem
            // 
            this.saveAllFramesToolStripMenuItem.Name = "saveAllFramesToolStripMenuItem";
            this.saveAllFramesToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.saveAllFramesToolStripMenuItem.Text = "All Frames";
            this.saveAllFramesToolStripMenuItem.Click += new System.EventHandler(this.saveSPRAllFramesToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPaletteToolStripMenuItem,
            this.loadSetToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // loadPaletteToolStripMenuItem
            // 
            this.loadPaletteToolStripMenuItem.Name = "loadPaletteToolStripMenuItem";
            this.loadPaletteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadPaletteToolStripMenuItem.Text = "Load Palette";
            this.loadPaletteToolStripMenuItem.Click += new System.EventHandler(this.loadPaletteToolStripMenuItem_Click);
            // 
            // loadSetToolStripMenuItem
            // 
            this.loadSetToolStripMenuItem.Name = "loadSetToolStripMenuItem";
            this.loadSetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadSetToolStripMenuItem.Text = "Load Set";
            this.loadSetToolStripMenuItem.Click += new System.EventHandler(this.loadSetToolStripMenuItem_Click);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 788);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1023, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "SkaaEditor ALPHA v0.1";
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(895, 306);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            // SkaaEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 810);
            this.Controls.Add(this.cbMultiColumn);
            this.Controls.Add(this.button1);
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
            this.Load += new System.EventHandler(this.skaaEditorMainForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPaletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private SkaaColorChooser.SkaaColorChooser skaaColorChooser;
        private System.Windows.Forms.CheckBox cbEdit;
        private SkaaImageBox imageEditorBox;
        private System.Windows.Forms.Button button1;
        private Timeline.TimelineControl timelineControl;
        private System.Windows.Forms.ToolStripMenuItem loadSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportBmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentFrameTobmp32bppToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem allFramesTobmp32bppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSPRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveAllFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSPRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private MultiColumnComboBox.MultiColumnComboBoxControl cbMultiColumn;
    }
}