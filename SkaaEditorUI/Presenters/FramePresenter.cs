#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System;
using System.Drawing;
using Capslock.WinForms.SpriteViewer;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class FramePresenter : SkaaFrame, IFrame
    {
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
