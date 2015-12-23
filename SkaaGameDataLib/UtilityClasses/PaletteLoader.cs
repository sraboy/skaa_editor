#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SkaaGameDataLib
{
    public static class PaletteLoader
    {
        private static readonly TraceSource Logger = new TraceSource($"{typeof(PaletteLoader)}", SourceLevels.All);

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

            Logger.TraceInformation($"Loaded palette: {filepath}");
            return pal;
        }
    }
}
