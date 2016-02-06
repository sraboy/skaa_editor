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

namespace SkaaGameDataLib.Util
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
        /// A file which has a format matching that off <see cref="FileFormats.SpriteFrameSpr"/> but which
        /// references a different table in std.set.
        /// </summary>
        /// <remarks>
        /// For example, i_town.res references the TOWNLAY table.
        /// </remarks>
        ResSpriteSpr,
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
        ResText,
        ResIdxAudio,
        /// <summary>
        /// A dBaseIII table
        /// </summary>
        /// <seealso cref="DbfFile"/>
        DbaseIII,
        SaveGame,
        Font,
        /// <summary>
        /// A 7KAA-formatted palette file
        /// </summary>
        Palette,
        WindowsBitmap,
        WindowsText,
        WindowsWaveAudio
    }
}
