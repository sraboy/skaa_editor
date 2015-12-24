﻿#region Copyright Notice
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
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using SkaaGameDataLib;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Cyotek.Windows.Forms;
using Capslock.WinForms.ImageEditor;
using System.Data;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI.Forms
{
    public partial class SkaaEditorMainForm : Form
    {
        //todo: load settings from file on form load
        //todo: add/improve Debug logging throughout solution
        public static readonly TraceSource Logger = new TraceSource("SkaaEditorMainForm", SourceLevels.All);

        #region Project Event
        private EventHandler _activeProjectChanged;
        public event EventHandler ActiveProjectChanged
        {
            add
            {
                if (_activeProjectChanged == null || !_activeProjectChanged.GetInvocationList().Contains(value))
                {
                    _activeProjectChanged += value;
                }
            }
            remove
            {
                _activeProjectChanged -= value;
            }
        }
        protected virtual void OnActiveProjectChanged(EventArgs e)
        {
            EventHandler handler = _activeProjectChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Private Members
        private oldProject _activeProject;
        private Properties.Settings props = Properties.Settings.Default;
        private bool _tempProjectFolder = false;
        private List<string> _tempFiles;
        #endregion

        #region Properties
        public oldProject ActiveProject
        {
            get
            {
                return this._activeProject;
            }
            set
            {
                if (this._activeProject != value)
                {
                    this._activeProject = value;
                    OnActiveProjectChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Constructors
        public SkaaEditorMainForm()
        {
            InitializeComponent();
            this._tempFiles = new List<string>();
        }
        #endregion

        #region Setup Methods
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //this has to happen early so NewProject() triggers the event.
            this.ActiveProjectChanged += SkaaEditorMainForm_ActiveProjectChanged;

            ////this is needed so we can respond when the user uses the tracking bar to change frames
            //this.timelineControl1.ActiveFrameChanged += timelineControl_ActiveFrameChanged;
            this.spriteViewer1.ActiveFrameChanged += SpriteViewer1_ActiveFrameChanged;
            //this event is only called when the entire image changes: during frame changes or loading/closing a sprite
            this.imageEditorBox.ImageChanged += imageEditorBox_ImageChanged;

            //this event is called any time the user edits the image
            this.imageEditorBox.ImageUpdated += imageEditorBox_ImageUpdated;

            //it only shows when zoomed in and that's exactly when it's most needed.
            this.imageEditorBox.ShowPixelGrid = true;
            this.showGridToolStripMenuItem.Checked = true;

            //need to adjust our actions based on the tool selected
            this.drawingToolbox.SelectedToolChanged += DrawingToolbox_SelectedToolChanged;

            //ConfigSettingsDebug();
            ConfigSettings();
            SetupUI();
        }

        private void SpriteViewer1_ActiveFrameChanged(object sender, EventArgs e)
        {
            SetupUI();
        }

        /// <summary>
        /// Provides for initial application settings like default directories, debug logging, etc.
        /// </summary>
        private void ConfigSettings()
        {
            props.ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
            Trace.AutoFlush = true;
            Logger.TraceEvent(TraceEventType.Start, 0, $"Log started: {string.Concat(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())}");

            props.DataDirectory = props.ApplicationDirectory + "data\\";
            props.ProjectsDirectory = props.DataDirectory + "projects\\";
            Directory.CreateDirectory(props.ProjectsDirectory);
            MakeTempFolder();
        }

        /// <summary>
        /// Loads <see cref="oldProject.ActivePalette"/>, if specified, and enables/disables the form's <see cref="SkaaColorChooser"/> object based on whether or not a palette is loaded.
        /// </summary>
        private void SetUpColorGrid()
        {
            if (this.ActiveProject?.ActivePalette != null)
            {
                Color[] entries = this.ActiveProject.ActivePalette.Entries;
                IEnumerable<Color> distinct = entries.Distinct();
                this.colorGridChooser.Colors = new ColorCollection(distinct);
                this.colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Value);
                this.colorGridChooser.Enabled = true;
            }
            else
            {
                this.colorGridChooser.Colors.Clear();
                this.colorGridChooser.Palette = Cyotek.Windows.Forms.ColorPalette.None;
                this.colorGridChooser.Enabled = false;
            }

            this.colorGridChooser.Refresh();
        }

        /// <summary>
        /// Sets/Resets various UI settings like menu options, etc.
        /// </summary>
        private void SetupUI()
        {
            SetUpColorGrid();

            //Can't load a sprite without at least a palette and project. Otherwise, we can always load a new sprite.
            this.openSpriteToolStripMenuItem.Enabled = (this.colorGridChooser.Enabled == false || this.ActiveProject == null) ? false : true;

            //disable editing until we've got a frame
            this.drawingToolbox.Enabled = this.ActiveProject?.ActiveFrame == null ? false : true;
            this.colorGridChooser.Enabled = this.ActiveProject?.ActiveFrame == null ? false : true;

            //can't open a set without a project
            this.openGameSetToolStripMenuItem.Enabled = this.ActiveProject == null ? false : true;

            //can't save what's not there
            this.saveSpriteToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null || this.ActiveProject?.ActiveSprite == null) ? false : true;
            this.exportPngToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null || this.ActiveProject?.ActiveSprite == null) ? false : true;
            this.saveGameSetToolStripMenuItem.Enabled = (this.ActiveProject?.ActiveGameSet == null) ? false : true;

            //need a sprite to navigate a sprite's frames
            this.spriteViewer1.Enabled = (this.ActiveProject?.ActiveSprite == null) ? false : true;

            //can't close a project that's not open
            this.closeProjectToolStripMenuItem.Enabled = this.ActiveProject == null ? false : true;

            //some help text until a sprite is loaded
            string help_text =
                "Be sure to open proper game set file before opening a sprite!\n" +
                "File >> Open Game Set >> Choose a SET file. (e.g., 7KAA's std.set).\n" +
                "You must also save your game set after saving any edits to a sprite.\n\n" +
                "To open/edit a sprite, its filename must match the original (e.g.,\"ballista.spr,\").\n\n" +
                "Please report bugs to steven.lavoiejr@gmail.com or https://www.github.com/sraboy/skaa_editor/.";
            this.imageEditorBox.Text = (this.imageEditorBox.Image == null) ? help_text : null;
        }
        #endregion

        #region UI Click Events and Open/Save
        ////////////////////////////////// New Things //////////////////////////////////
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrySaveCloseProject(null, null))
                NewProject();
        }
        //////////////////////////////// Opening Things ////////////////////////////////
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null)
                if (!TrySaveCloseProject(null, null))
                    return;

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Choose the directory containing the sprite[s] and matching game set file.";
                dlg.SelectedPath = props.ProjectsDirectory;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    OpenProject(dlg.SelectedPath);
                }
            }
        }
        private void openSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginOpenFile(FileFormats.SpriteSpr);
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //force closing the sprite for now
            //todo: ask user to associate a gameset to the sprite, or specify 'none'
            if (this.ActiveProject?.ActiveSprite != null && this.ActiveProject?.ActiveGameSet != null)
            {
                string msg = "This will close the currently open file. Continue?";

                if (MessageBox.Show(msg, "Wait!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.ActiveProject.ActiveSprite = null;
                    this.ActiveProject.ActiveGameSet = null;
                }
                else
                {
                    return;
                }
            }

            BeginOpenFile(FileFormats.GameSet);
        }
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginOpenFile(FileFormats.Any);
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginOpenFile(FileFormats.Palette);
        }
        //todo: change the methods above to a single generic FileOpen_Click() handler and check the sender to decide action
        private void SkaaEditorMainForm_DragDrop(object sender, DragEventArgs e)
        {
            List<KeyValuePair<string, string>> filesAndFormats = new List<KeyValuePair<string, string>>();
            List<string> files = new List<string>();

            //enumerate all files in multiple directories
            foreach (string filename in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Directory.Exists(filename))
                    files.AddRange(Directory.EnumerateFiles(filename, "*.*", SearchOption.AllDirectories));
                else if (File.Exists(filename))
                    files.Add(filename);
            }

            //open all of the files found above
            //todo: MDI for multiple sprites
            foreach (string filename in files)
            {
                TryOpenFile(filename, FileFormats.Any, null);

                //todo: move these to their own method or separate form
                //For debugging: Gets all files and their formats. Resets active gameset in case we open two set files
                //this.ActiveProject.ActiveGameSet = new System.Data.DataSet();
                //filesAndFormats.Add(new KeyValuePair<string, string>(filename, TryOpenFile(filename, FileFormats.Any, null).ToString()));
            }
            //For debugging. Used to generate table on 7kfans.com/wiki
            //WriteCsv(filesAndFormats);
        }
        private void SkaaEditorMainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (this.ActiveProject != null)
            {
                bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop);
                if (isFile)
                    e.Effect = DragDropEffects.Copy;
            }
        }

        private void BeginOpenFile(FileFormats format, string filepath = "")
        {
            if (this.ActiveProject == null)
                return;

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = props.ProjectDirectory == null || this._tempProjectFolder ? props.ProjectsDirectory : props.ProjectDirectory;

                switch (format)
                {
                    case FileFormats.GameSet: //set file
                        dlg.Filter = $"7KAA Game Set Files (*.set)|*{props.SetFileExtension}|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.SetFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () => this.ActiveProject.OpenGameSet(dlg.FileName));
                        break;
                    //case FileFormat.SpritePNG:
                    //    dlg.DefaultExt = ".png";
                    //    dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
                    //    dlg.FileName = filepath;
                    //    //ShowOpenFileDialog(dlg, format, () => this.ActiveProject.LoadSprite(dlg.FileName));
                    //    break;
                    //case FileFormat.FramePNG:
                    //    dlg.DefaultExt = ".png";
                    //    dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
                    //    dlg.FileName = filepath;
                    //    //ShowOpenFileDialog(dlg, () => Project.Export(dlg.FileName, this.ActiveProject.ActiveFrame));
                    //    break;
                    case FileFormats.SpriteSpr:
                        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.SprFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () => { this.ActiveProject.ActiveSprite = (SpritePresenter)oldProject.LoadSprite(dlg.FileName, this.ActiveProject.ActivePalette); });
                        this.ActiveProject.SetActiveSpriteSframeDbfDataView();
                        break;
                    case FileFormats.SpriteFrameSpr:
                        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.SprFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () =>
                        {
                            SpritePresenter spr;
                            if (this.ActiveProject.ActiveSprite == null)
                                spr = new SpritePresenter();
                            else
                                spr = this.ActiveProject.ActiveSprite;
                            spr.Frames.Add(oldProject.LoadFrame(dlg.FileName, this.ActiveProject.ActivePalette));
                            this.ActiveProject.ActiveSprite = spr;
                        });
                        break;
                    case FileFormats.DbaseIII:
                        dlg.Filter = $"7KAA Resource Files (*.res)|*{props.ResFileExtension}|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.ResFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () =>
                        {
                            Tuple<SpritePresenter, DataTable> tup = oldProject.LoadResDbf(dlg.FileName, this.ActiveProject.ActivePalette);
                            //this.ActiveProject.ActiveSprite = tup.Item1;
                            this.ActiveProject.ActiveGameSet = this.ActiveProject.ActiveGameSet ?? new DataSet();
                            this.ActiveProject.ActiveGameSet.Tables.Add(tup.Item2);
                            this.ActiveProject.ActiveGameSet.AddDataSource(Path.GetFileName(dlg.FileName));
                        });
                        break;
                    case FileFormats.Palette:
                        dlg.Filter = $"7KAA Palette Files (*.res) (*.col)|*{props.ResFileExtension};*.col|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.ResFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () => this.ActiveProject.OpenPalette(dlg.FileName));
                        break;
                    case FileFormats.ResIdxMultiBmp:
                        dlg.Filter = $"7KAA Resource Files (*.res)|*{props.ResFileExtension}|All Files (*.*)|*.*";
                        dlg.DefaultExt = props.ResFileExtension;
                        dlg.FileName = filepath;
                        OpenFile(dlg, format, () =>
                        {
                            Tuple<SpritePresenter, DataTable> tup = oldProject.LoadResIdxMultiBmp(dlg.FileName, this.ActiveProject.ActivePalette);
                            this.ActiveProject.ActiveGameSet = this.ActiveProject.ActiveGameSet ?? new DataSet();
                            this.ActiveProject.ActiveGameSet.Tables.Add(tup.Item2);
                            this.ActiveProject.ActiveGameSet.AddDataSource(Path.GetFileName(dlg.FileName));
                            this.ActiveProject.ActiveSprite = tup.Item1;
                        });
                        break;
                    case FileFormats.Any: //user did not specify file type via UI menus (drag/drop or generic Open File)
                        OpenFile(dlg, format, null);
                        break;
                }
            }

            if (format != FileFormats.GameSet && format != FileFormats.Palette)
                this.tsStatusLblFileType.Text = format.ToString();
        }
        private void OpenFile(OpenFileDialog dlg, FileFormats requestedFormat, Action openMethod)
        {
            if (dlg.FileName != string.Empty)
                TryOpenFile(dlg.FileName, requestedFormat, openMethod);
            else
                ShowOpenFileDialog(dlg, requestedFormat, openMethod);
        }
        private void ShowOpenFileDialog(OpenFileDialog dlg, FileFormats requestedFormat, Action openMethod)
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TryOpenFile(dlg.FileName, requestedFormat, openMethod);
            }
        }
        private FileFormats TryOpenFile(string filePath, FileFormats requestedFormat, Action openMethod)
        { //todo: eliminate double call to CheckFileType with Drag/Drop and "Open File...". Prompt user with recognized filetype and ask for confirmation
            if (filePath == string.Empty)
                throw new ArgumentException("Received null file path in TryOpenFile()!");

            Debug.Assert(requestedFormat != FileFormats.Unknown, "Cannot request to open a file of FileFormat.Unknown! Use FileFormat.Any when opening arbitrary files.");

            this.tsStatusLblFileType.Text = "Checking file type...";
            var actualFormat = FileTypeChecks.CheckFileType(filePath);

            if (requestedFormat == FileFormats.Any && actualFormat != FileFormats.Unknown) //user did not specify file type via UI menus (drag/drop or generic Open File)
            {
                BeginOpenFile(actualFormat, filePath);                                     //now make the request again with the real file type
            }
            else if (actualFormat == FileFormats.Unknown)                                  //we can't figure out what this. user must specify a file type
            {
                //MessageBox.Show($"Could not determine the data format of:\r\n \'{Path.GetFileName(filePath)}\'", "Unknown file type!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return actualFormat;
            }

            if (actualFormat == requestedFormat && openMethod != null)                   //user specified file type or we figured it out after user specified FileFormat.Any
            {
                openMethod();
            }

            return actualFormat;
        }
        //////////////////////////////// Saving Things ////////////////////////////////
        private void saveSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Logger.TraceInformation("The SkaaImageBox.Image object cannot be null!");


            bool changes = CheckSpriteForPendingChanges(this.ActiveProject?.ActiveSprite?.GameObject);
            if (changes)
            {
                SaveFile(FileFormats.SpriteSpr);
            }
        }
        private void saveGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                Logger.TraceInformation("Failed to save GameSet. There is no ActiveProject.");
            this.SaveFile(FileFormats.GameSet);
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                Logger.TraceInformation("Failed to save project. There is no ActiveProject.");

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Choose or create a directory in which to store your new files.";
                dlg.SelectedPath = props.ProjectDirectory == null || this._tempProjectFolder ? props.ProjectsDirectory : props.ProjectDirectory;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string projectName = Path.GetDirectoryName(dlg.SelectedPath);

                    props.ProjectDirectory = dlg.SelectedPath;
                    this._tempProjectFolder = false;

                    if (!Directory.Exists(props.ProjectDirectory))
                        Directory.CreateDirectory(props.ProjectDirectory);
                }
            }
        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null)
                TrySaveCloseProject(sender, e);
        }
        private void exportAllFramesToPngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
            {
                Logger.TraceInformation("The SkaaImageBox.Image object cannot be null!");
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG Images (*.png)|*.png";
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap bmp = this.ActiveProject.ActiveSprite.GameObject.ToBitmap())
                    {
                        using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                        {
                            bmp.Save(fs, ImageFormat.Png);
                        }
                    }
                }
            }
        }
        private void exportCurFrameToPngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
            {
                Logger.TraceInformation("The SkaaImageBox.Image object cannot be null!");
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG Images (*.png)|*.png";
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject.ActiveFrame.GameObject.IndexedBitmap.Bitmap = (this.imageEditorBox.Image as Bitmap);

                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                        this.imageEditorBox.Image.Save(fs, ImageFormat.Png);
                }
            }
        }
        private void saveSpriteFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: add export ASCII art
            //With ballista adds \n after every 62d character). 
            //var hex = BitConverter.ToString(frame.FrameData);

            if (this.imageEditorBox.Image == null)
                Logger.TraceInformation("ImageEditorBox.Image object cannot be null!");

            this.SaveFile(FileFormats.SpriteFrameSpr);
        }
        private void SaveFile(FileFormats format)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ProjectDirectory == null || this._tempProjectFolder ? props.ProjectsDirectory : props.ProjectDirectory;

                switch (format)
                {
                    case FileFormats.GameSet:
                        dlg.Filter = $"7KAA Game Set Files|*{props.SetFileExtension}";
                        dlg.DefaultExt = props.SetFileExtension;
                        dlg.FileName = "std.set";
                        ShowSaveFileDialog(dlg, () => this.ActiveProject.ActiveGameSet.SaveStandardGameSet(dlg.FileName));
                        break;
                    case FileFormats.SpritePNG:
                        dlg.DefaultExt = ".png";
                        dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
                        dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;
                        ShowSaveFileDialog(dlg, () => oldProject.Export(dlg.FileName, this.ActiveProject.ActiveSprite));
                        break;
                    case FileFormats.FramePNG:
                        dlg.DefaultExt = ".png";
                        dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
                        dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId + "_frame";
                        ShowSaveFileDialog(dlg, () => oldProject.Export(dlg.FileName, this.ActiveProject.ActiveFrame));
                        break;
                    case FileFormats.SpriteSpr:
                        dlg.DefaultExt = props.SprFileExtension;
                        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
                        dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;
                        ShowSaveFileDialog(dlg, () => oldProject.Save(dlg.FileName, this.ActiveProject.ActiveSprite));
                        break;
                    case FileFormats.SpriteFrameSpr:
                        dlg.DefaultExt = props.SprFileExtension;
                        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
                        dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;
                        ShowSaveFileDialog(dlg, () => oldProject.Save(dlg.FileName, this.ActiveProject.ActiveFrame));
                        break;
                    case FileFormats.DbaseIII:
                        dlg.DefaultExt = props.ResFileExtension;
                        dlg.Filter = $"7KAA Resource Files|*{props.ResFileExtension}|All Files (*.*)|*.*";
                        //todo: allow user to specify DataTable
                        break;
                    case FileFormats.ResIdxMultiBmp:
                        dlg.DefaultExt = props.ResFileExtension;
                        dlg.Filter = $"7KAA Resource Files|*{props.ResFileExtension}|All Files (*.*)|*.*";
                        dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;
                        ShowSaveFileDialog(dlg, () => this.ActiveProject.SaveResIdxMultiBmp(dlg.FileName));
                        break;
                    case FileFormats.Unknown:
                        break;
                }
            }
        }

        private void ShowSaveFileDialog(SaveFileDialog dlg, Action save)
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //this.toolStripStatLbl.Text = "Saving...";
                save();
                //this.toolStripStatLbl.Text = string.Empty;
                //AddDebugArg(Misc.GetCurrentMethod(), Path.GetFullPath(dlg.FileName));
            }
        }
        //////////////////////////////// Other Things ////////////////////////////////
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm abt = new AboutForm())
            {
                abt.StartPosition = FormStartPosition.CenterParent;
                abt.ShowDialog();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null)
                this.closeProjectToolStripMenuItem_Click(sender, e);

            this.Close();
        }
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.imageEditorBox.ShowPixelGrid = !this.imageEditorBox.ShowPixelGrid;
            (sender as ToolStripMenuItem).Checked = this.imageEditorBox.ShowPixelGrid;
        }
        #endregion

        #region Other Event Handlers
        //////////////////////////////// Frame/Sprite Updates ////////////////////////////////
        //private void timelineControl_ActiveFrameChanged(object sender, EventArgs e)
        //{
        //    //todo: look into this and refactor as necessary... it's just bad design
        //    //will end up setting ActiveFrame twice since this will be called because of ActiveProject_ActiveFrameChanged
        //    //but it's needed for the tracking bar to be able to make this update
        //    this.ActiveProject.ActiveFrame = this.ActiveProject.ActiveSprite?.Frames[this.timelineControl1.GetActiveFrameIndex()];
        //}
        //private void ActiveSprite_SpriteUpdated(object sender, EventArgs e) { }
        private void ActiveProject_ActiveSpriteChanged(object sender, EventArgs e)
        {
            //todo: implement Undo/Redo from here with pairs of old/new sprites
            //this.spriteViewer1.SetFrameList(this.ActiveProject?.ActiveSprite?.GetIFrames());
            this.ActiveProject.ActiveFrame = this.ActiveProject?.ActiveSprite?.Frames[0];

            //since a sprite has been un/loaded
            SetupUI();
        }
        private void ActiveProject_ActiveFrameChanged(object sender, EventArgs e)
        {
            if (this.ActiveProject?.ActiveFrame == null)
            {
                this.imageEditorBox.Image = null;
            }
            else
            {
                this.imageEditorBox.Image = this.ActiveProject.ActiveFrame.GameObject.IndexedBitmap.Bitmap;

                //if (!this.spriteViewer1.SetCurrentFrameTo(this.imageEditorBox.Image))
                //    throw new Exception("Failed to update TimelineControl as the specified image does not exist in the List!");
            }
        }
        //////////////////////////////// Editing/Drawing UI ////////////////////////////////
        private void DrawingToolbox_SelectedToolChanged(object sender, EventArgs e)
        {
            this.imageEditorBox.ChangeToolMode(sender, e);
        }
        private void imageEditorBox_ImageChanged(object sender, EventArgs e)
        {
            SetupUI();
        }
        private void imageEditorBox_ImageUpdated(object sender, EventArgs e)
        {
            if (this.imageEditorBox.SelectedTool != DrawingTools.Pan &&
                this.imageEditorBox.SelectedTool != DrawingTools.None)
            {
                this.spriteViewer1.UpdateFrame(this.ActiveProject.ActiveFrame);
                //this.ActiveProject.ActiveFrame.IndexedBitmap.PendingChanges = true;
                //this.timelineControl1.UpdateCurrentFrame(this.ActiveProject.ActiveFrame.IndexedBitmap.Bitmap);
            }
        }
        private void ColorGridChooser_ColorChanged(object sender, EventArgs e)
        {
            //todo: update this call with the secondary color, once implemented
            SetActiveColors((sender as ColorGrid).Color, Color.FromArgb(0, 0, 0, 0));
        }
        //////////////////////////////// Project/Environment ////////////////////////////////
        private void SkaaEditorMainForm_ActiveProjectChanged(object sender, EventArgs e)
        {
            SetupUI();
        }
        private void ActiveProject_PaletteChanged(object sender, EventArgs e)
        {
            SetupUI();
            SetDefaultActiveColors();
        }
        private void SkaaEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.WriteLine($"MainForm closing. Reason: {e.CloseReason}");

            foreach (string dir in this._tempFiles)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    Trace.WriteLine($"Temp directory deleted: {dir}");
                }
            }
            Logger.TraceEvent(TraceEventType.Start, 0, $"Log ended: {string.Concat(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())}\r\n\r\n");

            try
            {
                props.Save();
            }
            catch (ArgumentException)
            {
                //the user deleted the settings file while running
                //todo: handle this more elegantly
            }
        }
        #endregion

        #region Project Management    
        private void MakeTempFolder()
        {
            if (props.TempDirectory == string.Empty)
            {
                string tempPath = Path.GetTempPath();
                string randomFile = Path.GetRandomFileName();
                props.TempDirectory = tempPath + "SkaaEditor." + randomFile + '\\';
            }

            if (!Directory.Exists(props.TempDirectory))
            {
                if (Directory.CreateDirectory(props.TempDirectory) != null)
                {
                    this._tempFiles.Add(props.TempDirectory);
                    this._tempProjectFolder = true; //todo: this is hackish. Should be more along the lines of bool _projectSavedByUser, _projectAutoSaved
                }
                else
                    throw new Exception($"Failed to create temporary directory: {props.TempDirectory}");
            }
        }
        private void NewProject()
        {
            //use a temporary folder until the user saves
            string projectPath = props.TempDirectory + "\\project_" + DateTime.Now.ToString("yyyyMMddHHmm");
            Directory.CreateDirectory(projectPath);
            this._tempFiles.Add(projectPath);

            oldProject newProject = new oldProject();

            ////need these events to fire before loading the objects
            newProject.ActiveSpriteChanged += ActiveProject_ActiveSpriteChanged;
            newProject.ActiveFrameChanged += ActiveProject_ActiveFrameChanged;
            newProject.PaletteChanged += ActiveProject_PaletteChanged;

            string paletteFile = props.DataDirectory + props.PalStd;

            this.ActiveProject = newProject;
            this.ActiveProject.OpenPalette(paletteFile); //need to call this after setting ActiveProject so ActiveProject isn't null when we set up the ColorGrid
        }
        private void OpenProject(string projectPath)
        {
            Debug.Assert(projectPath != null, "Failed to specify a path to open!");

            //todo: enumerate the files to see what ProjectType it is
            oldProject open = new oldProject();

            props.ProjectDirectory = projectPath;
            open.ProjectName = Path.GetFileName(projectPath); //GetFileName just assumes the last thing is a "file" and will give us the directory name

            //need these events to fire before loading the objects
            //open.ActiveSpriteChanged += ActiveProject_ActiveSpriteChanged;
            //open.ActiveFrameChanged += ActiveProject_ActiveFrameChanged;
            open.PaletteChanged += ActiveProject_PaletteChanged;

            List<string> setFiles = Directory.EnumerateFiles(projectPath, "*.set").ToList();
            if (setFiles.Count > 1) //todo: allow user to select which to load
                Logger.TraceInformation($"User selected a directory with more than one SET file: {projectPath}");

            List<string> sprFiles = Directory.EnumerateFiles(projectPath, "*.spr").ToList();
            if (sprFiles.Count > 1) //todo: allow user to select which to load
                Logger.TraceInformation($"User selected a directory with more than one SPR file: {projectPath}");

            List<string> resFiles = Directory.EnumerateFiles(projectPath, "*.res").ToList();
            if (resFiles.Count > 1) //todo: allow user to select which to load
                Logger.TraceInformation($"User selected a directory with more than one RES file: {projectPath}");

            string paletteFile = props.ProjectDirectory + props.PalStd;

            if (setFiles.Count > 0)
                open.OpenGameSet(setFiles.ElementAt(0));

            this.ActiveProject = open;
            open.OpenPalette(paletteFile); //need to call this after setting ActiveProject so ActiveProject isn't null when we set up the ColorGrid


            if (sprFiles.Count > 0)
            {
                open.ActiveSprite = (SpritePresenter)oldProject.LoadSprite(sprFiles.ElementAt(0), this.ActiveProject.ActivePalette);
                open.SetActiveSpriteSframeDbfDataView();
                //this.spriteViewer1.SetFrameList(this.ActiveProject.ActiveSprite.GetIFrames());
            }
        }
        private void CloseProject()
        {
            this.drawingToolbox.CloseSelectedTool();

            this.ActiveProject.ActiveFrame = null;
            this.ActiveProject.ActiveSprite = null;
            this.ActiveProject.ActivePalette = null;

            ////Note to self: Unsubscribe even if you null the object https://msdn.microsoft.com/en-us/library/ms366768(v=vs.140).aspx.
            //this.ActiveProject.ActiveSpriteChanged -= ActiveProject_ActiveSpriteChanged;
            //this.ActiveProject.ActiveFrameChanged -= ActiveProject_ActiveFrameChanged;
            this.ActiveProject.PaletteChanged -= ActiveProject_PaletteChanged;

            this.spriteViewer1.SetFrameList(null);
            this.imageEditorBox.Image = null;

            this.ActiveProject = null; //do this last so the event fires after nulling imageEditorBox
        }
        /// <summary>
        /// Closes the current project and saves changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        private bool TrySaveCloseProject(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                return true;

            DialogResult saveChanges = UserShouldSaveChanges();

            if (saveChanges == DialogResult.Yes)
            {
                saveProjectToolStripMenuItem_Click(sender, e);
                CloseProject();
                return true;
            }
            else if (saveChanges == DialogResult.No)
            {
                CloseProject();
                return true;
            }
            else // (DialogResult.Cancel)
                return false;
        }
        private DialogResult UserShouldSaveChanges()
        {
            bool spriteHasChanges = CheckSpriteForPendingChanges(this.ActiveProject?.ActiveSprite?.GameObject);

            if (!spriteHasChanges)// && this.ActiveProject?.UnsavedSprites?.Count == 0)
                return DialogResult.No;
            else
                return MessageBox.Show("You have unsaved changes. Do you want to save these changes?", "Save?", MessageBoxButtons.YesNoCancel);
        }
        private bool CheckSpriteForPendingChanges(SkaaSprite spr)
        {
            if (spr == null)
                return false;

            bool frameHasChanges = false;
            foreach (SkaaFrame sf in spr.Frames)
            {
                //frameHasChanges = sf.PendingChanges | frameHasChanges;
            }

            return frameHasChanges;
        }
        #endregion

        #region Helper Methods
        //private void ProcessSpriteUpdates()
        //{
        //    //this.ActiveProject.ProcessUpdates(this.ActiveProject.ActiveFrame, this.imageEditorBox.Image as Bitmap);
        //    //this.ActiveProject.UnsavedSprites.Remove(this.ActiveProject.ActiveSprite);
        //}

        private void SetDefaultActiveColors()
        {
            Color primary = Color.Black, secondary = Color.FromArgb(0, 0, 0, 0);
            SetActiveColors(primary, secondary);

            //todo: detect transparent color in palette: Color.Transparent is {0,255,255,255} (trans white) but we use {0,0,0,0} (trans black) in pal_std.res.

            //Re:below - The comparisons are based on the Color.Name property, which our palette does not provide. Will need to search based on ARGB values instead.
            //Task<bool> FindBlack = Task<bool>.Factory.StartNew(() => this.colorGridChooser.Colors.Contains(Color.Black));
            //Task<bool> FindWhiteTrans = Task<bool>.Factory.StartNew(() => this.colorGridChooser.Colors.Contains(Color.Transparent));
            //Task<bool> FindBlackTrans = Task<bool>.Factory.StartNew(() => this.colorGridChooser.Colors.Contains(Color.FromArgb(0, 255, 255, 255)));
            //Task<Color> NotBlack = Task<Color>.Factory.StartNew(() => this.colorGridChooser.Colors.First(c => c != Color.Black && c != Color.Transparent && c != Color.FromArgb(0, 255, 255, 255)));
            //if (FindBlack.Result == true)
            //    primary = Color.Black;
            //else
            //    primary = NotBlack.Result;
            //if (FindWhiteTrans.Result == true)
            //    secondary = Color.FromArgb(0, 255, 255, 255);
            //else if (FindBlackTrans.Result == true)
            //    secondary = Color.FromArgb(0, 0, 0, 0);
            //else
            //    secondary = NotBlack.Result;
        }
        private void SetActiveColors(Color primary, Color secondary)
        {
            this.imageEditorBox.ActivePrimaryColor = primary;
            this.imageEditorBox.ActiveSecondaryColor = secondary;
        }
        #endregion

        private void btnBrowseGameSet_Click(object sender, EventArgs e)
        {
            BrowseGameSet();
        }

        private string BrowseGameSet()
        {
            string tableToSave = string.Empty;

            using (ObjectListViewContainer olvc = new ObjectListViewContainer())
            {
                //olvc.SetDataSource(this.ActiveProject?.ActiveGameSet?.Tables["SFRAME"]);
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Set", typeof(string)));
                dt.Columns.Add(new DataColumn("TableName", typeof(string)));
                dt.Columns.Add(new DataColumn("Save", typeof(bool)));

                foreach (DataTable dsdt in this.ActiveProject?.ActiveGameSet.Tables)
                {
                    //bool isStandardSet = Path.GetFileName((string) dsdt.ExtendedProperties["FileName"]) == "std.set" ? true : false;
                    var dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr.BeginEdit();
                    dr[0] = Path.GetFileName((string)dsdt.ExtendedProperties["FileName"]);
                    dr[1] = dsdt.TableName;
                    dr[2] = false;
                    dr.AcceptChanges();
                }
                dt.AcceptChanges();
                olvc.SetDataSource(dt);

                if (olvc.ShowDialog() == DialogResult.OK)
                {
                    tableToSave = olvc.TableToSave;
                }
            }

            return tableToSave;
        }
    }
}