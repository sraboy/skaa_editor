/****************************************************************************
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
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
****************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public class GameSet
    {
        private const int _rowNameSize = 9;
        private const int _resIndexSize = 13;
        private short _recordCount;
        private byte[] _rawData;
        //private List<ResIndex> _databaseRows;
        private MemoryStream _rawDataStream;
        private DataSet _databases;
        private string _workingPath;

        public DataSet Databases
        {
            get
            {
                return this._databases;
            }
            set
            {
                if (this._databases != value)
                { 
                    this._databases = value;
                }
            }
        }

        public GameSet(string filepath)
        {
            this._workingPath = Path.GetDirectoryName(filepath);

            //string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //    filepath + ";Extended Properties=dBase III";
        
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                this._rawData = new byte[fs.Length];
                fs.Read(this._rawData, 0, this._rawData.Length);
                //todo: fix this cheap hack
                this._rawDataStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this._rawDataStream);
                this._rawDataStream.Position = 0;
            }

            List < ResIndex > databaseRows = new List<ResIndex>(_recordCount);
            this.Databases = new DataSet("skaa_dbs");

            ReadSetFileToDataSet();
        }
        public Stream GetRawDataStream()
        {
            //todo: fix this cheap hack
            return this._rawDataStream;
        }
        /// <summary>
        /// Breaks up the SFRAME DataTable into individual DataTables, one for each sprite in the SFRAME DataTable
        /// </summary>
        /// <returns>
        /// A DataSet object, named "sprites", containing a separate table for each sprite in 
        /// the SFRAME DataTable, each named by the SpriteId from the original dBase object
        /// </returns>
        public DataSet GetSpriteTablesInDataSet()
        {
            DataTable sframeTable = this.Databases.Tables["SFRAME"];
            List<string> spriteNames = new List<string>();
            DataSet allSpritesSet = new DataSet("sprites");

            foreach (DataRow r in sframeTable.Rows)
            {
                DataTable curTable = allSpritesSet.Tables[r[0].ToString()];
                DataTable tbl;

                if (curTable != null)
                {
                    tbl = curTable;
                    tbl.ImportRow(r);
                }
                else
                {
                    tbl = sframeTable.Clone();
                    tbl.TableName = r[0].ToString();
                    spriteNames.Add(r[0].ToString());
                    tbl.ImportRow(r);
                    allSpritesSet.Tables.Add(tbl);
                }

                //DataColumn bitmapPtr = tbl.Columns[tbl.Columns.Count - 1]; //new DataColumn();
                //bitmapPtr.DataType = typeof(ushort);
                //bitmapPtr = tbl.Columns[tbl.Columns.Count - 1];
                //tbl.PrimaryKey = new DataColumn[] { bitmapPtr };
            }
            
            return allSpritesSet;
        }

        private void ReadSetFileToDataSet()
        {
            List<ResIndex> dataRows = GetGameSetRows();

            for (int i = 0; i < dataRows.Count; i++)
            {
                ResIndex r = dataRows[i];

                int dataSize;

                if (i + 1 >= dataRows.Count) //last one
                    dataSize = this._rawData.Length - r.offset;
                else
                    dataSize = dataRows[i + 1].offset - r.offset;


                //byte[] sframeData;
                //sframeData = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
                //string tempFile = r.name + ".dbf";

                //using (FileStream wfs = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Create))
                //    wfs.Write(sframeData, 0, dataSize);

                DbfFile file = new DbfFile(this._rawDataStream, r, dataSize);

                DataTable table = file.GetDataTable();

            }
        }

        //private void ReadSetFileToDataSet()
        //{
        //    //todo: abstract this out so it can build any named row
        //    List<ResIndex> dataRows = GetGameSetRows();

        //    for (int i = 0; i < dataRows.Count; i++)
        //    {
        //        ResIndex r = dataRows[i];
        //        //ResIndex row = dataRows.Find(r => r.name == "SFRAME");
        //        //int next_idx = dataRows.FindIndex(i => i.name == r.name) + 1;
        //        int dataSize;

        //        if (i + 1 >= dataRows.Count) //last one
        //            dataSize = this._rawData.Length - r.offset;
        //        else
        //            dataSize = dataRows[i + 1].offset - r.offset;


        //        byte[] sframeData;
        //        sframeData = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
        //        string tempFile = r.name + ".dbf";

        //        using (FileStream wfs = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Create))
        //            wfs.Write(sframeData, 0, dataSize);

        //        string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
        //            this._workingPath + "\\dbf" + ";Extended Properties=dBase III";

        //        using (OleDbConnection dbfFile = new OleDbConnection(connex))
        //        {
        //            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + tempFile + ']', dbfFile);

        //            #region sframe
        //            //if (r.name == "SFRAME") //special reading of this file to read BITMAPPTR properly
        //            //{
        //            //    /* We don't use an OleDbDataAdapter here because the adapter
        //            //     * gives us no control over the data; it's read-in en masse.
        //            //     * Unfortunately, the Jet DB engine does not allow SQL CONVERT()
        //            //     * nor are we able to just convert it after because 
        //            //     */

        //            //    dbfFile.Open();
        //            //    OleDbDataReader dr = cmd.ExecuteReader();
        //            //    DataTable table = new DataTable("SFRAME");

        //            //    // SPRITE  ACTION  DIR  FRAME  OFFSET_X  OFFSET_Y  WIDTH  HEIGHT FILENAME  BITMAPPTR
        //            //    table.Columns.Add(new DataColumn("SPRITE"));
        //            //    table.Columns.Add(new DataColumn("ACTION"));
        //            //    table.Columns.Add(new DataColumn("DIR"));
        //            //    table.Columns.Add(new DataColumn("FRAME"));
        //            //    table.Columns.Add(new DataColumn("OFFSET_X"));
        //            //    table.Columns.Add(new DataColumn("OFFSET_Y"));
        //            //    table.Columns.Add(new DataColumn("WIDTH"));
        //            //    table.Columns.Add(new DataColumn("HEIGHT"));
        //            //    table.Columns.Add(new DataColumn("FILENAME"));
        //            //    table.Columns.Add(new DataColumn("BITMAPPTR"));

        //            //    table.Columns["BITMAPPTR"].DataType = typeof(short);

        //            //    while (dr.Read())
        //            //    {
        //            //        short savePtr = 0;
        //            //        var spr = dr[0];
        //            //        //if ((string) spr == "PERSIAN")
        //            //        //{
        //            //        var act = dr[1];
        //            //        var dir = dr[2];
        //            //        var frame = dr[3];
        //            //        var offx = dr[4];
        //            //        var offy = dr[5];
        //            //        var width = dr[6];
        //            //        var height = dr[7];
        //            //        var filename = dr[8];
        //            //        var bitmapptr = dr[9];
        //            //        var type = bitmapptr.GetType();

        //            //        byte[] p = new byte[2];
        //            //        short ptr;

        //            //        if (dr[9].GetType() == typeof(DBNull))
        //            //        {
        //            //            ptr = 0;
        //            //        }
        //            //        else
        //            //        {
        //            //            p = Encoding.Unicode.GetBytes((string) dr[9]);

        //            //            if (((string) dr[9]).Length == 1)
        //            //            {
        //            //                byte[] bytes = Encoding.ASCII.GetBytes((string) dr[9]);
        //            //                ptr = (short) bytes[0];

        //            //            }
        //            //            else
        //            //                ptr = BitConverter.ToInt16(p, 0);
        //            //        }
        //            //        savePtr = ptr;
        //            //        //}

        //            //        DataRow cv = table.NewRow();
        //            //        cv[0] = dr[0];
        //            //        cv[1] = dr[1];
        //            //        cv[2] = dr[2];
        //            //        cv[3] = dr[3];
        //            //        cv[4] = dr[4];
        //            //        cv[5] = dr[5];
        //            //        cv[6] = dr[6];
        //            //        cv[7] = dr[7];
        //            //        cv[8] = dr[8];
        //            //        cv[9] = savePtr;

        //            //        table.ImportRow(cv);
        //            //    }
        //            //    this.Databases.Tables.Add(table);
        //            //}
        //            //else
        //            //{
        //            #endregion
        //            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
        //            { 
        //                DataTable table = new DataTable(r.name);
        //                dbfFile.Open();
        //                adapter.Fill(table);

        //                #region sframe
        //                if (r.name == "SFRAME") //special reading of this file to read BITMAPPTR properly
        //                {
        //                    DataTable conv = table.Clone();
        //                    conv.Columns["BITMAPPTR"].DataType = typeof(ushort);

        //                    foreach (DataRow dr in table.Rows)
        //                    {
        //                        byte[] p = new byte[2];
        //                        ushort ptr;

        //                        if (dr[9].GetType() == typeof(DBNull))
        //                        {
        //                            ptr = 0;
        //                        }
        //                        else
        //                        {
        //                            p = Encoding.UTF8.GetBytes((string) dr[9]);

        //                            //if (((string) dr[9]).Length == 1)
        //                            //{
        //                            //    var t = Encoding.ASCII.GetBytes((string) dr[9]);
        //                            //    Array ar = dr.ItemArray;
        //                            //    //p[1] = 0;
        //                            //}
        //                            if (p.Length > 1)
        //                                ptr = BitConverter.ToUInt16(p, 0);
        //                            else
        //                                ptr = ushort.MaxValue;
        //                        }
        //                        //dr.BeginEdit();
        //                        //dr.SetField("BITMAPPTR", ptr);
        //                        //dr.EndEdit();
        //                        //DataRow cvdr;
        //                        //cvdr.SetField(0, dr[0]);
        //                        //DataTable dt = new DataTable();
        //                        //dt.Columns.Add(new DataColumn("SPRITE"));
        //                        //dt.Columns.Add(new DataColumn("ACTION"));
        //                        //dt.Columns.Add(new DataColumn("DIR"));
        //                        //dt.Columns.Add(new DataColumn("FRAME"));
        //                        //dt.Columns.Add(new DataColumn("OFFSET_X"));
        //                        //dt.Columns.Add(new DataColumn("OFFSET_Y"));
        //                        //dt.Columns.Add(new DataColumn("WIDTH"));
        //                        //dt.Columns.Add(new DataColumn("HEIGHT"));
        //                        //dt.Columns.Add(new DataColumn("FILENAME"));
        //                        //dt.Columns.Add(new DataColumn("BITMAPPTR"));
        //                        //dt.Columns["BITMAPPTR"].DataType = typeof(ushort);

        //                        DataRow cv = conv.NewRow();
        //                        cv[0] = dr[0];
        //                        cv[1] = dr[1];
        //                        cv[2] = dr[2];
        //                        cv[3] = dr[3];
        //                        cv[4] = dr[4];
        //                        cv[5] = dr[5];
        //                        cv[6] = dr[6];
        //                        cv[7] = dr[7];
        //                        cv[8] = dr[8];
        //                        cv[9] = ptr;
        //                        conv.Rows.Add(cv);
        //                        //var dest = conv.NewRow();
        //                        //dest.ItemArray = cv.ItemArray.Clone() as object[];
        //                        //conv.Rows.Add(dest);
        //                    }

        //                    table = conv.Copy();
        //                }
        //                #endregion

        //                this.Databases.Tables.Add(table);
        //                //}
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Reads all the <see cref="ResIndex"/> rows from <see cref="_rawData"/>, a .set file.
        /// </summary>
        /// <returns>A List of <see cref="ResIndex"/> objects</returns>
        private List<ResIndex> GetGameSetRows()
        {
            List<ResIndex> dataRows = new List<ResIndex>();
            //using (MemoryStream ms = new MemoryStream(this._rawData))
            //{
            this._recordCount = (short) (this._rawDataStream.ReadByte() + (this._rawDataStream.ReadByte() * 256));
                
            //+1 because there's an empty row at the end of the header
            while (this._rawDataStream.Position < (this._recordCount + 1) * _resIndexSize)
            {
                ResIndex row;
                byte[] name = new byte[_rowNameSize];
                byte[] offset = new byte[sizeof(int)];

                this._rawDataStream.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                this._rawDataStream.Read(offset, 0, sizeof(int));

                row.name = Encoding.UTF8.GetString(name).Trim('\0');
                row.offset = BitConverter.ToInt32(offset, 0);

                if (row.name != "")
                    dataRows.Add(row);
                else
                    break;
            }//end while
            //}//end using MemoryStream

            return dataRows;
        }//end ReadRawData()

    }//end GameSet

    [Serializable]
    public struct ResIndex
    {
        //Lifted from ORESX.h, except string vs char[9]
        public string name;
        public int offset;
    };

    [Serializable]
    public class DbfFile
    {
        //constructed based on:
        // http://www.clicketyclick.dk/databases/xbase/format/dbf.html
        // http://www.digitalpreservation.gov/formats/fdd/fdd000325.shtml

        public struct DbfHeader
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

        public DbfHeader Header;
        public List<FieldDescriptor> FieldDescriptorArray;
        public ResIndex ResourceIndex;
        public int DataSize;

        //private byte[] _rawData;
        private MemoryStream _memoryStream;

        public DbfFile(MemoryStream rawData, ResIndex ridx, int dataSize)
        {
            //this._rawData = new byte[rawData.Length];
            //this._rawData = rawData;

            this._memoryStream = rawData;// new MemoryStream(this._rawData);

            this.Header = new DbfHeader();
            this.FieldDescriptorArray = new List<FieldDescriptor>();
            this.ResourceIndex = ridx;
            this.DataSize = dataSize;

            this.Header.LastEdited = new byte[3];
            this.Header.ReservedOne = new byte[2];
            this.Header.FreeRecordThread = new byte[4];
            this.Header.ReservedMultiUser = new byte[8];
            this.Header.ReservedTwo = new byte[2];

            ReadHeader();
            ReadFieldDescriptors();
        }

        private void ReadHeader()
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
            //int setFileHeaderLen = 0x1e3;
            

            long read = this._memoryStream.Position - 1;
            //long max = read + this.Header.LengthOfHeader;// + setFileHeaderLen;

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
            
            //if (this._memoryStream.ReadByte() == 0x0D)
            //    return;
        }

        public DataTable GetDataTable()
        {
            DataTable table = new DataTable(this.ResourceIndex.name);

            foreach(FieldDescriptor fd in this.FieldDescriptorArray)
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
    }
}
