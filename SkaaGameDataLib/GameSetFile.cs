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

        public static DataSet ReadAll(Stream str)
        {
            var defs = ResourceDatabase.ReadDefinitions(str);

            DataSet ds = new DataSet();

            foreach (KeyValuePair<string, uint> kv in defs)
            {
                str.Position = kv.Value; //the DBF's offset value in the set file
                DbfFile file = new DbfFile(kv.Key);
                file.ReadStream(str);
                DataTable dt = file.DataTable;
                dt.TableName = Path.GetFileNameWithoutExtension(kv.Key + ".dbf");
                ds.Tables.Add(dt);
            }

            return ds;
        }

        public static void WriteSet(DataSet ds)
        {

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
