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
using SkaaColorChooser;
using System.Drawing.Imaging;
using SkaaGameDataLib;
using System.Reflection;
using System.Linq;

namespace SkaaEditor
{
    public partial class SkaaEditorMainForm : Form
    {
        [field: NonSerialized]
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
        private bool _awaitingEdits = false;

        public Project ActiveProject
        {
            get
            {
                return this._activeProject;
            }
            set
            {
                if(this._activeProject != value)
                {
                    this._activeProject = value;                   
                    OnActiveProjectChanged(new EventArgs());
                }
            }
        }

        public SkaaEditorMainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets/Resets the UI. Called by the ActiveProjectChanged event.
        /// </summary>
        /// <remarks>
        /// This may be called any time to reset menu items and ensure event subscriptions 
        /// are done. EventHandlers already ensure they are not hooked multiple times.
        /// </remarks>
        private void SetupUI()
        {
            //todo: Allow for changing the palette. Will have to rebuild color chooser and all sprites

            //disallow opening sprites until a palette is loaded
            this.openSPRToolStripMenuItem.Enabled = (this.skaaColorChooser.Palette == null) ? false : true;
            //disable BMP export until a sprite is loaded
            this.exportBmpToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null) ? false : true;
            //enable loading a set. once a set is loaded, don't allow loading a new one
            this.loadSetToolStripMenuItem.Enabled = (this.ActiveProject == null || this.ActiveProject.ActiveGameSet == null) ? true : false;
            //enable loading a palette. once a palette is loaded, don't allow loading a new one
            this.loadPaletteToolStripMenuItem.Enabled = (this.ActiveProject == null || this.ActiveProject.Palette == null) ? true : false;
            //disable saving until a sprite is loaded
            this.saveSPRToolStripMenuItem.Enabled = (this.imageEditorBox.Image == null) ? false : true;
            //some help text until a sprite is loaded
            //this.imageEditorBox.Text = (this.imageEditorBox.Image == null) ? "Edit >> Load Palette\nFile >> Open >> Choose an SPR file.\nReport bugs to steven.lavoiejr@gmail.com" : null;
            this.imageEditorBox.Text = (this.imageEditorBox.Image == null) ? "File >> Open >> Choose an SPR file.\nReport bugs to steven.lavoiejr@gmail.com" : null;
            //disable the slider until a sprite is loaded
            this.timelineControl.SetSliderEnable((this.imageEditorBox.Image == null) ? false : true);
            //(re)set to the current status in case the setting was changed elsewhere (loaded project)
            this.showGridToolStripMenuItem.Checked = this.imageEditorBox.ShowPixelGrid;

            //event subscriptions
            this.ActiveProjectChanged += SkaaEditorMainForm_ActiveProjectChanged;
            if (this.ActiveProject != null)
            {
                this.ActiveProject.ActiveFrameChanged += ActiveProject_ActiveFrameChanged;
                this.ActiveProject.PaletteChanged += ActiveProject_PaletteChanged;
            }
            this.skaaColorChooser.ActiveColorChanged += skaaColorChooser_ActiveColorChanged;
            this.timelineControl.ActiveFrameChanged += timelineControl_ActiveFrameChanged;
            this.imageEditorBox.ImageChanged += imageEditorBox_ImageChanged;
            this.imageEditorBox.ImageUpdated += imageEditorBox_ImageUpdated;
            this.imageEditorBox.MouseUp += imageEditorBox_MouseUp;
        }

        private void ActiveProject_PaletteChanged(object sender, EventArgs e)
        {
            this.skaaColorChooser.Palette = this.ActiveProject.Palette;
        }

        private void SaveActiveFrame()
        {
            //todo: implement Undo/Redo from here with pairs of old/new frames
            this.ActiveProject.ActiveFrame.ImageBmp = this.imageEditorBox.Image as Bitmap;
            this.ActiveProject.ActiveFrame.FrameData = this.ActiveProject.ActiveFrame.BuildBitmap8bppIndexed();
        }
        private void UpdateImageEditorBoxImage()
        { 
            this.imageEditorBox.Image = this.ActiveProject.ActiveFrame.ImageBmp;
        }
        private void NewProject()
        {
            string workingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
            workingFolder += @"\..\..\..\_other\working";
            if (this.ActiveProject == null)
                this.ActiveProject = new Project(workingFolder, true); //auto-loads pal_std.res and std.set
#else
            workingFolder += "working";
            if(this.ActiveProject == null)
                this.ActiveProject = new Project(workingFolder, false); //does not load default palette and GameSet
#endif
        }

        private void skaaEditorMainForm_Load(object sender, EventArgs e)
        {
            SetupUI();
            NewProject();
        }

