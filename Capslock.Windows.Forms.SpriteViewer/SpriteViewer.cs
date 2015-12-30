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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using TrulyObservableCollection;

namespace Capslock.Windows.Forms.SpriteViewer
{
    public partial class SpriteViewer : UserControl
    {
        #region ActiveFrameChanged Event
        [field: NonSerialized]
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
        protected virtual void OnActiveFrameChanged(FrameChangedEventArgs e)
        {
            EventHandler handler = _activeFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private Guid _activeFrameGuid;

        public SpriteViewer()
        {
            InitializeComponent();
            this.timelineView1.ActiveFrameChanged += Timeline1_ActiveFrameChanged;
            this.listView1.ActiveFrameChanged += ListViewer1_ActiveFrameChanged;
            this.listView1.SetImageGetter(SpriteFrameImageGetter);
        }

        #region Public Methods
        public void SetImageGetter(ImageGetterDelegate imageGetter) => this.listView1.SetImageGetter(imageGetter);
        //public void SetFrameList(List<IFrame> frames)
        //{
        //    this.listViewer1.SetObjects(frames);
        //    this.timeline1.SetFrameList(frames);
        //    this._activeFrameGuid = frames[0].Guid;
        //}
        public void SetFrameList(TrulyObservableCollection<IFrame> frames)
        {
            this.listView1.SetObjects(frames);
            this.timelineView1.SetFrameList(frames);
            this._activeFrameGuid = frames[0].Guid;
        }


        //public void UpdateFrame(IFrame f)
        //{
        //    this.timelineView1.UpdateFrame(f);
        //}
        #endregion

        #region Private Methods
        private object SpriteFrameImageGetter(object rowObject)
        {
            IFrame f = (IFrame)rowObject;
            return ResizeImage(f.Bitmap, 40, 40);
        }
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        /// <remarks>Courtesy: http://stackoverflow.com/questions/1922040/resize-an-image-c-sharp </remarks>
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
        #endregion

        #region Event Handlers
        private void ListViewer1_ActiveFrameChanged(object sender, EventArgs e)
        {
            var fe = (e as FrameChangedEventArgs);
            this.timelineView1.SetCurrentFrameTo(fe.FrameGuid);
            OnActiveFrameChanged(fe);
        }
        private void Timeline1_ActiveFrameChanged(object sender, EventArgs e)
        {
            var fe = (e as FrameChangedEventArgs);
            this.listView1.SetSelectedItem(fe.FrameGuid);
            OnActiveFrameChanged(fe);
        }
        #endregion
    }
}
