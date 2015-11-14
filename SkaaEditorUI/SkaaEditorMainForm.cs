#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
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

namespace SkaaEditorUI
{
    public partial class SkaaEditorMainForm : Form
    {
        //todo: Allow for changing the palette. Will have to rebuild color chooser and all sprites
        //todo: add Debug logging throughout, eliminate "Calculating colors..."
        #region Debugging
#if DEBUG
        //Keep all variables, etc in the #if/#endif tags and create [Conditional("DEBUG")]
        //methods outside the tags. This will prevent us from having to remove/comment out 
        //all the debug-only members as calls to the methods can remain and will not be
        //built in the Release configuration.
        private List<DebugArgs> _debugArgs;
        private class DebugArgs
        {
            public string MethodName;
            public object Arg;
        }
#endif

        /* Keep methods outside the #if so we don't have to remove calls 
         * to them throughout the code or use #if checks there. They'll 
         * always get built but won't be called in release mode. This 
         * isn't possible with non-void methods or event handlers.
         */
        /// <summary>
        /// When debugging, creates and adds a new <see cref="DebugArgs"/> object 
        /// to the <see cref="SkaaEditorMainForm._debugArgs"/> List. This is 
        /// essentially a dirty hack for easy debugging with a global variable.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arg"></param>
        [Conditional("DEBUG")]
        private void AddDebugArg(string methodName, object arg)
        {
            if (this._debugArgs == null)
                this._debugArgs = new List<DebugArgs>();

            this._debugArgs.Add(new DebugArgs() { MethodName = methodName, Arg = arg });
        }
        /// <summary>
        /// Configures settings for debugging/development.
        /// </summary>
        [Conditional("DEBUG")]
        private void ConfigSettingsDebug()
        {
            this._debugArgs = new List<DebugArgs>();
            this.btnDebugAction.Visible = true;
            this.btnDebugAction.Click += btnDebugAction_Click;
            this.lbDebugActions.Visible = true;

            //////////////////////// sraboy-targets on my dev boxes ////////////////////////
            string skaaMilkEnvy = @"E:\Programming\GitHubVisualStudio\7kaa\data\";
            string skaaLemmiwinks = @"E:\Nerd\c_and_c++\7kaa\data";
            if (Directory.Exists(skaaMilkEnvy))
                props.SkaaDataDirectory = skaaMilkEnvy;
            else if (Directory.Exists(skaaLemmiwinks))
                props.SkaaDataDirectory = skaaLemmiwinks;
            if(props.SkaaDataDirectory != string.Empty)
                this.lbDebugActions.Items.Add("SaveAndCopyProject");
            ////////////////////////////////////////////////////////////////////////////////

            this.lbDebugActions.Items.Add("OpenDefaultBallistaSprite");
            this.lbDebugActions.Items.Add("SaveProjectToDateTimeDirectory");
        }
        /// <summary>
        /// Saves the current project files and copies them to the relevant 7KAA
        /// data directories so we can just run 7KAA and ensure the files are 
        /// in the correct format.
        /// </summary>
        [Conditional("DEBUG")]
        private void SaveAndCopyProject() 
        {
            SaveProjectToDateTimeDirectory();

            foreach (DebugArgs arg in this._debugArgs)
            {
                switch (arg.MethodName)
                {
                    case "saveGameSetToolStripMenuItem_Click":
                        string stdSetFile = props.SkaaDataDirectory + "resource\\std.set";
                        if (File.Exists(stdSetFile))
                        {
                            //make sure we have a backup of std.set
                            if (!File.Exists(stdSetFile + ".bak"))
                                File.Copy(stdSetFile, stdSetFile + ".bak");
                        }

                        File.Copy((string)arg.Arg, stdSetFile, true);
                        break;

                    case "saveSpriteToolStripMenuItem_Click":
                        string ballistaFile = props.SkaaDataDirectory + "sprite\\ballista.spr";
                        if (File.Exists(ballistaFile))
                        {
                            //make sure we have a backup of ballista
                            if (!File.Exists(ballistaFile + ".bak"))
                                File.Copy(ballistaFile, ballistaFile + ".bak");
                        }

                        File.Copy((string) arg.Arg, ballistaFile, true);
                        break;
                }
            }
        }
        [Conditional("DEBUG")]
        private void OpenDefaultBallistaSprite()
        {
            string sender = Misc.GetCurrentMethod();
            this._debugArgs = new List<DebugArgs>() { new DebugArgs() { MethodName = "OpenDefaultBallistaSprite", Arg = props.DataDirectory + "ballista.spr" } };
            this.openSpriteToolStripMenuItem_Click(sender, EventArgs.Empty);
            this._debugArgs = null;
        }
        [Conditional("DEBUG")]
        private void SaveProjectToDateTimeDirectory()
        {
            string projectName = "new_project_" + DateTime.Now.ToString("yyyyMMddHHmm");
            props.ProjectDirectory = props.ProjectsDirectory + projectName;

            if (!Directory.Exists(props.ProjectDirectory))
                Directory.CreateDirectory(props.ProjectDirectory);

            object sender = Misc.GetCurrentMethod();

            this.saveSpriteToolStripMenuItem_Click(sender, EventArgs.Empty);
            this.saveGameSetToolStripMenuItem_Click(sender, EventArgs.Empty);
        }
        private void btnDebugAction_Click(object sender, EventArgs e)
        {
            foreach (string debugAction in this.lbDebugActions.SelectedItems)
            {
                Type thisType = this.GetType();
                MethodInfo debugMethod = thisType.GetMethod(debugAction, BindingFlags.Instance | BindingFlags.NonPublic);
                debugMethod.Invoke(this, null);
            }

            this._debugArgs = null;
 
            //this.colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Hue);
            //this.colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Brightness);
            //this.colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Value);
        }
        #endregion

