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
        /*
            Event Handling
        */
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

//<<<<<<< HEAD
            this.picBoxFrame.Image = this.Frames[CurrentFrame];
            this.frameSlider.Value = this.CurrentFrame;
//=======
//            this._activeFrameIndex = this.ActiveSprite.Frames.FindIndex(0, (f => f == _activeFrame));
//            this.picBoxFrame.Image = this._activeFrame?.IndexedBitmap?.Bitmap;
//            this.frameSlider.Value = this._activeFrameIndex == -1 ? 0 : this._activeFrameIndex;

//            if (handler != null)
//            {
//                handler(this, e);
//            }
//        }
//        [field: NonSerialized]
//        private EventHandler _activeSpriteChanged;
//        public event EventHandler ActiveSpriteChanged
//        {
//            add
//            {
//                if (_activeSpriteChanged == null || !_activeSpriteChanged.GetInvocationList().Contains(value))
//                {
//                    _activeSpriteChanged += value;
//                }
//            }
//            remove
//            {
//                _activeSpriteChanged -= value;
//            }
//        }
//        protected virtual void RaiseActiveSpriteChangedEvent(EventArgs e)
//        {
//            EventHandler handler = _activeSpriteChanged;
//>>>>>>> alphav4_working

            if (handler != null)
            {
                handler(this, e);
            }
        }

//<<<<<<< HEAD
        // Variables
        private List<Image> Frames;
//=======
//        private Sprite _activeSprite;
//        private Frame _activeFrame;
//        private int _activeFrameIndex;
//        private int _preAnimateActiveFrameIndex;
//>>>>>>> alphav4_working

        public int CurrentFrame;
        public int AnimationStartPoint;
        public bool Initialized {
            get
            {
//<<<<<<< HEAD
                return this.Frames != null;
            }
        }


        // Constructor
//=======
//                return this.picBoxFrame;
//            }
//            set
//            {
//                if (this.picBoxFrame != value)
//                    this.picBoxFrame = value;
//            }
//        }

//        //todo: remove Sprite and Frame references. Replace with List<Bitmap>
//        public Sprite ActiveSprite
//        {
//            get
//            {
//                return this._activeSprite;
//            }
//            set
//            {
//                if(this._activeSprite != value)
//                {
//                    this._activeSprite = value;
//                    OnActiveSpriteChanged(EventArgs.Empty);
//                }
//            }
//        }
//        public Frame ActiveFrame
//        {
//            get
//            {
//                if (!this.animationTimer.Enabled)
//                    return this._activeFrame;
//                else
//                    return this._activeSprite.Frames[_preAnimateActiveFrameIndex];
//            }
//            set
//            {
//                if (this._activeFrame != value)
//                {
//                    this._activeFrame = value;
//                    OnActiveFrameChanged(EventArgs.Empty);
//                }
//            }
//        }
        
//>>>>>>> alphav4_working
        public TimelineControl()
        {
            InitializeComponent();

            this.picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            this.animationTimer.Enabled = false;
            this.animationTimer.Tick += AnimationTimer_Tick;
            this.animationTimer.Interval = 150;
        }

        public void SetFrameList(List<Image> frames)
        {
            this.Frames = frames;

            this.animationTimer.Enabled = false;
            if (Initialized)
            {
                this.frameSlider.Maximum = this.Frames.Count - 1;
                this.frameSlider.Minimum = 0;
                this.SetCurrentFrame();
            }
        }

        private void picBoxFrame_Click(object sender, MouseEventArgs e) 
        {
//<<<<<<< HEAD
            if (!Initialized) return;
//=======
            //this._activeFrameIndex = this.ActiveSprite == null ? 0 : this.ActiveSprite.Frames.FindIndex(0, (f => f == _activeFrame));
            //this.picBoxFrame.Image = this._activeFrame?.IndexedBitmap.Bitmap;
            //this.frameSlider.Value = this._activeFrameIndex >= 0 ? this._activeFrameIndex : 0;
//>>>>>>> alphav4_working

            if (!this.animationTimer.Enabled)
            {
                switch(e.Button)
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
                        } else
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
            this.CurrentFrame = frameSlider.Value;
            this.SetCurrentFrame();
        }
        private void picBoxFrame_DoubleClick(object sender, EventArgs e)
        {
            this.animationTimer.Enabled = !this.animationTimer.Enabled;
            if (this.animationTimer.Enabled)
            {
                this.AnimationStartPoint = this.CurrentFrame;
            } else
            {
                this.CurrentFrame = this.AnimationStartPoint - 1;
            }
            this.NextFrame();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            this.NextFrame();
        }

        private void NextFrame() {
            if (this.Initialized)
            {
                this.CurrentFrame = (this.CurrentFrame + 1) % (Frames.Count - 1);
                this.SetCurrentFrame();
            }
        }
        private void PrevFrame()
        {
            if (this.Initialized)
            {
                this.CurrentFrame--;
                if (this.CurrentFrame < 0)
                {
                    this.CurrentFrame = Frames.Count;
                }
                this.CurrentFrame %= Frames.Count - 1;
                this.SetCurrentFrame();
            }
        }
        private void SetCurrentFrame(Image frame)
        {
            this.CurrentFrame = this.Frames.IndexOf(frame);
            this.SetCurrentFrame();
        }
        private void SetCurrentFrame(int frameIndex)
        {
            this.CurrentFrame = frameIndex;
            this.SetCurrentFrame();
        }
        private void SetCurrentFrame()
        {
            this.picBoxFrame.Image = this.Frames[this.CurrentFrame];
            RaiseActiveFrameChangedEvent(EventArgs.Empty);
        }

        public Image GetActiveFrame()
        {
            return this.Frames[this.CurrentFrame];
        }
        //todo: rename this to SetCurrentFrame to keep naming consistency
        public void UpdateCurrentFrame(Image frame)
        {
            this.Frames[this.CurrentFrame] = frame;
            this.SetCurrentFrame();
        }
    }
}
