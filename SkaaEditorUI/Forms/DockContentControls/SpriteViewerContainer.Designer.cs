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
            this.spriteViewer = new Capslock.Windows.Forms.SpriteViewer.SpriteViewer();
            this.SuspendLayout();
            // 
            // userControl11
            // 
            this.spriteViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spriteViewer.Location = new System.Drawing.Point(0, 0);
            this.spriteViewer.Margin = new System.Windows.Forms.Padding(0);
            this.spriteViewer.Name = "spriteViewer";
            this.spriteViewer.Size = new System.Drawing.Size(272, 686);
            this.spriteViewer.TabIndex = 0;
            // 
            // SpriteViewerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 686);
            this.Controls.Add(this.spriteViewer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SpriteViewerContainer";
            this.Text = "SpriteViewerContainer";
            this.ResumeLayout(false);

        }

        #endregion
        private MultiImagePresenterBase _activeSprite;
        private SpriteViewer spriteViewer;
    }
}