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
            this.btnLoadSPR = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiplePictureBox1 = new MultiplePictureBox.MultiplePictureBox();
            this.skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
            this.loadPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadSPR
            // 
            this.btnLoadSPR.Location = new System.Drawing.Point(893, 158);
            this.btnLoadSPR.Name = "btnLoadSPR";
            this.btnLoadSPR.Size = new System.Drawing.Size(125, 51);
            this.btnLoadSPR.TabIndex = 0;
            this.btnLoadSPR.Text = "Load SPR";
            this.btnLoadSPR.UseVisualStyleBackColor = true;
            this.btnLoadSPR.Click += new System.EventHandler(this.btnLoadSPR_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(194, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(693, 759);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1023, 24);
            this.mainMenuStrip.TabIndex = 3;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPaletteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // multiplePictureBox1
            // 
            this.multiplePictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.multiplePictureBox1.Location = new System.Drawing.Point(893, 27);
            this.multiplePictureBox1.Name = "multiplePictureBox1";
            this.multiplePictureBox1.Size = new System.Drawing.Size(125, 125);
            this.multiplePictureBox1.TabIndex = 4;
            // 
            // skaaColorChooser1
            // 
            this.skaaColorChooser1.Location = new System.Drawing.Point(5, 28);
            this.skaaColorChooser1.Name = "skaaColorChooser1";
            this.skaaColorChooser1.Palette = null;
            this.skaaColorChooser1.Size = new System.Drawing.Size(186, 758);
            this.skaaColorChooser1.TabIndex = 5;
            // 
            // loadPaletteToolStripMenuItem
            // 
            this.loadPaletteToolStripMenuItem.Name = "loadPaletteToolStripMenuItem";
            this.loadPaletteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadPaletteToolStripMenuItem.Text = "Load Palette";
            this.loadPaletteToolStripMenuItem.Click += new System.EventHandler(this.loadPaletteToolStripMenuItem_Click);
            // 
            // SkaaEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 788);
            this.Controls.Add(this.skaaColorChooser1);
            this.Controls.Add(this.multiplePictureBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnLoadSPR);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "SkaaEditorMainForm";
            this.Text = "Skaa Editor for 7KAA";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadSPR;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private MultiplePictureBox.MultiplePictureBox multiplePictureBox1;
        private SkaaColorChooser.SkaaColorChooser skaaColorChooser1;
        private System.Windows.Forms.ToolStripMenuItem loadPaletteToolStripMenuItem;
    }
}

