namespace SkaaEditorControls
{
    partial class DrawingToolbox
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
            this.button2 = new System.Windows.Forms.CheckBox();
            this.btnFillTool = new System.Windows.Forms.CheckBox();
            this.btnPencilTool = new System.Windows.Forms.CheckBox();
            this.btnPanTool = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Appearance = System.Windows.Forms.Appearance.Button;
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(81, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 25);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnFillTool
            // 
            this.btnFillTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnFillTool.Image = global::SkaaEditorControls.Properties.Resources.PaintBucketIcon;
            this.btnFillTool.Location = new System.Drawing.Point(55, 3);
            this.btnFillTool.Name = "btnFillTool";
            this.btnFillTool.Size = new System.Drawing.Size(25, 25);
            this.btnFillTool.TabIndex = 2;
            this.btnFillTool.UseVisualStyleBackColor = true;
            this.btnFillTool.Click += new System.EventHandler(this.btnFillTool_Click);
            // 
            // btnPencilTool
            // 
            this.btnPencilTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnPencilTool.Image = global::SkaaEditorControls.Properties.Resources.PencilToolIcon;
            this.btnPencilTool.Location = new System.Drawing.Point(29, 3);
            this.btnPencilTool.Name = "btnPencilTool";
            this.btnPencilTool.Size = new System.Drawing.Size(25, 25);
            this.btnPencilTool.TabIndex = 1;
            this.btnPencilTool.UseVisualStyleBackColor = true;
            this.btnPencilTool.Click += new System.EventHandler(this.btnPencilTool_Click);
            // 
            // btnPanTool
            // 
            this.btnPanTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnPanTool.Image = global::SkaaEditorControls.Properties.Resources.PanToolIcon;
            this.btnPanTool.Location = new System.Drawing.Point(3, 3);
            this.btnPanTool.Name = "btnPanTool";
            this.btnPanTool.Size = new System.Drawing.Size(25, 25);
            this.btnPanTool.TabIndex = 0;
            this.btnPanTool.UseVisualStyleBackColor = true;
            this.btnPanTool.Click += new System.EventHandler(this.btnPanTool_Click);
            // 
            // DrawingToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnFillTool);
            this.Controls.Add(this.btnPencilTool);
            this.Controls.Add(this.btnPanTool);
            this.Name = "DrawingToolbox";
            this.Size = new System.Drawing.Size(109, 199);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Button btnPanTool;
        private System.Windows.Forms.CheckBox btnPanTool;
        private System.Windows.Forms.CheckBox btnPencilTool;
        private System.Windows.Forms.CheckBox button2;
        private System.Windows.Forms.CheckBox btnFillTool;
    }
}
