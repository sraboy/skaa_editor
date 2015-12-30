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
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using TrulyObservableCollection;

namespace Capslock.Windows.Forms.SpriteViewer
{
    public partial class TimelineView : UserControl
    {
        public static readonly TraceSource Logger = new TraceSource("Timeline", SourceLevels.All);

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
        private TrulyObservableCollection<IFrame> _frames;
        private List<Image> _frameImages;
        private int _currentFrameIndex;
        private int _animationStartPoint;
        private bool _isInitialized
        {
            get
            {
                return this._frameImages != null;
            }
        }

        public TimelineView()
        {
            InitializeComponent();

            this._picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            this._animationTimer.Enabled = false;
            this._animationTimer.Tick += AnimationTimer_Tick;
            this._animationTimer.Interval = 150;

            /* sraboy-21Dec15
            *  I wanted this to be generic enough to work with just images but need the IFrame
            *  functionality, primarily to get the Guid of the IFrame to raise the ActiveFrameChangedEvent.
            *
            *  This is because the caller may not know the index of the image they want to display. That
            *  means the caller would request a specific image by passing in that exact image. This may
            *  not be practical if images have been edited; it would require the caller to maintain a copy
            *  of the old, un-edited image in order to make a call to display that frame and/or update that 
            *  frame. I don't want image updates/changes to be restricted to the currently active frame as
            *  that would limit asynchronous functionality.
            *
            *  At least for now, I'm maintaining the List<Image> and will allow both to be used, so the control
            *  can be used for simpler image displays in other areas.
            */
            //this._frameImages = new List<Image>();
            this._frames = new TrulyObservableCollection<IFrame>();
        }

        #region Internal Methods
        internal void SetFrameList(List<Image> frames)
        {
            this._frameImages = frames;
            this._animationTimer.Enabled = false;
            SetupUI();
        }
        internal void SetFrameList(TrulyObservableCollection<IFrame> frames)
        {
            this._frames = frames;

            this._frameImages = this._frameImages ?? new List<Image>();

            foreach (IFrame f in frames)
            {
                this._frameImages.Add(f.Bitmap);
                f.PropertyChanged += Frame_PropertyChanged;
            }

            this._animationTimer.Enabled = false;
            SetupUI();
        }

        private void Frame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Refresh();
            //Invalidate(true);
        }

        internal Image GetActiveFrameImage()
        {
            return this._frameImages[this._currentFrameIndex];
        }
        internal IFrame GetActiveFrame()
        {
            return this._frames[this._currentFrameIndex];
        }
        internal int GetActiveFrameIndex()
        {
            return this._currentFrameIndex;
        }
        internal void UpdateCurrentFrame(IFrame frame)
        {
            this._currentFrameIndex = this._frames.IndexOf(this._frames.First(f => f.Guid == frame.Guid));
            this.UpdateCurrentFrame();
        }
        internal void UpdateFrame(IFrame frame)
        {
            var f = this._frames.First(img => img.Guid == frame.Guid);
            f = frame;
        }
        /// <summary>
        /// Updates the control to display the specified image, if it exists in the <see cref="List"/> 
        /// passed to <see cref="SetFrameList(List{Image})"/>
        /// </summary>
        /// <param name="frame">The image to find and display</param>
        /// <returns>true if the image was found and the control was updated, false otherwise</returns>
        internal bool SetCurrentFrameTo(Image frame)
        {
            this._currentFrameIndex = this._frameImages.FindIndex(img => img == frame);
            return SetCurrentFrame();
        }
        /// <summary>
        /// Updates the control to display the specified image, if it exists in the <see cref="List"/> 
        /// passed to <see cref="SetFrameList(List{IFrame})"/>
        /// </summary>
        /// <param name="frame">The <see cref="IFrame"/> to find and display</param>
        /// <returns>true if the <see cref="IFrame"/> was found and the control was updated, false otherwise</returns>
        internal bool SetCurrentFrameTo(IFrame frame)
        {
            this._currentFrameIndex = this._frames.IndexOf(this._frames.First(f => f.Guid == frame.Guid));
            return SetCurrentFrame();
        }
        internal bool SetCurrentFrameTo(Guid frameGuid)
        {
            this._currentFrameIndex = this._frames.IndexOf(this._frames.First(f => f.Guid == frameGuid));

            return SetCurrentFrame();
        }
        #endregion

        #region Private Methods
        private void NextFrame()
        {
            if (this._isInitialized)
            {
                this._currentFrameIndex = (this._currentFrameIndex + 1) % (_frameImages.Count - 1);
                this.UpdateCurrentFrame();
            }
        }
        private void PrevFrame()
        {
            if (this._isInitialized)
            {
                this._currentFrameIndex--;
                if (this._currentFrameIndex < 0)
                {
                    this._currentFrameIndex = _frameImages.Count;
                }
                this._currentFrameIndex %= _frameImages.Count - 1;
                this.UpdateCurrentFrame();
            }
        }
        private void UpdateCurrentFrame()
        {
            //if()
            this._picBoxFrame.Image = this._frameImages?[this._currentFrameIndex] ?? null;
            this._sliderBar.Value = this._currentFrameIndex;

            if (!this._animationTimer.Enabled)
                RaiseActiveFrameChangedEvent(new FrameChangedEventArgs(this._frames[this._currentFrameIndex].Guid));
        }
        private void SetupUI()
        {
            this._sliderBar.Maximum = _isInitialized ? this._frameImages.Count - 1 : 0;
            this._sliderBar.Minimum = 0;
            this._currentFrameIndex = 0;
            this.UpdateCurrentFrame();
        }
        private bool SetCurrentFrame()
        {
            if (this._currentFrameIndex != -1)
            {
                this.UpdateCurrentFrame();
                return true;
            }
            else
            {
                this._currentFrameIndex = 0;
                this.UpdateCurrentFrame();
                return false;
            }
        }
        #endregion

        #region Event Handlers
        private void picBoxFrame_Click(object sender, MouseEventArgs e)
        {
            if (!_isInitialized) return;

            if (!this._animationTimer.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        this.NextFrame();
                        break;
                    case MouseButtons.Right:
                        this.PrevFrame();
                        break;
                    case MouseButtons.Middle:
                        PictureBoxSizeMode SM = this._picBoxFrame.SizeMode;
                        if (SM == PictureBoxSizeMode.CenterImage)
                        {
                            SM = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            SM = PictureBoxSizeMode.CenterImage;
                        }
                        this._picBoxFrame.SizeMode = SM;
                        break;
                }
            }
        }
        private void frameSlider_ValueChanged(object sender, EventArgs e)
        {
            this._currentFrameIndex = _sliderBar.Value;
            this.UpdateCurrentFrame();
        }
        private void picBoxFrame_DoubleClick(object sender, EventArgs e)
        {
            if ((e as MouseEventArgs).Button == MouseButtons.Left)
            {
                this._animationTimer.Enabled = !this._animationTimer.Enabled;
                if (this._animationTimer.Enabled)
                {
                    this._animationStartPoint = this._currentFrameIndex;
                }
                else
                {
                    this._currentFrameIndex = this._animationStartPoint - 1;
                }
                this.NextFrame();
            }
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            this.NextFrame();
        }
        #endregion
    }
}
