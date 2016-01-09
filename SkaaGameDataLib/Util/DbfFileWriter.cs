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
using System.IO;
using System.Text;

namespace SkaaGameDataLib.Util
{
    public static class DbfFileWriter
    {
        /// <summary>
        /// Saves this <see cref="DataTable"/> to the specified stream in the DBF/dBaseIII format
        /// </summary>
        /// <param name="dt">The DataTable to save</param>
        /// <param name="str">The stream to which to write</param>
        public static void Save(this DataTable dt, Stream str)
        {
            DbfFile file = new DbfFile(dt);
            GetFieldDescriptorListFromSchema(file); //need to know how many we have to calculate below, before writing the header
            file.Header.LengthOfHeader += (short)(file.FieldDescriptors.Count * DbfFile.FieldDescriptor.Size);
            file.Header.LengthOfRecord = 1; //1 byte for RowState (0x20 or 0x2a)
            foreach (DataColumn col in dt.Columns)
                file.Header.LengthOfRecord += col.GetByteLength();

            WriteHeader(file, str);
            WriteFieldDescriptors(file, str);
            WriteDataTableToStream(file, str);
        }
        /// <summary>
        /// Saves this <see cref="DataTable"/> as the specified file in the DBF/dBaseIII format
        /// </summary>
        /// <param name="dt">The DataTable to save</param>
        /// <param name="filepath">The fully-qualified path of the file to write to</param>
        public static void Save(this DataTable dt, string filepath)
        {
            DbfFile file = new DbfFile(dt);

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                WriteHeader(file, fs);
                WriteFieldDescriptors(file, fs);
                WriteDataTableToStream(file, fs);
            }
        }
        private static void WriteHeader(DbfFile file, Stream str)
        {
            str.WriteByte(file.Header.Version);
            str.Write(file.Header.LastEdited, 0, 3);
            str.Write(BitConverter.GetBytes(file.Header.NumberOfRecords), 0, 4);
            str.Write(BitConverter.GetBytes(file.Header.LengthOfHeader), 0, 2);
            str.Write(BitConverter.GetBytes(file.Header.LengthOfRecord), 0, 2);
            str.Write(file.Header.ReservedOne, 0, 2);
            str.WriteByte(file.Header.IncompleteTransaction);
            str.WriteByte(file.Header.EncryptionFlag);
            str.Write(file.Header.FreeRecordThread, 0, 4);
            str.Write(file.Header.ReservedMultiUser, 0, 8);
            str.WriteByte(file.Header.MdxFlag);
            str.WriteByte(file.Header.Language);
            str.Write(file.Header.ReservedTwo, 0, 2);
        }
        private static void GetFieldDescriptorListFromSchema(DbfFile file)
        {
            //http://www.clicketyclick.dk/databases/xbase/format/data_types.html

            file.FieldDescriptors = new List<DbfFile.FieldDescriptor>();

            int fieldAddr = 1; //first is always 1

            foreach (DataColumn col in file.DataTable.Columns)
            {
                DbfFile.FieldDescriptor fd = col.GetFieldDescriptor();
                fd.FieldDataAddress = BitConverter.GetBytes(fieldAddr);
                //FieldDataAddress is the index/offset of this field's data in a record, if the records were byte arrays
                file.FieldDescriptors.Add(fd);
                fieldAddr += fd.FieldLength; //sum each previous column's ByteLength, which equals the FieldLength
            }
        }
        private static void WriteFieldDescriptors(DbfFile file, Stream str)
        {
            foreach (DbfFile.FieldDescriptor fd in file.FieldDescriptors)
            {
                StringBuilder sb = new StringBuilder(fd.FieldName);
                sb.Append((char)0x0, 11 - fd.FieldName.Length);
                string writeme = sb.ToString();
                str.Write(Encoding.UTF8.GetBytes(writeme), 0, 11);

                str.WriteByte((byte)fd.FieldType);
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
        private static void WriteDataTableToStream(DbfFile file, Stream str)
        {
            foreach (DataRow dr in file.DataTable.Rows)
            {
                str.WriteByte(DbfFile.RecordValidMarker);

                foreach (DataColumn col in file.DataTable.Columns)
                {
                    string value;

                    if (col.DataType == typeof(string))
                    {
                        byte[] bytes = new byte[col.GetByteLength()];
                        value = dr[col] == DBNull.Value ? " " : (string)dr[col];

                        if (col.ColumnName.EndsWith("PTR")) //actually a number (C pointer), not a string
                        {
                            int val = value == "0" ? Convert.ToInt32(0) : Convert.ToInt32(value);
                            bytes = BitConverter.GetBytes(val);
                        }
                        else
                        {
                            value = dr[col] == DBNull.Value ? string.Empty : (string)dr[col];
                            value = value.PadRight(col.GetByteLength(), ' ');
                            bytes = Encoding.GetEncoding(1252).GetBytes(value);
                        }
                        str.Write(bytes, 0, col.GetByteLength());
                    }
                    else if (col.DataType == typeof(long))
                    {
                        long val = dr[col] == DBNull.Value ? 0 : (long)dr[col];
                        value = val.ToString();
                        value = value.PadLeft(col.GetByteLength(), ' ');
                        byte[] bytes = new byte[col.GetByteLength()];
                        Encoding.GetEncoding(1252).GetBytes(value, 0, col.GetByteLength(), bytes, 0);
                        str.Write(bytes, 0, col.GetByteLength());
                    }
                    else
                    {
                        //todo: handle other DbfDataColumn types
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

            str.WriteByte(DbfFile.EofMarker);
        }
    }
}
