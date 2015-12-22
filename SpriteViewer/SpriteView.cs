#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/
#endregion

using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SpriteViewer
{
    public partial class SpriteView : UserControl
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
        protected virtual void RaiseActiveFrameChangedEvent(FrameChangedEventArgs e)
        {
            EventHandler handler = _activeFrameChanged;
              
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private Guid _activeFrameGuid;

        public SpriteView()
        {
            InitializeComponent();
            this.timeline1.ActiveFrameChanged += Timeline1_ActiveFrameChanged;
            this.listViewer1.ActiveFrameChanged += ListViewer1_ActiveFrameChanged;
            this.listViewer1.SetImageGetter(SpriteFrameImageGetter);
        }

        #region Public Methods
        public void SetImageGetter(ImageGetterDelegate imageGetter) => this.listViewer1.SetImageGetter(imageGetter);
        public void SetFrameList(List<IFrame> frames)
        {
            this.listViewer1.SetObjects(frames);
            this.timeline1.SetFrameList(frames);
            this._activeFrameGuid = frames[0].Guid;
        }
        public void UpdateFrame(IFrame f)
        {
            //this.timeline1.UpdateCurrentFrame()
        }
        #endregion

        #region Private Methods
        private object SpriteFrameImageGetter(object rowObject)
        {
            IFrame f = (IFrame) rowObject;
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
            RaiseActiveFrameChangedEvent(fe);
        }
        private void Timeline1_ActiveFrameChanged(object sender, EventArgs e)
        {
            var fe = (e as FrameChangedEventArgs);
            RaiseActiveFrameChangedEvent(fe);
        }
        //private void OnActiveFrameChanged(object sender, EventArgs e)
        //{
        //    var fe = (e as FrameChangedEventArgs);
        //    if (ChangeActiveFrame(fe.FrameGuid))
        //        RaiseActiveFrameChangedEvent(fe);
        //}
        #endregion
    }
}
