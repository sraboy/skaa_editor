using Capslock.Windows.Forms.ImageEditor;

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
            this._drawingToolbox = new Capslock.Windows.Forms.ImageEditor.DrawingToolbox();
            this._colorGrid = new SkaaEditorUI.Misc.ColorGrid();
            this.SuspendLayout();
            // 
            // _drawingToolbox
            // 
            this._drawingToolbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._drawingToolbox.Location = new System.Drawing.Point(-1, 1);
            this._drawingToolbox.Margin = new System.Windows.Forms.Padding(2);
            this._drawingToolbox.Name = "_drawingToolbox";
            this._drawingToolbox.Size = new System.Drawing.Size(175, 69);
            this._drawingToolbox.TabIndex = 18;
            // 
            // _colorGrid
            // 
            this._colorGrid.AutoAddColors = false;
            this._colorGrid.CellSize = new System.Drawing.Size(18, 18);
            this._colorGrid.Columns = 8;
            this._colorGrid.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this._colorGrid.Location = new System.Drawing.Point(-2, 76);
            this._colorGrid.Name = "_colorGrid";
            this._colorGrid.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this._colorGrid.ShowCustomColors = false;
            this._colorGrid.Size = new System.Drawing.Size(175, 679);
            this._colorGrid.TabIndex = 17;
            // 
            // ToolboxContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(159, 756);
            this.Controls.Add(this._drawingToolbox);
            this.Controls.Add(this._colorGrid);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ToolboxContainer";
            this.Text = "ToolboxContainer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private DrawingToolbox _drawingToolbox;
        private SkaaEditorUI.Misc.ColorGrid _colorGrid;
    }
}