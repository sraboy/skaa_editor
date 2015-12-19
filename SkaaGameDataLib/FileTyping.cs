#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/
#endregion
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

    public enum FileFormats
    {
        Unknown,
        SpriteSpr,
        SpriteFrameSpr,
        GameSet, //essentially a ResIdxDbf
        ResIdxMultiBmp,
        ResIdxOther,
        ResIdxText,
        ResIdxUnknown,
        ResIdxAudio,
        ResUnknown,
        DbaseIII,
        SaveGame,
        Font,
        Palette,
        FramePNG,
        SpritePNG,
        Any
    }

    public static class FileTypeChecks
    {
        public static readonly TraceSource Logger = new TraceSource("SkaaGameDataLib.FileTypeChecks", SourceLevels.All);

        public static FileFormats CheckFileType(string path)
        {
            string file_ext = Path.GetExtension(path);
            //string filename = Path.GetFileNameWithoutExtension(path);

            switch (file_ext)
            {
                case ".res":
                    return CheckResFileType(path);
                case ".icn":
                    return FileFormats.SpriteFrameSpr;
                case ".spr":
                    return FileFormats.SpriteSpr;
                case ".col":
                    return FileFormats.Palette;
                case ".set":
                    return FileFormats.GameSet;
                case ".bin":
                    return FileFormats.Unknown;
                case ".bmp":
                    return FileFormats.Unknown;
                default:
                    return CheckResFileType(path);
            }
        }
        
        /*********************Individual File Type Checkers*********************/
        private static FileFormats CheckResFileType(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string prefix = filename.Substring(0, 4);
            FileFormats format = FileFormats.Unknown;

            switch(prefix)
            {
                case "pal_":
                    format = FileFormats.Palette;
                    break;
                case "fnt_":
                    format = FileFormats.Font;
                    break;
                default:
                    if (prefix.Substring(0, 2) == "a_")
                        format = FileFormats.ResIdxAudio;
                    break;
            }

            if (format == FileFormats.Unknown)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    format = CheckResIdxFormats(fs); //check ResIdx (code_len = 9)

                    if (format == FileFormats.Unknown)
                        format = CheckResFormats(fs); //check Res (code_len = 8)

                    if (format == FileFormats.Unknown)
                        format = CheckSprFormats(fs); //check SpriteSpr/SpriteFrameSpr

                    if (format == FileFormats.Unknown) //check dBaseIII DBF
                        format = CheckDbfFormat(fs);
                }
            }

            return format;
        }
        private static FileFormats CheckResIdxFormats(Stream str)
        {
            FileFormats format;
            long oldPos = str.Position;

            var dic = ResourceDatabase.ReadDefinitions(str, true);
            if (dic == null)
                format = FileFormats.Unknown;
            else 
            {
                format = CheckResIdxMultiBmp(str, dic); //returns ResIdxUnknown if it can't verify ResIdxMultiBmp
            }

            str.Position = oldPos;
            return format;
        }
        private static FileFormats CheckResIdxMultiBmp(Stream str, Dictionary<string,uint> dic)
        {
            FileFormats format;
            long oldPos = str.Position;

            if(dic != null)
            {
                foreach (KeyValuePair<string, uint> kv in dic)
                {
                    uint recOffset = kv.Value;
                    str.Seek(recOffset, SeekOrigin.Begin);
                    format = CheckSprFormats(str);
                    if (format == FileFormats.Unknown) //default return value from CheckSprFormats()
                        return FileFormats.ResIdxUnknown;
                }
            }

            str.Position = oldPos;
            return FileFormats.ResIdxMultiBmp;
        }
        private static FileFormats CheckResFormats(Stream str)
        {
            FileFormats format;
            long oldPos = str.Position;

            var dic = ResourceDatabase.ReadDefinitions(str, false);
            if (dic == null)
                format = FileFormats.Unknown;
            else
                format = FileFormats.ResUnknown;

            str.Position = oldPos;
            return format;
        }
        private static FileFormats CheckSprFormats(Stream str)
        {
            FileFormats format;
            long oldPos = str.Position;

            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;
            IndexedBitmap ibmp = new IndexedBitmap(pal);


            if (ibmp.SetBitmapFromRleStream(str, FileFormats.SpriteSpr) != null)
                format = FileFormats.SpriteSpr;
            else
            {
                str.Position = oldPos;
                ibmp = new IndexedBitmap(pal);
                if (ibmp.SetBitmapFromRleStream(str, FileFormats.SpriteFrameSpr) != null)
                    format = FileFormats.SpriteFrameSpr;
                else
                    format = FileFormats.Unknown;
            }

            str.Position = oldPos;
            return format;
        }
        private static FileFormats CheckDbfFormat(Stream str)
        {
            FileFormats format;
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
                format = FileFormats.DbaseIII;
            }
            else
                format = FileFormats.Unknown;

            str.Position = oldPos;
            return format;
        }
   
    }
}
