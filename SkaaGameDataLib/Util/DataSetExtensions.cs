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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SkaaGameDataLib.Util
{
    public static class DataSetExtensions
    {
        public static readonly TraceSource Logger = new TraceSource($"{typeof(DataSetExtensions)}", SourceLevels.All);

        private static readonly string StandardGameSetDefaultName = "std.set";
        private static readonly string DataSourcesPropertyName = "DataSource";

        public static void AddDataTableFromNewSource(this DataSet ds, DataTable dt)
        {
            ds.AddDataSource(dt.GetDataSource());
            ds.Tables.Add(dt);
        }
        /// <summary>
        /// Returns a <see cref="List{T}"/> of all data sources in the <see cref="DataSet.ExtendedProperties"/> element with the name of <see cref="DataSourcesPropertyName"/>
        /// </summary>
        public static List<string> GetDataSourceList(this DataSet ds)
        {
            if (ds.ExtendedProperties.Count == 0 || !ds.ExtendedProperties.ContainsValue(DataSourcesPropertyName))
                return null;

            List<string> list = new List<string>();

            foreach (DictionaryEntry ent in ds.ExtendedProperties)
            {
                if (ent.Value.ToString() == DataSourcesPropertyName)
                    list.Add(ent.Key.ToString());
            }

            return list;
        }
        /// <summary>
        /// Adds a new "data source" string to the <see cref="DataSet.ExtendedProperties"/> <see cref="List{T}"/> named <see cref="DataSourcesPropertyName"/>
        /// </summary>
        /// <param name="datasource">The name of the data source to add</param>
        /// <remarks>If <see cref="DataSet.ExtendedProperties"/> does not contain <see cref="DataSourcesPropertyName"/>, it will be created.</remarks>
        internal static void AddDataSource(this DataSet ds, string datasource)
        {
            ds.ExtendedProperties.Add(datasource, DataSourcesPropertyName);
            Logger.TraceInformation($"Added data source: {datasource}");
        }
        /// <summary>
        /// Removes the specified data source from the <see cref="DataSet.ExtendedProperties"/> <see cref="List{T}"/> named <see cref="DataSourcesPropertyName"/>
        /// </summary>
        /// <param name="datasource">The name of the data source to remove</param>
        /// <returns>false if the specified data source does not exist, true otherwise</returns>
        public static void RemoveDataSource(this DataSet ds, string datasource)
        {
            ds.ExtendedProperties.Remove(datasource);
            Logger.TraceInformation($"Removed data source: {datasource}");
        }

        /// <summary>
        /// Opens the specified <see cref="FileFormats.ResIdxDbf"/>, adds all of its tables and records to the <see cref="DataSet"/> and adds the file's name, 
        /// from <see cref="Path.GetFileName(string)"/>, as a new data source
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>false if <see cref="DbfFile.ReadStream(Stream)"/> returned false, true otherwise</returns>
        public static bool OpenStandardGameSet(this DataSet ds, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                var defs = ResourceDefinitionReader.ReadDefinitions(fs, false);

                // Create a backup copy in the event Tables.Add() succeeds for one or more tables before 
                // failing. This will allow us to just return false without mucking up the DataSet with 
                // only some of the loaded tables.
                using (DataSet temp = new DataSet())
                {
                    temp.Merge(ds);

                    foreach (KeyValuePair<string, uint> kv in defs)
                    {
                        fs.Position = kv.Value; //the DBF's offset value in the set file
                        DbfFile file = new DbfFile();
                        if (file.ReadStream(fs) != true)
                            return false;
                        file.DataTable.TableName = Path.GetFileNameWithoutExtension(kv.Key);
                        file.DataTable.AddDataSource(Path.GetFileName((fs as FileStream)?.Name));

                        if (ds.Tables.Contains(file.DataTable.TableName))
                        {
                            Logger.TraceEvent(TraceEventType.Error, 0, $"Failed to open standard game set due to a duplicate table: {file.DataTable.TableName} in {filePath}");
                            return false;
                        }
                        else
                            temp.Tables.Add(file.DataTable);
                    }

                    // only add the tables once we're sure there are no duplicates
                    ds.Merge(temp);
                    ds.AddDataSource(Path.GetFileName(filePath));
                    Logger.TraceInformation($"Opened standard game set from: {filePath}");
                }
            }
            return true;
        }

        /// <summary>
        /// Saves this <see cref="DataSet"/> to the specified file in the format of <see cref="FileFormats.ResIdxDbf"/>, only including a 
        /// <see cref="DataTable"/> if its <see cref="SkaaGameDataLib.Util.DataTableExtensions.DataSourcePropertyName"/> is <see cref="StandardGameSetDefaultName"/>
        /// </summary>
        /// <param name="filepath">The path and file to save this <see cref="DataSet"/> to</param>
        public static void SaveStandardGameSet(this DataSet ds, string filepath) => SaveGameSet(ds, filepath, StandardGameSetDefaultName);
        /// <summary>
        /// Saves a portion of this <see cref="DataSet"/> to the specified file in the format of <see cref="FileFormats.ResIdxDbf"/>
        /// </summary>
        /// <param name="setName">Specifies the <see cref="SkaaGameDataLib.Util.DataTableExtensions.DataSourcePropertyName"/> to consider part of the standard game set</param>
        /// <param name="filepath">The path and file to save this <see cref="DataSet"/> to</param>
        public static void SaveGameSet(this DataSet ds, string filepath, string setName)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                var stream = ds.GetGameSetStream(setName);
                stream.Position = 0;
                stream.CopyTo(fs);
            }

            Logger.TraceInformation($"Saved game set: {setName} to {filepath}");
        }

        public static void ExportGameSetToCSV(this DataSet ds)
        {
            //todo: Move to GameSetPresenter w/ OpenFileDialog

            StreamWriter sw = null;
            StringBuilder sb = new StringBuilder();
            foreach (DataTable dataTable in ds.Tables)
            {
                sw = new StreamWriter($"{dataTable.TableName}.csv");
                sb.Clear();
                //write column headers
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    sb.Append(dataTable.Columns[j].ColumnName);
                    if (j != (dataTable.Columns.Count - 1))
                        sb.Append(",");
                }

                sw.WriteLine(sb);
                //write data
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    sb.Clear();

                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        sb.Append(dataTable.Rows[i][j]);
                        if (j != (dataTable.Columns.Count - 1))
                            sb.Append(",");
                    }
                    sw.WriteLine(sb.ToString());
                }
                sw.Close();
            }
        }

        /// <summary>
        /// Get a <see cref="MemoryStream"/> of the entire <see cref="DataSet"/>
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> in the format of std.set</returns>
        public static Stream GetGameSetStream(this DataSet ds) => GetGameSetStream(ds, null);
        /// <summary>
        /// Get a <see cref="MemoryStream"/> of the <see cref="DataSet"/> consisting of the standard game set, <see cref="StandardGameSetDefaultName"/>
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> in the format of std.set</returns>
        public static Stream GetStandardGameSetStream(this DataSet ds) => GetGameSetStream(ds, StandardGameSetDefaultName);
        /// <summary>
        /// Get a <see cref="MemoryStream"/> of the <see cref="DataSet"/> consisting of the specified set
        /// </summary>
        /// <param name="set">A value that matches the <see cref="DataSourcesPropertyName"/> element in <see cref="DataSet.ExtendedProperties"/>, 
        /// which further matches an element of each table's <see cref="DataTable.ExtendedProperties"/>. If null, all tables are used.</param>
        /// <returns>A <see cref="MemoryStream"/> in the format of std.set</returns>
        public static Stream GetGameSetStream(this DataSet ds, string set)
        {
            if (set == null)
                return null;

            Dictionary<string, int> dic = new Dictionary<string, int>();

            MemoryStream setStream = new MemoryStream();

            using (MemoryStream headerStream = new MemoryStream())
            {
                using (MemoryStream dbfStream = new MemoryStream())
                {
                    //get the record count for (number of tables in) the specified set
                    short record_count = 0;// = (short)ds.Tables.Count;
                    foreach (DataTable dt in ds.Tables)
                        if (dt.GetDataSource() == set)
                            record_count++;
                    //write the record_count
                    headerStream.Write(BitConverter.GetBytes(record_count), 0, sizeof(short));

                    //we'll need this to calculate each table's offset in the file
                    uint header_size = (uint)((record_count + 1) * ResourceDefinitionReader.ResIdxDefinitionSize) + sizeof(short);

                    foreach (DataTable dt in ds.Tables)
                    {
                        //ignore DataTables not part of the Standard Game Set
                        if (dt.GetDataSource() != set)
                            continue;

                        //write SET header's record definitions
                        //---------------------
                        //char[9] record_names
                        //uint32 record_offsets
                        //---------------------
                        dt.WriteResDefinition(headerStream, (uint)dbfStream.Position + header_size);

                        //writes out the DBF file
                        dt.Save(dbfStream);
                    }

                    //write SET file header-trailer (9 nulls followed by int filesize).
                    for (int i = 0; i < 9; i++)
                        headerStream.WriteByte(0x0);

                    //calculate and write out filesize
                    uint file_size = (uint)(dbfStream.Length + header_size);//(headerStream.Position + sizeof(uint)));
                    byte[] fileSize = BitConverter.GetBytes(file_size);
                    headerStream.Write(fileSize, 0, fileSize.Length);

                    //reset positions and copy streams to write out
                    headerStream.Position = dbfStream.Position = 0;
                    headerStream.CopyTo(setStream);
                    dbfStream.CopyTo(setStream);
                }
            }

            return setStream;
        }
    }
}
