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

namespace SkaaGameDataLib
{
    public class SpriteFrame
    {
        public int Size
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public Byte[] FrameData
        {
            get;
            set;
        }
        public Bitmap ImageBmp
        {
            get;
            set;
        }
        public ColorPalette Palette
        {
            get;
            set;
        }
        
        public SpriteFrame()
        {

        }
        public SpriteFrame(int size, int width, int height, ColorPalette palette)
        {
            this.Size = size;
            this.Height = height;
            this.Width = width;
            this.FrameData = new Byte[height * width];
            FrameData = Enumerable.Repeat<byte>(0xff, FrameData.Length).ToArray<byte>();
            this.Palette = palette;
        }

        public void GetPixels(FileStream stream)
        {
            //todo: find the best way to signify transparent pixels
            //since 0x00 is actually used for black, we need to use one of the 
            //unused palette entries 0xf8-0xff

            int pixelsToSkip = 0;
            Byte pixel;

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
        }//end GetPixels()
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
                    //bmp.SetPixel(x, y, Color.FromArgb(255, pixel));
                }
            }

            Color transparentByte = Color.FromArgb(0xff);
            bmp.MakeTransparent(transparentByte);
            this.ImageBmp = bmp;

            return this.ImageBmp;
        }
        public Byte[] BuildBitmap8bppIndexed()//Bitmap bmp32bppToConvert, ColorPalette indexedPallet)
        {
            //todo: verify all changes have been saved

            Byte palColorByte;                        
            byte transparentByte = 0xf8;
            int transparentByteCount = 0;
            int realOffset = 8;
            Byte[] indexedData = new Byte[this.Size + 4];

            // todo: will have to recalculate size if pixels change because the number of
            //       ommitted transparent bytes will have changed too
            Byte[] size = BitConverter.GetBytes(this.Size);
            Byte[] width = BitConverter.GetBytes((short) this.Width);
            Byte[] height = BitConverter.GetBytes((short) this.Height);

            Buffer.BlockCopy(size, 0, indexedData, 0, Buffer.ByteLength(size));
            Buffer.BlockCopy(width, 0, indexedData, 0 + Buffer.ByteLength(size), Buffer.ByteLength(width));
            Buffer.BlockCopy(height, 0, indexedData, 0 + Buffer.ByteLength(size) + Buffer.ByteLength(width), Buffer.ByteLength(width));

            List<Color> Palette = new List<Color>();
            foreach (Color c in this.Palette.Entries)
            {
                Palette.Add(c);
            }

            //the below is pretty much the same as GetPixel() but reversed(ish)
            for (int y = 0; y < ImageBmp.Height; ++y)
            {
                for (int x = 0; x < this.Width; ++x)
                {
                    Color pixel = ImageBmp.GetPixel(x, y);
                    var pixARGB = pixel.ToArgb();

                    palColorByte = Convert.ToByte(Palette.FindIndex(c => c == Color.FromArgb(pixARGB)));

                    if (palColorByte > 0xf8) //0xf9 - 0xff are transparent
                    {
                        transparentByteCount++;
                    }

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

                                //there is no other byte to write out
                                if (!lastByte)
                                {
                                    indexedData[realOffset] = palColorByte;
                                    realOffset++;
                                }
                                //indexedData[realOffset] = palColorByte;
                                //realOffset++;
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

                                //there is no other byte to write out
                                if (!lastByte)
                                {
                                    indexedData[realOffset] = palColorByte;
                                    realOffset++;
                                }

                                transparentByteCount = 0;
                            }
                        }
                        else
                        {
                            indexedData[realOffset] = palColorByte;
                            realOffset++;
                        }
                        //if (!lastByte)
                        //{
                        //    indexedData[realOffset] = palColorByte;
                        //    realOffset++;
                        //}
                    }
                }//end inner for
            }//end outer for

            return indexedData;
        }
    }
}
