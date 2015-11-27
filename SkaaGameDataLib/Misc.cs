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

    public enum FileFormat
    {
        Unknown,
        GameSet,
        Sprite,
        ResMultiBmp,
        ResBmp,
        DbaseIII
    }

    public static class Misc
    {
        public static object[] CheckFileType(string path)
        {
            string file_ext = Path.GetExtension(path);
            
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                string ext = Path.GetExtension(path);

                switch (file_ext)
                {
                    case ".res":
                        return CheckResFileType(fs);
                        break;
                    case ".spr":
                        return new object[] { Encoding.GetEncoding(1252).GetBytes("header not read"), FileFormat.Sprite };
                        break;
                    case ".set":
                        return new object[] { Encoding.GetEncoding(1252).GetBytes("header not read"), FileFormat.GameSet };
                        break;
                    default:
                        return CheckResFileType(fs);
                        //return FileFormat.Unknown;
                        break;
                }
            }
        }

        private static object[] CheckResFileType(Stream str)
        {
            FileFormat? format = null;
            long oldPos = str.Position;
            str.Position = 0;

            byte[] header = new byte[32];
            str.Read(header, 0, header.Length);
            uint size = BitConverter.ToUInt32(header, 0);
            ushort width = BitConverter.ToUInt16(header, 4);
            ushort height = BitConverter.ToUInt16(header, 6);

            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;
            IndexedBitmap ibmp = new IndexedBitmap(pal);
            try
            {
                ibmp.GetBitmapFromRleStream(str);
                if (ibmp.Bitmap.Width == width && ibmp.Bitmap.Height == height)
                    format = FileFormat.Sprite;
            }
            catch (Exception e)
            {
                if (e is FormatException)
                {
                    if (header[0] == 0x3 &&
                            header[1] <= (DateTime.Today.Year - 2000) && //a sensible year
                            (header[2] < 13 && header[2] > 0) &&         //a real month
                            (header[3] < 32 && header[3] > 0))           //a real day
                    {
                        DbfFile.DbfFileHeader dbfHeader = new DbfFile.DbfFileHeader();
                        str.Position = 0;
                        dbfHeader = DbfFile.ReadHeader(str); //todo: test broken DBF files and catch the exception
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
                            str.Position = recOffset;
                            byte[] recHeightWidth = new byte[4];
                            str.Read(recHeightWidth, 0, 4);
                            ushort recWidth = BitConverter.ToUInt16(recHeightWidth, (int) recOffset);
                            ushort recHeight = BitConverter.ToUInt16(recHeightWidth, (int) recOffset + 2);
                            format = FileFormat.ResMultiBmp;
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

            str.Position = oldPos;
            object[] data = new object[] { header, format };
            return data;
            //return format;
        }
    }
}
