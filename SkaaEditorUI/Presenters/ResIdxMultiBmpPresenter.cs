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
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SkaaGameDataLib.GameObjects;
using SkaaGameDataLib.Util;

namespace SkaaEditorUI.Presenters
{
    public class ResIdxMultiBmpPresenter : MultiImagePresenterBase
    {
        public static readonly TraceSource Logger = new TraceSource($"{typeof(ResIdxMultiBmpPresenter)}", SourceLevels.All);

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
        /// directly in its header rather than in the standard game set (see <see cref="GameSetFile"/>. The 
        /// <see cref="DataTable"/> only has fields for FrameName and FrameOffset; any other data is stored 
        /// elsewhere, generally in DBF (dBaseIII) files that may or may not be in the standard game set.
        /// </remarks>
        private static Tuple<SkaaSprite, DataTable> ReadFrames(string filepath, ColorPalette pal, bool offsetsOnly)
        {
            SkaaSprite spr = new SkaaSprite();
            DataTable dt = new DataTable();
            dt.AddDataSource(Path.GetFileName(filepath));
            dt.Columns.Add(new DataColumn() { DataType = typeof(string), ColumnName = SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameNameColumn });
            dt.Columns.Add(new DataColumn() { DataType = typeof(uint), ColumnName = SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameOffsetColumn });

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                //Read the file definitions from the ResIdx header.
                Dictionary<string, uint> dic = ResourceDefinitionReader.ReadDefinitions(fs, offsetsOnly);
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
                    iBmp.SetBitmapFromRleStream(fs, FileFormats.ResIdxFramesSpr);

                    spr.Frames.Add(sf);

                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);
                    row.BeginEdit();
                    row[SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameNameColumn] = key;
                    row[SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameOffsetColumn] = dic[key];
                    row.AcceptChanges();
                }
            }

            return new Tuple<SkaaSprite, DataTable>(spr, dt);
        }

        #region Overridden Public Methods
        public override SkaaSprite Load(string filePath, params object[] param)
        {
            GameSetPresenter gsp = null;

            try
            {
                gsp = (GameSetPresenter)param[0];
                if (gsp == null)
                    throw new ArgumentNullException();
            }
            catch (Exception e)
            {
                string errorMsg;

                if (e is IndexOutOfRangeException)      //param wasn't passed at all
                {
                    errorMsg = $"You must pass a non-null {typeof(GameSetPresenter)} so the ResIdx's {typeof(DataSet)} can be set.";
                    Logger.TraceEvent(TraceEventType.Error, 0, errorMsg);
                    throw new IndexOutOfRangeException(errorMsg);
                }
                else if (e is ArgumentNullException)     //thrown above
                {
                    errorMsg = $"You must pass a non-null {typeof(GameSetPresenter)} so the ResIdx's {typeof(DataSet)} can be set.";
                    Logger.TraceEvent(TraceEventType.Error, 0, errorMsg);
                    throw new ArgumentNullException(errorMsg);
                }
                else
                {
                    Logger.TraceEvent(TraceEventType.Error, 0, e.Message);
                    throw e;
                }
            }

            //hack: This is for files like i_raw.res which are just like normal ResIdx files, 
            //except they have no name records, just offsets. We end up getting another params
            //array that was passed in to this params.
            bool offsetsOnly = false;
            if (param.Length >= 2)
            {
                var arr = (object[])param[1];
                offsetsOnly = (bool)arr[0];
            }

            Tuple<SkaaSprite, DataTable> tup = ReadFrames(filePath, this.PalettePresenter.GameObject, offsetsOnly);

            gsp.GameObject = gsp.GameObject ?? new DataSet();
            DataSet ds = gsp.GameObject;

            if (ds.Tables.Contains(tup.Item2.TableName))
            {
                var newName = tup.Item2.TableName + "_2";
                tup.Item1.SpriteId = newName;
                tup.Item2.TableName = newName;
            }

            gsp.AddTable(tup.Item2);

            this.GameObject = tup.Item1;
            this.SpriteId = this.GameObject.SpriteId;

            BuildFramePresenters();
            return this.GameObject;
        }
        public override bool Save(string filePath, params object[] param)
        {
            //7KAA expects the definitions to be sorted low-to-high by offset
            this.DataView.Sort = SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameOffsetColumn;

            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Create))
                {
                    var table = this.DataView.ToTable();
                    var imageData = this.GetSpriteStream();

                    //write the record count
                    byte[] recordcount = BitConverter.GetBytes((short)table.Rows.Count);
                    fs.Write(recordcount, 0, recordcount.Length);

                    //write the ResIdx header
                    table.WriteAllRowsAsResDefinitions(fs);

                    //write an empty record to signify the file's size follows
                    for (int i = 0; i < ResourceDefinitionReader.ResIdxNameSize; i++)
                        fs.WriteByte(0x0);

                    //write the file's size
                    uint fileSize = (uint)(fs.Position + imageData.Length);
                    fileSize += 4; //we haven't written the four bytes of the file size yet, so fs.Position doesn't include it
                    byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                    fs.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                    //write the frame data
                    imageData.CopyTo(fs);
                }
            }
            catch (Exception e)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, e.Message);
                return false;
            }

            return true;
        }
        public override bool SetSpriteDataView(GameSetPresenter gsp)
        {
            DataView dv;

            dv = new DataView(gsp.GameObject?.Tables?[this.SpriteId]);
            this.DataView = dv;

            return this.GameObject.SetSpriteDataView(dv);
        }
        public override void RecalculateFrameOffsets()
        {
            if (this.DataView?.Table == null && !this.BitmapHasChanges)
                return;

            //calculate offset after file header
            long offset = 0;
            int definitionsSize = (this.DataView.Count + 1) * ResourceDefinitionReader.ResIdxDefinitionSize; //we add one for the empty record containing the file's size
            offset += 2; //the record count at the beginning of the file
            offset += definitionsSize;

            foreach (FramePresenter fp in this.Frames)
            {
                //sometimes, it gets lost due to working with 32-bit bitmaps
                fp.GameObject.IndexedBitmap.Bitmap.Palette = this.PalettePresenter.GameObject;

                //recalculate offset
                var bytes = fp.GameObject.GetSprBytes();
                fp.BitmapOffset = offset;

                //update the DataView
                if (this.DataView?.Table != null)
                {
                    this.DataView.Sort = SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameNameColumn;
                    var dr = this.DataView[this.DataView.Find(fp.Name)];
                    dr[SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameOffsetColumn] = fp.BitmapOffset;
                }

                //ResIdxMultiBmp doesn't include the four-byte size header that IndexedBitmap.GetRleBytesFromBitmap() includes for SPR files
                offset += bytes.Length - 4;
            }

            this.BitmapHasChanges = false;
        }
        #endregion

        #region Overridden Protected Methods
        /// <summary>
        /// Creates and returns a <see cref="MemoryStream"/> containing <see cref="SkaaFrame"/> data for 
        /// all <see cref="IFrame"/> objects in <see cref="Frames"/>. The first four bytes are ignored
        /// as they are the file's size, which is not used in ResIdx files.
        /// The <see cref="MemoryStream.Position"/> is reset to 0 before returning.
        /// </summary>
        protected override MemoryStream GetSpriteStream()
        {
            var str = new MemoryStream();
            var sprBytes = this.GameObject.GetSpriteFrameByteArrays();

            foreach (byte[] ba in sprBytes)
                str.Write(ba, 4, ba.Length - 4);

            str.Position = 0;

            return str;
        }
        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".res";
            dlg.Filter = $"7KAA Resource Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.SpriteId ?? null;
        }
        protected override void AddNewFrameDataRow(FramePresenter fr)
        {
            //Create a new DataRow for the new frame
            var dr = this.DataView.Table.NewRow();
            dr.BeginEdit();
            dr[SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameNameColumn] = fr.Name;
            dr[SkaaGameDataLib.Util.DataRowExtensions.ResIdxFrameOffsetColumn] = fr.BitmapOffset;
            dr.EndEdit();
            this.DataView.Table.Rows.Add(dr);
        }
        #endregion
    }
}
