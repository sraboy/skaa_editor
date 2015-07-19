using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SkaaEditor
{
    public partial class SkaaMainForm : Form
    {
        public SkaaMainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ResourceFile resfile = new ResourceFile("i_menu2.res");
            //pictureBox1.Image = resfile.Resources[0].initIMGStream();

            FileStream spritestream = File.OpenRead("sprite/ballista.spr");
            //ballista.spr length = 82578 (data_buf_size in init_import()

            /************************
             * Get sprite header data 
             ************************/
            Byte[] sprite_data_size_bytes = new Byte[4];
            spritestream.Read(sprite_data_size_bytes, 0, 4);
            int sprite_data_size = BitConverter.ToInt32(sprite_data_size_bytes,0);

            /************************
             * Get all of the frames
             ************************/
            //Byte[] sprite_data = new Byte[spritestream.Length];
            //spritestream.Read(sprite_data, 0, (int) spritestream.Length);


            List<SpriteFrame> sprFrames = new List<SpriteFrame>();
            
            //while(spritestream.Position < spritestream.Length)
            //{
                //jump to offset 17537, w=62, h=65 (cur_dir=1). + 4 byte (junk?) header
                spritestream.Seek(17537+4, SeekOrigin.Begin);
                Byte[] frame_size_bytes = new Byte[4];

                spritestream.Read(frame_size_bytes, 0, 4);
                short width = BitConverter.ToInt16(frame_size_bytes, 0);// >> 16;
                short height = BitConverter.ToInt16(frame_size_bytes, 2);// >> 16;

                SpriteFrame frame = new SpriteFrame(width, height);

                GetPixels(frame, spritestream);
                frame.BuildBitmap();
                sprFrames.Add(frame);
                pictureBox1.Image = sprFrames[0].Images[0];
                spritestream.Position = spritestream.Length;
            //}


            int zoomWidth = sprFrames[0].Images[0].Width * 2;
            int zoomHeight = sprFrames[0].Images[0].Height * 2;
            System.Drawing.Bitmap bmp = 
                new System.Drawing.Bitmap(sprFrames[0].Images[0], new System.Drawing.Size(zoomWidth, zoomHeight));

            pictureBox1.Image = bmp; // sprFrames[0].Images[0];
            spritestream.Close();
        }

        void GetPixels(SpriteFrame frame, FileStream stream)
        {
            int pixelsToSkip = 0;
            Byte pixel;

            for (int y = 0; y < frame.Height; ++y)//, destline += pitch)
            {
                for (int x = 0; x < frame.Width; ++x)
                {
                    if (pixelsToSkip != 0)
                    {
                        if (pixelsToSkip >= frame.Width - x)
                        {
                            pixelsToSkip -= (frame.Width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;
                        pixelsToSkip = 0;
                    }

                    try { pixel = Convert.ToByte(stream.ReadByte()); }
                    catch { return; /*got -1 for EOS*/ }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        frame.FrameData[frame.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixelsToSkip = stream.ReadByte() -1;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for
        }//end GetPixels()
    }
}
