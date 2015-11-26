using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public class IndexedBitmap
    {
        #region Event Handlers
        [field: NonSerialized]
        private EventHandler _imageUpdated;
        public event EventHandler ImageUpdated
        {
            add
            {
                if (_imageUpdated == null || !_imageUpdated.GetInvocationList().Contains(value))
                {
                    _imageUpdated += value;
                }
            }
            remove
            {
                _imageUpdated -= value;
            }
        }
        protected virtual void OnImageUpdated(EventArgs e)
        {
            EventHandler handler = _imageUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Private Members
        private int _pixelSize, _height, _width;
        private byte[] _frameBmpData, _frameRawData;
        private ColorPalette _palette;
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
                if (this._height != value)
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
        public byte[] ResBmpData
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
        public byte[] ResRawData
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
        
        public ColorPalette Palette
        {
            get
            {
                return _palette;
            }

            set
            {
                if(this._palette != value)
                    this._palette = value;
            }
        }
        public Bitmap ImageBmp;
        public bool PendingChanges;
        #endregion

        #region Constructors
        public IndexedBitmap() { }
        #endregion

        /// <summary>
        /// Saves the new 32-bit BMP and generates new SPR data based on the edited <see cref="Bitmap"/>
        /// </summary>
        /// <param name="bmp">The <see cref="Bitmap"/> object from which to get updates.</param>
        internal void ProcessUpdates(Bitmap bmp)
        {
            if (this.PendingChanges == true)
            {
                this.ImageBmp = bmp;
                UpdateResRawData();// this, this.Palette);
                this.PendingChanges = false;

                OnImageUpdated(EventArgs.Empty);  //this doesn't get triggered by FrameBmpToSpr()
            }
        }
        
        public void UpdateResRawData()//IndexedBitmap bmp, ColorPalette pal)
        {
            //need it nullable for the try/catch block below since that's the only other assignment
            byte? palColorByte = null;
            byte transparentByte = 0xf8;
            int transparentByteCount = 0;
            int realOffset = 8; //since our array offset is unaware of the SPR header data
            byte[] indexedData = new byte[this.PixelSize + 4];

            // todo: will have to recalculate height/width if bitmap size changes
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

            ////BuildBitmap8bppIndexed() may be called to save the 
            ////current image before making changes. So we build  
            ////a 32-bit BMP with current FrameData so it can be used 
            ////below and to build this SPR to return to the caller. 
            //if (sf.ImageBmp == null)
            //    FrameSprToBmp(sf);

            for (int y = 0; y < this.ImageBmp.Height; ++y)
            {
                for (int x = 0; x < this.ImageBmp.Width; ++x)
                {
                    Color pixel = this.ImageBmp.GetPixel(x, y);

                    int pixARGB = pixel.ToArgb();
                    //Color fromArgb = Color.FromArgb(pixARGB);
                    int idx = Palette.FindIndex(c => c == pixel);

                    //foreach (Color c in Palette)
                    //{
                    //    Debug.WriteLine($"Color c = {c.ToString()}");
                    //}
                    //Debug.WriteLine($"pixel = {pixel.ToString()} pixARGB = {pixARGB} ({pixARGB.ToString()}) | idx = {idx.ToString()}");

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
            this.ResRawData = indexedData;
        }
    }
}
