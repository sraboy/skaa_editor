﻿#region Copyright Notice
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SkaaGameDataLib
{
    [Serializable]
    public class SkaaSprite
    {
        public static readonly TraceSource Logger = new TraceSource("Sprite", SourceLevels.All);

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
        private List<SkaaFrame> _frames;
        private DataView _spriteDataView;
        private string _spriteId;
        #endregion

        #region Public Properties
        public List<SkaaFrame> Frames
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
        public DataView DataView
        {
            get
            {
                return this._spriteDataView;
            }
            internal set
            {
                if (this._spriteDataView != value)
                {
                    this._spriteDataView = value;
                }
            }
        }
        #endregion

        #region Constructors
        public SkaaSprite()
        {
            this.Frames = new List<SkaaFrame>();
        }
        #endregion

        public bool SetSpriteDataView(DataView dv)
        {
            if (dv != null)
            {
                this.DataView = dv;
                return this.MatchFrameOffsets();
            }
            else
            {
                Trace.WriteLine($"No DataView assigned to {this.SpriteId}");
                return false;
            }
        }
        public List<Image> GetFrameImages()
        {
           List<Image> frames = this.Frames.Select(x => x.IndexedBitmap.Bitmap).ToList<Image>();
            return frames;
        }
        /// <summary>
        /// Iterates through all the rows in the <see cref="SkaaSprite"/>'s <see cref="DataView"/> and 
        /// sets each of this sprite's <see cref="SkaaSpriteFrame"/>'s <see cref="SkaaSpriteFrame.GameSetDataRows"/>
        /// property to the DataRow with a BITMAPPTR matching <see cref="SkaaSpriteFrame.BitmapOffset"/>. It also
        /// reads the FILENAME property into <see cref="SkaaFrame.Name"/>.
        /// </summary>
        /// <returns>False if any frame did not have a match in the DataView. True otherwise.</returns>
        internal bool MatchFrameOffsets()
        {
            foreach (DataRowView drv in this.DataView)
            {
                int offset = Convert.ToInt32(drv.Row.ItemArray[9]);
                string name = (string) drv.Row.ItemArray[8];
                SkaaFrame sf = this.Frames.Find(f => f.BitmapOffset == offset);
                sf.Name = name;

                if (sf == null)
                {
                    //this should only happen when creating new sprites. 
                    Trace.WriteLine(($"Unable to find matching offset in Sprite.Frames for {this.SpriteId} and offset: {offset.ToString()}. nDid you forget to load the proper SET file for this sprite?"));
                    return false;
                }

                if (sf != null)
                    (sf as SkaaSpriteFrame).GameSetDataRows.Add(drv.Row);
            }

            return true;
        }
        /// <summary>
        /// Builds a <see cref="Bitmap"/> sprite sheet containing all the frames of the specified <see cref="SkaaSprite"/>
        /// with no padding between frames. The number of rows/columns of frames is the square root of the number of frames
        /// with an additional row added when the number of frames is not a perfect square.
        /// </summary>
        /// <returns>The newly-generated <see cref="Bitmap"/></returns>
        public Bitmap ToBitmap()
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
            foreach (SkaaSpriteFrame sf in this.Frames)
            {
                if (sf.IndexedBitmap.Bitmap.Width > spriteWidth)
                    spriteWidth = sf.IndexedBitmap.Bitmap.Width;
                if (sf.IndexedBitmap.Bitmap.Height > spriteHeight)
                    spriteHeight = sf.IndexedBitmap.Bitmap.Height;
            }

            //the total height/width of the image to be created
            int exportWidth = columns * spriteWidth,
                exportHeight = rows * spriteHeight;

            Bitmap bitmap = new Bitmap(exportWidth, exportHeight);

            try
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    int frameIndex = 0;

                    for (int y = 0; y < exportHeight; y += spriteHeight)
                    {
                        //once we hit the max frames, just break
                        for (int x = 0; x < exportWidth && frameIndex < this.Frames.Count; x += spriteWidth)
                        {
                            g.DrawImage(this.Frames[frameIndex].IndexedBitmap.Bitmap, new Point(x, y));
                            frameIndex++;
                        }
                    }
                }
            }
            catch
            {
                bitmap = null;
            }

            if (bitmap == null)
                Logger.TraceInformation($"Failed to create sprite sheet bitmap for {this.SpriteId}");
            
            return bitmap;
        }
        public byte[] ToSprFile()
        {
            byte[] save;

            using (MemoryStream ms = new MemoryStream())
            {
                foreach (SkaaFrame f in this.Frames)
                {
                    byte[] frameData = f.ToSprFile();
                    ms.Write(BitConverter.GetBytes(frameData.Length), 0, sizeof(int));
                    ms.Write(frameData, 0, frameData.Length);
                }

                ms.Position = 0;
                save = ms.ToArray();
            }

            return save;
        }
        public List<byte[]> GetSpriteFrameByteArrays()
        {
            List<byte[]> frames = new List<byte[]>();
            foreach (SkaaFrame f in this.Frames)
                frames.Add(f.ToSprFile());
            return frames;
        }
        public static SkaaSprite FromSprStream(Stream str, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
            if (str is FileStream)
                spr.SpriteId = Path.GetFileNameWithoutExtension(((FileStream) str).Name);
            try
            {
                while (str.Position < str.Length)
                {
                    IndexedBitmap iBmp = new IndexedBitmap(pal);
                    SkaaSpriteFrame sf = new SkaaSpriteFrame(spr);
                    sf.IndexedBitmap = iBmp;
                    sf.BitmapOffset = str.Position;
                    iBmp.SetBitmapFromRleStream(str, FileFormats.SpriteSpr);
                    spr.Frames.Add(sf);
                }
            }
            catch
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Failed to load sprite{" " + spr.SpriteId} from stream.");
                Debugger.Break();
            }
            return spr;
        }
    }
}