        #region Event Handlers
        private EventHandler _animateChanged;
        public event EventHandler AnimateChanged
        {
            add
            {
                if (_animateChanged == null || !_animateChanged.GetInvocationList().Contains(value))
                {
                    _animateChanged += value;
                }
            }
            remove
            {
                _animateChanged -= value;
            }
        }
        protected virtual void OnAnimateChanged(EventArgs e)
        {
            EventHandler handler = _animateChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
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
        private Project _activeProject;
        private TextWriterTraceListener _debugTxtWriter;
        private Properties.Settings props = Properties.Settings.Default;
        #endregion

        #region Properties
        public Project ActiveProject
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
        public SkaaEditorMainForm() { InitializeComponent(); }
        #endregion

        #region Setup Methods
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //this has to happen early so NewProject() triggers the event.
            this.ActiveProjectChanged += SkaaEditorMainForm_ActiveProjectChanged;

            //this is needed so we can respond when the user uses the tracking bar to change frames
            this.timelineControl.ActiveFrameChanged += timelineControl_ActiveFrameChanged;

            //this event is only called when the entire image changes: during frame changes or loading/closing a sprite
            this.imageEditorBox.ImageChanged += imageEditorBox_ImageChanged;

            //this event is called any time the user edits the image
            this.imageEditorBox.ImageUpdated += imageEditorBox_ImageUpdated;

            //it only shows when zoomed in and that's exactly when it's most needed.
            this.imageEditorBox.ShowPixelGrid = true;
            this.showGridToolStripMenuItem.Checked = true;

            //need to adjust our actions based on the tool selected
            this.drawingToolbox.SelectedToolChanged += DrawingToolbox_SelectedToolChanged;

            ConfigSettings();
            SetupUI();
        }
    
