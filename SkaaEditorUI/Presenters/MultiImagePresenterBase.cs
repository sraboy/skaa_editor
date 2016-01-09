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
using System.Drawing;
using System.IO;
using System.Linq;
using Capslock.Windows.Forms.SpriteViewer;
using SkaaGameDataLib.GameObjects;
using SkaaGameDataLib.Util;
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
        private bool _bitmapHasChanges = false;
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
                SetField(ref this._spriteId, value, () => OnPropertyChanged());
                this.GameObject.SpriteId = this._spriteId;
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
                SetField(ref this._dataView, value, () => OnPropertyChanged());
            }
        }
        public bool BitmapHasChanges
        {
            get
            {
                return _bitmapHasChanges;
            }

            set
            {
                this._bitmapHasChanges = value;
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
                fp.PropertyChanged += FramePresenter_PropertyChanged;
            }

            this.ActiveFrame = this.Frames[0];
        }

        private void FramePresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Bitmap")
                this._bitmapHasChanges = true;
        }

        protected abstract MemoryStream GetSpriteStream();
        protected abstract void AddNewFrameDataRow(FramePresenter fr);
        public abstract void RecalculateFrameOffsets();

        public abstract void SetSpriteDataView(GameSetPresenter gsp);

        /// <summary>
        /// Builds a <see cref="Bitmap"/> sprite sheet containing all of this object's frames. There is 
        /// no padding between frames. The number of rows/columns of frames is the square root of the number of frames
        /// with an additional row added when the number of frames is not a perfect square.
        /// </summary>
        /// <returns>The newly-generated <see cref="Bitmap"/></returns>
        public Bitmap GetSpriteSheet()
        {
            int totalFrames = this.Frames.Count;
            int spriteWidth = 0, spriteHeight = 0, columns = 0, rows = 0;

            double sqrt = Math.Sqrt(totalFrames);

            //figure out how many rows we need
            if (totalFrames % 1 != 0) //totalFrames is a perfect square
            {
                rows = (int)sqrt;
                columns = (int)sqrt;
            }
            else
            {
                rows = (int)Math.Floor(sqrt) + 1; //adds an additional row
                columns = (int)Math.Ceiling(sqrt);
            }

            //need the largest tile (by height and width) to set the row/column heights
            foreach (IFrame f in this.Frames)
            {
                if (f.Bitmap.Width > spriteWidth)
                    spriteWidth = f.Bitmap.Width;
                if (f.Bitmap.Height > spriteHeight)
                    spriteHeight = f.Bitmap.Height;
            }

            //the total height/width of the image to be created
            int exportWidth = columns * spriteWidth,
                exportHeight = rows * spriteHeight;

            Bitmap bitmap = new Bitmap(exportWidth, exportHeight);

            try
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    int frameIndex = 0;

                    for (int y = 0; y < exportHeight; y += spriteHeight)
                    {
                        //once we hit the max frames, just break
                        for (int x = 0; x < exportWidth && frameIndex < this.Frames.Count; x += spriteWidth)
                        {
                            g.DrawImage(this.Frames[frameIndex].Bitmap, new Point(x, y));
                            frameIndex++;
                        }
                    }
                }
            }
            catch
            {
                bitmap = null;
            }

            if (bitmap == null)
                Logger.TraceInformation($"Failed to create sprite sheet bitmap for {this.SpriteId}");

            return bitmap;
        }
        public FramePresenter AddNewFrame(string name, int height, int width)
        {
            //We want to place the new frame last, so we need to use the currently-last 
            //frame's properties to set the new one's BitmapOffset
            var lastFrame = this.Frames[this.Frames.Count - 1];

            //Build the new frame.
            SkaaFrame sf = new SkaaFrame();
            sf.IndexedBitmap = new IndexedBitmap(new Bitmap(width, height) { Palette = this.PalettePresenter.GameObject });
            sf.Name = name;

            //Note: We just use the bitmap's height & width, which essentially assumes there are no transparent pixels.
            //This is just a quick cheat, rather than calling GetSprBytes() to get the real offset based on RLE data.
            sf.BitmapOffset = lastFrame.BitmapOffset + (lastFrame.Bitmap.Height * lastFrame.Bitmap.Width);

            //Build the new FramePresenter. This takes care of setting up fp's properties for us.
            FramePresenter fp = new FramePresenter(sf);

            AddNewFrameDataRow(fp);


            //Update this and SkaaSprite's Frames
            this.Frames.Add(fp);
            this.GameObject.Frames.Add(sf);

            //Ensures RecalculateFrameOffsets will get the real offset for us later
            this.BitmapHasChanges = true;

            return fp;
        }

    }
}
