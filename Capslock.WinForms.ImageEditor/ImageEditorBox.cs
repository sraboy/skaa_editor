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
        private DrawingTools _toolMode;
        private Stream _panToolCursorStream;
        private Stream _pencilToolCursorStream;
        private Stream _paintBucketToolCursorStream;
        private Cursor _panCursor;
        private Cursor _pencilCursor;
        private Cursor _paintBucketCursor;
        private Queue<Point> _linePoints;
        #endregion

        [DefaultValue("false")]
        [Category("Behavior")]
        protected virtual bool IsDrawing { get; set; }
 
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
            //this.bmpWidth = bmp.Width;
            //this.bmpHeight = bmp.Height;

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
        }
        #endregion

        #region Overidden Mouse Events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(!this.IsDrawing)
                base.OnMouseDown(e);

            if (this.SelectedTool == DrawingTools.Line)
                this._linePoints.Enqueue(e.Location); //this.LineDraw(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.IsDrawing && e.Button == MouseButtons.Left && this.SelectedTool == DrawingTools.Pan)
                this.IsPanning = true;

            if (!this.IsDrawing)
                base.OnMouseMove(e);

            //if (this.SelectedTool == DrawingTools.Line)
            //    this._linePoints.Add(e.Location);//this.LineDraw(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageUpdated(EventArgs.Empty);
            }

            switch (this.SelectedTool)
            {
                case DrawingTools.PaintBucket:
                    this.BeginPaintBucketFill(e);
                    break;
                case DrawingTools.Pencil:
                    this.PencilDraw(e);
                    break;
                case DrawingTools.Line:
                    //this.LineDraw(e);
                    this._linePoints.Enqueue(e.Location);
                    this.LineDraw(e);
                    break;
            }

            base.OnMouseUp(e);
        }
        #endregion

        
        protected virtual void BeginPaintBucketFill(MouseEventArgs e)
        {        //todo: Debug.Assert(this.IsSelecting==false) and this.IsPanning...

            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

            Point currentPoint = this.PointToImage(e.X, e.Y);

            if ((currentPoint.X < Image.Width && currentPoint.Y < Image.Height) && (currentPoint.X >= 0 && currentPoint.Y >= 0))
            {
                if (e.Button == MouseButtons.Left)
                {
                    //this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    PaintBucketFill(this.Image as Bitmap, currentPoint, this.fbmp.GetPixel(currentPoint.X, currentPoint.Y), this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }

                this.Invalidate(this.ViewPortRectangle);
            }
        }
        protected virtual void LineDraw(MouseEventArgs e)
        {        //todo: Debug.Assert(this.IsSelecting==false) and this.IsPanning...
            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

            if (this._linePoints.Count > 1)
            {
                using (Graphics g = Graphics.FromImage(this.Image))
                {
                    using (Pen p = new Pen(this.ActivePrimaryColor, 1))
                        g.DrawLine(p, _linePoints.Dequeue(), _linePoints.Dequeue());
                }

                this.Invalidate();
            }
            else
                this._linePoints.Clear();
            //this._linePoints.Add(e.Location);
        }
        protected virtual void PencilDraw(MouseEventArgs e)
        {        //todo: Debug.Assert(this.IsSelecting==false) and this.IsPanning...

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
                    //this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    this.fbmp.SetPixel(currentPixel.X, currentPixel.Y, this.ActivePrimaryColor);
                    this.fbmp.UnlockImage();
                }
                if (e.Button == MouseButtons.Right)
                {
                    //replaced slow SetPixel with FastBitmap from BitmapProcessing lib
                    //(this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, this._skaaTransparentColor);
                    //this.fbmp = new FastBitmap(this.Image as Bitmap);
                    this.fbmp.LockImage();
                    this.fbmp.SetPixel(currentPixel.X, currentPixel.Y, this.ActiveSecondaryColor);
                    this.fbmp.UnlockImage();
                }
                
                this.Invalidate(this.ViewPortRectangle);
            }
        }

        public virtual void ChangeToolMode(object sender, EventArgs e)//ToolModes vm)
        {
            this.Focus();
            this.SelectedTool = (e as DrawingToolSelectedEventArgs).SelectedTool;
            ChangeDrawingToolCursor(this.SelectedTool);
        }
        protected virtual void PaintBucketFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Func<Color, Color, bool> ColorMatch = (a, b) => { return (a.ToArgb() & 0xffffffff) == (b.ToArgb() & 0xffffffff); };

            //mostly courtesy http://rosettacode.org/wiki/Bitmap/Flood_fill
            //some off-by-one errors: (w.X >= 0) and (e.X <= bmp.Width - 1) were only > and <

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
            //if (this.SelectedTool == DrawingTools.Line && this._linePoints.Count > 1)
            //{
            //    using (Graphics g = Graphics.FromImage(this.Image))
            //    {
            //        using (Pen p = new Pen(this.ActivePrimaryColor, 1))
            //            g.DrawLine(p, _linePoints.Dequeue(), _linePoints.Dequeue());

            //        g.Save();
            //    }

            //    this._linePoints.Clear();
            //    //e.Graphics.DrawLines(Pens.CadetBlue, this._linePoints.ToArray());
            //}
            //else
                base.OnPaint(e);
        }
    }
}
