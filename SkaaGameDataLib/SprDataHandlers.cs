using System;
using System.Collections.Generic;
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
    public static class SprDataHandlers
    {
        public static Bitmap FrameSprToBmp(SpriteFrameResource sf, ColorPalette pal)
        {
            int idx;
            Bitmap bmp = new Bitmap(sf.Width, sf.Height);
            FastBitmap fbmp = new FastBitmap(bmp);
            fbmp.LockImage();

            for (int y = 0; y < sf.Height; y++)
            {
                for (int x = 0; x < sf.Width; x++)
                {
                    idx = sf.ResBmpData[y * sf.Width + x];
                    Color pixel = pal.Entries[idx];
                    fbmp.SetPixel(x, y, pixel);
                }
            }
            fbmp.UnlockImage();
            Color transparentByte = Color.FromArgb(0xff);
            bmp.MakeTransparent(transparentByte);
            return bmp;
        }
        
        public static void SprStreamToSpriteFrame(SpriteFrameResource sf, Stream stream)
        {
            sf.SprBitmapOffset = (int) stream.Position;
            //Read Header
            byte[] frame_size_bytes = new byte[8];
            stream.Read(frame_size_bytes, 0, 8);
            int sprSize = BitConverter.ToInt32(frame_size_bytes, 0);
            sf.Width = BitConverter.ToInt16(frame_size_bytes, 4);
            sf.Height = BitConverter.ToInt16(frame_size_bytes, 6);
            sf.PixelSize = sf.Height * sf.Width;
            sf.ResBmpData = new byte[sf.PixelSize];
            sf.ResRawData = new byte[sf.PixelSize];

            //initialize it to an unused transparent-pixel-marker
            //todo: Verify 0xff is/isn't used in any of the other sprites
            sf.ResBmpData = Enumerable.Repeat<byte>(0xff, sf.PixelSize).ToArray();
            sf.ResRawData = Enumerable.Repeat<byte>(0xff, sf.PixelSize).ToArray();

            Buffer.BlockCopy(frame_size_bytes, 0, sf.ResRawData, 0, 8);

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
                        sf.ResRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    catch
                    {
                        //got -1 for EOF
                        byte[] resize = sf.ResRawData;
                        Array.Resize<byte>(ref resize, bytesRead);
                        sf.ResRawData = resize;
                        return;
                    }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        sf.ResBmpData[sf.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        pixelsToSkip = pixel - 1;
                        sf.ResRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for

            byte[] resizeMe = sf.ResRawData;
            Array.Resize<byte>(ref resizeMe, bytesRead);
            sf.ResRawData = resizeMe;
        }

        public static void ResStreamToSpriteFrame(SpriteFrameResource res, Stream stream)
        {
            //todo: read number of records before getting here
            //todo: set object name
            
            //Read Header
            byte[] frame_size_bytes = new byte[8];
            stream.Read(frame_size_bytes, 0, 8);
            int sprSize = BitConverter.ToInt32(frame_size_bytes, 0);
            res.Width = BitConverter.ToInt16(frame_size_bytes, 4);
            res.Height = BitConverter.ToInt16(frame_size_bytes, 6);
            res.PixelSize = res.Height * res.Width;
            res.ResBmpData = new byte[res.PixelSize];
            res.ResRawData = new byte[res.PixelSize];

            string resourceName;
            byte[] resoure_name = new byte[8];
            stream.Read(resoure_name, 0, 8);
            resourceName = Encoding.GetEncoding(1252).GetString(resoure_name, 0, 8).TrimEnd('\0');

            byte[] resource_offset = new byte[8];
            stream.Read(resource_offset, 0, 8); //need to read 8 else the byte array returned is too short
            stream.Position -= 3; //backup due to above. the offsets are only 5 bytes
            res.SprBitmapOffset = (int) BitConverter.ToUInt64(resource_offset, 0);


            //int sprSize = BitConverter.ToInt32(frame_size_bytes, 0);
            //sf.Width = BitConverter.ToInt16(frame_size_bytes, 4);
            //sf.Height = BitConverter.ToInt16(frame_size_bytes, 6);
            res.PixelSize = res.Height * res.Width;
            res.ResBmpData = new byte[res.PixelSize];
            res.ResRawData = new byte[res.PixelSize];


            //initialize it to an unused transparent-pixel-marker
            //todo: Verify 0xff is/isn't used in any of the other sprites
            res.ResBmpData = Enumerable.Repeat<byte>(0xff, res.PixelSize).ToArray();
            res.ResRawData = Enumerable.Repeat<byte>(0xff, res.PixelSize).ToArray();

            //Buffer.BlockCopy(frame_size_bytes, 0, sf.FrameRawData, 0, 8);

            int pixelsToSkip = 0;
            int bytesRead = 8; //start after the header info
            byte pixel;

            for (int y = 0; y < res.Height; ++y)
            {
                for (int x = 0; x < res.Width; ++x)
                {
                    if (pixelsToSkip != 0)  //only if we've previously identified transparent bits
                    {
                        if (pixelsToSkip >= res.Width - x) //greater than one line
                        {
                            pixelsToSkip -= (res.Width - x); // skip to next line
                            break;
                        }

                        x += pixelsToSkip;  //skip reading the indicated amount of bytes for transparent pictures
                        pixelsToSkip = 0;
                    }

                    try
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        res.ResRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    catch
                    {
                        //got -1 for EOF
                        byte[] resize = res.ResRawData;
                        Array.Resize<byte>(ref resize, bytesRead);
                        res.ResRawData = resize;
                        return;
                    }

                    if (pixel < 0xf8)//MIN_TRANSPARENT_CODE) //normal pixel
                    {
                        res.ResBmpData[res.Width * y + x] = pixel;
                    }
                    else if (pixel == 0xf8)//MANY_TRANSPARENT_CODE)
                    {
                        pixel = Convert.ToByte(stream.ReadByte());
                        pixelsToSkip = pixel - 1;
                        res.ResRawData[bytesRead] = pixel;
                        bytesRead++;
                    }
                    else //f9,fa,fb,fc,fd,fe,ff
                    {
                        pixelsToSkip = 256 - pixel - 1;	// skip (neg al) pixels
                    }
                }//end inner for
            }//end outer for

            byte[] resizeMe = res.ResRawData;
            Array.Resize<byte>(ref resizeMe, bytesRead);
            res.ResRawData = resizeMe;
        }
        /// <summary>
        /// Builds a <see cref="Bitmap"/> sprite sheet containing all the frames of the specified <see cref="Sprite"/>
        /// with no padding between frames. The number of rows/columns of frames is the square root of the number of frames
        /// with an additional row added when the number of frames is not a perfect square.
        /// </summary>
        /// <param name="spr">The <see cref="Sprite"/> for which to generate a <see cref="Bitmap"/></param>
        /// <returns>The newly-generated <see cref="Bitmap"/></returns>
        public static Bitmap SpriteToBmp(Sprite spr)
        {
            int totalFrames = spr.Frames.Count;
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
            foreach (SpriteFrameResource sf in spr.Frames)
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
