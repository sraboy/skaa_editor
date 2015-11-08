/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
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

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using SkaaEditorControls;
using System.Drawing.Imaging;
using SkaaGameDataLib;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using MultiColumnComboBox;
using System.Text;
using System.Diagnostics;
using System.Resources;
using static SkaaEditorUI.ErrorHandler;

namespace SkaaEditorUI
{
    public partial class SkaaEditorMainForm : Form
    {
        private Properties.Settings props = Properties.Settings.Default;

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

        private Project _activeProject;
        //todo: add Debug logging throughout
        //todo: Move all error handling to Error()
        private TextWriterTraceListener _debugTxtWriter;

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

        public SkaaEditorMainForm()
        {
            InitializeComponent();
            ConfigSettings();
        }

        /// <summary>
        /// Sets/Resets the UI. Called by the ActiveProjectChanged event.
        /// </summary>
        /// <remarks>
        /// This may be called any time to reset menu items and ensure event subscriptions 
        /// are done. EventHandlers already ensure they are not hooked multiple times.
        /// </remarks>

        /////////////////////////////////// Setup //////////////////////////////////////////
        private void ConfigSettings()
        {
            //            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //#if DEBUG
            //            currentDirectory += @"\..\..\..\_other\working";
            //#else
            //                        currentDirectory += "\\data";
            //#endif

            //string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + '\\' +
            //                    Assembly.GetExecutingAssembly().GetName().Name;

            props.ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
            props.DataDirectory = props.ApplicationDirectory + "data\\";
            props.TempDirectory = props.ApplicationDirectory + "temp\\";
            props.ProjectsDirectory = props.ApplicationDirectory + "projects\\";
            Directory.CreateDirectory(props.TempDirectory);
            _debugTxtWriter = new TextWriterTraceListener(props.ApplicationDirectory + "debug_log.txt");
            Debug.Listeners.Add(_debugTxtWriter);
        }
        private void SetupUI()
        {
            //todo: Allow for changing the palette. Will have to rebuild color chooser and all sprites

            //disallow opening sprites until a palette & set are loaded
            this.openSpriteToolStripMenuItem.Enabled = (this.skaaColorChooser.Palette == null || this.ActiveProject?.ActiveGameSet == null) ? false : true;
            //disable saving until a sprite is loaded
            this.saveSpriteToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null) ? false : true;

            //disable BMP export until a sprite is loaded
            this.exportBmpToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null) ? false : true;
            
            //enable loading a set. once a set is loaded, don't allow loading a new one
            //this.loadSetToolStripMenuItem.Enabled = (this.ActiveProject == null || this.ActiveProject.ActiveGameSet == null) ? true : false;
            //enable loading a palette. once a palette is loaded, don't allow loading a new one
            //this.loadPaletteToolStripMenuItem.Enabled = (this.ActiveProject == null || this.ActiveProject.SuperPal == null || this.ActiveProject.SuperPal.ActivePalette == null) ? true : false;

            
            //some help text until a sprite is loaded
            //this.imageEditorBox.Text = (this.imageEditorBox.Image == null) ? "Edit >> Load Palette\nFile >> Open >> Choose an SPR file.\nReport bugs to steven.lavoiejr@gmail.com" : null;
            //this.imageEditorBox.Text = (this.imageEditorBox.Image == null) ? "File >> Open Sprite >> Choose an SPR file.\nReport bugs to steven.lavoiejr@gmail.com" : null;
            
            //disable the slider until a sprite is loaded
            this.timelineControl.SetSliderEnable((this.imageEditorBox.Image == null) ? false : true);
            //(re)set to the current status in case the setting was changed elsewhere (loaded project)
            this.showGridToolStripMenuItem.Checked = this.imageEditorBox.ShowPixelGrid;

            //event subscriptions
            this.ActiveProjectChanged += SkaaEditorMainForm_ActiveProjectChanged;

            if (this.ActiveProject != null)
            {
                this.ActiveProject.ActiveSpriteChanged += ActiveProject_ActiveSpriteChanged;
                this.ActiveProject.ActiveFrameChanged += ActiveProject_ActiveFrameChanged;
                this.ActiveProject.PaletteChanged += ActiveProject_PaletteChanged;
            }

