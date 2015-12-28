namespace SkaaEditorUI.Forms.DockContentControls
{
    partial class GameSetViewerContainer
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
            this.dataListView1.Location = new System.Drawing.Point(2, 1);
            this.dataListView1.Name = "dataListView1";
            this.dataListView1.Size = new System.Drawing.Size(688, 523);
            this.dataListView1.TabIndex = 0;
            this.dataListView1.UseCompatibleStateImageBehavior = false;
            this.dataListView1.View = System.Windows.Forms.View.Details;
            // 
            // GameSetViewerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(690, 524);
            this.Controls.Add(this.dataListView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "GameSetViewerContainer";
            this.Text = "ObjectListViewContainer";
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.DataListView dataListView1;
    }
}