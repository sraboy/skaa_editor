using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public static class ResourceDatabaseWriter
    {
        public static readonly TraceSource Logger = new TraceSource("ResourceDatabaseWriter", SourceLevels.All);

        public static void WriteDefinition(this DataTable dt, Stream str, uint offset, bool isIdx)
        {
            int nameSize, definitionSize;

            if (isIdx)
            {
                nameSize = ResourceDatabase.ResIdxNameSize;
                definitionSize = ResourceDatabase.ResIdxDefinitionSize;
            }
            else
            {
                nameSize = ResourceDatabase.ResNameSize;
                definitionSize = ResourceDatabase.ResDefinitionSize;
            }

            string recordName = dt.TableName.PadRight(nameSize, (char) 0x0);
            byte[] record_name = new byte[nameSize];
            record_name = Encoding.GetEncoding(1252).GetBytes(recordName);
            str.Write(record_name, 0, nameSize);

            byte[] record_size = new byte[ResourceDatabase.OffsetSize];
            record_size = BitConverter.GetBytes(Convert.ToUInt32(offset));
            str.Write(record_size, 0, ResourceDatabase.OffsetSize);
        }

        public static void WriteDefinition(this DataRow dr, Stream str, uint offset, bool isIdx)
        {
            int nameSize, definitionSize;

            if (isIdx)
            {
                nameSize = ResourceDatabase.ResIdxNameSize;
                definitionSize = ResourceDatabase.ResIdxDefinitionSize;
            }
            else
            {
                nameSize = ResourceDatabase.ResNameSize;
                definitionSize = ResourceDatabase.ResDefinitionSize;
            }

            string recordName = dr["FrameName"].ToString().PadRight(nameSize, (char) 0x0);
            byte[] record_name = new byte[nameSize];
            record_name = Encoding.GetEncoding(1252).GetBytes(recordName);
            str.Write(record_name, 0, nameSize);

            byte[] record_size = new byte[ResourceDatabase.OffsetSize];
            record_size = BitConverter.GetBytes(Convert.ToUInt32(offset));
            str.Write(record_size, 0, ResourceDatabase.OffsetSize);
        }
    }
}
