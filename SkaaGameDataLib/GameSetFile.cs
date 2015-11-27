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
    public static class GameSetFile
    {
        /* ----  SET File Format  ----
         *  short recordCount
         *  dBaseTableName
         *  DbfFileOffset (database table data)
         *         .
         *         .
         *  DbfFile[s]
         *         .
         *         .
         */

        public static FileStream Open(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            return fs;
        }

        public static void ReadSetFile(this DataSet ds, Stream str)
        {
            var defs = ResourceDatabase.ReadDefinitions(str);

            foreach (KeyValuePair<string, uint> kv in defs)
            {
                str.Position = kv.Value; //the DBF's offset value in the set file
                DbfFile file = new DbfFile();
                file.ReadStream(str);
                file.DataTable.TableName = Path.GetFileNameWithoutExtension(kv.Key);// + ".dbf");
                ds.Tables.Add(file.DataTable);
            }
        }

        public static void Save(this DataSet ds, string filepath)
        {
            int rowcount = 0;

            foreach (DataTable dt in ds.Tables)
                rowcount += dt.Rows.Count;

            //rowcount + 1 for the additional empty record
            //sizeof(short) for the first two bytes, representing the number of records
            int setHeaderSize = ((rowcount + 1) * ResourceDatabase.DefinitionSize) + sizeof(short);

            Dictionary<string, int> dic = new Dictionary<string, int>();

            using (FileStream setStream = new FileStream(filepath, FileMode.Create))
            {
                //setStream.Seek(setHeaderSize, SeekOrigin.Begin); //leave room for the header

                using (MemoryStream dbfStream = new MemoryStream())//new FileStream(filepath, FileMode.Create))
                {
                    dbfStream.Write(BitConverter.GetBytes(rowcount), 0, 2);

                    foreach (DataTable dt in ds.Tables)
                    {
                        dt.WriteDefinition(setStream, (uint)dbfStream.Position); //writes dt's definition to the SET header
                        dt.Save(dbfStream);
                    }

                    //write SET file header-trailer (9 nulls followed by int filesize).
                    for (int i = 0; i < 13; i++)
                        setStream.WriteByte(0x0);

                    dbfStream.CopyTo(dbfStream);
                }
            }
        }

//        public static Stream SaveGameSetToFile(DataSet ds)// string filePath)//, GameSet set)
//        {
//            using (FileStream fs = new FileStream(filePath, FileMode.Create))
//            {
//                //write SET file header
//                fs.Write(BitConverter.GetBytes(this._recordCount), 0, 2);
//                foreach (dBaseContainer db in this._databaseContainers)
//                {
//                    fs.Write(Encoding.GetEncoding(1252).GetBytes(db.Name.PadRight(_rowNameSize, (char) 0x0)), 0, _rowNameSize);
//                    fs.Write(BitConverter.GetBytes(Convert.ToInt32(db.Offset)), 0, 4);
//                }
//                //write SET file header-trailer (9 nulls followed by int filesize). write size after writing DBFs.
//                for (int i = 0; i < 13; i++)
//                    fs.WriteByte(0x0);
//                long fileSizeBookmark = fs.Position - 4;
//                //write out DBFs
//                foreach (dBaseContainer db in this._databaseContainers)
//                {
//#if DEBUG
//                    //writes out individual DBFs... easier to inspect SFRAME, etc
//                    string path = this._dbfDirectory + "test";
//                    Directory.CreateDirectory(path);
//                    db.DbfFileObject.WriteAndClose(path);
//#endif
//                    db.DbfFileObject.WriteToStream(fs);
//                }
//                fs.Position = fileSizeBookmark;
//                int fileSize = (int) fs.Length;
//                fs.Write(BitConverter.GetBytes(fileSize), 0, 4);
//            }
//        }
//        private static Stream BuildDbf(DataTable dt)
//        {
//            DbfFile file = new DbfFile(dt);
//        }
    }
}
