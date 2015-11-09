using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditorUI
{
    /// <summary>
    /// Used to encapsulate a <see cref="ColorPalette"/>, its name and the <see cref="MemoryStream"/> 
    /// containing the raw RES file data.
    /// </summary>
    public class PaletteResource
    {
        public string FileName;
        public MemoryStream ResMemoryStream;
        public ColorPalette ColorPaletteObject;
    }
    /// <summary>
    /// Used to encapsulate a <see cref="Sprite"/>, its name and the <see cref="MemoryStream"/> 
    /// containing the raw SPR file data.
    /// </summary>
    public class SpriteResource
    {
        public string FileName;
        public MemoryStream SprMemoryStream;
        public Sprite SpriteObject;
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
                    //ZipArchiveEntry paletteEntry = arch.CreateEntry(Path.GetFileName(p.SuperPal.PaletteFileName));
                    //p.SuperPal.PaletteFileMemoryStream.WriteTo(paletteEntry.Open());
                    
                    //ZipArchiveEntry setEntry = arch.CreateEntry(Path.GetFileName(p.ActiveGameSet.FileName));
                    //p.ActiveGameSet.RawDataStream.WriteTo(setEntry.Open());
                    //ZipArchiveEntry spriteEntry = arch.CreateEntry(Path.GetFileName(p.SuperSpr.SpriteFileName));
                    //p.SuperSpr.SpriteFileMemoryStream.WriteTo(spriteEntry.Open());

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

            Project p = new Project(false);
            //p.LoadPalette(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "res"));
            //p.LoadGameSet(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "set"));
            //p.LoadSprite(path + '\\' + arch.Entries.First(ent => ent.Name.Split(new char[] { '.' })[1] == "spr"));
            return p;
        }
    }
    public static class Misc
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }
        public static void Error(string message)
        {
            Trace.WriteLine(message);
#if DEBUG
            throw new Exception(message);
#endif
        }
    }
}
