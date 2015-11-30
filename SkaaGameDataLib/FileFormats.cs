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
        GameSet, //essentially a ResIdxDbf
        ResIdxMultiBmp,
        ResIdxOther,
        ResIdxText,
        ResIdxUnknown,
        ResIdxAudio,
        ResUnknown,
        DbaseIII,
        SaveGame,
        Font,
        Palette,
        FramePNG,
        SpritePNG,
        Any
    }
}
