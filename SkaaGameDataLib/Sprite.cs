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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//todo: replace default parameters with overloads to reduce issues with reflection and calling from other languages

namespace SkaaGameDataLib
{
    [Serializable]
    public class Sprite
    {
        [NonSerialized]
        private ColorPalette _pallet;
        
        [field: NonSerialized]
        private EventHandler _paletteUpdated;
        public event EventHandler PaletteUpdated
        {
            add
            {
                if (_paletteUpdated == null || !_paletteUpdated.GetInvocationList().Contains(value))
                {
                    _paletteUpdated += value;
                }
            }
            remove
            {
                _paletteUpdated -= value;
            }
        }
        protected virtual void OnPaletteUpdated(EventArgs e)
        {
            EventHandler handler = _paletteUpdated;

            if (handler != null)
                handler(this, e);
        }

        public ColorPalette Palette
        {
            get
            {
                return this._pallet;
            }
            set
            {
                if(this._pallet != value)
                {
                    this._pallet = value;
                    OnPaletteUpdated(new EventArgs());
                }
            }
        }
        public List<SpriteFrame> Frames
        {
            get;
            set;
        }
        //public DataTable GameSetDataTable;
        public DataView SpriteRows;
        public string SpriteId
        {
            get;
            set;
        }

        #region Constructors
        /// <summary>
        /// A default constructor which performs no initialization or setup except an internal event subscription <see cref="PaletteUpdated"/>
        /// </summary>
        public Sprite()
        {
            this.PaletteUpdated += Sprite_PaletteUpdated;
        }
        /// <summary>
        /// Creates a new Sprite object with the specified ColorPalette and instantiates an empty List of SpriteFrames in <see cref="Frames"/>.
        /// </summary>
        /// <param name="pal">The palette to use for this sprite. Accessible via <see cref="Palette"/></param>
        public Sprite(ColorPalette pal)
        {
            this.PaletteUpdated += Sprite_PaletteUpdated;
            this.Palette = pal;
            this.Frames = new List<SpriteFrame>();
        }
        #endregion

        /// <summary>
        /// Adds either a new frame or, if provided, the specified frame to the <see cref="Frames"/> List
        /// </summary>
        /// <param name="sf">A particular frame to add</param>
        /// <returns>The new frame</returns>
        public SpriteFrame AddFrame(SpriteFrame sf = null)
        {
            if (sf == null)
            {
                sf = new SpriteFrame();
                this.Frames.Add(sf);
                return sf;
            }

            this.Frames.Add(sf);
            //sf.FrameUpdated += SpriteFrameUpdated;
            return sf;
        }

        //private void SpriteFrameUpdated(object sender, EventArgs e)
        //{

        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Builds a 7KAA-formatted SPR containing all of this sprite's frames
        /// </summary>
        /// <returns>A byte array containing the SPR data that can be written directly to a file</returns>
        public byte[] BuildSPR()
        {
            List<byte[]> SpriteFrameDataArrays = new List<byte[]>();
            int initSize = 0;

            //#if DEBUG
            //            int rows = 0;
            //            foreach (SpriteFrame sf in this.Frames)
            //            {
            //                if (sf.GameSetDataRow != null)
            //                    rows++;
            //                else
            //                    Debug.Assert(sf.GameSetDataRow == null, string.Format("SpriteFrame with offset {0} has no GameSetDataRow.", sf.SprBitmapOffset));
            //            }
            //#endif

            for(int i = 0; i < this.Frames.Count; i++)
            {
                SpriteFrame sf = this.Frames[i];

                sf.GameSetDataRow.BeginEdit();
                //sf.SprBitmapOffset = initSize;
                //sf.GameSetDataRow.ItemArray[9] = sf.SprBitmapOffset.ToString();
                sf.GameSetDataRow.AcceptChanges();

                SpriteFrameDataArrays.Add(sf.BuildBitmap8bppIndexed());
                initSize += (sf.SprFrameRawDataSize + 4); //add another four for int size

                if (i < this.Frames.Count - 1) //not the last one
                    this.Frames[i+1].NewSprBitmapOffset = initSize;   
            }

            //convert the List<byte[]> to a byte[]
            int lastSize = 0;
            byte[] save = new byte[initSize];

            foreach (byte[] b in SpriteFrameDataArrays)
            {
                Buffer.BlockCopy(b, 0, save, lastSize, Buffer.ByteLength(b));
                lastSize += b.Length;
            }

            return save;
        }
        ///// <summary>
        ///// Finds the DataTable for this <see cref="Sprite"/> in the provided DataSource by looking
        ///// for a DataTable which has a name that matches this sprite's <see cref="SpriteId"/>. The
        ///// sprite's <see cref="GameSetDataTable"/> is set to the matching DataTable or null if 
        ///// none is found.
        ///// </summary>
        ///// <param name="ds">The DataSource in which to look for the matching DataTable.</param>
        ///// <returns>The sprite's <see cref="GameSetDataTable"/>, which will be null if no match was found.</returns>
        //public DataTable GetTable(DataSet ds)
        //{
        //    this.GameSetDataTable = ds.Tables[this.SpriteId];
        //    return this.GameSetDataTable;
        //}
      
