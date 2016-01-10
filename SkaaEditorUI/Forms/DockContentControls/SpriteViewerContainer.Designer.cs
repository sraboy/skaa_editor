using Capslock.Windows.Forms.SpriteViewer;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI.Forms.DockContentControls
{
    partial class SpriteViewerContainer
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
            this._spriteViewer = new Capslock.Windows.Forms.SpriteViewer.SpriteViewer();
            this.SuspendLayout();
            // 
            // spriteViewer
            // 
            this._spriteViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._spriteViewer.Location = new System.Drawing.Point(-5, 0);
            this._spriteViewer.Name = "spriteViewer";
            this._spriteViewer.Size = new System.Drawing.Size(286, 675);
            this._spriteViewer.TabIndex = 20;
            // 
            // SpriteViewerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 674);
            this.Controls.Add(this._spriteViewer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SpriteViewerContainer";
            this.Text = "SpriteViewerContainer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private SpriteViewer _spriteViewer;
        private MultiImagePresenterBase _activeSprite;
    }
}