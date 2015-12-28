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
using System.Windows.Forms;
using Capslock.Windows.Forms.SpriteViewer;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class FramePresenter : PresenterBase<SkaaFrame>, IFrame
    {
        #region Private Members
        private Guid _guid;
        private Bitmap _bitmap;
        private long _bitmapOffset;
        private string _name;
        #endregion

        public Bitmap Bitmap
        {
            get
            {
                return this._bitmap;
            }
            set
            {
                SetField(ref this._bitmap, value, () => OnPropertyChanged(GetDesignModeValue(() => this.Bitmap)));
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
                SetField(ref this._guid, value, () => OnPropertyChanged(GetDesignModeValue(() => this.Guid)));
            }
        }
        public long BitmapOffset
        {
            get
            {
                return this._bitmapOffset;
            }
            set
            {
                SetField(ref this._bitmapOffset, value, () => OnPropertyChanged(GetDesignModeValue(() => this.BitmapOffset)));
            }
        }
        public string Name
        {
            get
            {
                return this.GameObject.Name;
            }
            set
            {
                SetField(ref this._name, value, () => OnPropertyChanged(GetDesignModeValue(() => this.Name)));
            }
        }

        public FramePresenter() { }

        public FramePresenter(SkaaFrame sgf)
        {
            this.Guid = Guid.NewGuid();
            this.GameObject = this.GameObject ?? new SkaaFrame();
            this.GameObject.Name = sgf.Name;
            this.GameObject.BitmapOffset = sgf.BitmapOffset;
            this.GameObject.IndexedBitmap = sgf.IndexedBitmap;

            this.Bitmap = this.GameObject.IndexedBitmap.Bitmap;
            this.Name = this.GameObject.Name;
            this.BitmapOffset = this.GameObject.BitmapOffset;
        }

        public override SkaaFrame Load(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".spr";
            dlg.Filter = $"7KAA Sprite Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.Name ?? null;
        }
    }
}
