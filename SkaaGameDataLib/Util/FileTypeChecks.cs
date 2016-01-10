#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SkaaGameDataLib.Util
{
    public static class FileTypeChecks
    {
        public static readonly TraceSource Logger = new TraceSource("SkaaGameDataLib.FileTypeChecks", SourceLevels.All);

        /// <summary>
        /// Verifies the file's type by extension. *.res files are passed to <see cref="CheckResFileType(string)"/> for 
        /// separate tests.
        /// </summary>
        /// <param name="filePath">The full path of the file to check</param>
        /// <returns>
        /// The recognized <see cref="FileFormats"/> format or 
        /// <see cref="FileFormats._Unknown"/> if the file could not be recognized
        /// </returns>
        public static FileFormats CheckFileType(string filePath)
        {
            string file_ext = Path.GetExtension(filePath);
            //string filename = Path.GetFileNameWithoutExtension(path);

            switch (file_ext)
            {
                case ".res":
                    return CheckResFileType(filePath);
                case ".icn":
                    return FileFormats.SpriteFrameSpr;
                case ".spr":
                    return FileFormats.SpriteSpr;
                case ".col":
                    return FileFormats.Palette;
                case ".set":
                    return FileFormats.ResIdxDbf;
                case ".bin":
                    return FileFormats._Unknown;
                case ".bmp":
                    return FileFormats.WindowsBitmap;
                case ".txt":
                    return FileFormats.WindowsText;
                case ".wav":
                    return FileFormats.WindowsWaveAudio;
                default:
                    return CheckResFileType(filePath);
            }
        }

        /*********************Individual File Type Checkers*********************/
        /// <summary>
        /// First checks for a filename prefix identifying the file as a palette, font or audio. If it
        /// doesn't match, the file is passed to more in-depth file type checks that attempt to read the
        /// file and identify its type by assuming it is a particular type and attempting to use it.
        /// The failure to identify a type is identified through the generation of exceptions in the 
        /// various methods.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// <see cref="FileFormats._Unknown"/> if the file can't be identified. Otherwise,
        /// the identified file type.
        /// </returns>
        private static FileFormats CheckResFileType(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string prefix = filename.Substring(0, 4);
            FileFormats format = FileFormats._Unknown;

            switch (prefix)
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

            if (format == FileFormats._Unknown)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    format = CheckResIdxFormats(fs);

                    if (format == FileFormats._Unknown)
                        if (CheckSprFormats(fs) == FileFormats.SpriteSpr) //check SpriteSpr/SpriteFrameSpr
                            format = FileFormats.ResSpriteSpr;

                    if (format == FileFormats._Unknown) //check dBaseIII DBF
                        format = CheckDbfFormat(fs);
                }
            }

            return format;
        }
        private static FileFormats CheckResIdxFormats(Stream str)
        {
            FileFormats format;
            long oldPos = str.Position;

            var dic = ResourceDefinitionReader.ReadDefinitions(str);
            if (dic == null)
                format = FileFormats._Unknown;
            else
            {
                format = CheckResIdxMultiBmp(str, dic); //returns ResIdxUnknown if it can't verify ResIdxMultiBmp
            }

            str.Position = oldPos;
            return format;
        }
        private static FileFormats CheckResIdxMultiBmp(Stream str, Dictionary<string, uint> dic)
        {
            FileFormats format;
            long oldPos = str.Position;

            if (dic != null)
            {
                foreach (KeyValuePair<string, uint> kv in dic)
                {
                    uint recOffset = kv.Value;
                    str.Seek(recOffset, SeekOrigin.Begin);
                    format = CheckSprFormats(str);
                    if (format == FileFormats._Unknown) //default return value from CheckSprFormats()
                        return FileFormats._ResIdxFile;
                }
            }

            str.Position = oldPos;
            return FileFormats.ResIdxMultiBmp;
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
                    format = FileFormats._Unknown;
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
                format = FileFormats._Unknown;

            str.Position = oldPos;
            return format;
        }
    }
}
