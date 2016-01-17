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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capslock.Windows.Forms.SpriteViewer;
using SkaaGameDataLib.GameObjects;
using SkaaGameDataLib.Util;

namespace SkaaEditorUI.Presenters
{
    public class FramePresenter : PresenterBase<SkaaFrame>, IFrame
    {
        #region Private Members
        private Guid _guid;
        private Bitmap _bitmap;
        private Bitmap _thumbnail;
        private long _bitmapOffset;
        private string _name;
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets the <see cref="Bitmap"/> property to the specified value and raises the
        /// <see cref="PresenterBase{T}.PropertyChanged"/> event without doing a comparison.
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return this._bitmap;
            }
            set
            {
                this._bitmap = value;
                this.GameObject.IndexedBitmap.Bitmap = this._bitmap;
                BuildThumbnail();
                OnPropertyChanged();
                //We can't use SetField unless we implement a custom comparer for Bitmaps.
                //More often than not, it is likely the case that the image has indeed changed.
                //SetField(ref this._bitmap, value, () => OnPropertyChanged());// GetDesignModeValue(() => this.Bitmap)));
            }
        }
        public Bitmap Thumbnail
        {
            get
            {
                return this._thumbnail;
            }

            set
            {
                this._thumbnail = value;
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
                SetField(ref this._guid, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.Guid)));
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
                SetField(ref this._bitmapOffset, value, () => OnPropertyChanged());
                this.GameObject.BitmapOffset = this._bitmapOffset;
            }
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                SetField(ref this._name, value, () => OnPropertyChanged());
                this.GameObject.Name = this._name;
            }
        }
        #endregion

        #region Constructors
        public FramePresenter() { }
        public FramePresenter(SkaaFrame sgf)
        {
            this.Guid = Guid.NewGuid();
            this.GameObject = sgf;

            this.Bitmap = this.GameObject.IndexedBitmap.Bitmap;
            this.Name = this.GameObject.Name;
            this.BitmapOffset = this.GameObject.BitmapOffset;
        }
        #endregion

        #region Overridden Methods
        public override SkaaFrame Load(string filePath, params object[] param)
        {
            var pal = (ColorPalette)param[0];

            SkaaFrame frame = new SkaaFrame();
            frame.IndexedBitmap = new IndexedBitmap(pal);

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                frame.IndexedBitmap.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);
            }

            return frame;
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
        #endregion

        private async void BuildThumbnail()
        {
            Task<Bitmap> resize = new Task<Bitmap>(t => ResizeImage(this.Bitmap, 40, 40), 1000);
            resize.Start();
            await resize;
            this.Thumbnail = resize.Result;
        }
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
