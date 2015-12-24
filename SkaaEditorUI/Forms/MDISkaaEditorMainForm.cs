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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaEditorUI.Forms.DockPanels;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib;
using WeifenLuo.WinFormsUI.Docking;
using static SkaaEditorUI.SProjectManager;

namespace SkaaEditorUI.Forms
{
    public partial class MDISkaaEditorMainForm : Form
    {
        private SpritePresenter _activeSprite;
        private ToolboxContainer _toolBoxContainer;
        private SpriteViewerContainer _spriteViewerContainer;

        public ColorPalette ActivePalette
        {
            get
            {
                return this.ActiveSprite?.ActiveFrame?.Bitmap.Palette;
            }
        }
        public SpritePresenter ActiveSprite
        {
            get
            {
                return _activeSprite;
            }

            set
            {
                this._activeSprite = value;
            }
        }

        public MDISkaaEditorMainForm()
        {
            InitializeComponent();
            SetUpDockPanel();
            ProjectManager.SetMainForm(this);
            ProjectManager.ActiveProject.ActiveSpriteChanged += ActiveProject_ActiveSpriteChanged;
            ProjectManager.ActiveProject.PaletteChanged += ActiveProject_PaletteChanged;
        }



        private void SetUpDockPanel()
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            this._toolBoxContainer = new ToolboxContainer();
            this._spriteViewerContainer = new SpriteViewerContainer();

            this.dockPanel.ActiveDocumentChanged += DockPanel_ActiveDocumentChanged;

            iec.Show(dockPanel, DockState.Document);
            this._toolBoxContainer.Show(dockPanel, DockState.DockLeft);
            this._spriteViewerContainer.Show(dockPanel, DockState.DockRight);
        }

        public void SetPalette(ColorPalette pal)
        {
            this._toolBoxContainer.SetColorPalette(((ImageEditorContainer)this.dockPanel.ActiveDocument)?.ActiveSprite?.ActivePalette);
        }

        /// <summary>
        /// Closes the current project and saves changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        private bool TrySaveCloseProject()
        {
            DialogResult saveChanges = UserShouldSaveChanges();

            if (saveChanges == DialogResult.Yes)
            {
                ProjectManager.CloseProject();
                return true;
            }
            else if (saveChanges == DialogResult.No)
            {
                ProjectManager.CloseProject();
                return true;
            }
            else // (DialogResult.Cancel)
                return false;
        }

        private DialogResult UserShouldSaveChanges()
        {
            bool spriteHasChanges = false;//CheckSpriteForPendingChanges(this.ActiveProject?.ActiveSprite);

            if (!spriteHasChanges)// && this.ActiveProject?.UnsavedSprites?.Count == 0)
                return DialogResult.No;
            else
                return MessageBox.Show("You have unsaved changes. Do you want to save these changes?", "Save?", MessageBoxButtons.YesNoCancel);
        }

        #region Event Handlers
        private void toolStripBtnOpenProject_Click(object sender, EventArgs e)
        {

        }
        private void openSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Open<SpritePresenter>();
            ProjectManager.OpenSprite();
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.OpenPalette();
        }
        private void ActiveProject_PaletteChanged(object sender, EventArgs e) => SetPalette(this.ActivePalette);
        private void ActiveProject_ActiveSpriteChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            SetPalette(((ImageEditorContainer)this.dockPanel.ActiveDocument)?.ActiveSprite?.ActivePalette);
        }
        private void toolStripBtnNewProject_Click(object sender, EventArgs e)
        {
            if (TrySaveCloseProject())
                ProjectManager.CreateNewProject();
        }
        #endregion
    }
}
