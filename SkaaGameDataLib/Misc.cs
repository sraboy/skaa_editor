using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    /*  File Formats
        
        spr files:
        [frames] uint32 size; short width; short height; [/frames]
        ----------------------------------------------------------
        res files (bmp):
        uint32 size; short width; short height; rle_bmp_data;
        ----------------------------------------------------------
        res files (multi-bmp):
        short record_count;
        [records] char[9] record_name; uint32 bmp_offset; [/records]
        [bmps] short width; short height; rle_bmp_data; [/bmps]
        ----------------------------------------------------------
        res files (dbf):
        short dBaseVersion=0x3; 
        byte[3] dateLastEdited = { 0xYY 0xMM 0xDD }
        ...dbf data...
        ----------------------------------------------------------
        res files (tut_text):
        short record_count;
        [records] char[9] record_name; uint32 text_offset; [/records]
        [texts] short width; short height; rle_bmp_data; [/texts]
    */



    public static class Misc
    {
        public static readonly TraceSource Logger = new TraceSource("SkaaGameDataLib_Misc", SourceLevels.All);

        public static FileFormat CheckFileType(string path)
        {
            string file_ext = Path.GetExtension(path);
            //string filename = Path.GetFileNameWithoutExtension(path);

            switch (file_ext)
            {
                case ".res":
                    //object[] res_data = GetResTypeAndHeader(path);
                    //return (FileFormat)res_data[1];
                    return CheckResFileType(path);
                    break;
                //case ".icn":
                //    break;
                case ".spr":
                    return FileFormat.SpriteSpr;
                    break;
                case ".set":
                    return FileFormat.GameSet;
                    break;
                default:
                    return FileFormat.Unknown;
                    break;
            }
        }

        private static FileFormat CheckResFileType(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string prefix = filename.Substring(0, 4);

            switch(prefix)
            {
                case "pal_":
                    return FileFormat.Palette;
                case "fnt_":
                    return FileFormat.Font;
                default:
                    if (prefix.Substring(0, 2) == "a_")
                        return FileFormat.ResXAudio;
                    else
                        break;
            }
            
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                //check RESX
                var dic = ResourceDatabase.ReadDefinitions(fs);
                if (dic != null)
                    return FileFormat.ResXUnknown; //todo: check ResXMultiBmp, ResXText

                //check SPR
                fs.Position = 0;
                return CheckSpriteFileFormat(fs);
            }

            return FileFormat.Unknown;
        }
        private static FileFormat CheckSpriteFileFormat(Stream str)
        {
            long oldPos = str.Position;
            byte[] header = new byte[32];
            str.Read(header, 0, header.Length);
            uint size = BitConverter.ToUInt32(header, 0);
            ushort width = BitConverter.ToUInt16(header, 4);
            ushort height = BitConverter.ToUInt16(header, 6);

            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;
            IndexedBitmap ibmp = new IndexedBitmap(pal);

            try
            {
                str.Position = oldPos + 4;//+4 to skip uint size
                ibmp.SetBitmapFromRleStream(str);
                if (ibmp.Bitmap.Width == width && ibmp.Bitmap.Height == height)
                    return FileFormat.SpriteSpr;
            }
            catch (FormatException fe)
            {
                return FileFormat.Unknown;
            }

            return FileFormat.Unknown;
        }

        public static object[] GetFileFormatListing(string path)
        {
            string file_ext = Path.GetExtension(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            switch (file_ext)
            {
                case ".res":
                    return GetResTypeAndHeader(path);
                    break;
                case ".spr":
                    return new object[] { Encoding.GetEncoding(1252).GetBytes("header not read"), FileFormat.SpriteSpr };
                    break;
                case ".set":
                    return new object[] { Encoding.GetEncoding(1252).GetBytes("header not read"), FileFormat.GameSet };
                    break;
                default:
                    return GetResTypeAndHeader(path);
                    //return FileFormat.Unknown;
                    break;
            }

        }
        private static object[] GetResTypeAndHeader(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                FileFormat? format = null;
                long oldPos = fs.Position;
                fs.Position = 0;

                byte[] header = new byte[32];
                fs.Read(header, 0, header.Length);
                uint size = BitConverter.ToUInt32(header, 0);
                ushort width = BitConverter.ToUInt16(header, 4);
                ushort height = BitConverter.ToUInt16(header, 6);

                ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;
                IndexedBitmap ibmp = new IndexedBitmap(pal);
                try
                {
                    ibmp.SetBitmapFromRleStream(fs);
                    if (ibmp.Bitmap.Width == width && ibmp.Bitmap.Height == height)
                        format = FileFormat.SpriteSpr;
                }
                catch (Exception e)
                {
                    if (e is FormatException)
                    {
                        fs.Position = oldPos;

                        if (header[0] == 0x3 &&
                                (
                                header[1] >= 90 ||                           //greater than 1990
                                header[1] <= (DateTime.Today.Year - 2000)    //this century for any new files
                                ) &&
                                (header[2] < 13 && header[2] > 0) &&         //a real month
                                (header[3] < 32 && header[3] > 0))           //a real day
                        {
                            DbfFile.DbfFileHeader dbfHeader = new DbfFile.DbfFileHeader();
                            fs.Position = 0;
                            dbfHeader = DbfFile.ReadHeader(fs); //todo: test broken DBF files and catch the exception
                            format = FileFormat.DbaseIII;
                        }
                        else
                        {
                            try
                            {
                                ushort recCount = BitConverter.ToUInt16(header, 0);
                                byte[] name = new byte[9];
                                Array.Copy(header, 2, name, 0, 9);
                                string recName = Encoding.GetEncoding(1252).GetString(name);
                                uint recOffset = BitConverter.ToUInt32(header, 11);
                                fs.Position = recOffset;
                                byte[] recHeightWidth = new byte[4];
                                fs.Read(recHeightWidth, 0, 4);
                                ushort recWidth = BitConverter.ToUInt16(recHeightWidth, (int) recOffset);
                                ushort recHeight = BitConverter.ToUInt16(recHeightWidth, (int) recOffset + 2);
                                format = FileFormat.ResXMultiBmp;
                            }
                            catch (Exception ex)
                            {
                                if (ex is ArgumentOutOfRangeException)
                                    format = FileFormat.Unknown;
                                else
                                    Debugger.Break();
                            }
                        }
                    }
                    else
                    {
                        format = FileFormat.Unknown;
                        //Debugger.Break();
                    }
                }

                fs.Position = oldPos;
                object[] data = new object[] { header, format };
                return data;
                //return format;
            }
        }
    }
}
