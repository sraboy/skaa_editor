#region Copyright Notice
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
        #region Private Fields
        private static readonly string ClipboardFormat = System.Drawing.Imaging.ImageFormat.Png.ToString();//"PortableNetworkGraphics";

        private Bitmap _dragTempSelectedImage;
        private Image _layerOneImage;
        private Color _activePrimaryColor;
        private Color _activeSecondaryColor;
        private FastBitmap fbmp;
        private DrawingTools _selectedTool;
        private Cursor _panCursor;
        private Cursor _pencilCursor;
        private Cursor _paintBucketCursor;
        private Point _startMousePosition;
        private Point _dragStartAnchor;
        private Point _startScrollPosition;
        private Queue<Point> _linePoints;
        private bool _isPanning;
        private bool _snapSelectionToGrid;
        private bool _isDrawing = false;
        private bool _isDragging = false;
        #endregion

        #region Public Properties
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
            get { return this._selectedTool; }
            protected set
            {
                if (this._selectedTool != value)
                {
                    this._selectedTool = value;

                    if (this._selectedTool == DrawingTools.SelectRectangle)
                        this.SelectionMode = ImageBoxSelectionMode.Rectangle;
                }

                if (this._selectedTool == DrawingTools.None)
                {
                    SelectNone();
                    this._selectedTool = DrawingTools.None;
                    this.SelectionMode = ImageBoxSelectionMode.None;
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
        public bool SnapSelectionToGrid
        {
            get
            {
                return _snapSelectionToGrid;
            }

            set
            {
                this._snapSelectionToGrid = value;
            }
        }
        public bool IsDragging
        {
            get
            {
                return this._isDragging;
            }

            set
            {
                this._isDragging = value;
            }
        }
        public bool IsDrawing
        {
            get
            {
                return this._isDrawing;
            }

            set
            {
                this._isDrawing = value;
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
            //have to load these dynamically since they're not monotone, which is all that is normally supported
#if DEBUG
            var str = this.GetType().Assembly.GetManifestResourceNames();
#endif
            this._panCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PanToolCursor.cur")));
            this._pencilCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PencilToolCursor.cur")));
            this._paintBucketCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.Cursors.PaintBucketToolCursor.cur")));

            this._linePoints = new Queue<Point>();          //for drawing a line
            this.AutoPan = false;                           //the base constructor sets this to true but we have a tool for panning
            this.SnapSelectionToGrid = true;                //dragging to select rectangles will snap to the pixel grid, which helps in capturing specific areas while zoomed in
            this.KeyDown += ImageEditorBox_KeyDown;
        }
        #endregion

        #region Mouse & Keyboard Events
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
                case DrawingTools.SelectRectangle:
                    if (e.Button == MouseButtons.Right && !IsPointInSelectionRegion(e.Location))
                    {
                        this.SelectionRegion = RectangleF.Empty;
                        //remove the old saved copy of our background image
                        this._layerOneImage = null;
                    }
                    break;
            }

            base.OnMouseClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.IsDrawing)
                base.OnMouseDown(e);

            this._startMousePosition = e.Location; //used for panning and for a location to paste an image

            switch (this.SelectedTool)
            {
                case DrawingTools.Line:
                    this._linePoints.Enqueue(e.Location);
                    break;
                case DrawingTools.Pencil:
                    this.PencilDraw(e);
                    break;
                case DrawingTools.SelectRectangle:
                    if (!this.IsSelecting
                        && !this.IsDragging
                        && this.SelectionRegion != RectangleF.Empty
                        && IsPointInSelectionRegion(PointToImage(e.Location)))
                    {
                        this.IsDragging = true;
                        //save this so we can click & drag using this point as the anchor
                        this._startMousePosition = new Point((int)this.SelectionRegion.X, (int)this.SelectionRegion.Y);
                        this._dragStartAnchor = PointToImage(e.Location);
                        //save the selected image
                        this._dragTempSelectedImage = GetSelectedImage() as Bitmap;
                        //cut out the selection
                        CutRectangleFromImage(this.SelectionRegion.Location, this.SelectionRegion.Size);
                        //save the cut image for drawing the background while dragging
                        this._layerOneImage = this._layerOneImage ?? this.Image.Clone() as Image;
                    }
                    break;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //todo: Implement a real-time drawn line for the line tool. It should match the zoom level and pixelation of the image.
            base.OnMouseMove(e);

            if (this.IsDragging)
                this.IsSelecting = false; //because base.OnMouseMove() will end up resetting it to true in StartDrag()

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                switch (this.SelectedTool)
                {
                    case DrawingTools.Pencil:
                        this.PencilDraw(e);
                        break;
                    case DrawingTools.SelectRectangle:
                        if (this.IsDragging)
                        {
                            var curPos = PointToImage(e.Location);
                            Point offset = new Point();
                            offset.X = this._startMousePosition.X + (curPos.X - this._dragStartAnchor.X);
                            offset.Y = this._startMousePosition.Y + (curPos.Y - this._dragStartAnchor.Y);

                            //draw selected image as mouse moves
                            OverlayBitmap(this._dragTempSelectedImage, offset);
                        }
                        else if (this.SnapSelectionToGrid && e.Button == MouseButtons.Left)
                        {
                            //remove the old saved copy of our background image
                            this._layerOneImage = null;

                            if (this.IsSelecting)
                            {
                                if (this.SelectionRegion != RectangleF.Empty)
                                {
                                    var oldRegion = this.SelectionRegion;
                                    this.SelectionRegion = new RectangleF(
                                        (int)oldRegion.X,
                                        (int)oldRegion.Y,
                                        (int)oldRegion.Width + 1,
                                        (int)oldRegion.Height + 1);
                                }
                            }
                        }
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
            if (this.IsDragging)
            {
                var curPos = PointToImage(e.Location);
                Point offset = new Point();
                offset.X = this._startMousePosition.X + (curPos.X - this._dragStartAnchor.X);
                offset.Y = this._startMousePosition.Y + (curPos.Y - this._dragStartAnchor.Y);
                this.SelectionRegion = new RectangleF(offset, this._dragTempSelectedImage.Size);
                this.IsDragging = false;
                this._dragTempSelectedImage = null;
                OnImageChanged(EventArgs.Empty);
            }

            if (this.IsDrawing)
            {
                this.IsDrawing = false;
                OnImageChanged(EventArgs.Empty);
            }
        }
        protected virtual void ImageEditorBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C) &&     //Ctrl-C for copy
                this.SelectionRegion != RectangleF.Empty && //The user has selected a region (which also means SelectedTool is SelectRectangle)
                this.LimitSelectionToImage == true)         //True by default in the base constructor. We don't want to copy stuff outside the image because it may not match the palette.
            {
                PutSelectionToClipboard();
            }
            else if (e.KeyData == (Keys.Control | Keys.X) && //Ctrl-X for cut
                     this.SelectionRegion != RectangleF.Empty &&
                     this.LimitSelectionToImage == true)
            {
                CutSelectionToClipboard();
            }
            else if (e.KeyData == (Keys.Control | Keys.V) && //Ctrl-V for paste
                     Clipboard.ContainsData(ClipboardFormat) &&
                     this.IsPointInImage(this._startMousePosition))
            {
                PasteImageFromClipboard();
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
            var newTool = (e as DrawingToolSelectedEventArgs).SelectedTool;
            this.SelectedTool = newTool;
            this.ChangeDrawingToolCursor(this.SelectedTool);
        }
        public virtual void Resize(int NewWidth, int NewHeight, bool MaintainAspectRatio)
        {
            if (MaintainAspectRatio)
                this.Image = ResizeAndScale(this.Image, NewWidth);
            else
                this.Image = ResizeAndCrop(this.Image, NewWidth, NewHeight);

        }
        #endregion

        #region Editing Methods
        protected virtual void LineDraw(MouseEventArgs e)
        {
            this.IsDrawing = true;
            this.IsSelecting = false;
            this.IsPanning = false;

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
                this._isDrawing = true;
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
            this._isDrawing = true;
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
        protected virtual void Fill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
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
        protected static Image ResizeAndCrop(Image original, int width, int height)
        {
            Bitmap cropped = new Bitmap(width, height, original.PixelFormat);
            cropped.Palette = original.Palette;

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(original,
                            new Rectangle(0, 0, cropped.Width, cropped.Height),
                            new Rectangle(0, 0, cropped.Width, cropped.Height),
                            GraphicsUnit.Pixel);
            }

            return cropped;
        }
        protected static Image ResizeAndScale(Image original, int width)
        {
            double div = ((double)original.Width / (double)original.Height);
            double targetHeight = Convert.ToDouble(width) / div;

            Bitmap scaled = new Bitmap(width, (int)targetHeight, original.PixelFormat);
            scaled.Palette = original.Palette;

            using (Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                g.DrawImage(original,
                            new Rectangle(0, 0, scaled.Width, scaled.Height),
                            new Rectangle(0, 0, original.Width, original.Height),
                            GraphicsUnit.Pixel);
            }

            return scaled;
        }

        private void PutSelectionToClipboard()
        {
            //We can't use SetImage() because the alpha channel is lost
            //so we use a DataObject that just stores the bits as-is
            var obj = new DataObject();
            var bmp = GetSelectedImage();

            //We also specify our own format or Windows will bogart
            //the object and save it as a Bitmap with no transparency.
            obj.SetData(ClipboardFormat, bmp);

            Clipboard.Clear();
            Clipboard.SetDataObject(obj);
        }
        private void PasteImageFromClipboard()
        {
            if (Clipboard.ContainsData(ClipboardFormat))
            {
                var bmp = Clipboard.GetData(ClipboardFormat) as Bitmap;
                var location = this.PointToImage(_startMousePosition);

                //saves our background image so we can drag this part around
                this._layerOneImage = this.Image.Clone() as Image;

                OverlayBitmap(bmp, location);
                this.SelectionRegion = new RectangleF(location, bmp.Size);
            }
        }
        private void CutSelectionToClipboard()
        {
            PutSelectionToClipboard();
            CutRectangleFromImage(this.SelectionRegion.Location, this.SelectionRegion.Size);
        }
        /// <summary>
        /// Draws the specified bitmap on top of the current image
        /// </summary>
        /// <param name="bmp">The new bitmap to draw</param>
        /// <param name="location">The top-left point, in image coordinates, at which to start drawing</param>
        private void OverlayBitmap(Bitmap bmp, Point location)
        {
            if (this.IsDragging)
                this.Image = this._layerOneImage.Clone() as Image;

            Bitmap final = new Bitmap(Math.Max(bmp.Width, this.Image.Width), Math.Max(bmp.Height, this.Image.Height), this.Image.PixelFormat);

            using (Graphics g = Graphics.FromImage(final))
            {
                g.DrawImage(this.Image, 0, 0);
                g.DrawImage(bmp, location);
            }

            this.Image = final;
        }
        private void CutRectangleFromImage(PointF location, SizeF size)
        {
            Bitmap final = new Bitmap(this.Image.Width, this.Image.Height, this.Image.PixelFormat);

            using (Graphics g = Graphics.FromImage(this.Image))
            {
                //todo: should use a color KNOWN not to be in the palette
                //with this, we'll end up making any other Aqua pixels transparent

                using (var br = new SolidBrush(Color.Aqua))
                {
                    g.FillRectangle(br, new RectangleF(location, size));
                    (this.Image as Bitmap).MakeTransparent(Color.Aqua);
                }
            }

            this.Refresh();
        }
        #endregion

        protected virtual bool IsPointInSelectionRegion(Point p)
        {
            bool inX = false;
            bool inY = false;

            if (p.X > this.SelectionRegion.X && p.X < (this.SelectionRegion.X + this.SelectionRegion.Width))
                inX = true;
            if (p.Y > this.SelectionRegion.Y && p.Y < (this.SelectionRegion.Y + this.SelectionRegion.Height))
                inY = true;

            return inX & inY;
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
                default:
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
