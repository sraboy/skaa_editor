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

namespace SkaaGameDataLib
{
    public class Sprite
    {
        private ColorPalette _pallet;

        public ColorPalette Palette
        {
            get
            {
                return this._pallet;
            }
            set
            {
                if(this._pallet != value)
                {
                    this._pallet = value;
                    UpdateFrames();
                }
            }
        }
        public List<SpriteFrame> Frames
        {
            get;
            set;
        }
        public Sprite(ColorPalette pal)
        {
            this.Palette = pal;
            this.Frames = new List<SpriteFrame>();
        }

        public SpriteFrame AddFrame(SpriteFrame sf = null)
        {
            if (sf == null)
            {
                sf = new SpriteFrame();
                this.Frames.Add(sf);
                return sf;
            }

            this.Frames.Add(sf);
            return sf;
        }

        public void UpdateFrames()
        {
            if (this.Frames != null)
            {
                foreach (SpriteFrame sf in this.Frames)
                {
                    sf.Palette = this.Palette;
                }
            }
        }

        public Byte[] BuildSPR()
        {
            List<byte[]> SPRArrays = new List<byte[]>();
            int initSize = 0;
            
            foreach (SpriteFrame sf in this.Frames)
            {
                SPRArrays.Add(sf.BuildBitmap8bppIndexed());
                initSize += (sf.Size + 4); //add another four for ulong size
            }

            int lastSize = 0;
            Byte[] save = new Byte[initSize];

            foreach (Byte[] ba in SPRArrays)
            {
                Buffer.BlockCopy(ba, 0, save, lastSize, Buffer.ByteLength(ba));
                lastSize += ba.Length;
            }

            return save;
        }
    }
}
