using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
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
        private class DbfFileHeader
        {
            /* Header example
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
            public byte[] LastEdited = new byte[3];
            public int NumberOfRecords;
            public short LengthOfHeader;
            public short LengthOfRecord;
            public byte[] ReservedOne = new byte[2];
            public byte IncompleteTransaction;
            public byte EncryptionFlag;
            public byte[] FreeRecordThread = new byte[4];
            public byte[] ReservedMultiUser = new byte[8];
            public byte MdxFlag;
            public byte Language;
            public byte[] ReservedTwo = new byte[2];

            public static DbfFileHeader GetDefaultHeader()
            {
                DbfFileHeader header = new DbfFileHeader();

                header.Version = 0x3;
                header.LastEdited = new byte[3] { 0x62, 0x03, 0x12 };
                header.NumberOfRecords = 0x0;
                header.LengthOfHeader = BitConverter.ToInt16(new byte[] { 0x61, 0x01 }, 0);
                header.LengthOfRecord = 0x29;
                header.ReservedOne = new byte[2] { 0x0, 0x0 };
                header.IncompleteTransaction = 0x0;
                header.EncryptionFlag = 0x0;
                header.FreeRecordThread = new byte[4] { 0x0, 0x0, 0x0, 0x0 };
                header.ReservedMultiUser = new byte[8] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                header.MdxFlag = 0x0;
                header.Language = 0x0;
                header.ReservedTwo = new byte[2] { 0x0, 0x0 };

                return header;
            }
        }
        private class FieldDescriptor
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

        //private MemoryStream _memoryStream;
        private byte _eofMarker = 0x1a;
        private byte _recordIsValidMarker = 0x20; //vs 0x2a for "is deleted"
        private DataTable _dataTable;
        private DbfFileHeader _header;
        private List<FieldDescriptor> _fieldDescriptors;
        private string _tableName;
        private int _dataSize;

        public DataTable DataTable
        {
            get
            {
                return _dataTable;
            }

            private set
            {
                this._dataTable = value;
            }
        }

        public DbfFile() { this._fieldDescriptors = new List<FieldDescriptor>(); }
        public DbfFile(string tableName)
        {
            this._fieldDescriptors = new List<FieldDescriptor>();
            this._tableName = tableName;
            this.DataTable = new DataTable();
        }

        public static DbfFile FromDataTable(DataTable dt)
        {
            DbfFile file = new DbfFile();
            file._fieldDescriptors = new List<FieldDescriptor>();

            int millenium = 2000; //Free Y3K bug 

            file._header = DbfFileHeader.GetDefaultHeader();
            file._header.NumberOfRecords = dt.Rows.Count + 1;
            file._header.LastEdited = new byte[] { (byte) (DateTime.Today.Year - millenium), (byte) DateTime.Today.Month, (byte) DateTime.Today.Day };
            file.DataTable = dt;

            return file;
        }

        public int GetSize() { return _dataSize; }
        private void SetTable()
        {

        }

        #region Reading DBF Files
        public void ReadStream(Stream str)
        {
            this._header = ReadHeader(str);
            this._dataSize = this._header.LengthOfHeader + (this._header.LengthOfRecord * this._header.NumberOfRecords);
            this._fieldDescriptors = ReadFieldDescriptors(str);
            this.FillSchemaFromFieldDescriptorList(this.DataTable);
            this.DataTable = ReadTableData(str);
        }
        /// <summary>
        /// Fills a DataTable based on the dBaseIII field descriptor information.
        /// </summary>
        /// <returns>The new DataTable</returns>
        private DataTable ReadTableData(Stream str)
        {
            //populate rows
            while (str.Position < str.Length - 1) // -1 since last byte is 0x1a for EOF
            {
                DataRow row = this.DataTable.NewRow();
                this.DataTable.Rows.Add(row);

                byte check = (byte) str.ReadByte();
                if (check != this._recordIsValidMarker)
                {
                    //check EOF first
                    if (check == this._eofMarker || str.Position < str.Length)
                        break;
                    else
                        throw new Exception(string.Format("Record is not marked as valid (preceded by 0x20)! Byte is {0}.", check.ToString()));
                }

                for (int i = 0; i < this._fieldDescriptors.Count; i++)
                {
                    FieldDescriptor fd = this._fieldDescriptors[i];
                    byte[] bytes = new byte[fd.FieldLength];
                    str.Read(bytes, 0, bytes.Length);
                    row.BeginEdit();

                    switch (fd.FieldType)
                    {
                        case 'C':
                            if (fd.FieldName.EndsWith("PTR"))
                            {
                                int val = BitConverter.ToInt32(bytes, 0);
                                row[i] = bytes == null ? "0" : val.ToString();
                            }
                            else
                                row[i] = Encoding.GetEncoding(1252).GetString(bytes);
                            break;
                        case 'N':
                            string strg = bytes == null ? "0" : Encoding.GetEncoding(1252).GetString(bytes).Trim();
                            int conv = strg == string.Empty ? 0 : Convert.ToInt32(strg);
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

                    if (i != 0 && i % _fieldDescriptors.Count == 0) //only after getting all columns, so at the end of each row
                    {
                        check = (byte) str.ReadByte();
                        if (check != this._recordIsValidMarker)
                            throw new Exception(string.Format("Record is not marked as valid (preceded by 0x20)! Byte is {0}.", check.ToString()));
                    }
                }
            }

            this.DataTable.AcceptChanges();
            this.DataTable = this.DataTable;
            return this.DataTable;
        }
        private void FillSchemaFromFieldDescriptorList(DataTable table)
        {
            foreach (FieldDescriptor fd in this._fieldDescriptors)
            {
                //DataColumn col = new DataColumn(fd.FieldName);
                DbaseIIIDataColumn col = new DbaseIIIDataColumn(fd.FieldName);
                col.DataType = typeof(object);

                //http://www.clicketyclick.dk/databases/xbase/format/data_types.html
                switch (fd.FieldType)
                {
                    case 'C': //string < 254 chars
                        col.DataType = typeof(string);
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
        private DbfFileHeader ReadHeader(Stream str)
        {
            DbfFileHeader header = new DbfFileHeader();
            //header.LastEdited = new byte[3];
            //header.ReservedOne = new byte[2];
            //header.FreeRecordThread = new byte[4];
            //header.ReservedMultiUser = new byte[8];
            //header.ReservedTwo = new byte[2];

            header.Version = (byte) str.ReadByte();
            str.Read(header.LastEdited, 0, 3);

            byte[] numRecs = new byte[4];
            str.Read(numRecs, 0, 4);
            header.NumberOfRecords = BitConverter.ToInt32(numRecs, 0);

            byte[] lenHeader = new byte[2];
            str.Read(lenHeader, 0, 2);
            header.LengthOfHeader = BitConverter.ToInt16(lenHeader, 0);

            byte[] lenRecord = new byte[2];
            str.Read(lenRecord, 0, 2);
            header.LengthOfRecord = BitConverter.ToInt16(lenRecord, 0);

            str.Read(header.ReservedOne, 0, 2);
            header.IncompleteTransaction = (byte) str.ReadByte();
            header.EncryptionFlag = (byte) str.ReadByte();
            str.Read(header.FreeRecordThread, 0, 4);
            str.Read(header.ReservedMultiUser, 0, 8);
            header.MdxFlag = (byte) str.ReadByte();
            header.Language = (byte) str.ReadByte();
            str.Read(header.ReservedTwo, 0, 2);

            return header;
        }
        private List<FieldDescriptor> ReadFieldDescriptors(Stream str)
        {
            //todo: document this... I have no idea what I did here.
            List<FieldDescriptor> fdlist = new List<FieldDescriptor>();

            byte check = 0x0;

            while (check != 0xD)
            {
                if(check != 0x0)
                    str.Position--; //backup since we read an extra byte to get the check value

                FieldDescriptor fd = new FieldDescriptor();

                byte[] fieldName = new byte[11];
                str.Read(fieldName, 0, 11);
                fd.FieldName = Encoding.UTF8.GetString(fieldName).Trim(new char[] { '\0' });

                fd.FieldType = (char) str.ReadByte();

                fd.FieldDataAddress = new byte[4];
                str.Read(fd.FieldDataAddress, 0, 4);

                fd.FieldLength = (byte) str.ReadByte();
                fd.DecimalCount = (byte) str.ReadByte();

                fd.ReservedMultiUserOne = new byte[2];
                str.Read(fd.ReservedMultiUserOne, 0, 2);

                fd.WorkAreaId = (byte) str.ReadByte();

                fd.ReservedMultiUserTwo = new byte[2];
                str.Read(fd.ReservedMultiUserTwo, 0, 2);

                fd.FlagSetFields = (byte) str.ReadByte();

                fd.Reserved = new byte[7];
                str.Read(fd.Reserved, 0, 7);

                fd.IndexFieldFlag = (byte) str.ReadByte();

                fdlist.Add(fd);

                check = (byte) str.ReadByte();               
            }

            return fdlist;
        }
        #endregion

        public void WriteToStream(Stream fs)
        {
            WriteHeader(fs);
            WriteFieldDescriptors(fs);
            WriteDataTableToStream(fs);
        }
        public void WriteAndClose(string path)
        {
            string fileName = this._tableName + ".dbf";
            using (FileStream fs = new FileStream(path + '\\' + fileName, FileMode.Create))
            { 
                WriteHeader(fs);
                WriteFieldDescriptors(fs);
                WriteDataTableToStream(fs);
            }
        }

        private void WriteHeader(Stream str)
        {
            str.WriteByte(this._header.Version);
            str.Write(this._header.LastEdited, 0, 3);
            str.Write(BitConverter.GetBytes(this._header.NumberOfRecords), 0, 4);
            str.Write(BitConverter.GetBytes(this._header.LengthOfHeader), 0, 2);
            str.Write(BitConverter.GetBytes(this._header.LengthOfRecord), 0, 2);
            str.Write(this._header.ReservedOne, 0, 2);
            str.WriteByte(this._header.IncompleteTransaction);
            str.WriteByte(this._header.EncryptionFlag);
            str.Write(this._header.FreeRecordThread, 0, 4);
            str.Write(this._header.ReservedMultiUser, 0, 8);
            str.WriteByte(this._header.MdxFlag);
            str.WriteByte(this._header.Language);
            str.Write(this._header.ReservedTwo, 0, 2);
        }
        private void WriteFieldDescriptors(Stream str)
        {
            foreach (FieldDescriptor fd in this._fieldDescriptors)
            {
                StringBuilder sb = new StringBuilder(fd.FieldName);
                sb.Append((char) 0x0, 11 - fd.FieldName.Length);
                string writeme = sb.ToString();
                str.Write(Encoding.UTF8.GetBytes(writeme), 0, 11);

                str.WriteByte((byte) fd.FieldType);
                str.Write(fd.FieldDataAddress, 0, 4);
                str.WriteByte(fd.FieldLength);
                str.WriteByte(fd.DecimalCount);
                str.Write(fd.ReservedMultiUserOne, 0, 2);
                str.WriteByte(fd.WorkAreaId);
                str.Write(fd.ReservedMultiUserTwo, 0, 2);
                str.WriteByte(fd.FlagSetFields);
                str.Write(fd.Reserved, 0, 7);
                str.WriteByte(fd.IndexFieldFlag);
            }

            str.WriteByte(0xD);
        }
        private void WriteDataTableToStream(Stream str)
        {
            foreach(DataRow dr in this.DataTable.Rows)
            {
                str.WriteByte(this._recordIsValidMarker); //row divider
                foreach (DbaseIIIDataColumn col in this.DataTable.Columns)
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
                        str.Write(bytes, 0, col.ByteLength);
                    }
                    else if (col.DataType == typeof(long))
                    {
                        long val = (long) dr[col];
                        value = val.ToString();
                        value = value.PadLeft(col.ByteLength, ' ');
                        byte[] bytes = new byte[col.ByteLength];
                        Encoding.GetEncoding(1252).GetBytes(value, 0, col.ByteLength, bytes, 0);
                        str.Write(bytes, 0, col.ByteLength);
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

            str.WriteByte(this._eofMarker);
        }
    }
}
