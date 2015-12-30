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
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class ResIdxMultiBmpPresenter : MultiImagePresenterBase
    {
        private DataTable _dataTable;

        public DataTable DataTable
        {
            get
            {
                return _dataTable;
            }

            private set
            {
                this._dataTable = value;
            }
        }

        public override SkaaSprite Load(string filePath, params object[] param)
        {
            Tuple<SkaaSprite, DataTable> tup = ReadFrames(filePath, this.PalettePresenter.GameObject);
            //DataSet ds = new DataSet();
            //ds.Tables.Add(tup.Item2);
            //ds.AddDataSource(Path.GetFileName(filePath));
            this.GameObject = tup.Item1;
            this.DataTable = tup.Item2;
            this.Frames = BuildFramePresenters();
            return this.GameObject;
        }

        private static Tuple<SkaaSprite, DataTable> ReadFrames(string filepath, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
            DataTable dt = new DataTable();
            dt.ExtendedProperties.Add(SkaaGameDataLib.DataTableExtensions.DataSourcePropertyName, filepath);
            dt.Columns.Add(new DataColumn() { DataType = typeof(string), ColumnName = SkaaGameDataLib.DataRowExtensions.FrameNameColumn });
            dt.Columns.Add(new DataColumn() { DataType = typeof(uint), ColumnName = SkaaGameDataLib.DataRowExtensions.FrameOffsetColumn });

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                Dictionary<string, uint> dic = ResourceDatabase.ReadDefinitions(fs, true);
                spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
                dt.TableName = spr.SpriteId;

                foreach (string key in dic.Keys)
                {
                    fs.Position = dic[key];
                    SkaaSpriteFrame sf = new SkaaSpriteFrame(null);
                    sf.Name = key;
                    IndexedBitmap iBmp = new IndexedBitmap(pal);
                    sf.IndexedBitmap = iBmp;
                    iBmp.SetBitmapFromRleStream(fs, FileFormats.SpriteFrameSpr);

                    spr.Frames.Add(sf);

                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);
                    row.BeginEdit();
                    row[SkaaGameDataLib.DataRowExtensions.FrameNameColumn] = key;
                    row[SkaaGameDataLib.DataRowExtensions.FrameOffsetColumn] = dic[key];
                    row.AcceptChanges();
                }
            }

            return new Tuple<SkaaSprite, DataTable>(spr, dt);
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".res";
            dlg.Filter = $"7KAA Resource Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.SpriteId ?? null;
        }
    }
}
