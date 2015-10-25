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
        public event EventHandler ActiveFrameChanged;
        protected virtual void OnActiveFrameChanged(EventArgs e)
        {
            EventHandler handler = ActiveFrameChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private string WorkingFolder;

        private Sprite _activeSprite;
        private SpriteFrame _activeFrame;
        private GameSet _activeGameSet;
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
                    this._palette = value;
            }
        }

        public Project(string path)
        {
            this.WorkingFolder = path;
            LoadGameSet(WorkingFolder);
            LoadPalette(WorkingFolder);
            this._spriteTables = BreakSFRAME();   
        }

        public DataSet BreakSFRAME()
        {
            DataTable sframeTable = this.ActiveGameSet.Databases.Tables["SFRAME"];
            List<string> spriteNames = new List<string>();
            DataSet allSpritesSet = new DataSet("sprites");

            foreach (DataRow r in sframeTable.Rows)
            {
                DataTable curTable = allSpritesSet.Tables[r[0].ToString()];

                if (curTable != null)//.Find(t => t.TableName == r[0].ToString()) != null)
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

        public void LoadGameSet(string filepath)
        {
            byte[] setData;
            
            //string path = Path.GetDirectoryName(stdset); //E:\Documents\Visual Studio 2015\Projects\skaa_editor\_other\working\;"
            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                filepath + ";Extended Properties=dBase III";

            FileStream fs = new FileStream(filepath + "\\std.set", FileMode.Open);

            setData = new byte[fs.Length];
            fs.Read(setData, 0, setData.Length);

            this.ActiveGameSet = new GameSet(setData, filepath);
        }
        public ColorPalette LoadPalette(String Path)
        {
            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;// = new ColorPalette();

            FileStream fs = File.OpenRead(Path + "\\pal_std.res");
            fs.Seek(8, SeekOrigin.Begin);

            for (int i = 0; i < 256; i++)
            {
                int r = fs.ReadByte();
                int g = fs.ReadByte();
                int b = fs.ReadByte();

                if (i < 0xf9) //0xf9 is the lowest transparent color byte
                    pal.Entries[i] = Color.FromArgb(255, r, g, b);
                else //0xf9 - 0xff
                    pal.Entries[i] = Color.FromArgb(0, r, g, b);
            }

            this.Palette = pal;

            

            return this.Palette;
        }
        //public void LoadPalette(ColorPalette pal)
        //{
        //    this.ActiveSprite = new Sprite(pal);
        //}
        public Stream SaveProject()
        {
            return Serialization.Serialize(this);//ZipProject(this);
        }
    }

    public static class Serialization
    {
        //public static ZipArchive ZipProject(object o)
        //{
        //    ZipArchive zip;

        //    using (FileStream fs = new FileStream(@"E:\test.skp", FileMode.Create))
        //    { 
        //        using (MemoryStream ms = Serialize(o) as MemoryStream)
        //        {
        //            //fs.Write(ms.ToArray(), 0, (int) ms.Length);
        //            zip = new ZipArchive(fs, ZipArchiveMode.Update);
        //            //{
        //            ZipArchiveEntry entry = zip.CreateEntry("project");
                    
        //            //}
        //        }
        //    }
        //    //ZipArchive zip = new ZipArchive(Serialization.Serialize(o), ZipArchiveMode.Update);
        //    return zip;
        //}

        internal static Stream Serialize(object o)
        {
            MemoryStream ms = new MemoryStream();
            IFormatter fm = new BinaryFormatter();
            fm.Serialize(ms, o);
            return ms;
          

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    new BinaryFormatter().Serialize(ms, this);
            //    Convert.ToBase64String(ms.ToArray());
            //    return ms;
            //}

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
    }
}
