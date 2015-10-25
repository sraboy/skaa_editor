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

        private Sprite _activeSprite;
        private SpriteFrame _activeFrame;
        private GameSet _activeGameSet;

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

        public Project()
        {

        }

        public Stream SaveProject()
        {
            return Serialization.Serialize(this);//ZipProject(this);
        }
    }

    public static class Serialization
    {
        public static ZipArchive ZipProject(object o)
        {
            ZipArchive zip;

            using (FileStream fs = new FileStream(@"E:\test.skp", FileMode.Create))
            { 
                using (MemoryStream ms = Serialize(o) as MemoryStream)
                {
                    //fs.Write(ms.ToArray(), 0, (int) ms.Length);
                    zip = new ZipArchive(fs, ZipArchiveMode.Update);
                    //{
                    ZipArchiveEntry entry = zip.CreateEntry("project");
                    
                    //}
                }
            }
            //ZipArchive zip = new ZipArchive(Serialization.Serialize(o), ZipArchiveMode.Update);
            return zip;
        }

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
