using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    //todo: create custom palette class based on a color list that can be used with ColorPicker

    /// <summary>
    /// Used to encapsulate a <see cref="ColorPalette"/>, its name and the <see cref="MemoryStream"/> 
    /// containing the raw RES file data.
    /// </summary>
    public class PaletteResource
    {
        public string FileName;
        public MemoryStream ResMemoryStream;
        public ColorPalette ColorPaletteObject;
    }
}
