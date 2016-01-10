using Capslock.Windows.Forms.ImageEditor;
using Cyotek.Windows.Forms;

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
            this._colorGrid = new Cyotek.Windows.Forms.ColorGrid();
            this._drawingToolbox = new Capslock.Windows.Forms.ImageEditor.DrawingToolbox();
            this.SuspendLayout();
            // 
            // _colorGrid
            // 
            this._colorGrid.AutoAddColors = false;
            this._colorGrid.AutoSize = false;
            this._colorGrid.CellSize = new System.Drawing.Size(16, 16);
            this._colorGrid.Columns = 10;
            this._colorGrid.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this._colorGrid.Location = new System.Drawing.Point(0, 26);
            this._colorGrid.Margin = new System.Windows.Forms.Padding(1);
            this._colorGrid.Name = "_colorGrid";
            this._colorGrid.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this._colorGrid.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this._colorGrid.ShowCustomColors = false;
            this._colorGrid.Size = new System.Drawing.Size(163, 470);
            this._colorGrid.Spacing = new System.Drawing.Size(2, 2);
            this._colorGrid.TabIndex = 17;
            // 
            // _drawingToolbox
            // 
            this._drawingToolbox.AutoSize = true;
            this._drawingToolbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._drawingToolbox.Location = new System.Drawing.Point(0, 0);
            this._drawingToolbox.Margin = new System.Windows.Forms.Padding(0);
            this._drawingToolbox.Name = "_drawingToolbox";
            this._drawingToolbox.Size = new System.Drawing.Size(161, 25);
            this._drawingToolbox.TabIndex = 18;
            // 
            // ToolboxContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(169, 497);
            this.Controls.Add(this._drawingToolbox);
            this.Controls.Add(this._colorGrid);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToolboxContainer";
            this.Text = "ToolboxContainer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private DrawingToolbox _drawingToolbox;
        private ColorGrid _colorGrid;
    }
}