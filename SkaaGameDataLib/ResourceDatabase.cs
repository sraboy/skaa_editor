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
    public static class ResourceDatabase
    {
        //private Properties.Settings props = Properties.Settings.Default;

        /* ----  SET File Format  ----
         * short recordCount
         * ResourceIndex[recordCount]
         * Database[recordCount]
         */

        //private string _tempDirectory, _dbfDirectory, _setFilePath;
        //private const int _rowNameSize = 9;
        //private const int _resIndexSize = 13;

        //public List<DatabaseContainer> DatabaseContainers;
        //public MemoryStream RawDataStream;
        //public DataSet GameDataSet;

        ///// <summary>
        ///// Initializes a new <see cref="GameSetFile"/> object.
        ///// </summary>
        ///// <param name="setFilePath">The complete path to the SET file that this object represents.</param>
        ///// <param name="tempDirectoryPath">The complete path to a temporary directory the class can use freely.</param>
        //public ResourceDatabase(string setFilePath, string tempDirectoryPath)
        //{
        //    this._setFilePath = setFilePath;
        //    this._tempDirectory = tempDirectoryPath;
        //    this._dbfDirectory = this._tempDirectory + "dbf\\";
        //    //this.DatabaseContainers = new List<DatabaseContainer>(this._recordCount);
        //}

        public static DataSet OpenReadSetFile(string filePath, string tempDirectory)
        {
            var RawDataStream = new MemoryStream();

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                fs.Position = 0;
                fs.CopyTo(RawDataStream);
                RawDataStream.Position = 0;
            }

            Dictionary<string, uint> dic = ReadDatabaseDefinitions(RawDataStream, 9);

            var DatabaseContainers = new List<DatabaseContainer>(dic.Count);

            foreach (KeyValuePair<string, uint> kv in dic)
                DatabaseContainers.Add(new DatabaseContainer() { Name = kv.Key, Offset = (int)kv.Value });

            return BuildDataSetFromDbfFiles(Path.GetFileName(filePath), DatabaseContainers, RawDataStream, tempDirectory);
        }

        public static Dictionary<string, uint> ReadDatabaseDefinitions(Stream RawDataStream, int nameSize)//, int offSetSize)
        {
            int definitionSize = nameSize + 5; //4-byte int plus null

            byte[] recCount = new byte[2];
            RawDataStream.Read(recCount, 0, 2);
            int recordCount = BitConverter.ToInt16(recCount, 0);

            Dictionary<string, uint> dbContainers = new Dictionary<string, uint>(recordCount);

            //need (recordCount + 1) because there's always an extra (empty) row/record at the end of the header
            while (RawDataStream.Position < (recordCount) * definitionSize)
            {
                DatabaseContainer db = new DatabaseContainer();
                byte[] name = new byte[nameSize];
                byte[] offset = new byte[4];

                RawDataStream.Read(name, 0, nameSize); //offset is 0 from ms.Position
                RawDataStream.Read(offset, 0, 4);

                db.Name = Encoding.GetEncoding(1252).GetString(name).Trim('\0');

                var sh1 = BitConverter.ToInt16(offset.Reverse().ToArray(), 0);
                var sh2 = BitConverter.ToInt16(offset.Reverse().ToArray(), 2);
                var all = sh1 + sh2;
                db.Offset = all;//BitConverter.ToInt32(offset.Reverse().ToArray(), 0);

                RawDataStream.ReadByte(); //a null at the end of every definition

                if (db.Name == "") //always the final "record" with nulls for name and the file's size for the offset
                {
                    //int fileSize = db.Offset;
                    break;
                }
                else
                    dbContainers.Add(db.Name, (uint)db.Offset);
            }

            return dbContainers;
        }

        private static DataSet BuildDataSetFromDbfFiles(string dataSetName, List<DatabaseContainer> DatabaseContainers, Stream RawDataStream, string tempDirectory)
        {
            DataSet ds = new DataSet(dataSetName);

            foreach (DatabaseContainer db in DatabaseContainers)
            {
                int index = DatabaseContainers.FindIndex(d => d.Name == db.Name);

                int dataSize;
                if (index + 1 >= DatabaseContainers.Count) //last row, just use the DBF's length (as if it were a separate file) 
                    dataSize = (int) RawDataStream.Length - db.Offset;
                else
                    dataSize = DatabaseContainers[index + 1].Offset - db.Offset;

                RawDataStream.Position = db.Offset;
                byte[] sframeData = new byte[dataSize];
                RawDataStream.Read(sframeData, 0, dataSize);

                string dbfFileName = db.Name + ".dbf";

                //we have to write all the DBFs
                Directory.CreateDirectory(tempDirectory);
                using (FileStream wfs = new FileStream(tempDirectory + dbfFileName, FileMode.Create))
                    wfs.Write(sframeData, 0, dataSize);

                DataTable dt;

                dt = new DataTable(Path.GetFileNameWithoutExtension(dbfFileName));

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fsSframe = new FileStream(tempDirectory + dbfFileName, FileMode.Open))
                        fsSframe.CopyTo(ms);

                    db.DbfFileObject = new DbfFile(ms, dt.TableName, (int) ms.Length);
                    dt = db.DbfFileObject.FillAndGetTable();
                }

                db.Table = dt;
                ds.Tables.Add(db.Table);
            }

            return ds;
        }

//        public static MemoryStream DataSetToStream(string filePath, DataSet ds)//, GameSet set)
//        {
//            MemoryStream ms = new MemoryStream();
//            //using (FileStream fs = new FileStream(filePath, FileMode.Create))
//            //{
//                int recordCount = 0;

//                foreach (DataTable dt in ds.Tables)
//                    recordCount += dt.Rows.Count;

//                //write SET file header
//                ms.Write(BitConverter.GetBytes(recordCount), 0, 2);
                
//                foreach (DataTable dt in ds.Tables)
//                {
//                    ms.Write(Encoding.GetEncoding(1252).GetBytes(dt.TableName.PadRight(_rowNameSize, (char) 0x0)), 0, _rowNameSize);
//                    ms.Write(BitConverter.GetBytes(Convert.ToInt32(db.Offset)), 0, 4);
//                }

//                //write SET file header-trailer (9 nulls followed by int filesize). write size after writing DBFs.
//                for (int i = 0; i < 13; i++)
//                    ms.WriteByte(0x0);

//                long fileSizeBookmark = ms.Position - 4;


//                //write out DBFs
//                foreach (DataTable dt in ds.Tables)
//                {
////#if DEBUG
////                    //writes out individual DBFs... easier to inspect SFRAME, etc
////                    string path = tempDirectory + "test";
////                    Directory.CreateDirectory(path);
////                    db.DbfFileObject.WriteAndClose(path);
////#endif
//                    db.DbfFileObject.WriteToStream(ms);
//                }
//                ms.Position = fileSizeBookmark;
//                int fileSize = (int) ms.Length;
//                ms.Write(BitConverter.GetBytes(fileSize), 0, 4);
//            //}

//            return ms;
//        }

        public static MemoryStream BuildSetFile(MemoryStream str)
        {
            MemoryStream ms = new MemoryStream();

            return ms;
        }
    }
}
