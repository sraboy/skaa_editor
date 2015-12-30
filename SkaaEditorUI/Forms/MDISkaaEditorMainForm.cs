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
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SkaaEditorUI.Forms.DockContentControls;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms
{
    public partial class MDISkaaEditorMainForm : Form
    {
        private ToolboxContainer _toolBoxContainer;
        private SpriteViewerContainer _spriteViewerContainer;
        private GameSetViewerContainer _gameSetViewerContainer;
        private ProjectManager ProjectManager = new ProjectManager();

        public ColorPalette ActivePalette
        {
            get
            {
                return this._toolBoxContainer.ActivePalette;
            }
        }

        public MDISkaaEditorMainForm()
        {
            InitializeComponent();
            SetUpDockPanel();
            ProjectManager.SetMainForm(this);
            this.showGridToolStripMenuItem.Checked = true;
            this._toolBoxContainer.ColorChanged += ToolboxContainer_ColorChanged;
            this._toolBoxContainer.SelectedToolChanged += ToolboxContainer_SelectedToolChanged;
        }

        private void SetUpDockPanel()
        {
            this._toolBoxContainer = new ToolboxContainer();
            this._toolBoxContainer.HideOnClose = true;
            this._spriteViewerContainer = new SpriteViewerContainer();
            this._spriteViewerContainer.HideOnClose = true;
            this._gameSetViewerContainer = new GameSetViewerContainer();
            this._gameSetViewerContainer.HideOnClose = true;

            this._dockPanel.ActiveDocumentChanged += DockPanel_ActiveDocumentChanged;
            _toolBoxContainer.Show(_dockPanel, DockState.DockLeft);
            _spriteViewerContainer.Show(_dockPanel, DockState.DockRight);
            OpenNewTab();

            //we don't want this as the ActiveDocument, so show it after OpenNewTab()
            this._gameSetViewerContainer.Show(_dockPanel, DockState.Document);
        }



        #region Click Events
        private void ToolboxContainer_SelectedToolChanged(object sender, EventArgs e)
        {
            var iec = (ImageEditorContainer)this._dockPanel.ActiveDocument;
            iec.ChangeToolMode(sender, e);
        }
        private void ToolboxContainer_ColorChanged(object sender, EventArgs e)
        {
            //todo: create a ColorChangedEventArgs so we can just pass sender/e like SelectedToolChanged
            var iec = (ImageEditorContainer)this._dockPanel.ActiveDocument;
            iec.SetActiveColors((sender as Cyotek.Windows.Forms.ColorGrid).Color, Color.FromArgb(0, 0, 0, 0));
        }
        private void toolStripBtnNewProject_Click(object sender, EventArgs e)
        {
            //if (TrySaveCloseProject())
            //    ProjectManager.CreateNewProject();
        }
        private void toolStripBtnOpenProject_Click(object sender, EventArgs e)
        {
            if (OpenProject() == false)
                MessageBox.Show("Failed to open project!");
            //Browse folders to find a project directory
            //Check all file types and load them
        }

        private void openSpriteSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<SpritePresenter>() == null)
                MessageBox.Show("Failed to load sprite!");
        }
        private void openSpriteResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<ResIdxMultiBmpPresenter>() == null)
                MessageBox.Show("Failed to load sprite!");
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenPalette() == false)
                MessageBox.Show("Failed to load palette!");
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenGameSet() == null)
                MessageBox.Show("Failed to load game set!");
        }
        private void saveSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = this._dockPanel.ActiveDocument as ImageEditorContainer;
            var spr = doc.ActiveSprite;

            ProjectManager.Save(spr);
        }
        #endregion


        /// <summary>
        /// Closes the current project and saves any changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        private bool TrySaveCloseProject()
        {
            DialogResult saveChanges = UserShouldSaveChanges();

            if (saveChanges == DialogResult.Yes)
            {
                //ProjectManager.CloseProject();
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
        /// <summary>
        /// Opens a new document tab, which is automatically set as the <see cref="DockPanel.ActiveDocument"/>
        /// </summary>
        public void OpenNewTab()
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            iec.Show(_dockPanel, DockState.Document);
            iec.ActiveSpriteChanged += ImageEditorContainer_ActiveSpriteChanged;
            iec.ImageChanged += ImageEditorContainer_ImageChanged;
        }

        public void SetActiveSprite(MultiImagePresenterBase spr)
        {
            var iec = (ImageEditorContainer)this._dockPanel.ActiveDocument;// ?? new ImageEditorContainer();
            iec.SetSprite(spr);
            this._spriteViewerContainer.SetSprite(spr);
        }
        /// <summary>
        /// Opens a <see cref="SkaaSprite"/>
        /// </summary>
        /// <typeparam name="T">A presenter class that implements <see cref="MultiImagePresenterBase"/></typeparam>
        /// <returns>The new presenter of type <paramref name="T"/> if successfull, <c>null</c> otherwise</returns>
        public T OpenSprite<T>() where T : MultiImagePresenterBase, new()
        {
            //check for a palette first
            //if not loaded, set it
            if (this._toolBoxContainer.ActivePalette == null)
                if (OpenPalette() == false)
                    return null;

            T spr = (T)ProjectManager.Open<SkaaSprite, T>(FileFormats.SpriteSpr);

            if (spr == null) //user canceled or loading failed
                return null;

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


            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
            return spr;
        }

        public bool OpenPalette()
        {
            var pal = (ColorPalettePresenter)ProjectManager.Open<System.Drawing.Imaging.ColorPalette, ColorPalettePresenter>(FileFormats.Palette);
            if (pal?.GameObject == null)
                return false;
            else
                _toolBoxContainer.SetPalette(pal.GameObject);

            return true;
        }

        public bool OpenProject()
        {
            throw new NotImplementedException();
        }

        public GameSetPresenter OpenGameSet()
        {
            GameSetPresenter gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>(FileFormats.GameSet, true);

            if (gsp.GameObject == null)
                return null;

            this._gameSetViewerContainer.GameSetPresenter = gsp;
            SetSpriteDataViews(gsp);

            return gsp;
        }

        private void SetSpriteDataViews(GameSetPresenter gsp)
        {
            ProjectManager.SetSpriteDataViews(gsp);
            //var spr = (this._dockPanel.ActiveDocument as ImageEditorContainer).ActiveSprite;
            //this._spriteViewerContainer.SetSprite(spr);
        }

        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            //get the palette for the currently-loaded sprite, if any
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
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
            this._spriteViewerContainer.SetSprite(iec?.ActiveSprite);
        }

        /// <summary>
        /// Forces redraws of child controls displaying the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageEditorContainer_ImageChanged(object sender, EventArgs e)
        {
            //todo: force an update of the individual cell containing the image
            // It only updates after a mouseover. Refresh/Invalidate/Update
            // do nothing but SetSprite actually resets the list of objects,
            // which works, but could be a performance issue with large sprites.
            var iec = sender as ImageEditorContainer;
            this._spriteViewerContainer.SetSprite(iec?.ActiveSprite);
        }


    }
}
