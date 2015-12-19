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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public class DbaseIIIDataColumn : DataColumn
    {
        /// <summary>
        /// Describes the length, in bytes, that this field must occupy in the DBF file. Text fields
        /// should be padded on the right with spaces (0x20) while number fields should be padded
        /// on the left with nulls (0x00). Numbers can be "padded" by using a <see cref="Convert"/> 
        /// function that corresponds to the needed number type. This field is separate and 
        /// unrelated to the <see cref="DataColumn.MaxLength"/> property.
        /// </summary>
        public byte ByteLength;

        public DbaseIIIDataColumn() : base() { }
        public DbaseIIIDataColumn(string columnName) : base(columnName) { }
        public DbaseIIIDataColumn(string columnName, Type dataType) : base(columnName, dataType) { }

        internal DbfFile.FieldDescriptor GetFieldDescriptor()
        {
            DbfFile.FieldDescriptor fd = new DbfFile.FieldDescriptor();

            fd.FieldName = this.ColumnName;
            fd.FieldLength = this.ByteLength;
            
            fd.DecimalCount = 0;
            fd.WorkAreaId = 0;
            fd.ReservedMultiUserOne = Enumerable.Repeat<byte>(0x0, 2).ToArray();
            fd.ReservedMultiUserTwo = Enumerable.Repeat<byte>(0x0, 2).ToArray();
            fd.FlagSetFields = 0;
            fd.Reserved = Enumerable.Repeat<byte>(0x0, 7).ToArray();
            fd.IndexFieldFlag = 0;

            if (this.DataType == typeof(string))
            {
                fd.FieldType = 'C';
            }
            else if (this.DataType == typeof(long))
            {
                fd.FieldType = 'N'; //int64 (up to 18 chars according to dBase spec)
            }
            else if (this.DataType == typeof(bool)) //nullable bool, byte
                fd.FieldType = 'L';
            else if (this.DataType == typeof(double)) //double (8 bytes)
                fd.FieldType = 'O';
            //else if (this.DataType == typeof(dBaseShortDate)) //YYYYMMDD
            //    fd.FieldType = 'D';
            //else if (this.DataType == typeof(dBaseTimestamp)) //long1 = days since 1-Jan-4713 BC, long2 = hrs * 3600000 + min * 60000 + sec * 1000
            //    fd.FieldType = '@';
            //else if (this.DataType == typeof(dbaseAutoIncrement)) //auto-increment (long)
            //    fd.FieldType = '+';
            else
                throw new Exception($"Unknown column type: \'{this.ColumnName}\' is {this.DataType.ToString()}");

            return fd;
        }
    }
}
