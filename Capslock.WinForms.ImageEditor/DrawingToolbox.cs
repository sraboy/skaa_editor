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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Capslock.WinForms.ImageEditor
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

 


