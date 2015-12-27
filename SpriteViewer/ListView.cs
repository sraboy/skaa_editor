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
using BrightIdeasSoftware;
using TrulyObservableCollection;

namespace Capslock.WinForms.SpriteViewer
{
    public partial class ListView : UserControl
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

        //public ObjectListView ObjectListViewControl
        //{
        //    get
        //    {
        //        return this.objectListView1;
        //    }
        //    set
        //    {
        //        this.objectListView1 = value;
        //    }
        //}

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
            //if(this.objectListView1.Columns.Count > 0)
            this.objectListView1.SetObjects(frames);
        }
        internal void SetSelectedItem(Guid frameGuid)
        {
            IEnumerable<IFrame> frames = this.objectListView1.Objects as IEnumerable<IFrame>;
            var frame = frames.Single(f => f.Guid == frameGuid);
            this.objectListView1.SelectObject(frame);
        }
        #endregion

        #region Private Methods
        private object SpriteFrameImageGetter(object rowObject)
        {
            IFrame f = (IFrame) rowObject;
            if (this.objectListView1.RowHeight < f?.Bitmap?.Height)
                this.objectListView1.RowHeight = (int) f?.Bitmap?.Height;
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
            IFrame f = olv.SelectedObject as IFrame;
            if (f != null)
                RaiseActiveFrameChangedEvent(new FrameChangedEventArgs(f.Guid));
        }
        #endregion

 
    }
}
