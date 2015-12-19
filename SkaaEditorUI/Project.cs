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
    /// <summary>
    /// Represents a set of open files and settings.
    /// </summary>
    [Serializable]
    public partial class Project
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

        /*
        * Note OpenXYZ vs LoadXYZ. 
        * OpenXYZ methods are setting up the project by changing the palette, etc. They will 
        * change its state are are necessary for future operations with the Project. These 
        * should be used sparingly.
        * 
        * LoadXYZ methods are static and return the loaded objects for the caller to handle.
        * These are preferred to reduce the complexity of this class and because they may be used
        * be independent of the project. 
        *
        * Ann odd method here is LoadResIdxMultiBmp(). It follows the convention but requires the 
        * caller to set up the ActiveGameSet as necessary. This is because, unlike Sprite with the 
        * standard GameSet (std.set), these files contain their own DataTable.
        */

        public void OpenGameSet() => OpenGameSet(props.DataDirectory + props.SetStd);
        /// <summary>
        /// This function will open the specified 7KAA SET file.
        /// </summary>
        /// <param name="filepath">The complete path to the SET file.</param>
        /// <remarks>
        ///  A SET file, like 7KAA's std.set, simply contains multiple dBase III databases stitched together.
        /// </remarks>
        public bool OpenGameSet(string filepath)
        {
            if (!File.Exists(filepath))
                filepath = props.DataDirectory + props.SetStd;

            using (FileStream fs = GameSetFile.Open(filepath))
            {
                //todo: test reading additional set files into the same DataSet
                if(this.ActiveGameSet == null)
                    this.ActiveGameSet = new DataSet();

                if(this.ActiveGameSet.OpenGameSet(fs) == false) return false;
            }

            SetActiveSpriteSframeDbfDataView();

            return false;
        }
        /// <summary>
        /// Loads a palette file.
        /// </summary>
        /// <param name="filepath">The specific palette file to load.</param>
        /// <returns>A ColorPalette built from the palette file</returns>
        public void OpenPalette(string filepath)
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
        public static Sprite LoadSprite(string filepath, ColorPalette pal)
        {
            if (pal == null)
            { 
                Logger.TraceEvent(TraceEventType.Error, 0, "Cannot load a Sprite without a specified palette.");
                return null;
            }

            Sprite spr;

            using (FileStream spritestream = File.OpenRead(filepath))
                spr = Sprite.FromSprStream(spritestream, pal);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);

            return spr;
        }
        public static Frame LoadFrame(string filepath, ColorPalette pal)
        {          
            SpriteFrame frame = new SpriteFrame(null);
            frame.IndexedBitmap = new IndexedBitmap(pal);

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                frame.IndexedBitmap.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);
            }

            return frame;
        }
        public static Tuple<Sprite, DataSet> LoadResIdxMultiBmp(string filepath, ColorPalette pal)
        {
            Sprite spr = new Sprite();
            DataTable dt = new DataTable();
            dt.ExtendedProperties.Add("FileName", filepath);
            dt.Columns.Add(new DataColumn() { DataType = typeof(string), ColumnName = "FrameName" });
            dt.Columns.Add(new DataColumn() { DataType = typeof(uint), ColumnName = "FrameOffset" });

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                Dictionary<string, uint> dic = ResourceDatabase.ReadDefinitions(fs, true);
                spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
                dt.TableName = spr.SpriteId;

                foreach (string key in dic.Keys)
                {
                    fs.Position = dic[key];
                    SpriteFrame sf = new SpriteFrame(null);
                    sf.Name = key;
                    IndexedBitmap iBmp = new IndexedBitmap(pal);
                    sf.IndexedBitmap = iBmp;
                    iBmp.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);
                    
                    spr.Frames.Add(sf);

                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);
                    row.BeginEdit();
                    row["FrameName"] = key;
                    row["FrameOffset"] = dic[key];
                    row.AcceptChanges();
                }
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return new Tuple<Sprite, DataSet>( spr, ds );
        }
        public static Tuple<Sprite, DataSet> LoadResDbf(string filepath, ColorPalette pal)
        {
            Sprite spr = new Sprite();
            DataSet ds = new DataSet();

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                DbfFile file = new DbfFile();

                if(file.ReadStream(fs) != true)
                    throw new Exception("Failed to read DBF file.");

                file.DataTable.TableName = Path.GetFileNameWithoutExtension(filepath);
                file.DataTable.ExtendedProperties.Add("FileName", filepath);
                ds.Tables.Add(file.DataTable);
            }

            return new Tuple<Sprite, DataSet>(spr, ds);
        }

        public void SaveResIdxMultiBmp(string filepath)
        {
            using (FileStream residxstream = new FileStream(filepath, FileMode.Create))
            {
                using (MemoryStream headerstream = new MemoryStream())
                {
                    DataTable dt = this.ActiveGameSet.Tables[this.ActiveSprite.SpriteId];
                    int headersize = (dt.Rows.Count + 1) * ResourceDatabase.ResIdxDefinitionSize;
                    byte[] recordcount = BitConverter.GetBytes((short) dt.Rows.Count + 1); //+1 for the empty record
                    headerstream.Write(recordcount, 0, recordcount.Length);

                    int datalen;

                    using (MemoryStream bmpstream = new MemoryStream())
                    {
                        foreach(Frame f in this.ActiveSprite.Frames)
                        {
                            byte[] framedata = f.ToSprFile();
                            bmpstream.Write(framedata, 0, framedata.Length);
                            //update the frame's [future] offset in the file-to-be-written (needed for ResIdx header)
                            f.BitmapOffset = headersize + bmpstream.Position;
                            //also update that offset in the ActiveGameSet, in case the user keeps working
                            DataRow dr = dt.Select($"FrameName = {f.Name}")[0];
                            dr.BeginEdit();
                            dr["FrameOffset"] = f.BitmapOffset;
                            dr.EndEdit();
                            
                            //write a header entry for each frame
                            ResourceDatabaseWriter.WriteDefinition(dr, headerstream, (uint)f.BitmapOffset, true);
                        }

                        //used to calculate file size below
                        datalen = (int)bmpstream.Length;

                        //write out empty record with file size
                        for (int i = 0; i < ResourceDatabase.ResIdxDefinitionSize; i++)
                            headerstream.WriteByte(0x0); //null name entry
                        byte[] filesize = BitConverter.GetBytes((uint) (headersize + datalen));
                        headerstream.Write(filesize, 0, filesize.Length); //file's size

                        //reset positions and copy streams to write out
                        bmpstream.Position = headerstream.Position = 0;
                        headerstream.CopyTo(residxstream);
                        bmpstream.CopyTo(residxstream);
                    }
                }
            }
        }
        public static void Save(string filepath, Sprite spr)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                byte[] spr_data = spr.ToSprFile();
                fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
            }
        }
        public static void Save(string filepath, Frame f)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                byte[] spr_data = f.ToSprFile();
                fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
            }
        }

        public static void Export(string filepath, Sprite spr)
        {
            spr.ToBitmap().Save(filepath);
        }
        public static void Export(string filepath, Frame f)
        {
            f.IndexedBitmap.Bitmap.Save(filepath);
        }
        
        public void SetActiveSpriteSframeDbfDataView()
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
