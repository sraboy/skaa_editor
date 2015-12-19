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
using System.Drawing.Imaging;
using System.IO;
using Cyotek.Windows.Forms;

namespace SkaaEditorControls
{
    public partial class SkaaColorChooser : ColorGrid
    {
        public new ColorCollection CustomColors = null;

        //todo: add ActivePrimaryColor & ActiveSecondaryColor, right-click to select secondary
        //todo: automatically deduce Black and Transparent ARGB values based on palette to provide as defaults

        public SkaaColorChooser() : base ()
        {
            //Turns off the empty boxes at the bottom of the grid
            this.ShowCustomColors = false;

            //The palette is set to "Standard256" for the sake of designer showing 
            //256 colors. We want it set to None to ensure we don't accidentally 
            //show colors that don't exist in our palette
            this.Palette = Cyotek.Windows.Forms.ColorPalette.None;
        }
    }
}
