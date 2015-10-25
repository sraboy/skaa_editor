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

        private string WorkingFolder;

        private Sprite _activeSprite;
        private SpriteFrame _activeFrame;
        private GameSet _activeGameSet;
        [NonSerialized]
        private ColorPalette _palette;
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
        public GameSet ActiveGameSet
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
        public ColorPalette Palette
        {
            get
            {
                return this._palette;
            }
            set
            {
                if (this._palette != value)
                { 
                    this._palette = value;
                    OnPaletteChanged(new EventArgs());
                }
            }
        }

        //todo: save the palette's name since we can't serialize it

        public Project ()
        {
        }

        public Project(string path, bool loadDefaults)
        {
            this.WorkingFolder = path;

            if (loadDefaults)
            {
                LoadPalette(WorkingFolder);
                LoadGameSet(WorkingFolder);
            }
        }

        public DataSet BreakSFRAME()
        {
            DataTable sframeTable = this.ActiveGameSet.Databases.Tables["SFRAME"];
            List<string> spriteNames = new List<string>();
            DataSet allSpritesSet = new DataSet("sprites");

            foreach (DataRow r in sframeTable.Rows)
            {
                DataTable curTable = allSpritesSet.Tables[r[0].ToString()];

                if (curTable != null)
                {
                    DataTable tbl = curTable;
                    tbl.ImportRow(r);
                }
                else
                {
                    DataTable tbl = sframeTable.Clone();
                    tbl.TableName = r[0].ToString();
                    spriteNames.Add(r[0].ToString());
                    tbl.ImportRow(r);
                    allSpritesSet.Tables.Add(tbl);
                }
            }

            return allSpritesSet;
        }

        public void LoadGameSet(string filepath = null)
        {
            if (filepath == null)
                filepath = this.WorkingFolder;

            // If a set is chosen by the user, we'll get a full
            // file path. The 'connex' string below can't have
            // a file name, just a path. This is because the path 
            // is considered the 'database' and the file is a 'table'
            // as far as OLEDB/Jet is concerned.
            FileAttributes attr = File.GetAttributes(filepath);
            string filename;
            if (attr.HasFlag(FileAttributes.Directory))
                filename = "std.set";
            else
            {
                filename = Path.GetFileName(filepath);
                filepath = Path.GetDirectoryName(filepath);
            }

            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                filepath + ";Extended Properties=dBase III";

            using (FileStream fs = new FileStream(filepath + '\\' + filename, FileMode.Open))
            { 
                byte[] setData = new byte[fs.Length];
                fs.Read(setData, 0, setData.Length);
                this.ActiveGameSet = new GameSet(setData, filepath);
            }

            this._spriteTables = BreakSFRAME();
        }

        public ColorPalette LoadPalette(string filepath = null)
        {
            if (filepath == null)
                filepath = this.WorkingFolder;

            FileAttributes attr = File.GetAttributes(filepath);
            string filename;
            if (attr.HasFlag(FileAttributes.Directory))
                filename = "pal_std.res";
            else
            {
                filename = Path.GetFileName(filepath);
                filepath = Path.GetDirectoryName(filepath);
            }


            this.Palette = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;          

            using (FileStream fs = File.OpenRead(filepath + '\\' + filename))
            { 
                fs.Seek(8, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    int r = fs.ReadByte();
                    int g = fs.ReadByte();
                    int b = fs.ReadByte();

                    if (i < 0xf9) //0xf9 is the lowest transparent color byte
                        this.Palette.Entries[i] = Color.FromArgb(255, r, g, b);
                    else          //0xf9 - 0xff
                        this.Palette.Entries[i] = Color.FromArgb(0, r, g, b);
                }
            }
            return this.Palette;
        }
        /// <summary>
        /// Serializes the project with a BinaryFormatter
        /// </summary>
        /// <returns>A MemoryStream containing the serialized project data</returns>
        public Stream SaveProject()
        {
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
    }
}
