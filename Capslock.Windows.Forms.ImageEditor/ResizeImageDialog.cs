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
        /// <summary>
        /// Set to false in <see cref="pixelHeightUpDown_ValueChanged(object, EventArgs)"/> and
        /// <see cref="pixelWidthUpDown_ValueChanged(object, EventArgs)"/> so the two methods
        /// don't enter an infinite loop that causes either a stack overflow or goes beyond the 
        /// maximum and minimum values allowed.
        /// </summary>
        private bool _processValueChangedEvent = true;

        public ResizeImageDialog(int originalWidth, int originalHeight)
        {
            InitializeComponent();

            this.OriginalWidth = originalWidth;
            this.OriginalHeight = originalHeight;
            this.pixelHeightUpDown.Value = this.OriginalHeight;
            this.pixelWidthUpDown.Value = this.OriginalWidth;
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
            this.MaintainAspectRatio = cbMaintainAspectRatio.Checked;
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
        private void percentUpDown_ValueChanged(object sender, EventArgs e)
        {
            int val = (int)this.percentUpDown.Value;
            double scale = val / 100.0;

            this.pixelWidthUpDown.Value *= (decimal)scale;
            this.pixelHeightUpDown.Value *= (decimal)scale;
        }
        private void pixelHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.cbMaintainAspectRatio.Checked && !this.rbPercentage.Checked && _processValueChangedEvent)
            {
                var diff = this.pixelHeightUpDown.Value - this.OriginalHeight;

                if (this.pixelWidthUpDown.Value + diff >= this.pixelWidthUpDown.Minimum && 
                    this.pixelWidthUpDown.Value + diff <= this.pixelWidthUpDown.Maximum)
                {
                    this.pixelWidthUpDown.Enabled = true;
                    _processValueChangedEvent = false;
                    this.pixelWidthUpDown.Value = this.OriginalWidth + diff;
                    _processValueChangedEvent = true;
                }
                else
                    this.pixelWidthUpDown.Enabled = false;
            }
        }
        private void pixelWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (this.cbMaintainAspectRatio.Checked && !this.rbPercentage.Checked && _processValueChangedEvent)
            {
                var diff = this.pixelWidthUpDown.Value - this.OriginalWidth;

                if (this.pixelHeightUpDown.Value + diff >= this.pixelHeightUpDown.Minimum &&
                    this.pixelHeightUpDown.Value + diff <= this.pixelHeightUpDown.Maximum)
                {
                    this.pixelHeightUpDown.Enabled = true;
                    _processValueChangedEvent = false;
                    this.pixelHeightUpDown.Value = this.OriginalHeight + diff;
                    _processValueChangedEvent = true;
                }
                else
                    this.pixelHeightUpDown.Enabled = false;
            }
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
            NumericUpDown nud = (NumericUpDown)sender;
            nud.Select(0, nud.Text.Length);
        }
        private void OnUpDownLeave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
