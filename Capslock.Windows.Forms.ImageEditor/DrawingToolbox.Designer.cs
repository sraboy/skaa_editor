namespace Capslock.Windows.Forms.ImageEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawingToolbox));
            this.btnSelectRectangleTool = new System.Windows.Forms.CheckBox();
            this.btnLineTool = new System.Windows.Forms.CheckBox();
            this.btnPaintBucketTool = new System.Windows.Forms.CheckBox();
            this.btnPencilTool = new System.Windows.Forms.CheckBox();
            this.btnPanTool = new System.Windows.Forms.CheckBox();
            this.btnResizeImageTool = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectRectangleTool
            // 
            this.btnSelectRectangleTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSelectRectangleTool.Image = global::Capslock.Windows.Forms.ImageEditor.Properties.Resources.RectangleSelectToolIcon;
            this.btnSelectRectangleTool.Location = new System.Drawing.Point(3, 34);
            this.btnSelectRectangleTool.Name = "btnSelectRectangleTool";
            this.btnSelectRectangleTool.Size = new System.Drawing.Size(25, 25);
            this.btnSelectRectangleTool.TabIndex = 4;
            this.btnSelectRectangleTool.UseVisualStyleBackColor = true;
            this.btnSelectRectangleTool.CheckedChanged += new System.EventHandler(this.btnTool_Click);
            // 
            // btnLineTool
            // 
            this.btnLineTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnLineTool.Image = ((System.Drawing.Image)(resources.GetObject("btnLineTool.Image")));
            this.btnLineTool.Location = new System.Drawing.Point(81, 3);
            this.btnLineTool.Name = "btnLineTool";
            this.btnLineTool.Size = new System.Drawing.Size(25, 25);
            this.btnLineTool.TabIndex = 3;
            this.btnLineTool.UseVisualStyleBackColor = true;
            this.btnLineTool.CheckedChanged += new System.EventHandler(this.btnTool_Click);
            // 
            // btnPaintBucketTool
            // 
            this.btnPaintBucketTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnPaintBucketTool.Image = global::Capslock.Windows.Forms.ImageEditor.Properties.Resources.PaintBucketIcon;
            this.btnPaintBucketTool.Location = new System.Drawing.Point(55, 3);
            this.btnPaintBucketTool.Name = "btnPaintBucketTool";
            this.btnPaintBucketTool.Size = new System.Drawing.Size(25, 25);
            this.btnPaintBucketTool.TabIndex = 2;
            this.btnPaintBucketTool.UseVisualStyleBackColor = true;
            this.btnPaintBucketTool.Click += new System.EventHandler(this.btnTool_Click);
            // 
            // btnPencilTool
            // 
            this.btnPencilTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnPencilTool.Image = global::Capslock.Windows.Forms.ImageEditor.Properties.Resources.PencilToolIcon;
            this.btnPencilTool.Location = new System.Drawing.Point(29, 3);
            this.btnPencilTool.Name = "btnPencilTool";
            this.btnPencilTool.Size = new System.Drawing.Size(25, 25);
            this.btnPencilTool.TabIndex = 1;
            this.btnPencilTool.UseVisualStyleBackColor = true;
            this.btnPencilTool.Click += new System.EventHandler(this.btnTool_Click);
            // 
            // btnPanTool
            // 
            this.btnPanTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnPanTool.Image = global::Capslock.Windows.Forms.ImageEditor.Properties.Resources.PanToolIcon;
            this.btnPanTool.Location = new System.Drawing.Point(3, 3);
            this.btnPanTool.Name = "btnPanTool";
            this.btnPanTool.Size = new System.Drawing.Size(25, 25);
            this.btnPanTool.TabIndex = 0;
            this.btnPanTool.UseVisualStyleBackColor = true;
            this.btnPanTool.Click += new System.EventHandler(this.btnTool_Click);
            // 
            // btnResizeImageTool
            // 
            this.btnResizeImageTool.Image = global::Capslock.Windows.Forms.ImageEditor.Properties.Resources.ResizeIcon;
            this.btnResizeImageTool.Location = new System.Drawing.Point(29, 34);
            this.btnResizeImageTool.Name = "btnResizeImageTool";
            this.btnResizeImageTool.Size = new System.Drawing.Size(25, 25);
            this.btnResizeImageTool.TabIndex = 6;
            this.btnResizeImageTool.UseVisualStyleBackColor = true;
            this.btnResizeImageTool.Click += new System.EventHandler(this.btnTool_Click);
            // 
            // DrawingToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnResizeImageTool);
            this.Controls.Add(this.btnSelectRectangleTool);
            this.Controls.Add(this.btnLineTool);
            this.Controls.Add(this.btnPaintBucketTool);
            this.Controls.Add(this.btnPencilTool);
            this.Controls.Add(this.btnPanTool);
            this.Name = "DrawingToolbox";
            this.Size = new System.Drawing.Size(109, 68);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Button btnPanTool;
        private System.Windows.Forms.CheckBox btnPanTool;
        private System.Windows.Forms.CheckBox btnPencilTool;
        private System.Windows.Forms.CheckBox btnLineTool;
        private System.Windows.Forms.CheckBox btnPaintBucketTool;
        private System.Windows.Forms.CheckBox btnSelectRectangleTool;
        private System.Windows.Forms.Button btnResizeImageTool;
    }
}
