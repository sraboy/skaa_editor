using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditorUI
{
    /// <summary>
    /// Used to encapsulate a <see cref="Sprite"/>, its name and the <see cref="MemoryStream"/> 
    /// containing the raw SPR file data.
    /// </summary>
    public class SpriteResource
    {
        public string FileName;
        public MemoryStream SprMemoryStream;
        public Sprite SpriteObject;
    }
}
