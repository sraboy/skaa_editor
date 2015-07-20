using System.Windows.Forms;
namespace MultiplePictureBox
{
    partial class MultiplePictureBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picBoxFrame = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxFrame
            // 
            this.picBoxFrame.Location = new System.Drawing.Point(3, 3);
            this.picBoxFrame.Name = "picBoxFrame";
            this.picBoxFrame.Size = new System.Drawing.Size(121, 121);
            this.picBoxFrame.TabIndex = 3;
            this.picBoxFrame.TabStop = false;
            this.picBoxFrame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picBoxFrame_Click);
            // 
            // MultiplePictureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picBoxFrame);
            this.Name = "MultiplePictureBox";
            this.Size = new System.Drawing.Size(125, 125);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxFrame;
    }
}
