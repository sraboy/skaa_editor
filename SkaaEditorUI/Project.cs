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

namespace SkaaEditor
{
    [Serializable]
    public class Project
    {
        
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

        private string Serialize()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Project));
            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw))
                {
                    xs.Serialize(xw, this);
                    return sw.ToString();
                }
            }
        }

        public ZipArchive SaveProject()
        {
            ZipArchive zip = new ZipArchive(new MemoryStream());

            return zip;
        }
    }
}
