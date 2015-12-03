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

using SkaaEditorControls;

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
            this.spriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openSpriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGameSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveGameSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSpriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSpriteFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFrameTobmp32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportAllFramesTo32bppBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnDebugAction = new System.Windows.Forms.Button();
            this.lbDebugActions = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripBtnNewProject = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnOpenProject = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnSaveProject = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnCloseProject = new System.Windows.Forms.ToolStripButton();
            this.drawingToolbox = new Capslock.WinForms.ImageEditor.DrawingToolbox();
            this.colorGridChooser = new SkaaEditorControls.SkaaColorChooser();
            this.timelineControl = new SkaaEditorControls.TimelineControl();
            this.imageEditorBox = new Capslock.WinForms.ImageEditor.ImageEditorBox();
            this.tsStatusLblFileType = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.saveToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.spriteToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.newProjectToolStripMenuItem.Text = "Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // spriteToolStripMenuItem
            // 
            this.spriteToolStripMenuItem.Enabled = false;
            this.spriteToolStripMenuItem.Name = "spriteToolStripMenuItem";
            this.spriteToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.spriteToolStripMenuItem.Text = "Sprite";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.toolStripSeparator3,
            this.openSpriteToolStripMenuItem,
            this.openGameSetToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openProjectToolStripMenuItem.Text = "&Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openFileToolStripMenuItem.Text = "File...";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
            // 
            // openSpriteToolStripMenuItem
            // 
            this.openSpriteToolStripMenuItem.Name = "openSpriteToolStripMenuItem";
            this.openSpriteToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openSpriteToolStripMenuItem.Text = "Sprite";
            this.openSpriteToolStripMenuItem.Click += new System.EventHandler(this.openSpriteToolStripMenuItem_Click);
            // 
            // openGameSetToolStripMenuItem
            // 
            this.openGameSetToolStripMenuItem.Name = "openGameSetToolStripMenuItem";
            this.openGameSetToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openGameSetToolStripMenuItem.Text = "Game Set";
            this.openGameSetToolStripMenuItem.Click += new System.EventHandler(this.openGameSetToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveGameSetToolStripMenuItem,
            this.saveSpriteToolStripMenuItem,
            this.saveSpriteFrameToolStripMenuItem});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveProjectToolStripMenuItem.Text = "&Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(148, 6);
            // 
            // saveGameSetToolStripMenuItem
            // 
            this.saveGameSetToolStripMenuItem.Name = "saveGameSetToolStripMenuItem";
            this.saveGameSetToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveGameSetToolStripMenuItem.Text = "Game Set";
            this.saveGameSetToolStripMenuItem.Click += new System.EventHandler(this.saveGameSetToolStripMenuItem_Click);
            // 
            // saveSpriteToolStripMenuItem
            // 
            this.saveSpriteToolStripMenuItem.Name = "saveSpriteToolStripMenuItem";
            this.saveSpriteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveSpriteToolStripMenuItem.Text = "Sprite";
            this.saveSpriteToolStripMenuItem.Click += new System.EventHandler(this.saveSpriteToolStripMenuItem_Click);
            // 
            // saveSpriteFrameToolStripMenuItem
            // 
            this.saveSpriteFrameToolStripMenuItem.Name = "saveSpriteFrameToolStripMenuItem";
            this.saveSpriteFrameToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveSpriteFrameToolStripMenuItem.Text = "Sprite Frame";
            this.saveSpriteFrameToolStripMenuItem.Click += new System.EventHandler(this.saveSpriteFrameToolStripMenuItem_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportPngToolStripMenuItem,
            this.loadPaletteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // exportPngToolStripMenuItem
            // 
            this.exportPngToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentFrameTobmp32bppToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportAllFramesTo32bppBmpToolStripMenuItem});
            this.exportPngToolStripMenuItem.Name = "exportPngToolStripMenuItem";
            this.exportPngToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exportPngToolStripMenuItem.Text = "Export PNG";
            // 
            // currentFrameTobmp32bppToolStripMenuItem
            // 
            this.currentFrameTobmp32bppToolStripMenuItem.Name = "currentFrameTobmp32bppToolStripMenuItem";
            this.currentFrameTobmp32bppToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.currentFrameTobmp32bppToolStripMenuItem.Text = "Current Frame";
            this.currentFrameTobmp32bppToolStripMenuItem.Click += new System.EventHandler(this.exportCurFrameToPngToolStripMenuItem_Click);
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
            this.exportAllFramesTo32bppBmpToolStripMenuItem.Click += new System.EventHandler(this.exportAllFramesToPngToolStripMenuItem_Click);
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatusLblFileType});
            this.statusStrip1.Location = new System.Drawing.Point(0, 809);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1023, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "SkaaEditor ALPHA v0.1";
            // 
            // btnDebugAction
            // 
            this.btnDebugAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDebugAction.AutoSize = true;
            this.btnDebugAction.BackColor = System.Drawing.Color.MistyRose;
            this.btnDebugAction.Location = new System.Drawing.Point(911, 643);
            this.btnDebugAction.Name = "btnDebugAction";
            this.btnDebugAction.Size = new System.Drawing.Size(82, 23);
            this.btnDebugAction.TabIndex = 11;
            this.btnDebugAction.Text = "Debug Action";
            this.btnDebugAction.UseVisualStyleBackColor = false;
            this.btnDebugAction.Visible = false;
            // 
            // lbDebugActions
            // 
            this.lbDebugActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDebugActions.FormattingEnabled = true;
            this.lbDebugActions.Location = new System.Drawing.Point(893, 672);
            this.lbDebugActions.Name = "lbDebugActions";
            this.lbDebugActions.Size = new System.Drawing.Size(124, 134);
            this.lbDebugActions.TabIndex = 15;
            this.lbDebugActions.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnNewProject,
            this.toolStripBtnOpenProject,
            this.toolStripBtnSaveProject,
            this.toolStripBtnCloseProject});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1023, 25);
            this.toolStrip1.TabIndex = 17;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripBtnNewProject
            // 
            this.toolStripBtnNewProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnNewProject.Image = global::SkaaEditorUI.Properties.Resources.MenuFileNewIcon;
            this.toolStripBtnNewProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnNewProject.Name = "toolStripBtnNewProject";
            this.toolStripBtnNewProject.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnNewProject.Text = "toolStripButton1";
            this.toolStripBtnNewProject.ToolTipText = "New Project";
            this.toolStripBtnNewProject.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // toolStripBtnOpenProject
            // 
            this.toolStripBtnOpenProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnOpenProject.Image = global::SkaaEditorUI.Properties.Resources.MenuFileOpenIcon;
            this.toolStripBtnOpenProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnOpenProject.Name = "toolStripBtnOpenProject";
            this.toolStripBtnOpenProject.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnOpenProject.Text = "toolStripButton2";
            this.toolStripBtnOpenProject.ToolTipText = "Open Project";
            this.toolStripBtnOpenProject.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // toolStripBtnSaveProject
            // 
            this.toolStripBtnSaveProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnSaveProject.Image = global::SkaaEditorUI.Properties.Resources.MenuFileSaveIcon;
            this.toolStripBtnSaveProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSaveProject.Name = "toolStripBtnSaveProject";
            this.toolStripBtnSaveProject.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnSaveProject.Text = "toolStripButton3";
            this.toolStripBtnSaveProject.ToolTipText = "Save Project";
            this.toolStripBtnSaveProject.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // toolStripBtnCloseProject
            // 
            this.toolStripBtnCloseProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnCloseProject.Image = global::SkaaEditorUI.Properties.Resources.MenuFileCloseIcon;
            this.toolStripBtnCloseProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnCloseProject.Name = "toolStripBtnCloseProject";
            this.toolStripBtnCloseProject.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnCloseProject.Text = "toolStripButton4";
            this.toolStripBtnCloseProject.ToolTipText = "Close Project";
            this.toolStripBtnCloseProject.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // drawingToolbox
            // 
            this.drawingToolbox.Location = new System.Drawing.Point(1, 52);
            this.drawingToolbox.Margin = new System.Windows.Forms.Padding(2);
            this.drawingToolbox.Name = "drawingToolbox";
            this.drawingToolbox.Size = new System.Drawing.Size(175, 69);
            this.drawingToolbox.TabIndex = 16;
            // 
            // colorGridChooser
            // 
            this.colorGridChooser.AutoAddColors = false;
            this.colorGridChooser.CellSize = new System.Drawing.Size(18, 18);
            this.colorGridChooser.Columns = 8;
            this.colorGridChooser.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this.colorGridChooser.Location = new System.Drawing.Point(0, 127);
            this.colorGridChooser.Name = "colorGridChooser";
            this.colorGridChooser.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this.colorGridChooser.ShowCustomColors = false;
            this.colorGridChooser.Size = new System.Drawing.Size(175, 679);
            this.colorGridChooser.TabIndex = 14;
            this.colorGridChooser.ColorChanged += new System.EventHandler(this.ColorGridChooser_ColorChanged);
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
            this.imageEditorBox.ActivePrimaryColor = System.Drawing.Color.Empty;
            this.imageEditorBox.ActiveSecondaryColor = System.Drawing.Color.Empty;
            this.imageEditorBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageEditorBox.AutoPan = false;
            this.imageEditorBox.Font = new System.Drawing.Font("Calibri", 12.75F);
            this.imageEditorBox.GridCellSize = 12;
            this.imageEditorBox.Location = new System.Drawing.Point(181, 52);
            this.imageEditorBox.Name = "imageEditorBox";
            this.imageEditorBox.Size = new System.Drawing.Size(706, 754);
            this.imageEditorBox.TabIndex = 8;
            // 
            // tsStatusLblFileType
            // 
            this.tsStatusLblFileType.Name = "tsStatusLblFileType";
            this.tsStatusLblFileType.Size = new System.Drawing.Size(1008, 17);
            this.tsStatusLblFileType.Spring = true;
            this.tsStatusLblFileType.Text = "No File Loaded";
            this.tsStatusLblFileType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SkaaEditorMainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 831);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.drawingToolbox);
            this.Controls.Add(this.lbDebugActions);
            this.Controls.Add(this.colorGridChooser);
            this.Controls.Add(this.btnDebugAction);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.timelineControl);
            this.Controls.Add(this.imageEditorBox);
            this.Controls.Add(this.mainMenuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "SkaaEditorMainForm";
            this.Text = "Skaa Editor for 7KAA";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkaaEditorMainForm_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SkaaEditorMainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SkaaEditorMainForm_DragEnter);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private Capslock.WinForms.ImageEditor.ImageEditorBox imageEditorBox;
        private System.Windows.Forms.Button btnDebugAction;
        private SkaaEditorControls.TimelineControl timelineControl;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentFrameTobmp32bppToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportAllFramesTo32bppBmpToolStripMenuItem;
        private SkaaColorChooser colorGridChooser;
        private System.Windows.Forms.ListBox lbDebugActions;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGameSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSpriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSpriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private Capslock.WinForms.ImageEditor.DrawingToolbox drawingToolbox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripBtnNewProject;
        private System.Windows.Forms.ToolStripButton toolStripBtnOpenProject;
        private System.Windows.Forms.ToolStripButton toolStripBtnSaveProject;
        private System.Windows.Forms.ToolStripButton toolStripBtnCloseProject;
        private System.Windows.Forms.ToolStripMenuItem saveSpriteFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPaletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLblFileType;
    }
}