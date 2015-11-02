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
    public class Database
    {
        //Lifted from ORESX.h
        //private char[] name;
        public string Name; //char[9] in the file
        public int Offset;
        public DataTable Table;

        public Database() { }
    }
}
