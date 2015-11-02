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
        public GameSetFile SetFile;
        private DataSet _databases;
        public DataSet SetDatabase
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
            this.SetFile = new GameSetFile(filepath);
            this.SetFile.Open();
            this.SetDatabase = this.SetFile.GetDataSet();
            //this.Databases = new DataSet(this.SetFile.FileName);
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
            DataTable sframeTable = this.SetDatabase.Tables["SFRAME"];
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

        //        private void FillTable(DataTable dt, OleDbCommand cmd, OleDbConnection conn)
        //        {
        //            // Execute the SELECT command and gets a reader
        //            OleDbDataReader dr = cmd.ExecuteReader();
        //            dt.Columns[9].MaxLength = -1;
        //            dt.Columns[9].DataType = typeof(uint);
        //            dt.AcceptChanges();

        //            while (dr.Read())
        //            {


        //                object sprite = dr.IsDBNull(0) ? (object) DBNull.Value : dr.GetString(0);
        ////#if DEBUG
        ////                if (sprite.ToString() == "BALLISTA")
        ////                    Debugger.Break();
        ////#endif
        //                object action = dr.IsDBNull(1) ? (object) DBNull.Value : dr.GetString(1);
        //                object dir = dr.IsDBNull(2) ? (object) DBNull.Value : dr.GetString(2);
        //                object frame = dr.IsDBNull(3) ? (object) DBNull.Value : Convert.ToInt16(dr.GetString(3));
        //                object offset_x = dr.IsDBNull(4) ? (object) DBNull.Value : Convert.ToDouble(dr.GetValue(4));
        //                object offset_y = dr.IsDBNull(5) ? (object) DBNull.Value : Convert.ToDouble(dr.GetValue(5));
        //                object width = dr.IsDBNull(6) ? (object) DBNull.Value : Convert.ToInt32(dr.GetValue(6));
        //                object height = dr.IsDBNull(7) ? (object) DBNull.Value : Convert.ToInt32(dr.GetValue(7));
        //                object filename = dr.IsDBNull(8) ? (object) DBNull.Value : dr.GetString(8);
        ////#if DEBUG
        ////                if (sprite.ToString() == "BALLISTA" && filename.ToString() == "WB-M4-2")
        ////                    Debugger.Break();
        ////#endif
        //                //dr.GetBytes() has invalid cast errors all the time. GetChars() works fine for some reason.

        //                //object bitmapptr = dr.IsDBNull(9) ? (object) DBNull.Value : new Func<uint> (() => { byte[] bytes = new byte[4]; dr.GetBytes(9, 0, bytes, 0, 4); return Convert.ToUInt32(bytes); })();
        //                Func<byte[]> ConvertCharsToBytes = () => { char[] chars = new char[4]; byte[] bytes = new byte[4]; dr.GetChars(9, 0, chars, 0, 4); return Encoding.GetEncoding(437).GetBytes(chars);};
        //                object bitmapptr = dr.IsDBNull(9) ? 0 : BitConverter.ToUInt32(ConvertCharsToBytes(), 0);

        //                //MemoryStream stream = dr.GetStream(0) as MemoryStream;


        //                DataRow row = dt.NewRow();
        //                row.BeginEdit();

        //                row[0] = sprite;
        //                row[1] = action;
        //                row[2] = dir;
        //                row[3] = frame;
        //                row[4] = offset_x;
        //                row[5] = offset_y;
        //                //if (width == null)
        //                //    row[6] = DBNull.Value;

        //                row[6] = width;// ?? (object)DBNull.Value;
        //                row[7] = height;
        //                row[8] = filename;
        //                row[9] = bitmapptr;//Convert.ToUInt32(bitmapptr.ToString());

        //                dt.Rows.Add(row);
        //                row.AcceptChanges();
        //            }
        //        }

        public DataView GetSpriteDataView(string spriteId)
        {
            DataView dv = new DataView(this.SetDatabase.Tables["SFRAME"]);// , "BALLISTA",)
            dv.RowFilter = string.Format("SPRITE = '{0}'", spriteId);
            return dv;
        }
        //public void MergeDataTableChanges(Sprite spriteToUpdate, string tableName)//string filepath, DataTable tableToMerge)
        //{
        //    DataTable newDataTable;
        //    DataTable tableToMerge = spriteToUpdate.GameSetDataTable;//dr.Table;
        //    //newDataTable = this.Databases.Tables[tableToMerge.TableName];
        //    newDataTable = this.SetDatabase.Tables[tableName];
        //    newDataTable.Merge(tableToMerge, true, MissingSchemaAction.Add);
        //}

        public void BuildNewGameSet()
        {
            //GameSetFile file = new GameSetFile(this.SetFile.Directory + '\\' + "newSet.set");
            this.SetFile.SaveGameSetToFile(this.SetFile.Directory + '\\' + "newSet.set");
        }

    }//end GameSet
}
