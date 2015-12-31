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
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
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
        protected virtual void OnActiveFrameChanged(FrameChangedEventArgs e)
        {
            EventHandler handler = _activeFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private int _currentFrameIndex;
        private TrulyObservableCollection<IFrame> _frames;
        private IFrame _activeFrame;
        private IFrame _animationStartFrame;


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

                    if (this._activeFrame == null)
                        this._currentFrameIndex = -1;
                    else
                        this._currentFrameIndex = this._frames.IndexOf(this._frames.Single(f => f.Guid == this._activeFrame.Guid));
                    UpdateCurrentFrame();
                }
            }
        }


        public TimelineView()
        {
            InitializeComponent();

            this._picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            this._animationTimer.Enabled = false;
            this._animationTimer.Tick += AnimationTimer_Tick;
            this._animationTimer.Interval = 150;
            this._frames = new TrulyObservableCollection<IFrame>();
        }

        #region Internal Methods
        internal void SetFrameList(TrulyObservableCollection<IFrame> frames)
        {
            this._frames = frames;

            if (this._frames?.Count > 0)
            {
                foreach (IFrame f in frames)
                    f.PropertyChanged += Frame_PropertyChanged;

                this.ActiveFrame = frames[0];
                this._animationTimer.Enabled = false;
            }

            SetupUI();
        }

        private void Frame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Refresh();
            //Invalidate(true);
        }
        #endregion

        #region Private Methods
        private void NextFrame()
        {
            if (this._frames.Count > 1)
            {
                this._currentFrameIndex = (this._currentFrameIndex + 1) % (this._frames.Count - 1);
                UpdateCurrentFrame();
            }
        }
        private void PrevFrame()
        {
            if (this._frames.Count > 1)
            {
                this._currentFrameIndex--;
                if (this._currentFrameIndex < 0)
                {
                    this._currentFrameIndex = this._frames.Count;
                }
                this._currentFrameIndex %= this._frames.Count - 1;
                UpdateCurrentFrame();
            }
        }
        private void UpdateCurrentFrame()
        {
            if (this._currentFrameIndex == -1)
            {
                this._activeFrame = null;
                this._picBoxFrame.Image = null;
                return;
            }
            if (this._frames.Count > 0)
            {
                this._picBoxFrame.Image = this._frames[this._currentFrameIndex].Bitmap ?? null;
                this._sliderBar.Value = this._currentFrameIndex;

                if (!this._animationTimer.Enabled)
                    OnActiveFrameChanged(new FrameChangedEventArgs(this._frames[this._currentFrameIndex]));
            }
        }
        private void SetupUI()
        {
            this._sliderBar.Maximum = this._frames?.Count - 1 ?? 0;
            this._sliderBar.Minimum = 0;
            this.ActiveFrame = this._frames?[0];
        }
        private bool SetCurrentFrame()
        {
            if (this._currentFrameIndex != -1)
            {
                UpdateCurrentFrame();
                return true;
            }
            else
            {
                this._currentFrameIndex = 0;
                UpdateCurrentFrame();
                return false;
            }
        }
        #endregion

        #region Event Handlers
        private void picBoxFrame_Click(object sender, MouseEventArgs e)
        {
            if (this._frames.Count < 1)
                return;

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
                    this._animationStartFrame = this._frames[this._currentFrameIndex];
                }
                else
                {
                    this._currentFrameIndex = this._frames.IndexOf(this._animationStartFrame) - 1;
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
