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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SkaaGameDataLib
{
    /// <summary>
    /// Represents a dBaseIII table (not to be confused with dBaseIII+ or FoxPro) in a DBF file and makes it manipulable via a DataTable. 
    /// </summary>
    /// <remarks>
    /// Constructed with help from: 
    /// http://www.clicketyclick.dk/databases/xbase/format/dbf.html 
    /// http://www.digitalpreservation.gov/formats/fdd/fdd000325.shtml
    /// </remarks>
    [Serializable]
    public class DbfFile
    {
        internal class DbfFileHeader
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
                //header.NumberOfRecords = 0x0;
                //header.LengthOfHeader = BitConverter.ToInt16(new byte[] { 0x61, 0x01 }, 0);
                //header.LengthOfRecord = ResourceDatabase.DefinitionSize;
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
        internal class FieldDescriptor
        {
            public const int Size = 11 + 1 + 4 + 1 + 1 + 2 + 1 + 2 + 1 + 7 + 1;

            public string FieldName; //char[11]
            public char FieldType;
            public byte[] FieldDataAddress = new byte[4];
            public byte FieldLength;
            public byte DecimalCount;
            public byte[] ReservedMultiUserOne = new byte[2];
            public byte WorkAreaId;
            public byte[] ReservedMultiUserTwo = new byte[2];
            public byte FlagSetFields;
            public byte[] Reserved = new byte[7];
            public byte IndexFieldFlag;
        }

        private DataTable _dataTable;
        private DbfFileHeader _header;
        private List<FieldDescriptor> _fieldDescriptors;
        private string _tableName;
        private int _dataSize;

        internal const byte RecordValidMarker = 0x20;
        internal const byte RecordDeletedMarker = 0x2a;
        internal const byte EofMarker = 0x1a;
        internal const byte Terminator = 0xD; //follows the header, after the list of FieldDescriptors

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
        internal DbfFileHeader Header
        {
            get
            {
                return _header;
            }

            set
            {
                this._header = value;
            }
        }
        internal List<FieldDescriptor> FieldDescriptors
        {
            get
            {
                return _fieldDescriptors;
            }

            set
            {
                this._fieldDescriptors = value;
            }
        }
        internal string TableName
        {
            get
            {
                return _tableName;
            }

            set
            {
                this._tableName = value;
            }
        }
        internal int DataSize
        {
            get
            {
                return _dataSize;
            }

            set
            {
                this._dataSize = value;
            }
        }

        #region Constructors
        public DbfFile() { this._dataTable = new DataTable(); }
        public DbfFile(DataTable dt)
        {
            int millenium = 2000; //Free Y3K bug 

            this.Header = DbfFileHeader.GetDefaultHeader();
            this.Header.NumberOfRecords = dt.Rows.Count;// + 1;
            this.Header.LastEdited = new byte[] { (byte)(DateTime.Today.Year - millenium), (byte)DateTime.Today.Month, (byte)DateTime.Today.Day };

            this.Header.LengthOfHeader = (short)
                                      (sizeof(byte) //version
                                       + this.Header.LastEdited.Length
                                       + sizeof(int) //NumberOfRecords
                                       + sizeof(short) //LengthOfHeader
                                       + sizeof(short) //LengthOfRecord
                                       + this.Header.ReservedOne.Length
                                       + sizeof(byte) //IncompleteTransaction
                                       + sizeof(byte) //EncryptionFlag
                                       + this.Header.FreeRecordThread.Length
                                       + this.Header.ReservedMultiUser.Length
                                       + sizeof(byte) //MdxFlag
                                       + sizeof(byte) //Language
                                       + this.Header.ReservedTwo.Length
                                       //+ (this.Header.NumberOfRecords * this.Header.LengthOfRecord)
                                       //+ (this.Header.NumberOfRecords * FieldDescriptor.Size)
                                       + sizeof(byte) //Terminator
                                       );

            this._dataTable = dt;
        }
        #endregion

        //public static DbfFile FromDataTable(DataTable dt)
        //{
        //    DbfFile file = new DbfFile();
        //    file._fieldDescriptors = new List<FieldDescriptor>();
        //    int millenium = 2000; //Free Y3K bug 
        //    file._header = DbfFileHeader.GetDefaultHeader();
        //    file._header.NumberOfRecords = dt.Rows.Count + 1;
        //    file._header.LastEdited = new byte[] { (byte) (DateTime.Today.Year - millenium), (byte) DateTime.Today.Month, (byte) DateTime.Today.Day };
        //    file.DataTable = dt;
        //    return file;
        //}

        //public int GetSize() { return _dataSize; }

        #region Reading DBF Files
        //todo: make these static extensions to DataTable
        public bool ReadStream(Stream str)
        {
            this.Header = ReadHeader(str);
            if (this.Header == null)
            {
                return false;
            }
            this.DataSize = this.Header.LengthOfHeader + (this.Header.LengthOfRecord * this.Header.NumberOfRecords);
            this.FieldDescriptors = ReadFieldDescriptors(str);
            this.FillSchemaFromFieldDescriptorList(this.DataTable);
            this._dataTable = ReadTableData(str);
            return true;
        }
        internal static DbfFileHeader ReadHeader(Stream str)
        {
            DbfFileHeader header = new DbfFileHeader();

            header.Version = (byte) str.ReadByte();
            if (header.Version != 3)
                return null;
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

                if (str.Position > str.Length)
                    throw new Exception("Attempted to read past end of file!");

                byte check = (byte) str.ReadByte();

                if (check == EofMarker)
                {
                    row.Delete();
                    break;
                }
                else if (check == RecordValidMarker)
                {
                    for (int i = 0; i < this.FieldDescriptors.Count; i++)
                    {
                        FieldDescriptor fd = this.FieldDescriptors[i];
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
                                    row[i] = Encoding.GetEncoding(1252).GetString(bytes).Trim('\0');
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

                        //if (i != 0 && i % FieldDescriptors.Count == 0) //only after getting all columns, so at the end of each row
                        //{
                        //    check = (byte) str.ReadByte();
                        //    if (check != this.RecordIsValidMarker)
                        //        throw new Exception(string.Format("Record is not marked as valid (preceded by 0x20)! Byte is {0}.", check.ToString()));
                        //}
                    }
                }
                else if (check == RecordDeletedMarker)
                    row.Delete();
                else
                    throw new Exception($"Unknown row state: {check}.");
            }

            this._dataTable.AcceptChanges();
            this._dataTable = this.DataTable;
            return this.DataTable;
        }
        private void FillSchemaFromFieldDescriptorList(DataTable table)
        {
            foreach (FieldDescriptor fd in this.FieldDescriptors)
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
                        col.ByteLength = fd.FieldLength;//no null terminator
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
                    case '@': //long1 = days since 1-Jan-4713 BC, long2 = hrs * 3600000 + min * 60000 + sec * 1000
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
        private List<FieldDescriptor> ReadFieldDescriptors(Stream str)
        {
            List<FieldDescriptor> fdlist = new List<FieldDescriptor>();

            byte checkForTerminator = 0x0;

            while (checkForTerminator != Terminator)
            {
                if(checkForTerminator != 0x0)
                    str.Position--; //backup since we read an extra byte to get the check value

                FieldDescriptor fd = new FieldDescriptor();

                byte[] fieldName = new byte[11];
                str.Read(fieldName, 0, 11);
                fd.FieldName = Encoding.UTF8.GetString(fieldName).Trim(new char[] { '\0' });

                fd.FieldType = (char) str.ReadByte();

                str.Read(fd.FieldDataAddress, 0, 4);

                fd.FieldLength = (byte) str.ReadByte();
                fd.DecimalCount = (byte) str.ReadByte();
                Debug.Assert(fd.DecimalCount == 0, $"Encounted non-zero DecimalCount: {fd.DecimalCount}."); //if found, must be < 15 per spec

                str.Read(fd.ReservedMultiUserOne, 0, 2);
                Debug.Assert(fd.ReservedMultiUserOne[0] == 0 && fd.ReservedMultiUserOne[1] == 0, "Encounted non-zero ReservedMultiUserOne.");

                fd.WorkAreaId = (byte) str.ReadByte();
                Debug.Assert(fd.WorkAreaId == 0, $"Encounted non-zero WorkAreaId: {fd.WorkAreaId}.");

                str.Read(fd.ReservedMultiUserTwo, 0, 2);
                Debug.Assert(fd.ReservedMultiUserTwo[0] == 0 && fd.ReservedMultiUserTwo[1] == 0, "Encounted non-zero ReservedMultiUserTwo.");

                fd.FlagSetFields = (byte) str.ReadByte();
                Debug.Assert(fd.WorkAreaId == 0, $"Encounted non-zero WorkAreaId: {fd.WorkAreaId}.");

                str.Read(fd.Reserved, 0, 7);
                foreach(byte b in fd.Reserved)
                    Debug.Assert(b == 0, $"Encounted non-zero Reserved: {b}.");

                fd.IndexFieldFlag = (byte) str.ReadByte();
                Debug.Assert(fd.IndexFieldFlag == 0, $"Encounted non-zero IndexFieldFlag: {fd.IndexFieldFlag}.");

                fdlist.Add(fd);

                checkForTerminator = (byte) str.ReadByte();               
            }

            return fdlist;
        }
        #endregion
    }
}
