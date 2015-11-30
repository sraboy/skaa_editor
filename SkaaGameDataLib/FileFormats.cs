using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public enum FileFormat
    {
        Unknown,
        SpriteSpr,
        SpriteFrameSpr,
        ResXDbf, //format used by the SET file
        //ResXDbf = GameSet,
        ResXMultiBmp,
        ResXOther,
        ResXText,
        ResXUnknown,
        ResXAudio,
        //ResBmp = SpriteFrameSpr,
        ResIcon,
        DbaseIII,
        SaveGame,
        Font,
        Palette,
        FramePNG,
        SpritePNG,
        Any
    }
}
