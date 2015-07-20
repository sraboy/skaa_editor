using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
