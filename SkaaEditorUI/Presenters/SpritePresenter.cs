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
    public class SpritePresenter : SkaaSprite
    {
        public SpritePresenter() { }
        public SpritePresenter(SkaaSprite sgs)
        {
            this.Frames = sgs.Frames;
            this.SpriteId = sgs.SpriteId;
        }

        public List<IFrame> GetIFrames()
        {
            List<IFrame> frames = new List<IFrame>();// = (SkaaEditorFrame)this.Frames[0];
            foreach (SkaaFrame sf in this.Frames)
            {
                frames.Add(new FramePresenter(sf));
            }

            return frames;
        }
    }
}