            this.skaaColorChooser.ActiveColorChanged += skaaColorChooser_ActiveColorChanged;
            this.timelineControl.ActiveFrameChanged += timelineControl_ActiveFrameChanged;
            this.timelineControl.ActiveSpriteChanged += TimelineControl_ActiveSpriteChanged;
            this.imageEditorBox.ImageChanged += imageEditorBox_ImageChanged;
            this.imageEditorBox.ImageUpdated += imageEditorBox_ImageUpdated;
        }
        private void NewProject(bool loadDefaults)
        {
            if (this.ActiveProject == null)
                this.ActiveProject = new Project(loadDefaults);
            else
                closeProjectToolStripMenuItem_Click(null, null);

            ActiveProject_PaletteChanged(null, null);
            SetupUI();
        }
        private void PopulateSpriteList()
        {
            //SFRAME column names:
            //SPRITE ACTION DIR FRAME OFFSET_X OFFSET_Y WIDTH HEIGHT FILENAME BITMAPPTR
            this.cbMultiColumn.DrawMode = DrawMode.OwnerDrawVariable;
            
            if (this.ActiveProject != null && this.ActiveProject.ActiveSprite != null)
            {
                this.cbMultiColumn.Enabled = true;
                this.cbMultiColumn.DataSource = null;
                this.cbMultiColumn.DataSource = this.ActiveProject.ActiveSprite.SpriteDataView;
                this.cbMultiColumn.DisplayMember = "SPRITE";
                this.cbMultiColumn.ValueMember = "ACTION";
            }
            else
            {
                this.cbMultiColumn.DataSource = null;
                this.cbMultiColumn.Enabled = false;
            }

            // todo: try a GetRow(DataTable dt) to SpriteFrame
            // it can iterate through the rows and look for 
            // a matching offset, which needs to be calc'd/stored
            // from the SPR file during reading. If there's no
            // match, go through all the others first, then find
            // the closest one and use that offset difference to 
            // guess for the user, in case they open a previously
            // edited sprite without having saved a GameSet.
        }
        private void skaaEditorMainForm_Load(object sender, EventArgs e)
        {
            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            

            NewProject(true);
        }
        /////////////////////////////////// Loading Events //////////////////////////////////////////
        private void openSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* To see SPR loading in action, view ResourceDb::init_imported() 
            *  in ORESDB.cpp around line 72. The resName will be "sprite\\NAME.SPR".
            * 
            *  No need to follow its call into File::file_open() in OFILE.cpp at 
            *  line 53. Though the files are well-structured, they are considered 
            *  FLAT by 7KAA.
            *
            *  data_buf_size is set to the actual size of the entire file.
            */

            if (this.skaaColorChooser.Palette == null)
                return;

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;

                dlg.Filter = dlg.DefaultExt = props.SpriteFileExtension;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject.ActiveSprite = this.ActiveProject.LoadSprite(dlg.FileName);
                    this.ActiveProject.ActiveSprite.SpriteUpdated += ActiveSprite_SpriteUpdated;

                    this.exportBmpToolStripMenuItem.Enabled = true;
                    this.timelineControl.ActiveSprite = this.ActiveProject.ActiveSprite;
                    this.timelineControl.ActiveFrame = this.ActiveProject.ActiveFrame;
                    this.timelineControl.SetMaxFrames(this.ActiveProject.ActiveSprite.Frames.Count - 1); //-1 for 0-index
                }
            }
        }
        private void openGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.Filter = dlg.DefaultExt = props.GameSetFileExtension;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ActiveProject.LoadGameSet(dlg.FileName);
                }
            }
        }
        //////////////////////////////////// Saving Events //////////////////////////////////////////
        private void saveSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
            {
                string error = "The SkaaImageBox.Image object cannot be null!";
                Debug.WriteLine(error);
                throw new Exception(error);
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.Filter = dlg.DefaultExt = props.SpriteFileExtension;
#if DEBUG
                dlg.FileName = "new_" + this.ActiveProject.ActiveSprite.SpriteId + DateTime.Now.ToString("yyyyMMddHHMM") + dlg.DefaultExt;
#else
                dlg.FileName = "new_" + this.ActiveProject.ActiveSprite.SpriteId + dlg.DefaultExt;
#endif
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    UpdateSprite();

                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        byte[] save = this.ActiveProject.ActiveSprite.BuildSpr();
                        fs.Write(save, 0, Buffer.ByteLength(save));
                    }
                }
            }
        }
        private void saveGameSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
            {
                string error = "ActiveProject cannot be null!";
                Debug.WriteLine(error);
                throw new Exception(error);
            }


            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.Filter = dlg.DefaultExt = props.GameSetFileExtension;
                UpdateSprite();
#if DEBUG
                dlg.FileName = "new_set-" + this.ActiveProject.ActiveSprite.SpriteId + DateTime.Now.ToString("yyyyMMddHHMM") + dlg.DefaultExt;
#else
                dlg.FileName = "new_set-" + this.ActiveProject.ActiveSprite.SpriteId + dlg.DefaultExt;
