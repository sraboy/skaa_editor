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
using System.Drawing.Drawing2D;

namespace Capslock.WinForms.ImageEditor
{
    [DefaultProperty("Image")]
    [ToolboxBitmap(typeof(ImageBox), "ImageBox.bmp")]
    [ToolboxItem(true)]
    public partial class ImageEditorBox : ImageBox
    {
        #region Private Vars
        private bool _isPanning;
        private Color _activePrimaryColor;
        private Color _activeSecondaryColor;
        private FastBitmap fbmp;
        private DrawingTools _toolMode;
        private Cursor _panCursor;
        private Cursor _pencilCursor;
        private Cursor _paintBucketCursor;
        private Point _startMousePosition;
        private Point _startScrollPosition;
        private Queue<Point> _linePoints;
        #endregion

        [DefaultValue("false")]
        [Category("Behavior")]
        protected virtual bool IsDrawing { get; set; }
 
        #region Public Accessors
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
        public DrawingTools SelectedTool
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
        ///   Gets a value indicating whether this control is panning. Overridden to eliminate the changing of the cursor.
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
                            this._startScrollPosition = this.AutoScrollPosition;
                        }
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

            EventHandler handler = ImageUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnImageChanged(EventArgs e)
        {
            this.fbmp = new FastBitmap((Bitmap) this.Image);
            base.OnImageChanged(e);
        }
        #endregion

        #region Constructor
        public ImageEditorBox() : base()
        {
            //have to load these dynamically since they're not just black/white
            //var str = this.GetType().Assembly.GetManifestResourceNames();
            this._panCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PanToolCursor.cur")));
            this._pencilCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PencilToolCursor.cur")));
            this._paintBucketCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PaintBucketToolCursor.cur")));
            this._linePoints = new Queue<Point>();

            this.AutoPan = false; //the base constructor sets this to true
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseClick(MouseEventArgs e)
        {
            switch (this.SelectedTool)
            {
                case DrawingTools.Pencil:
                    this.PencilDraw(e);
                    break;
            }

            base.OnMouseClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(!this.IsDrawing)
                base.OnMouseDown(e);

            switch(this.SelectedTool)
            {
                case DrawingTools.Line:
                    this._linePoints.Enqueue(e.Location);
                    //this._drawLinePoints.Add(e.Location);
                    break;
                case DrawingTools.Pencil:
                    this.PencilDraw(e);
                    break;
                case DrawingTools.Pan:
                    this._startMousePosition = e.Location;
                    break;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (!this.IsDrawing)
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                switch (this.SelectedTool)
                {
                    case DrawingTools.Pencil:
                        this.PencilDraw(e);
                        break;
                }
            }

            this.Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageUpdated(EventArgs.Empty);
            }

            switch (this.SelectedTool)
            {
                case DrawingTools.PaintBucket:
                    this.PaintBucketFill(e);
                    break;
                case DrawingTools.Line:
                    this._linePoints.Enqueue(e.Location);
                    //this._drawLinePoints.Clear();
                    this.LineDraw(e);
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Overrides base to perform tool-based panning instead of auto-panning with the mouse
        /// </summary>
        /// <param name="e">
        ///   The <see cref="MouseEventArgs" /> instance containing the event data.
        /// </param>
        protected override void ProcessPanning(MouseEventArgs e)
        {
            if (this.SelectedTool == DrawingTools.Pan)
            {
                if (!this.ViewSize.IsEmpty && this.SelectionMode == ImageBoxSelectionMode.None)
                {
                    if (!this.IsPanning && (this.HScroll | this.VScroll))
                    {
                        this._startMousePosition = e.Location;
                        this.IsPanning = true;
                    }

                    if (this.IsPanning)
                    {
                        int x;
                        int y;
                        Point position;

                        if (!this.InvertMouse)
                        {
                            x = -_startScrollPosition.X + (_startMousePosition.X - e.Location.X);
                            y = -_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y);
                        }
                        else
                        {
                            x = -(_startScrollPosition.X + (_startMousePosition.X - e.Location.X));
                            y = -(_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y));
                        }

                        position = new Point(x, y);

                        this.UpdateScrollPosition(position);
                    }
                }
            }
        }

        #region Public Methods
        public virtual void ChangeToolMode(object sender, EventArgs e)
        {
            this.Focus();
            this.SelectedTool = (e as DrawingToolSelectedEventArgs).SelectedTool;
            this.ChangeDrawingToolCursor(this.SelectedTool);
        }
        #endregion

