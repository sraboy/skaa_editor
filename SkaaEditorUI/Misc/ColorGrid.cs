#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
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
