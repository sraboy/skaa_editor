using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    //todo: create custom palette class based on a color list that can be used with ColorPicker

    public static class PaletteLoader
    {
        public const int MaxColors = 256;

        public static ColorPalette FromResFile(string filepath)
        {
            ColorPalette pal;

            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                pal = bmp.Palette; //returns a copy of ColorPalette (value type)

            using (FileStream fs = File.OpenRead(filepath))
            {
                fs.Seek(8, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    int r = fs.ReadByte();
                    int g = fs.ReadByte();
                    int b = fs.ReadByte();

                    if (i < 0xf9) //0xf9 is the lowest transparent color byte
                        pal.Entries[i] = Color.FromArgb(255, r, g, b);
                    else          //0xf9 - 0xff
                        pal.Entries[i] = Color.FromArgb(0, r, g, b);
                }
            }

            return pal;
        }
    }
}
