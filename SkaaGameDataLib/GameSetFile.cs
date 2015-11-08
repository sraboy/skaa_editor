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
        private string /*_setFileName, _setFileDirectory,*/ _tempDirectory, _dbfDirectory, _setFilePath;
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
            //this._setFileName = Path.GetFileName(setFilePath);
            //this._setFileDirectory = Path.GetDirectoryName(setFilePath);
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

                //string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                //                this._dbfDirectory + 
                //                ";Extended Properties=dBase III";

                //using (OleDbConnection dbConnection = new OleDbConnection(connex))
                //{
                //    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + dbfFileName + ']', dbConnection);

                //    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                //    {

                        dt = new DataTable(Path.GetFileNameWithoutExtension(dbfFileName));

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fsSframe = new FileStream(this._dbfDirectory + dbfFileName, FileMode.Open))
                        fsSframe.CopyTo(ms);

                    db.DbfFileObject = new DbfFile(ms, dt.TableName, (int) ms.Length);
                    dt = db.DbfFileObject.FillAndGetTable();
                }
                //    }
                //}

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
                    
                    //writes out individual DBFs
                    if(System.IO.Directory.Exists(filePath + "dbf\\test"))
                    { 
                        string path = Path.GetDirectoryName(filePath) + "\\dbf\\test";
                        db.DbfFileObject.WriteAndClose(path);
                    }
#endif
                    db.DbfFileObject.WriteToStream(fs);
                }
                fs.Position = fileSizeBookmark;
                int fileSize = (int) fs.Length;
                fs.Write(BitConverter.GetBytes(fileSize), 0, 4);
            }
        }

        #region deprecated
        //private void WriteSframe(FileStream fs, GameSet set)
        //{
        //    //write individual databases
        //    DataTable dt = set.GameDataSet.Tables["SFRAME"];
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        fs.WriteByte(0x20); //row divider
        //        for (int i = 0; i < dr.ItemArray.Count(); i++)
        //        {
        //            byte[] bytes;
        //            switch (i)
        //            {
        //                case 0: //char[8] SPRITE
        //                    string sprName = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
        //                    sprName = sprName.Length < 8 ? sprName.PadRight(8, (char) 0x20) : sprName;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(sprName);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 1: //char[2] ACTION
        //                    string action = (dr[i] == DBNull.Value) ? "0" : (string) dr[i];
        //                    action = action.Length < 2 ? action.PadRight(2, (char) 0x20) : action;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(action);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 2: //char[2] DIR
        //                    string dir = (dr[i] == DBNull.Value) ? "0" : (string) dr[i];
        //                    dir = dir.Length < 2 ? dir.PadRight(2, (char) 0x20) : dir;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(dir);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 3: //char[2] FRAME
        //                    string frame = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
        //                    frame = frame.Length < 2 ? frame.PadRight(2, (char) 0x20) : frame;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(frame);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 4: //char[4] OFFSET_X
        //                    string offset_x = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
        //                    offset_x = offset_x.Length < 4 ? offset_x.PadLeft(4, (char) 0x20) : offset_x;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(offset_x);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 5: //char[4] OFFSET_Y
        //                    string offset_y = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
        //                    offset_y = offset_y.Length < 4 ? offset_y.PadLeft(4, (char) 0x20) : offset_y;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(offset_y);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 6: //char[3] WIDTH
        //                    string width = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
        //                    width = width.Length < 3 ? width.PadLeft(3, (char) 0x20) : width;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(width);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 7: //char[3] HEIGHT
        //                    string height = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
        //                    height = height.Length < 3 ? height.PadLeft(3, (char) 0x20) : height;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(height);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 8: //char[8] FILENAME
        //                    string filename = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
        //                    filename = filename.Length < 8 ? filename.PadRight(8, (char) 0x20) : filename;
        //                    bytes = Encoding.GetEncoding(1252).GetBytes(filename);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //                case 9: //char[4] BITMAPPTR
        //                    int ptr = (dr[i] == DBNull.Value) ? 0 : Convert.ToInt32((string) dr[i]);
        //                    bytes = BitConverter.GetBytes(ptr);
        //                    fs.Write(bytes, 0, bytes.Length);
        //                    break;
        //            }
        //        }
        //    }
        //    fs.WriteByte(0x1a); //EOF
        //}
        #endregion
    }
}
