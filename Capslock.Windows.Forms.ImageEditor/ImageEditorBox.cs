﻿#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
* Copyright © 2013-2015 Cyotek Ltd.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BitmapProcessing;
using Cyotek.Windows.Forms;

namespace Capslock.Windows.Forms.ImageEditor
{
    [DefaultProperty("Image")]
    [ToolboxBitmap(typeof(ImageBox), "ImageBox.bmp")]
    [ToolboxItem(true)]
    public partial class ImageEditorBox : ImageBox
    {
        //todo: with a _bitmap var, expose Image as a Bitmap to eliminate the casting that goes on with all the drawing
        #region Private Fields
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
        public override Image Image
        {
            get
            {
                return base.Image;
            }

            set
            {
                //if (!FastBitmapComparer.Compare((base.Image as Bitmap), (value as Bitmap)))
                if (!Utils.Compare(base.Image, value))
                {
                    base.Image = value;
                    OnImageChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises the <see cref="Cyotek.Windows.Forms.ImageBox.ImageChanged" /> event. Overridden to use <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnImageChanged(EventArgs e)
        {
            this.fbmp = new FastBitmap((Bitmap)this.Image);
            base.OnImageChanged(e);
        }
        #endregion

        #region Constructor
        public ImageEditorBox() : base()
        {
            //have to load these dynamically since they're not just black/white
#if DEBUG
            var str = this.GetType().Assembly.GetManifestResourceNames();
#endif
            this._panCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PanToolCursor.cur")));
            this._pencilCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PencilToolCursor.cur")));
            this._paintBucketCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PaintBucketToolCursor.cur")));
            this._linePoints = new Queue<Point>();

            this.AutoPan = false; //the base constructor sets this to true
        }
        #endregion

        #region Mouse Events
        /// <summary>
        /// Calls the base class' <see cref="Control.OnMouseClick(MouseEventArgs)"/> 
        /// event after any drawing action is completed, based on <see cref="SelectedTool"/>.
        /// <see cref="DrawingTools.Pencil"/> is the only tool that functions with just a click,
        /// by only filling in the single pixel.
        /// </summary>
        /// <remarks>
        /// Other <see cref="DrawingTools"/> are based on multiple clicks, mouse movement or MouseUp.
        /// For example, <see cref="DrawingTools.PaintBucket"/> requires 
        /// <see cref="OnMouseUp(MouseEventArgs)"/> so that the user can hold the mouse button down 
        /// and then adjust to ensure they paint the proper area.
        /// </remarks>
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
            if (!this.IsDrawing)
                base.OnMouseDown(e);

            switch (this.SelectedTool)
            {
                case DrawingTools.Line:
                    this._linePoints.Enqueue(e.Location);
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
            //todo: Implement a real-time drawn line for the line tool. It should match the zoom level and pixelation of the image.
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

            switch (this.SelectedTool)
            {
                case DrawingTools.PaintBucket:
                    this.PaintBucketFill(e);
                    break;
                case DrawingTools.Line:
                    this._linePoints.Enqueue(e.Location);
                    this.LineDraw(e);
                    break;
            }

            // Each of the drawing methods below sets this.IsDrawing = true. We only
            // want OnMouseUp() raising the OnImageChanged event if we were drawing and
            // not just because the user clicked somewhere; therefore, those methods do
            // not set this.IsDrawing = false when they're done.
            //
            // This is a bit dirty but, so far, it works.
            // todo: Abstract out a Draw() method, passing in a delegate to the actual drawing method
            // That way, Draw() can set IsDrawing then raise OnImageChanged itself. It'll be a lot 
            // cleaner than worrying about where it's set within which methods. Also, Draw() can handle
            // calling Invalidate(). The drawing methods should simply manipulate the image; though LineDraw
            // will still need to Invalidate the control once the real-time line is drawn.
            if (this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageChanged(EventArgs.Empty);
            }
        }
        #endregion

        /// <summary>
        /// Overrides base to perform tool-based panning instead of auto-panning with the mouse
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseEventArgs" /> instance containing the event data.
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

            switch (e.Button)
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
        {
            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

            Point currentPoint = this.PointToImage(e.X, e.Y);

            if ((currentPoint.X < Image.Width && currentPoint.Y < Image.Height) && (currentPoint.X >= 0 && currentPoint.Y >= 0))
            {
                if (e.Button == MouseButtons.Left) //todo: implement Right-click for secondary color
                {
                    this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage(); //note: if using the unsafe Bitmap comparer and Fill were changed to return the edited image, set this.Image after fbmp.UnlockImage()
                    Fill(this.Image as Bitmap, currentPoint, this.fbmp.GetPixel(currentPoint.X, currentPoint.Y), this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }

                this.Invalidate(this.ViewPortRectangle);
            }
        }

        private void Fill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Func<Color, Color, bool> ColorMatch = (a, b) => { return (a.ToArgb() & 0xffffffff) == (b.ToArgb() & 0xffffffff); };

            //Some changes made, but the original algorithm is courtesy of http://rosettacode.org/wiki/Bitmap/Flood_fill

            Queue<Point> q = new Queue<Point>();
            q.Enqueue(pt);

            while (q.Count > 0)
            {
                if (q.Count > (bmp.Width - 1) * (bmp.Height - 1))
                    Debugger.Break();

                Point n = q.Dequeue();
                if (ColorMatch(this.fbmp.GetPixel(n.X, n.Y), replacementColor))
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