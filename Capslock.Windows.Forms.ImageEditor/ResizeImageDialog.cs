using System;
using System.Windows.Forms;

namespace Capslock.Windows.Forms.ImageEditor
{
    public partial class ResizeImageDialog : Form
    {
        public int OriginalWidth = 0;
        public int OriginalHeight = 0;
        public int NewWidth = 0;
        public int NewHeight = 0;
        public bool MaintainAspectRatio;

        public ResizeImageDialog(int originalWidth, int originalHeight)
        {
            this.OriginalWidth = originalWidth;
            this.OriginalHeight = originalHeight;

            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.pixelWidthUpDown.Value > 0 && this.pixelHeightUpDown.Value > 0)
            {
                this.NewWidth = (int)this.pixelWidthUpDown.Value;
                this.NewHeight = (int)this.pixelHeightUpDown.Value;
                this.MaintainAspectRatio = cbMaintainAspectRatio.Checked;
            }

            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbMaintainAspectRatio_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (this.rbAbsoluteSize.Checked)
            {
                this.pixelWidthUpDown.Enabled = true;
                this.pixelHeightUpDown.Enabled = true;
                this.cbMaintainAspectRatio.Enabled = true;
                this.percentUpDown.Enabled = false;
            }
            else if (this.rbPercentage.Checked)
            {
                this.pixelWidthUpDown.Enabled = false;
                this.pixelHeightUpDown.Enabled = false;
                this.cbMaintainAspectRatio.Enabled = false;
                this.percentUpDown.Enabled = true;
            }
        }
        private void precentUpDown_ValueChanged(object sender, EventArgs e)
        {
            int val = (int)this.percentUpDown.Value;
            double scale = val / 100.0;

            this.pixelWidthUpDown.Value *= (decimal)scale;
            this.pixelHeightUpDown.Value *= (decimal)scale;
        }
        private void OnUpDownKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                (sender as NumericUpDown).Value += 1;
            else if (e.KeyCode == Keys.Down)
                (sender as NumericUpDown).Value -= 1;
        }
        private void OnUpDownEnter(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void OnUpDownLeave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
