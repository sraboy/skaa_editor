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

namespace SkaaGameDataLib
{
    [Serializable]
    public class SpriteFrame
    {
        private int _sprFrameRawDataSize, _pixelSize, _height, _width;
        private Sprite _parentSprite;

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
        public int SprFrameRawDataSize
        {
            get
            {
                return this._sprFrameRawDataSize;
            }
            set
            {
                if (this._sprFrameRawDataSize != value)
                    this._sprFrameRawDataSize = value;
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
        public byte[] FrameData
        {
            get;
            set;
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
        public ColorPalette Palette
        {
            get
            {
                return this.ParentSprite.Palette;
            }
        }
        public DataRow GameSetDataRow
        {
            get;
            set;
        }
        public uint? SprBitmapOffset;
        public bool PendingRawChanges;

        #region Constructors
        /// <summary>
        /// Default constructor, required for some functions.
        /// </summary>
        internal SpriteFrame() { }

        /// <summary>
        /// Initializes the new sprite frame of the specified size pre-filled with 0xff (transparent byte).
        /// </summary>
        /// <param name="sizeOfFrame">The size in bytes of the frame, including 2 bytes each for height and width</param>
        /// <param name="width">The width of the frame in pixels</param>
        /// <param name="height">The height of the frame in pixels</param>
        /// <param name="parent">The Sprite to which this frame belongs</param>
        /// <remarks> 
        /// We preset all bytes to 0xff, an unused palette entry that signifies a 
        /// transparent pixel.The default is 0x00, but that's actually used for 
        /// black.This is required due to the manual compression the 7KAA developers
        /// used in the SPR files. See <see cref="SetPixels(FileStream)"/> for the implementation.
        /// </remarks>
        public SpriteFrame(int sizeOfFrame, int width, int height, Sprite parent)
        {
            this.ParentSprite = parent;
            this.SprFrameRawDataSize = sizeOfFrame;
            this.Height = height;
            this.Width = width;

            this.PixelSize = this.Height * this.Width;
            this.FrameData = new byte[PixelSize];
            FrameData = Enumerable.Repeat<byte>(0xff, PixelSize).ToArray();
        }
        #endregion

        /// <summary>
        /// Fills this frame's <see cref="FrameData"/> byte array with the colors specifed in the <paramref name="stream"/> parameter.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> of 8bpp-indexed SPR data. The object must either have its pixel data beginning at [0] 
        /// or already have its <see cref="Stream.Position"/> set past any header, like the SPR's size, width and height.
        /// </param>
        /// <remarks>
        /// Note: <see cref="FrameData"/> is pre-filled with 0xff bytes. See the class constructors for details. 
        /// Simply put, since 0x00 is actually used for black, we need to use one of the palette entries that 
        /// signifies transparency. In pal_std.res, this is 0xf8-0xff; 0xff was chosen because it does not appear 
        /// to be used at all. 
        /// </remarks>
        public void SetPixels(Stream stream)
        {
            //todo:documentation: Verify 0xff is/isn't used and update explanation.

            int pixelsToSkip = 0;
            byte pixel;

            for (int y = 0; y < this.Height; ++y)
            {
                for (int x = 0; x < this.Width; ++x)
                {
                    if (pixelsToSkip != 0)  //only if we've previous identified transparent bits
                    {
                        if (pixelsToSkip >= this.Width - x) //greater than one line
                        {
                            pixelsToSkip -= (this.Width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;  //skip reading the indicated amount of bytes for transparent pictures
                        pixelsToSkip = 0;
                    }

                    try { pixel = Convert.ToByte(stream.ReadByte()); }
                    catch { return; /*got -1 for EOF*/ }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        this.FrameData[this.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixelsToSkip = stream.ReadByte() - 1;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for
        }
        public Bitmap BuildBitmap32bpp()
        {
            int idx;
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    idx = FrameData[y * this.Width + x];
                    Color pixel = this.Palette.Entries[idx];
                    
                    bmp.SetPixel(x, y, pixel);
                }
            }

            Color transparentByte = Color.FromArgb(0xff);
            bmp.MakeTransparent(transparentByte);
            this.ImageBmp = bmp;

            return this.ImageBmp;
        }

        /// <summary>
        /// Supplies an SPR-formatted version of this frame.
        /// </summary>
        /// <returns>Returns a byte array containing the frame data in 7KAA's 
        /// SPR format: int32 size, int16 width, int16 height, byte[] data.</returns>
        public byte[] BuildBitmap8bppIndexed()
        {
            byte palColorByte;                        
            byte transparentByte = 0xf8;
            int transparentByteCount = 0;
            int realOffset = 8; //since our array offset is unaware of the header
            byte[] indexedData = new byte[this.PixelSize + 4];

            // todo: will have to recalculate size if pixels change because the number of
            //       ommitted transparent bytes will have changed too
            byte[] width = BitConverter.GetBytes((short) this.Width);
            byte[] height = BitConverter.GetBytes((short) this.Height);

            /**************************************************************************
            *  BitConverter is required, rather than Convert.ToByte(), so we can 
            *  get the full 16- or 32-bit representations of the values. This is also  
            *  why Height and Width are both cast to short, to ensure we get a 16-bit
            *  representation of each value to match the binary's file format of:
            *  ____________________________________________________________ 
            *  | 4 byte Size | 2 byte Width | 2 byte Height | byte[] data |
            **************************************************************************/
            int seek_pos = 4; //first four bytes are for SprSize, at the end of the function
            Buffer.BlockCopy(width, 0, indexedData, seek_pos, width.Length);
            seek_pos += width.Length;
            Buffer.BlockCopy(height, 0, indexedData, seek_pos, height.Length);
            seek_pos += height.Length;

            List<Color> Palette = new List<Color>();
            foreach (Color c in this.Palette.Entries)
            {
                Palette.Add(c);
            }

            //This function may be called to save the current
            //image before making changes. So we need to build
            //a 32-bit BMP with current FrameData and then build
            //this SPR to return to the caller. This ensures the
            //caller has an original as a runtime backup for undo.
            if (this.ImageBmp == null)
                BuildBitmap32bpp();

            //the below is pretty much the same as GetPixel() but reversed(ish)
            for (int y = 0; y < this.ImageBmp.Height; ++y)
            {
                for (int x = 0; x < this.ImageBmp.Width; ++x)
                {
                    Color pixel = this.ImageBmp.GetPixel(x, y);
                    var pixARGB = pixel.ToArgb();
                    palColorByte = Convert.ToByte(Palette.FindIndex(c => c == Color.FromArgb(pixARGB)));

                    if (palColorByte > 0xf8) //0xf9 - 0xff are transparent
                        transparentByteCount++;

                    // Once we hit a non-zero pixel, we need to write out the transparent pixel marker
                    // and the count of transparent pixels. We then write out the current non-zero pixel.
                    // The second expression after || below is to check if we're on the last pixel of the
                    // image. If so, and the final pixels were colored, there won't be a next pixel to be 
                    // below 0xf8 so we need to write it out anyway.
                    bool lastByte = (x == (this.Width - 1) && (y == (this.Height - 1)));

                    if (palColorByte <= 0xf8 || lastByte)
                    { 
                        if (transparentByteCount > 0)  
                        {
                            // Write 0xf8[dd] where [dd] is transparent byte count, unless the
                            // number of transparent bytes is 6 or less, then just use the other
                            // codes below. Seems like the devs were pretty ruthless in trying to 
                            // save disk space back in 1997.
                            if (transparentByteCount > 7) 
                            { 
                                indexedData[realOffset] = transparentByte;
                                realOffset++;
                                indexedData[realOffset] = Convert.ToByte(transparentByteCount);
                                realOffset++;
                                transparentByteCount = 0;
                            }
                            else
                            {
                                //less than 8 and 7kaa cuts down on file size by just writing one byte      
                                //transparentByteCount = 2: 0xfe
                                //transparentByteCount = 3: 0xfd
                                //transparentByteCount = 4: 0xfc
                                //transparentByteCount = 5: 0xfb
                                //transparentByteCount = 6: 0xfa
                                //transparentByteCount = 7: 0xf9
                                indexedData[realOffset] = Convert.ToByte(0xff - (transparentByteCount - 1));
                                realOffset++;
                                transparentByteCount = 0;
                            }
                        }
                        
                        //there is no other byte to write out
                        if (!lastByte)
                        {
                            indexedData[realOffset] = palColorByte;
                            realOffset++;
                        }
                    }
                }//end inner for
            }//end outer for

            this.SprFrameRawDataSize = realOffset - 4;
            byte[] size = BitConverter.GetBytes(this.SprFrameRawDataSize);
            if (size.Length > 4) throw new Exception("SPR size must be Int32!");
            Buffer.BlockCopy(size, 0, indexedData, 0, size.Length);
            Array.Resize<byte>(ref indexedData, realOffset);

            return indexedData;
        }
    }
}
