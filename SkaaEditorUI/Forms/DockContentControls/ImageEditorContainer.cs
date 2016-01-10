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
using System.Linq;
using Capslock.Windows.Forms.ImageEditor;
using SkaaEditorUI.Presenters;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockContentControls
{
    public partial class ImageEditorContainer : DockContent
    {
        #region Events
        private EventHandler _activeSpriteChanged;
        public event EventHandler ActiveSpriteChanged
        {
            add
            {
                if (_activeSpriteChanged == null || !_activeSpriteChanged.GetInvocationList().Contains(value))
                {
                    _activeSpriteChanged += value;
                }
            }
            remove
            {
                _activeSpriteChanged -= value;
            }
        }
        private void OnActiveSpriteChanged(EventArgs e)
        {
            EventHandler handler = _activeSpriteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private EventHandler _imageChanged;
        public event EventHandler ImageChanged
        {
            add
            {
                if (_imageChanged == null || !_imageChanged.GetInvocationList().Contains(value))
                {
                    _imageChanged += value;
                }
            }
            remove
            {
                _imageChanged -= value;
            }
        }
        private void OnImagedChanged(EventArgs e)
        {
            EventHandler handler = _imageChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Private Fields
        private MultiImagePresenterBase _activeSprite;
        private static Action<int, int, bool> _resizeImageMethod;
        #endregion

        #region Public Properties
        public MultiImagePresenterBase ActiveSprite
        {
            get
            {
                return _activeSprite;
            }

            private set
            {
                this._activeSprite = value;
                if (this._activeSprite != null)
                    this._activeSprite.ActiveFrameChanged += ActiveSprite_ActiveFrameChanged;
                OnActiveSpriteChanged(EventArgs.Empty);
            }
        }
        public Image Image //todo: Image should be immutable since edits are always done in ImageEditBox. Either return a clone, return a new IImage interface or separately expose the properties.
        {
            get
            {
                return this._imageEditorBox.Image;
            }
        }
        /// <summary>
        /// The method <see cref="DrawingToolbox"/> will call when resizing an image 
        /// with the options set in <see cref="ResizeImageDialog"/>
        /// </summary>
        public static Action<int, int, bool> ResizeImageMethod
        {
            get
            {
                return _resizeImageMethod;
            }

            private set
            {
                _resizeImageMethod = value;
            }
        }
        #endregion

        #region Constructor
        public ImageEditorContainer()
        {
            InitializeComponent();
            ResizeImageMethod = this._imageEditorBox.Resize;
            this._imageEditorBox.ShowPixelGrid = true;
            SetActiveColors(Color.Black, Color.FromArgb(0, 0, 0, 0));
        }
        #endregion

        #region Private Methods
        private void ImageEditorBox_ImageChanged(object sender, EventArgs e)
        {
            if (this._imageEditorBox.SelectedTool != DrawingTools.Pan &&
                this._imageEditorBox.SelectedTool != DrawingTools.None)
            {
                this.ActiveSprite.ActiveFrame.Bitmap = this._imageEditorBox.Image as Bitmap;
                OnImagedChanged(EventArgs.Empty);
            }
        }
        private void ActiveSprite_ActiveFrameChanged(object sender, EventArgs e)
        {
            this._imageEditorBox.Image = this.ActiveSprite.ActiveFrame.Bitmap;
        }
        #endregion

        #region Public Methods
        public void SetActiveColors(Color primary, Color secondary)
        {
            this._imageEditorBox.ActivePrimaryColor = primary;
            this._imageEditorBox.ActiveSecondaryColor = secondary;
        }
        public void SetSprite(MultiImagePresenterBase spr)
        {
            this.ActiveSprite = spr;
            this.TabText = this.ActiveSprite?.SpriteId ?? "New Sprite";
            this._imageEditorBox.Image = spr?.ActiveFrame?.Bitmap;
            this._imageEditorBox.ImageChanged += ImageEditorBox_ImageChanged;
        }
        public void ChangeToolMode(object sender, EventArgs e)
        {
            this._imageEditorBox.ChangeToolMode(sender, e);
        }
        #endregion
    }
}
