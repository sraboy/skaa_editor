using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace SkaaEditorUI.Presenters
{
    public class GameSetPresenter : PresenterBase<DataSet>
    {
        //protected static readonly new string _fileExtension = ".set";
        private static readonly Dictionary<string, string> _fileTypes = new Dictionary<string, string>() { { "Game Set", ".std" } };

        protected override Dictionary<string, string> FileTypes
        {
            get
            {
                return _fileTypes;
            }
        }

        /// <summary>
        /// Loads a 7KAA-format SET file (e.g., std.set)
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="merge">Whether to merge the loaded <see cref="DataSet"/> with the current <see cref="GameObject"/></param>
        /// <returns>A new <see cref="DataSet"/> containing all the tables and records of the specified file</returns>
        protected override DataSet Load(string filePath, params object[] param)
        {
            bool merge = (bool)param[0];

            DataSet ds = new DataSet();

            if (ds.OpenStandardGameSet(filePath) == false)
                return null;

            if (merge)
                this.GameObject?.Merge(ds);
            else
                this.GameObject = ds;

            return this.GameObject;
        }
    }
}
