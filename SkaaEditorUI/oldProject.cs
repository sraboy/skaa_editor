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
    public partial class oldProject
    {
        public static readonly TraceSource Logger = new TraceSource("oldProject", SourceLevels.All);
        
        #region Private Members
        private Properties.Settings props = Properties.Settings.Default;
        private FramePresenter _activeFrame;
        private ColorPalette _activePalette;
        private SpritePresenter _activeSprite;
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
        public SpritePresenter ActiveSprite
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
        public FramePresenter ActiveFrame
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
        public oldProject() { this.Initialize(); }
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
                if(this.ActiveGameSet.OpenStandardGameSet(fs) == false) return false;
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
        /// Opens an SPR file and creates a <see cref="SkaaSprite"/> object for it
        /// </summary>
        /// <param name="filepath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SkaaSprite"/></returns>
        /// <remarks>
        /// The original game code for reading SPR files can be found <code>ResourceDb::init_imported()</code> 
        /// in src/ORESDB.cpp around line 72. The <code>resName</code> will be "sprite\\NAME.SPR". SPR files are 
        /// are considered <code>FLAT</code> by 7KAA. 
        /// </remarks>
        public static SpritePresenter LoadSprite(string filepath, ColorPalette pal)
        {
            if (pal == null)
            { 
                Logger.TraceEvent(TraceEventType.Error, 0, "Cannot load a Sprite without a specified palette.");
                return null;
            }

            SkaaSprite spr;

            using (FileStream spritestream = File.OpenRead(filepath))
                spr = SkaaSprite.FromSprStream(spritestream, pal);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);

            return new SpritePresenter(spr);
        }
        public static FramePresenter LoadFrame(string filepath, ColorPalette pal)
        {
            SkaaFrame frame = new SkaaFrame();
            frame.IndexedBitmap = new IndexedBitmap(pal);

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                frame.IndexedBitmap.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);
            }

            return new FramePresenter(frame);
        }
        public static Tuple<SpritePresenter, DataTable> LoadResIdxMultiBmp(string filepath, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
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
                    SkaaSpriteFrame sf = new SkaaSpriteFrame(null);
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

            return new Tuple<SpritePresenter, DataTable>( new SpritePresenter(spr), dt );
        }
        public static Tuple<SpritePresenter, DataTable> LoadResDbf(string filepath, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
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

            return new Tuple<SpritePresenter, DataTable>(new SpritePresenter(spr), dt);
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
                        foreach(SkaaFrame f in this.ActiveSprite.Frames)
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

                if (t == typeof(SkaaSprite))
                {
                    byte[] spr_data = (obj as SkaaSprite).ToSprFile();
                    fs.Write(spr_data, 0, Buffer.ByteLength(spr_data));
                }
                else if (t == typeof(SkaaFrame))
                {
                    byte[] spr_data = (obj as SkaaFrame).ToSprFile();
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
            if(obj.GetType() == typeof(SkaaSprite))
                (obj as SkaaSprite).ToBitmap().Save(filepath);
            else if (obj.GetType() == typeof(SkaaFrame))
                (obj as SkaaFrame).IndexedBitmap.Bitmap.Save(filepath);
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
