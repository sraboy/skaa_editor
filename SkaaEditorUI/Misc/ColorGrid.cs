using System;
using System.Drawing;
using System.Windows.Forms;
using Cyotek.Windows.Forms;

namespace SkaaEditorUI.Misc
{
    public partial class ColorGrid : Cyotek.Windows.Forms.ColorGrid
    {
        private Color _activePrimaryColor;
        private Color _activeSecondaryColor;

        public Color ActiveSecondaryColor
        {
            get { return this._activeSecondaryColor; }
            set
            {
                int newIndex;

                this._activeSecondaryColor = value;

                if (!value.IsEmpty)
                {
                    // the new color matches the color at the current index, so don't change the index
                    // this stops the selection hopping about if you have duplicate colors in a palette
                    // otherwise, if the colors don't match, then find the index that does
                    newIndex = this.GetColor(this.ColorIndex) == value ? this.ColorIndex : this.GetColorIndex(value);

                    if (newIndex == InvalidIndex)
                    {
                        newIndex = this.AddCustomColor(value);
                    }
                }
                else
                {
                    newIndex = InvalidIndex;
                }

                this.ColorIndex = newIndex;

                this.OnColorChanged(EventArgs.Empty);
            }
        }
        public Color ActivePrimaryColor
        {
            get { return this._activePrimaryColor; }
            set
            {
                int newIndex;

                this._activePrimaryColor = value;

                if (!value.IsEmpty)
                {
                    // the new color matches the color at the current index, so don't change the index
                    // this stops the selection hopping about if you have duplicate colors in a palette
                    // otherwise, if the colors don't match, then find the index that does
                    newIndex = this.GetColor(this.ColorIndex) == value ? this.ColorIndex : this.GetColorIndex(value);

                    if (newIndex == InvalidIndex)
                    {
                        newIndex = this.AddCustomColor(value);
                    }
                }
                else
                {
                    newIndex = InvalidIndex;
                }

                this.ColorIndex = newIndex;

                this.OnColorChanged(EventArgs.Empty);
            }
        }

        protected override void ProcessMouseClick(MouseEventArgs e)
        {
            ColorHitTestInfo hitTest;
            if (e.Button == MouseButtons.Left)
            {
                hitTest = this.HitTest(e.Location);

                if (hitTest.Source != ColorSource.None)
                {
                    this.ActivePrimaryColor = hitTest.Color;
                    this.ColorIndex = hitTest.Index;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                hitTest = this.HitTest(e.Location);

                if (hitTest.Source != ColorSource.None)
                {
                    this.ActiveSecondaryColor = hitTest.Color;
                    this.ColorIndex = hitTest.Index;
                }
            }
        }
    }
}
