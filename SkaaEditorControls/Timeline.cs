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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkaaEditorControls
{
    public partial class TimelineControl : UserControl
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
        protected virtual void RaiseActiveFrameChangedEvent(EventArgs e)
        {
            EventHandler handler = _activeFrameChanged;

            this.picBoxFrame.Image = this._frameImages[_currentFrameIndex];
            this.frameSlider.Value = this._currentFrameIndex;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

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

        public TimelineControl()
        {
            InitializeComponent();

            this.picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            this.animationTimer.Enabled = false;
            this.animationTimer.Tick += AnimationTimer_Tick;
            this.animationTimer.Interval = 150;
        }

        #region Public Methods
        public void SetFrameList(List<Image> frames)
        {
            this._frameImages = frames;

            this.animationTimer.Enabled = false;
            if (_isInitialized)
            {
                this.frameSlider.Maximum = this._frameImages.Count - 1;
                this.frameSlider.Minimum = 0;
                this.UpdateCurrentFrame();
            }
        }
        public Image GetActiveFrame()
        {
            return this._frameImages[this._currentFrameIndex];
        }
        public int GetActiveFrameIndex()
        {
            return this._currentFrameIndex;
        }
        /// <summary>
        /// Changes the internal representation of the currently-displayed image to the specified image
        /// </summary>
        /// <param name="frame">The new image</param>
        public void UpdateCurrentFrame(Image frame)
        {
            this._frameImages[this._currentFrameIndex] = frame;
            this.UpdateCurrentFrame();
        }
        /// <summary>
        /// Updates the control to display the specified image, if it exists in the <see cref="List"/> 
        /// passed to <see cref="SetFrameList(List{Image})"/>
        /// </summary>
        /// <param name="frame">The image to find and display</param>
        /// <returns>true if the image was found and the control was updated, false otherwise</returns>
        public bool SetCurrentFrameTo(Image frame)
        {
            this._currentFrameIndex = this._frameImages.FindIndex(img => img == frame);

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
        //private void UpdateCurrentFrame(Image frame)
        //{
        //    this.CurrentFrame = this.Frames.IndexOf(frame);
        //    this.UpdateCurrentFrame();
        //}
        //private void UpdateCurrentFrame(int frameIndex)
        //{
        //    this.CurrentFrame = frameIndex;
        //    this.UpdateCurrentFrame();
        //}
        private void UpdateCurrentFrame()
        {
            this.picBoxFrame.Image = this._frameImages[this._currentFrameIndex];
            RaiseActiveFrameChangedEvent(EventArgs.Empty);
        }
        #endregion

        #region Event Handlers
        private void picBoxFrame_Click(object sender, MouseEventArgs e)
        {
            if (!_isInitialized) return;

            if (!this.animationTimer.Enabled)
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
                        PictureBoxSizeMode SM = this.picBoxFrame.SizeMode;
                        if (SM == PictureBoxSizeMode.CenterImage)
                        {
                            SM = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            SM = PictureBoxSizeMode.CenterImage;
                        }
                        this.picBoxFrame.SizeMode = SM;
                        break;
                }
            }
        }
        private void frameSlider_ValueChanged(object sender, EventArgs e)
        {
            this._currentFrameIndex = frameSlider.Value;
            this.UpdateCurrentFrame();
        }
        private void picBoxFrame_DoubleClick(object sender, EventArgs e)
        {
            this.animationTimer.Enabled = !this.animationTimer.Enabled;
            if (this.animationTimer.Enabled)
            {
                this._animationStartPoint = this._currentFrameIndex;
            }
            else
            {
                this._currentFrameIndex = this._animationStartPoint - 1;
            }
            this.NextFrame();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            this.NextFrame();
        }
        #endregion
    }
}
