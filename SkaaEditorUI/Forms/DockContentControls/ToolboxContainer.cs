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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Capslock.Windows.Forms.ImageEditor;
using Cyotek.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockContentControls
{
    public partial class ToolboxContainer : DockContent
    {
        #region Events (Exposed From Child Controls)
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
                this._colorGrid.ColorChanged += value;
            }
            remove
            {
                this._colorGrid.ColorChanged -= value;
            }
        }
        #endregion

        #region Private Fields
        private System.Drawing.Imaging.ColorPalette _activePalette;
        #endregion

        #region Public Properties
        public System.Drawing.Imaging.ColorPalette ActivePalette
        {
            get
            {
                return this._activePalette;
            }
        }
        /// <summary>
        /// The method <see cref="DrawingToolbox"/> will call when resizing an image 
        /// with the options set in <see cref="ResizeImageDialog"/>
        /// </summary>
        public Action<int, int, bool> ResizeImageDelegate
        {
            get
            {
                return this._drawingToolbox.ResizeImageDelegate;
            }
            set
            {
                this._drawingToolbox.ResizeImageDelegate = value;
            }
        }
        #endregion

        #region Constructor
        public ToolboxContainer()
        {
            InitializeComponent();

            //Turns off the empty boxes at the bottom of the grid
            this._colorGrid.ShowCustomColors = false;

            //The palette is set to "Standard256" for the sake of designer showing 
            //256 colors. At runtime, we want it set to None so the user doesn't 
            //see a palette we may not support.
            this._colorGrid.Palette = ColorPalette.None;

            this.Enabled = false;
            SetPalette(null);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The original <see cref="Image"/> that will be resized by <see cref="DrawingToolbox.ResizeImageDelegate"/>.
        /// Its properties are used, for example, to populate the <see cref="ResizeImageDialog"/> with default values.
        /// </summary>
        /// <param name="ImageToEdit">The image from which to gather properties to expose in various UI elements</param>
        public void SetImageToEdit(Image ImageToEdit)
        {
            DrawingToolbox.ImageToEdit = ImageToEdit;
            ToggleEnable();
        }
        public void SetPalette(System.Drawing.Imaging.ColorPalette pal)
        {
            this._activePalette = pal;

            if (pal != null)
            {
                IEnumerable<Color> distinct = pal.Entries.Distinct();
                this._colorGrid.Colors = new ColorCollection(distinct);
                this._colorGrid.Colors.Sort(ColorCollectionSortOrder.Value);
            }
            else
            {
                this._colorGrid.Colors.Clear();
                this._colorGrid.Palette = ColorPalette.None;
            }

            ToggleEnable();
            //this._colorGridChooser.Refresh();
        }
        #endregion

        #region Private Methods
        private void ToggleEnable()
        {
            this.Enabled =
                (DrawingToolbox.ImageToEdit == null &&
                this._activePalette == null) ?
                false :
                true;

            this._colorGrid.Enabled = this.Enabled;
            this._drawingToolbox.Enabled = this.Enabled;
        }
        #endregion
    }
}
