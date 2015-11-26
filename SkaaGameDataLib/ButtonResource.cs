using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    class ButtonResource : IndexedBitmap
    {
        public string ResourceName;
        public int BitmapOffset;

        public ButtonResource(ColorPalette pal) : base(pal) { }
    }
}
