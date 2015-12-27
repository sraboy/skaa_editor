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
using Capslock.WinForms.SpriteViewer;
using SkaaEditorUI.Presenters;
using System;
using System.Linq;
using TrulyObservableCollection;

namespace SkaaEditorUI.Forms.DockPanels
{
    public partial class SpriteViewerContainer : DockContent
    {
        private SpriteView spriteViewer;
        private MultiImagePresenterBase _activeSprite;

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
        #endregion

        public MultiImagePresenterBase ActiveSprite
        {
            get
            {
                return _activeSprite;
            }

            set
            {
                this._activeSprite = value;
                OnActiveSpriteChanged(EventArgs.Empty);
            }
        }

        public SpriteViewerContainer()
        {
            InitializeComponent();
        }

        //public void UpdateFrame(IFrame frame)
        //{
        //    var f = this.ActiveSprite.Frames.
        //}

        public void SetSprite(MultiImagePresenterBase spr, int activeFrameIndex = 0)
        {
            this.ActiveSprite = spr;
            this.ActiveSprite?.SetActiveFrame(activeFrameIndex);
            this.spriteViewer.SetFrameList(this.ActiveSprite.Frames);
        }

        private void InitializeComponent()
        {
            this.spriteViewer = new Capslock.WinForms.SpriteViewer.SpriteView();
            this.SuspendLayout();
            // 
            // spriteViewer
            // 
            this.spriteViewer.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right);
            this.spriteViewer.Location = new System.Drawing.Point(1, 0);
            this.spriteViewer.Name = "spriteViewer1";
            this.spriteViewer.Size = new System.Drawing.Size(316, 830);
            this.spriteViewer.MinimumSize = new System.Drawing.Size(316, 830);
            this.spriteViewer.TabIndex = 20;
            // 
            // SpriteViewerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 828);
            this.Controls.Add(this.spriteViewer);
            this.Name = "SpriteViewerContainer";
            this.Text = "SpriteViewerContainer";
            this.ResumeLayout(false);
        }
    }
}
