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
using System.Diagnostics;
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
        // We should never instantiate these. They're only set based on the ProjectManager's ActiveSprite
        private static MultiImagePresenterBase _activeSprite;
        private static ColorPalettePresenter _activePalette;

        private ToolboxContainer _toolBoxContainer;
        private SpriteViewerContainer _spriteViewerContainer;

        public MDISkaaEditorMainForm()
        {
            InitializeComponent();
            SetUpDockPanel();
            ProjectManager.SetMainForm(this);
        }

        private void SetUpDockPanel()
        {
            this._toolBoxContainer = new ToolboxContainer();
            this._toolBoxContainer.HideOnClose = true;
            this._spriteViewerContainer = new SpriteViewerContainer();
            this._spriteViewerContainer.HideOnClose = true;

            this._dockPanel.ActiveDocumentChanged += DockPanel_ActiveDocumentChanged;
            _toolBoxContainer.Show(_dockPanel, DockState.DockLeft);
            _spriteViewerContainer.Show(_dockPanel, DockState.DockRight);
            OpenNewTab();
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

        private void OpenNewTab()
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            iec.Show(_dockPanel, DockState.Document);
            iec.ActiveSpriteChanged += ImageEditorContainer_ActiveSpriteChanged;
        }

        private void toolStripBtnNewProject_Click(object sender, EventArgs e)
        {
            if (TrySaveCloseProject())
                ProjectManager.CreateNewProject();
        }
        private void toolStripBtnOpenProject_Click(object sender, EventArgs e)
        {
            //Browse folders to find a project directory
            //Check all file types and load them
        }
        private void openSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSprite<SpritePresenter>( sender, e);
        }
        private void spriteResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSprite<ResIdxMultiBmpPresenter>(sender, e);
        }

        private void SetActiveSprite(MultiImagePresenterBase spr)
        {
            var iec = (ImageEditorContainer)this._dockPanel.ActiveDocument;// ?? new ImageEditorContainer();
            iec.SetSprite(spr);
        }

        private void OpenSprite<T>(object sender, EventArgs e) where T : MultiImagePresenterBase, new()
        {
            //check for a palette first
            //if not loaded, set it
            var pal = this._toolBoxContainer.ActivePalette;
            if (pal == null)
                loadPaletteToolStripMenuItem_Click(sender, e);

            T spr = (T)ProjectManager.Open<SkaaSprite, T>(FileFormats.SpriteSpr, ProjectManager.ActiveProject.GameSet);

            if (spr == null) //user canceled or loading failed
                return;

            if (spr.Frames.Count > 0)
            {
                var doc = ((ImageEditorContainer)this._dockPanel.ActiveDocument);

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }
        }

        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pal = (ColorPalettePresenter)ProjectManager.Open<ColorPalette, ColorPalettePresenter>(FileFormats.Palette);
            _toolBoxContainer.SetPalette(pal.GameObject);
        }

        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            //get the palette for the currently-loaded sprite, if any
            var iec = (ImageEditorContainer)this._dockPanel.ActiveDocument;
            var pal = iec?.ActiveSprite?.PalettePresenter;

            if (pal != null)
                this._toolBoxContainer.SetPalette(pal.GameObject);
        }
        /// <summary>
        /// Updates the palette in the <see cref="ToolboxContainer"/> with that of the <see cref="SkaaSprite"/> the user is viewing
        /// </summary>
        private void ImageEditorContainer_ActiveSpriteChanged(object sender, EventArgs e)
        {
            var iec = sender as ImageEditorContainer;
            this._toolBoxContainer.SetPalette(iec?.ActiveSprite?.PalettePresenter?.GameObject);
        }
        
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameSetPresenter gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>(true);
            
        }

        public ColorPalette GetActivePalette()
        {
            return this._toolBoxContainer.ActivePalette;
        }
    }
}
