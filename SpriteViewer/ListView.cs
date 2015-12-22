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

namespace SpriteViewer
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
        internal void SetObjects(List<IFrame> frames)
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
