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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SkaaGameDataLib.Util
{
    public static class DataTableExtensions
    {
        public static readonly TraceSource Logger = new TraceSource($"{typeof(DataTableExtensions)}", SourceLevels.All);
        private static readonly string DataSourcePropertyName = "DataSource";

        public static void AddDataSource(this DataTable dt, string dataSource)
        {
            dt.ExtendedProperties[DataSourcePropertyName] = dataSource;
        }
        public static string GetDataSource(this DataTable dt)
        {
            if (dt.ExtendedProperties.Count == 0 || !dt.ExtendedProperties.Contains(DataSourcePropertyName))
                return null;

            return dt.ExtendedProperties[DataSourcePropertyName].ToString();
        }
        /// <summary>
        /// Writes a <see cref="ResourceDefinitionReader"/> header for this <see cref="DataTable"/> to the <see cref="Stream"/>
        /// </summary>
        /// <param name="str">The <see cref="Stream"/> to write the header to</param>
        /// <param name="offset">The offset in the <see cref="Stream"/> at which to begin writing</param>
        public static void WriteResDefinition(this DataTable dt, Stream str, uint offset)
        {
            string recordName = dt.TableName.PadRight(ResourceDefinitionReader.ResIdxNameSize, (char)0x0);
            byte[] record_name = new byte[ResourceDefinitionReader.ResIdxNameSize];
            record_name = Encoding.GetEncoding(1252).GetBytes(recordName);
            str.Write(record_name, 0, ResourceDefinitionReader.ResIdxNameSize);

            byte[] record_size = new byte[ResourceDefinitionReader.ResIdxOffsetSize];
            record_size = BitConverter.GetBytes(Convert.ToUInt32(offset));
            str.Write(record_size, 0, ResourceDefinitionReader.ResIdxOffsetSize);
        }
        /// <summary>
        /// Calls <see cref="DataRowExtensions.WriteResDefinition(DataRow, Stream, uint)"/> for each row in the table
        /// </summary>
        public static void WriteAllRowsAsResDefinitions(this DataTable dt, Stream str)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr.WriteResDefinition(str, (uint)str.Position);
            }
        }
    }
}
