namespace SkaaEditorUI.Forms.DockContentControls
{
    partial class ObjectListViewContainer
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
            this.dataListView1 = new BrightIdeasSoftware.DataListView();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataListView1
            // 
            this.dataListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListView1.CellEditUseWholeCell = false;
            this.dataListView1.DataSource = null;
            this.dataListView1.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.dataListView1.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.dataListView1.Location = new System.Drawing.Point(1, 34);
            this.dataListView1.Name = "dataListView1";
            this.dataListView1.Size = new System.Drawing.Size(614, 451);
            this.dataListView1.TabIndex = 0;
            this.dataListView1.UseCompatibleStateImageBehavior = false;
            this.dataListView1.View = System.Windows.Forms.View.Details;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(3, 9);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(197, 13);
            this.lblInstructions.TabIndex = 1;
            this.lblInstructions.Text = "Select the table you wish to save below:";
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(282, 4);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Done!";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // ObjectListViewContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 485);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.dataListView1);
            this.Name = "ObjectListViewContainer";
            this.Text = "ObjectListViewContainer";
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.DataListView dataListView1;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Button btnDone;
    }
}