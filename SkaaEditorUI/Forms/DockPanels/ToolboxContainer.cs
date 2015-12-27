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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Capslock.WinForms.ImageEditor;
using SkaaEditorControls;
using WeifenLuo.WinFormsUI.Docking;
using Cyotek.Windows.Forms;
using System;

namespace SkaaEditorUI.Forms.DockPanels
{
    public partial class ToolboxContainer : DockContent
    {
        private DrawingToolbox _drawingToolbox;
        private SkaaColorChooser _colorGridChooser;
        private System.Drawing.Imaging.ColorPalette _activePalette;

        public System.Drawing.Imaging.ColorPalette ActivePalette
        {
            get
            {
                return this._activePalette;
            }
        }
        public event EventHandler SelectedToolChanged
        {
            add
            {
                this._drawingToolbox.SelectedToolChanged += value;
            }
            remove
            {
                this._drawingToolbox.SelectedToolChanged -= value;
            }
        }
        public event EventHandler ColorChanged
        {
            add
            {
                this._colorGridChooser.ColorChanged += value;
            }
            remove
            {
                this._colorGridChooser.ColorChanged -= value;
            }
        }

        public ToolboxContainer()
        {
            Initialize();
            SetPalette(null);
        }

        private void Initialize(System.Drawing.Imaging.ColorPalette pal = null)
        {
            this._drawingToolbox = new DrawingToolbox();
            this._colorGridChooser = new SkaaColorChooser();
            this.SuspendLayout();
            // 
            // drawingToolbox
            // 
            this._drawingToolbox.Location = new System.Drawing.Point(1, 1);
            this._drawingToolbox.Margin = new System.Windows.Forms.Padding(2);
            this._drawingToolbox.Name = "drawingToolbox";
            this._drawingToolbox.Size = new System.Drawing.Size(175, 69);
            this._drawingToolbox.TabIndex = 18;
            // 
            // colorGridChooser
            // 
            this._colorGridChooser.AutoAddColors = false;
            this._colorGridChooser.CellSize = new System.Drawing.Size(18, 18);
            this._colorGridChooser.Columns = 8;
            this._colorGridChooser.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this._colorGridChooser.Location = new System.Drawing.Point(0, 76);
            this._colorGridChooser.Name = "colorGridChooser";
            this._colorGridChooser.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this._colorGridChooser.ShowCustomColors = false;
            this._colorGridChooser.Size = new System.Drawing.Size(175, 679);
            this._colorGridChooser.TabIndex = 17;
            // 
            // ToolboxContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 756);
            this.Controls.Add(this._drawingToolbox);
            this.Controls.Add(this._colorGridChooser);
            this.Name = "ToolboxContainer";
            this.Text = "ToolboxContainer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void SetPalette(System.Drawing.Imaging.ColorPalette pal)
        {
            this._activePalette = pal;

            if (pal != null)
            {
                IEnumerable<Color> distinct = pal.Entries.Distinct();
                this._colorGridChooser.Colors = new ColorCollection(distinct);
                this._colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Value);
                this._colorGridChooser.Enabled = true;
            }
            else
            {
                this._colorGridChooser.Colors.Clear();
                this._colorGridChooser.Palette = ColorPalette.None;
                this._colorGridChooser.Enabled = false;
            }

            this._colorGridChooser.Refresh();
        }

    }
}
