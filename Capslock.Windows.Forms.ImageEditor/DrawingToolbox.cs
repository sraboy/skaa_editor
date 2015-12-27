#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
* Copyright © 2013-2015 Cyotek Ltd.
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
using System.Windows.Forms;

namespace Capslock.Windows.Forms.ImageEditor
{
    public partial class DrawingToolbox : UserControl
    {
        #region Events
        [NonSerialized]
        private EventHandler _selectedToolChanged;
        /// <summary>
        /// This event is raised when the currently-selected tool changes. It's default <code>add</code> accessor protects against
        /// multiple subscriptions from the same subscriber by checking its invocation list.
        /// </summary>
        public virtual event EventHandler SelectedToolChanged
        {
            add
            {
                if (_selectedToolChanged == null || !_selectedToolChanged.GetInvocationList().Contains(value))
                {
                    _selectedToolChanged += value;
                }
            }
            remove
            {
                _selectedToolChanged -= value;
            }
        }
        protected virtual void OnSelectedToolChanged(DrawingToolSelectedEventArgs e)
        {
            EventHandler handler = _selectedToolChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private DrawingTools _selectedTool = DrawingTools.None;
        /// <summary>
        /// The currently-selected tool from <see cref="DrawingTools"/>
        /// </summary>
        public virtual DrawingTools SelectedTool
        {
            get
            {
                return this._selectedTool;
            }
            protected set
            {
                if (this._selectedTool != value)
                {
                    this._selectedTool = value;
                    OnSelectedToolChanged(new DrawingToolSelectedEventArgs(this._selectedTool));
                }
            }
        }


        public DrawingToolbox()
        {
            InitializeComponent();

            this._selectedTool = DrawingTools.None;
        }


        private void btnTool_Click(object sender, EventArgs e)
        {
            CheckBox cb = (sender as CheckBox);
            switch (cb.Name)
            {
                case "btnPanTool":
                    this.SelectedTool = DrawingTools.Pan;
                    break;
                case "btnPencilTool":
                    this.SelectedTool = DrawingTools.Pencil;
                    break;
                case "btnPaintBucketTool":
                    this.SelectedTool = DrawingTools.PaintBucket;
                    break;
                case "btnLineTool":
                    this.SelectedTool = DrawingTools.Line;
                    break;
                default:
                    this.SelectedTool = DrawingTools.None;
                    break;
            }
            ToggleCheckBoxes(sender);
        }
        private void ToggleCheckBoxes(object sender)
        {
            string senderCheckBoxName = (sender as CheckBox)?.Name;

            foreach(CheckBox c in this.Controls)
            {
                //don't change the sender, set others to false
                c.Checked = c.Name == senderCheckBoxName ? c.Checked : false;
            }
        }

        /// <summary>
        /// This method will change the <see cref="SelectedTool"/> to <see cref="DrawingTools.None"/>.
        /// </summary>
        /// <remarks>
        /// Use this if the user's current activity makes them ineligible to use a tool (e.g., the image has been closed).
        /// </remarks>
        public void CloseSelectedTool()
        {
            this.SelectedTool = DrawingTools.None;
            ToggleCheckBoxes(null);
        }
    }
}

 


