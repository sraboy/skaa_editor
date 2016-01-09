using System;
using System.Windows.Forms;

namespace SkaaEditorUI.Forms
{
    public partial class AddNewFrame : Form
    {
        public string FrameName = string.Empty;
        public int FrameHeight = 0;
        public int FrameWidth = 0;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text != string.Empty && this.txtHeight.Text != string.Empty && this.txtHeight.Text != string.Empty)
            {
                this.FrameName = this.txtName.Text.ToUpper();
                try
                {
                    this.FrameHeight = Convert.ToInt32(this.txtHeight.Text);
                    this.FrameWidth = Convert.ToInt32(this.txtWidth.Text);
                    this.Close();
                }
                catch
                {
                    this.DialogResult = DialogResult.Abort;
                }
            }
        }

        public AddNewFrame()
        {
            InitializeComponent();
        }
    }
}
