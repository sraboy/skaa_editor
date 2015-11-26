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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public class Sprite
    {
        #region Event Handlers
        [NonSerialized]
        private EventHandler _spriteUpdated;
        public event EventHandler SpriteUpdated
        {
            add
            {
                if (_spriteUpdated == null || !_spriteUpdated.GetInvocationList().Contains(value))
                {
                    _spriteUpdated += value;
                }
            }
            remove
            {
                _spriteUpdated -= value;
            }
        }
        protected virtual void OnSpriteUpdated(EventArgs e)
        {
            EventHandler handler = _spriteUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Private Members
        private List<SpriteFrameResource> _frames;
        private string _spriteId;
        private SpriteResource _resource;
        #endregion
        
        #region Public Properties
        public List<SpriteFrameResource> Frames
        {
            get
            {
                return this._frames;
            }
            set
            {
                if(this._frames != value)
                {
                    this._frames = value;
                }
            }
        }
        public string SpriteId
        {
            get
            {
                return this._spriteId;
            }
            set
            {
                if (this._spriteId != value)
                {
                    this._spriteId = value;
                }
            }
        }
        public SpriteResource Resource
        {
            get
            {
                return this._resource;
            }
            set
            {
                if (this._resource != value)
                {
                    this._resource = value;
                }
            }
        }
        #endregion

        #region Constructors
        public Sprite()//ColorPalette pal)
        {
            this.Frames = new List<SpriteFrameResource>();
            this.Resource = new SpriteResource();
        }
        #endregion

        public bool SetSpriteDataView(DataView dv)
        {
            if (dv != null)
            {
                this.Resource.SpriteDataView = dv;
                return this.Resource.MatchFrameOffsets(this);
            }
            else
            {
                Trace.WriteLine($"No DataView assigned to {this.SpriteId}");
                return false;
            }
        }
        /// <summary>
        /// Builds a <see cref="Bitmap"/> sprite sheet containing all the frames of the specified <see cref="Sprite"/>
        /// with no padding between frames. The number of rows/columns of frames is the square root of the number of frames
        /// with an additional row added when the number of frames is not a perfect square.
        /// </summary>
        /// <returns>The newly-generated <see cref="Bitmap"/></returns>
        public Bitmap ToBitmap()//Sprite spr)
        {
            int totalFrames = this.Frames.Count;
            int spriteWidth = 0, spriteHeight = 0, columns = 0, rows = 0;

            double sqrt = Math.Sqrt(totalFrames);

            //figure out how many rows we need
            if (totalFrames % 1 != 0) //totalFrames is a perfect square
            {
                rows = (int) sqrt;
                columns = (int) sqrt;
            }
            else
            {
                rows = (int) Math.Floor(sqrt) + 1; //adds an additional row
                columns = (int) Math.Ceiling(sqrt);
            }

            //need the largest tile (by height and width) to set the row/column heights
            foreach (SpriteFrameResource sf in this.Frames)
            {
                if (sf.Width > spriteWidth)
                    spriteWidth = sf.Width;
                if (sf.Height > spriteHeight)
                    spriteHeight = sf.Height;
            }

            //the total height/width of the image to be created
            int exportWidth = columns * spriteWidth,
                exportHeight = rows * spriteHeight;

            Bitmap bitmap = new Bitmap(exportWidth, exportHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                int frameIndex = 0;

                for (int y = 0; y < exportHeight; y += spriteHeight)
                {
                    //once we hit the max frames, just break
                    for (int x = 0; x < exportWidth && frameIndex < this.Frames.Count; x += spriteWidth)
                    {
                        g.DrawImage(this.Frames[frameIndex].Bitmap, new Point(x, y));
                        frameIndex++;
                    }
                }
            }

            return bitmap;
        }
    }
}
