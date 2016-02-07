#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BitmapProcessing;

namespace SkaaGameDataLib.Util
{
    [Serializable]
    public class IndexedBitmap
    {
        //todo: Inherit from Image and/or implement the methods as Extension Methods to Bitmap
        public static readonly TraceSource Logger = new TraceSource("IndexedBitmap", SourceLevels.All);

        #region Private Static Fields
        private static readonly int MaxByteSize = 6000000;
        private static readonly int MaxPixelHeight = 2000;
        private static readonly int MaxPixelWidth = 2000;
        /// <summary>
        /// Used in <see cref="Bitmap.MakeTransparent(Color)"/>. "White transparent" is used in 7KAA's 
        /// sprites and the opacity is set to 0% as needed.
        /// </summary>
        private static readonly Color _skaaTransparentColor = Color.FromArgb(0, 255, 255, 255);
        #endregion

        #region Event Handlers
        [NonSerialized]
        private EventHandler _paletteChanged;
        public event EventHandler PaletteChanged
        {
            add
            {
                if (_paletteChanged == null || !_paletteChanged.GetInvocationList().Contains(value))
                {
                    _paletteChanged += value;
                }
            }
            remove
            {
                _paletteChanged -= value;
            }
        }
        protected virtual void OnPaletteChanged(EventArgs e)
        {
            EventHandler handler = _paletteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Public Members
        public Bitmap Bitmap;
        #endregion

        #region Constructors
        public IndexedBitmap(Bitmap bmp)
        {
            this.Bitmap = bmp;
        }
        public IndexedBitmap(ColorPalette pal)
        {
            this.Bitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            this.Bitmap.Palette = pal;
        }
        #endregion


        public static byte[] GetRleBytesFromBitmap(Bitmap bmp)
        {
            int transparentByteCount = 0;
            int realOffset = 8; //since our array offset is unaware of the frame header data (4 byte size + 2 bytes each for height & width)
            byte[] indexedData = new byte[(bmp.Size.Height * bmp.Size.Width) + 8]; //frame data + frame header
            bool transparentByteFound = false;

            byte[] width = BitConverter.GetBytes((short)bmp.Width);
            byte[] height = BitConverter.GetBytes((short)bmp.Height);

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

            if (bmp.Palette.Entries.Count() < 1)
            {
                string msg = "Bitmap palette has no entries!";
                Logger.TraceEvent(TraceEventType.Error, 0, msg);
                Debugger.Break();
                throw new Exception(msg);
            }

            //todo:performance:cache this list
            List<Color> Palette = bmp.Palette.Entries.ToList();

            //This provides a significant speed increase for large images
            //due to the GetPixel() call in the loop below
            FastBitmap fbmp = new FastBitmap(bmp);
            fbmp.LockImage();

            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    Color pixel = fbmp.GetPixel(x, y);
                    int pixARGB = pixel.ToArgb();

                    if (pixARGB == 0x00 && transparentByteFound)
                    {
                        transparentByteCount++;
                        continue;
                    }

                    if (pixARGB == 0x00)
                    {
                        transparentByteFound = true;
                        transparentByteCount++;
                        continue;
                    }

                    //we found a normal pixel after a bunch of transparent ones
                    if (pixARGB != 0x00 && transparentByteFound && transparentByteCount > 0)
                    {
                        while (transparentByteCount >= 255)
                        {
                            indexedData[realOffset] = 0xf8;
                            realOffset++;
                            indexedData[realOffset] = 255;
                            realOffset++;
                            transparentByteCount -= 255;
                        }

                        if (transparentByteCount <= 7)
                            indexedData[realOffset] = (byte)(256 - transparentByteCount);
                        else
                        {
                            indexedData[realOffset] = 0xf8;
                            realOffset++;
                            indexedData[realOffset] = (byte)transparentByteCount;
                        }

                        //reset our count
                        transparentByteCount = 0;
                        transparentByteFound = false;
                        //go forward one for the transparentByteCount or 0xf9-0xff pixel
                        realOffset++;
                    }

                    int idx = Palette.FindIndex(c => c == pixel);
                    byte palColorByte;

                    if (idx == -1)
                    {
                        string rgb = pixARGB.ToString("X");
                        Logger.TraceEvent(TraceEventType.Error, 0, $"Unknown color: {rgb}");
                        Debugger.Break();
                        //default to black
                        idx = Palette.FindIndex(c => c == Color.Black);
                        //but if that's missing too, just take whatever color is first
                        if (idx == -1)
                            idx = 0;
                    }

                    //allows us to see the exception's message
                    try
                    {
                        palColorByte = Convert.ToByte(idx);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine($"Exception in FrameBmpToSpr: {e.Message}");
                        throw e;
                    }

                    //write the normal pixel
                    indexedData[realOffset] = palColorByte;
                    realOffset++;

                }//end inner for
            }//end outer for

            fbmp.UnlockImage();

            //the two continue statements above would cause us to miss any
            //transparent bytes at the very end (bottom-right of image)
            if (transparentByteFound && transparentByteCount > 0)
            {
                while (transparentByteCount >= 255)
                {
                    indexedData[realOffset] = 0xf8;
                    realOffset++;
                    indexedData[realOffset] = 255;
                    realOffset++;
                    transparentByteCount -= 255;
                }

                if (transparentByteCount <= 7)
                    indexedData[realOffset] = (byte)(256 - transparentByteCount);
                else
                {
                    indexedData[realOffset] = 0xf8;
                    realOffset++;
                    indexedData[realOffset] = (byte)transparentByteCount;
                }

                //reset our count
                transparentByteCount = 0;
                transparentByteFound = false;
                //go forward one for the transparentByteCount or 0xf9-0xff pixel
                realOffset++;
            }
            //subtract four because the int32 size in the header is exclusive of those bytes used for the int32 size
            byte[] size = BitConverter.GetBytes(realOffset - 4);
            if (size.Length > 4)
            {
                string error = $"Bitmap's raw size must be <= Int32.MaxValue! Size for Bitmap at offset {realOffset} is {size.ToString()}";
                Trace.WriteLine(error);
                throw new Exception(error);
            }
            Buffer.BlockCopy(size, 0, indexedData, 0, size.Length);

