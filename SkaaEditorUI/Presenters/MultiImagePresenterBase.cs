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
using System.Data;
using System.IO;
using Capslock.Windows.Forms.SpriteViewer;
using SkaaGameDataLib;
using TrulyObservableCollection;

namespace SkaaEditorUI.Presenters
{
    public abstract class MultiImagePresenterBase : PresenterBase<SkaaSprite>, IMultiImagePresenter
    {
        private TrulyObservableCollection<IFrame> _frames = new TrulyObservableCollection<IFrame>();
        private ColorPalettePresenter _palettePresenter;
        private IFrame _activeFrame;
        private string _spriteId;
        private DataView _dataView;

        public TrulyObservableCollection<IFrame> Frames
        {
            get
            {
                return this._frames;
            }
            set
            {
                SetField(ref this._frames, value, () => OnPropertyChanged());// GetDesignModeValue(() => this.Frames)));
            }
        }
        public ColorPalettePresenter PalettePresenter
        {
            get
            {
                return _palettePresenter;
            }

            set
            {
                SetField(ref this._palettePresenter, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.PalettePresenter)));
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
                SetField(ref this._activeFrame, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.ActiveFrame)));
            }
        }
        public string SpriteId
        {
            get
            {
                return this._spriteId;
            }

            set
            {
                SetField(ref this._spriteId, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.SpriteId)));
            }
        }
        public DataView DataView
        {
            get
            {
                return this._dataView;
            }
            internal set
            {
                SetField(ref this._dataView, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.DataView)));
            }
        }

        protected TrulyObservableCollection<IFrame> BuildFramePresenters()
        {
            TrulyObservableCollection<IFrame> frames = new TrulyObservableCollection<IFrame>();

            foreach (SkaaFrame f in this.GameObject.Frames)
            {
                var fp = new FramePresenter(f);
                frames.Add(fp);
            }

            return frames;
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

        public void LoadPalette(string filePath)
        {
            this.PalettePresenter = new ColorPalettePresenter();
            this.PalettePresenter.Load(filePath, null);
        }

        public void SetActiveFrame(int index)
        {
            this.ActiveFrame = this.Frames?[index];
        }

        public void SetSpriteDataView(GameSetPresenter gsp)
        {
            DataView dv;

            dv = new DataView(gsp.GameObject?.Tables?["SFRAME"]);
            dv.RowFilter = $"SPRITE = '{this.SpriteId.ToUpper()}'";

            this.DataView = dv;
            this.GameObject.SetSpriteDataView(dv);

            this.Frames = BuildFramePresenters();
        }
    }
}
