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
using System.Diagnostics;

namespace SpriteViewer
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
        private List<IFrame> _frames;
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
            this._frameImages = new List<Image>();
            this._frames = new List<IFrame>();
        }

        #region Internal Methods
        internal void SetFrameList(List<Image> frames)
        {
            this._frameImages = frames;
            this._animationTimer.Enabled = false;
            SetupUI();
        }
        internal void SetFrameList(List<IFrame> frames)
        {
            this._frames = frames;
            foreach (IFrame f in frames)
                this._frameImages.Add(f.Bitmap);

            this._animationTimer.Enabled = false;
            SetupUI();
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
        //internal void UpdateCurrentFrame(Image frame)
        //{
        //    this._currentFrameIndex = this._frameImages.FindIndex(img => img == frame);
        //    this.UpdateCurrentFrame();
        //}
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
            this._currentFrameIndex = this._frames.FindIndex(f => f == frame);
            return SetCurrentFrame();
        }
        internal bool SetCurrentFrameTo(Guid frameGuid)
        {
            this._currentFrameIndex = this._frames.FindIndex(f => f.Guid == frameGuid);

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
