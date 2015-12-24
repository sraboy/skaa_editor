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
        private static SpritePresenter _activeSprite;
        private static ColorPalettePresenter _activePalette;

        private ToolboxContainer _toolBoxContainer;
        private SpriteViewerContainer _spriteViewerContainer;

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
        
        [NonSerialized]
        private EventHandler _activePaletteChanged;
        public event EventHandler ActivePaletteChanged
        {
            add
            {
                if (_activePaletteChanged == null || !_activePaletteChanged.GetInvocationList().Contains(value))
                {
                    _activePaletteChanged += value;
                }
            }
            remove
            {
                _activePaletteChanged -= value;
            }
        }
        private void OnActivePaletteChanged(EventArgs e)
        {
            EventHandler handler = _activePaletteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public ColorPalettePresenter ActivePalette
        {
            get
            {
                return _activePalette;
            }
            set
            {
                if (_activePalette != value)
                {
                    _activePalette = value;
                    OnActivePaletteChanged(EventArgs.Empty);
                }
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
                if (_activeSprite != value)
                {
                    _activeSprite = value;
                    OnActiveSpriteChanged(EventArgs.Empty);
                }
            }
        }

        public MDISkaaEditorMainForm()
        {
            InitializeComponent();
            SetUpDockPanel();
            ProjectManager.SetMainForm(this);
        }

        private void ActiveSprite_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var iec = (ImageEditorContainer)this.dockPanel.ActiveDocument ?? new ImageEditorContainer();
            iec.SetSprite(this.ActiveSprite);
        }

        private void SetUpDockPanel()
        {
            this._toolBoxContainer = new ToolboxContainer();
            this._toolBoxContainer.HideOnClose = true;
            this._spriteViewerContainer = new SpriteViewerContainer();
            this._spriteViewerContainer.HideOnClose = true;

            this.dockPanel.ActiveDocumentChanged += DockPanel_ActiveDocumentChanged;
            _toolBoxContainer.Show(dockPanel, DockState.DockLeft);
            _spriteViewerContainer.Show(dockPanel, DockState.DockRight);
            OpenNewTab(this.ActiveSprite);
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

        private void OpenNewTab(SpritePresenter spr)
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            iec.Show(dockPanel, DockState.Document);
            iec.SetSprite(spr);
            iec.ActiveSpriteChanged += ImageEditorContainer_ActiveSpriteChanged;
        }



        #region Event Handlers
        ///////////////////////////// Button Clicks /////////////////////////////
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
            //set palette first
            if (this.ActivePalette?.GameObject == null)
                loadPaletteToolStripMenuItem_Click(sender, e);

            SpritePresenter spr = (SpritePresenter)ProjectManager.Open<SkaaSprite, SpritePresenter>(FileFormats.SpriteSpr, this.ActivePalette.GameObject, ProjectManager.ActiveProject.GameSet);
            spr.PropertyChanged += ActiveSprite_PropertyChanged;

            var doc = ((ImageEditorContainer)this.dockPanel.ActiveDocument);
            if (doc?.ActiveSprite == null)
                this.ActiveSprite = spr;
            else
                OpenNewTab(spr);

            spr.SetActiveFrame(0);
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActivePalette = (ColorPalettePresenter)ProjectManager.Open<ColorPalette, ColorPalettePresenter>(FileFormats.Palette);
            _toolBoxContainer.SetPalette(this.ActivePalette.GameObject);
        }
        ///////////////////////////// Other UI Changes /////////////////////////////
        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            //get the palette for the currently-loaded sprite, if any
            var iec = (ImageEditorContainer)this.dockPanel.ActiveDocument;
            var pal = iec?.ActiveSprite?.Palette;

            if (pal != null)
                this.ActivePalette.GameObject = pal;

            UpdatePalette(iec);
        }
        /// <summary>
        /// Updates the palette in the <see cref="ToolboxContainer"/> with that of the <see cref="SkaaSprite"/> the user is viewing
        /// </summary>
        private void ImageEditorContainer_ActiveSpriteChanged(object sender, EventArgs e)
        {
            var iec = sender as ImageEditorContainer;
            UpdatePalette(iec);
        }

        private void UpdatePalette(ImageEditorContainer iec)
        {
            this._toolBoxContainer.SetPalette(iec?.ActiveSprite?.Palette);
        }
        #endregion

        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameSetPresenter gsp = (GameSetPresenter)ProjectManager.Open<DataSet, GameSetPresenter>(true);
            
        }
    }
}
