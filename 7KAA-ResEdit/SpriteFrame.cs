using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SkaaEditor
{
    public static class Helper
    {
        public static ColorPalette LoadPalette()
        {
            ColorPalette pal_std = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;// = new ColorPalette();

            FileStream fs = File.OpenRead("../../data/resource/pal_std.res");
            fs.Seek(8, SeekOrigin.Begin);

            for (int i = 0; i < 256; i++)
            {
                int r = fs.ReadByte();
                int g = fs.ReadByte();
                int b = fs.ReadByte();

                pal_std.Entries[i] = Color.FromArgb(255, r, g, b);
            }

            return pal_std;
        }
    }

    public class Sprite
    {

    }

    public class SpriteFrame
    {
        public int Height
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public Byte[] FrameData
        {
            get;
            set;
        }
        public List<Bitmap> Images
        {
            get;
            set;
        }

        public SpriteFrame(int width, int height)
        {
            this.Height = height;
            this.Width = width;

            Images = new List<Bitmap>();
            FrameData = new Byte[height * width];
        }

        public void GetPixels(FileStream stream)
        {
            int pixelsToSkip = 0;
            Byte pixel;

            for (int y = 0; y < this.Height; ++y)
            {
                for (int x = 0; x < this.Width; ++x)
                {
                    if (pixelsToSkip != 0)
                    {
                        if (pixelsToSkip >= this.Width - x)
                        {
                            pixelsToSkip -= (this.Width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;
                        pixelsToSkip = 0;
                    }

                    try { pixel = Convert.ToByte(stream.ReadByte()); }
                    catch { return; /*got -1 for EOS*/ }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        this.FrameData[this.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixelsToSkip = stream.ReadByte() - 1;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for
        }//end GetPixels()
        public void BuildBitmap()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);//, PixelFormat.Format8bppIndexed);
            Color[] pal = Helper.LoadPalette().Entries;

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {                    
                    Color pixel = pal[FrameData[y * this.Width + x]];
                    bmp.SetPixel(x, y, pixel);
                    bmp.SetPixel(x, y, Color.FromArgb(255, pixel));
                    
                }
            }
            Images.Add(bmp);
        }//end BuildBitmap()
    }
}
