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
