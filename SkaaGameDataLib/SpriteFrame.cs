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
    [Serializable]
    public class SpriteFrame
    {
        #region Event Handlers
        [field: NonSerialized]
        private EventHandler _frameUpdated;
        public event EventHandler FrameUpdated
        {
            add
            {
                if (_frameUpdated == null || !_frameUpdated.GetInvocationList().Contains(value))
                {
                    _frameUpdated += value;
                }
            }
            remove
            {
                _frameUpdated -= value;
            }
        }
        protected virtual void OnFrameUpdated(EventArgs e)
        {
            EventHandler handler = _frameUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Private Members
        private int /*_sprFrameRawDataSize,*/ _pixelSize, _height, _width;
        private Sprite _parentSprite;
        private byte[] _frameBmpData, _frameRawData;
        #endregion

        #region Public Members
        /// <summary>
        /// The size, in pixels, of the frame. Simple height * width.
        /// </summary>
        public int PixelSize
        {
            get
            {
                return this._pixelSize;
            }
            set
            {
                if (this._pixelSize != value)
                    this._pixelSize = value;
            }
        }       
        public int Height
        {
            get
            {
                return this._height;
            }
            set
            {
                if(this._height != value)
                {
                    this._height = value;
                }
            }
        }
        public int Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (this._width != value)
                {
                    this._width = value;
                }
            }
        }
        public byte[] FrameBmpData
        {
            get
            {
                return this._frameBmpData;
            }
            set
            {
                if (this._frameBmpData != value)
                    this._frameBmpData = value;
            }
        }
        public byte[] FrameRawData
        {
            get
            {
                return this._frameRawData;
            }
            set
            {
                if (this._frameRawData != value)
                    this._frameRawData = value;
            }
        }
        public Bitmap ImageBmp
        {
            get;
            set;
        }
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
        public List<DataRow> GameSetDataRows
        {
            get;
            set;
        }
        public int SprBitmapOffset;
        public bool PendingChanges;
        #endregion

        #region Constructors
        public SpriteFrame(int sprOffset)
        {
            this.SprBitmapOffset = sprOffset;
            this.GameSetDataRows = new List<DataRow>();
        }
        /// <summary>
        /// Initializes a new <see cref="SpriteFrame"/>.
        /// </summary>
        /// <param name="parentSprite">The <see cref="Sprite"/> containing this <see cref="SpriteFrame"/></param>
        /// <param name="stream"></param>
        public SpriteFrame(Sprite parentSprite)//, Stream stream)
        {
            this.ParentSprite = parentSprite;
            //this.SprBitmapOffset = (int) stream.Position;
            this.GameSetDataRows = new List<DataRow>();
            //this.FrameRawData = GameDataHandlers.SprStreamToSpriteFrame(stream);
            //ReadSprData(stream);
            //BuildBitmapFromSpr();
        }
        #endregion

        /// <summary>
        /// Saves the new 32-bit BMP and generates new SPR data based on the edited <see cref="Bitmap"/>
        /// </summary>
        internal void ProcessUpdates(Bitmap bmp)
        {
            if (this.PendingChanges == true)
            {
                this.ImageBmp = bmp;
                SprDataHandlers.FrameBmpToSpr(this, this.ParentSprite.Resource.Palette);
                this.PendingChanges = false;
               
                OnFrameUpdated(EventArgs.Empty);  //this doesn't get triggered by FrameBmpToSpr()
            }
        }
    }
    
}