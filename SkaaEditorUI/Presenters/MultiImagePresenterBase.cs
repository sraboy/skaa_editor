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
using System.Data;
using System.IO;
using System.Linq;
using Capslock.Windows.Forms.SpriteViewer;
using SkaaGameDataLib;
using TrulyObservableCollection;

namespace SkaaEditorUI.Presenters
{
    public abstract class MultiImagePresenterBase : PresenterBase<SkaaSprite>, IMultiImagePresenter
    {
        #region Private Fields
        private TrulyObservableCollection<IFrame> _frames = new TrulyObservableCollection<IFrame>();
        private ColorPalettePresenter _palettePresenter;
        private IFrame _activeFrame;
        private string _spriteId;
        private DataView _dataView;
        #endregion

        #region ActiveFrameChanged Event
        private EventHandler _activeFrameChanged;
        public event EventHandler ActiveFrameChanged
        {
            add
            {
                if (_activeFrameChanged == null || !_activeFrameChanged.GetInvocationList().Contains(value))
                {
                    _activeFrameChanged += value;
                }
            }
            remove
            {
                _activeFrameChanged -= value;
            }
        }
        protected virtual void OnActiveFrameChanged(EventArgs e)
        {
            EventHandler handler = _activeFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Public Properties
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
                //SetField(ref this._activeFrame, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.ActiveFrame)));
                if (this._activeFrame != value)
                {
                    this._activeFrame = value;
                    OnActiveFrameChanged(EventArgs.Empty);
                }
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
        #endregion

        /// <summary>
        /// Creates a <see cref="FramePresenter"/> for every <see cref="SkaaFrame"/> in the <see cref="SkaaSprite"/>
        /// referenced by <see cref="PresenterBase{T}.GameObject"/>. Calling this more than once on the same object
        /// will result in new <see cref="FramePresenter"/> objects being generated, which come with new <see cref="Guid"/>
        /// values.
        /// </summary>
        protected void BuildFramePresenters()
        {
            this.Frames = new TrulyObservableCollection<IFrame>();

            foreach (SkaaFrame f in this.GameObject.Frames)
            {
                var fp = new FramePresenter(f);
                this.Frames.Add(fp);
            }

            this.ActiveFrame = this.Frames[0];
        }
        /// <summary>
        /// Creates and returns a <see cref="MemoryStream"/> containing <see cref="SkaaFrame"/> data for 
        /// all <see cref="IFrame"/> objects in <see cref="Frames"/>. The <see cref="MemoryStream.Position"/>
        /// is reset to 0 before returning.
        /// </summary>
        protected MemoryStream GetSpriteStream()
        {
            var str = new MemoryStream();
            var sprBytes = this.GameObject.GetSpriteFrameByteArrays();

            foreach (byte[] ba in sprBytes)
                str.Write(ba, 0, ba.Length);

            str.Position = 0;

            return str;
        }

        public abstract void SetSpriteDataView(GameSetPresenter gsp);
    }
}
