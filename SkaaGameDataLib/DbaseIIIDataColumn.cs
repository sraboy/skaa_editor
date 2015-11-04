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
        /// on the left with nulls (0x00). This is separate and unrelated to <see cref="DataColumn.MaxLength"/>
        /// </summary>
        public int ByteLength;

        public DbaseIIIDataColumn() : base() { }
        public DbaseIIIDataColumn(string columnName) : base(columnName) { }
        public DbaseIIIDataColumn(string columnName, Type dataType) : base(columnName, dataType) { }
    }
}
