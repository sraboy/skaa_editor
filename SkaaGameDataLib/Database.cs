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
        //[field: NonSerialized]
        //private EventHandler _databaseUpdated;
        //public event EventHandler DatabaseUpdated
        //{
        //    add
        //    {
        //        if (_databaseUpdated == null || !_databaseUpdated.GetInvocationList().Contains(value))
        //        {
        //            _databaseUpdated += value;
        //        }
        //    }
        //    remove
        //    {
        //        _databaseUpdated -= value;
        //    }
        //}
        //protected virtual void OnDatabaseUpdated(EventArgs e)
        //{
        //    EventHandler handler = _databaseUpdated;

        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        //Lifted from ORESX.h
        public string Name; //char[9] in the file
        public int Offset;
        public DataTable Table;
        //public bool hasChanges;

        public DatabaseContainer()
        {

        }
    }
}
