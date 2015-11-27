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
        public const int DefinitionNameSize = 9;   //8 chars + null
        public const int DefinitionOffsetSize = 4; //uint32
        public const int DefinitionSize = 13;      //add the above two 

        /// <summary>
        /// Reads a the header of a stream containing char[9], uint32 data: a datatable's name and its offset in a file
        /// </summary>
        /// <param name="str">The stream from which to read. The first two bytes must be a uint16 containing the number of definitions</param>
        /// <returns></returns>
        public static Dictionary<string, uint> ReadDefinitions(Stream str)
        {
            byte[] recCount = new byte[2];
            str.Read(recCount, 0, 2);
            ushort recordCount = BitConverter.ToUInt16(recCount, 0);
            Dictionary<string, uint> nameOffsetPairs = new Dictionary<string, uint>(recordCount);

            while (str.Position < (recordCount) * DefinitionSize)
            {
                byte[] b_name = new byte[DefinitionNameSize];
                byte[] b_offset = new byte[4];
                string name = string.Empty;
                uint offset;

                str.Read(b_name, 0, DefinitionNameSize); //offset is 0 from ms.Position
                str.Read(b_offset, 0, 4);

                name = Encoding.GetEncoding(1252).GetString(b_name).Trim('\0');
                offset = BitConverter.ToUInt32(b_offset, 0);

                nameOffsetPairs.Add(name, offset);
            }

            return nameOffsetPairs;
        }
        public static void WriteDefinition(this DataTable dt, Stream str, uint offset)
        {
            str.Write(Encoding.GetEncoding(1252).GetBytes(dt.TableName.PadRight(DefinitionNameSize, (char) 0x0)), 0, DefinitionNameSize);
            str.Write(BitConverter.GetBytes(Convert.ToUInt32(offset)), 0, DefinitionOffsetSize);
        }
    }
}
