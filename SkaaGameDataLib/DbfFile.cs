using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

    /// <summary>
    /// Represents a dBase III (not to be confused with dBase III+ or FoxPro) file and makes it manipulable via a DataTable.
    /// </summary>
    /// <remarks>
    /// Constructed with help from: 
    /// http://www.clicketyclick.dk/databases/xbase/format/dbf.html 
    /// http://www.digitalpreservation.gov/formats/fdd/fdd000325.shtml
    /// </remarks>
    [Serializable]
    public class DbfFile
    {
        public struct DbfPreHeader
        {
            /*
            03 version
            62 03 12 yymmdd last edited
            58 23 00 00 number of records
            61 01 length of header
            29 00 length of record
            00 00 reserved
            00 inc transaction
            00 encryption
            00 00 00 00 free record thread
            00 00 00 00 00 00 00 00 reserved for multi-user
            00 mdx
            00 lang
            00 00 reserved
            */

            public byte Version;
            public byte[] LastEdited;
            public int NumberOfRecords;
            public short LengthOfHeader;
            public short LengthOfRecord;
            public byte[] ReservedOne;
            public byte IncompleTransaction;
            public byte EncryptionFlag;
            public byte[] FreeRecordThread;
            public byte[] ReservedMultiUser;
            public byte MdxFlag;
            public byte Language;
            public byte[] ReservedTwo;
        }
        public struct FieldDescriptor
        {
            public string FieldName; //char[11]
            public char FieldType;
            public byte[] FieldDataAddress;
            public byte FieldLength;
            public byte DecimalCount;
            public byte[] ReservedMultiUserOne;
            public byte WorkAreaId;
            public byte[] ReservedMultiUserTwo;
            public byte FlagSetFields;
            public byte[] Reserved;
            public byte IndexFieldFlag;
        }

        private MemoryStream _memoryStream;
        private byte _eofMarker = 0x1a;
        private byte _recordIsValidMarker = 0x20; //vs 0x2a for "is deleted"
        private DataTable _dataTable;

        public DbfPreHeader Header;
        public List<FieldDescriptor> FieldDescriptorArray;
        public string TableName;
        public int DataSize;

        public DbfFile(MemoryStream rawData, string tableName, int dataSize)
        {
            this._memoryStream = rawData;
            this._memoryStream.Position = 0;

            this.Header = new DbfPreHeader();
            this.FieldDescriptorArray = new List<FieldDescriptor>();
            this.TableName = tableName;
            this.DataSize = dataSize;

            this.Header.LastEdited = new byte[3];
            this.Header.ReservedOne = new byte[2];
            this.Header.FreeRecordThread = new byte[4];
            this.Header.ReservedMultiUser = new byte[8];
            this.Header.ReservedTwo = new byte[2];

            ReadPreHeader();
            ReadFieldDescriptors();
        }
        /// <summary>
        /// Reads a DBF file assuming no type information and provides byte arrays for each cell. It
        /// is up to the caller to resolve data types. A columns are of type object.
        /// </summary>
        /// <returns>A DataTable containing records filled with byte arrays.</returns>
        public DataTable RawFill()
        {
            //todo: verify previous byte is still 0xD

            DataTable table = new DataTable(this.TableName);

            //build columns
            foreach (FieldDescriptor fd in this.FieldDescriptorArray)
            {
                DataColumn col = new DataColumn(fd.FieldName);
                col.DataType = typeof(object);
                table.Columns.Add(col);
            }

            //populate rows
            while (this._memoryStream.Position < this._memoryStream.Length)
            {
                DataRow row = table.NewRow();
                table.Rows.Add(row);

                for (int i = 0; i < this.FieldDescriptorArray.Count; i++)
                {
                    FieldDescriptor fd = this.FieldDescriptorArray[i];
                    byte[] val = new byte[fd.FieldLength];
                    this._memoryStream.Read(val, 0, val.Length);
                    row.BeginEdit();
                    row[i] = val;
                    row.AcceptChanges();
                }
            }

            table.AcceptChanges();
            return table;
        }
        /// <summary>
        /// Fills a DataTable based on the dBaseIII field descriptor information.
        /// </summary>
        /// <returns>The new DataTable</returns>
        public DataTable FillAndGetTable()
        {
            DataTable table = new DataTable(this.TableName);

            //build columns
            FillSchema(table);

            //populate rows
            while (this._memoryStream.Position < this._memoryStream.Length - 1) // -1 since last byte is 0x1a for EOF
            {
                DataRow row = table.NewRow();
                table.Rows.Add(row);

                byte check = (byte) this._memoryStream.ReadByte();
                if (check != this._recordIsValidMarker)
                {
                    //check EOF first
                    if (check == this._eofMarker || this._memoryStream.Position < this._memoryStream.Length)
                        break;
                    else
                        throw new Exception(string.Format("Record is not marked as valid (preceded by 0x20)! Byte is {0}.", check.ToString()));
                }

                for (int i = 0; i < this.FieldDescriptorArray.Count; i++)
                {
                    FieldDescriptor fd = this.FieldDescriptorArray[i];
                    byte[] bytes = new byte[fd.FieldLength];
                    this._memoryStream.Read(bytes, 0, bytes.Length);
                    row.BeginEdit();
                    //row[i] = bytes;

                    switch (fd.FieldType)
                    {
                        case 'C':
                            //if (fd.FieldName == "BITMAPPTR")
                            if(fd.FieldName.EndsWith("PTR"))
                            {
                                int val = BitConverter.ToInt32(bytes, 0);
                                row[i] = bytes == null ? "0" : val.ToString();
                            }
                            else
                                row[i] = Encoding.GetEncoding(1252).GetString(bytes);
                            break;
                        case 'N':
                            string str = bytes == null ? "0" : Encoding.GetEncoding(1252).GetString(bytes).Trim();
                            int conv = str == string.Empty ? 0 : Convert.ToInt32(str);// BitConverter.ToInt16(val, 1);
                            row[i] = conv;
                            break;
                        case 'L': //nullable bool, byte
                            throw new NotImplementedException("Encountered /'L/' for nullable bool!");
                            break;
                        case 'D': //YYYYMMDD
                            throw new NotImplementedException("Encountered /'D/' for YYYYMMDD!");
                            break;
                        case '@': //long1 = days since 1 Jan 4713, long2 = hrs * 3600000 + min * 60000 + sec * 1000
                            throw new NotImplementedException("Encountered /'@/' for time!");
                            break;
                        case 'O': //double (8 bytes)
                            throw new NotImplementedException("Encountered /'O/' for double!");
                            break;
                        case '+': //auto-increment (long)
                            throw new NotImplementedException("Encountered /'+/' for auto-increment!");
                            break;
                    }

                    row.AcceptChanges();

                    if (i != 0 && i % FieldDescriptorArray.Count == 0) //only after getting all columns, so at the end of each row
                    {
                        check = (byte) this._memoryStream.ReadByte();
                        if (check != this._recordIsValidMarker)
                            throw new Exception(string.Format("Record is not marked as valid (preceded by 0x20)! Byte is {0}.", check.ToString()));
                    }
                }
            }

            if (_memoryStream.ReadByte() != this._eofMarker)
                throw new Exception("Expected byte of 0x1A for EOF!");

            table.AcceptChanges();
            this._dataTable = table;
            return this._dataTable;
        }
        public void FillSchema(DataTable table)
        {
            foreach (FieldDescriptor fd in this.FieldDescriptorArray)
            {
                //DataColumn col = new DataColumn(fd.FieldName);
                DbaseIIIDataColumn col = new DbaseIIIDataColumn(fd.FieldName);
                col.DataType = typeof(object);

                //http://www.clicketyclick.dk/databases/xbase/format/data_types.html
                switch (fd.FieldType)
                {
                    case 'C': //string < 254 chars
                        col.DataType = typeof(string);
                        //col.MaxLength = 11;// 10 for 32-bit int max (4,294,967,295) + /0
                        //Below ensures we can fit an int's string representation since the original 
                        //length is specified in bytes for the char[]. Go larger if specified (like DES in HEADER.DBF).
                        col.MaxLength = fd.FieldLength < 11 ? 11 : fd.FieldLength;
                        col.ByteLength = fd.FieldLength;// + 1; //+ 1 for null terminator
                        break;
                    case 'N': //int64 (up to 18 chars according to dBase spec)
                        col.DataType = typeof(long);
                        col.ByteLength = fd.FieldLength;
                        break;
                    case 'L': //nullable bool, byte
                        throw new NotImplementedException("Encountered /'L/' for nullable bool!");
                        break;
                    case 'D': //YYYYMMDD
                        throw new NotImplementedException("Encountered /'D/' for YYYYMMDD!");
                        break;
                    case '@': //long1 = days since 1 Jan 4713, long2 = hrs * 3600000 + min * 60000 + sec * 1000
                        throw new NotImplementedException("Encountered /'@/' for time!");
                        break;
                    case 'O': //double (8 bytes)
                        throw new NotImplementedException("Encountered /'O/' for double!");
                        break;
                    case '+': //auto-increment (long)
                        throw new NotImplementedException("Encountered /'+/' for auto-increment!");
                        break;
                }

                table.Columns.Add(col);
            }
        }

        private void ReadPreHeader()
        {
            this.Header.Version = (byte) _memoryStream.ReadByte();
            _memoryStream.Read(this.Header.LastEdited, 0, 3);

            byte[] numRecs = new byte[4];
            _memoryStream.Read(numRecs, 0, 4);
            this.Header.NumberOfRecords = BitConverter.ToInt32(numRecs, 0);

            byte[] lenHeader = new byte[2];
            _memoryStream.Read(lenHeader, 0, 2);
            this.Header.LengthOfHeader = BitConverter.ToInt16(lenHeader, 0);

            byte[] lenRecord = new byte[2];
            _memoryStream.Read(lenRecord, 0, 2);
            this.Header.LengthOfRecord = BitConverter.ToInt16(lenRecord, 0);

            _memoryStream.Read(this.Header.ReservedOne, 0, 2);
            this.Header.IncompleTransaction = (byte) _memoryStream.ReadByte();
            this.Header.EncryptionFlag = (byte) _memoryStream.ReadByte();
            _memoryStream.Read(this.Header.FreeRecordThread, 0, 4);
            _memoryStream.Read(this.Header.ReservedMultiUser, 0, 8);
            this.Header.MdxFlag = (byte) _memoryStream.ReadByte();
            this.Header.Language = (byte) _memoryStream.ReadByte();
            _memoryStream.Read(this.Header.ReservedTwo, 0, 2);
        }
        private void ReadFieldDescriptors()
        {
            byte check = 0x0;

            while (check != 0xD)
            {
                if(check != 0x0)
                    _memoryStream.Position--;

                FieldDescriptor fd = new FieldDescriptor();

                byte[] fieldName = new byte[11];
                _memoryStream.Read(fieldName, 0, 11);
                fd.FieldName = Encoding.UTF8.GetString(fieldName).Trim(new char[] { '\0' });

                fd.FieldType = (char) _memoryStream.ReadByte();

                fd.FieldDataAddress = new byte[4];
                _memoryStream.Read(fd.FieldDataAddress, 0, 4);

                fd.FieldLength = (byte) _memoryStream.ReadByte();
                fd.DecimalCount = (byte) _memoryStream.ReadByte();

                fd.ReservedMultiUserOne = new byte[2];
                _memoryStream.Read(fd.ReservedMultiUserOne, 0, 2);

                fd.WorkAreaId = (byte) _memoryStream.ReadByte();

                fd.ReservedMultiUserTwo = new byte[2];
                _memoryStream.Read(fd.ReservedMultiUserTwo, 0, 2);

                fd.FlagSetFields = (byte) _memoryStream.ReadByte();

                fd.Reserved = new byte[7];
                _memoryStream.Read(fd.Reserved, 0, 7);

                fd.IndexFieldFlag = (byte) _memoryStream.ReadByte();

                this.FieldDescriptorArray.Add(fd);

                check = (byte) _memoryStream.ReadByte();
                //if (check == 0xD)
                //    break;
                //else
                
            }
        }
        
        public void WriteAndClose(string path)
        {
            string fileName = this.TableName + ".dbf";
            using (FileStream fs = new FileStream(path + '\\' + fileName, FileMode.Create))
            { 
                WritePreHeader(fs);
                WriteFieldDescriptors(fs);
                WriteDataTable(fs);
            }
        }
        private void WritePreHeader(FileStream fs)
        {
            fs.WriteByte(this.Header.Version);
            fs.Write(this.Header.LastEdited, 0, 3);
            fs.Write(BitConverter.GetBytes(this.Header.NumberOfRecords), 0, 4);
            fs.Write(BitConverter.GetBytes(this.Header.LengthOfHeader), 0, 2);
            fs.Write(BitConverter.GetBytes(this.Header.LengthOfRecord), 0, 2);
            fs.Write(this.Header.ReservedOne, 0, 2);
            fs.WriteByte(this.Header.IncompleTransaction);
            fs.WriteByte(this.Header.EncryptionFlag);
            fs.Write(this.Header.FreeRecordThread, 0, 4);
            fs.Write(this.Header.ReservedMultiUser, 0, 8);
            fs.WriteByte(this.Header.MdxFlag);
            fs.WriteByte(this.Header.Language);
            fs.Write(this.Header.ReservedTwo, 0, 2);
        }
        private void WriteFieldDescriptors(FileStream fs)
        {
            foreach (FieldDescriptor fd in this.FieldDescriptorArray)
            {
                StringBuilder sb = new StringBuilder(fd.FieldName);
                sb.Append((char) 0x0, 11 - fd.FieldName.Length);
                string writeme = sb.ToString();
                fs.Write(Encoding.UTF8.GetBytes(writeme), 0, 11);

                fs.WriteByte((byte) fd.FieldType);
                fs.Write(fd.FieldDataAddress, 0, 4);
                fs.WriteByte(fd.FieldLength);
                fs.WriteByte(fd.DecimalCount);
                fs.Write(fd.ReservedMultiUserOne, 0, 2);
                fs.WriteByte(fd.WorkAreaId);
                fs.Write(fd.ReservedMultiUserTwo, 0, 2);
                fs.WriteByte(fd.FlagSetFields);
                fs.Write(fd.Reserved, 0, 7);
                fs.WriteByte(fd.IndexFieldFlag);
            }

            fs.WriteByte(0xD);
        }
        private void WriteDataTable(FileStream fs)
        {
            foreach(DataRow dr in this._dataTable.Rows)
            {
                fs.WriteByte(this._recordIsValidMarker); //row divider
                foreach (DbaseIIIDataColumn col in this._dataTable.Columns)
                {
                    string value;
                    if (col.DataType == typeof(string))
                    {
                        byte[] bytes = new byte[col.ByteLength];
                        value = (string) dr[col];

                        if (col.ColumnName.EndsWith("PTR"))
                        {
                            int val = value == "0" ? 0 : Convert.ToInt32(value);
                            bytes = BitConverter.GetBytes(val);
                        }
                        else
                        {
                            value = (string) dr[col];
                            value.PadRight(col.ByteLength, ' ');
                            bytes = Encoding.GetEncoding(1252).GetBytes(value);
                        }
                        fs.Write(bytes, 0, col.ByteLength);
                    }
                    else if (col.DataType == typeof(long))
                    {
                        long val = (long) dr[col];
                        value = val.ToString();
                        value = value.PadLeft(col.ByteLength, ' ');
                        byte[] bytes = new byte[col.ByteLength];
                        Encoding.GetEncoding(1252).GetBytes(value, 0, col.ByteLength, bytes, 0);
                        fs.Write(bytes, 0, col.ByteLength);
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("Unknown DataColumn Type: {0}", col.DataType.ToString()));
                        //case 'L': //nullable bool, byte
                        //    throw new NotImplementedException("Encountered /'L/' for nullable bool!");
                        //    break;
                        //case 'D': //YYYYMMDD
                        //    throw new NotImplementedException("Encountered /'D/' for YYYYMMDD!");
                        //    break;
                        //case '@': //long1 = days since 1 Jan 4713, long2 = hrs * 3600000 + min * 60000 + sec * 1000
                        //    throw new NotImplementedException("Encountered /'@/' for time!");
                        //    break;
                        //case 'O': //double (8 bytes)
                        //    throw new NotImplementedException("Encountered /'O/' for double!");
                        //    break;
                        //case '+': //auto-increment (long)
                        //    throw new NotImplementedException("Encountered /'+/' for auto-increment!");
                        //    break;
                    }
                }
            }

            fs.WriteByte(this._eofMarker);
        }
    }
}
