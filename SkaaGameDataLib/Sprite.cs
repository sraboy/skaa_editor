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
        [field: NonSerialized]
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
                    OnPaletteUpdated(EventArgs.Empty);
                }
            }
        }
        public List<SpriteFrame> Frames
        {
            get;
            set;
        }
        public DataView SpriteDataView;
        public string SpriteId
        {
            get;
            set;
        }
        public bool PendingChanges;

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
        /// <summary>
        /// Builds a 7KAA-formatted SPR containing all of this sprite's frames
        /// </summary>
        /// <returns>A byte array containing the SPR data that can be written directly to a file</returns>
        public byte[] BuildSPR()
        {
            List<byte[]> SpriteFrameDataArrays = new List<byte[]>();
            int initSize = 0;

            for(int i = 0; i < this.Frames.Count; i++)
            {
                SpriteFrame sf = this.Frames[i];
                SpriteFrameDataArrays.Add(sf.BuildBitmap8bppIndexed());
                initSize += sf.FrameRawData.Length;
            }

            //convert the List<byte[]> to a byte[]
            int lastSize = 0;
            byte[] newSprData = new byte[initSize];

            foreach (byte[] b in SpriteFrameDataArrays)
            {
                Buffer.BlockCopy(b, 0, newSprData, lastSize, b.Length);
                lastSize += b.Length;
            }

            return newSprData;
        }
        /// <summary>
        /// Iterates through all the rows in this <see cref="Sprite"/>'s <see cref="GameSetDataTable"/> and 
        /// sets each of this sprite's <see cref="SpriteFrame"/>'s <see cref="SpriteFrame.GameSetDataRows"/>
        /// property to the DataRow with a BITMAPPTR matching <see cref="SpriteFrame.SprBitmapOffset"/>.
        /// </summary>
        public void MatchFrameOffsets()
        {
            foreach (DataRowView drv in this.SpriteDataView)//GameSetDataTable.Rows)
            {
                int offset = Convert.ToInt32(drv.Row.ItemArray[9]);
                SpriteFrame sf = this.Frames.Find(f => f.SprBitmapOffset == offset);
                
                Debug.WriteLineIf(sf == null, string.Format("Unable to find matching offset in Sprite.Frames for {0} and offset: {1}.", this.SpriteId, offset.ToString()));

                if (sf != null)
                    sf.GameSetDataRows.Add(drv.Row);
            }
        }
        public void ProcessUpdates(SpriteFrame frameToUpdate, Bitmap bmpWithChanges)
        {
            frameToUpdate.ProcessUpdates(bmpWithChanges);
            frameToUpdate.SprFrameRawDataSize = frameToUpdate.FrameRawData.Length;

            int offset = 0;
            for(int i = 0; i < this.Frames.Count; i++)
            {
                SpriteFrame sf = this.Frames[i];
                offset += sf.FrameRawData.Length;
                if (i < this.Frames.Count - 1)
                { 
                    if(this.Frames[i + 1].SprBitmapOffset != offset)
                    { 
                        this.Frames[i + 1].SprBitmapOffset = offset;
                        foreach (DataRow dr in this.Frames[i + 1].GameSetDataRows)
                        { 
                            dr.BeginEdit();
                            dr[9] = offset.ToString();
                            dr.AcceptChanges(); //calls EndEdit() implicitly
                        }
                    }
                }
            }

            BuildSPR();
            OnSpriteUpdated(EventArgs.Empty);
        }
        private void Sprite_PaletteUpdated(object sender, EventArgs e) { }
    }
}
