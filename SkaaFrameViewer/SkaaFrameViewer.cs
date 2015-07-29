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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkaaGameDataLib;

namespace SkaaFrameViewer
{
    public partial class SkaaFrameViewer : UserControl
    {
        public event EventHandler ActiveFrameChanged;
        protected virtual void OnActiveFrameChanged(EventArgs e)
        {
            EventHandler handler = ActiveFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        Sprite _activeSprite;
        SpriteFrame _activeFrame;
        int _activeFrameIndex;

        public Sprite ActiveSprite
        {
            get
            {
                return this._activeSprite;
            }
            set
            {
                if(this._activeSprite != value)
                {
                    this._activeSprite = value;
                }
            }
        }
        public SpriteFrame ActiveFrame
        {
            get
            {
                return this._activeFrame;
            }
            set
            {
                if (this._activeFrame != value)
                {
                    this._activeFrame = value;
                    this._activeFrameIndex = this._activeSprite.Frames.FindIndex(0, (f => f == _activeFrame));
                    this.picBoxFrame.Image = this._activeFrame.ImageBmp;
                    this.OnActiveFrameChanged(null);
                }
            }
        }

        public SkaaFrameViewer()
        {
            InitializeComponent();
            picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void picBoxFrame_Click(object sender, MouseEventArgs e) 
        {
            if (ActiveFrame == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _activeFrameIndex++;
                _activeFrameIndex %= (ActiveSprite.Frames.Count - 1);
                this.ActiveFrame = this.ActiveSprite.Frames[_activeFrameIndex];
                //picBoxFrame.Image = ActiveSprite.Frames[_activeFrameIndex].ImageBmp;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _activeFrameIndex--;
                _activeFrameIndex = (_activeFrameIndex % (ActiveSprite.Frames.Count - 1) + (ActiveSprite.Frames.Count - 1)) % (ActiveSprite.Frames.Count - 1);
                // special mod() function above to actually cycle negative numbers around. Turns out % isn't 
                // a real mod() function, just remainder.
                this.ActiveFrame = this.ActiveSprite.Frames[_activeFrameIndex];
                //picBoxFrame.Image = ActiveSprite.Frames[_activeFrameIndex].ImageBmp;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                //todo: change this to raise an event that a new frame was selected so MainForm can updated the editor with the selected frame
                if (picBoxFrame.SizeMode == PictureBoxSizeMode.CenterImage)
                    picBoxFrame.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    picBoxFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        
    }
}
