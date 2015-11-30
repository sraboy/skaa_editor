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
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SkaaEditorUI
{
    [Serializable]
    public class Project
    {
        public static readonly TraceSource Logger = new TraceSource("Project", SourceLevels.All);
        
        #region Private Members
        private Properties.Settings props = Properties.Settings.Default;
        private Frame _activeFrame;
        private ColorPalette _activePalette;
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
                    OnActiveSpriteChanged(EventArgs.Empty);
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
                    OnActiveFrameChanged(EventArgs.Empty);
                }
            }
        }
        public ColorPalette ActivePalette
        {
            get
            {
                return this._activePalette;
            }
            set
            {
                if (this._activePalette != value)
                {
                    this._activePalette = value;
                    OnPaletteChanged(EventArgs.Empty);
                }
            }
        }
        public DataSet ActiveGameSet;
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

                if(this.ActiveGameSet.Open(fs) == false) return false;
            }

            SetActiveSpriteDataView();

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
            this.ActivePalette = PaletteLoader.FromResFile(filepath);

            if (this.ActivePalette == null)
                Logger.TraceEvent(TraceEventType.Error, 0, $"Failed to load palette: {filepath}");
        }
        /// <summary>
        /// Opens an SPR file and creates a <see cref="Sprite"/> object for it
        /// </summary>
        /// <param name="filepath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="Sprite"/></returns>
        /// <remarks>
        /// The original game code for reading SPR files can be found <code>ResourceDb::init_imported()</code> 
        /// in src/ORESDB.cpp around line 72. The <code>resName</code> will be "sprite\\NAME.SPR". SPR files are 
        /// are considered <code>FLAT</code> by 7KAA. 
        /// </remarks>
        public Sprite OpenSprite(string filepath)
        {
            if (this.ActivePalette == null)
            { 
                Logger.TraceEvent(TraceEventType.Error, 0, "Cannot load a Sprite if the ActivePalette is null.");
                return null;
            }

            Sprite spr;

            using (FileStream spritestream = File.OpenRead(filepath))
                spr = Sprite.FromSprStream(spritestream, this.ActivePalette);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);

            SetActiveSpriteDataView();
            return spr;
        }

        public Sprite LoadResXMultiBmp(string filepath)
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
                    //iBmp.SetBitmapFromRleStream(fs);
                    Debugger.Break(); //need to fix this function
                    spr.Frames.Add(sf);

                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);
                    row.BeginEdit();
                    row["FrameName"] = key;
                    row["FrameOffset"] = dic[key];
                    row.AcceptChanges();
                }
            }

            //this.ProjectType = ProjectTypes.Interface;
            this.ActiveGameSet = new DataSet();
            this.ActiveGameSet.Tables.Add(dt);

            return spr;
        }

        public static void Save(string filename, Sprite spr)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                byte[] spr_data = spr.ToSprFile();
                fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
            }
        }
        public static void Save(string filename, Frame f)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                byte[] spr_data = f.ToSprFile();
                fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
            }
        }

        public static void Export(string filename, Sprite spr)
        {
            spr.ToBitmap().Save(filename);
        }
        public static void Export(string filename, Frame f)
        {
            f.IndexedBitmap.Bitmap.Save(filename);
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