            //Since FrameData is set to ((Width * Height) + 4), its length will
            //be based on the real pixels, not the "compressed" length with the 
            //transparent pixels. Getting this right makes it impossible to calculate 
            //the offsets of the next frames in the sprite to build a new game set.
            Array.Resize<byte>(ref indexedData, realOffset);
            return indexedData;
        }
        /// <summary>
        /// Builds a <see cref="System.Drawing.Bitmap"/> from an byte array of indexed color values based on the provided palette.
        /// Algorithm based on IMGbltAreaTransDecompress in IB_ATD.cpp.
        /// </summary>
        /// <param name="bitmapBytes">Byte array containing the pixel data and no header</param>
        /// <param name="pal">The palette to assign to the new Bitmap</param>
        /// <param name="height">The height of the Bitmap, in pixels</param>
        /// <param name="width">The width of the Bitmap, in pixels</param>
        /// <returns>A new <see cref="System.Drawing.Bitmap"/> with a <see cref="PixelFormat"/> of <see cref="PixelFormat.Format8bppIndexed"/></returns>
        public static Bitmap GetBitmapFromRleBytes(byte[] bitmapBytes, ColorPalette pal, int height, int width)
        {
            //~28ms on large images in i_menu.res

            int idx;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            bmp.MakeTransparent(_skaaTransparentColor);

            //much faster GetPixel/SetPixel usage
            FastBitmap fbmp = new FastBitmap(bmp);
            fbmp.LockImage();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    idx = bitmapBytes[y * width + x];
                    Color pixel = pal.Entries[idx];
                    fbmp.SetPixel(x, y, pixel);
                }
            }

            fbmp.UnlockImage();
            bmp.Palette = pal;
            return bmp;
        }

        public Bitmap SetBitmapFromRleStream(Stream str, FileFormats form)
        {
            this.Bitmap = DecodeRleStream(str, form);
            return this.Bitmap;
        }

        /// <summary>
        /// Reads a Run Length Encoded stream, where only transparent bytes are RLE, and builds a <see cref="System.Drawing.Bitmap"/>
        /// </summary>
        /// <param name="stream">A stream with its <see cref="Stream.Position"/> set to the first byte of the header, which is two int16 values for width and height.</param>
        private Bitmap DecodeRleStream(Stream stream, FileFormats form)
        {
            //~600ms on large images in i_menu.res

            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] frame_size_bytes;
            int width,     //width in pixels, as read from the stream
                height,    //height in pixels, as read from the stream
                calcSize;  //number of pixels: height * width

            if (form == FileFormats.SpriteSpr)
            {
                frame_size_bytes = new byte[8];
                stream.Read(frame_size_bytes, 0, 8);
                width = BitConverter.ToUInt16(frame_size_bytes, 4);
                height = BitConverter.ToUInt16(frame_size_bytes, 6);
                calcSize = height * width;
            }
            else if (form == FileFormats.ResIdxFramesSpr)
            {
                frame_size_bytes = new byte[4];
                stream.Read(frame_size_bytes, 0, 4);
                width = BitConverter.ToUInt16(frame_size_bytes, 0);
                height = BitConverter.ToUInt16(frame_size_bytes, 2);
                calcSize = height * width;
            }
            else
                return null;

            //The max values here are arbitrary numbers that seem like reasonable cutoffs for 7KAA's purposes. 
            //The purpose is to ensure this function is the only one that bothers to attempt to read the
            //stream's header. The game runs at 640x480 so there are no high-res images with a height
            //and width so large. The size is larger to allow for the few huge sprites with tons of frames.
            //Any file failing these tests are assumed to not be FileFormat.SpriteSpr.
            if (calcSize < 1 ||
                calcSize > MaxByteSize ||
                width > MaxPixelWidth ||
                height > MaxPixelHeight)
                return null;

            byte[] resBmpData = new byte[calcSize];

            //initialize it to an unused transparent-pixel-marker
            resBmpData = Enumerable.Repeat<byte>(0xff, calcSize).ToArray();

            int pixelsToSkip = 0;
            int bytesRead = 8; //start after the header info
            byte pixel;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (pixelsToSkip != 0)  //only if we found transparent bits on the last iteration and haven't parsed them all yet
                    {
                        if (pixelsToSkip >= width - x) //greater than one line
                        {
                            pixelsToSkip -= (width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;  //skip reading the indicated amount of bytes for transparent pictures
                        pixelsToSkip = 0;
                    }

                    if (stream.Position + 1 > stream.Length)
                        return null;

                    pixel = (byte)stream.ReadByte();
                    bytesRead++;

                    if (pixel < 0xf8) //normal pixel
                    {
                        resBmpData[width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)
                    {
                        if (stream.Position + 1 > stream.Length)
                            return null;
                        pixel = (byte)stream.ReadByte();
                        pixelsToSkip = (byte)pixel - 1;
                        bytesRead++;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 255 - pixel;	// skip this many pixels since they're pre-initialized to transparent
                    }
                }//end inner for
            }//end outer for

            sw.Stop();

            return GetBitmapFromRleBytes(resBmpData, this.Bitmap.Palette, height, width);
        }
    }
}
