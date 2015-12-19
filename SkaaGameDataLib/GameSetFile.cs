#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/
#endregion
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
