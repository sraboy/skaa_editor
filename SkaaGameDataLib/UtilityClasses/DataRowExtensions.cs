﻿#region Copyright Notice
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
using System.Data;
using System.IO;
using System.Text;

namespace SkaaGameDataLib
{
    public static class DataRowExtensions
    {
        public static readonly string FrameNameColumn = "FrameName";
        public static readonly string FrameOffsetColumn = "FrameOffset";

        public static void WriteResDefinition(this DataRow dr, Stream str, uint offset, bool isIdx)
        {
            int nameSize, definitionSize;

            if (isIdx)
            {
                nameSize = ResourceDefinitionReader.ResIdxNameSize;
                definitionSize = ResourceDefinitionReader.ResIdxDefinitionSize;
            }
            else
            {
                nameSize = ResourceDefinitionReader.ResNameSize;
                definitionSize = ResourceDefinitionReader.ResDefinitionSize;
            }

            string recordName = dr[FrameNameColumn].ToString().PadRight(nameSize, (char) 0x0);
            byte[] record_name = new byte[nameSize];
            record_name = Encoding.GetEncoding(1252).GetBytes(recordName);
            str.Write(record_name, 0, nameSize);

            byte[] record_size = new byte[ResourceDefinitionReader.OffsetSize];
            record_size = BitConverter.GetBytes((uint)dr[FrameOffsetColumn]);
            str.Write(record_size, 0, ResourceDefinitionReader.OffsetSize);
        }
    }
}
