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
        private SkaaEditorFrame _activeFrame;
        private ColorPalette _activePalette;
        private SkaaEditorSprite _activeSprite;
        private string _projectName;
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
        public SkaaEditorSprite ActiveSprite
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
        public SkaaEditorFrame ActiveFrame
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
        #endregion

        #region Constructors & Initialization
        public Project() { this.Initialize(); }
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
                this.ActiveGameSet = this.ActiveGameSet ?? new DataSet();
                if(this.ActiveGameSet.OpenGameSet(fs) == false) return false;
            }

            SetActiveSpriteSframeDbfDataView();

            return true;
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
        /// Opens an SPR file and creates a <see cref="SkaaGameSprite"/> object for it
        /// </summary>
        /// <param name="filepath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SkaaGameSprite"/></returns>
        /// <remarks>
        /// The original game code for reading SPR files can be found <code>ResourceDb::init_imported()</code> 
        /// in src/ORESDB.cpp around line 72. The <code>resName</code> will be "sprite\\NAME.SPR". SPR files are 
        /// are considered <code>FLAT</code> by 7KAA. 
        /// </remarks>
        public static SkaaEditorSprite LoadSprite(string filepath, ColorPalette pal)
        {
            if (pal == null)
            { 
                Logger.TraceEvent(TraceEventType.Error, 0, "Cannot load a Sprite without a specified palette.");
                return null;
            }

            SkaaGameSprite spr;

            using (FileStream spritestream = File.OpenRead(filepath))
                spr = SkaaGameSprite.FromSprStream(spritestream, pal);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);

            return new SkaaEditorSprite(spr);
        }
        public static SkaaEditorFrame LoadFrame(string filepath, ColorPalette pal)
        {
            SkaaGameFrame frame = new SkaaGameFrame();
            frame.IndexedBitmap = new IndexedBitmap(pal);

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                frame.IndexedBitmap.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);
            }

            return new SkaaEditorFrame(frame);
        }
        public static Tuple<SkaaEditorSprite, DataTable> LoadResIdxMultiBmp(string filepath, ColorPalette pal)
        {
            SkaaGameSprite spr = new SkaaGameSprite();
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
                    SkaaGameSpriteFrame sf = new SkaaGameSpriteFrame(null);
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

            return new Tuple<SkaaEditorSprite, DataTable>( new SkaaEditorSprite(spr), dt );
        }
        public static Tuple<SkaaEditorSprite, DataTable> LoadResDbf(string filepath, ColorPalette pal)
        {
            SkaaGameSprite spr = new SkaaGameSprite();
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                DbfFile file = new DbfFile();

                if(file.ReadStream(fs) != true)
                    throw new Exception("Failed to read DBF file.");

                file.DataTable.TableName = Path.GetFileNameWithoutExtension(filepath);
                file.DataTable.ExtendedProperties.Add("FileName", filepath);
                dt = file.DataTable;
            }

            return new Tuple<SkaaEditorSprite, DataTable>(new SkaaEditorSprite(spr), dt);
        }

        public void SaveResIdxMultiBmp(string filepath)
        {
            using (FileStream residxstream = new FileStream(filepath, FileMode.Create))
            {
                using (MemoryStream headerstream = new MemoryStream())
                {
                    DataTable dt = this.ActiveGameSet.Tables[this.ActiveSprite.SpriteId];
                    if (dt.TableName != Path.GetFileNameWithoutExtension((string) dt.ExtendedProperties["FileName"]))
                        throw new Exception("TableName does not match file's original file name!");

                    int headersize = (dt.Rows.Count + 1) * ResourceDatabase.ResIdxDefinitionSize;
                    byte[] recordcount = BitConverter.GetBytes((short) dt.Rows.Count + 1); //+1 for the empty record
                    headerstream.Write(recordcount, 0, recordcount.Length);

                    int datalen;

                    using (MemoryStream bmpstream = new MemoryStream())
                    {
                        foreach(SkaaGameFrame f in this.ActiveSprite.Frames)
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
        public static void Save<T>(string filepath, T obj)//Sprite spr)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                Type t = obj.GetType();

                if (t == typeof(SkaaGameSprite))
                {
                    byte[] spr_data = (obj as SkaaGameSprite).ToSprFile();
                    fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
                }
                else if (t == typeof(SkaaGameFrame))
                {
                    byte[] spr_data = (obj as SkaaGameFrame).ToSprFile();
                    fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
                }
                else if (t == typeof(DataTable))
                {
                    DataTable dt = (obj as DataTable);
                    dt.Save(fs);
                }
            }
        }

        public static void Export<T>(string filepath, T obj)
        {
            if(obj.GetType() == typeof(SkaaGameSprite))
                (obj as SkaaGameSprite).ToBitmap().Save(filepath);
            else if (obj.GetType() == typeof(SkaaGameFrame))
                (obj as SkaaGameFrame).IndexedBitmap.Bitmap.Save(filepath);
        }
        
        public void SetActiveSpriteSframeDbfDataView()
        {
            if (this.ActiveSprite != null)
            {
                DataView dv = new DataView(this.ActiveGameSet?.Tables["SFRAME"]);
                if (dv != null)
                {
                    dv.RowFilter = $"SPRITE = '{this.ActiveSprite.SpriteId}'";
                    this.ActiveSprite.SetSpriteDataView(dv);
                }
            }
        }

    }
}