#endif

                if (dlg.ShowDialog() == DialogResult.OK)
                {        
                    this.ActiveProject.ActiveGameSet.SaveGameSet(dlg.FileName);
                }
            }
        }
        private void exportCurFrameTo32bppBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Error("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.Filter = dlg.DefaultExt = props.SpriteFileExtension;
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId + dlg.DefaultExt;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //updates this frame's ImageBmp based on changes
                    this.ActiveProject.ActiveFrame.ImageBmp = (this.imageEditorBox.Image as Bitmap);

                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                        this.imageEditorBox.Image.Save(fs, ImageFormat.Bmp);
                }
            }
        }
        private void exportAllFramesTo32bppBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                Error("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = props.ApplicationDirectory;
                dlg.Filter = dlg.DefaultExt = props.SpriteFileExtension;
                dlg.FileName = this.ActiveProject.ActiveSprite.SpriteId + dlg.DefaultExt;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    UpdateSprite();

                    int totalFrames = this.ActiveProject.ActiveSprite.Frames.Count;
                    int spriteWidth = 0, spriteHeight = 0, high = 0, low = 0;

                    double sqrt = Math.Sqrt((double) totalFrames);

                    if (totalFrames % 1 != 0) //totalFrames is a perfect square
                    {
                        low = (int) sqrt;
                        high = (int) sqrt;
                    }
                    else
                    {
                        low = (int) Math.Floor(sqrt) + 1; //adds an additional row
                        high = (int) Math.Ceiling(sqrt);
                    }

                    //need the largest height and width to tile the export
                    foreach (SpriteFrame sp in this.ActiveProject.ActiveSprite.Frames)
                    {
                        if (sp.Width > spriteWidth)
                            spriteWidth = sp.Width;
                        if (sp.Height > spriteHeight)
                            spriteHeight = sp.Height;
                    }

                    //calculated height and width of the bitmap
                    //based on tiles of the largest possible size
                    int exportWidth = high * spriteWidth,
                        exportHeight = low * spriteHeight;

                    using (Bitmap bitmap = new Bitmap(exportWidth, exportHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            int frameIndex = 0;

                            for (int y = 0; y < exportHeight; y += spriteHeight)
                            {
                                //once we hit the max frames, just break
                                for (int x = 0; x < exportWidth && frameIndex < this.ActiveProject.ActiveSprite.Frames.Count; x += spriteWidth)
                                {
                                    g.DrawImage(this.ActiveProject.ActiveSprite.Frames[frameIndex].ImageBmp, new Point(x, y));
                                    frameIndex++;
                                }
                            }
                        }

                        using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                            bitmap.Save(fs, ImageFormat.Bmp);
                    }
                }//end if
            }//end using SaveFileDialog
        }
        /////////////////////////// Update Sprite and Frame ///////////////////////////
        private void UpdateSprite()
        {
            this.ActiveProject.UpdateSprite(this.ActiveProject.ActiveFrame, imageEditorBox.Image as Bitmap);
        }
        private void TimelineControl_ActiveSpriteChanged(object sender, EventArgs e)
        {
            this.ActiveProject.ActiveSprite = this.timelineControl.ActiveSprite;
            //timelineControl.ActiveFrame gets changed by ActiveProject_ActiveSpriteChanged()
        }
        private void timelineControl_ActiveFrameChanged(object sender, EventArgs e)
        {
            this.ActiveProject.ActiveFrame = timelineControl.ActiveFrame;
        }
        private void ActiveSprite_SpriteUpdated(object sender, EventArgs e)
        {
            PopulateSpriteList();
        }
        private void ActiveProject_ActiveSpriteChanged(object sender, EventArgs e)
        {
            //todo: implement Undo/Redo from here with pairs of old/new sprites
            this.timelineControl.ActiveSprite = this.ActiveProject.ActiveSprite;
            this.ActiveProject.ActiveFrame = this.ActiveProject.ActiveSprite.Frames[0];
            PopulateSpriteList();
        }
        private void ActiveProject_ActiveFrameChanged(object sender, EventArgs e)
        {
            //SaveActiveFrame();

            if (this.ActiveProject.ActiveFrame == null)
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
        ///////////////////////////////////////////////////////////////////////////////


        private void btnFeatureTest_Click(object sender, EventArgs e)
        {
            //SkaaSAVEditorTest savEditor = new SkaaSAVEditorTest();
            //savEditor.Show();

            UpdateSprite();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm abt = new AboutForm())
                abt.Show();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

     
        private void SkaaEditorMainForm_ActiveProjectChanged(object sender, EventArgs e)
        {
            //sets the palette which causes the color chooser's buttons to be filled
            if (this.ActiveProject?._skaaEditorPalette != null)// && this.ActiveProject.SuperPal != null)
                this.skaaColorChooser.Palette = this.ActiveProject._skaaEditorPalette.ActivePalette;
            else //user has closed the project (it is now null)
            {
                this.imageEditorBox.Image = null;
                this.skaaColorChooser.Palette = null;
            }
            //SetupUI(); //called by imageEditorBox_ImageChanged()
        }
        private void skaaColorChooser_ActiveColorChanged(object sender, EventArgs e)
        {
            this.imageEditorBox.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
        }
        private void cbMultiColumn_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DataRow selection;
            int? offset = null;

            if (/*this.ActiveProject != null &&*/ this.ActiveProject?.ActiveSprite != null)
            {
                selection = (this.cbMultiColumn.SelectedItem as DataRowView).Row;
                offset = Convert.ToInt32(selection[9]);//selection.GetNullableUInt32FromIndex(9);
                this.ActiveProject.ActiveFrame = this.ActiveProject.ActiveSprite.Frames.Find(sf => sf.SprBitmapOffset == offset);
            }

            if (this.ActiveProject?.ActiveFrame == null)
            {
                Error($"Unable to find matching offset in Sprite.Frames for \"{this.ActiveProject.ActiveSprite.SpriteId}\" and offset: {offset.ToString()}.");
            }
        }
        private void imageEditorBox_ImageChanged(object sender, EventArgs e)
        {
            SetupUI();
        }
        private void imageEditorBox_ImageUpdated(object sender, EventArgs e)
        {
            // cbEdit.Checked is used as the equivalent of imageEditorBox.IsDrawing, but
            // IsDrawing is actually set false by the time we get to here.
            if (this.cbEdit.Checked)
            {
                this.timelineControl.PictureBoxImageFrame.Image = imageEditorBox.Image;
                //UpdateSprite();
            }
        }
        private void cbEdit_CheckedChanged(object sender, EventArgs e)
        {
            this.imageEditorBox.EditMode = !this.imageEditorBox.EditMode;
        }
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.imageEditorBox.ShowPixelGrid = !this.imageEditorBox.ShowPixelGrid;
            (sender as ToolStripMenuItem).Checked = this.imageEditorBox.ShowPixelGrid;
        }
        private void ActiveProject_PaletteChanged(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                this.skaaColorChooser.Palette = null;
            else if (this.ActiveProject._skaaEditorPalette != null)
                this.skaaColorChooser.Palette = this.ActiveProject._skaaEditorPalette.ActivePalette;
        }


