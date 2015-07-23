/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
*
*   Much thanks to the creators of the base class, Cyotek Ltd. The ImageBox
*   class is licensed under the MIT License. You may find more information
*   here: http://www.cyotek.com/blog/imagebox-1-1-4-0-update and
*   here: https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox.
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Cyotek.Windows.Forms;

namespace SkaaEditor
{
    public partial class SkaaImageBox : ImageBox
    {
        #region Private Vars
        private Boolean _editMode;
        private Boolean _isDrawing;
        private Color _activeColor;
        #endregion
        #region Accessor Methods
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (_editMode != value)
                {
                    _editMode = value;
                    this.OnEditModeChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Behavior")]
        public Color ActiveColor
        {
            get { return _activeColor; }
            set
            {
                if (_activeColor != value)
                {
                    _activeColor = value;
                    this.OnActiveColorChanged(EventArgs.Empty);
                }
            }
        }
        public Boolean IsDrawing
        {
            get
            {
                return this._isDrawing;
            }
            set
            {
                if (this._isDrawing != value)
                {
                    this._isDrawing = value;
                }
            }
        }
        #endregion

        public event EventHandler ImageUpdated;
        protected virtual void OnImageUpdated(EventArgs e)
        {
            EventHandler handler = ImageUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnEditModeChanged(EventArgs eventArgs)
        {
        }
        private void OnActiveColorChanged(EventArgs eventArgs)
        {
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            PenDraw(e);

            if (!this.Focused)
            {
                this.Focus();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            PenDraw(e);
            base.OnMouseMove(e);
        }
        private void PenDraw(MouseEventArgs e)
        {
            if (this.EditMode == true && this.Image != null)
            {
                this.IsDrawing = true;
                this.IsSelecting = false;
                this.IsPanning = false;

                Point currentPixel;
                currentPixel = this.PointToImage(e.X, e.Y);

                if ((currentPixel.X < Image.Width && currentPixel.Y < Image.Height) && (currentPixel.X >= 0 && currentPixel.Y >= 0))
                {
                    if (e.Button == MouseButtons.Left)
                        (this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, this.ActiveColor);
                    if (e.Button == MouseButtons.Right)
                        (this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, Color.Black); //todo:setup secondary color as transparent. will have to update save to convert trans to black

                    this.Invalidate(this.ViewPortRectangle);
                    this.Update();
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if(this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageUpdated(null);
            }
            base.OnMouseUp(e);
        }
    }
}
