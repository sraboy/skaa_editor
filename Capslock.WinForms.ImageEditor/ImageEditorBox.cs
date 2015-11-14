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
using Cyotek.Windows.Forms;
using System.Drawing.Imaging;
using BitmapProcessing;
using System.Diagnostics;
using System.IO;

namespace Capslock.WinForms.ImageEditor
{
    public partial class ImageEditorBox : ImageBox
    {
        #region Private Vars
        //private bool _editMode;
        //private bool _isDrawing;
        //private bool _panMode;
        private bool _isPanning;
        private int bmpWidth = 0, bmpHeight = 0;
        private Color _activePrimaryColor;
        private Color _activeSecondaryColor;
        private FastBitmap fbmp;
        private Point _startScrollPosition;
        private ToolModes _toolMode;
        private Stream _panToolCursorStream;
        private Stream _pencilToolCursorStream;
        private Stream _paintBucketToolCursorStream;
        private Cursor _panCursor;
        private Cursor _pencilCursor;
        private Cursor _paintBucketCursor;
        #endregion

        [DefaultValue("false")]
        [Category("Behavior")]
        protected virtual bool IsDrawing { get; set; }
        //{
        //    get
        //    {
        //        return this._isDrawing;
        //    }
        //    set
        //    {
        //        if (this._isDrawing != value)
        //        {
        //            this._isDrawing = value;
        //        }
        //    }
        //}

