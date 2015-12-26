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
using WeifenLuo.WinFormsUI.Docking;
using Capslock.WinForms.ImageEditor;
using SkaaEditorUI.Presenters;
using System;
using System.Linq;

namespace SkaaEditorUI.Forms.DockPanels
{
    public partial class ImageEditorContainer : DockContent
    {
        private ImageEditorBox _imageEditorBox;
        private MultiImagePresenterBase _activeSprite;

        #region Events
        [NonSerialized]
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
        #endregion

        public MultiImagePresenterBase ActiveSprite
        {
            get
            {
                return _activeSprite;
            }

            private set
            {
                this._activeSprite = value;
                OnActiveSpriteChanged(EventArgs.Empty);
            }
        }

        public ImageEditorContainer()
        {
            InitializeComponent();
        }

        public void SetSprite(MultiImagePresenterBase spr, int activeFrameIndex = 0)
        {
            this.ActiveSprite = spr;
            this.ActiveSprite?.SetActiveFrame(activeFrameIndex);
            this._imageEditorBox.Image = spr?.ActiveFrame?.Bitmap;
        }

        private void InitializeComponent()
        {
            this._imageEditorBox = new Capslock.WinForms.ImageEditor.ImageEditorBox();
            this.SuspendLayout();
            // 
            // imageEditorBox
            // 
            this._imageEditorBox.ActivePrimaryColor = System.Drawing.Color.Empty;
            this._imageEditorBox.ActiveSecondaryColor = System.Drawing.Color.Empty;
            this._imageEditorBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right);
            this._imageEditorBox.AutoPan = false;
            this._imageEditorBox.Font = new System.Drawing.Font("Calibri", 12.75F);
            this._imageEditorBox.GridCellSize = 12;
            this._imageEditorBox.Location = new System.Drawing.Point(1, 1);
            this._imageEditorBox.Name = "imageEditorBox";
            this._imageEditorBox.Size = new System.Drawing.Size(778, 777);
            this._imageEditorBox.TabIndex = 9;
            // 
            // ImageEditorContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 778);
            this.Controls.Add(this._imageEditorBox);
            this.Name = "ImageEditorContainer";
            this.Text = "ImageEditorContainer";
            this.ResumeLayout(false);

        }
    }
}