        private void skaaColorChooser_ActiveColorChanged(object sender, EventArgs e)
        {
            this.imageEditorBox.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
        }
        private void timelineControl_ActiveFrameChanged(object sender, EventArgs e)
        {
            //todo: save/cache changes to current frame
            this.SaveActiveFrame();
            this.ActiveProject.ActiveFrame = timelineControl.ActiveFrame;
        }        
        private void imageEditorBox_ImageChanged(object sender, EventArgs e)
        {
           SetupUI();
        }
        private void imageEditorBox_ImageUpdated(object sender, EventArgs e)
        {
            // cbEdit.Checked is used as the equivalent for imageEditorBox.IsDrawing,  
            // which is actually false by the time we get to here.
            if (this.cbEdit.Checked)
            {
                this._awaitingEdits = true;
                this.timelineControl.PictureBoxImageFrame.Image = imageEditorBox.Image;
            }                       
        }
        private void SkaaEditorMainForm_ActiveProjectChanged(object sender, EventArgs e)
        {
            //sets the palette which causes the color chooser's buttons to be filled
            if (this.ActiveProject != null)
                this.skaaColorChooser.Palette = this.ActiveProject.Palette;
            else
                this.skaaColorChooser.Palette = null;

            SetupUI();
        }
        private void cbEdit_CheckedChanged(object sender, EventArgs e)
        {
            this.imageEditorBox.EditMode = !this.imageEditorBox.EditMode;
        }
        private void ActiveProject_ActiveFrameChanged(object sender, EventArgs e)
        {
            UpdateImageEditorBoxImage();
        }
        private void imageEditorBox_MouseUp(object sender, MouseEventArgs e)
        {
            // cbEdit.Checked is used as the equivalent for imageEditorBox.IsDrawing,  
            // which is actually false by the time we get to here.
            if (this.cbEdit.Checked && this._awaitingEdits)
            {
                SaveActiveFrame();
            }
        }
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.imageEditorBox.ShowPixelGrid = !this.imageEditorBox.ShowPixelGrid;
            (sender as ToolStripMenuItem).Checked = this.imageEditorBox.ShowPixelGrid;
        }

        #region Loading Events
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null)
            {
                if (!Confirm())
                    return;
            }

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.DefaultExt = ".skp";
                if(dlg.ShowDialog() == DialogResult.OK)
                { 
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open))
                        this.ActiveProject = Project.LoadProject(fs);

                    SetupUI();
                }
            }
            
        }

        private bool Confirm() { return true; } //todo: work on Confirm() after figuring out opening UI
        private void openSPRToolStripMenuItem_Click(object sender, EventArgs e)
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
                dlg.DefaultExt = ".spr";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream spritestream = File.OpenRead(dlg.FileName))
                    { 
                        this.ActiveProject.ActiveSprite = new Sprite(this.skaaColorChooser.Palette);
                        long x = spritestream.Length;

                        while (spritestream.Position < spritestream.Length)
                        {
                            byte[] frame_size_bytes = new byte[8];

                            spritestream.Read(frame_size_bytes, 0, 8);
                    
                            int size = BitConverter.ToInt32(frame_size_bytes, 0);
                            short width = BitConverter.ToInt16(frame_size_bytes, 4);
                            short height = BitConverter.ToInt16(frame_size_bytes, 6);

                            SpriteFrame frame = new SpriteFrame(size, width, height, this.skaaColorChooser.Palette);

                            frame.SetPixels(spritestream);

                            //todo: add export ASCII art
                            //With ballista adds \n after every 62d character). 
                            //Verified alignment of pixels as read.
                            //var hex = BitConverter.ToString(frame.FrameData);

                            frame.BuildBitmap32bpp();

                            this.ActiveProject.ActiveSprite.Frames.Add(frame);
                        }

                        this.exportBmpToolStripMenuItem.Enabled = true;
                    }

                    this.ActiveProject.ActiveFrame = this.ActiveProject.ActiveSprite.Frames[0];
                    this.timelineControl.ActiveSprite = this.ActiveProject.ActiveSprite;
                    this.timelineControl.ActiveFrame = this.ActiveProject.ActiveFrame;
                    this.timelineControl.SetMaxFrames(this.ActiveProject.ActiveSprite.Frames.Count - 1); //-1 for 0-index
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
                    this.ActiveProject.LoadPalette(Path.GetDirectoryName(dlg.FileName));

                this.openToolStripMenuItem.Enabled = true;
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
                dlg.SupportMultiDottedExtensions = true;
                ActiveProject.LoadGameSet(dlg.FileName);
            }

        }
        #endregion

        #region Saving Events
        private void saveSPRFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                throw new ArgumentNullException("The SkaaImageBox.Image object cannot be null! How'd you even do that?");

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
        private void saveSPRAllFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                throw new ArgumentNullException("The SkaaImageBox.Image object cannot be null! How'd you even do that?");

            using (SaveFileDialog dlg = new SaveFileDialog())
            { 
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        byte[] save = this.ActiveProject.ActiveSprite.BuildSPR();
                        fs.Write(save, 0, Buffer.ByteLength(save));
                    }
                }
            }
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                throw new ArgumentNullException("There is no ActiveProject! How'd you even do that?");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.DefaultExt = ".skp";
                dlg.FileName = "SKAA Editor Project.skp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (MemoryStream ms = this.ActiveProject.SaveProject() as MemoryStream)
                    {
                        byte[] array = ms.ToArray();

                        using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                            fileStream.Write(array, 0, array.Length);
                    }
                }
            }
        }
        private void exportCurFrameTo32bppBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageEditorBox.Image == null)
                throw new ArgumentNullException("The SkaaImageBox.Image object cannot be null! How'd you even do that?");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
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
                throw new ArgumentNullException("The SkaaImageBox.Image object cannot be null! How'd you even do that?");

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //updates this frame's ImageBmp based on changes
                    this.ActiveProject.ActiveFrame.ImageBmp = (this.imageEditorBox.Image as Bitmap);
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
        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm abt = new AboutForm())
                abt.Show();
        }
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeProjectToolStripMenuItem_Click(sender, e);
        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject == null)
                throw new ArgumentNullException("There is no ActiveProject! How'd you even do that?");

            this.ActiveProject = null;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SkaaSAVEditorTest savEditor = new SkaaSAVEditorTest();
            savEditor.Show();
        }


    }
}
