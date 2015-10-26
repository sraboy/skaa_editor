using System;
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
            string filename;

            // If a set is chosen by the user, we'll get a full file path. The 'connex' string below can't have
            // a file name, just a path. This is because the path is considered the 'database' and the file is
            // a 'table' as far as OLEDB/Jet is concerned.
            FileAttributes attr = File.GetAttributes(filepath);
            if (attr.HasFlag(FileAttributes.Directory))
                filename = "std.set";
            else
            {
                filename = Path.GetFileName(filepath);
                filepath = Path.GetDirectoryName(filepath);
            }

            this._workingPath = filepath;

            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                filepath + ";Extended Properties=dBase III";

            
            using (FileStream fs = new FileStream(filepath + '\\' + filename, FileMode.Open))
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
            
            //todo: save the List<ResIndex> from GetGameSetRows() to parse the other tables
            BuildSFRAMEDataSet(GetGameSetRows());
        }
        public Stream GetRawDataStream()
        {
            //todo: fix this cheap hack
            return this._rawDataStream;
        }
        public DataSet GetSpritesDataSet()
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
        private void BuildSFRAMEDataSet(List<ResIndex> dataRows)
        {
            ResIndex row = dataRows.Find(r => r.name == "SFRAME");
            int idx = dataRows.FindIndex(r => r.name == "SFRAME");
            int dataSize = dataRows[idx + 1].offset - row.offset;
            byte[] sframeData;   

            sframeData = new ArraySegment<byte>(this._rawData, row.offset, dataSize).ToArray();

            string tempFile = "sframe.dbf";

            using (FileStream wfs = new FileStream(tempFile, FileMode.Create))
            {
                wfs.Write(sframeData, 0, dataSize);
            }

            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    this._workingPath + ";Extended Properties=dBase III";

            OleDbConnection sframeDBF = new OleDbConnection(connex);
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + tempFile + ']', sframeDBF);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable table = new DataTable("SFRAME");

            sframeDBF.Open();
            adapter.Fill(table);

            this.Databases.Tables.Add(table);
        }

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
