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

namespace SkaaGameDataLib
{
    [Serializable]
    public class Sprite
    {
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
        private List<SpriteFrame> _frames;
        private string _spriteId;
        private SpriteResource _resource;
        #endregion
        
        #region Public Properties
        public List<SpriteFrame> Frames
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
        public SpriteResource Resource
        {
            get
            {
                return this._resource;
            }
            set
            {
                if (this._resource != value)
                {
                    this._resource = value;
                }
            }
        }
        #endregion
        //public bool PendingChanges;

        #region Constructors
        public Sprite(ColorPalette pal)
        {
            this.Frames = new List<SpriteFrame>();
            this.Resource = new SpriteResource(pal);
        }
        #endregion

        public bool SetSpriteDataView(DataView dv)
        {
            this.Resource.SpriteDataView = dv;
            return this.Resource.MatchFrameOffsets(this);
        }
    }
}
