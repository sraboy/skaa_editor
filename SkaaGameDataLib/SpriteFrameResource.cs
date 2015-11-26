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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Data;
using BitmapProcessing;

namespace SkaaGameDataLib
{
    public class SpriteFrameResource : IndexedBitmap
    {
        private Sprite _parentSprite;

        public Sprite ParentSprite
        {
            get
            {
                return this._parentSprite;
            }
            set
            {
                if (this._parentSprite != value)
                    this._parentSprite = value;
            }
        }
        public List<DataRow> GameSetDataRows;
        public int SprBitmapOffset;

        /// <summary>
        /// Initializes a new <see cref="SpriteFrameResource"/>.
        /// </summary>
        /// <param name="parentSprite">The <see cref="Sprite"/> containing this <see cref="SpriteFrameResource"/></param>
        /// <param name="stream"></param>
        public SpriteFrameResource(Sprite parentSprite) : base()
        {
            this.ParentSprite = parentSprite;
            this.GameSetDataRows = new List<DataRow>();
        }
        public SpriteFrameResource(int sprOffset) : base()
        {
            this.SprBitmapOffset = sprOffset;
            this.GameSetDataRows = new List<DataRow>();
        }
    }
}