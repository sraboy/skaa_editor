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
    public class GameSet
    {
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
        private List<ResIndex> _databaseRows;
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

        public GameSet(byte[] setData = null, string path = null)
        {
            this._databaseRows = new List<ResIndex>(_recordCount);
            this._databases = new DataSet("skaa_dbs");

            if (setData != null && path != null)
            {
                this._workingPath = path;
                BuildDataSet(setData);
            }
        }

        private void ReadRawData(byte[] setData)
        {
            this._rawData = setData;

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
                        this._databaseRows.Add(row);
                    else
                        break;

                }//end while
            }
        }//end ReadRawData()

        public void BuildDataSet(byte[] setData)
        {
            if (setData == null)
                return;

            ReadRawData(setData);

            ResIndex row = this._databaseRows.Find(r => r.name == "SFRAME");
            int idx = this._databaseRows.FindIndex(r => r.name == "SFRAME");
            int dataSize = this._databaseRows[idx + 1].offset - row.offset;
            byte[] sframeData;   

            sframeData = new ArraySegment<byte>(this._rawData, row.offset, dataSize).ToArray();

            string tempFile = "sframe.dbf";//Path.GetTempFileName();

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
            this._databases.Tables.Add(table);
        }//end BuildDataSet()
    }//end GameSet
}
