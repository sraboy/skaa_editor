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

using System.Collections.Generic;
using System.Data;

namespace SkaaGameDataLib
{
    public class SkaaSpriteFrame : SkaaFrame
    {
        private SkaaSprite _parentSprite;
        public string Test = "test";
        public string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }
        public long BitmapOffset
        { 
            get
            {
                return base.BitmapOffset;
            }
            set
            {
                base.BitmapOffset = value;
            }
        }
   
        public SkaaSprite ParentSprite;
        public List<DataRow> GameSetDataRows;

        /// <summary>
        /// Initializes a new <see cref="SkaaSpriteFrame"/>.
        /// </summary>
        /// <param name="parentSprite">The <see cref="SkaaSprite"/> containing this <see cref="SkaaSpriteFrame"/></param>
        /// <param name="stream"></param>
        public SkaaSpriteFrame(SkaaSprite parentSprite)
        {
            this.ParentSprite = parentSprite;
            this.GameSetDataRows = new List<DataRow>();
        }
    }
}