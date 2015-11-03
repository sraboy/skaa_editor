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

        public List<DatabaseContainer> DatabaseContainers;
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
            this.DatabaseContainers = new List<DatabaseContainer>(this._recordCount);
        }

        public DataSet OpenAndRead()
        {
            using (FileStream fs = new FileStream(this._directory + '\\' + this._fileName, FileMode.Open))
            {
                this.RawDataStream = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(this.RawDataStream);
                this.RawDataStream.Position = 0;
            }

            ReadDatabaseDefinitions();
            return BuildDataSetFromDbfFiles();
        }
        private void ReadDatabaseDefinitions()
        {
            byte[] recCount = new byte[2];
            this.RawDataStream.Read(recCount, 0, 2);
            this._recordCount = BitConverter.ToInt16(recCount, 0);

            //need (this._recordCount + 1) because there's always an extra (empty) row/record at the end of the header
            while (this.RawDataStream.Position < (this._recordCount + 1) * _resIndexSize)
            {
                DatabaseContainer db = new DatabaseContainer();
                byte[] name = new byte[_rowNameSize];
                byte[] offset = new byte[sizeof(int)];

                this.RawDataStream.Read(name, 0, _rowNameSize); //offset is 0 from ms.Position
                this.RawDataStream.Read(offset, 0, sizeof(int));

                db.Name = Encoding.UTF8.GetString(name).Trim('\0');
                db.Offset = BitConverter.ToInt32(offset, 0);

                if (db.Name != "")
                    this.DatabaseContainers.Add(db);
                else
                    break;
            }
        }
        private DataSet BuildDataSetFromDbfFiles()
        {
            DataSet ds = new DataSet(this.FileName);

            foreach (DatabaseContainer db in this.DatabaseContainers)
            {
                string fileName = WriteTempDBFFile(db);
                db.Table = GetTable(fileName);               
                ds.Tables.Add(db.Table);
            }

            return ds;
        }
        private string WriteTempDBFFile(DatabaseContainer db)//int index, Database r)
        {
            int dataSize;
            int index = this.DatabaseContainers.FindIndex(d => d.Name == db.Name);

            if (index + 1 >= this.DatabaseContainers.Count) //last row, just use the DBF's length (as if it were a separate file) 
                dataSize = (int) this.RawDataStream.Length - db.Offset;//this._rawData.Length - r.offset;
            else
                dataSize = this.DatabaseContainers[index + 1].Offset - db.Offset;

            this.RawDataStream.Position = db.Offset;
            byte[] sframeData = new byte[dataSize];//[(int) this.RawDataStream.Length];// = new ArraySegment<byte>(this._rawData, r.offset, dataSize).ToArray();
            this.RawDataStream.Read(sframeData, 0, dataSize);// db.Offset, dataSize);

            string tempFileName = db.Name + ".dbf";

            using (FileStream wfs = new FileStream(this.Directory + "\\dbf\\" + tempFileName, FileMode.Create))
                wfs.Write(sframeData, 0, dataSize);

            return tempFileName;
        }
        private DataTable GetTable(string dbfFileName)//int index)
        {
            string connex = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
               this.Directory + "\\dbf" + ";Extended Properties=dBase III";

            return GetDataTableFromDBFFile(connex, dbfFileName);//, db);
        }
        private DataTable GetDataTableFromDBFFile(string connex, string dbfFileName)//, Database db)
        {
            DataTable dt;

            using (OleDbConnection dbConnection = new OleDbConnection(connex))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + dbfFileName + ']', dbConnection);

                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    //DataTable table = new DataTable(db.Name);
                    dt = new DataTable(Path.GetFileNameWithoutExtension(dbfFileName));
                    dt = FillTableCustom(dbfFileName, dt.TableName);

                    //if (dt.TableName == "SFRAME")
                    //{
                    //    //dbConnection.Close();
                    //    dt = FillTableCustom(dbfFileName, dt.TableName);
                    //}
                    //else
                    //{
                    //    dbConnection.Open();
                    //    //adapter.FillSchema(db.Table, SchemaType.Mapped);
                    //    adapter.Fill(dt);
                    //}
                    
                    return dt;
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
                return dbfFile.FillAndGetTable();
            }
        }


        public void SaveGameSetToFile(string filePath, GameSet set)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(BitConverter.GetBytes(this._recordCount), 0, 2);

                int lowestOffset = int.MaxValue;

                foreach (DatabaseContainer db in this.DatabaseContainers)
                {
                    if (db.Offset < lowestOffset)
                        lowestOffset = db.Offset;

                    byte[] paddedName = Encoding.ASCII.GetBytes(db.Name.PadRight(9, '\0'));
                    byte[] offset = new byte[4];
                    offset = BitConverter.GetBytes(db.Offset);
                    fs.Write(paddedName, 0, 9);
                    fs.Write(offset, 0, 4);
                }

                //write the extra padding at the end
                for(int i = 0; i < 9; i++)
                    fs.WriteByte(0x0);


                WriteSframe(fs, set);


                //foreach(Database db in this.Databases)
                //{
                //    DataTable dt = db.Table;

                //    foreach(DataRow dr in dt.Rows)
                //    {
                //        for(int i = 0; i < dr.ItemArray.Count(); i++)
                //        {
                            
                //        }
                //    }

                //}
            }
        }

        private void WriteSframe(FileStream fs, GameSet set)
        {
            //write individual databases
            DataTable dt = set.SetDatabase.Tables["SFRAME"];

            foreach (DataRow dr in dt.Rows)
            {
                fs.WriteByte(0x20); //row divider

                for (int i = 0; i < dr.ItemArray.Count(); i++)
                {
                    byte[] bytes;
                    switch (i)
                    {
                        case 0: //char[8] SPRITE
                            string sprName = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
                            sprName = sprName.Length < 8 ? sprName.PadRight(8, (char) 0x20) : sprName;
                            bytes = Encoding.ASCII.GetBytes(sprName);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 1: //char[2] ACTION
                            string action = (dr[i] == DBNull.Value) ? "0" : (string) dr[i];
                            action = action.Length < 2 ? action.PadRight(2, (char) 0x20) : action;
                            bytes = Encoding.ASCII.GetBytes(action);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 2: //char[2] DIR
                            string dir = (dr[i] == DBNull.Value) ? "0" : (string) dr[i];
                            dir = dir.Length < 2 ? dir.PadRight(2, (char) 0x20) : dir;
                            bytes = Encoding.ASCII.GetBytes(dir);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 3: //char[2] FRAME
                            string frame = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
                            frame = frame.Length < 2 ? frame.PadRight(2, (char) 0x20) : frame;
                            bytes = Encoding.ASCII.GetBytes(frame);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 4: //char[4] OFFSET_X
                            string offset_x = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
                            offset_x = offset_x.Length < 4 ? offset_x.PadLeft(4, (char) 0x20) : offset_x;
                            bytes = Encoding.ASCII.GetBytes(offset_x);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 5: //char[4] OFFSET_Y
                            string offset_y = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
                            offset_y = offset_y.Length < 4 ? offset_y.PadLeft(4, (char) 0x20) : offset_y;
                            bytes = Encoding.ASCII.GetBytes(offset_y);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 6: //char[3] WIDTH
                            string width = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
                            width = width.Length < 3 ? width.PadLeft(3, (char) 0x20) : width;
                            bytes = Encoding.ASCII.GetBytes(width);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 7: //char[3] HEIGHT
                            string height = (dr[i] == DBNull.Value) ? string.Empty : ((int) ((long) dr[i])).ToString();
                            height = height.Length < 3 ? height.PadLeft(3, (char) 0x20) : height;
                            bytes = Encoding.ASCII.GetBytes(height);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 8: //char[8] FILENAME
                            string filename = (dr[i] == DBNull.Value) ? string.Empty : (string) dr[i];
                            filename = filename.Length < 8 ? filename.PadRight(8, (char) 0x20) : filename;
                            bytes = Encoding.ASCII.GetBytes(filename);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case 9: //char[4] BITMAPPTR
                            int ptr = (dr[i] == DBNull.Value) ? 0 : Convert.ToInt32((string) dr[i]);
                            bytes = BitConverter.GetBytes(ptr);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }

            fs.WriteByte(0x1a); //EOF
        }


    }
}
