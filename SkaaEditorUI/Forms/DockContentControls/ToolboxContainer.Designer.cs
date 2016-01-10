using Capslock.Windows.Forms.ImageEditor;
using SkaaEditorControls;

namespace SkaaEditorUI.Forms.DockContentControls
{
    partial class ToolboxContainer
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
            this._drawingToolbox = new DrawingToolbox();
            this._colorGridChooser = new SkaaColorChooser();
            this.SuspendLayout();
            // 
            // drawingToolbox
            // 
            this._drawingToolbox.Location = new System.Drawing.Point(1, 1);
            this._drawingToolbox.Margin = new System.Windows.Forms.Padding(2);
            this._drawingToolbox.Name = "drawingToolbox";
            this._drawingToolbox.Size = new System.Drawing.Size(175, 69);
            this._drawingToolbox.TabIndex = 18;
            // 
            // colorGridChooser
            // 
            this._colorGridChooser.AutoAddColors = false;
            this._colorGridChooser.CellSize = new System.Drawing.Size(18, 18);
            this._colorGridChooser.Columns = 8;
            this._colorGridChooser.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this._colorGridChooser.Location = new System.Drawing.Point(0, 76);
            this._colorGridChooser.Name = "colorGridChooser";
            this._colorGridChooser.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this._colorGridChooser.ShowCustomColors = false;
            this._colorGridChooser.Size = new System.Drawing.Size(175, 679);
            this._colorGridChooser.TabIndex = 17;
            // 
            // ToolboxContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 756);
            this.Controls.Add(this._drawingToolbox);
            this.Controls.Add(this._colorGridChooser);
            this.Name = "ToolboxContainer";
            this.Text = "ToolboxContainer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private DrawingToolbox _drawingToolbox;
        private SkaaColorChooser _colorGridChooser;
    }
}