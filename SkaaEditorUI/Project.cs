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

namespace SkaaEditor
{
    [Serializable]
    public class Project
    {
        [Serializable]
        public struct SuperPalette
        {
            public string PaletteFileName;
            public MemoryStream PaletteFileMemoryStream;
            [NonSerialized]
            public ColorPalette ActivePalette;

        }
        [field: NonSerialized]
        public struct SuperGameSet
        {
            public string GameSetFileName;
            public MemoryStream GameSetFileMemoryStream;
            public GameSet ActiveGameSet;

        }
        [NonSerialized]
        public SuperPalette PalStruct;
        [NonSerialized]
        public SuperGameSet SetStruct;

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

        private string _workingFolder;
        private MemoryStream _paletteFileMemoryStream;
        private Sprite _activeSprite;
        private SpriteFrame _activeFrame;
        //private GameSet _activeGameSet;
        [NonSerialized]
        //private ColorPalette _palette;
        private DataSet _spriteTables = new DataSet("sprites");

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
                }
            }
        }
        //public GameSet ActiveGameSet
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
                    OnActiveFrameChanged(new EventArgs());
                }
            }
        }

        public ColorPalette ActivePalette
        {
            get
            {
                return this.PalStruct.ActivePalette;
            }
            set
            {
                if (this.PalStruct.ActivePalette != value)
                {
                    this.PalStruct.ActivePalette = value;
                    OnPaletteChanged(new EventArgs());
                }
            }
        }
        public GameSet ActiveGameSet
        {
            get
            {
                return this.SetStruct.ActiveGameSet;
            }
            set
            {
                if (this.SetStruct.ActiveGameSet != value)
                {
                    this.SetStruct.ActiveGameSet = value;
                }
            }
        }
        //public ColorPalette Palette
        //{
        //    get
        //    {
        //        return this._palette;
        //    }
        //    set
        //    {
        //        if (this._palette != value)
        //        { 
        //            this._palette = value;
        //            OnPaletteChanged(new EventArgs());
        //        }
        //    }
        //}
        //public MemoryStream PaletteFileMemoryStream
        //{
        //    get
        //    {
        //        return this._paletteFileMemoryStream;
        //    }
        //    set
        //    {
        //        if(this._paletteFileMemoryStream != value)
        //        {
        //            this._paletteFileMemoryStream = value;
        //        }
        //    }
        //}
        //public string PaletteFileName;

        public Project ()
        {
        }

        public Project(string path, bool loadDefaults)
        {
            this._workingFolder = path;

            if (loadDefaults)
            {
                LoadPalette(_workingFolder);
                LoadGameSet(_workingFolder);
            }
        }


        /// <summary>
        /// This function will open a file containing multiple dBase III databases, like 7KAA's std.set. 
        /// </summary>
        /// <param name="filepath"></param>
        public void LoadGameSet(string filepath = null)
        {
            if (filepath == null)
                filepath = this._workingFolder;

            //// If a set is chosen by the user, we'll get a full
            //// file path. The 'connex' string below can't have
            //// a file name, just a path. This is because the path 
            //// is considered the 'database' and the file is a 'table'
            //// as far as OLEDB/Jet is concerned.
            //FileAttributes attr = File.GetAttributes(filepath);
            //if (attr.HasFlag(FileAttributes.Directory))
            //    this.SetStruct.GameSetFileName = "std.set";
            //else
            //{
            //    this.SetStruct.GameSetFileName = Path.GetFileName(filepath);
            //    filepath = Path.GetDirectoryName(filepath);
            //}

            //string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //    filepath + ";Extended Properties=dBase III";

            ////open & read the file
            //using (FileStream fs = new FileStream(filepath + '\\' + this.SetStruct.GameSetFileName, FileMode.Open))
            //{ 
            //    byte[] setData = new byte[fs.Length];
            //    fs.Read(setData, 0, setData.Length);

            //    this.ActiveGameSet = new GameSet(setData, filepath);
            //}
            this.ActiveGameSet = new GameSet(filepath);
            //todo: fix this cheap hack
            this.SetStruct.GameSetFileMemoryStream = this.ActiveGameSet.GetRawDataStream() as MemoryStream;
            this.SetStruct.GameSetFileName = filepath;
            this._spriteTables = this.ActiveGameSet.GetSpritesDataSet();
        }
        public ColorPalette LoadPalette(string filepath = null)
        {
            
            if (filepath == null)
                filepath = this._workingFolder;

            FileAttributes attr = File.GetAttributes(filepath);
            //string filename;
            if (attr.HasFlag(FileAttributes.Directory))
                this.PalStruct.PaletteFileName = "pal_std.res";
            else
            {
                this.PalStruct.PaletteFileName = Path.GetFileName(filepath);
                filepath = Path.GetDirectoryName(filepath);
            }


            this.ActivePalette = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;          

            using (FileStream fs = File.OpenRead(filepath + '\\' + this.PalStruct.PaletteFileName))
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
                this.PalStruct.PaletteFileMemoryStream = new MemoryStream();//FileStream(filepath + '\\' + this.PaletteFileName, FileMode.Open, FileAccess.Read);
                fs.Position = 0;
                fs.CopyTo(this.PalStruct.PaletteFileMemoryStream);
            }

            return this.ActivePalette;
        }
        /// <summary>
        /// Serializes the project with a BinaryFormatter
        /// </summary>
        /// <returns>A MemoryStream containing the serialized project data</returns>
        public Stream SaveProject()
        {
            Serialization.ZipProjectDir(this, this._workingFolder + '\\' + "project.zip");

            return Serialization.Serialize(this);
        }
        /// <summary>
        /// Deserializes the stream into a <see cref="Project"/> object
        /// </summary>
        /// <param name="str">The stream of the <see cref="Project"/> object</param>
        /// <returns>A project object with the deserialized data</returns>
        public static Project LoadProject(Stream str)
        {
            return (Project) Serialization.Deserialize(str);
        }
    }

    public static class Serialization
    {
        internal static Stream Serialize(object o)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter fm = new BinaryFormatter();
                fm.Serialize(ms, o);
                return ms;
            }

            //todo: implement XmlSerializer with Base64-encoded binary blobs, or refs to files in archive
            //XmlSerializer xs = new XmlSerializer(typeof(Project));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (XmlWriter xw = XmlWriter.Create(ms))
            //    {
            //        xs.Serialize(xw, this);
            //        return ms;
            //    }
            //}
        }

        internal static object Deserialize(Stream str)
        {
            return new BinaryFormatter().Deserialize(str);// as MemoryStream;
        }

        internal static void ZipProjectDir(Project p, string filePath)
        {
            /*
                projectDir\
                    palettes\
                        pal_std.res
                    gamesets\
                        std.set
                    sprites\
                        ballista.spr
             */

            using (FileStream zipStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) 
            {
                using (ZipArchive arch = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry paletteEntry = arch.CreateEntry("palettes\\" + p.PalStruct.PaletteFileName);
                    p.PalStruct.PaletteFileMemoryStream.WriteTo(paletteEntry.Open());
                    ZipArchiveEntry setEntry = arch.CreateEntry("gamesets\\" + p.SetStruct.GameSetFileName);
                    p.SetStruct.GameSetFileMemoryStream.WriteTo(setEntry.Open());
                    //using (StreamWriter sw = new StreamWriter(paletteEntry.Open()))
                    //{
                    //    //sw.Write(Convert.ToBase64CharArray(p.PaletteFileMemoryStream.ToArray(), 0, (int) p.PaletteFileMemoryStream.Length, );
                    //    sw.WriteLine("Information about this package.");
                    //    sw.WriteLine("========================");
                    //}

                }
            }

        }
        //internal static MemoryStream ZipProject(Project p)
        //{
        //    byte[] zipStream;

        //    string filename = p.PaletteFileName;

        //    //using (MemoryStream os = new MemoryStream())
        //    //{
        //    MemoryStream os = new MemoryStream();
        //    GZipStream gz = new GZipStream(os, CompressionLevel.Fastest);
        //    p.PaletteFileMemoryStream.CopyTo(gz);

        //    using (var arch = new ZipArchive(os, ZipArchiveMode.Create, true))
        //    {
        //        var fileInArchive = arch.CreateEntry(filename, CompressionLevel.Optimal);

        //        using (var entryStream = fileInArchive.Open())
        //        {
                        
        //            using (var fileToCompressStream = new MemoryStream())
        //            {
        //                p.PaletteFileMemoryStream.CopyTo(entryStream);
        //                //fileToCompressStream.CopyTo(entryStream);
        //            }
        //        }
                    
        //        //}
        //        //zipStream = os.ToArray();
        //        return os;
        //    }
            
        //}
    }
}
