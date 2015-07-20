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
        bool zoom = false;
        int imageOnDisplay = 0;

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
                picBoxFrame.Image = this.Frames[0];
        }

        private void picBoxFrame_Click(object sender, MouseEventArgs e) 
        {
            if (this.Frames.Count == 0)
                return;

            if (e.Button == MouseButtons.Left)
            {
                //hack to reset zoom
                zoom = false;
                picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(Frames[imageOnDisplay].Width, Frames[imageOnDisplay].Height));

                imageOnDisplay++;
                imageOnDisplay %= (Frames.Count - 1);
                picBoxFrame.Image = Frames[imageOnDisplay];
            }
            else if (e.Button == MouseButtons.Right)
            {
                //hack to reset zoom
                zoom = false;
                picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(Frames[imageOnDisplay].Width, Frames[imageOnDisplay].Height));

                imageOnDisplay--;
                imageOnDisplay = (imageOnDisplay % (Frames.Count - 1) + (Frames.Count - 1)) % (Frames.Count - 1);
                // special mod() function above to actually cycle negative numbers around. Turns out % isn't 
                // a real mod() function, just remainder.
                picBoxFrame.Image = Frames[imageOnDisplay];
            }
            else if (e.Button == MouseButtons.Middle)
            {
                int zoomWidth, zoomHeight;

                // TODO: zoom is still a bit wonky. zoom = false above is a temp hack
                switch (zoom)
                {
                    case true:
                        zoomWidth = Frames[imageOnDisplay].Width;
                        zoomHeight = Frames[imageOnDisplay].Height;
                        picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(zoomWidth, zoomHeight));
                        zoom = false;
                        break;
                    case false:
                        zoomWidth = picBoxFrame.Image.Width * 2;
                        zoomHeight = picBoxFrame.Image.Height * 2;
                        picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(zoomWidth, zoomHeight));
                        zoom = true;
                        break;
                }

            }
        }

        
    }
}
