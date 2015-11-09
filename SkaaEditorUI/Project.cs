using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SkaaGameDataLib;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;

namespace SkaaEditorUI
{
    [Serializable]
    public class Project
    {
        #region Private Members
        private Properties.Settings props = Properties.Settings.Default;
        private SpriteFrame _activeFrame;
        private SkaaGameSet _activeGameSet;

        [NonSerialized]
        private PaletteResource _skaaEditorPalette;
        [NonSerialized]
        private SpriteResource _skaaEditorSprite;
        #endregion

        #region Event Handlers
        [field: NonSerialized]
        private EventHandler _activeFrameChanged;
        public event EventHandler ActiveFrameChanged
        {
            add
            {
                if (_activeFrameChanged == null || !_activeFrameChanged.GetInvocationList().Contains(value))
                {
                    _activeFrameChanged += value;
                }
            }
            remove
            {
                _activeFrameChanged -= value;
            }
        }
        protected virtual void OnActiveFrameChanged(EventArgs e)
        {
            EventHandler handler = _activeFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        [field: NonSerialized]
        private EventHandler _paletteChanged;
        public event EventHandler PaletteChanged
        {
            add
            {
                if (_paletteChanged == null || !_paletteChanged.GetInvocationList().Contains(value))
                {
                    _paletteChanged += value;
                }
            }
            remove
            {
                _paletteChanged -= value;
            }
        }
        protected virtual void OnPaletteChanged(EventArgs e)
        {
            EventHandler handler = _paletteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        [field: NonSerialized]
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
        protected virtual void OnActiveSpriteChanged(EventArgs e)
        {
            EventHandler handler = _activeSpriteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Public Members
        public Sprite ActiveSprite
        {
            get
            {
                return this._skaaEditorSprite == null ? null : this._skaaEditorSprite.SpriteObject;
            }
            set
            {
                if (this._skaaEditorSprite.SpriteObject != value)
                {
                    this._skaaEditorSprite.SpriteObject = value;
                    OnActiveSpriteChanged(null);
                }
            }
        }
        public SpriteFrame ActiveFrame
        {
            get
            {
                return this._activeFrame;
            }
            set
            {
                if (this._activeFrame != value)
                {
                    this._activeFrame = value;
                    OnActiveFrameChanged(null);
                }
            }
        }
        public ColorPalette ActivePalette
        {
            get
            {
                return this._skaaEditorPalette.ColorPaletteObject;
            }
            set
            {
                if (this._skaaEditorPalette.ColorPaletteObject != value)
                {
                    this._skaaEditorPalette.ColorPaletteObject = value;
                    OnPaletteChanged(EventArgs.Empty);
                }
            }
        }
        public SkaaGameSet ActiveGameSet
        {
            get
            {
                return this._activeGameSet;
            }
            set
            {
                if (this._activeGameSet != value)
                {
                    this._activeGameSet = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new project, optionally loading the default palette and game set.
        /// </summary>
        /// <param name="loadDefaults">True to load pal_std.res and std.set, false otherwise.</param>
        public Project(bool loadDefaults)
        {
            if (loadDefaults)
                Load(props.DataDirectory + props.DefaultPaletteFile, props.DataDirectory + props.DefaultGameSetFile);
        }
        public void Load(string paletteFilePath, string gameSetFilePath)
        {
            //this.ActiveSpriteChanged += Project_ActiveSpriteChanged;
            //this.ActiveFrameChanged += Project_ActiveFrameChanged;

            LoadPalette(paletteFilePath);
            LoadGameSet(gameSetFilePath);
        }

        /// <summary>
        /// Load the default game set file, <see cref="Properties.Settings.DefaultPaletteFile"/>.
        /// </summary>
        public void LoadGameSet() => LoadGameSet(props.DataDirectory + props.DefaultGameSetFile);

        /// <summary>
        /// This function will open a 7KAA SET file. 
        /// </summary>
        /// <param name="filepath">The complete path to the SET file.</param>
        /// <remarks>
        ///  A SET file, like 7KAA's std.set, simply contains multiple dBase III databases stitched together.
        /// </remarks>
        public void LoadGameSet(string filepath)
        {
            this.ActiveGameSet = new SkaaGameSet(filepath, props.TempDirectory);
        }

        /// <summary>
        /// Load the default palette file, <see cref="Properties.Settings.DefaultGameSetFile"/>.
        /// </summary>
        public void LoadPalette() => LoadPalette(props.DataDirectory + props.DefaultGameSetFile);
        /// <summary>
        /// Loads a palette file.
        /// </summary>
        /// <param name="filepath">The specific palette file to load.</param>
        /// <returns>A ColorPalette built from the palette file</returns>
        public ColorPalette LoadPalette(string filepath)
        {
            this._skaaEditorPalette = new PaletteResource();
            this.ActivePalette = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;

            using (FileStream fs = File.OpenRead(filepath))//filepath + '\\' + this._skaaEditorPalette.PaletteFileName))
            {
                fs.Seek(8, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    int r = fs.ReadByte();
                    int g = fs.ReadByte();
                    int b = fs.ReadByte();

                    if (i < 0xf9) //0xf9 is the lowest transparent color byte
                        this.ActivePalette.Entries[i] = Color.FromArgb(255, r, g, b);
                    else          //0xf9 - 0xff
                        this.ActivePalette.Entries[i] = Color.FromArgb(0, r, g, b);
                }
                this._skaaEditorPalette.ResMemoryStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this._skaaEditorPalette.ResMemoryStream);
            }

            return this.ActivePalette;
        }
        /// <summary>
        /// Opens an SPR file and creates a <see cref="Sprite"/> object for it
        /// </summary>
        /// <param name="filepath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="Sprite"/></returns>
        /// <remarks>
        /// The original game code for reading SPR files can be found <code>ResourceDb::init_imported()</code> 
        /// in src/ORESDB.cpp around line 72. The <code>resName</code> will be "sprite\\NAME.SPR". (There's 
        /// no need to follow the call into <code>File::file_open()</code> in OFILE.cpp. Though the files are 
        /// well-structured, they are considered FLAT by 7KAA.
        /// </remarks>
        public Sprite LoadSprite(string filepath)
        {
            if (this.ActivePalette == null)
                return null;

            this._skaaEditorSprite = new SpriteResource();
            Sprite spr = new Sprite(this.ActivePalette);

            using (FileStream spritestream = File.OpenRead(filepath))
            {
                long x = spritestream.Length;

                while (spritestream.Position < spritestream.Length)
                {
                    SpriteFrame frame = new SpriteFrame(spr, spritestream);
                    spr.Frames.Add(frame);
                }

                //this.ActiveSprite = spr; //this ends up firing the event too early. return the spr instead.
                this._skaaEditorSprite.FileName = Path.GetFileName(filepath);
                this._skaaEditorSprite.SprMemoryStream = new MemoryStream();
                spritestream.Position = 0;
                spritestream.CopyTo(this._skaaEditorSprite.SprMemoryStream);
                spritestream.Position = 0;
            }

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
            spr.SpriteDataView = this.ActiveGameSet.GetSpriteDataView(spr.SpriteId);
            spr.MatchFrameOffsets();

            return spr;
        }

        /// <summary>
        /// Serializes the project with a BinaryFormatter
        /// </summary>
        /// <returns>A MemoryStream containing the serialized project data</returns>
        public Stream SaveProject()
        {
            return Serialization.Serialize(this);
        }
        public void SaveProject(string filepath)
        {

            //if (filepath == null)
            //        ProjectZipper.ZipProject(this, this._workingFolder + '\\' + "new_project.skp");
            //    else
            //        ProjectZipper.ZipProject(this, filepath);
        }
        public static Project LoadProject(Stream str)
        {
            return (Project) Serialization.Deserialize(str);
        }
        public static Project LoadProject(string filePath)
        {
            return ProjectZipper.LoadZipProject(filePath);
        }
       
        //public void UpdateGameSet(string tableName)
        //{
            ////making sure all our frames get any new offsets
            //if (this.ActiveSprite != null)
            //{
            //    this.ActiveSprite.BuildSPR();

            //    foreach (SpriteFrame sf in this.ActiveSprite.Frames)
            //    {
            //        //it's got a new offset
            //        if (sf.NewSprBitmapOffset != sf.SprBitmapOffset)
            //        {
            //            sf.GameSetDataRow.BeginEdit();
            //            sf.GameSetDataRow[9] = sf.NewSprBitmapOffset.ToString();
            //            sf.GameSetDataRow.AcceptChanges();
            //            sf.SprBitmapOffset = sf.NewSprBitmapOffset;
            //            sf.NewSprBitmapOffset = 0;
            //        }
            //    }
            //}
            //this.ActiveSprite.SpriteDataView = this.ActiveGameSet.GetSpriteDataView(this.ActiveSprite.SpriteId);
            //this.ActiveGameSet.SaveGameSet();
        //}

        public void UpdateSprite(SpriteFrame sf, Bitmap bmp)
        {
            this.ActiveSprite.ProcessUpdates(this.ActiveFrame, bmp);
            //return true;
        }
    }
}
