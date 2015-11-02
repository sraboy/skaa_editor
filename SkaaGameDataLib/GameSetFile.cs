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
    public class GameSetFile
    {
        /* ----  SET File Format  ----
         * short recordCount
         * ResourceIndex[recordCount]
         * Database[recordCount]
         */
        private string _fileName, _directory;
        private short _recordCount;
        private const int _rowNameSize = 9;
        private const int _resIndexSize = 13;

        public List<Database> Databases;
        public MemoryStream RawDataStream;
        public string FileName
        {
            get
            {
                return this._fileName;
            }
        }
        public string Directory
        {
            get
            {
                return this._directory;
            }
        }

        public GameSetFile() { }
        public GameSetFile(string filepath)
        {
            this._fileName = Path.GetFileName(filepath);
            this._directory = Path.GetDirectoryName(filepath);
            this.Databases = new List<Database>(this._recordCount);
        }

        public void Open()
        {
            using (FileStream fs = new FileStream(this._directory + '\\' + this._fileName, FileMode.Open))
            {
                //this._rawData = new byte[fs.Length];
                //fs.Read(this._rawData, 0, this._rawData.Length);

                this.RawDataStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this.RawDataStream);
                this.RawDataStream.Position = 0;

                //return this.RawDataStream;
            }

            ReadDatabaseDefinitions();
        }
        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet(this.FileName);

            foreach (Database db in this.Databases)
                ds.Tables.Add(GetTable(db));

            return ds;
        }

        private void ReadDatabaseDefinitions()
        {
            byte[] recCount = new byte[2];
            this.RawDataStream.Read(recCount, 0, 2);
            this._recordCount = BitConverter.ToInt16(recCount, 0);

            //need (this._recordCount + 1) because there's always an extra (empty) row/record at the end of the header
            while (this.RawDataStream.Position < (this._recordCount + 1) * _resIndexSize)
            {
                Database db = new Database();
                byte[] name = new byte[_rowNameSize];
                byte[] offset = new byte[sizeof(int)];

                this.RawDataStream.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                this.RawDataStream.Read(offset, 0, sizeof(int));

                db.Name = Encoding.UTF8.GetString(name).Trim('\0');
                db.Offset = BitConverter.ToInt32(offset, 0);

                if (db.Name != "")
                    this.Databases.Add(db);
                else
                    break;
            }
        }
        private DataTable GetTable(Database db)//int index)
        {
            //Database database = this.Databases.Find(d => d.Name == db.Name);
            string fileName = WriteTempDBFFile(db);// index, db);

            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
               this.Directory + "\\dbf" + ";Extended Properties=dBase III";

            return GetDataTableFromDBFFile(connex, fileName, db);
        }
        private string WriteTempDBFFile(Database db)//int index, Database r)
        {
            int dataSize;
            int index = this.Databases.FindIndex(d => d.Name == db.Name);

            if (index + 1 >= this.Databases.Count) //last row, just use the DBF's length (as if it were a separate file) 
                dataSize = (int) this.RawDataStream.Length - db.Offset;//this._rawData.Length - r.offset;
            else
                dataSize = this.Databases[index + 1].Offset - db.Offset;

            this.RawDataStream.Position = db.Offset;
            byte[] sframeData = new byte[dataSize];//[(int) this.RawDataStream.Length];// = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
            this.RawDataStream.Read(sframeData, 0, dataSize);// db.Offset, dataSize);

            string tempFileName = db.Name + ".dbf";

            using (FileStream wfs = new FileStream(this.Directory + "\\dbf\\" + tempFileName, FileMode.Create))
                wfs.Write(sframeData, 0, dataSize);

            return tempFileName;
        }
        private DataTable GetDataTableFromDBFFile(string connex, string dbfFileName, Database db)
        {
            using (OleDbConnection dbConnection = new OleDbConnection(connex))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + dbfFileName + ']', dbConnection);

                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    //DataTable table = new DataTable(db.Name);
                    db.Table = new DataTable(db.Name);

                    if (db.Name == "SFRAME")
                    {
                        //dbConnection.Close();
                        db.Table = FillTableCustom(dbfFileName, db.Name);
                    }
                    else
                    {
                        dbConnection.Open();
                        //adapter.FillSchema(db.Table, SchemaType.Mapped);
                        adapter.Fill(db.Table);
                    }

                    return db.Table;
                }
            }
        }
        private DataTable FillTableCustom(string dbfFileName, string tableName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fsSframe = new FileStream(this.Directory + "\\dbf\\" + dbfFileName, FileMode.Open))
                    fsSframe.CopyTo(ms);

                DbfFile dbfFile = new DbfFile(ms, tableName, (int) ms.Length);
                return dbfFile.GetConvertedRawDataTable();
            }
        }
    }
}