        /// <summary>
        /// Provides for initial application settings like default directories, debug logging, etc.
        /// </summary>
        private void ConfigSettings()
        {
            props.ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';

            _debugTxtWriter = new TextWriterTraceListener(props.ApplicationDirectory + "debug_log.txt");
            Trace.AutoFlush = true;
            Trace.Listeners.Add(_debugTxtWriter);

            props.DataDirectory = props.ApplicationDirectory + "data\\";
            props.TempDirectory = props.ApplicationDirectory + "temp\\";
            props.ProjectsDirectory = props.ApplicationDirectory + "projects\\";
            Directory.CreateDirectory(props.ProjectsDirectory);
            Directory.CreateDirectory(props.TempDirectory);

            ConfigSettingsDebug();
        }
  
        /// <summary>
        /// Loads <see cref="Project.ActivePalette"/>, if specified, and enables/disables the form's <see cref="SkaaColorChooser"/> object based on whether or not a palette is loaded.
        /// </summary>
        private void SetUpColorGrid()
        {
            if (this.ActiveProject?.ActivePalette != null)
            {
                Color[] entries = this.ActiveProject.ActivePalette.Entries;
                IEnumerable<Color> distinct = entries.Distinct();
                this.colorGridChooser.Colors = new ColorCollection(distinct);
                this.colorGridChooser.Colors.Sort(ColorCollectionSortOrder.Value);
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
            this.exportBmpToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null || this.ActiveProject?.ActiveSprite == null) ? false : true;
            this.saveGameSetToolStripMenuItem.Enabled = (this.ActiveProject?.ActiveGameSet == null) ? false : true;

            //need a sprite to navigate a sprite's frames
            this.timelineControl.Enabled = (this.ActiveProject?.ActiveSprite == null) ? false : true;

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

        #region Menu/Toolstrip Button Clicks
        ////////////////////////////// Opening Things //////////////////////////////
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null)
            {
                if (TrySaveCloseProject(null, null))
                    NewProject();
            }
            else
                NewProject();      
        }
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveProject != null)
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
#if DEBUG
            if (sender.ToString() == "OpenDefaultBallistaSprite")
            {
                this.ActiveProject.LoadSprite(this._debugArgs[0].Arg.ToString());
                this.ActiveProject.ActiveSprite.SpriteUpdated += ActiveSprite_SpriteUpdated;
                this.exportBmpToolStripMenuItem.Enabled = true;
                this.timelineControl.ActiveSprite = this.ActiveProject.ActiveSprite;
                this.timelineControl.ActiveFrame = this.ActiveProject.ActiveFrame;
                return;
            }
