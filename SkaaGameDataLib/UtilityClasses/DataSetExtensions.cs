#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditorUI
{
    public static class DataSetExtensions
    {
        public static void AddDataSource(this DataSet ds, string datasource)
        {
            List<string> dataSources = ds.ExtendedProperties["DataSources"] as List<string> ?? new List<string>();
            dataSources.Add("std.set");
            ds.ExtendedProperties["DataSources"] = dataSources;
        }

        public static bool OpenGameSet(this DataSet ds, Stream str)
        {
            var defs = ResourceDatabase.ReadDefinitions(str, true);

            foreach (KeyValuePair<string, uint> kv in defs)
            {
                str.Position = kv.Value; //the DBF's offset value in the set file
                DbfFile file = new DbfFile();
                if (file.ReadStream(str) != true)
                    return false;
                file.DataTable.TableName = Path.GetFileNameWithoutExtension(kv.Key);// + ".dbf");
                file.DataTable.ExtendedProperties.Add("FileName", (str as FileStream)?.Name);
                ds.Tables.Add(file.DataTable);
            }

            ds.AddDataSource("std.set");
            return true;
        }

        public static void SaveGameSet(this DataSet ds, string filepath)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            using (FileStream setStream = new FileStream(filepath, FileMode.Create))
            {
                using (MemoryStream headerStream = new MemoryStream())
                {
                    using (MemoryStream dbfStream = new MemoryStream())
                    {
                        //write SET header's record_count
                        short record_count = (short) ds.Tables.Count;
                        headerStream.Write(BitConverter.GetBytes(record_count), 0, sizeof(short));
                        uint header_size = (uint) ((record_count + 1) * ResourceDatabase.ResIdxDefinitionSize) + sizeof(short);

                        foreach (DataTable dt in ds.Tables)
                        {
                            //ignore DataTables not part of the Standard Game Set
                            if (Path.GetFileName((string) dt.ExtendedProperties["FileName"]) != "std.set")
                                continue;

                            //write SET header's record definitions
                            //---------------------
                            //char[9] record_names
                            //uint32 record_offsets
                            //---------------------
                            dt.WriteDefinition(headerStream, (uint) dbfStream.Position + header_size, true);

                            //writes out the DBF file
                            dt.Save(dbfStream);
                        }

                        //write SET file header-trailer (9 nulls followed by int filesize).
                        for (int i = 0; i < 9; i++)
                            headerStream.WriteByte(0x0);

                        //calculate and write out filesize
                        uint file_size = (uint) (dbfStream.Length + header_size);//(headerStream.Position + sizeof(uint)));
                        byte[] fileSize = BitConverter.GetBytes(file_size);
                        headerStream.Write(fileSize, 0, fileSize.Length);

                        //reset positions and copy streams to write out
                        headerStream.Position = dbfStream.Position = 0;
                        headerStream.CopyTo(setStream);
                        dbfStream.CopyTo(setStream);
                    }
                }
            }
        }
    }
}
