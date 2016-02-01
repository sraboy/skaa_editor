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
            this.cbTables = new System.Windows.Forms.ComboBox();
            this.dataListView1 = new BrightIdeasSoftware.FastDataListView();
            this.cbDataSources = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbTables
            // 
            this.cbTables.FormattingEnabled = true;
            this.cbTables.Location = new System.Drawing.Point(286, 1);
            this.cbTables.Margin = new System.Windows.Forms.Padding(0);
            this.cbTables.Name = "cbTables";
            this.cbTables.Size = new System.Drawing.Size(224, 21);
            this.cbTables.TabIndex = 1;
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
            this.dataListView1.Location = new System.Drawing.Point(2, 25);
            this.dataListView1.Name = "dataListView1";
            this.dataListView1.ShowGroups = false;
            this.dataListView1.Size = new System.Drawing.Size(688, 499);
            this.dataListView1.TabIndex = 0;
            this.dataListView1.UseCompatibleStateImageBehavior = false;
            this.dataListView1.View = System.Windows.Forms.View.Details;
            this.dataListView1.VirtualMode = true;
            // 
            // cbDataSources
            // 
            this.cbDataSources.FormattingEnabled = true;
            this.cbDataSources.Location = new System.Drawing.Point(72, 1);
            this.cbDataSources.Margin = new System.Windows.Forms.Padding(0);
            this.cbDataSources.Name = "cbDataSources";
            this.cbDataSources.Size = new System.Drawing.Size(142, 21);
            this.cbDataSources.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Data Source:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data Table:";
            // 
            // GameSetViewerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(690, 524);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDataSources);
            this.Controls.Add(this.cbTables);
            this.Controls.Add(this.dataListView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "GameSetViewerContainer";
            this.Text = "ObjectListViewContainer";
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbTables;
        private BrightIdeasSoftware.FastDataListView dataListView1;
        private System.Windows.Forms.ComboBox cbDataSources;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}