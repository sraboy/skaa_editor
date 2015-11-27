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
        private class dBaseContainer
        {
            //Lifted from ORESX.h
            public string Name; //char[9] in std.set
            public int Offset;
            public DataTable Table;
            public DbfFile DbfFileObject;
        }

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

        private const int _rowNameSize = 9; //8 chars + null
        //private const int _dbfFileOffsetSize = 4; //uint32
        private const int _resIndexSize = 13; //add the above two 

        /// <summary>
        /// Reads a the header of a stream containing char[9], uint32 data: a datatable's name and its offset in a file
        /// </summary>
        /// <param name="str">The stream from which to read. The first two bytes must be a uint16 containing the number of definitions</param>
        /// <returns></returns>
        public static Dictionary<string, uint> ReadDefinitions(Stream str)//, int nameSize)//, int offSetSize)
        {
            byte[] recCount = new byte[2];
            str.Read(recCount, 0, 2);
            ushort recordCount = BitConverter.ToUInt16(recCount, 0);
            Dictionary<string, uint> nameOffsetPairs = new Dictionary<string, uint>(recordCount);

            while (str.Position < (recordCount) * _resIndexSize)
            {
                byte[] b_name = new byte[_rowNameSize];
                byte[] b_offset = new byte[4];
                string name = string.Empty;
                uint offset;

                str.Read(b_name, 0, _rowNameSize); //offset is 0 from ms.Position
                str.Read(b_offset, 0, 4);

                name = Encoding.GetEncoding(1252).GetString(b_name).Trim('\0');
                offset = BitConverter.ToUInt32(b_offset, 0);

                nameOffsetPairs.Add(name, offset);
            }

            return nameOffsetPairs;
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

//        public static MemoryStream BuildSetFile(MemoryStream str, DataSet ds)
//        {
//            MemoryStream ms = new MemoryStream();
//            int _recordCount = 0;

//            foreach (DataTable dt in ds.Tables)
//                _recordCount += dt.Rows.Count;

//            //write SET file header
//            ms.Write(BitConverter.GetBytes(_recordCount), 0, 2);
//            foreach (DataTable dt in ds.Tables)
//            {
//                ms.Write(Encoding.GetEncoding(1252).GetBytes(dt.TableName.PadRight(_rowNameSize, (char) 0x0)), 0, _rowNameSize);
//                ms.Write(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, 4);//BitConverter.GetBytes(Convert.ToInt32(db.Offset)), 0, 4);
//            }

//            //write SET file header-trailer (9 nulls followed by int filesize). write size after writing DBFs.
//            for (int i = 0; i < 13; i++)
//                ms.WriteByte(0x0);

//            long fileSizeBookmark = fs.Position - 4;

//            //write out DBFs
//            foreach (dBaseContainer db in this._databaseContainers)
//            {
//#if DEBUG
//                //writes out individual DBFs... easier to inspect SFRAME, etc
//                string path = this._dbfDirectory + "test";
//                Directory.CreateDirectory(path);
//                db.DbfFileObject.WriteAndClose(path);
//#endif
//                db.DbfFileObject.WriteToStream(fs);
//            }
//            fs.Position = fileSizeBookmark;
//            int fileSize = (int) fs.Length;
//            fs.Write(BitConverter.GetBytes(fileSize), 0, 4);
            

//            return ms;
//        }
    }
}
