using System;
using System.Drawing.Imaging;
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
using System.IO;
using Capslock.WinForms.SpriteViewer;
using SkaaEditorUI.Misc;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public abstract class MultiImagePresenterBase : PresenterBase<SkaaSprite>, IMultiImagePresenter<SkaaSprite>
    {
        private ColorPalettePresenter _palettePresenter = new ColorPalettePresenter();
        private IFrame _activeFrame;
        private TrulyObservableCollection<FramePresenter> _frames = new TrulyObservableCollection<FramePresenter>();

        public ColorPalettePresenter PalettePresenter
        {
            get
            {
                return _palettePresenter;
            }

            set
            {
                SetField(ref this._palettePresenter, value, () => OnPropertyChanged(GetDesignModeValue(() => this.PalettePresenter)));
            }
        }
        public IFrame ActiveFrame
        {
            get
            {
                return _activeFrame;
            }
            set
            {
                SetField(ref this._activeFrame, value, () => OnPropertyChanged(GetDesignModeValue(() => this.ActiveFrame)));
            }
        }
        public TrulyObservableCollection<FramePresenter> Frames
        {
            get
            {
                return this._frames;
            }
            set
            {
                SetField(ref this._frames, value, () => OnPropertyChanged(GetDesignModeValue(() => this.Frames)));
            }
        }

        protected void SetFrames()
        {
            foreach (SkaaFrame f in this.GameObject.Frames)
            {
                var fp = new FramePresenter(f);
                this.Frames.Add(fp);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }

        public Stream GetSpriteStream()
        {
            var str = new MemoryStream();
            var sprBytes = this.GameObject.GetSpriteFrameByteArrays();
            foreach (byte[] ba in sprBytes)
                str.Write(ba, 0, ba.Length);

            str.Position = 0;

            return str;
        }

        //public IMultiImagePresenter<SkaaSprite> Open()
        //{
        //    return (IMultiImagePresenter<SkaaSprite>)((IPresenterBase<SkaaSprite>)this).Open<SkaaSprite>(this.FileFormat, this.PalettePresenter);
        //}

        public void LoadPalette(string filePath)
        {
            this.PalettePresenter.Load(filePath, null);
        }

        public void SetActiveFrame(int index)
        {
            this.ActiveFrame = this.Frames?[index];
        }
    }
}
