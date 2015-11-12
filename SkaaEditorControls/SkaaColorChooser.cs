#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Cyotek.Windows.Forms;

namespace SkaaEditorControls
{
    public partial class SkaaColorChooser : ColorGrid
    {
        //private bool _colorPickerActive = false;
        //public bool ColorPickerActive
        //{
        //    get
        //    {
        //        return this._colorPickerActive;
        //    }
        //    set
        //    {
        //        if (this._colorPickerActive != value)
        //        {
        //            this._colorPickerActive = value;
        //            OnEditModeChanged(EventArgs.Empty);
        //        }
        //    }
        //}
        //public SkaaScreenColorPicker ColorDropper;
        //[field: NonSerialized]
        //private EventHandler _colorPickerActiveChanged;
        //public event EventHandler ColorPickerActiveChanged
        //{
        //    add
        //    {
        //        if (_colorPickerActiveChanged == null || !_colorPickerActiveChanged.GetInvocationList().Contains(value))
        //        {
        //            _colorPickerActiveChanged += value;
        //        }
        //    }
        //    remove
        //    {
        //        _colorPickerActiveChanged -= value;
        //    }
        //}
        //protected void OnColorPickerActiveChanged(EventArgs e)
        //{
        //    EventHandler handler = _colorPickerActiveChanged;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        public new ColorCollection CustomColors = null;

        public SkaaColorChooser() : base ()
        {
            //turns off the empty boxes at the bottom of the grid
            this.ShowCustomColors = false;
            //it's set to "Standard256" for the sake of designer showing 256 colors
            //we want it set to None to ensure we don't accidentally show colors that
            //don't exist in our palette
            this.Palette = Cyotek.Windows.Forms.ColorPalette.None;
        }

        //public SkaaColorChooser(ColorCollection colors, ColorCollection customColors)
        //    : base(colors, customColors)
        //{
        //    //this.ColorDropper = new SkaaScreenColorPicker();
        //    //this.ColorDropper.ColorChanged += Picker_ColorChanged;
        //}

        //protected override void EditColor(int colorIndex)
        //{
        //    this.ColorDropper.SetCaptureMode();
        //    //if (_eyedropperCursor == null)
        //    //{
        //    //    // ReSharper disable AssignNullToNotNullAttribute
        //    //    _eyedropperCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Namespace, ".Resources.eyedropper.cur")));
        //    //}
        //}
        //protected override void OnMouseDoubleClick(MouseEventArgs e)
        //{
        //    return;
        //    //ColorHitTestInfo hitTest;
        //    //base.OnMouseDoubleClick(e);
        //    //hitTest = this.HitTest(e.Location);
        //    //if (e.Button == MouseButtons.Left && (hitTest.Source == ColorSource.Custom && this.ColorPickerActive != ColorEditingMode.None || hitTest.Source == ColorSource.Standard && this.ColorPickerActive == ColorEditingMode.Both))
        //    //{
        //    //    this._colorPickerActive = true;
        //    //}
        //}

        //private void Picker_ColorChanged(object sender, EventArgs e) { }
    }

    //public partial class SkaaScreenColorPicker : ScreenColorPicker
    //{
    //    //public Cursor EyeDropperCursor;
    //    public void SetCaptureMode()
    //    {
    //        this.IsCapturing = true;
    //    }
    //    public void MoveMouse()
    //    {
    //        if (this.IsCapturing)
    //        {
    //            this.UpdateSnapshot();
    //        }
    //    }
    //    /// <summary>
    //    /// Updates the snapshot.
    //    /// </summary>
    //    protected override void UpdateSnapshot()
    //    {
    //        return;
    //        Point cursor;
    //        cursor = MousePosition;
    //        cursor.X -= this.SnapshotImage.Width / 2;
    //        cursor.Y -= this.SnapshotImage.Height / 2;
    //        using (Graphics graphics = Graphics.FromImage(this.SnapshotImage))
    //        {
    //            Point center;
    //            // clear the image first, in case the mouse is near the borders of the screen so there isn't enough copy content to fill the area
    //            graphics.Clear(Color.Empty);
    //            // copy the image from the screen
    //            graphics.CopyFromScreen(cursor, Point.Empty, this.SnapshotImage.Size);
    //            // update the active color
    //            center = this.GetCenterPoint();
    //            this.Color = this.SnapshotImage.GetPixel(center.X, center.Y);
    //            // force a redraw
    //            this.HasSnapshot = true;
    //            this.Refresh(); // just calling Invalidate isn't enough as the display will lag
    //        }
    //    }
    //    ///// <summary>
    //    ///// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
    //    ///// </summary>
    //    ///// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
    //    //protected override void OnMouseDown(MouseEventArgs e)
    //    //{
    //    //    base.OnMouseDown(e);
    //    //    if (e.Button == MouseButtons.Left && !this.IsCapturing)
    //    //    {
    //    //        if (EyeDropperCursor == null)
    //    //        {
    //    //            // ReSharper disable AssignNullToNotNullAttribute
    //    //            EyeDropperCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Namespace, ".Resources.eyedropper.cur")));
    //    //        }
    //    //        // ReSharper restore AssignNullToNotNullAttribute
    //    //        this.Cursor = EyeDropperCursor;
    //    //        this.IsCapturing = true;
    //    //        this.Invalidate();
    //    //    }
    //    //}
    //}
}
