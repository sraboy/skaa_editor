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
