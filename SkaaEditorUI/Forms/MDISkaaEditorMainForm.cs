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

        internal ColorPalette ActivePalette
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
            this.Shown += MDISkaaEditorMainForm_Shown;
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
            this._toolBoxContainer.Show(_dockPanel, DockState.DockLeft);
            this._spriteViewerContainer.Show(_dockPanel, DockState.DockRight);
            OpenNewImageEditorContainerTab();

            //we don't want this as the ActiveDocument, so show it after OpenNewTab()
            this._gameSetViewerContainer.Show(_dockPanel, DockState.Document);
        }


        #region Project-Related Menu Item Clicks
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
        private void toolStripBtnSaveProject_Click(object sender, EventArgs e)
        {

        }
        private void toolStripBtnCloseProject_Click(object sender, EventArgs e)
        {

        }
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Other Events
        private void MDISkaaEditorMainForm_Shown(object sender, EventArgs e)
        {
            //dbgOpenBallistaSprite();
            dbgOpenIButtonResIdxMultiBmp();
            //dbgOpenIButtonResIdxMultiBmpAndStandardGameSet();
            //dbgOpenBallistaSpriteAndStandardGameSet();
            ToggleUIProjectOptions();
            ToggleUISaveOptions();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
            SetActiveSprite(iec?.ActiveSprite);

            ToggleUISaveOptions();
        }
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
        private void ImageEditorContainer_ActiveSpriteChanged(object sender, EventArgs e) { }
        private void ImageEditorContainer_ImageChanged(object sender, EventArgs e) { }
        private void MultiImagePresenterBase_ActiveFrameChanged(object sender, EventArgs e) { }
        #endregion

        #region Open File Menu Item Clicks
        private void openSpriteSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<SpritePresenter>() == false)
                MessageBox.Show("Failed to load sprite!");
        }
        private void openSpriteResIdxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<ResIdxMultiBmpPresenter>() == false)
                MessageBox.Show("Failed to load sprite!");
        }
        private void openPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenPalette() == false)
                MessageBox.Show("Failed to load palette!");
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenGameSet(true) == false)
                MessageBox.Show("Failed to load game set!");
        }
        #endregion

        #region Save/Export File Menu Item Clicks
        private void saveSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
            var spr = iec.ActiveSprite;

            if (iec.ActiveSprite is SpritePresenter)
            {
                ProjectManager.Save(spr);
                //todo: prompt to also save GameSet since offsets may have changed
            }
            else if (iec.ActiveSprite is ResIdxMultiBmpPresenter)
            {
                ProjectManager.Save(spr);
            }
        }
        private void saveGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Save(this._gameSetViewerContainer.GameSetPresenter);
        }

        private void exportSpriteFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
            var fr = iec.ActiveSprite.ActiveFrame as FramePresenter;
            ProjectManager.Save(fr);
        }
        #endregion

        /// <summary>
        /// Opens a <see cref="SkaaSprite"/> or <see cref="FileFormats.ResIdxMultiBmp"/>
        /// </summary>
        /// <typeparam name="T">A presenter class that implements <see cref="MultiImagePresenterBase"/></typeparam>
        /// <returns>The new presenter of type <paramref name="T"/> if successfull, <c>null</c> otherwise</returns>
        internal bool OpenSprite<T>() where T : MultiImagePresenterBase, new()
        {
            //check for a palette first
            //if not loaded, set it
            if (this._toolBoxContainer.ActivePalette == null)
                if (OpenPalette() == false)
                    return false;

            T spr = (T)ProjectManager.Open<SkaaSprite, T>(this._gameSetViewerContainer.GameSetPresenter);

            if (spr == null) //user canceled or loading failed
                return false;

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewImageEditorContainerTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewImageEditorContainerTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }


            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
            spr.ActiveFrameChanged += MultiImagePresenterBase_ActiveFrameChanged;
            return true;
        }
        internal bool OpenPalette()
        {
            var pal = (ColorPalettePresenter)ProjectManager.Open<System.Drawing.Imaging.ColorPalette, ColorPalettePresenter>(FileFormats.Palette);
            if (pal?.GameObject == null)
                return false;
            else
                _toolBoxContainer.SetPalette(pal.GameObject);

            return true;
        }
        internal bool OpenProject()
        {
            throw new NotImplementedException();
        }
        internal bool OpenGameSet(bool mergeDataSets)
        {
            GameSetPresenter gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>();

            if (gsp.GameObject == null)
                return false;

            if (mergeDataSets)
                this._gameSetViewerContainer.GameSetPresenter.Merge(gsp);
            else
                this._gameSetViewerContainer.GameSetPresenter = gsp;

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);

            return true;
        }
        private void SetSpriteDataViews(GameSetPresenter gsp)
        {
            if (gsp == null)
                return;

            ProjectManager.SetSpriteDataViews(gsp);
        }
  
        /// <summary>
        /// Enables or disables various file saving-related UI options based on the current status of the application
        /// </summary>
        private void ToggleUISaveOptions()
        {
            //todo: if GameSetPresenter.GameObject doesn't contain std.set files, disable this
            //user has loaded the game set or a ResIdx file has made a DataSet for its header data
            this.saveGameSetToolStripMenuItem.Enabled = this._gameSetViewerContainer?.GameSetPresenter?.GameObject == null;

            if (this._dockPanel.ActiveDocument is ImageEditorContainer)       //user is viewing an image
            {
                //we can save just about anything as an individual frame
                this.exportSpriteFrameToolStripMenuItem.Enabled = true;

                var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;

                if (iec.ActiveSprite is SpritePresenter)                      //image is from an SPR sprite
                {
                    this.saveSpriteToolStripMenuItem.Enabled = true;
                    //this.saveSpriteResIdxToolStripMenuItem.Enabled = false;
                }
                else if (iec.ActiveSprite is ResIdxMultiBmpPresenter)         //image is from a ResIdx "sprite"
                {
                    //this.saveSpriteResIdxToolStripMenuItem.Enabled = true;
                    this.saveSpriteToolStripMenuItem.Enabled = true;
                }
            }
            else if (this._dockPanel.ActiveDocument is GameSetViewerContainer) //user is viewing the game set
            {
                this.saveSpriteToolStripMenuItem.Enabled = false;
                this.exportSpriteFrameToolStripMenuItem.Enabled = false;
                //this.saveSpriteResIdxToolStripMenuItem.Enabled = false;
            }
        }
        /// <summary>
        /// Enables or disables various project-related UI options based on the current status of the application
        /// </summary>
        private void ToggleUIProjectOptions()
        {
            toolStripBtnNewProject.Enabled = false;
            toolStripBtnOpenProject.Enabled = false;
            toolStripBtnSaveProject.Enabled = false;
            toolStripBtnCloseProject.Enabled = false;
            newProjectToolStripMenuItem.Enabled = false;
            saveProjectToolStripMenuItem.Enabled = false;
            closeProjectToolStripMenuItem.Enabled = false;
        }
        /// <summary>
        /// Closes the current project and saves any changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        internal bool TrySaveCloseProject()
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
        internal DialogResult UserShouldSaveChanges()
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
        internal ImageEditorContainer OpenNewImageEditorContainerTab()
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            iec.Show(_dockPanel, DockState.Document);
            iec.ActiveSpriteChanged += ImageEditorContainer_ActiveSpriteChanged;
            iec.ImageChanged += ImageEditorContainer_ImageChanged;
            return iec;
        }

        internal void SetActiveSprite(MultiImagePresenterBase spr)
        {
            var iec = (this._dockPanel.ActiveDocument as ImageEditorContainer);
            iec?.SetSprite(spr);
            this._spriteViewerContainer.SetSprite(iec?.ActiveSprite);
            this._toolBoxContainer.SetPalette(iec?.ActiveSprite?.PalettePresenter?.GameObject);
            ToggleUISaveOptions();
        }

    
    }
}