        #region Public Accessors
        //[DefaultValue(false)]
        //[Category("Behavior")]
        //public bool EditMode
        //{
        //    get { return _editMode; }
        //    set
        //    {
        //        if (_editMode != value)
        //        {
        //            _editMode = value;
        //            //this.OnEditModeChanged(EventArgs.Empty);
        //        }
        //    }
        //}
        //[DefaultValue(false)]
        //[Category("Behavior")]
        //public bool PanMode
        //{
        //    get { return _panMode; }
        //    set
        //    {
        //        if (_panMode != value)
        //        {
        //            _panMode = value;
        //        }
        //    }
        //}
        [Category("Behavior")]
        public Color ActivePrimaryColor
        {
            get { return _activePrimaryColor; }
            set
            {
                if (_activePrimaryColor != value)
                {
                    _activePrimaryColor = value;
                }
            }
        }
        [Category("Behavior")]
        public Color ActiveSecondaryColor
        {
            get { return _activeSecondaryColor; }
            set
            {
                if (_activeSecondaryColor != value)
                {
                    _activeSecondaryColor = value;
                }
            }
        }
        [DefaultValue("None")]
        [Category("Behavior")]
        public ToolModes ToolMode
        {
            get { return _toolMode; }
            protected set
            {
                if (_toolMode != value)
                {
                    _toolMode = value;
                }
            }
        }
        /// <summary>
        ///   Gets a value indicating whether this control is panning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this control is panning; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override bool IsPanning
        {
            get { return _isPanning; }
            protected set
            {
                if (_isPanning != value)
                {
                    CancelEventArgs args;

                    args = new CancelEventArgs();

                    if (value)
                    {
                        this.OnPanStart(args);
                    }
                    else
                    {
                        this.OnPanEnd(EventArgs.Empty);
                    }

                    if (!args.Cancel)
                    {
                        _isPanning = value;

                        if (value)
                        {
                            _startScrollPosition = this.AutoScrollPosition;
                            //this.Cursor = Cursors.SizeAll; 
                            //sraboy-12Nov15-we're handling the cursor in SkaaEditorMainForm for now
                        }
                        //else
                        //{
                        //    this.Cursor = Cursors.Default;
                        //}
                    }
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler ImageUpdated;
        protected virtual void OnImageUpdated(EventArgs e)
        {
            Bitmap bmp = this.Image as Bitmap;
            this.fbmp = new FastBitmap(bmp);
            this.bmpWidth = bmp.Width;
            this.bmpHeight = bmp.Height;

            EventHandler handler = ImageUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Overidden Mouse Events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(this.Image != null)
            {
                if (!this.Focused)
                {
                    this.Focus();
                }

                switch (this.ToolMode)
                {
                    //if editing, we don't want to call base.OnMouseDown() because
                    //the control defaults to panning since it wasn't made for editing
                    case ToolModes.Pencil:
                        this.PencilDraw(e);
                        break;
                    case ToolModes.Pan:
                        base.OnMouseDown(e);
                        break;
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Image != null)
            {
                switch (this.ToolMode)
                {
                    //if editing, we don't want to call base.OnMouseMove() because
                    //the control defaults to panning since it wasn't made for editing
                    case ToolModes.Pencil:
                        this.PencilDraw(e);
                        break;
                    case ToolModes.Pan:
                        base.OnMouseMove(e);
                        break;
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageUpdated(EventArgs.Empty);
            }

            if(this.ToolMode == ToolModes.PaintBucket)
                this.PaintBucketFill(e);
           
            base.OnMouseUp(e);
        }
        #endregion

        protected virtual void PaintBucketFill(MouseEventArgs e)
        {
            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

            Point currentPoint = this.PointToImage(e.X, e.Y);

            if ((currentPoint.X < Image.Width && currentPoint.Y < Image.Height) && (currentPoint.X >= 0 && currentPoint.Y >= 0))
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    FloodFill(this.Image as Bitmap, currentPoint, this.fbmp.GetPixel(currentPoint.X, currentPoint.Y),this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }

                this.Invalidate(this.ViewPortRectangle);
            }
        }

        private static bool ColorMatch(Color a, Color b)
        {
            return (a.ToArgb() & 0xffffff) == (b.ToArgb() & 0xffffff);
        }

        protected virtual void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(pt);

            while (q.Count > 0)
            {
                Point n = q.Dequeue();
                if (!ColorMatch(this.fbmp.GetPixel(n.X, n.Y), targetColor))
                    continue;
                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X > 0) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y), targetColor))
                {
                    this.fbmp.SetPixel(w.X, w.Y, replacementColor);
                    if ((w.Y > 0) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y - 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < bmp.Height - 1) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y + 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X < bmp.Width - 1) && ColorMatch(this.fbmp.GetPixel(e.X, e.Y), targetColor))
                {
                    this.fbmp.SetPixel(e.X, e.Y, replacementColor);
                    if ((e.Y > 0) && ColorMatch(this.fbmp.GetPixel(e.X, e.Y - 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y - 1));
                    if ((e.Y < bmp.Height - 1) && ColorMatch(this.fbmp.GetPixel(e.X, e.Y + 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y + 1));
                    e.X++;
                }
            }
        }

        //protected virtual void Fill(Point node, Color targetPixelColor, Color replacementColor)
        //{
        //    bool alpha = targetPixelColor.A == replacementColor.A;
        //    bool red = targetPixelColor.R == replacementColor.R;
        //    bool green = targetPixelColor.G == replacementColor.G;
        //    bool blue = targetPixelColor.B == replacementColor.B;
        //    if (alpha && red && green && blue)
        //        return;
        //    Queue<Point> points = new Queue<Point>();
        //    points.Enqueue(node);
        //    Point editingPoint, north, south, east, west;
        //    while (points.Count > 0)
        //    {
        //        editingPoint = points.Dequeue();
        //        targetPixelColor = this.fbmp.GetPixel(editingPoint.X, editingPoint.Y);
        //        alpha = targetPixelColor.A == replacementColor.A;
        //        red = targetPixelColor.R == replacementColor.R;
        //        green = targetPixelColor.G == replacementColor.G;
        //        blue = targetPixelColor.B == replacementColor.B;
        //        if (!(alpha && red && green && blue))
        //            this.fbmp.SetPixel(editingPoint.X, editingPoint.Y, replacementColor);
        //        else //they're the same
        //        {
        //            if(points.Count > 0)
        //                points.Dequeue();
        //            continue;
        //        }
        //        if (editingPoint.Y - 1 > 0)
        //        {
        //            north = editingPoint;
        //            north.Y--;
        //            points.Enqueue(north);
        //        }
        //        else
        //            north = Point.Empty;
        //        if (editingPoint.Y + 1 < this.Image.Height)
        //        {
        //            south = editingPoint;
        //            south.Y++;
        //            points.Enqueue(south);
        //        }
        //        else
        //            south = Point.Empty;
        //        if (editingPoint.X + 1 < this.Image.Width)
        //        {
        //            east = editingPoint;
        //            east.X++;
        //            points.Enqueue(east);
        //        }
        //        else
        //            east = Point.Empty;
        //        if (editingPoint.X - 1 > 0)
        //        {
        //            west = editingPoint;
        //            west.X--;
        //            points.Enqueue(west);
        //        }
        //        else
        //            west = Point.Empty;
        //    }            
        //}

