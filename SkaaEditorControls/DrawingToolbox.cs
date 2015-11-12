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

namespace SkaaEditorControls
{
    public partial class DrawingToolbox : UserControl
    {
        [NonSerialized]
        private EventHandler _selectedToolChanged;
        public event EventHandler SelectedToolChanged
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

        private DrawingTools _selectedTool = DrawingTools.None;
        public DrawingTools SelectedTool
        {
            get
            {
                return this._selectedTool;
            }
            private set
            {
                if (this._selectedTool != value)
                {
                    this._selectedTool = value;
                    OnSelectedToolChanged(new DrawingToolSelectedEventArgs(this._selectedTool));
                }
            }
        }
        private Cursor _toolCursor;
        public Cursor ToolCursor
        {
            get
            {
                return this._toolCursor;
            }
            private set
            {
                if (this._toolCursor != value)
                    this._toolCursor = value;
            }
        }


        private Stream _panToolCursorStream;
        private Stream _pencilToolCursorStream;
        private Stream _paintBucketToolCursorStream;

        public DrawingToolbox()
        {
            InitializeComponent();
            this._selectedTool = DrawingTools.None;
            this._panToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PanToolCursor.cur"));
            this._pencilToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PencilToolCursor.cur"));
            this._paintBucketToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PaintBucketToolCursor.cur"));
        }

        private void btnPanTool_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                this.ToolCursor = new Cursor(this._panToolCursorStream);
                this.SelectedTool = DrawingTools.Pan;
                this._panToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void btnPencilTool_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                this.ToolCursor = new Cursor(this._pencilToolCursorStream);
                this.SelectedTool = DrawingTools.Pencil;
                this._pencilToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void btnFillTool_Click(object sender, EventArgs e)
        {
            if((sender as CheckBox).Checked == true)
            { 
                this.ToolCursor = new Cursor(this._paintBucketToolCursorStream);
                this.SelectedTool = DrawingTools.PaintBucket;
                this._paintBucketToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void ToggleCheckBoxes(object sender)
        {
            CheckBox cb = sender as CheckBox;

            foreach(CheckBox c in this.Controls)
            {
                //don't change the sender, set others to false
                c.Checked = c.Name == cb.Name ? c.Checked : false;
            }
        }
    }
}

 


