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

        private int /*_sprFrameRawDataSize,*/ _pixelSize, _height, _width;
        private Sprite _parentSprite;
        private byte[] _frameBmpData, _frameRawData;

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
        /// <summary>
        /// The size, in bytes, of the SPR data, not counting the four bytes used to represent this value.
        /// This value needs to be recalculated if the frame is edited due to the transparency compression.
        /// </summary>
        //public int SprFrameRawDataSize
        //{
        //    get
        //    {
        //        return this._sprFrameRawDataSize;
        //    }
        //    set
        //    {
        //        if (this._sprFrameRawDataSize != value)
        //            this._sprFrameRawDataSize = value;
        //    }
        //}
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
        //public ColorPalette Palette
        //{
        //    get
        //    {
        //        return this.ParentSprite.Palette;
        //    }
        //}

        public List<DataRow> GameSetDataRows
        {
            get;
            set;
        }

        public int SprBitmapOffset;
        public bool PendingChanges;

        public SpriteFrame(int sprOffset)
        {
            this.SprBitmapOffset = sprOffset;
            this.GameSetDataRows = new List<DataRow>();
        }

        //public SpriteFrame() { }
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

        /// <summary>
        /// Fills this frame's <see cref="FrameBmpData"/> and <see cref="FrameRawData"/> byte arrays.
        /// </summary>
        /// <param name="stream">
        /// <remarks>
        /// Note: <see cref="FrameBmpData"/> is pre-filled with 0xff bytes. See the class constructors for details. 
        /// Simply put, since 0x00 is actually used for black, we need to use one of the palette entries that 
        /// signifies transparency. In pal_std.res, this is 0xf8-0xff; 0xff was chosen because it does not appear 
        /// to be used at all. 
        /// </remarks>
        
        //public Bitmap BuildBitmapFromSpr()
        //{
        //    int idx;
        //    Bitmap bmp = new Bitmap(this.Width, this.Height);
        //    FastBitmap fbmp = new FastBitmap(bmp);
        //    fbmp.LockImage();
        //    for (int y = 0; y < this.Height; y++)
        //    {
        //        for (int x = 0; x < this.Width; x++)
        //        {
        //            idx = FrameBmpData[y * this.Width + x];
        //            Color pixel = this.Palette.Entries[idx];
        //            fbmp.SetPixel(x, y, pixel);
        //        }
        //    }
        //    fbmp.UnlockImage();
        //    Color transparentByte = Color.FromArgb(0xff);
        //    bmp.MakeTransparent(transparentByte);
        //    this.ImageBmp = bmp;
        //    return this.ImageBmp;
        //}

        ///// <summary>
        ///// Supplies an SPR-formatted version of this frame.
        ///// </summary>
        ///// <returns>Returns a byte array containing the frame data in 7KAA's 
        ///// SPR format: int32 size, int16 width, int16 height, byte[] data.</returns>
        //public byte[] BuildBitmap8bppIndexed()
        //{
        //    byte palColorByte;                        
        //    byte transparentByte = 0xf8;
        //    int transparentByteCount = 0;
        //    int realOffset = 8; //since our array offset is unaware of the header
        //    byte[] indexedData = new byte[this.PixelSize + 4];

        //    // todo: will have to recalculate height/width if bitmap size changes
        //    byte[] width = BitConverter.GetBytes((short) this.Width);
        //    byte[] height = BitConverter.GetBytes((short) this.Height);

        //    /**************************************************************************
        //    *  BitConverter is required, rather than Convert.ToByte(), so we can 
        //    *  get the full 16- or 32-bit representations of the values. This is also  
        //    *  why Height and Width are both cast to short, to ensure we get a 16-bit
        //    *  representation of each value to match the binary's file format of:
        //    *  ____________________________________________________________ 
        //    *  | 4 byte Size | 2 byte Width | 2 byte Height | byte[] data |
        //    **************************************************************************/
        //    int seek_pos = 4; //first four bytes are for SprSize, at the end of the function
        //    Buffer.BlockCopy(width, 0, indexedData, seek_pos, width.Length);
        //    seek_pos += width.Length;
        //    Buffer.BlockCopy(height, 0, indexedData, seek_pos, height.Length);
        //    seek_pos += height.Length;

        //    List<Color> Palette = new List<Color>();
        //    foreach (Color c in this.Palette.Entries)
        //    {
        //        Palette.Add(c);
        //    }

        //    //BuildBitmap8bppIndexed() may be called to save the 
        //    //current image before making changes. So we build  
        //    //a 32-bit BMP with current FrameData so it can be used 
        //    //below and to build this SPR to return to the caller. 
        //    if (this.ImageBmp == null)
        //        BuildBitmapFromSpr();

        //    //the below is pretty much the same as SetPixel() but reversed(ish)
        //    for (int y = 0; y < this.ImageBmp.Height; ++y)
        //    {
        //        for (int x = 0; x < this.ImageBmp.Width; ++x)
        //        {
        //            Color pixel = this.ImageBmp.GetPixel(x, y);
        //            var pixARGB = pixel.ToArgb();
        //            palColorByte = Convert.ToByte(Palette.FindIndex(c => c == Color.FromArgb(pixARGB)));

        //            if (palColorByte > 0xf8) //0xf9 - 0xff are transparent
        //                transparentByteCount++;

        //            // Once we hit a non-zero pixel, we need to write out the transparent pixel marker
        //            // and the count of transparent pixels. We then write out the current non-zero pixel.
        //            // The second expression after || below is to check if we're on the last pixel of the
        //            // image. If so, and the final pixels were colored, there won't be a next pixel to be 
        //            // below 0xf8 so we need to write it out anyway.
        //            bool lastByte = (x == (this.Width - 1) && (y == (this.Height - 1)));

        //            if (palColorByte <= 0xf8 || lastByte)
        //            { 
        //                if (transparentByteCount > 0)  
        //                {
        //                    // Write 0xf8[dd] where [dd] is transparent byte count, unless the
        //                    // number of transparent bytes is 6 or less, then just use the other
        //                    // codes below. Seems like the devs were pretty ruthless in trying to 
        //                    // save disk space back in 1997.
        //                    if (transparentByteCount > 7) 
        //                    { 
        //                        indexedData[realOffset] = transparentByte;
        //                        realOffset++;
        //                        indexedData[realOffset] = Convert.ToByte(transparentByteCount);
        //                        realOffset++;
        //                        transparentByteCount = 0;
        //                    }
        //                    else
        //                    {
        //                        //less than 8 and 7kaa cuts down on file size by just writing one byte      
        //                        //transparentByteCount = 2: 0xfe
        //                        //transparentByteCount = 3: 0xfd
        //                        //transparentByteCount = 4: 0xfc
        //                        //transparentByteCount = 5: 0xfb
        //                        //transparentByteCount = 6: 0xfa
        //                        //transparentByteCount = 7: 0xf9
        //                        indexedData[realOffset] = Convert.ToByte(0xff - (transparentByteCount - 1));
        //                        realOffset++;
        //                        transparentByteCount = 0;
        //                    }
        //                }
                        
        //                //there is no other byte to write out
        //                if (!lastByte)
        //                {
        //                    indexedData[realOffset] = palColorByte;
        //                    realOffset++;
        //                }
        //            }
        //        }//end inner for
        //    }//end outer for

        //    //this.SprFrameRawDataSize = realOffset - 4;
        //    byte[] size = BitConverter.GetBytes(realOffset - 4); //subtract four because int32 size does not count the int32
        //    if (size.Length > 4) throw new Exception("SPR size must be Int32!");
        //    Buffer.BlockCopy(size, 0, indexedData, 0, size.Length);

        //    //Since FrameData is set to ((Width * Height) + 4), its length will
        //    //be based on the real pixels, not the "compressed" length with
        //    //the transparent pixels. This makes it impossible to calculate the
        //    //offsets of the next frames in the sprite to build a new game set.
            
        //    Array.Resize<byte>(ref indexedData, realOffset);
        //    this.FrameRawData = indexedData;

        //    return this.FrameRawData;
        //    //return indexedData;
        //}

        /// <summary>
        /// Saves the new 32-bit BMP and generates new SPR data based on the edited <see cref="Bitmap"/>
        /// </summary>
        internal void ProcessUpdates(Bitmap bmp)
        {
            this.ImageBmp = bmp;
            this.PendingChanges = true;
            
            this.FrameRawData = SprDataHandlers.FrameBmpToSpr(this, bmp.Palette);
            OnFrameUpdated(EventArgs.Empty);
        }
    }
    
}