        protected virtual void PencilDraw(MouseEventArgs e)
        {
            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

            Point currentPixel = this.PointToImage(e.X, e.Y);

            if ((currentPixel.X < Image.Width && currentPixel.Y < Image.Height) && (currentPixel.X >= 0 && currentPixel.Y >= 0))
            {
                if (e.Button == MouseButtons.Left)
                {
                    //replaced slow SetPixel with FastBitmap from BitmapProcessing lib
                    //(this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, this.ActiveColor);
                    this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    this.fbmp.SetPixel(currentPixel.X, currentPixel.Y, this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }
                if (e.Button == MouseButtons.Right)
                {
                    //replaced slow SetPixel with FastBitmap from BitmapProcessing lib
                    //(this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, this._skaaTransparentColor);
                    this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    this.fbmp.SetPixel(currentPixel.X, currentPixel.Y, this.ActiveSecondaryColor);
                    this.fbmp.UnlockImage();
                }
                
                this.Invalidate(this.ViewPortRectangle);
            }
        }

        public ImageEditorBox() : base()
        {
            //have to load these dynamically since they're not just black/white
            var str = this.GetType().Assembly.GetManifestResourceNames();
            this._panCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PanToolCursor.cur")));
            this._pencilCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PencilToolCursor.cur")));
            this._paintBucketCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PaintBucketToolCursor.cur")));
            
            //this._panToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PanToolCursor.cur"));
            //this._pencilToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PencilToolCursor.cur"));
            //this._paintBucketToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PaintBucketToolCursor.cur"));

            //this.ImageChanged += ImageEditorBox_ImageChanged;
        }

        public virtual void ChangeToolMode(object sender, EventArgs e)//ToolModes vm)
        {
            this.Focus();
            this.ToolMode = (e as DrawingToolSelectedEventArgs).SelectedTool;
            ChangeCursor();
        }

        protected virtual void ChangeCursor()
        {
            switch (this.ToolMode)
            {
                case ToolModes.PaintBucket: //todo: need to detect this and fill, not just pencil draw
                    this.Cursor = this._paintBucketCursor;//new Cursor(this._paintBucketToolCursorStream);
                    break;
                case ToolModes.Pencil:
                    this.Cursor = this._pencilCursor;//new Cursor(this._pencilToolCursorStream);
                    break;
                case ToolModes.Pan:
                    this.Cursor = this._panCursor;//new Cursor(this._panToolCursorStream);
                    break;
                case ToolModes.None:
                    this.Cursor = this.DefaultCursor;
                    break;
            }
        }

        //private void ImageEditorBox_ImageChanged(object sender, EventArgs e)
        //{
        //    Bitmap bmp = this.Image as Bitmap;
        //    this.fbmp = new FastBitmap(bmp);
        //    this.bmpWidth = bmp.Width;
        //    this.bmpHeight = bmp.Height;
        //}
    }
}
