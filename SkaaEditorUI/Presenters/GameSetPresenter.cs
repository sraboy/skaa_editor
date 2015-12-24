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
