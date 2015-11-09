using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitmapProcessing;

namespace SkaaGameDataLib
{
    public static class SprDataHandlers
    {
        public static Bitmap FrameSprToBmp(SpriteFrame sf, ColorPalette pal)
        {
            int idx;
            Bitmap bmp = new Bitmap(sf.Width, sf.Height);
            FastBitmap fbmp = new FastBitmap(bmp);
            fbmp.LockImage();

            for (int y = 0; y < sf.Height; y++)
            {
                for (int x = 0; x < sf.Width; x++)
                {
                    idx = sf.FrameBmpData[y * sf.Width + x];
                    Color pixel = pal.Entries[idx];
                    fbmp.SetPixel(x, y, pixel);
                }
            }
            fbmp.UnlockImage();
            Color transparentByte = Color.FromArgb(0xff);
            bmp.MakeTransparent(transparentByte);
            return bmp;
            //return this.ImageBmp;
        }
        public static byte[] FrameBmpToSpr(SpriteFrame sf, ColorPalette pal)
        {
            byte palColorByte;
            byte transparentByte = 0xf8;
            int transparentByteCount = 0;
            int realOffset = 8; //since our array offset is unaware of the header
            byte[] indexedData = new byte[sf.PixelSize + 4];

            // todo: will have to recalculate height/width if bitmap size changes
            byte[] width = BitConverter.GetBytes((short) sf.Width);
            byte[] height = BitConverter.GetBytes((short) sf.Height);

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
            foreach (Color c in pal.Entries)
            {
                Palette.Add(c);
            }

            ////BuildBitmap8bppIndexed() may be called to save the 
            ////current image before making changes. So we build  
            ////a 32-bit BMP with current FrameData so it can be used 
            ////below and to build this SPR to return to the caller. 
            //if (sf.ImageBmp == null)
            //    FrameSprToBmp(sf);

            for (int y = 0; y < sf.ImageBmp.Height; ++y)
            {
                for (int x = 0; x < sf.ImageBmp.Width; ++x)
                {
                    Color pixel = sf.ImageBmp.GetPixel(x, y);
                    var pixARGB = pixel.ToArgb();
                    palColorByte = Convert.ToByte(Palette.FindIndex(c => c == Color.FromArgb(pixARGB)));

                    if (palColorByte > 0xf8) //0xf9 - 0xff are transparent
                        transparentByteCount++;

                    // Once we hit a non-zero pixel, we need to write out the transparent pixel marker
                    // and the count of transparent pixels. We then write out the current non-zero pixel.
                    // The second expression after || below is to check if we're on the last pixel of the
                    // image. If so, and the final pixels were colored, there won't be a next pixel to be 
                    // below 0xf8 so we need to write it out anyway.
                    bool lastByte = (x == (sf.Width - 1) && (y == (sf.Height - 1)));

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

            byte[] size = BitConverter.GetBytes(realOffset - 4); //subtract four because int32 size does not count the int32
            if (size.Length > 4) throw new Exception("SPR size must be Int32!");
            Buffer.BlockCopy(size, 0, indexedData, 0, size.Length);

            //Since FrameData is set to ((Width * Height) + 4), its length will
            //be based on the real pixels, not the "compressed" length with
            //the transparent pixels. This makes it impossible to calculate the
            //offsets of the next frames in the sprite to build a new game set.
            Array.Resize<byte>(ref indexedData, realOffset);
            //sf.FrameRawData = indexedData;

            return indexedData;
            //return this.FrameRawData;
            //return indexedData;
        }
        public static void SprStreamToSpriteFrame(SpriteFrame sf, Stream stream)
        {
            //todo: Verify 0xff is/isn't used in any sprite and update explanation.

            sf.SprBitmapOffset = (int) stream.Position;
            //Read Header
            byte[] frame_size_bytes = new byte[8];
            stream.Read(frame_size_bytes, 0, 8);
            int sprSize = BitConverter.ToInt32(frame_size_bytes, 0);
            sf.Width = BitConverter.ToInt16(frame_size_bytes, 4);
            sf.Height = BitConverter.ToInt16(frame_size_bytes, 6);
            sf.PixelSize = sf.Height * sf.Width;
            sf.FrameBmpData = new byte[sf.PixelSize];
            sf.FrameRawData = new byte[sf.PixelSize];
            sf.FrameBmpData = Enumerable.Repeat<byte>(0xff, sf.PixelSize).ToArray();
            sf.FrameRawData = Enumerable.Repeat<byte>(0xff, sf.PixelSize).ToArray();

            int pixelsToSkip = 0;
            int bytesRead = 8; //start after the header info
            byte pixel;

            for (int y = 0; y < sf.Height; ++y)
            {
                for (int x = 0; x < sf.Width; ++x)
                {
                    if (pixelsToSkip != 0)  //only if we've previously identified transparent bits
                    {
                        if (pixelsToSkip >= sf.Width - x) //greater than one line
                        {
                            pixelsToSkip -= (sf.Width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;  //skip reading the indicated amount of bytes for transparent pictures
                        pixelsToSkip = 0;
                    }

                    try
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        sf.FrameRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    catch
                    {
                        //got -1 for EOF
                        byte[] resize = sf.FrameRawData;
                        Array.Resize<byte>(ref resize, bytesRead);
                        return;
                    }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        sf.FrameBmpData[sf.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        pixelsToSkip = pixel - 1;
                        sf.FrameRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for

            byte[] resizeMe = sf.FrameRawData;
            Array.Resize<byte>(ref resizeMe, bytesRead);
        }
        public static Bitmap SpriteToBmp(Sprite spr)
        {
            int totalFrames = spr.Frames.Count;
            int spriteWidth = 0, spriteHeight = 0, high = 0, low = 0;

            double sqrt = Math.Sqrt((double) totalFrames);

            if (totalFrames % 1 != 0) //totalFrames is a perfect square
            {
                low = (int) sqrt;
                high = (int) sqrt;
            }
            else
            {
                low = (int) Math.Floor(sqrt) + 1; //adds an additional row
                high = (int) Math.Ceiling(sqrt);
            }

            //need the largest height and width to tile the export
            foreach (SpriteFrame sf in spr.Frames)
            {
                if (sf.Width > spriteWidth)
                    spriteWidth = sf.Width;
                if (sf.Height > spriteHeight)
                    spriteHeight = sf.Height;
            }

            //calculated height and width of the bitmap
            //based on tiles of the largest possible size
            int exportWidth = high * spriteWidth,
                exportHeight = low * spriteHeight;

            using (Bitmap bitmap = new Bitmap(exportWidth, exportHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    int frameIndex = 0;

                    for (int y = 0; y < exportHeight; y += spriteHeight)
                    {
                        //once we hit the max frames, just break
                        for (int x = 0; x < exportWidth && frameIndex < spr.Frames.Count; x += spriteWidth)
                        {
                            g.DrawImage(spr.Frames[frameIndex].ImageBmp, new Point(x, y));
                            frameIndex++;
                        }
                    }
                }

                return bitmap;
            }
        }
    }
}
