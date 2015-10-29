using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditor
{
    public struct SuperPalette
    {
        public string PaletteFileName;
        public MemoryStream PaletteFileMemoryStream;
        public ColorPalette ActivePalette;

    }
    public struct SuperGameSet
    {
        public string GameSetFileName;
        public MemoryStream GameSetFileMemoryStream;
        public GameSet ActiveGameSet;
    }
    public struct SuperSprite
    {
        public string SpriteFileName;
        public MemoryStream SpriteFileMemoryStream;
        public Sprite ActiveSprite;
    }
}
