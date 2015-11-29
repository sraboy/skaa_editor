using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public enum FileFormat
    {
        Unknown = 0,
        SpriteSpr = 1,
        SpriteFrameSpr = 2,
        GameSet = 3,
        ResXDbf = GameSet,
        ResXMultiBmp = 4,
        ResXOther = 5,
        ResXText = 6,
        ResXUnknown = 7,
        ResXAudio = 8,
        ResBmp = SpriteFrameSpr,
        DbaseIII = 9,
        SaveGame = 10,
        Font = 11,
        Palette = 12,
        FramePNG = 12,
        SpritePNG = 14,
        Any
    }
}
