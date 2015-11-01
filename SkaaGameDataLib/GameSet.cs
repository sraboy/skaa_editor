﻿/****************************************************************************
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
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
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
        public MemoryStream RawDataStream;
        public string FileName;

        public GameSet(string filepath)
        {
            this._workingPath = Path.GetDirectoryName(filepath);
            this.FileName = Path.GetFileName(filepath);
            //string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //    filepath + ";Extended Properties=dBase III";
        
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                this._rawData = new byte[fs.Length];
                fs.Read(this._rawData, 0, this._rawData.Length);
                //todo: fix this cheap hack
                this.RawDataStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this.RawDataStream);
                this.RawDataStream.Position = 0;
            }

            List < ResIndex > databaseRows = new List<ResIndex>(_recordCount);
            this.Databases = new DataSet("skaa_dbs");

            ReadSetFileToDataSet();
        }
        //public Stream GetRawDataStream()
        //{
        //    //todo: fix this cheap hack
        //    this.RawDataStream.Position = 0;
        //    return this.RawDataStream;
        //}

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
            DataSet allSpritesSet = new DataSet("sprites");

            foreach (DataRow r in sframeTable.Rows)
            {
                //string name = Encoding.ASCII.GetString((byte[])r[0]);
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
                    //spriteNames.Add(r[0].ToString());
                    tbl.ImportRow(r);
                    allSpritesSet.Tables.Add(tbl);
                }
            }

            return allSpritesSet;
        }
        #region old DbfFile Reader
        //private void ReadSetFileToDataSet()
        //{
        //    List<ResIndex> dataRows = GetGameSetRows();

        //    for (int i = 0; i < dataRows.Count; i++)
        //    {
        //        ResIndex r = dataRows[i];
        //        int dataSize;

        //        if (i + 1 >= dataRows.Count) //last one
        //            dataSize = this._rawData.Length - r.offset;
        //        else
        //            dataSize = dataRows[i + 1].offset - r.offset;

        //        //byte[] sframeData;
        //        //sframeData = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
        //        //string tempFile = r.name + ".dbf";
        //        //using (FileStream wfs = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Create))
        //        //    wfs.Write(sframeData, 0, dataSize);

        //        DbfFile file = new DbfFile(this._rawDataStream, r, dataSize);
        //        DataTable table = file.GetDataTable();
        //    }
        //}
        #endregion
        private void ReadSetFileToDataSet()
        {
            List<ResIndex> dataRows = GetGameSetRows();

            for (int i = 0; i < dataRows.Count; i++)
            {
                ResIndex r = dataRows[i];
                int dataSize;

                if (i + 1 >= dataRows.Count) //last row, just use the DBF's length (as if it were a separate file) 
                    dataSize = this._rawData.Length - r.offset;
                else
                    dataSize = dataRows[i + 1].offset - r.offset;

                byte[] sframeData = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
                string tempFile = r.name + ".dbf";

                using (FileStream wfs = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Create))
                    wfs.Write(sframeData, 0, dataSize);

                string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    this._workingPath + "\\dbf" + ";Extended Properties=dBase III";

                using (OleDbConnection dbConnex = new OleDbConnection(connex))
                {
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + tempFile + ']', dbConnex);
                    #region old sframe
                    //if (r.name == "SFRAME") //special reading of this file to read BITMAPPTR properly
                    //{
                    //    /* We don't use an OleDbDataAdapter here because the adapter
                    //     * gives us no control over the data; it's read-in en masse.
                    //     * Unfortunately, the Jet DB engine does not allow SQL CONVERT()
                    //     * nor are we able to just convert it after because 
                    //     */

                    //    dbfFile.Open();
                    //    OleDbDataReader dr = cmd.ExecuteReader();
                    //    DataTable table = new DataTable("SFRAME");

                    //    // SPRITE  ACTION  DIR  FRAME  OFFSET_X  OFFSET_Y  WIDTH  HEIGHT FILENAME  BITMAPPTR
                    //    table.Columns.Add(new DataColumn("SPRITE"));
                    //    table.Columns.Add(new DataColumn("ACTION"));
                    //    table.Columns.Add(new DataColumn("DIR"));
                    //    table.Columns.Add(new DataColumn("FRAME"));
                    //    table.Columns.Add(new DataColumn("OFFSET_X"));
                    //    table.Columns.Add(new DataColumn("OFFSET_Y"));
                    //    table.Columns.Add(new DataColumn("WIDTH"));
                    //    table.Columns.Add(new DataColumn("HEIGHT"));
                    //    table.Columns.Add(new DataColumn("FILENAME"));
                    //    table.Columns.Add(new DataColumn("BITMAPPTR"));

                    //    table.Columns["BITMAPPTR"].DataType = typeof(short);

                    //    while (dr.Read())
                    //    {
                    //        short savePtr = 0;
                    //        var spr = dr[0];
                    //        //if ((string) spr == "PERSIAN")
                    //        //{
                    //        var act = dr[1];
                    //        var dir = dr[2];
                    //        var frame = dr[3];
                    //        var offx = dr[4];
                    //        var offy = dr[5];
                    //        var width = dr[6];
                    //        var height = dr[7];
                    //        var filename = dr[8];
                    //        var bitmapptr = dr[9];
                    //        var type = bitmapptr.GetType();

                    //        byte[] p = new byte[2];
                    //        short ptr;

                    //        if (dr[9].GetType() == typeof(DBNull))
                    //        {
                    //            ptr = 0;
                    //        }
                    //        else
                    //        {
                    //            p = Encoding.Unicode.GetBytes((string) dr[9]);

                    //            if (((string) dr[9]).Length == 1)
                    //            {
                    //                byte[] bytes = Encoding.ASCII.GetBytes((string) dr[9]);
                    //                ptr = (short) bytes[0];

                    //            }
                    //            else
                    //                ptr = BitConverter.ToInt16(p, 0);
                    //        }
                    //        savePtr = ptr;
                    //        //}

                    //        DataRow cv = table.NewRow();
                    //        cv[0] = dr[0];
                    //        cv[1] = dr[1];
                    //        cv[2] = dr[2];
                    //        cv[3] = dr[3];
                    //        cv[4] = dr[4];
                    //        cv[5] = dr[5];
                    //        cv[6] = dr[6];
                    //        cv[7] = dr[7];
                    //        cv[8] = dr[8];
                    //        cv[9] = savePtr;

                    //        table.ImportRow(cv);
                    //    }
                    //    this.Databases.Tables.Add(table);
                    //}
                    //else
                    //{
                    #endregion
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        DataTable table = new DataTable(r.name);
                        dbConnex.Open();

                        adapter.FillSchema(table, SchemaType.Source);
                        if(table.TableName == "SFRAME")
                        {
                            dbConnex.Close();

                            #region old manual reader
                            //using (FileStream fsFixDataType = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Open))
                            //{
                            //    using (MemoryStream ms = new MemoryStream())
                            //    { 
                            //        fsFixDataType.CopyTo(ms);
                            //        using (FileStream fsBackup = new FileStream(this._workingPath + "\\dbf\\" + table.TableName + "_fixed.dbf", FileMode.OpenOrCreate))
                            //        { 
                            //            ms.CopyTo(fsBackup);
                            //        }
                            //        ms.Position = 0x14B;
                            //        char colType = (char) ms.ReadByte();
                            //        if(colType == 'C')
                            //        {
                            //            ms.Position--;
                            //            ms.WriteByte(0x4e); //0x4e = N = Number
                            //        }
                            //        ms.CopyTo(fsFixDataType);
                            //    }
                            //}
                            ////cmd = new OleDbCommand("SELECT * FROM [" + table.TableName + "_fixed.dbf" + ']', dbfFile);
                            //dbConnex.Open();
                            #endregion

                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (FileStream fsSframe = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Open))
                                {
                                    fsSframe.CopyTo(ms);
                                }

                                DbfFile dbfFile = new DbfFile(ms, r.name, (int) ms.Length);
                                table = dbfFile.GetConvertedRawDataTable();
                            }

                            //FillTable(table, cmd, dbConnex);
                        }
                        else
                            adapter.Fill(table);
                        #region old sframe
                        //if (r.name == "SFRAME") //special reading of this file to read BITMAPPTR properly
                        //{
                        //    DataTable conv = table.Clone();
                        //    conv.Columns["BITMAPPTR"].DataType = typeof(ushort);

                        //    foreach (DataRow dr in table.Rows)
                        //    {
                        //        byte[] p = new byte[2];
                        //        ushort ptr;

                        //        if (dr[9].GetType() == typeof(DBNull))
                        //        {
                        //            ptr = 0;
                        //        }
                        //        else
                        //        {
                        //            p = Encoding.UTF8.GetBytes((string) dr[9]);

                        //            //if (((string) dr[9]).Length == 1)
                        //            //{
                        //            //    var t = Encoding.ASCII.GetBytes((string) dr[9]);
                        //            //    Array ar = dr.ItemArray;
                        //            //    //p[1] = 0;
                        //            //}
                        //            if (p.Length > 1)
                        //                ptr = BitConverter.ToUInt16(p, 0);
                        //            else
                        //                ptr = ushort.MaxValue;
                        //        }
                        //        //dr.BeginEdit();
                        //        //dr.SetField("BITMAPPTR", ptr);
                        //        //dr.EndEdit();
                        //        //DataRow cvdr;
                        //        //cvdr.SetField(0, dr[0]);
                        //        //DataTable dt = new DataTable();
                        //        //dt.Columns.Add(new DataColumn("SPRITE"));
                        //        //dt.Columns.Add(new DataColumn("ACTION"));
                        //        //dt.Columns.Add(new DataColumn("DIR"));
                        //        //dt.Columns.Add(new DataColumn("FRAME"));
                        //        //dt.Columns.Add(new DataColumn("OFFSET_X"));
                        //        //dt.Columns.Add(new DataColumn("OFFSET_Y"));
                        //        //dt.Columns.Add(new DataColumn("WIDTH"));
                        //        //dt.Columns.Add(new DataColumn("HEIGHT"));
                        //        //dt.Columns.Add(new DataColumn("FILENAME"));
                        //        //dt.Columns.Add(new DataColumn("BITMAPPTR"));
                        //        //dt.Columns["BITMAPPTR"].DataType = typeof(ushort);

                        //        DataRow cv = conv.NewRow();
                        //        cv[0] = dr[0];
                        //        cv[1] = dr[1];
                        //        cv[2] = dr[2];
                        //        cv[3] = dr[3];
                        //        cv[4] = dr[4];
                        //        cv[5] = dr[5];
                        //        cv[6] = dr[6];
                        //        cv[7] = dr[7];
                        //        cv[8] = dr[8];
                        //        cv[9] = ptr;
                        //        conv.Rows.Add(cv);
                        //        //var dest = conv.NewRow();
                        //        //dest.ItemArray = cv.ItemArray.Clone() as object[];
                        //        //conv.Rows.Add(dest);
                        //    }
                        //
                        //    table = conv.Copy();
                        //}
                        #endregion
                        this.Databases.Tables.Add(table);
                    }
                }
            }
        }
        private void FillTable(DataTable dt, OleDbCommand cmd, OleDbConnection conn)
        {
            // Execute the SELECT command and gets a reader
            OleDbDataReader dr = cmd.ExecuteReader();
            dt.Columns[9].MaxLength = -1;
            dt.Columns[9].DataType = typeof(uint);
            dt.AcceptChanges();

            while (dr.Read())
            {
                
                
                object sprite = dr.IsDBNull(0) ? (object) DBNull.Value : dr.GetString(0);
//#if DEBUG
//                if (sprite.ToString() == "BALLISTA")
//                    Debugger.Break();
//#endif
                object action = dr.IsDBNull(1) ? (object) DBNull.Value : dr.GetString(1);
                object dir = dr.IsDBNull(2) ? (object) DBNull.Value : dr.GetString(2);
                object frame = dr.IsDBNull(3) ? (object) DBNull.Value : Convert.ToInt16(dr.GetString(3));
                object offset_x = dr.IsDBNull(4) ? (object) DBNull.Value : Convert.ToDouble(dr.GetValue(4));
                object offset_y = dr.IsDBNull(5) ? (object) DBNull.Value : Convert.ToDouble(dr.GetValue(5));
                object width = dr.IsDBNull(6) ? (object) DBNull.Value : Convert.ToInt32(dr.GetValue(6));
                object height = dr.IsDBNull(7) ? (object) DBNull.Value : Convert.ToInt32(dr.GetValue(7));
                object filename = dr.IsDBNull(8) ? (object) DBNull.Value : dr.GetString(8);
//#if DEBUG
//                if (sprite.ToString() == "BALLISTA" && filename.ToString() == "WB-M4-2")
//                    Debugger.Break();
//#endif
                //dr.GetBytes() has invalid cast errors all the time. GetChars() works fine for some reason.

                //object bitmapptr = dr.IsDBNull(9) ? (object) DBNull.Value : new Func<uint> (() => { byte[] bytes = new byte[4]; dr.GetBytes(9, 0, bytes, 0, 4); return Convert.ToUInt32(bytes); })();
                Func<byte[]> ConvertCharsToBytes = () => { char[] chars = new char[4]; byte[] bytes = new byte[4]; dr.GetChars(9, 0, chars, 0, 4); return Encoding.GetEncoding(437).GetBytes(chars);};
                object bitmapptr = dr.IsDBNull(9) ? 0 : BitConverter.ToUInt32(ConvertCharsToBytes(), 0);

                //MemoryStream stream = dr.GetStream(0) as MemoryStream;


                DataRow row = dt.NewRow();
                row.BeginEdit();
                
                row[0] = sprite;
                row[1] = action;
                row[2] = dir;
                row[3] = frame;
                row[4] = offset_x;
                row[5] = offset_y;
                //if (width == null)
                //    row[6] = DBNull.Value;

                row[6] = width;// ?? (object)DBNull.Value;
                row[7] = height;
                row[8] = filename;
                row[9] = bitmapptr;//Convert.ToUInt32(bitmapptr.ToString());
                
                dt.Rows.Add(row);
                row.AcceptChanges();
            }
        }
        /// <summary>
        /// Reads all the <see cref="ResIndex"/> rows from <see cref="_rawData"/>, a .set file.
        /// </summary>
        /// <returns>A List of <see cref="ResIndex"/> objects</returns>
        private List<ResIndex> GetGameSetRows()
        {
            List<ResIndex> dataRows = new List<ResIndex>();
            byte[] recCount = new byte[2];
            this.RawDataStream.Read(recCount, 0, 2);
            this._recordCount = BitConverter.ToInt16(recCount, 0);
            //this._recordCount = (short) (this._rawDataStream.ReadByte() + (this._rawDataStream.ReadByte() * 256));
                
            //need (this._recordCount + 1) because there's always an extra (empty) row/record at the end of the header
            while (this.RawDataStream.Position < (this._recordCount + 1) * _resIndexSize)
            {
                ResIndex row;
                byte[] name = new byte[_rowNameSize];
                byte[] offset = new byte[sizeof(int)];

                this.RawDataStream.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                this.RawDataStream.Read(offset, 0, sizeof(int));

                row.name = Encoding.UTF8.GetString(name).Trim('\0');
                row.offset = BitConverter.ToInt32(offset, 0);

                if (row.name != "")
                    dataRows.Add(row);
                else
                    break;
            }//end while
            //}//end using MemoryStream

            return dataRows;
        }

        public void MergeDataTableChanges(DataRow dr)//string filepath, DataTable tableToMerge)
        {
            DataTable newDataTable;
            DataTable tableToMerge = dr.Table;

            newDataTable = this.Databases.Tables[tableToMerge.TableName];
            newDataTable.Merge(tableToMerge, true, MissingSchemaAction.Add);

            //this.Databases.Tables[tableToMerge.TableName].Merge(tableToMerge, true, MissingSchemaAction.Add);
        }

    }//end GameSet
}
