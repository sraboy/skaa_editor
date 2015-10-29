using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public struct ResIndex
    {
        //Lifted from ORESX.h, except string vs char[9]
        public string name;
        public int offset;
    };

    /// <summary>
    /// [DEPRECATED] Sprite can now properly convert BITMAPPTR in <see cref="Sprite.MatchFrameOffsets()"/>.
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
                public string FieldName;
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
            public ResIndex ResourceIndex;
            public int DataSize;

            public DbfFile(MemoryStream rawData, ResIndex ridx, int dataSize)
            {
                this._memoryStream = rawData;

                this.Header = new DbfPreHeader();
                this.FieldDescriptorArray = new List<FieldDescriptor>();
                this.ResourceIndex = ridx;
                this.DataSize = dataSize;

                this.Header.LastEdited = new byte[3];
                this.Header.ReservedOne = new byte[2];
                this.Header.FreeRecordThread = new byte[4];
                this.Header.ReservedMultiUser = new byte[8];
                this.Header.ReservedTwo = new byte[2];

                ReadPreHeader();
                ReadFieldDescriptors();
            }
            public DataTable GetDataTable()
            {
                DataTable table = new DataTable(this.ResourceIndex.name);

                foreach (FieldDescriptor fd in this.FieldDescriptorArray)
                {
                    DataColumn col = new DataColumn(fd.FieldName);
                    col.MaxLength = fd.FieldLength;

                    //http://www.clicketyclick.dk/databases/xbase/format/data_types.html
                    switch (fd.FieldType)
                    {
                        case 'C': //string < 254 chars
                            col.DataType = typeof(string);
                            break;
                        case 'N': //int64 (18 chars)
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
                }

                DataRow row = table.NewRow();
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
                long read = this._memoryStream.Position - 1;

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
                    read += 32; //size of FieldDescriptor record

                    _memoryStream.Seek(read, SeekOrigin.Current);
                    byte check = (byte) _memoryStream.ReadByte();
                    if (check == 0xD)
                        return;
                    else
                        _memoryStream.Seek((read * -1) - 1, SeekOrigin.Current);
                }
            }
        }
}
