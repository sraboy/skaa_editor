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
using System.Data;
using System.IO;
using System.Windows.Forms;
using SkaaGameDataLib.Util;

namespace SkaaEditorUI.Presenters
{
    class DbfFilePresenter : PresenterBase<DataTable>
    {
        public override DataTable Load(string filePath, params object[] param)
        {
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                DbfFile file = new DbfFile();

                if (file.ReadStream(fs) != true)
                    throw new Exception("Failed to read DBF file.");

                file.DataTable.TableName = Path.GetFileNameWithoutExtension(filePath);
                file.DataTable.ExtendedProperties.Add("FileName", filePath);
                dt = file.DataTable;
            }

            return dt;
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            throw new NotImplementedException();
        }
    }
}
