using Capslock.Windows.Forms.ImageEditor;

namespace SkaaEditorUI.Forms.DockContentControls
{
    partial class ImageEditorContainer
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
            this._imageEditorBox = new Capslock.Windows.Forms.ImageEditor.ImageEditorBox();
            this.SuspendLayout();
            // 
            // imageEditorBox
            // 
            this._imageEditorBox.ActivePrimaryColor = System.Drawing.Color.Empty;
            this._imageEditorBox.ActiveSecondaryColor = System.Drawing.Color.Empty;
            this._imageEditorBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right);
            this._imageEditorBox.AutoPan = false;
            this._imageEditorBox.Font = new System.Drawing.Font("Calibri", 12.75F);
            this._imageEditorBox.GridCellSize = 12;
            this._imageEditorBox.Location = new System.Drawing.Point(1, 1);
            this._imageEditorBox.Name = "imageEditorBox";
            this._imageEditorBox.Size = new System.Drawing.Size(778, 777);
            this._imageEditorBox.TabIndex = 9;
            // 
            // ImageEditorContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 778);
            this.Controls.Add(this._imageEditorBox);
            this.Name = "ImageEditorContainer";
            this.Text = "ImageEditorContainer";
            this.ResumeLayout(false);

        }
        #endregion

        private ImageEditorBox _imageEditorBox;
    }
}