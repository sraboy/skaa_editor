using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace SkaaGameDataLib
{
    public class SpriteResource
    {
        #region Private Members
        [NonSerialized]
        private ColorPalette _pallet;
        private byte[] _sprData;
        private string _fileName;
        private DataView _spriteDataView;
        #endregion

        #region Event Handlers
        [NonSerialized]
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

        [NonSerialized]
        private EventHandler _spriteObjectChanged;
        public event EventHandler SpriteObjectChanged
        {
            add
            {
                if (_spriteObjectChanged == null || !_spriteObjectChanged.GetInvocationList().Contains(value))
                {
                    _spriteObjectChanged += value;
                }
            }
            remove
            {
                _spriteObjectChanged -= value;
            }
        }
        protected virtual void OnSpriteObjectChanged(EventArgs e)
        {
            EventHandler handler = _spriteObjectChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Public Properties
        public string FileName
        {
            get
            {
                return this._fileName;
            }
            set
            {
                if (this._fileName != value)
                {
                    this._fileName = value;
                }
            }
        }
        public DataView SpriteDataView
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
        public byte[] SprData
        {
            get
            {
                return this._sprData;
            }
            private set
            {
                if (this._sprData != value)
                {
                    this._sprData = value;
                }
            }
        }
        #endregion

        internal ColorPalette Palette
        {
            get
            {
                return this._pallet;
            }
            set
            {
                if (this._pallet != value)
                {
                    this._pallet = value;
                    OnPaletteUpdated(EventArgs.Empty);
                }
            }
        }

        #region Constructors
        public SpriteResource()
        {
            Initialize();
        }
        public SpriteResource(ColorPalette pal)
        {
            this.Palette = pal;
            Initialize();
        }
        public void Initialize()
        {
            this.PaletteUpdated += SpriteResource_PaletteUpdated;
        }

        private void SpriteResource_PaletteUpdated(object sender, EventArgs e)
        {
            //todo: rebuild the BMP with the new palette
            Trace.WriteLine("SpriteFrame palette updated.");
        }
        #endregion

        /// <summary>
        /// Iterates through all the rows in the <see cref="Sprite"/>'s <see cref="GameSetDataTable"/> and 
        /// sets each of this sprite's <see cref="SpriteFrame"/>'s <see cref="SpriteFrame.GameSetDataRows"/>
        /// property to the DataRow with a BITMAPPTR matching <see cref="SpriteFrame.SprBitmapOffset"/>.
        /// </summary>
        /// <returns>False if any frame did not have a match in the DataView. True otherwise.</returns>
        internal bool MatchFrameOffsets(Sprite spr)
        {
            foreach (DataRowView drv in this.SpriteDataView)
            {
                int offset = Convert.ToInt32(drv.Row.ItemArray[9]);
                SpriteFrame sf = spr.Frames.Find(f => f.SprBitmapOffset == offset);

                if (sf == null)
                {
                    //this should only happen when creating new sprites. 
                    Trace.WriteLine(($"Unable to find matching offset in Sprite.Frames for {spr.SpriteId} and offset: {offset.ToString()}. nDid you forget to load the proper SET file for this sprite?"));
                    return false;
                }

                if (sf != null)
                    sf.GameSetDataRows.Add(drv.Row);
            }

            return true;
        }

        public void ProcessUpdates(SpriteFrame frameToUpdate, Bitmap bmpWithChanges)
        {
            //update the bitmap, if frameToUpdate.PendingChanges is true
            frameToUpdate.ProcessUpdates(bmpWithChanges);
            Sprite spr = frameToUpdate.ParentSprite;

            List<byte[]> SpriteFrameDataArrays = new List<byte[]>();

            //update the SprBitmapOffset since changes will change the size of the  
            //FrameRawData when it gets written to an SPR. The game depends on having 
            //the exact offsets to the SPR data.
            int offset = 0;
            for (int i = 0; i < spr.Frames.Count; i++)
            {
                SpriteFrame sf = spr.Frames[i];
                offset += sf.FrameRawData.Length;
                Debug.Assert(sf.FrameRawData != null, $"Sprite {sf.ParentSprite.SpriteId}'s SpriteFrame's FrameRawData is null!");
                
                //we depend on short-circuit evaluation here.If i isn't less then the Frames.Count - 1, 
                //we'll end up with an out-of-bounds exception. We can't just test for PendingChanges because
                //changes in one SpriteFrame will affect offsets in others, not in itself.
                if ((i < spr.Frames.Count - 1) && (spr.Frames[i + 1].SprBitmapOffset != offset))
                {
                    spr.Frames[i + 1].SprBitmapOffset = offset;

                    foreach (DataRow dr in spr.Frames[i + 1].GameSetDataRows)
                    {
                        dr.BeginEdit();
                        dr[9] = offset.ToString();
                        dr.AcceptChanges(); //calls EndEdit() implicitly
                    }
                    sf.PendingChanges = false;
                }
                //Killing two birds with one for loop. See below.
                SpriteFrameDataArrays.Add(sf.FrameRawData);
            }

            //convert the List<byte[]> to a byte[]
            int lastSize = 0;
            byte[] newSprData = new byte[offset]; //offset now equals the total size of all the frames

            foreach (byte[] b in SpriteFrameDataArrays)
            {
                Buffer.BlockCopy(b, 0, newSprData, lastSize, b.Length);
                lastSize += b.Length;
            }

            this._sprData = newSprData;
        }
    }
}
