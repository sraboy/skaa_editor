using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;
using SpriteViewer;

namespace SkaaEditorUI
{
    public class SkaaEditorSprite : SkaaGameSprite
    {
        public SkaaEditorSprite() { }
        public SkaaEditorSprite(SkaaGameSprite sgs)
        {
            this.Frames = sgs.Frames;
            this.SpriteId = sgs.SpriteId;
        }

        public List<IFrame> GetIFrames()
        {
            List<IFrame> frames = new List<IFrame>();// = (SkaaEditorFrame)this.Frames[0];
            foreach (SkaaGameFrame sf in this.Frames)
            {
                frames.Add(new SkaaEditorFrame(sf));
            }

            return frames;
        }
    }
}
