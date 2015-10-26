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
        public struct SuperPalette
        {
            public string PaletteFileName;
            public MemoryStream PaletteFileMemoryStream;
            public ColorPalette ActivePalette;

        }
        public struct SuperGameSet
        {
            public string GameSetFileName;
            public MemoryStream GameSetFileMemoryStream;
            public GameSet ActiveGameSet;
        }
        public struct SuperSprite
        {
            public string SpriteFileName;
            public MemoryStream SpriteFileMemoryStream;
            public Sprite ActiveSprite;
        }

        [NonSerialized]
        public SuperPalette PalStruct;
        [NonSerialized]
        public SuperGameSet SetStruct;
        [NonSerialized]
        public SuperSprite SprStruct;

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
        //private MemoryStream _paletteFileMemoryStream;
        //private Sprite _activeSprite;
        private SpriteFrame _activeFrame;
        //private GameSet _activeGameSet;
        [NonSerialized]
        //private ColorPalette _palette;
        private DataSet _spriteTables = new DataSet("sprites");

        public Sprite ActiveSprite
        {
            get
            {
                return this.SprStruct.ActiveSprite;
            }
            set
            {
                if (this.SprStruct.ActiveSprite != value)
                {
                    this.SprStruct.ActiveSprite = value;
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

            string filename;
            // If a set is chosen by the user, we'll get a full file path. The 'connex' string in the can't have
            // a file name, just a path. This is because the path is considered the 'database' and the file is
            // a 'table' as far as OLEDB/Jet is concerned.
            FileAttributes attr = File.GetAttributes(filepath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                filename = "std.set";
                filepath = filepath + '\\' + filename;
            }
            else
            {
                filename = Path.GetFileName(filepath);
                //filepath = Path.GetDirectoryName(filepath);
            }

            this.ActiveGameSet = new GameSet(filepath);
            //todo: fix this cheap hack
            this.SetStruct.GameSetFileMemoryStream = this.ActiveGameSet.GetRawDataStream() as MemoryStream;
            this.SetStruct.GameSetFileName = filename;
            this._spriteTables = this.ActiveGameSet.GetSpriteTablesInDataSet();
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
        public Sprite LoadSprite(string filepath)
        {
            if (this.ActivePalette == null)
                return null;

            Sprite spr = new Sprite(this.ActivePalette);

            using (FileStream spritestream = File.OpenRead(filepath))
            {
                long x = spritestream.Length;

                while (spritestream.Position < spritestream.Length)
                {
                    byte[] frame_size_bytes = new byte[8];

                    spritestream.Read(frame_size_bytes, 0, 8);

                    int size = BitConverter.ToInt32(frame_size_bytes, 0);
                    short width = BitConverter.ToInt16(frame_size_bytes, 4);
                    short height = BitConverter.ToInt16(frame_size_bytes, 6);

                    SpriteFrame frame = new SpriteFrame(size, width, height, this.ActivePalette);

                    frame.SetPixels(spritestream);
                    frame.BuildBitmap32bpp();
                    spr.Frames.Add(frame);
                }

                this.SprStruct.SpriteFileMemoryStream = new MemoryStream();
                spritestream.Position = 0;
                spritestream.CopyTo(this.SprStruct.SpriteFileMemoryStream);
                this.SprStruct.SpriteFileName = Path.GetFileName(filepath);
            }

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
        public object SaveProject(string filepath)
        {
            try
            { 
                if(filepath == null)
                    Serialization.ZipProject(this, this._workingFolder + '\\' + "new_project.skp");
                else
                    Serialization.ZipProject(this, filepath);
            }
            catch(Exception ex)
            {
                return ex;
            }

            return true;
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
        public static Project LoadProject(string filePath)
        {
            return Serialization.LoadZipProject(filePath);
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
        internal static void ZipProject(Project p, string filePath)
        {            
            using (FileStream zipStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) 
            {
                using (ZipArchive arch = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    ZipArchiveEntry paletteEntry = arch.CreateEntry(Path.GetFileName(p.PalStruct.PaletteFileName));
                    p.PalStruct.PaletteFileMemoryStream.WriteTo(paletteEntry.Open());
                    ZipArchiveEntry setEntry = arch.CreateEntry(Path.GetFileName(p.SetStruct.GameSetFileName));
                    p.SetStruct.GameSetFileMemoryStream.WriteTo(setEntry.Open());
                    ZipArchiveEntry spriteEntry = arch.CreateEntry(Path.GetFileName(p.SprStruct.SpriteFileName));
                    p.SprStruct.SpriteFileMemoryStream.WriteTo(spriteEntry.Open());

                    //using (StreamWriter sw = new StreamWriter(paletteEntry.Open()))
                    //{
                    //    //sw.Write(Convert.ToBase64CharArray(p.PaletteFileMemoryStream.ToArray(), 0, (int) p.PaletteFileMemoryStream.Length, );
                    //    sw.WriteLine("Information about this package.");
                    //    sw.WriteLine("========================");
                    //}
                }
            }
        }
        internal static Project LoadZipProject(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            //todo: Open for updating 
            ZipArchive arch = ZipFile.Open(filePath, ZipArchiveMode.Read);
            //todo: change this to a temp folder with randomly generated name
            arch.ExtractToDirectory(path + '\\' + Path.GetFileNameWithoutExtension(filePath));
            //foreach(ZipArchiveEntry ent in arch.Entries)
            //{
            //    string[] name;
            //    name = ent.Name.Split( new char[]{'.'} );

            //    switch(name[1])
            //    {
            //        case "res":
            //            //p.PalStruct.PaletteFileName = ent.Name;
            //            p.LoadPalette()
            //            break;
            //        case "set":
            //            p.SetStruct.GameSetFileName = ent.Name;
            //            break;
            //        case "spr":
            //            p.SprStruct.SpriteFileName = ent.Name;
            //            break;
            //        default:
            //            throw new FileFormatException("Expecting RES, SET and SPR file extensions.\nReceived " + name[1]);
            //    }
            //}
            
            Project p = new Project(Path.GetDirectoryName(filePath), false);
            p.LoadPalette(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "res"));
            p.LoadGameSet(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "set"));
            p.LoadSprite(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "spr"));
            return p;
        }
    }
}
