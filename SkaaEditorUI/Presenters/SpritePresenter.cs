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
using SkaaGameDataLib.GameObjects;

namespace SkaaEditorUI.Presenters
{
    public class SpritePresenter : MultiImagePresenterBase
    {
        #region Constructors
        public SpritePresenter() { }
        #endregion

        #region Overridden Public Methods
        /// <summary>
        /// Creates a <see cref="SkaaSprite"/> object from an SPR-formatted file
        /// </summary>
        /// <param name="filePath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SkaaSprite"/></returns>
        public override SkaaSprite Load(string filePath, params object[] param)
        {
            SkaaSprite spr;

            using (FileStream fs = File.OpenRead(filePath))
                spr = SkaaSprite.FromSprStream(fs, this.PalettePresenter.GameObject);

            this.GameObject = spr;
            this.SpriteId = spr.SpriteId;

            BuildFramePresenters();
            return this.GameObject;
        }
        public override bool Save(string filePath, params object[] param)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                this.GetSpriteStream().CopyTo(fs);
            }

            return true;
        }
        public override void SetSpriteDataView(GameSetPresenter gsp)
        {
            DataView dv;

            dv = new DataView(gsp.GameObject?.Tables?["SFRAME"]);
            dv.RowFilter = $"SPRITE = '{this.SpriteId.ToUpper()}'";

            this.DataView = dv;
            this.GameObject.SetSpriteDataView(dv);

            UpdateFrameNamesAndOffsets();
        }
        public override void RecalculateFrameOffsets()
        {
            if (this.DataView == null || !this.BitmapHasChanges)
                return;

            long offset = 0;

            foreach (FramePresenter fp in this.Frames)
            {
                //recalculate offset
                var bytes = fp.GameObject.GetSprBytes();
                fp.BitmapOffset = offset;
                offset += bytes.LongLength;

                //update the DataView
                this.DataView.Sort = SkaaGameDataLib.Util.DataRowExtensions.SprFrameNameColumn;

                var drv = this.DataView.FindRows(fp.Name);

                for (int i = 0; i < drv.Length; i++)
                {
                    drv[i].BeginEdit();
                    drv[i][SkaaGameDataLib.Util.DataRowExtensions.SprFrameOffsetColumn] = fp.BitmapOffset;
                    drv[i].EndEdit();
                }

            }

            this.BitmapHasChanges = false;
        }
        #endregion

        #region Overridden Protected Methods
        /// <summary>
        /// Creates and returns a <see cref="MemoryStream"/> containing <see cref="SkaaFrame"/> data for 
        /// all <see cref="IFrame"/> objects in <see cref="Frames"/>. The <see cref="MemoryStream.Position"/>
        /// is reset to 0 before returning.
        /// </summary>
        protected override MemoryStream GetSpriteStream()
        {
            var str = new MemoryStream();
            var sprBytes = this.GameObject.GetSpriteFrameByteArrays();

            foreach (byte[] ba in sprBytes)
                str.Write(ba, 0, ba.Length);

            str.Position = 0;

            return str;
        }
        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".spr";
            dlg.Filter = $"7KAA Sprite Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.SpriteId ?? null;
        }
        protected override void AddNewFrameDataRow(FramePresenter fr)
        {
            //todo: Need data for the other SFRAME columns
            throw new NotImplementedException();

            //Create a new DataRow for the new frame
            var dr = this.DataView.Table.NewRow();
            dr.BeginEdit();
            dr[SkaaGameDataLib.Util.DataRowExtensions.SprFrameNameColumn] = fr.Name;
            dr[SkaaGameDataLib.Util.DataRowExtensions.SprFrameOffsetColumn] = fr.BitmapOffset;
            dr.EndEdit();
            this.DataView.Table.Rows.Add(dr);
        }
        #endregion

        /// <summary>
        /// The underlying <see cref="SkaaSprite"/> updates its <see cref="SkaaFrame.Name"/> and <see cref="SkaaFrame.BitmapOffset"/>
        /// values based on the information in the <see cref="MultiImagePresenterBase.DataView"/> so we need to update ours as well.
        /// </summary>
        private void UpdateFrameNamesAndOffsets()
        {
            foreach (FramePresenter fp in this.Frames)
            {
                fp.Name = fp.GameObject.Name;
                fp.BitmapOffset = fp.GameObject.BitmapOffset;
            }
        }
    }
}
