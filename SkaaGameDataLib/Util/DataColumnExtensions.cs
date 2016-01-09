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
using System.Data;
using System.Linq;

namespace SkaaGameDataLib.Util
{
    public static class DataColumnExtensions
    {
        /// <summary>
        /// Describes the length, in bytes, that this field must occupy in the DBF file. Text fields
        /// should be padded on the right with spaces (0x20) while number fields should be padded
        /// on the left with nulls (0x00). Numbers can be "padded" by using a <see cref="Convert"/> 
        /// function that corresponds to the needed number type. This field is separate and 
        /// unrelated to the <see cref="DataColumn.MaxLength"/> property.
        /// </summary>
        public static readonly string ByteLengthPropertyName = "ByteLength";
        /// <summary>
        /// Returns the value of the <see cref="DataColumn.ExtendedProperties"/> element named <see cref="ByteLengthPropertyName"/>
        /// </summary>
        public static byte GetByteLength(this DataColumn dc)
        {
            return (byte)(dc.ExtendedProperties[ByteLengthPropertyName] ?? 0);
        }
        /// <summary>
        /// Sets the value of the <see cref="DataColumn.ExtendedProperties"/> element named <see cref="ByteLengthPropertyName"/>
        /// </summary>
        public static void SetByteLength(this DataColumn dc, byte value)
        {
            if (!dc.ExtendedProperties.Contains(ByteLengthPropertyName))
                dc.ExtendedProperties.Add(ByteLengthPropertyName, value);
            else
                dc.ExtendedProperties[ByteLengthPropertyName] = value;

        }
        internal static DbfFile.FieldDescriptor GetFieldDescriptor(this DataColumn dc)
        {
            DbfFile.FieldDescriptor fd = new DbfFile.FieldDescriptor();

            fd.FieldName = dc.ColumnName;
            fd.FieldLength = dc.GetByteLength();

            fd.DecimalCount = 0;
            fd.WorkAreaId = 0;
            fd.ReservedMultiUserOne = Enumerable.Repeat<byte>(0x0, 2).ToArray();
            fd.ReservedMultiUserTwo = Enumerable.Repeat<byte>(0x0, 2).ToArray();
            fd.FlagSetFields = 0;
            fd.Reserved = Enumerable.Repeat<byte>(0x0, 7).ToArray();
            fd.IndexFieldFlag = 0;

            if (dc.DataType == typeof(string))
            {
                fd.FieldType = 'C';
            }
            else if (dc.DataType == typeof(long))
            {
                fd.FieldType = 'N'; //int64 (up to 18 chars according to dBase spec)
            }
            else if (dc.DataType == typeof(bool)) //nullable bool, byte
                fd.FieldType = 'L';
            else if (dc.DataType == typeof(double)) //double (8 bytes)
                fd.FieldType = 'O';
            //else if (this.DataType == typeof(dBaseShortDate)) //YYYYMMDD
            //    fd.FieldType = 'D';
            //else if (this.DataType == typeof(dBaseTimestamp)) //long1 = days since 1-Jan-4713 BC, long2 = hrs * 3600000 + min * 60000 + sec * 1000
            //    fd.FieldType = '@';
            //else if (this.DataType == typeof(dbaseAutoIncrement)) //auto-increment (long)
            //    fd.FieldType = '+';
            else
                throw new Exception($"Unknown column type: \'{dc.ColumnName}\' is {dc.DataType.ToString()}");

            return fd;
        }
    }
}
