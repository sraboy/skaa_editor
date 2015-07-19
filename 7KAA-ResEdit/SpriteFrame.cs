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

            FileStream fs = File.OpenRead("resource/pal_std.res");
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

            Images = new List<Bitmap>();// = new System.Windows.Media.Imaging.BitmapImage();
            FrameData = new Byte[height * width];

            //make all the bytes in our bitmap 0xff, the transparent color
            //Array.ForEach<Byte>(FrameData, new Action<Byte>(delegate(Byte b) { b = 0xff; }));

            //Array.Clear(FrameData, 0, FrameData.Length);
        }

        public void BuildBitmap()
        {
            var b = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
            b.Palette = Helper.LoadPalette();
            //b.MakeTransparent(Color.Transparent);

            var BoundsRect = new Rectangle(0, 0, this.Width, this.Height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);
            bmpData.Stride = 1;
            IntPtr ptr = bmpData.Scan0;

            Marshal.Copy(FrameData, 0, ptr, FrameData.Length);
            b.UnlockBits(bmpData);
            Images.Add(b);

            //MemoryStream ms = new System.IO.MemoryStream(FrameData);
            //Bitmap img = new Bitmap(ms);

            //Images.Add(img);
        }

    }
}
