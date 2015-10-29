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
        public DataTable GameSetDataTable
        {
            get;
            set;
        }
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
            return sf;
        }
        /// <summary>
        /// Builds a 7KAA-formatted SPR containing all of this sprite's frames
        /// </summary>
        /// <returns>A byte array containing the SPR data that can be written directly to a file</returns>
        public byte[] BuildSPR()
        {
            List<byte[]> SPRArrays = new List<byte[]>();
            int initSize = 0;
            
            foreach (SpriteFrame sf in this.Frames)
            {
                SPRArrays.Add(sf.BuildBitmap8bppIndexed());
                initSize += (sf.SprFrameRawDataSize + 4); //add another four for ulong size
            }

            int lastSize = 0;
            byte[] save = new byte[initSize];

            foreach (byte[] ba in SPRArrays)
            {
                Buffer.BlockCopy(ba, 0, save, lastSize, Buffer.ByteLength(ba));
                lastSize += ba.Length;
            }

            return save;
        }

        /// <summary>
        /// Finds the DataTable for this <see cref="Sprite"/> in the provided DataSource by looking
        /// for a DataTable which has a name that matches this sprite's <see cref="SpriteId"/>. The
        /// sprite's <see cref="GameSetDataTable"/> is set to the matching DataTable or null if 
        /// none is found.
        /// </summary>
        /// <param name="ds">The DataSource in which to look for the matching DataTable.</param>
        /// <returns>The sprite's <see cref="GameSetDataTable"/>, which will be null if no match was found.</returns>
        public DataTable GetTable(DataSet ds)
        {
            this.GameSetDataTable = ds.Tables[this.SpriteId];
            return this.GameSetDataTable;
        }

        /// <summary>
        /// Iterates through all the rows in this <see cref="Sprite"/>'s <see cref="GameSetDataTable"/> and 
        /// sets each of this sprite's <see cref="SpriteFrame"/>'s <see cref="SpriteFrame.GameSetDataRow"/>
        /// property to the DataRow with a BITMAPPTR matching <see cref="SpriteFrame.SprBitmapOffset"/>.
        /// </summary>
        public void MatchFrameOffsets()
        { 
#if DEBUG
            //counts how many frames find matches for offsets
            int frameOffsetMatches = 0; 
            //a list that can be copy/pasted to Excel and compared against a manual DBF dump
            List<uint?> offsets = new List<uint?>();
            foreach (SpriteFrame s in this.Frames)
                offsets.Add(s.SprBitmapOffset);
#endif

            foreach (DataRow dr in this.GameSetDataTable.Rows)
            {
                string x = dr.ItemArray[9].ToString();

                /* Codepage 437 is needed due to the use of Extended ASCII.
                 *
                 * For example, in SFRAME.dbf, the BITMAPPTR is labeled as a
                 * CHAR type (which is just a string in dBase III parlance).
                 * Because of this, it's not possible to read the bytes in as
                 * numbers, even though they're just used as pointers in the 
                 * original code.
                 */
                byte[] bytes = Encoding.GetEncoding(437).GetBytes(x);

                uint? offset = null;
                
                switch(bytes.Length)
                {
                    case 0:
                        offset = 0;
                        break;
                    case 1:
                        offset = bytes[0];
                        break;
                    case 2:
                        offset = BitConverter.ToUInt16(bytes, 0);
                        break;
                    case 3:
                        byte[] copy = new byte[4];
                        bytes.CopyTo(copy, 0);
                        offset = BitConverter.ToUInt32(copy, 0);
                        break;
                    case 4:
                        offset = BitConverter.ToUInt32(bytes, 0);
                        break;
                    default:
                        throw new ArgumentNullException("There was an issue reading offsets from the sprite's DataTable that will cause the variable \'offset\' to remain null.");
                }                    

                SpriteFrame sf = this.Frames.Find(f => f.SprBitmapOffset == offset);

                if (sf != null)
                {
                    sf.GameSetDataRow = dr;
#if DEBUG
                    frameOffsetMatches++;
#endif
                }
                else
                {
                    throw new ArgumentNullException(string.Format("Unable to find matching offset in Sprite.Frames for {0} and offset: {1}.", this.SpriteId, offset.ToString()));
                }
            }
        }

        private void Sprite_PaletteUpdated(object sender, EventArgs e) { }
    }
}
