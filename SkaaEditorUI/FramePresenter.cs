using System;
using System.Drawing;
using SpriteViewer;
using SkaaGameDataLib;

namespace SkaaEditorUI
{
    public class FramePresenter : SkaaFrame, IFrame
    {
        private IndexedBitmap _indexedBitmap;
        private Guid _guid;

        public bool PendingChanges;
        public Bitmap Bitmap
        {
            get
            {
                return base.IndexedBitmap.Bitmap;
            }
            set
            {
                base.IndexedBitmap.Bitmap = value;
            }
        }
        public Guid Guid
        {
            get
            {
                return this._guid;
            }
            set
            {
                this._guid = value;
            }
        }

        long IFrame.BitmapOffset
        {
            get
            {
                return this.BitmapOffset;
            }
        }
        string IFrame.Name
        {
            get
            {
                return this.Name;
            }
        }

        public FramePresenter(SkaaFrame sgf)
        {
            this.Guid = Guid.NewGuid();
            this.PendingChanges = false;

            this.Name = sgf.Name;
            this.BitmapOffset = sgf.BitmapOffset;
            this.IndexedBitmap = sgf.IndexedBitmap;
        }
    }
}
