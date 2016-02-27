﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public class GameSet
    {
        [Serializable]
        private struct ResIndex
        {
            //Lifted from ORESX.h, except string vs char[9]
            internal string name;
            internal int offset;
        };
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

        //dBase III header = 03 62 03 12 58 23 00 00 61 01 29 (??)
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

                if (curTable != null)
                {
                    DataTable tbl = curTable;
                    tbl.ImportRow(r);
                }
                else
                {
                    DataTable tbl = sframeTable.Clone();
                    tbl.TableName = r[0].ToString();
                    spriteNames.Add(r[0].ToString());
                    tbl.ImportRow(r);
                    allSpritesSet.Tables.Add(tbl);
                }
            }
            
            return allSpritesSet;
        }

        private void ReadSetFileToDataSet()
        {
            //todo: abstract this out so it can build any named row
            List<ResIndex> dataRows = GetGameSetRows();

            for(int i = 0; i < dataRows.Count; i++)
            {
                ResIndex r = dataRows[i];
                //ResIndex row = dataRows.Find(r => r.name == "SFRAME");
                //int next_idx = dataRows.FindIndex(i => i.name == r.name) + 1;
                int dataSize;

                if (i + 1 >= dataRows.Count) //last one
                    dataSize = this._rawData.Length - r.offset;
                else
                    dataSize = dataRows[i + 1].offset - r.offset;


                byte[] sframeData;
                sframeData = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
                string tempFile = r.name + ".dbf";

                using (FileStream wfs = new FileStream(this._workingPath + "\\dbf\\" + tempFile, FileMode.Create))
                    wfs.Write(sframeData, 0, dataSize);

                string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    this._workingPath + "\\dbf" + ";Extended Properties=dBase III";

                using (OleDbConnection dbfFile = new OleDbConnection(connex))
                { 
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + tempFile + ']', dbfFile);
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    { 
                        DataTable table = new DataTable(r.name);
                        dbfFile.Open();
                        adapter.Fill(table);
                        this.Databases.Tables.Add(table);
                    }
                }
            }
        }
        /// <summary>
        /// Reads all the <see cref="ResIndex"/> rows from <see cref="_rawData"/>, a .set file.
        /// </summary>
        /// <returns>A List of <see cref="ResIndex"/> objects</returns>
        private List<ResIndex> GetGameSetRows()
        {
            List<ResIndex> dataRows = new List<ResIndex>();
            using (MemoryStream ms = new MemoryStream(this._rawData))
            {
                this._recordCount = (short) (ms.ReadByte() + (ms.ReadByte() * 256));
                //+1 because there's an empty row at the end of the header
                while (ms.Position < (this._recordCount + 1) * _resIndexSize)
                {
                    ResIndex row;
                    byte[] name = new byte[_rowNameSize];
                    byte[] offset = new byte[sizeof(int)];

                    ms.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                    ms.Read(offset, 0, sizeof(int));

                    row.name = Encoding.UTF8.GetString(name).Trim('\0');
                    row.offset = BitConverter.ToInt32(offset, 0);

                    if (row.name != "")
                        dataRows.Add(row);
                    else
                        break;

                }//end while
            }//end using MemoryStream

            return dataRows;
        }//end ReadRawData()

    }//end GameSet
}
