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
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SkaaEditorUI.Forms.DockContentControls;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib.GameObjects;
using SkaaGameDataLib.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms
{
    public partial class MDISkaaEditorMainForm : Form
    {
        private ToolboxContainer _toolBoxContainer;
        private SpriteViewerContainer _spriteViewerContainer;
        private GameSetViewerContainer _gameSetViewerContainer;
        private ProjectManager ProjectManager = new ProjectManager();
        private Timer _recalculateFrameOffsetsTimer = new Timer();

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
            this.DragEnter += MDISkaaEditorMainForm_DragEnter;
            this.DragDrop += MDISkaaEditorMainForm_DragDrop;
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
            //OpenNewImageEditorContainerTab();

            //hiding for alphaV4 release
            //we don't want this as the ActiveDocument, so show it after OpenNewTab()
            //this._gameSetViewerContainer.Show(_dockPanel, DockState.Document);
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

        #region Open File Menu Item Clicks
        private void openSpriteSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<SpritePresenter>() == false)
                MessageBox.Show($"Failed to load sprite ({typeof(SpritePresenter)})!");
        }
        private void openSpriteResIdxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSprite<ResIdxMultiBmpPresenter>() == false)
                MessageBox.Show($"Failed to load sprite ({typeof(ResIdxMultiBmpPresenter)})!");
        }
        private void openPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenPalette() == false)
                MessageBox.Show("Failed to load palette!");
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenGameSet() == false)
                MessageBox.Show("Failed to load game set!");
            else
                ToggleUISaveEditOptions(); //hack: while we don't have a gamesetviewercontainer.gamesetchanged event
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
        private void exportSpriteSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                var doc = this._dockPanel.ActiveDocument as ImageEditorContainer;

                if (dlg.ShowDialog() == DialogResult.OK && doc != null)
                {
                    doc.ActiveSprite.GetSpriteSheet().Save(dlg.FileName);
                }
            }
        }
        #endregion

        #region Other Click Events
        private void addFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
            if (iec == null)
                return;

            using (AddNewFrame dlg = new AddNewFrame())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var frame = iec.ActiveSprite.AddNewFrame(dlg.FrameName, dlg.FrameHeight, dlg.FrameWidth);

                    //Reset the ActiveSprite so the UI is aware of the new frame. This has to happen 
                    //before setting the ActiveFrame (below) because the various controls don't yet
                    //know about this new frame. For example, TimelineView's trackbar operates based
                    //on a minimum of 0 and maximum of the number of frames. The number of frames isn't
                    //updated until it is passed the "new version" of the sprite here.
                    SetActiveSprite(iec.ActiveSprite);

                    //Make the new frame the ActiveFrame so the user can begin editing it immediately
                    iec.ActiveSprite.ActiveFrame = frame;
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm dlg = new AboutForm())
            {
                dlg.ShowDialog();
            }
        }
        #endregion

        #region Other Events
        private void MDISkaaEditorMainForm_Shown(object sender, EventArgs e)
        {
            //dbgOpenBallistaSprite();
            //dbgOpenIButtonResIdxMultiBmp();
            //dbgOpenIButtonResIdxMultiBmpAndStandardGameSet();
            //dbgOpenBallistaSpriteAndStandardGameSet();

            //We use a 30s timer to recalculate the offsets from image changes
            //because it's a relatively hefty operation. Doing it on every edit
            //make the UI slow and clunky.
            this._recalculateFrameOffsetsTimer.Enabled = true;
            this._recalculateFrameOffsetsTimer.Interval = 30000;
            this._recalculateFrameOffsetsTimer.Tick += RecalculateFrameOffsetsTimer_Tick;
            this._recalculateFrameOffsetsTimer.Start();

            ToggleUIProjectOptions();
            ToggleUISaveEditOptions();
        }
        private void MDISkaaEditorMainForm_DragEnter(object sender, DragEventArgs e)
        {
            bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (isFile)
                e.Effect = DragDropEffects.Copy;
        }
        private void MDISkaaEditorMainForm_DragDrop(object sender, DragEventArgs e)
        {
            List<KeyValuePair<string, FileFormats>> filesAndFormats = new List<KeyValuePair<string, FileFormats>>();
            List<string> files = new List<string>();

            //enumerate all files in multiple directories
            foreach (string filename in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Directory.Exists(filename))
                    files.AddRange(Directory.EnumerateFiles(filename, "*.*", SearchOption.AllDirectories));
                else if (File.Exists(filename))
                    files.Add(filename);
            }

            foreach (string filename in files)
                filesAndFormats.Add(new KeyValuePair<string, FileFormats>(filename, FileTypeChecks.CheckFileType(filename)));

            //open the palette first so the user is presented with an OpenFileDialog
            //if a SpritePresenter is first

            if (filesAndFormats.Exists(kvp => kvp.Value == FileFormats.Palette))
            {
                var pal = filesAndFormats.Find(kvp => kvp.Value == FileFormats.Palette);
                if (OpenPalette(pal.Key) == false)
                    MessageBox.Show($"Failed to load palette: {pal.Key}");
                filesAndFormats.Remove(pal);
            }

            //ProjectManager.Open() functions off assuming the user needs to navigate to a file
            //and will use the FileDialogs. We need to bypass those.
            foreach (var kvp in filesAndFormats)
            {
                switch (kvp.Value)
                {
                    case FileFormats.ResIdxDbf:
                        if (OpenGameSet(kvp.Key) == false)
                            MessageBox.Show($"Failed to load game set: {kvp.Key}");
                        break;
                    case FileFormats.SpriteSpr:
                        if (OpenSprite<SpritePresenter>(kvp.Key) == false)
                            MessageBox.Show($"Failed to load sprite ({typeof(SpritePresenter)}): {kvp.Key}");
                        break;
                    case FileFormats.ResIdxMultiBmp:
                        if (OpenSprite<ResIdxMultiBmpPresenter>(kvp.Key) == false)
                            MessageBox.Show($"Failed to load sprite ({typeof(ResIdxMultiBmpPresenter)}): {kvp.Key}");
                        break;

                }
            }

            ToggleUISaveEditOptions();
        }
        private void RecalculateFrameOffsetsTimer_Tick(object sender, EventArgs e)
        {
            foreach (ImageEditorContainer iec in this._dockPanel.Documents)
                iec.ActiveSprite?.RecalculateFrameOffsets();
        }
        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;
            SetActiveSprite(iec?.ActiveSprite);
            this._toolBoxContainer.SetImageToEdit(iec?.Image);
            ToggleUISaveEditOptions();
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
        /// <summary>
        /// Updates various UI elements when <see cref="ImageEditorContainer.Image"/> is edited.
        /// </summary>
        /// <remarks>
        /// While there are methods here to update the UI when the image is changed by virtue
        /// of changing the <see cref="ImageEditorContainer.ActiveSprite"/> or navigating its
        /// frames, this method is necessary to update the UI when <see cref="ImageEditorContainer"/>
        /// alters the image internally. For example, image resizing is handled entirely internal to
        /// the class.
        /// </remarks>
        private void ImageEditorContainer_ImageChanged(object sender, EventArgs e)
        {
            var iec = (sender as ImageEditorContainer);

            //If the image was resized, ToolBoxContainer needs its new dimensions
            //in order to display them if it is resized again.
            this._toolBoxContainer.SetImageToEdit(iec?.Image);

            //We also need to display those new dimensions.
            SetStatusStrip(iec?.ActiveSprite);
        }
        private void MultiImagePresenterBase_ActiveFrameChanged(object sender, EventArgs e)
        {
            SetStatusStrip(sender as MultiImagePresenterBase);
        }
        #endregion

        /// <summary>
        /// Opens a <see cref="SkaaSprite"/> or <see cref="FileFormats.ResIdxMultiBmp"/>
        /// </summary>
        /// <typeparam name="T">A presenter class that implements <see cref="MultiImagePresenterBase"/></typeparam>
        /// <returns>The new presenter of type <paramref name="T"/> if successfull, <c>null</c> otherwise</returns>
        internal bool OpenSprite<T>(string filePath = null) where T : MultiImagePresenterBase, new()
        {
            //check for a palette first
            //if not loaded, set it
            if (this._toolBoxContainer.ActivePalette == null)
                if (OpenPalette() == false)
                    return false;
            T spr;

            if (filePath == null)
                spr = (T)ProjectManager.Open<SkaaSprite, T>(this._gameSetViewerContainer.GameSetPresenter);
            else
                spr = (T)ProjectManager.Open<SkaaSprite, T>(filePath, this._gameSetViewerContainer.GameSetPresenter);

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
        internal bool OpenPalette(string filePath = null)
        {
            ColorPalettePresenter pal;

            if (filePath == null)
                pal = (ColorPalettePresenter)ProjectManager.Open<ColorPalette, ColorPalettePresenter>();
            else
                pal = (ColorPalettePresenter)ProjectManager.Open<ColorPalette, ColorPalettePresenter>(filePath);

            if (pal?.GameObject == null)
                return false;
            else
                _toolBoxContainer.SetPalette(pal.GameObject);

            return true;
        }
        internal bool OpenGameSet(string filePath = null, bool mergeDataSets = true)
        {
            GameSetPresenter gsp;

            if (filePath == null)
                gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>();
            else
                gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>(filePath);

            if (gsp.GameObject == null)
                return false;

            if (mergeDataSets)
                this._gameSetViewerContainer.GameSetPresenter.Merge(gsp);
            else
                this._gameSetViewerContainer.GameSetPresenter = gsp;

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);

            return true;
        }
        internal bool OpenProject()
        {
            throw new NotImplementedException();
        }

        internal void SetSpriteDataViews(GameSetPresenter gsp) => ProjectManager.SetSpriteDataViews(gsp);
        /// <summary>
        /// Enables or disables various file saving-related UI options based on the current status of the application
        /// </summary>
        internal void ToggleUISaveEditOptions()
        {
            //todo: if GameSetPresenter.GameObject doesn't contain std.set files, disable this
            //user has loaded the game set or a ResIdx file has made a DataSet for its header data
            this.saveGameSetToolStripMenuItem.Enabled = this._gameSetViewerContainer?.GameSetPresenter?.GameObject != null;

            if (this._dockPanel.ActiveDocument is ImageEditorContainer)       //user is viewing an image
            {
                //we can save just about anything as an individual frame
                this.exportSpriteFrameToolStripMenuItem.Enabled = true;

                var iec = this._dockPanel.ActiveDocument as ImageEditorContainer;

                if (iec.ActiveSprite is SpritePresenter || iec.ActiveSprite is ResIdxMultiBmpPresenter)
                {
                    this.addFrameToolStripMenuItem.Enabled = true;
                    this.saveSpriteToolStripMenuItem.Enabled = true;
                    this.exportSpriteSheetToolStripMenuItem.Enabled = true;
                }
            }
            else //if (this._dockPanel.ActiveDocument is GameSetViewerContainer) //user is viewing the game set
            {
                this.addFrameToolStripMenuItem.Enabled = false;
                this.saveSpriteToolStripMenuItem.Enabled = false;
                this.exportSpriteFrameToolStripMenuItem.Enabled = false;
                this.exportSpriteSheetToolStripMenuItem.Enabled = false;
            }
        }
        /// <summary>
        /// Enables or disables various project-related UI options based on the current status of the application
        /// </summary>
        internal void ToggleUIProjectOptions()
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
            bool saveChanges = UserShouldSaveChanges();

            if (saveChanges == true)
            {
                //todo: save project
                ProjectManager.CloseProject();
                return true;
            }
            else
            {
                ProjectManager.CloseProject();
                return true;
            }
        }
        internal bool UserShouldSaveChanges()
        {
            foreach (ImageEditorContainer iec in this._dockPanel.Documents)
            {
                if (iec.ActiveSprite?.BitmapHasChanges == true)
                    return true;
            }

            return false;
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

            //Because ImageEditorContainer.ResizeImageMethod refers to an instance method
            //set in it's non-static constructor, we can't set this until an ImageEditorContainer
            //has been created.
            this._toolBoxContainer.ResizeImageDelegate = ImageEditorContainer.ResizeImageMethod;
            return iec;
        }
        /// <summary>
        /// Updates the UI to display the information for the specified sprite, or 
        /// clears/disables various UI items if <paramref name="spr"/> is null.
        /// </summary>
        /// <param name="spr">A <see cref="MultiImagePresenterBase"/> or null</param>
        internal void SetActiveSprite(MultiImagePresenterBase spr)
        {
            var iec = (this._dockPanel.ActiveDocument as ImageEditorContainer);
            iec?.SetSprite(spr);
            this._spriteViewerContainer.SetSprite(iec?.ActiveSprite);
            this._toolBoxContainer.SetPalette(iec?.ActiveSprite?.PalettePresenter?.GameObject);
            this._toolBoxContainer.SetImageToEdit(iec?.Image);

            SetStatusStrip(iec?.ActiveSprite);
            ToggleUISaveEditOptions();
        }
        internal void SetStatusStrip(MultiImagePresenterBase spr)
        {
            if (spr != null)
            {
                this.tsStatusLblFileType.Text = $"{spr.GetType().Name}: {spr.SpriteId}";
            }
            else
                this.tsStatusLblFileType.Text = "No File Loaded";

            string bmpSize = string.Empty;

            if (spr?.ActiveFrame?.Bitmap != null)
                bmpSize = $"{spr.ActiveFrame.Name}: {spr.ActiveFrame.Bitmap.Height} x {spr.ActiveFrame.Bitmap.Width}";

            this.tsStatusLblImageStats.Text = bmpSize;
        }
    }
}
