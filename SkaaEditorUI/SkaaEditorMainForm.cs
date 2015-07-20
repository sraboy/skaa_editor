using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SkaaEditor
{
    public partial class SkaaEditorMainForm : Form
    {
        //hack for when I break these projects' builds and lose the DLLs
        //the Designer will freak out
        //SkaaColorChooser.SkaaColorChooser skaaColorChooser1;
        //MultiplePictureBox.MultiplePictureBox multiplePictureBox1;

        public SkaaEditorMainForm()
        {
            //skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
            //multiplePictureBox1 = new MultiplePictureBox.MultiplePictureBox();

            InitializeComponent();

            if (skaaColorChooser1.Palette == null)
                btnLoadSPR.Enabled = false;
        }


        private void btnLoadSPR_Click(object sender, EventArgs e)
        {
            if (skaaColorChooser1.Palette == null)
                return;

            //ResourceFile resfile = new ResourceFile("i_menu2.res");
            //pictureBox1.Image = resfile.Resources[0].initIMGStream();

            FileStream spritestream = File.OpenRead("../../data/sprite/ballista.spr");
            //ballista.spr length = 82578 (data_buf_size in init_import()

            /************************
             * Get all of the frames
             ************************/
            //Byte[] sprite_data = new Byte[spritestream.Length];
            //spritestream.Read(sprite_data, 0, (int) spritestream.Length);


            List<SpriteFrame> spriteFrames = new List<SpriteFrame>();

            while (spritestream.Position < spritestream.Length)
            {
                ////jump to offset 17537, w=62, h=65 (cur_dir=1) + 4 byte (junk?)
                //spritestream.Seek(17537+4, SeekOrigin.Begin);
                Byte[] frame_size_bytes = new Byte[4];
                spritestream.Seek(4, SeekOrigin.Current); //skip the 32-bit size value
                spritestream.Read(frame_size_bytes, 0, 4);

                short width = BitConverter.ToInt16(frame_size_bytes, 0);// >> 16;
                short height = BitConverter.ToInt16(frame_size_bytes, 2);// >> 16;
                
                //Just a cheap hack to get around VS issues with the Designer
                //SkaaColorChooser.SkaaColorChooser skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
                
                SpriteFrame frame = new SpriteFrame(width, height, skaaColorChooser1.Palette);

                frame.GetPixels(spritestream);

                //debugging: gives an ASCII representation of image 
                //(add \n after every 62d character). Verified alignment of pixels as read.
                //var hex = BitConverter.ToString(frame.FrameData);

                frame.BuildBitmap();
                
                spriteFrames.Add(frame);

                // TODO: Just a hack since we skip pixels that are preset to 0x00.
                // Will need to write those pixels as the actual Color.Transparent
                // so we can have black in our images.
                frame.Image.MakeTransparent(System.Drawing.Color.Black);

                //pictureBox1.Image = frame.Image;
                
                //end early, just get one to test
                //spritestream.Position = spritestream.Length;
            }

            //int zoomWidth = spriteFrames[0].Image.Width * 1;
            //int zoomHeight = spriteFrames[0].Image.Height * 1;
            //System.Drawing.Bitmap bmp =
            //    new System.Drawing.Bitmap(spriteFrames[0].Image, new System.Drawing.Size(zoomWidth, zoomHeight));

            foreach(SpriteFrame sf in spriteFrames)
                multiplePictureBox1.AddImage(sf.Image);
            
            spritestream.Close();
        }

        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".res";
            dlg.SupportMultiDottedExtensions = true;

            if (dlg.ShowDialog() == DialogResult.OK)
                skaaColorChooser1.LoadPalette(dlg.FileName);

            btnLoadSPR.Enabled = true;
        }
    }
}
