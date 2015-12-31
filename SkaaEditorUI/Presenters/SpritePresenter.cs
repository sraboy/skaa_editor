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
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class SpritePresenter : MultiImagePresenterBase
    {
        #region Private Members
        private string _spriteId;
        #endregion

        #region Constructors
        public SpritePresenter() { }
        [Obsolete("This is for the old project class and should no longer be used.")]
        public SpritePresenter(SkaaSprite spr)
        {
            throw new NotImplementedException();
        }
        #endregion

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

            SetIFrames();
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

            SetIFrames();
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            dlg.DefaultExt = ".spr";
            dlg.Filter = $"7KAA Sprite Files (*{dlg.DefaultExt})|*{dlg.DefaultExt}|All Files (*.*)|*.*";
            dlg.FileName = this.SpriteId ?? null;
        }


    }
}
