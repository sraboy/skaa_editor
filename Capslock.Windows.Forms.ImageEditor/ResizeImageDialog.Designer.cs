namespace Capslock.Windows.Forms.ImageEditor
{
    partial class ResizeImageDialog
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
            this.cbMaintainAspectRatio = new System.Windows.Forms.CheckBox();
            this.newWidthLabel1 = new System.Windows.Forms.Label();
            this.newHeightLabel1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pixelWidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.pixelHeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.rbAbsoluteSize = new System.Windows.Forms.RadioButton();
            this.pixelsLabel1 = new System.Windows.Forms.Label();
            this.percentUpDown = new System.Windows.Forms.NumericUpDown();
            this.percentSignLabel = new System.Windows.Forms.Label();
            this.pixelsLabel2 = new System.Windows.Forms.Label();
            this.rbPercentage = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pixelWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pixelHeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMaintainAspectRatio
            // 
            this.cbMaintainAspectRatio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbMaintainAspectRatio.Location = new System.Drawing.Point(31, 63);
            this.cbMaintainAspectRatio.Name = "cbMaintainAspectRatio";
            this.cbMaintainAspectRatio.Size = new System.Drawing.Size(122, 15);
            this.cbMaintainAspectRatio.TabIndex = 25;
            this.cbMaintainAspectRatio.Text = "Maintain aspect ratio";
            this.cbMaintainAspectRatio.CheckedChanged += new System.EventHandler(this.cbMaintainAspectRatio_CheckedChanged);
            // 
            // newWidthLabel1
            // 
            this.newWidthLabel1.Location = new System.Drawing.Point(32, 85);
            this.newWidthLabel1.Name = "newWidthLabel1";
            this.newWidthLabel1.Size = new System.Drawing.Size(79, 16);
            this.newWidthLabel1.TabIndex = 0;
            this.newWidthLabel1.Text = "Width";
            this.newWidthLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // newHeightLabel1
            // 
            this.newHeightLabel1.Location = new System.Drawing.Point(32, 109);
            this.newHeightLabel1.Name = "newHeightLabel1";
            this.newHeightLabel1.Size = new System.Drawing.Size(79, 16);
            this.newHeightLabel1.TabIndex = 3;
            this.newHeightLabel1.Text = "Height";
            this.newHeightLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(81, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(159, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pixelWidthUpDown
            // 
            this.pixelWidthUpDown.Location = new System.Drawing.Point(120, 84);
            this.pixelWidthUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.pixelWidthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pixelWidthUpDown.Name = "pixelWidthUpDown";
            this.pixelWidthUpDown.Size = new System.Drawing.Size(72, 20);
            this.pixelWidthUpDown.TabIndex = 1;
            this.pixelWidthUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.pixelWidthUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.pixelWidthUpDown.ValueChanged += new System.EventHandler(this.pixelWidthUpDown_ValueChanged);
            this.pixelWidthUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.pixelWidthUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnUpDownKeyUp);
            this.pixelWidthUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // pixelHeightUpDown
            // 
            this.pixelHeightUpDown.Location = new System.Drawing.Point(120, 108);
            this.pixelHeightUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.pixelHeightUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pixelHeightUpDown.Name = "pixelHeightUpDown";
            this.pixelHeightUpDown.Size = new System.Drawing.Size(72, 20);
            this.pixelHeightUpDown.TabIndex = 4;
            this.pixelHeightUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.pixelHeightUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.pixelHeightUpDown.ValueChanged += new System.EventHandler(this.pixelHeightUpDown_ValueChanged);
            this.pixelHeightUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.pixelHeightUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnUpDownKeyUp);
            this.pixelHeightUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // rbAbsoluteSize
            // 
            this.rbAbsoluteSize.AutoSize = true;
            this.rbAbsoluteSize.Checked = true;
            this.rbAbsoluteSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbAbsoluteSize.Location = new System.Drawing.Point(12, 39);
            this.rbAbsoluteSize.Name = "rbAbsoluteSize";
            this.rbAbsoluteSize.Size = new System.Drawing.Size(107, 18);
            this.rbAbsoluteSize.TabIndex = 24;
            this.rbAbsoluteSize.TabStop = true;
            this.rbAbsoluteSize.Text = "By absolute size";
            this.rbAbsoluteSize.CheckedChanged += new System.EventHandler(this.OnRadioButtonCheckedChanged);
            // 
            // pixelsLabel1
            // 
            this.pixelsLabel1.Location = new System.Drawing.Point(200, 85);
            this.pixelsLabel1.Name = "pixelsLabel1";
            this.pixelsLabel1.Size = new System.Drawing.Size(36, 19);
            this.pixelsLabel1.TabIndex = 2;
            this.pixelsLabel1.Text = "pixels";
            this.pixelsLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // percentUpDown
            // 
            this.percentUpDown.Enabled = false;
            this.percentUpDown.Location = new System.Drawing.Point(124, 15);
            this.percentUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.percentUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.percentUpDown.Name = "percentUpDown";
            this.percentUpDown.Size = new System.Drawing.Size(72, 20);
            this.percentUpDown.TabIndex = 23;
            this.percentUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.percentUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.percentUpDown.ValueChanged += new System.EventHandler(this.percentUpDown_ValueChanged);
            this.percentUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.percentUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnUpDownKeyUp);
            // 
            // percentSignLabel
            // 
            this.percentSignLabel.Location = new System.Drawing.Point(204, 16);
            this.percentSignLabel.Name = "percentSignLabel";
            this.percentSignLabel.Size = new System.Drawing.Size(32, 16);
            this.percentSignLabel.TabIndex = 13;
            this.percentSignLabel.Text = "%";
            this.percentSignLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pixelsLabel2
            // 
            this.pixelsLabel2.Location = new System.Drawing.Point(200, 112);
            this.pixelsLabel2.Name = "pixelsLabel2";
            this.pixelsLabel2.Size = new System.Drawing.Size(36, 16);
            this.pixelsLabel2.TabIndex = 5;
            this.pixelsLabel2.Text = "pixels";
            this.pixelsLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbPercentage
            // 
            this.rbPercentage.AutoSize = true;
            this.rbPercentage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbPercentage.Location = new System.Drawing.Point(12, 12);
            this.rbPercentage.Name = "rbPercentage";
            this.rbPercentage.Size = new System.Drawing.Size(100, 18);
            this.rbPercentage.TabIndex = 22;
            this.rbPercentage.Text = "By percentage";
            this.rbPercentage.CheckedChanged += new System.EventHandler(this.OnRadioButtonCheckedChanged);
            // 
            // ResizeImageDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(243, 193);
            this.Controls.Add(this.pixelsLabel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbAbsoluteSize);
            this.Controls.Add(this.rbPercentage);
            this.Controls.Add(this.pixelWidthUpDown);
            this.Controls.Add(this.pixelHeightUpDown);
            this.Controls.Add(this.pixelsLabel1);
            this.Controls.Add(this.newHeightLabel1);
            this.Controls.Add(this.newWidthLabel1);
            this.Controls.Add(this.cbMaintainAspectRatio);
            this.Controls.Add(this.percentUpDown);
            this.Controls.Add(this.percentSignLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResizeImageDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ResizeImage";
            ((System.ComponentModel.ISupportInitialize)(this.pixelWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pixelHeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label percentSignLabel;
        private System.Windows.Forms.NumericUpDown percentUpDown;
        private System.Windows.Forms.RadioButton rbAbsoluteSize;
        private System.Windows.Forms.CheckBox cbMaintainAspectRatio;
        private System.Windows.Forms.Label newWidthLabel1;
        private System.Windows.Forms.Label newHeightLabel1;
        private System.Windows.Forms.Label pixelsLabel1;
        private System.Windows.Forms.Label pixelsLabel2;
        private System.Windows.Forms.NumericUpDown pixelWidthUpDown;
        private System.Windows.Forms.NumericUpDown pixelHeightUpDown;
        private System.Windows.Forms.RadioButton rbPercentage;
    }
}