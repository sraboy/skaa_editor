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
        private string _dataTableName;

        public override SkaaSprite Load(string filePath, params object[] param)
        {
            Tuple<SkaaSprite, DataTable> tup = ReadFrames(filePath, this.PalettePresenter.GameObject);
            GameSetPresenter gsp = null;

            try
            {
                gsp = (GameSetPresenter)param[0];
                if (gsp == null)
                    throw new ArgumentNullException();
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException)      //param wasn't passed at all
                    throw new IndexOutOfRangeException($"You must pass a non-null {typeof(GameSetPresenter)} so the ResIdx's {typeof(DataSet)} can be set.");
                else if (e is ArgumentNullException)     //thrown above
                    throw new ArgumentNullException($"You must pass a non-null {typeof(GameSetPresenter)} so the ResIdx's {typeof(DataSet)} can be set.");
                else
                    throw e;
            }

            gsp.GameObject = gsp.GameObject ?? new DataSet();
            DataSet ds = ((GameSetPresenter)param[0]).GameObject;

            ds.Tables.Add(tup.Item2);
            ds.AddDataSource(Path.GetFileName(filePath));
            this.GameObject = tup.Item1;
            this._dataTableName = tup.Item2.TableName;
            SetIFrames();
            return this.GameObject;
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        public override void SetSpriteDataView(GameSetPresenter gsp)
        {
            DataView dv;

            dv = new DataView(gsp.GameObject?.Tables?[_dataTableName]);

            this.DataView = dv;
            this.GameObject.SetSpriteDataView(dv);

            //this.Frames = BuildFramePresenters();
        }

        /// <summary>
        /// Reads all the frame data from the specified file, of type <see cref="FileFormats.ResIdxMultiBmp"/>
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        /// <param name="pal">The <see cref="ColorPalette"/> to use in building the frame's image</param>
        /// <returns>
        /// A <see cref="Tuple{T1, T2}"/> where <c>T1</c> is the new <see cref="SkaaSprite"/> and <c>T2</c> is
        /// the <see cref="DataTable"/> read from the ResIdx header.
        /// </returns>
        /// <remarks>
        /// This differs from loading an SPR file in that the file contains the <see cref="SkaaFrame"/> data
        /// directly in its header rather than in the standard game set. The <see cref="DataTable"/> only has
        /// fields for FrameName and FrameOffset; any other data is stored in other dBaseIII files.
        /// </remarks>
        private static Tuple<SkaaSprite, DataTable> ReadFrames(string filepath, ColorPalette pal)
        {
            SkaaSprite spr = new SkaaSprite();
            DataTable dt = new DataTable();
            dt.ExtendedProperties.Add(SkaaGameDataLib.DataTableExtensions.DataSourcePropertyName, filepath);
            dt.Columns.Add(new DataColumn() { DataType = typeof(string), ColumnName = SkaaGameDataLib.DataRowExtensions.FrameNameColumn });
            dt.Columns.Add(new DataColumn() { DataType = typeof(uint), ColumnName = SkaaGameDataLib.DataRowExtensions.FrameOffsetColumn });

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                //Read the file definitions from the ResIdx header.
                Dictionary<string, uint> dic = ResourceDatabase.ReadDefinitions(fs, true);
                spr.SpriteId = Path.GetFileNameWithoutExtension(filepath);
                dt.TableName = spr.SpriteId;

                foreach (string key in dic.Keys)
                {
                    SkaaSpriteFrame sf = new SkaaSpriteFrame(null);
                    sf.BitmapOffset = dic[key];
                    fs.Position = sf.BitmapOffset;
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

        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".res";
            dlg.Filter = $"7KAA Resource Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.SpriteId ?? null;
        }
    }
}
