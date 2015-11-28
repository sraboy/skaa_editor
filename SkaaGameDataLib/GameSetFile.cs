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
            //int rowcount = 0;

            //foreach (DataTable dt in ds.Tables)
            //    rowcount += dt.Rows.Count;

            //rowcount + 1 for the fake record at the end (nine null bytes and uint SET file size
            //sizeof(short) for the first two bytes, representing the number of records
            //int setHeaderSize = ((rowcount + 1) * ResourceDatabase.DefinitionSize) + sizeof(short);

            Dictionary<string, int> dic = new Dictionary<string, int>();

            using (FileStream setStream = new FileStream(filepath, FileMode.Create))
            {
                using (MemoryStream headerStream = new MemoryStream())
                {
                    //setStream.Seek(setHeaderSize, SeekOrigin.Begin); //leave room for the header

                    using (MemoryStream dbfStream = new MemoryStream())//new FileStream(filepath, FileMode.Create))
                    {
                        //write SET header's record_count
                        short record_count = (short) ds.Tables.Count;
                        headerStream.Write(BitConverter.GetBytes(record_count), 0, sizeof(short));
                        uint header_size = (uint) ((record_count + 1) * ResourceDatabase.DefinitionSize) + sizeof(short);

                        foreach (DataTable dt in ds.Tables)
                        {
                            //write SET header's record definitions
                            //---------------------
                            //char[9] record_names
                            //uint32 record_offsets
                            //---------------------
                            dt.WriteDefinition(headerStream, (uint) dbfStream.Position + header_size);

                            //writes out the DBF file
                            dt.Save(dbfStream);
                        }

                        //write SET file header-trailer (9 nulls followed by int filesize).
                        for (int i = 0; i < 9; i++)
                            headerStream.WriteByte(0x0);
                        //calculate filesize
                        uint file_size = (uint) (dbfStream.Position + (headerStream.Position + sizeof(uint)));
                        byte[] fileSize = BitConverter.GetBytes(file_size);
                        headerStream.Write(fileSize, 0, fileSize.Length);
                        headerStream.Position = dbfStream.Position = 0;
                        headerStream.CopyTo(setStream);
                        dbfStream.CopyTo(setStream);
                    }
                }
            }
        }
    }
}
