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
                case ".col":
                    return FileFormat.Palette;
                case ".set":
                    return FileFormat.GameSet;
                    break;
                case ".bin":
                    return FileFormat.Unknown;
                case ".bmp":
                    return FileFormat.Unknown;
                default:
                    return CheckResFileType(path);//FileFormat.Unknown;
                    break;
            }
        }

        private static FileFormat CheckResFileType(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string prefix = filename.Substring(0, 4);
            FileFormat format = FileFormat.Unknown;

            switch(prefix)
            {
                case "pal_":
                    format = FileFormat.Palette;
                    break;
                case "fnt_":
                    format = FileFormat.Font;
                    break;
                default:
                    if (prefix.Substring(0, 2) == "a_")
                        format = FileFormat.ResIdxAudio;
                    break;
            }

            if (format == FileFormat.Unknown)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    format = CheckResIdxFormats(fs); //check ResIdx (code_len = 9)

                    if (format == FileFormat.Unknown)
                        format = CheckResFormats(fs); //check Res (code_len = 8)

                    if (format == FileFormat.Unknown)
                        format = CheckSprFormats(fs); //check SpriteSpr/SpriteFrameSpr

                    if (format == FileFormat.Unknown) //check dBaseIII DBF
                        format = CheckDbfFormat(fs);
                }
            }

            return format;
        }
        private static FileFormat CheckResIdxFormats(Stream str)
        {
            FileFormat format;
            long oldPos = str.Position;

            var dic = ResourceDatabase.ReadDefinitions(str, true);
            if (dic == null)
                format = FileFormat.Unknown;
            else //check ResIdx subformats
            {
                format = CheckResIdxMultiBmp(str, dic);//FileFormat.ResIdxUnknown;
            }

            str.Position = oldPos;
            return format;
        }
        private static FileFormat CheckResIdxMultiBmp(Stream str, Dictionary<string,uint> dic)
        {
            FileFormat format;
            long oldPos = str.Position;

            if(dic != null)
            {
                foreach (KeyValuePair<string, uint> kv in dic)
                {
                    uint recOffset = kv.Value;
                    str.Seek(recOffset, SeekOrigin.Begin);
                    format = CheckSprFormats(str);
                    if (format == FileFormat.Unknown) //default return value from CheckSprFormats()
                        return FileFormat.ResIdxUnknown;
                }
            }

            str.Position = oldPos;
            return FileFormat.ResIdxMultiBmp;
        }
        private static FileFormat CheckResFormats(Stream str)
        {
            FileFormat format;
            long oldPos = str.Position;

            var dic = ResourceDatabase.ReadDefinitions(str, false);
            if (dic == null)
                format = FileFormat.Unknown;
            else
                format = FileFormat.ResUnknown;

            str.Position = oldPos;
            return format;
        }
        private static FileFormat CheckSprFormats(Stream str)
        {
            FileFormat format;
            long oldPos = str.Position;

            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;
            IndexedBitmap ibmp = new IndexedBitmap(pal);


            if (ibmp.SetBitmapFromRleStream(str, FileFormat.SpriteSpr) != null)
                format = FileFormat.SpriteSpr;
            else
            {
                str.Position = oldPos;
                ibmp = new IndexedBitmap(pal);
                if (ibmp.SetBitmapFromRleStream(str, FileFormat.SpriteFrameSpr) != null)
                    format = FileFormat.SpriteFrameSpr;
                else
                    format = FileFormat.Unknown;
            }

            str.Position = oldPos;
            return format;
        }
        private static FileFormat CheckDbfFormat(Stream str)
        {
            FileFormat format;
            long oldPos = str.Position;

            byte[] header = new byte[32];
            str.Read(header, 0, header.Length);
            uint size = BitConverter.ToUInt32(header, 0);
            ushort width = BitConverter.ToUInt16(header, 4);
            ushort height = BitConverter.ToUInt16(header, 6);
            if (header[0] == 0x3 &&
                    (header[1] >= 90 ||                          //greater than 1990
                     header[1] <= (DateTime.Today.Year - 2000))  //this century for any new files
                     &&
                    (header[2] < 13 && header[2] > 0) &&         //a real month
                    (header[3] < 32 && header[3] > 0))           //a real day
            {
                //todo: make DbfFile.ReadHeader() check itself and returns null if bad
                //DbfFile.DbfFileHeader dbfHeader = new DbfFile.DbfFileHeader();
                //str.Position = oldPos;
                //dbfHeader = DbfFile.ReadHeader(str); //todo: test broken DBF files and catch the exception
                format = FileFormat.DbaseIII;
            }
            else
                format = FileFormat.Unknown;

            str.Position = oldPos;
            return format;
        }
   
        //                    try
        //                    {
        //                        ushort recCount = BitConverter.ToUInt16(header, 0);
        //                        byte[] name = new byte[9];
        //                        Array.Copy(header, 2, name, 0, 9);
        //                        string recName = Encoding.GetEncoding(1252).GetString(name);
        //                        uint recOffset = BitConverter.ToUInt32(header, 11);
        //                        fs.Position = recOffset;
        //                        byte[] recHeightWidth = new byte[4];
        //                        fs.Read(recHeightWidth, 0, 4);
        //                        ushort recWidth = BitConverter.ToUInt16(recHeightWidth, (int) recOffset);
        //                        ushort recHeight = BitConverter.ToUInt16(recHeightWidth, (int) recOffset + 2);
        //                        format = FileFormat.ResXMultiBmp;
        //                    }
   
    }
}
