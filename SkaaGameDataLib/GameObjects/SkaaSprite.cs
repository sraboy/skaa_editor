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
                if (this._frames != value)
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
            if (dv?.Table != null)
            {
                this.DataView = dv;

                string frameOffsetColName;
                string frameNameColName;

                if (dv.Table.Columns.Contains("BITMAPPTR"))
                    frameNameColName = "BITMAPPTR";
                else
                    frameNameColName = "FrameOffset";

                if (dv.Table.Columns.Contains("FILENAME"))
                    frameOffsetColName = "FILENAME";
                else
                    frameOffsetColName = "FrameName";

                return this.MatchFramesToDataView(frameNameColName, frameOffsetColName);
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
        /// Iterates through all the rows in <see cref="SkaaSprite.DataView"/> and sets each of this
        /// sprite's <see cref="SkaaSpriteFrame"/>'s <see cref="SkaaSpriteFrame.GameSetDataRows"/>
        /// property to the DataRow with a matching <see cref="SkaaSpriteFrame.BitmapOffset"/> or, if none is found, 
        /// a matching <see cref="SkaaFrame.Name"/>. The frame's name and offset are set based on the corresponding
        /// cell in the DataView.
        /// </summary>
        /// <param name="frameNameColName">Either FILENAME for SFRAME in std.set or FrameName for ResIdxMultiBmp files</param>
        /// <param name="frameOffsetColName">Either BITMAPPTR for SFRAME in std.set or FrameOffset for ResIdxMultiBmp files</param>
        /// <returns>False if any frame did not have a match in the DataView. True otherwise.</returns>
        /// <remarks>
        /// SkaaSprite was based primarily on SPR files so a lot of the code was
        /// specific to those formats and ResIdxMultiBmp-formatted RES files, like
        /// i_button.res for example, were hacked in around that.
        /// 
        /// This method will find matching frames in its Frames list based first off of the
        /// offset and, failing that, off the name. This is because SPR files have no
        /// frame name data in them so this was used to find those frames in std.set.
        /// These frames had their offsets calculated when the file was read. This
        /// allowed for opening an SPR file and opening std.set afterwards.
        /// ResIdxMultiBmp files have frame names in them so that can be used as a
        /// fallback.
        /// 
        /// Additionally, an SPR's frames' names and offsets are stored in columns
        /// called "FILENAME" and "BITMAPPTR". This is in std.set's SFRAME table and
        /// is also used in other files. However, ResIdx files don't have field names
        /// like DBF files, so they're assigned "FrameName" and "FrameOffset" by the
        /// DataRowExtensions class. I chose to use these for ResIdxMultiBmpr ather 
        /// than BITMAPPTR and FILENAME primarily because FILENAME would be confusing 
        /// since there are no actual files for these frames; this is from the original developers.
        /// </remarks>
        internal bool MatchFramesToDataView(string frameNameColName, string frameOffsetColName)
        {
            foreach (DataRowView drv in this.DataView)
            {
                int offset = Convert.ToInt32(drv.Row[frameNameColName]);
                string name = (string)drv.Row[frameOffsetColName];
                SkaaFrame sf = this.Frames.Find(f => f.BitmapOffset == offset) ?? this.Frames.Find(f => f.Name == name);
                sf.Name = name;
                sf.BitmapOffset = offset;

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
        /// Builds a <see cref="List{T}"/> of byte arrays, one array for each <see cref="SkaaFrame"/> in <see cref="SkaaSprite.Frames"/>
        /// </summary>
        /// <returns>The <see cref="List{T}"/> where T is a <see cref="T:byte[]"/></returns>
        public List<byte[]> GetSpriteFrameByteArrays()
        {
            List<byte[]> frames = new List<byte[]>();
            foreach (SkaaFrame f in this.Frames)
                frames.Add(f.ToSprFile());
            return frames;
        }
        /// <summary>
        /// Creates a new <see cref="SkaaSprite"/> from a stream of SPR-formatted data
        /// </summary>
        /// <param name="str">The stream to read the SPR data from</param>
        /// <param name="pal">The <see cref="ColorPalette"/> to apply to the sprite's images</param>
        /// <returns>A new <see cref="SkaaSprite"/></returns>
        /// <remarks>
        /// The original game code for reading SPR files can be found <c>ResourceDb::init_imported()</c> 
        /// in src/ORESDB.cpp around line 72. The <c>resName</c> will be "sprite\\NAME.SPR". SPR files are 
        /// are considered <c>FLAT</c> by 7KAA. 
        /// </remarks>
        public static SkaaSprite FromSprStream(Stream str, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
            if (str is FileStream)
                spr.SpriteId = Path.GetFileNameWithoutExtension(((FileStream)str).Name);
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
            catch (Exception e)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Failed to load sprite: {spr.SpriteId} (Exception: {e.Message})");
                Debugger.Break();
            }
            return spr;
        }
    }
}