#endif
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = props.SpriteFileExtension;
                dlg.Filter = $"7KAA Sprite Files (.spr)|*{props.SpriteFileExtension}";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject.LoadSprite(dlg.FileName);
                    this.ActiveProject.ActiveSprite.SpriteUpdated += ActiveSprite_SpriteUpdated;

                    this.exportBmpToolStripMenuItem.Enabled = true;
                    this.timelineControl.ActiveSprite = this.ActiveProject.ActiveSprite;
                    this.timelineControl.ActiveFrame = this.ActiveProject.ActiveFrame;
                }
            }
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //force closing the sprite for now
            //todo: ask user to associate set to sprite
            if (this.ActiveProject?.ActiveSprite != null && this.ActiveProject?.ActiveGameSet != null)
            {
                string msg = "This will close the current sprite. Continue?";

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

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = props.GameSetFileExtension;
                dlg.Filter = $"7KAA Game Set Files (.set)|*{props.GameSetFileExtension}";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject.LoadGameSet(dlg.FileName);
                }
            }
        }
        //////////////////////////////// Saving Things ////////////////////////////////
        private void saveSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Misc.Error("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ProjectDirectory == null ? props.ProjectsDirectory : props.ProjectDirectory;
                dlg.DefaultExt = props.SpriteFileExtension;
                dlg.Filter = $"7KAA Sprite Files (.spr)|*{props.SpriteFileExtension}";
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.toolStripStatLbl.Text = "Building Sprite...";
                    ProcessSpriteUpdates();

                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        byte[] save = this.ActiveProject.ActiveSprite.Resource.SprData;
                        fs.Write(save, 0, Buffer.ByteLength(save));

                        AddDebugArg(Misc.GetCurrentMethod(), Path.GetFullPath(dlg.FileName));
                    }
                    this.ActiveProject.UnsavedSprites.Remove(this.ActiveProject.ActiveSprite);
                    this.toolStripStatLbl.Text = string.Empty;
                }
            }
        }
        private void saveGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                Misc.Error("ActiveProject cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ProjectDirectory == null ? props.ProjectsDirectory : props.ProjectDirectory;
                dlg.DefaultExt = props.GameSetFileExtension;
                dlg.Filter = $"7KAA Game Set Files (.set)|*{props.GameSetFileExtension}";

                if (this.ActiveProject.ActiveSprite != null && this.ActiveProject.ActiveFrame != null)
                {
                    this.toolStripStatLbl.Text = "Building Sprite...";
                    ProcessSpriteUpdates();
                    this.toolStripStatLbl.Text = string.Empty;
                }
                else
                    return;

                dlg.FileName = "std.set";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.toolStripStatLbl.Text = "Saving Game Set...";
                    this.ActiveProject.ActiveGameSet.SaveGameSet(dlg.FileName);
                    this.toolStripStatLbl.Text = string.Empty;

                    AddDebugArg(Misc.GetCurrentMethod(), Path.GetFullPath(dlg.FileName));
                }
            }
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                Misc.Error("There is no ActiveProject!");

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Choose or create a directory in which to store your new files.";
                dlg.SelectedPath = props.ProjectDirectory == null ? props.ProjectsDirectory : props.ProjectDirectory;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string projectName = Path.GetDirectoryName(dlg.SelectedPath);
                    props.ProjectDirectory = dlg.SelectedPath;

                    if (!Directory.Exists(props.ProjectDirectory))
                        Directory.CreateDirectory(props.ProjectDirectory);

                    this.saveSpriteToolStripMenuItem_Click(Misc.GetCurrentMethod(), EventArgs.Empty);
                    this.saveGameSetToolStripMenuItem_Click(Misc.GetCurrentMethod(), EventArgs.Empty);
                }
            }
        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveProject != null)
                TrySaveCloseProject(sender, e);
        }
        //////////////////////////////// Other Things ////////////////////////////////
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm abt = new AboutForm())
                abt.Show();
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

        #region Change Events
        //////////////////////////////// Frame/Sprite Updates ////////////////////////////////
        private void timelineControl_ActiveFrameChanged(object sender, EventArgs e)
        {
            //will end up setting ActiveFrame twice since this will be called because of ActiveProject_ActiveFrameChanged
            //but it's needed for the tracking bar to be able to make this update
            this.ActiveProject.ActiveFrame = timelineControl.ActiveFrame;
        }
        private void ActiveSprite_SpriteUpdated(object sender, EventArgs e) { }
        private void ActiveProject_ActiveSpriteChanged(object sender, EventArgs e)
        {
            //todo: implement Undo/Redo from here with pairs of old/new sprites
            this.timelineControl.ActiveSprite = this.ActiveProject?.ActiveSprite;
            this.ActiveProject.ActiveFrame = this.ActiveProject?.ActiveSprite?.Frames[0];

            //since a sprite has been un/loaded
            SetupUI();
        }
        private void ActiveProject_ActiveFrameChanged(object sender, EventArgs e)
        {
            if (this.ActiveProject?.ActiveFrame == null)
            {
                this.imageEditorBox.Image = null;
                this.timelineControl.ActiveFrame = null;
            }
            else
            {
                this.imageEditorBox.Image = this.ActiveProject.ActiveFrame.ImageBmp;
                this.timelineControl.ActiveFrame = this.ActiveProject.ActiveFrame;
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
            if (this.imageEditorBox.ToolMode != ToolModes.Pan && 
                this.imageEditorBox.ToolMode != ToolModes.None)
            {
                FrameIsEdited(this.ActiveProject.ActiveFrame);
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
            SetUpColorGrid();
            SetDefaultActiveColors();
        }
        private void SkaaEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.WriteLine($"MainForm closed. Reason: {e.CloseReason}");

#if !DEBUG
            Directory.Delete(props.TempDirectory, true);
            Trace.WriteLine($"Temp directory wiped: {props.TempDirectory}");
#endif
        }
        #endregion

        #region Old Menu Items
        //todo: re-implement these features
        private void exportAllFramesTo32bppBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Misc.Error("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = ".bmp";
                dlg.Filter = $"Bitmap Images (.bmp)|*bmp";
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ProcessSpriteUpdates();

                    using (Bitmap bmp = SprDataHandlers.SpriteToBmp(this.ActiveProject.ActiveSprite))
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                        bmp.Save(fs, ImageFormat.Bmp);
                }
            }
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.FileName = "pal_std.res";
                dlg.DefaultExt = ".res";
                dlg.SupportMultiDottedExtensions = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject.LoadPalette(Path.GetDirectoryName(dlg.FileName));
                }
            }
        }
        private void saveSPRFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: add export ASCII art
            //With ballista adds \n after every 62d character). 
            //var hex = BitConverter.ToString(frame.FrameData);

            if (this.imageEditorBox.Image == null)
                Misc.Error("ImageEditorBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        ProcessSpriteUpdates();
                        byte[] spr_data = this.ActiveProject.ActiveFrame.FrameRawData;
                        fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
                    }
                }
            }
        }
        private void exportCurFrameTo32bppBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Misc.Error("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.DefaultExt = ".bmp";
                dlg.Filter = $"Bitmap Images (.bmp)|*bmp";
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //updates this frame's ImageBmp based on changes
                    this.ActiveProject.ActiveFrame.ImageBmp = (this.imageEditorBox.Image as Bitmap);

                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                        this.imageEditorBox.Image.Save(fs, ImageFormat.Bmp);
                }
            }
        }
        #endregion

        #region Helper Methods
        
        private void ProcessSpriteUpdates()
        {
            this.ActiveProject.ActiveSprite.Resource.ProcessUpdates(this.ActiveProject.ActiveFrame, imageEditorBox.Image as Bitmap);
        }
        /// <summary>
        /// This method marks the frame as requiring updates. When the parent sprite processes edits/changes,
        /// the frame rebuilds its FrameRawData byte array.
        /// </summary>
        /// <param name="sf">The <see cref="SpriteFrame"/> to mark as requiring updates.</param>
        /// <remarks>
        /// Setting <see cref="Sprite.PendingChanges"/> ensures we only rebuild the <see cref="SpriteFrame.FrameRawData"/> 
        /// arrays for frames actually having edits. The only thing unedited framesmay  need to update is their new offset 
        /// value for the SET file, if they follow any edited frames in the file, since the sizes of the preceding frames change.
        /// </remarks>
        private void FrameIsEdited(SpriteFrame sf)
        {
            //Currently only called from imageEditorBox_ImageUpdated
            sf.PendingChanges = true;

            //todo: implement an UpdateImage() method in Timelinecontrol
            //Update the TimeLineControl so the user can see the changes in the size it will be viewed in the game
            this.timelineControl.PictureBoxImageFrame.Image = imageEditorBox.Image;
        }
        private void SetDefaultActiveColors()
        {
            Color primary = Color.Black, secondary = Color.FromArgb(0,0,0,0);
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

        #region Project Management    
        private void NewProject(string paletteFilePath = null, string gameSetFilePath = null)
        {
            if (paletteFilePath == null || gameSetFilePath == null)
            {
                paletteFilePath = props.DataDirectory + props.DefaultPaletteFile;
                gameSetFilePath = props.DataDirectory + props.DefaultGameSetFile;
            }

            OpenProject(string.Empty);
        }
        private void OpenProject(string projectPath)
        { 
            Project open = new Project();

            if (projectPath == string.Empty || projectPath == props.ProjectsDirectory)
            {
                string dir = props.ProjectsDirectory + DateTime.Now.ToString("yyyyMMddHHmm");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                else
                {
                    Misc.Error("No directory created!");
                    dir = props.ProjectsDirectory + DateTime.Now.ToString("yyyyMMddHHmmfff");
                    Directory.CreateDirectory(dir);
                }

                projectPath = dir;
            }

            props.ProjectDirectory = projectPath;
            open.ProjectName = Path.GetFileName(projectPath); //GetFileName just assumes the last thing is a "file" and will give us the directory name

            List<string> setFiles = Directory.EnumerateFiles(projectPath, "*.set").ToList();
            if (setFiles.Count > 1) //todo: allow user to select which to load
                Misc.Error("Please select a directory with only one SET file!");

            List<string> sprFiles = Directory.EnumerateFiles(projectPath, "*.spr").ToList();
            if (sprFiles.Count > 1) //todo: allow user to select which to load
                Misc.Error("Please select a directory with only one SPR file!");


            string paletteFile = props.ProjectDirectory + props.DefaultPaletteFile;
            if (!File.Exists(paletteFile))
                paletteFile = props.DataDirectory + props.DefaultPaletteFile;

            open.ActiveSpriteChanged += ActiveProject_ActiveSpriteChanged;
            open.ActiveFrameChanged += ActiveProject_ActiveFrameChanged;
            open.PaletteChanged += ActiveProject_PaletteChanged;

            if (setFiles.Count > 0)
                open.LoadGameSet(setFiles.ElementAt(0));


            this.ActiveProject = open;
            open.LoadPalette(paletteFile); //need to call this after setting ActiveProject so ActiveProject isn't null when we set up the ColorGrid

            if (sprFiles.Count > 0)
                open.LoadSprite(sprFiles.ElementAt(0));
        }
        private void CloseProject()
        {
            this.drawingToolbox.CloseSelectedTool();

            this.ActiveProject.ActiveFrame = null;
            this.ActiveProject.ActiveSprite = null;
            this.ActiveProject.ActivePalette = null;

            //Note to self: Unsubscribe even if you null the object https://msdn.microsoft.com/en-us/library/ms366768(v=vs.140).aspx.
            this.ActiveProject.ActiveSpriteChanged -= ActiveProject_ActiveSpriteChanged;
            this.ActiveProject.ActiveFrameChanged -= ActiveProject_ActiveFrameChanged;
            this.ActiveProject.PaletteChanged -= ActiveProject_PaletteChanged;

            this.timelineControl.ActiveFrame = null;
            this.timelineControl.ActiveSprite = null;
            this.imageEditorBox.Image = null;

            this.ActiveProject = null; //do this last so the event fires after nulling imageEditorBox
        }
        /// <summary>
        /// Closes the current project and saves changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        private bool TrySaveCloseProject(object sender, EventArgs e)
        {
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
            bool spriteHasChanges = CheckSpriteForPendingChanges(this.ActiveProject?.ActiveSprite);

            if (!spriteHasChanges && this.ActiveProject?.UnsavedSprites?.Count == 0)
                return DialogResult.No;
            else
                return MessageBox.Show("You have unsaved changes. Do you want to save these changes?", "Save?", MessageBoxButtons.YesNoCancel);
        }
        private bool CheckSpriteForPendingChanges(Sprite spr)
        {
            if (spr == null) return false;

            bool frameHasChanges = false;
            foreach (SpriteFrame sf in spr.Frames)
            {
                frameHasChanges = sf.PendingChanges | frameHasChanges;
            }

            return frameHasChanges;
        }
        #endregion

        #endregion
    }
}
