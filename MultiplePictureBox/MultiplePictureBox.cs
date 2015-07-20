using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiplePictureBox
{
    public partial class MultiplePictureBox : UserControl
    {
        private int imageOnDisplay = 0;

        private List<Bitmap> Frames;

        public MultiplePictureBox()
        {
            InitializeComponent();
            Frames = new List<Bitmap>();
        }

        public void AddImage(Bitmap bmp)
        {
            Frames.Add(bmp);

            if (Frames.Count == 1) //first image
                pictureBox1.Image = this.Frames[0];
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Frames[ ++imageOnDisplay % Frames.Count ];
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Frames[ --imageOnDisplay % Frames.Count ];
        }
    }
}
