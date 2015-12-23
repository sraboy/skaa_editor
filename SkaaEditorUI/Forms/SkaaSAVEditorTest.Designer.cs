namespace SkaaEditorUI
{
    partial class SkaaSAVEditorTest
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
            this.btnLoadGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLoadGame
            // 
            this.btnLoadGame.Location = new System.Drawing.Point(496, 366);
            this.btnLoadGame.Name = "btnLoadGame";
            this.btnLoadGame.Size = new System.Drawing.Size(75, 23);
            this.btnLoadGame.TabIndex = 0;
            this.btnLoadGame.Text = "Load SAV";
            this.btnLoadGame.UseVisualStyleBackColor = true;
            this.btnLoadGame.Click += new System.EventHandler(this.btnLoadGame_Click);
            // 
            // SkaaSAVEditorTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 401);
            this.Controls.Add(this.btnLoadGame);
            this.Name = "SkaaSAVEditorTest";
            this.Text = "SkaaSAVEditorTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadGame;
    }
}