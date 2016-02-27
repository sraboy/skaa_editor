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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using SkaaGameDataLib.Util;

namespace SkaaEditorUI.Presenters
{
    public class GameSetPresenter : PresenterBase<DataSet>
    {
        public GameSetPresenter()
        {
            this.GameObject = new DataSet();
        }

        /// <summary>
        /// Adds the specified <see cref="DataTable"/> to the <see cref="DataSet"/> (<see cref="PresenterBase{T}.GameObject"/>)
        /// and adds its data source to the DataSet's <see cref="DataSet.ExtendedProperties"/>. It then raises the <see cref="PresenterBase.PropertyChanged"/>
        /// event on "GameObject".
        /// </summary>
        /// <param name="dt"></param>
        public void AddTable(DataTable dt)
        {
            this.GameObject.AddDataTableFromNewSource(dt);
            OnPropertyChanged("GameObject");
        }
        public bool RemoveTable(string tableName)
        {
            var t = this.GameObject.Tables[tableName];
            if (t != null)
            {
                List<string> datasources = new List<string>();

                foreach (DictionaryEntry p in t.ExtendedProperties)
                    this.GameObject.RemoveDataSource(p.Value.ToString());

                this.GameObject.Tables.Remove(t);
                OnPropertyChanged("GameObject");
                return true;
            }

            return false;
        }
        public void ExportToCSV() => this.GameObject.ExportGameSetToCSV();
        /// <summary>
        /// Loads a 7KAA-format SET file (e.g., std.set)
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="loadParam"></param>
        /// <returns>A new <see cref="DataSet"/> containing all the tables and records of the specified file</returns>
        public override DataSet Load(string filePath, params object[] loadParam)
        {
            DataSet ds = new DataSet();

            if (ds.OpenStandardGameSet(filePath) == false)
                return null;

            this.GameObject = ds;

            return this.GameObject;
        }
        public void Merge(GameSetPresenter gsp)
        {
            this.GameObject.Merge(gsp.GameObject);
            OnPropertyChanged("GameObject");
        }
        public override bool Save(string filePath, params object[] param)
        {
            this.GameObject.SaveStandardGameSet(filePath);
            return true;
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".set";
            dlg.Filter = $"7KAA Set Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = "std";
        }
    }
}
