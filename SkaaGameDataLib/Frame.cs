using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public class Frame
    {
        public IndexedBitmap IndexedBitmap;
        public string Name;
        public long BitmapOffset;

        public byte[] ToSprFile()
        {
            return IndexedBitmap.GetRleBytesFromBitmap(this.IndexedBitmap.Bitmap);
        }
    }
}
