/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaEditor
{
    //MOVED TO SkaaColorChooser control

    //public static class Helper
    //{
    //    public static ColorPalette LoadPalette()
    //    {
    //        //TODO: string param from OpenFileDlg

    //        ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;// = new ColorPalette();

    //        FileStream fs = File.OpenRead("../../data/resource/pal_std.res");
    //        fs.Seek(8, SeekOrigin.Begin);

    //        for (int i = 0; i < 256; i++)
    //        {
    //            int r = fs.ReadByte();
    //            int g = fs.ReadByte();
    //            int b = fs.ReadByte();

    //            pal.Entries[i] = Color.FromArgb(255, r, g, b);
    //        }

    //        return pal;
    //    }
    //}
}
