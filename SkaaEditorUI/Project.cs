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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkaaGameDataLib;
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
        private Frame _activeFrame;
        private PaletteResource _skaaEditorPalette;
        private Sprite _activeSprite;
        private string _projectName;
        //private List<Sprite> _unsavedSprites;
        private ProjectTypes _projectType;
        #endregion

        #region Events
        [NonSerialized]
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
        [NonSerialized]
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
                return this._activeSprite;
            }
            set
            {
                if (this._activeSprite != value)
                {
                    this._activeSprite = value;
                    OnActiveSpriteChanged(null);
                }
            }
        }
        public Frame ActiveFrame
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
                return this._skaaEditorPalette?.ColorPaletteObject;
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
        public DataSet ActiveGameSet;
        //public SkaaGameSet ActiveGameSet
        //{
        //    get
        //    {
        //        return this._activeGameSet;
        //    }
        //    set
        //    {
        //        if (this._activeGameSet != value)
        //        {
        //            this._activeGameSet = value;
        //        }
        //    }
        //}
        //todo: this should be a generic list
        //public List<Sprite> UnsavedSprites
        //{
        //    get
        //    {
        //        return this._unsavedSprites;
        //    }
        //    private set
        //    {
        //        if (this._unsavedSprites != value)
        //            this._unsavedSprites = value;
        //    }
        //}
        public string ProjectName
        {
            get
            {
                return this._projectName;
            }
            set
            {
                if (this._projectName != value)
                {
                    this._projectName = value;
                }
            }
        }
        public ProjectTypes ProjectType
        {
            get
            {
                return _projectType;
            }

            set
            {
                if(this._projectType != value)
                    this._projectType = value;
            }
        }
        #endregion

        #region Constructors & Initialization
        public Project() { this.Initialize(); }
        public Project(ProjectTypes type) { this.ProjectType = type; this.Initialize(); }
        private void Initialize() { /*this.UnsavedSprites = new List<Sprite>();*/ }
        #endregion

        
        public void LoadGameSet() => LoadGameSet(props.DataDirectory + props.SetStd);
        /// <summary>
        /// This function will open the specified 7KAA SET file.
        /// </summary>
        /// <param name="filepath">The complete path to the SET file.</param>
        /// <remarks>
        ///  A SET file, like 7KAA's std.set, simply contains multiple dBase III databases stitched together.
        /// </remarks>
        public bool LoadGameSet(string filepath)
        {
            if (!File.Exists(filepath))
                filepath = props.DataDirectory + props.SetStd;

            using (FileStream fs = GameSetFile.Open(filepath))
            {
                //todo: test reading additional set files into the same DataSet
                if(this.ActiveGameSet == null)
                    this.ActiveGameSet = new DataSet();

                this.ActiveGameSet.ReadSetFile(fs);
            }

            SetActiveSpriteDataView();

            if (this.ActiveGameSet != null)
                return true;
            else
                return false;
        }

        //public void LoadDefaultSpritePalette() => LoadPalette(props.DataDirectory + props.PalStd);
        //public void LoadDefaultMenuPalette() => LoadPalette(props.DataDirectory + props.PalMenu);
        /// <summary>
        /// Loads a palette file.
        /// </summary>
        /// <param name="filepath">The specific palette file to load.</param>
        /// <returns>A ColorPalette built from the palette file</returns>
        public void LoadPalette(string filepath)
        {
            this._skaaEditorPalette = new PaletteResource();

            //have to keep the event from firing before the palette is loaded, so don't mess with ActivePalette yet
            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;

            using (FileStream fs = File.OpenRead(filepath))
            {
                fs.Seek(8, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    int r = fs.ReadByte();
                    int g = fs.ReadByte();
                    int b = fs.ReadByte();

                    if (i < 0xf9) //0xf9 is the lowest transparent color byte
                        pal.Entries[i] = Color.FromArgb(255, r, g, b);
                    else          //0xf9 - 0xff
                        pal.Entries[i] = Color.FromArgb(0, r, g, b);
                }
                this._skaaEditorPalette.ResMemoryStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this._skaaEditorPalette.ResMemoryStream);
            }

            this.ActivePalette = pal;
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
            { 
                Misc.LogMessage("Cannot load a Sprite if the ActivePalette is null.");
                return null;
            }

            //have to keep the event from firing before the sprite is loaded, so don't mess with ActiveSprite yet
            Sprite spr = new Sprite();
            
            using (FileStream spritestream = File.OpenRead(filepath))
            {
                while (spritestream.Position < spritestream.Length)
                {
                    IndexedBitmap iBmp = new IndexedBitmap(this.ActivePalette);
                    SpriteFrame sf = new SpriteFrame(spr);
                    sf.IndexedBitmap = iBmp;
                    spritestream.Position += 4; //skip the int32 size value at the start
                    iBmp.GetBitmapFromRleStream(spritestream);
                    spr.Frames.Add(sf);
                    spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
                }
            }

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);

            this.ActiveSprite = spr;
            SetActiveSpriteDataView();
            return spr;
        }
        public Sprite LoadInterfaceResource(string filepath)
        {
            Sprite spr = new Sprite();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { DataType = typeof(string), ColumnName = "FrameName" });
            dt.Columns.Add(new DataColumn() { DataType = typeof(uint), ColumnName = "FrameOffset" });

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                Dictionary<string, uint> dic = ResourceDatabase.ReadDefinitions(fs);
                spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
                dt.TableName = spr.SpriteId;

                foreach (string key in dic.Keys)
                {
                    fs.Position = dic[key];
                    SpriteFrame sf = new SpriteFrame(spr);

                    IndexedBitmap iBmp = new IndexedBitmap(this.ActivePalette);
                    sf.IndexedBitmap = iBmp;
                    iBmp.GetBitmapFromRleStream(fs);
                    spr.Frames.Add(sf);

                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);
                    row.BeginEdit();
                    row["FrameName"] = key;
                    row["FrameOffset"] = dic[key];
                    row.AcceptChanges();
                }
            }

            this.ProjectType = ProjectTypes.Interface;
            this.ActiveGameSet = new DataSet();
            this.ActiveGameSet.Tables.Add(dt);

            return spr;
        }
        private void SetActiveSpriteDataView()
        {
            if (this.ActiveSprite != null && this.ProjectType == ProjectTypes.Sprite)
            {
                DataView dv = new DataView(this.ActiveGameSet?.Tables["SFRAME"]);
                if (dv != null)
                {
                    dv.RowFilter = string.Format("SPRITE = '{0}'", this.ActiveSprite.SpriteId);
                    this.ActiveSprite.SetSpriteDataView(dv);
                }
            }
        }
    }
}