#region Old Menu Items
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: confirm closing the current project first
            this.ActiveProject = null;
            NewProject(false);
        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveProject = null;
            NewProject(true);
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                throw new ArgumentNullException("There is no ActiveProject! How'd you even do that?");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.DefaultExt = ".skp";
                dlg.FileName = "new_project_" + DateTime.Now.ToString("yyyyMMddHHMM") + ".skp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //this.ActiveProject.SaveProject(dlg.FileName);
                }
            }
        }
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: confirm user doesn't want to save the current changes
            //if (this.ActiveProject != null)
            //{
            //    if (!Confirm())
            //        return;
            //}

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.DefaultExt = ".skp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveProject = Project.LoadProject(dlg.FileName);
                    SetupUI();
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
                    if (this.ActiveProject == null)
                        NewProject(false);

                    this.ActiveProject.LoadPalette(Path.GetDirectoryName(dlg.FileName));
                }

                //this.openToolStripMenuItem.Enabled = true;
            }
        }
        private void loadSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Load the standard set, std.set, and open the SFRAME database. The SFRAME DB is 
             * at offset 0x1FA55-0x7afce. Fittingly, we're using a 15yr old database engine to
             * get at our >15yr old data.
             * 
             * Courtesy to multiple StackOverlow threads for clearing up some issues for me here.
            */

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.FileName = "std.set";
                dlg.DefaultExt = ".set";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dlg.SupportMultiDottedExtensions = true;
                    ActiveProject.LoadGameSet(dlg.FileName);
                }
            }

        }
        private void saveSPRFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: add export ASCII art
            //With ballista adds \n after every 62d character). 
            //var hex = BitConverter.ToString(frame.FrameData);

            if (this.imageEditorBox.Image == null)
                throw new Exception("The SkaaImageBox.Image object cannot be null!");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create)) //truncates the current file if it exists already
                    {
                        byte[] spr_data = this.ActiveProject.ActiveFrame.BuildBitmap8bppIndexed();
                        fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
                    }
                }
            }
        }
#endregion

        private void SkaaEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine($"MainForm closed. Reason: {e.CloseReason}");
            Directory.Delete(props.TempDirectory, true);
        }

    }
}
