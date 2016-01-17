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
using System.Diagnostics;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Capslock.Windows.Forms.SpriteViewer
{
    public partial class SpriteViewer : UserControl
    {
        public static readonly TraceSource Logger = new TraceSource($"{typeof(SpriteViewer)}", SourceLevels.All);

        #region Private Fields
        private IMultiImagePresenter _activeSprite;
        private int _currentAnimationFrameIndex;
        private int _thumbnailSize = 40;
        private bool _updateRequired;
        #endregion

        #region Public Properties
        public IMultiImagePresenter ActiveSprite
        {
            get
            {
                return _activeSprite;
            }
            private set
            {
                this._activeSprite = value;
            }
        }
        [Category("Appearance"),
        Description("The height/width to resize images to when displaying thumbnails. Also defines the ObjectListView.RowHeight value."),
        DefaultValue(typeof(int), "40")]
        public int ThumbnailSize
        {
            get
            {
                return _thumbnailSize;
            }

            set
            {
                this._thumbnailSize = value;
            }
        }
        #endregion

        #region Constructor
        public SpriteViewer()
        {
            InitializeComponent();
            SetupUI();
        }
        #endregion

        #region Private Methods
        private void SetupUI()
        {
            this.animationTimer.Enabled = false;
            this.animationTimer.Tick += animationTimer_Tick;
            this.animationTimer.Interval = 150;

            this._updateTimer.Enabled = true;
            this._updateTimer.Tick += updateTimer_Tick;
            this._updateTimer.Interval = 1000;

            this.trackBar.Maximum = this.ActiveSprite?.Frames?.Count - 1 ?? 0;
            this.trackBar.Minimum = 0;

            this.objectListView.RowHeight = this.ThumbnailSize;
            this.objectListView.ShowImagesOnSubItems = true;
            this.colImage.IsEditable = false; //otherwise, the user can "edit" by typing text, which just looks odd and is usless
            this.colImage.ImageGetter = ImageGetter;

            SetObjectListViewActiveFrame();
            SetTrackBarActiveFrame();
            SetPictureBoxActiveFrame();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (this._updateRequired)
            {
                this._updateRequired = false;
                this.objectListView.RebuildColumns();
            }
        }

        private void ResetUI()
        {
            this.trackBar.Maximum = this.ActiveSprite?.Frames?.Count - 1 ?? 0;

            SetObjectListViewActiveFrame();
            SetTrackBarActiveFrame();
            SetPictureBoxActiveFrame();
        }
        /// <summary>
        /// Sets the <see cref="ObjectListView.SelectedItem"/> to the object that
        /// corresponds to <see cref="IMultiImagePresenter.ActiveFrame"/>
        /// </summary>
        private void SetObjectListViewActiveFrame()
        {
            this.objectListView.SelectedItem = null;
            this.objectListView.SelectObject(this.ActiveSprite?.ActiveFrame);
        }
        private void SetTrackBarActiveFrame(int? value = null)
        {
            if (value == null)
            {
                int idx = this.ActiveSprite?.Frames?.IndexOf(this.ActiveSprite.ActiveFrame) ?? 0;
                if (idx == -1)
                    idx = 0;

                this.trackBar.Value = idx;
            }
            else
                this.trackBar.Value = (int)value;
        }
        private void SetPictureBoxActiveFrame(int? value = null)
        {
            if (value == null)
                this.pictureBox.Image = this.ActiveSprite?.ActiveFrame?.Bitmap;
            else
                this.pictureBox.Image = this.ActiveSprite.Frames[(int)value].Bitmap;
        }
        private object ImageGetter(object rowObject)
        {
            IFrame f = (IFrame)rowObject;
            return f.Thumbnail;
        }
        #endregion

        #region Public Methods
        public void SetActiveSprite(IMultiImagePresenter spr)
        {
            this._activeSprite = spr;

            this.objectListView.SetObjects(this.ActiveSprite?.Frames);

            if (this.ActiveSprite?.Frames?.Count > 0)
            {
                foreach (IFrame f in this.ActiveSprite.Frames)
                    f.PropertyChanged += IFrame_PropertyChanged;

                this.animationTimer.Enabled = false;
            }

            ResetUI();
        }
        #endregion

        #region Event Handlers
        private void trackBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.ActiveSprite.ActiveFrame = this.ActiveSprite.Frames[this.trackBar.Value];
            SetObjectListViewActiveFrame();
            SetPictureBoxActiveFrame();
        }
        private void objectListView_SelectionChanged(object sender, EventArgs e)
        {
            var olv = sender as ObjectListView;
            this.ActiveSprite.ActiveFrame = olv.SelectedObject as IFrame;
            SetTrackBarActiveFrame();
            SetPictureBoxActiveFrame();
        }
        private void pictureBox_Click(object sender, System.EventArgs e)
        {
            if (!this.animationTimer.Enabled)
            {
                int index = -1;

                switch ((e as MouseEventArgs).Button)
                {
                    case MouseButtons.Left:
                        if (this.trackBar.Value + 1 > this.ActiveSprite.Frames.Count - 1)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = this.trackBar.Value + 1;
                        }
                        break;
                    case MouseButtons.Right:
                        if (this.trackBar.Value - 1 < 0)
                        {
                            index = this.ActiveSprite.Frames.Count - 1;
                        }
                        else
                        {
                            index = this.trackBar.Value - 1;
                        }
                        break;
                    case MouseButtons.Middle:
                        PictureBoxSizeMode sizeMode = this.pictureBox.SizeMode;
                        if (sizeMode == PictureBoxSizeMode.CenterImage)
                        {
                            sizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            sizeMode = PictureBoxSizeMode.CenterImage;
                        }
                        this.pictureBox.SizeMode = sizeMode;
                        break;
                }

                if (index > -1)
                {
                    this.ActiveSprite.ActiveFrame = this.ActiveSprite.Frames[index];
                    SetObjectListViewActiveFrame();
                    SetTrackBarActiveFrame();
                }
            }
        }
        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if ((e as MouseEventArgs).Button == MouseButtons.Left)
            {
                this.animationTimer.Enabled = !this.animationTimer.Enabled;
                if (this.animationTimer.Enabled)
                {
                    this._currentAnimationFrameIndex = this.ActiveSprite.Frames.IndexOf(this.ActiveSprite.ActiveFrame);
                }
                else
                {
                    this.ActiveSprite.ActiveFrame = this.ActiveSprite.Frames[this._currentAnimationFrameIndex];
                }

                SetTrackBarActiveFrame();
                SetPictureBoxActiveFrame();
            }
        }
        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if (this.trackBar.Value + 1 < this.ActiveSprite.Frames.Count - 1)
            {
                this._currentAnimationFrameIndex++;

                SetPictureBoxActiveFrame(this._currentAnimationFrameIndex);
                SetTrackBarActiveFrame(this._currentAnimationFrameIndex);
            }
            else
            {
                this._currentAnimationFrameIndex = 0;

                SetPictureBoxActiveFrame(this._currentAnimationFrameIndex);
                SetTrackBarActiveFrame(this._currentAnimationFrameIndex);
            }
        }
        private void IFrame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Calling RebuildColumns() for every property change takes forever
            //and is unnecessary. If a bitmap is update, then all the offsets
            //in the sprite get updated. We'd be rebuilding all the columns
            //every time each of them fires the event.
            this._updateRequired = true;
        }
        #endregion
    }
}
