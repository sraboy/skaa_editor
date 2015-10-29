﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditor
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
        private Sprite _activeSprite;
        private string _spriteFileName;
        private MemoryStream _spriteFileMemoryStream;

        public string SpriteFileName
        {
            get
            {
                return this._spriteFileName;
            }
            set
            {
                if (this._spriteFileName != value)
                {
                    this._spriteFileName = value;
                }
            }
        }
        public MemoryStream SpriteFileMemoryStream
        {
            get
            {
                return this._spriteFileMemoryStream;
            }
            set
            {
                if (this._spriteFileMemoryStream != value)
                {
                    this._spriteFileMemoryStream = value;
                }
            }
        }
        public Sprite ActiveSprite
        {
            get
            {
                return this._activeSprite;
            }
            set
            {
                if(this._activeSprite != value)
                {
                    this._activeSprite = value;
                }
            }
        }
    }

    public static class Extensions
    {
        public static uint? GetNullableUInt32FromIndex(this DataRow dr, int itemArrayIndex)
        {


            string x = dr[itemArrayIndex].ToString();

            /* Codepage 437 is needed due to the use of Extended ASCII.
             *
             * For example, in SFRAME.dbf, the BITMAPPTR is labeled as a
             * CHAR type (which is just a string in dBase III parlance).
             * Because of this, it's not possible to read the bytes in as
             * numbers, even though they're just used as pointers in the 
             * original code.
             */
            byte[] bytes = Encoding.GetEncoding(437).GetBytes(x);

            uint? offset = null;

            switch (bytes.Length)
            {
                case 0:
                    offset = 0;
                    break;
                case 1:
                    offset = bytes[0];
                    break;
                case 2:
                    offset = BitConverter.ToUInt16(bytes, 0);
                    break;
                case 3:
                    byte[] copy = new byte[4];
                    bytes.CopyTo(copy, 0);
                    offset = BitConverter.ToUInt32(copy, 0);
                    break;
                case 4:
                    offset = BitConverter.ToUInt32(bytes, 0);
                    break;
                default:
                    throw new ArgumentNullException("There was an issue reading offsets from the sprite's DataTable that will cause the variable \'offset\' to remain null.");
            }

            return offset;
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
    public static class ProjectZipper
    {
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
