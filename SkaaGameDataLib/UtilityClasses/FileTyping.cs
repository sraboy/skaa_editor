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

namespace SkaaGameDataLib
{
    /// <summary>
    /// Used to describe the various file and data formats in use by 7KAA. Values beginning with an underscore
    /// are special values not used by the game.
    /// </summary>
    /// <remarks>
    /// Values beginning with an underscore are special values not used by the game but used for file 
    /// identification purposes or to provide a generic description for formats that share a base type. 
    /// For example, <see cref="FileFormats._ResIdxFile"/> is used in documentation to describe all data 
    /// using the ResIdx file format. It is only the format of the records within those files that differs,
    /// and this is referred to as its "data format" within documentation at http://www.7kfans.com.
    /// <para>
    /// A quick view of some file formats:
    /// spr files:
    /// [frames]
    /// uint32 size; short width; short height;[/frames]
    /// ----------------------------------------------------------
    /// res files(bmp):
    /// uint32 size; short width; short height; rle_bmp_data;
    /// ----------------------------------------------------------
    /// res files(multi-bmp):
    /// short record_count;
    /// [records]
    /// char[9] record_name; uint32 bmp_offset;[/records]
    /// [bmps]
    /// short width; short height; rle_bmp_data; [/bmps]
    /// ----------------------------------------------------------
    /// res files(dbf):
    /// short dBaseVersion = 0x3;
    /// byte[3] dateLastEdited = { 0xYY 0xMM 0xDD }
    /// ...dbf data...
    /// ----------------------------------------------------------
    /// res files (tut_text):
    /// short record_count;
    /// [records]
    /// char[9] record_name; uint32 text_offset;[/records]
    /// [texts]
    /// short width; short height; rle_bmp_data; [/texts]
    /// </para>
    /// </remarks>
    public enum FileFormats
    {
        /// <summary>
        /// This value is only used in file identification routines or to refer to an arbitrary format in documentation and should not otherwise be used. It differs
        /// from <see cref="_Unknown"/> in that it implies the file type is unknown and no attempt has yet been made to identify it.
        /// </summary>
        [Obsolete("This value is only used in file identification routines or to refer to an arbitrary format in documentation and should not otherwise be used.")]
        _Any,
        /// <summary>
        /// This value is only used in file identification routines or to refer to an unknown format in documentation and should not otherwise be used. It differs
        /// from <see cref="_Any"/> in that it implies the file type could not be identified.
        /// </summary>
        /// <seealso cref="ResourceDefinitionReader.ReadDefinitions(Stream, bool)"/>
        [Obsolete("This value is only used in file identification routines or to refer to an unknown format in documentation and should not otherwise be used.")]
        _Unknown,
        /// <summary>
        /// This value is only used in file identification routines or to refer to the ResIdx format in documentation and should not otherwise be used.
        /// </summary>
        /// <seealso cref="ResourceDefinitionReader.ReadDefinitions(Stream, bool)"/>
        [Obsolete("This value is only used in file identification routines or to refer to the ResIdx format in documentation and should not otherwise be used.")]
        _ResIdxFile,
        /// <summary>
        /// This value is only used in file identification routines or to refer to the Res format in documentation and should not otherwise be used.
        /// </summary>
        /// <seealso cref="ResourceDefinitionReader.ReadDefinitions(Stream, bool)"/>
        [Obsolete("This value is only used in file identification routines or to refer to the Res format in documentation and should not otherwise be used.")]
        _ResFile,
        /// <summary>
        /// A file, generally with an SPR extension, that contains only <see cref="SkaaFrame"/> data and no additional header/trailer or identifier
        /// </summary>
        /// <see cref="IndexedBitmap"/>
        SpriteSpr,
        /// <summary>
        /// Describes image data, generally within a file with an extension of SPR or RES. The data contains a <see cref="UInt32"/> value describing
        /// its size, in bytes, followed by two <see cref="UInt16"/> values describing its width and height, in pixels.
        /// </summary>
        /// <seealso cref="SkaaSpriteFrame"/>
        /// <see cref="SkaaFrame"/>
        /// <see cref="IndexedBitmap"/>
        SpriteFrameSpr,
        /// <summary>
        /// This format is only used for 7KAA's game set files, of which there is only one
        /// distributed with the game (as of release 2.14): std.set.
        /// </summary>
        /// <seealso cref="GameSetFile"/>
        ResIdxDbf,
        /// <summary>
        /// These files are similar to <see cref="SpriteSpr"/> but they have a <see cref="_ResIdxFile"/> header containing
        /// the names and offsets of each of the images in the file. These images, instead of being animation frames for a 
        /// sprite, are simply different images. Nonetheless, the image data is formatted the same as <see cref="SkaaFrame"/>.
        /// </summary>
        /// <seealso cref="ResourceDefinitionReader.ReadDefinitions(Stream, bool)"/>
        /// <see cref="IndexedBitmap"/>
        ResIdxMultiBmp,
        ResIdxText,
        ResIdxAudio,
        /// <summary>
        /// A dBaseIII table
        /// </summary>
        /// <seealso cref="DbfFile"/>
        DbaseIII,
        SaveGame,
        Font,
        Palette,
        FramePNG,
        SpritePNG,
    }

    public static class FileTypeChecks
    {
        public static readonly TraceSource Logger = new TraceSource("SkaaGameDataLib.FileTypeChecks", SourceLevels.All);

        /// <summary>
        /// Verifies the file's type by extension. *.res files are passed to <see cref="CheckResFileType(string)"/> for 
        /// separate tests.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
                    return FileFormats.ResIdxDbf;
                case ".bin":
                    return FileFormats._Unknown;
                case ".bmp":
                    return FileFormats._Unknown;
                default:
                    return CheckResFileType(path);
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
                    format = CheckResIdxFormats(fs); //check ResIdx (code_len = 9)

                    if (format == FileFormats._Unknown)
                        format = CheckResFormats(fs); //check Res (code_len = 8)

                    if (format == FileFormats._Unknown)
                        format = CheckSprFormats(fs); //check SpriteSpr/SpriteFrameSpr

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

            var dic = ResourceDefinitionReader.ReadDefinitions(str, true);
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
        private static FileFormats CheckResFormats(Stream str)
        {
            FileFormats format;
            long oldPos = str.Position;

            var dic = ResourceDefinitionReader.ReadDefinitions(str, false);
            if (dic == null)
                format = FileFormats._Unknown;
            else
                format = FileFormats._ResFile;

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
