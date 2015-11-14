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
        private Properties.Settings props = Properties.Settings.Default;

        /* ----  SET File Format  ----
         * short recordCount
         * ResourceIndex[recordCount]
         * Database[recordCount]
         */
        private string _tempDirectory, _dbfDirectory, _setFilePath;
        private short _recordCount;
        private const int _rowNameSize = 9;
        private const int _resIndexSize = 13;

        public int FileSize;
        public List<DatabaseContainer> DatabaseContainers;
        public MemoryStream RawDataStream;
        public DataSet GameDataSet;

        /// <summary>
        /// Initializes a new <see cref="GameSetFile"/> object.
        /// </summary>
        /// <param name="setFilePath">The complete path to the SET file that this object represents.</param>
        /// <param name="tempDirectoryPath">The complete path to a temporary directory the class can use freely.</param>
        public GameSetFile(string setFilePath, string tempDirectoryPath)
        {
            this._setFilePath = setFilePath;
            this._tempDirectory = tempDirectoryPath;
            this._dbfDirectory = this._tempDirectory + "dbf\\";

            this.DatabaseContainers = new List<DatabaseContainer>(this._recordCount);
        }

        public void OpenAndRead()
        {
            using (FileStream fs = new FileStream(this._setFilePath, FileMode.Open))
            {
                this.RawDataStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this.RawDataStream);
                this.RawDataStream.Position = 0;
            }

            ReadDatabaseDefinitions();
            this.GameDataSet = BuildDataSetFromDbfFiles();
        }
        private void ReadDatabaseDefinitions()
        {
            byte[] recCount = new byte[2];
            this.RawDataStream.Read(recCount, 0, 2);
            this._recordCount = BitConverter.ToInt16(recCount, 0);

            //need (this._recordCount + 1) because there's always an extra (empty) row/record at the end of the header
            while (this.RawDataStream.Position < (this._recordCount) * _resIndexSize)
            {
                DatabaseContainer db = new DatabaseContainer();
                byte[] name = new byte[_rowNameSize];
                byte[] offset = new byte[sizeof(int)];

                this.RawDataStream.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                this.RawDataStream.Read(offset, 0, sizeof(int));

                db.Name = Encoding.GetEncoding(1252).GetString(name).Trim('\0');
                db.Offset = BitConverter.ToInt32(offset, 0);


                if (db.Name == "") //always the final "record" with nulls for name and the file's size for the offset
                {
                    this.FileSize = db.Offset;
                    break;
                }
                else
                    this.DatabaseContainers.Add(db);
            }
        }
        private DataSet BuildDataSetFromDbfFiles()
        {
            DataSet ds = new DataSet(Path.GetFileName(this._setFilePath));

            foreach (DatabaseContainer db in this.DatabaseContainers)
            {
                int index = this.DatabaseContainers.FindIndex(d => d.Name == db.Name);

                int dataSize;
                if (index + 1 >= this.DatabaseContainers.Count) //last row, just use the DBF's length (as if it were a separate file) 
                    dataSize = (int) this.RawDataStream.Length - db.Offset;
                else
                    dataSize = this.DatabaseContainers[index + 1].Offset - db.Offset;

                this.RawDataStream.Position = db.Offset;
                byte[] sframeData = new byte[dataSize];
                this.RawDataStream.Read(sframeData, 0, dataSize);

                string dbfFileName = db.Name + ".dbf";

                //we have to write all the DBGs
                Directory.CreateDirectory(this._dbfDirectory);
                using (FileStream wfs = new FileStream(this._dbfDirectory + dbfFileName, FileMode.Create))
                    wfs.Write(sframeData, 0, dataSize);

                DataTable dt;

                dt = new DataTable(Path.GetFileNameWithoutExtension(dbfFileName));

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fsSframe = new FileStream(this._dbfDirectory + dbfFileName, FileMode.Open))
                        fsSframe.CopyTo(ms);

                    db.DbfFileObject = new DbfFile(ms, dt.TableName, (int) ms.Length);
                    dt = db.DbfFileObject.FillAndGetTable();
                }

                db.Table = dt;
                ds.Tables.Add(db.Table);
            }

            return ds;
        }

        internal void SaveGameSetToFile(string filePath)//, GameSet set)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                //write SET file header
                fs.Write(BitConverter.GetBytes(this._recordCount), 0, 2);
                foreach (DatabaseContainer db in this.DatabaseContainers)
                {
                    fs.Write(Encoding.GetEncoding(1252).GetBytes(db.Name.PadRight(_rowNameSize, (char) 0x0)), 0, _rowNameSize);
                    fs.Write(BitConverter.GetBytes(Convert.ToInt32(db.Offset)), 0, 4);
                }

                //write SET file header-trailer (9 nulls followed by int filesize). write size after writing DBFs.
                for (int i = 0; i < 13; i++)
                    fs.WriteByte(0x0);

                long fileSizeBookmark = fs.Position - 4;

                //write out DBFs
                foreach (DatabaseContainer db in this.DatabaseContainers)
                {
#if DEBUG
                    //writes out individual DBFs... easier to inspect SFRAME, etc
                    string path = this._dbfDirectory + "test";
                    Directory.CreateDirectory(path);
                    db.DbfFileObject.WriteAndClose(path);
#endif
                    db.DbfFileObject.WriteToStream(fs);
                }
                fs.Position = fileSizeBookmark;
                int fileSize = (int) fs.Length;
                fs.Write(BitConverter.GetBytes(fileSize), 0, 4);
            }
        }
    }
}
