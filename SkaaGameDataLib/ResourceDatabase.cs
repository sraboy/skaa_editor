using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public class ResourceDatabase
    {
        public static readonly TraceSource Logger = new TraceSource("ResourceDatabase", SourceLevels.All);

        /// <summary>
        /// The number of characters, including null, that is used to name a ResIdx 
        /// field. Shorter names are padded with spaces (0x20).
        /// </summary>
        public static readonly int ResIdxNameSize = 9;   //8 chars + null
        /// <summary>
        /// The number of characters, that is be used to name a Res 
        /// field. Shorter names are padded with spaces (0x20).
        /// </summary>
        public static readonly int ResNameSize = 8;   //8 chars
        /// <summary>
        /// The number of bytes used to represent the offset of a RESX 
        /// record. Smaller values are padded with nulls (0x0).
        /// </summary>
        public static readonly int OffsetSize = 4; //uint32
        /// <summary>
        /// The total size of a RESX definition.
        /// </summary>
        public static readonly int ResIdxDefinitionSize = 13;      //add the above two
        /// <summary>
        /// The total size of a Res definition.
        /// </summary>
        public static readonly int ResDefinitionSize = 12;      //add the above two
        /// <summary>
        /// Arbitrary number that shouldn't be necessary for 7KAA. Since the record count
        /// is read from the file, it could be any 16-bit value. Since ResX files have no
        /// header by which to identify the file, we assume a file claiming more than 150
        /// records is not a valid RESX file. 
        /// </summary>
        public static readonly int MaxRecordCount = 150;

        /// <summary>
        /// Reads a the header of a stream containing char[9], uint32 data: a datatable's name and its offset in a file
        /// </summary>
        /// <param name="str">The stream from which to read. The first two bytes must be a uint16 containing the number of definitions</param>
        /// <returns></returns>
        public static Dictionary<string, uint> ReadDefinitions(Stream str, bool isIdx)
        {
            int nameSize, definitionSize;

            if(isIdx)
            {
                nameSize = ResIdxNameSize;
                definitionSize = ResIdxDefinitionSize;
            }
            else
            {
                nameSize = ResNameSize;
                definitionSize = ResDefinitionSize;
            }

            byte[] recCount = new byte[2];
            str.Read(recCount, 0, 2);
            ushort recordCount = BitConverter.ToUInt16(recCount, 0);

            if (recordCount == 0 || recordCount > MaxRecordCount)
            {
                //Debugger.Break();
                return null;
                //throw new FormatException($"File has {recordCount} records.");
            }

            Dictionary<string, uint> nameOffsetPairs = new Dictionary<string, uint>(recordCount);

            while (str.Position < (recordCount) * nameSize)
            {
                byte[] b_name = new byte[nameSize];
                byte[] b_offset = new byte[OffsetSize];
                string name = string.Empty;
                uint offset;

                str.Read(b_name, 0, nameSize); //offset is 0 from ms.Position
                str.Read(b_offset, 0, OffsetSize);

                name = Encoding.GetEncoding(1252).GetString(b_name).Trim('\0');
                offset = BitConverter.ToUInt32(b_offset, 0);

                if (offset > str.Length) //i_raw.res has only 12 records but will still hit on this one
                {
                    //testing for ResIdx vs all the other resources. 
                    //ResIdx.NameLen = 9, used by ImageRes, SetRes
                    //others have NameLen = 8
                    //Debugger.Break();
                    return null; 
                }

                try
                {
                    nameOffsetPairs.Add(name, offset);
                }
                catch (ArgumentException ae) //either key already exists or (ArgumentNullException) key is null
                {
                    string fileNameMsg = string.Empty;
                    if (str is FileStream)
                        fileNameMsg = $" Filename: {((FileStream) str).Name}";

                    Logger.TraceEvent(TraceEventType.Verbose, 0, $"Failed to read RESX definitions for {name} at offset {offset}. {fileNameMsg}");
                    return null;
                }
                
            }

            return nameOffsetPairs;
        }
    }
}