        /// <summary>
        /// Iterates through all the rows in this <see cref="Sprite"/>'s <see cref="GameSetDataTable"/> and 
        /// sets each of this sprite's <see cref="SpriteFrame"/>'s <see cref="SpriteFrame.GameSetDataRow"/>
        /// property to the DataRow with a BITMAPPTR matching <see cref="SpriteFrame.SprBitmapOffset"/>.
        /// </summary>
        public void MatchFrameOffsets()
        {
            //Comparison<SpriteFrame> comp = new Comparison<SpriteFrame>(Misc.CompareFrameOffset);
            //this.Frames.Sort(comp);
            //#if DEBUG
            //            //counts how many frames find matches for offsets
            //            int frameOffsetMatches = 0;
            //            //a list that can be copy/pasted to Excel and compared against a manual DBF dump
            //            List<uint?> offsets = new List<uint?>();
            //            foreach (SpriteFrame s in this.Frames)
            //                offsets.Add(s.SprBitmapOffset);
            //#endif

            foreach (DataRowView drv in this.SpriteRows)//GameSetDataTable.Rows)
            {

                int offset = Convert.ToInt32(drv.Row.ItemArray[9]);

                SpriteFrame sf = this.Frames.Find(f => f.SprBitmapOffset == offset);
                if(sf == null)
                {
                    throw new ArgumentNullException(string.Format("Unable to find matching offset in Sprite.Frames for {0} and offset: {1}.", this.SpriteId, offset.ToString()));
                }
                else
                {
                    sf.GameSetDataRow = drv.Row;
                }

                //List<SpriteFrame> matches = this.Frames.FindAll(f => f.SprBitmapOffset == offset);

//                if (matches.Count > 0)//!= null)// && sf.GameSetDataRow == null)
//                {
//                    foreach (SpriteFrame sf in matches)
//                    {
//                        sf.GameSetDataRow = dr;
//#if DEBUG
//                        frameOffsetMatches++;
//                    }
//#endif
//                }
//                else
//                {
//                    throw new ArgumentNullException(string.Format("Unable to find matching offset in Sprite.Frames for {0} and offset: {1}.", this.SpriteId, offset.ToString()));
//                }
            }

#if DEBUG
            int rows = 0;
            foreach (SpriteFrame sf in this.Frames)
            {
                if (sf.GameSetDataRow != null)
                    rows++;
                else
                    throw new ArgumentNullException(string.Format("SpriteFrame with offset {0} has no GameSetDataRow.", sf.SprBitmapOffset));
            }
#endif
        }

        private void Sprite_PaletteUpdated(object sender, EventArgs e) { }
    }
}
