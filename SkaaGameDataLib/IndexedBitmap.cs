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
using BitmapProcessing;

namespace SkaaGameDataLib
{
    [Serializable]
    public class IndexedBitmap
    {
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
        public bool PendingChanges;
        #endregion

        #region Constructors
        public IndexedBitmap(ColorPalette pal)
        {
            this.Bitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            this.Bitmap.Palette = pal;
        }
        #endregion
        
        public static byte[] GetRleBytesFromBitmap(Bitmap bmp)
        {
            //need it nullable for the try/catch block below since that's the only other assignment
            byte? palColorByte = null;
            byte transparentByte = 0xf8;
            int transparentByteCount = 0;
            int realOffset = 8; //since our array offset is unaware of the SPR header data
            byte[] indexedData = new byte[(bmp.Size.Height * bmp.Size.Width) + 4];

            // todo: will have to recalculate height/width if bitmap size changes
            byte[] width = BitConverter.GetBytes((short) bmp.Width);
            byte[] height = BitConverter.GetBytes((short) bmp.Height);

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
            foreach (Color c in bmp.Palette.Entries)
            {
                Palette.Add(c);
            }

            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    int pixARGB = pixel.ToArgb();
                    int idx = Palette.FindIndex(c => c == pixel);

                    if (idx == -1)
                    {
                        throw new Exception($"Unknown color: {pixARGB}");
                    }

                    //allows us to see the exception's message from the SaveProjectToDateTimeDirectory() Invoke in SkaaEditorMainForm.cs
                    try
                    {
                        palColorByte = Convert.ToByte(idx);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine($"Exception in FrameBmpToSpr: {e.Message}");
                    }

                    if (palColorByte > 0xf8) //0xf9 - 0xff are transparent
                        transparentByteCount++;
                    
                    // Once we hit a non-zero pixel, we need to write out the transparent pixel marker
                    // and the count of transparent pixels. We then write out the current non-zero pixel.
                    // The second expression after || below is to check if we're on the last pixel of the
                    // image. If so, and the final pixels were colored, there won't be a next pixel to be 
                    // below 0xf8 so we need to write it out anyway.
                    bool lastByte = (x == (bmp.Width - 1) && (y == (bmp.Height - 1)));

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
                            indexedData[realOffset] = (byte) palColorByte; //have to cast to non-nullable byte
                            realOffset++;
                        }
                    }
                }//end inner for
            }//end outer for

            //subtract four because the int32 size in the header is exclusive of those bytes used for the int32 size
            byte[] size = BitConverter.GetBytes(realOffset - 4);
            if (size.Length > 4)
            {
                string error = $"Bitmap's raw size must be Int32! Size for Bitmap at offset {realOffset} is {size.ToString()}";
                Trace.WriteLine(error);
                throw new Exception(error);
            }
            Buffer.BlockCopy(size, 0, indexedData, 0, size.Length);

            //Since FrameData is set to ((Width * Height) + 4), its length will
            //be based on the real pixels, not the "compressed" length with
            //the transparent pixels. This makes it impossible to calculate the
            //offsets of the next frames in the sprite to build a new game set.
            Array.Resize<byte>(ref indexedData, realOffset);
            //this.ResRawData = indexedData;
            return indexedData;
        }
        public static Bitmap GetBitmapFromRleBytes(byte[] bitmapBytes, ColorPalette pal, int height, int width)
        {
            int idx;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            bmp.Palette = pal;
            bmp.MakeTransparent(Color.Transparent);

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
            Color transparentByte = Color.FromArgb(0xff);
            bmp.MakeTransparent(transparentByte);

            return bmp;
        }

        public Bitmap GetBitmapFromRleStream(Stream str)
        {
            DecodeRleStream(str);
            return this.Bitmap;
        }
        /// <summary>
        /// Builds a <see cref="System.Drawing.Bitmap"/> from an byte array of indexed color values based on the provided palette
        /// </summary>
        /// <param name="bitmapBytes">Byte array containing the pixel data and no header</param>
        /// <param name="pal">The palette to assign to the new Bitmap</param>
        /// <param name="height">The height of the Bitmap, in pixels</param>
        /// <param name="width">The width of the Bitmap, in pixels</param>
        /// <returns>A new <see cref="System.Drawing.Bitmap"/> with a <see cref="PixelFormat"/> of <see cref="PixelFormat.Format8bppIndexed"/></returns>

        /// <summary>
        /// Reads a Run Length Encoded stream, where only transparent bytes are RLE, and builds a <see cref="System.Drawing.Bitmap"/>
        /// </summary>
        /// <param name="stream">A stream with its <see cref="Stream.Position"/> set to the first byte of the header, which is two int16 values for width and height.</param>
        private void DecodeRleStream(Stream stream)
        {
            //Read Header
            byte[] frame_size_bytes = new byte[4];
            stream.Read(frame_size_bytes, 0, 4);
            //int idxImgSize = BitConverter.ToInt32(frame_size_bytes, 0);
            int width = BitConverter.ToUInt16(frame_size_bytes, 0);
            int height = BitConverter.ToUInt16(frame_size_bytes, 2);
            int size = height * width;
            if (size > 6000000 || width > 2000 || height > 2000)
                throw new FormatException($"File is too large: {size} bytes, width: {width}, height: {height}");
            byte[] resBmpData = new byte[size];
            //byte[] resRawData = new byte[size];

            //initialize it to an unused transparent-pixel-marker
            resBmpData = Enumerable.Repeat<byte>(0xff, size).ToArray();
            //resRawData = Enumerable.Repeat<byte>(0xff, size).ToArray();
            //todo: Verify 0xff is/isn't used in any of the other sprites

            //todo: make sure this gets calculated and written out instead of saved statically here
            //Buffer.BlockCopy(frame_size_bytes, 0, this.ResRawData, 0, 8);

            int pixelsToSkip = 0;
            int bytesRead = 8; //start after the header info
            byte? pixel = null; //init to null or compiler complains about lack of assignment since assignment is in a try block

            //bool eof = false;

            for (int y = 0; y < height/* && eof == false*/; ++y)
            {
                for (int x = 0; x < width/* && eof == false*/; ++x)
                {
                    if (pixelsToSkip != 0)  //only if we've previously identified transparent bits
                    {
                        if (pixelsToSkip >= width - x) //greater than one line
                        {
                            pixelsToSkip -= (width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;  //skip reading the indicated amount of bytes for transparent pictures
                        pixelsToSkip = 0;
                    }

                    //try
                    //{
                        pixel = (byte)stream.ReadByte();
                        //resRawData[bytesRead] = (byte)pixel;
                        bytesRead++;
                    if (bytesRead > stream.Length)
                        throw new FormatException("File is not in the proper format!");
                    //}
                    //catch
                    //{
                    //    //got -1 for EOF
                    //    //byte[] resize = resRawData;
                    //    //Array.Resize<byte>(ref resize, bytesRead);
                    //    //resRawData = resize;
                    //    eof = true;
                    //    break;
                    //}

                    Debug.Assert(pixel != null, "pixel was unassigned!");

                    if (pixel < 0xf8) //MIN_TRANSPARENT_CODE (normal pixel)
                    {
                        resBmpData[width * y + x] = (byte)pixel;
                    }
                    else if (pixel == 0xf8) //MANY_TRANSPARENT_CODE
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        pixelsToSkip = (byte)pixel - 1;
                        //resRawData[bytesRead] = (byte)pixel;
                        bytesRead++;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - (byte)pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for

            //byte[] resizeMe = resRawData;
            //Array.Resize<byte>(ref resizeMe, bytesRead);
            //resRawData = resizeMe;

            this.Bitmap = GetBitmapFromRleBytes(resBmpData, this.Bitmap.Palette, height, width);
            //return resBmpData;//IndexedBitmap.ByteArrayToBitmap(resBmpData, this.Palette, this.Height, this.Width);
        }
    }
}
