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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using TrulyObservableCollection;

namespace Capslock.Windows.Forms.SpriteViewer
{
    public partial class ListView : UserControl
    {
        private IFrame _activeFrame;

        #region ActiveFrameChanged Event
        [field: NonSerialized]
        private EventHandler _activeFrameChanged;

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
                    this.objectListView1.SelectedItem = null;
                    this.objectListView1.SelectObject(this._activeFrame);
                    OnActiveFrameChanged(new FrameChangedEventArgs(this._activeFrame));
                }
            }
        }

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

        public ListView()
        {
            InitializeComponent();
            SetUpObjectListView();
        }

        #region Internal Methods
        internal void SetImageGetter(ImageGetterDelegate imageGetter)
        {
            this.colImage.ImageGetter = imageGetter;
        }
        internal void SetColumns(List<string> columns)
        {
            //todo: dynamically generate columns with Name/Aspect set from List<string>
        }
        internal void SetObjects(TrulyObservableCollection<IFrame> frames)
        {
            this.objectListView1.SetObjects(frames);
            if (frames != null)
            {
                foreach (IFrame f in frames)
                    f.PropertyChanged += Frame_PropertyChanged;

                this.ActiveFrame = frames[0];
            }
        }

        private void Frame_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }
        #endregion

        #region Private Methods
        private object SpriteFrameImageGetter(object rowObject)
        {
            IFrame f = (IFrame)rowObject;
            if (this.objectListView1.RowHeight < f?.Bitmap?.Height)
                this.objectListView1.RowHeight = (int)f?.Bitmap?.Height;
            return f.Bitmap;
        }
        private void SetUpObjectListView()
        {
            this.objectListView1.RowHeight = 40;
            this.objectListView1.ShowImagesOnSubItems = true;
            this.objectListView1.SelectionChanged += ObjectListView1_SelectionChanged;
        }
        #endregion

        #region Event Handlers
        private void ObjectListView1_SelectionChanged(object sender, EventArgs e)
        {
            var olv = sender as ObjectListView;
            this.ActiveFrame = olv.SelectedObject as IFrame;
        }
        #endregion


    }
}
