using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public static class Misc
    {
        public static uint? GetNullableUInt32FromIndex(this DataRow dr, int itemArrayIndex)
        {
            string x = dr[itemArrayIndex].ToString();

            /* Codepage 437 is needed due to the use of Extended ASCII.
             *
             * For example, in SFRAME.dbf, the BITMAPPTR is labeled as a
             * CHAR type (which is just a string in dBase III parlance).
             * Because of this, it's not possible to read the bytes in as
             * numbers, even though they're just used as pointers in the 
             * original code.
             */
            byte[] bytes = Encoding.GetEncoding(437).GetBytes(x);

            uint? offset = null;

            switch (bytes.Length)
            {
                case 0:
                    offset = 0;
                    break;
                case 1:
                    offset = bytes[0];
                    break;
                case 2:
                    offset = BitConverter.ToUInt16(bytes, 0);
                    break;
                case 3:
                    byte[] copy = new byte[4];
                    bytes.CopyTo(copy, 0);
                    offset = BitConverter.ToUInt32(copy, 0);
                    break;
                case 4:
                    offset = BitConverter.ToUInt32(bytes, 0);
                    break;
                default:
                    throw new ArgumentNullException("There was an issue reading offsets from the sprite's DataTable that will cause the variable \'offset\' to remain null.");
            }

            return offset;
        }

        public static int CompareFrameOffset(SpriteFrame a, SpriteFrame b)
        {
            if (a.SprBitmapOffset <= b.SprBitmapOffset)
                return (int) a.SprBitmapOffset;
            else
                return (int) b.SprBitmapOffset;

            //uint? aLen = a.GameSetDataRow.GetNullableUInt32FromIndex(9);
            //uint? bLen = b.GameSetDataRow.GetNullableUInt32FromIndex(9);

            //if (aLen.Value <= bLen.Value)
            //    return (int) aLen.Value;
            //else
            //    return (int) bLen.Value;
        }
    }

    [Serializable]
    public struct ResIndex
    {
        //Lifted from ORESX.h, except string vs char[9]
        public string name;
        public int offset;
    };

    /// <summary>
    /// [INCOMPLETE] Does not yet read the data from the file, only the table's schema data in the header.
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

        public DbfPreHeader Header;
        public List<FieldDescriptor> FieldDescriptorArray;
        //public ResIndex ResourceIndex;
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
        /// is up to the caller to resolve data types.
        /// </summary>
        /// <returns>A DataTable containing records filled with byte arrays.</returns>
        public DataTable GetRawDataTable()
        {
            //todo: verify previous byte is still 0xD

            DataTable table = new DataTable(this.TableName);

            //build columns
            foreach (FieldDescriptor fd in this.FieldDescriptorArray)
            {
                DataColumn col = new DataColumn(fd.FieldName);
                col.DataType = typeof(object);

                //http://www.clicketyclick.dk/databases/xbase/format/data_types.html
                //switch (fd.FieldType)
                //{
                //    case 'C': //string < 254 chars
                //        col.DataType = typeof(string);
                //        col.MaxLength = fd.FieldLength + 1; //+ 1 for null terminator
                //        break;
                //    case 'N': //int64 (up to 18 chars)
                //        col.DataType = typeof(long);
                //        break;
                //    case 'L': //nullable bool, byte
                //        throw new NotImplementedException("Encountered /'L/' for nullable bool!");
                //        break;
                //    case 'D': //YYYYMMDD
                //        throw new NotImplementedException("Encountered /'D/' for YYYYMMDD!");
                //        break;
                //    case '@': //long1 = days since 1 Jan 4713, long2 = hrs * 3600000 + min * 60000 + sec * 1000
                //        throw new NotImplementedException("Encountered /'@/' for time!");
                //        break;
                //    case 'O': //double (8 bytes)
                //        throw new NotImplementedException("Encountered /'O/' for double!");
                //        break;
                //    case '+': //auto-increment (long)
                //        throw new NotImplementedException("Encountered /'+/' for auto-increment!");
                //        break;
                //}

                table.Columns.Add(col);
            }

            //populate rows
            while(this._memoryStream.Position < this._memoryStream.Length)
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
                    //switch(fd.FieldType)
                    //{
                    //    case 'C':
                    //        row[i] = System.Text.Encoding.ASCII.GetString(val);
                    //        break;
                    //    case 'N':
                    //        string str = System.Text.Encoding.ASCII.GetString(val);
                    //        int conv = Convert.ToInt32(str);// BitConverter.ToInt16(val, 1);
                    //        row[i] = conv;
                    //        break;
                    //    case 'L': //nullable bool, byte
                    //        throw new NotImplementedException("Encountered /'L/' for nullable bool!");
                    //        break;
                    //    case 'D': //YYYYMMDD
                    //        throw new NotImplementedException("Encountered /'D/' for YYYYMMDD!");
                    //        break;
                    //    case '@': //long1 = days since 1 Jan 4713, long2 = hrs * 3600000 + min * 60000 + sec * 1000
                    //        throw new NotImplementedException("Encountered /'@/' for time!");
                    //        break;
                    //    case 'O': //double (8 bytes)
                    //        throw new NotImplementedException("Encountered /'O/' for double!");
                    //        break;
                    //    case '+': //auto-increment (long)
                    //        throw new NotImplementedException("Encountered /'+/' for auto-increment!");
                    //        break;
                    //}
                    row.AcceptChanges();
                }
            }

            table.AcceptChanges();
            return table;
        }
        public DataTable GetConvertedRawDataTable()
        {
            DataTable table = new DataTable(this.TableName);

            //build columns
            foreach (FieldDescriptor fd in this.FieldDescriptorArray)
            {
                DataColumn col = new DataColumn(fd.FieldName);
                col.DataType = typeof(object);

                //http://www.clicketyclick.dk/databases/xbase/format/data_types.html
                switch (fd.FieldType)
                {
                    case 'C': //string < 254 chars
                        col.DataType = typeof(string);
                        col.MaxLength = 11;// 10 for 32-bit int max (4,294,967,295) + /0      // fd.FieldLength + 1; //+ 1 for null terminator
                        break;
                    case 'N': //int64 (up to 18 chars according to dBase spec)
                        col.DataType = typeof(long);
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

            //populate rows
            while (this._memoryStream.Position < this._memoryStream.Length)
            {
                DataRow row = table.NewRow();
                table.Rows.Add(row);
                

                byte recordDivider = (byte) this._memoryStream.ReadByte();
                if (recordDivider != 0x20)
                {
                    //check EOF first
                    if (recordDivider == 0x1a || this._memoryStream.Position < this._memoryStream.Length)
                        break;
                    else
                        throw new Exception(string.Format("Record divider is not 0x20! Byte is {0}.", recordDivider.ToString()));
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
                            if(fd.FieldName == "BITMAPPTR")
                            {
                                int val = BitConverter.ToInt32(bytes, 0);
                                row[i] = bytes==null ? "0" : val.ToString();
                            }
                            else
                                row[i] = Encoding.ASCII.GetString(bytes);
                            break;
                        case 'N':
                            string str = Encoding.ASCII.GetString(bytes);
                            int conv = Convert.ToInt32(str);// BitConverter.ToInt16(val, 1);
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

                    if(i != 0 && i % FieldDescriptorArray.Count == 0) //only after getting all columns, so at the end of each row
                    {
                        recordDivider = (byte) this._memoryStream.ReadByte();
                        if (recordDivider != 0x20)
                            throw new Exception(string.Format("Record divider is not 0x20! Byte is {0}.", recordDivider.ToString()));
                    }
                }
            }

            table.AcceptChanges();
            return table;
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

            while (true)
            {
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
                if (check == 0xD)
                    break;
                else
                    _memoryStream.Position--;
            }
        }
    }
}
