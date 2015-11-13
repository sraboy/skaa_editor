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

namespace Capslock.WinForms.ImageEditor
{
    public class DrawingToolSelectedEventArgs : EventArgs
    {
        private ToolModes _selectedTool;
        public ToolModes SelectedTool
        {
            get
            {
                return this._selectedTool;
            }
            private set
            {
                if (this._selectedTool != value)
                    this._selectedTool = value;
            }
        }

        public DrawingToolSelectedEventArgs(ToolModes selectedTool)
        {
            this.SelectedTool = selectedTool;
        }
    }
}