        #region Drawing Methods
        protected virtual void LineDraw(MouseEventArgs e)
        {
            this.IsDrawing = true;

            Color col;

            switch(e.Button)
            {
                case MouseButtons.Left:
                    col = this.ActivePrimaryColor;
                    break;
                case MouseButtons.Right:
                    col = this.ActiveSecondaryColor;
                    break;
                default:
                    col = this.ActivePrimaryColor;
                    break;
            }
            
            Point startPoint = Point.Empty;
            Point endPoint = Point.Empty;
           
            if (this._linePoints.Count > 1)
            {
                startPoint = this.PointToImage(this._linePoints.Dequeue());
                endPoint = this.PointToImage(this._linePoints.Dequeue());

                using (Graphics g = Graphics.FromImage(this.Image))
                {
                    using (Pen p = new Pen(col, 1))
                        g.DrawLine(p, startPoint, endPoint);
                }

                this._linePoints.Clear();
                this.Invalidate();
            }
        }
        protected virtual void PencilDraw(MouseEventArgs e)
        {
            this.IsDrawing = true;

            Point currentPixel = this.PointToImage(e.X, e.Y);

            if ((currentPixel.X < Image.Width && currentPixel.Y < Image.Height) && (currentPixel.X >= 0 && currentPixel.Y >= 0))
            {
                this.IsDrawing = true;
                this.IsSelecting = false;
                this.IsPanning = false;

                Color col = Color.Empty;

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        col = this.ActivePrimaryColor;
                        break;
                    case MouseButtons.Right:
                        col = this.ActiveSecondaryColor;
                        break;
                }

                if (col != Color.Empty)
                {
                    this.fbmp.LockImage();
                    this.fbmp.SetPixel(currentPixel.X, currentPixel.Y, col);
                    this.fbmp.UnlockImage();
                    this.Invalidate(this.ViewPortRectangle);
                }
            }
        }
        protected virtual void PaintBucketFill(MouseEventArgs e)
        {        //todo: Debug.Assert(this.IsSelecting==false) and this.IsPanning...

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
                    Fill(this.Image as Bitmap, currentPoint, this.fbmp.GetPixel(currentPoint.X, currentPoint.Y), this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }

                this.Invalidate(this.ViewPortRectangle);
            }
        }
        private void Fill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Func<Color, Color, bool> ColorMatch = (a, b) => { return (a.ToArgb() & 0xffffffff) == (b.ToArgb() & 0xffffffff); };

            //original algorithm courtesy http://rosettacode.org/wiki/Bitmap/Flood_fill

            Queue<Point> q = new Queue<Point>();
            q.Enqueue(pt);

            while (q.Count > 0)
            {
                Point n = q.Dequeue();
                if (!ColorMatch(this.fbmp.GetPixel(n.X, n.Y), targetColor))
                    continue;
                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X >= 0) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y), targetColor))
                {
                    this.fbmp.SetPixel(w.X, w.Y, replacementColor);
                    if ((w.Y > 0) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y - 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < bmp.Height - 1) && ColorMatch(this.fbmp.GetPixel(w.X, w.Y + 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X <= bmp.Width - 1) && ColorMatch(this.fbmp.GetPixel(e.X, e.Y), targetColor))
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
        #endregion

        protected virtual void ChangeDrawingToolCursor(DrawingTools tool)
        {
            switch (tool)
            {
                case DrawingTools.PaintBucket:
                    this.Cursor = this._paintBucketCursor;
                    break;
                case DrawingTools.Pencil:
                    this.Cursor = this._pencilCursor;
                    break;
                case DrawingTools.Pan:
                    this.Cursor = this._panCursor;
                    break;
                case DrawingTools.Line:
                    this.Cursor = Cursors.Cross;
                    break;
                case DrawingTools.None:
                    this.Cursor = this.DefaultCursor;
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.AllowPainting)
            {
                // draw the background
                this.DrawBackground(e);

                // draw the image
                if (!this.ViewSize.IsEmpty)
                {
                    this.DrawImageBorder(e.Graphics);
                }
                if (this.VirtualMode)
                {
                    this.OnVirtualDraw(e);
                }
                else if (this.Image != null)
                {
                    this.DrawImage(e.Graphics);
                }

                // draw the grid
                if (this.ShowPixelGrid && !this.VirtualMode)
                {
                    this.DrawPixelGrid(e.Graphics);
                }

                // draw the selection
                if (this.SelectionRegion != Rectangle.Empty)
                {
                    this.DrawSelection(e);
                }

                // text
                if (!string.IsNullOrEmpty(this.Text) && this.TextDisplayMode != ImageBoxGridDisplayMode.None)
                {
                    this.DrawText(e);
                }

                //if (this._drawLinePoints.Count > 1)
                //{
                //    ////Image overDraw = new Bitmap(this.Image.Width, this.Image.Height);
                //    ////Point bmpOne = this.PointToImage(this._drawLinePoints[0]);
                //    ////Point bmpTwo = this.PointToImage(this._drawLinePoints[1]);
                //    ////RectangleF rect;
                //    ////e.Graphics.SetClip(this.GetInsideViewPort(true));
                //    ////rect = this.GetOffsetRectangle(this.SelectionRegion);
                //    ////using (Graphics g = Graphics.FromImage(overDraw))
                //    ////{
                //    ////    g.Clear(Color.FromArgb(0, 255, 255, 255));
                //    ////    using (Pen p = new Pen(this.ActivePrimaryColor, 1))
                //    ////        g.DrawLine(p, bmpOne, bmpTwo);
                //    ////    e.Graphics.DrawImage(overDraw, rect.Location);
                //    ////}
                //    e.Graphics.MultiplyTransform(ScaleM);
                //    using (Pen pen = new Pen(Color.Red, 10f))
                //    {
                //        PointF center = new PointF(this.ViewPortRectangle.Width / 2f,
                //                                   this.ViewPortRectangle.Height / 2f);
                //        center = new PointF((float)(center.X / this.ZoomFactor), (float) (center.Y / this.ZoomFactor));
                //        foreach (PointF pt in this._drawLinePoints)
                //        {

                //            using (SolidBrush brush = new SolidBrush(pen.Color))
                //            {
                //                float pw = pen.Width;
                //                float pr = pw / 2f;
                //                e.Graphics.FillEllipse(brush, new RectangleF(pt.X - pr, pt.Y - pr, pw, pw));
                //            }
                //            e.Graphics.DrawLine(Pens.Yellow, center, pt);
                //        }
                //    }
                //    //using (Pen p = new Pen(this.ActivePrimaryColor, (int) (this.ZoomFactor / 2)))
                //    //    e.Graphics.DrawLine(p, this._drawLinePoints[0], this._drawLinePoints[1]);


                //    this._drawLinePoints.Remove(this._drawLinePoints[1]);
                //}

                base.OnPaint(e);
            }
        }
    }
}
