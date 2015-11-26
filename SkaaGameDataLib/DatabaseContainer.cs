using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    /// <summary>
    /// Represents the rows in the header of a SET file and the databases they refer to. 
    /// </summary>
    [Serializable]
    public class DatabaseContainer
    {
        //Lifted from ORESX.h
        public string Name; //char[9] in std.set
        public int Offset;
        public DataTable Table;
        public DbfFile DbfFileObject;

        public DatabaseContainer()
        {

        }
    }
}
