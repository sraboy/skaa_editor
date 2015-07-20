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
        private bool _editMode;// = false;
        private Color _activeColor;// = ;

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
                this.IsSelecting = false;
                this.IsPanning = false;

                Point currentPixel;
                currentPixel = this.PointToImage(e.X, e.Y);

                if ((currentPixel.X < Image.Width && currentPixel.Y < Image.Height) && (currentPixel.X >= 0 && currentPixel.Y >= 0))
                {
                    if (e.Button == MouseButtons.Left)
                        (this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, this.ActiveColor);
                    if (e.Button == MouseButtons.Right)
                        (this.Image as Bitmap).SetPixel(currentPixel.X, currentPixel.Y, Color.White);

                    this.Invalidate(this.ViewPortRectangle);
                    this.Update();
                }
            }
        }
    }
}
