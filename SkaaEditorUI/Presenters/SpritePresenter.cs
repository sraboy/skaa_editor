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
using System.Collections.Generic;
using SkaaGameDataLib;
using Capslock.WinForms.SpriteViewer;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq.Expressions;
using System;
using System.IO;
using SkaaEditorUI.Misc;

namespace SkaaEditorUI.Presenters
{
    public class SpritePresenter : PresenterBase<SkaaSprite>
    {
        private static readonly Dictionary<string, string> _fileTypes = new Dictionary<string, string>() { { "SpriteSpr", ".spr" }, { "SpriteRes", ".res" } };

        #region Private Members
        private IFrame _activeFrame;
        private string _spriteId;
        private TrulyObservableCollection<FramePresenter> _frames = new TrulyObservableCollection<FramePresenter>();
        #endregion

        #region Public Members
        public ColorPalette Palette
        {
            get
            {
                return this.ActiveFrame?.Bitmap?.Palette;
            }
        }
        public IFrame ActiveFrame
        {
            get
            {
                return _activeFrame;
            }
            set
            {
                SetField(ref this._activeFrame, value, () => OnPropertyChanged(GetDesignModeValue(() => this.ActiveFrame)));
            }
        }
        public string SpriteId
        {
            get
            {
                return this.GameObject.SpriteId;
            }
            set
            {
                SetField(ref this._spriteId, value, () => OnPropertyChanged(GetDesignModeValue(() => this.SpriteId)));
            }
        }
        public TrulyObservableCollection<FramePresenter> Frames
        {
            get
            {
                return this._frames;
            }
            set
            {
                SetField(ref this._frames, value, () => OnPropertyChanged(GetDesignModeValue(() => this.Frames)));
            }
        }
        #endregion

        protected override Dictionary<string, string> FileTypes
        {
            get
            {
                return _fileTypes;
            }
        }

        #region Constructors
        public SpritePresenter()
        {

        }
        [Obsolete("This is for the old project class and should no longer be used.")]
        public SpritePresenter(SkaaSprite spr)
        {
            throw new NotImplementedException();
            this.GameObject = spr;
            this.ActiveFrame = (IFrame)spr.Frames[0];
            SetFrames();
        }
        #endregion

        private void SetFrames()
        {
            foreach (SkaaFrame f in this.GameObject.Frames)
            {
                var fp = new FramePresenter(f);
                this.Frames.Add(fp);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }
        //private List<IFrame> GetIFrames()
        //{
        //    List<IFrame> frames = new List<IFrame>();
        //    foreach (SkaaFrame sf in frames)
        //    {
        //        frames.Add(new FramePresenter(sf));
        //    }
        //    return frames;
        //}

        /// <summary>
        /// Creates a <see cref="SkaaSprite"/> object from an SPR-formatted file
        /// </summary>
        /// <param name="filePath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SkaaSprite"/></returns>
        /// <remarks>
        protected override SkaaSprite Load(string filePath, params object[] param)
        {
            ColorPalette pal = (param[0] as object[])[0] as ColorPalette;

            if (pal == null)
                throw new ArgumentNullException("param", "You must specify a ColorPalette to load a sprite.");

            SkaaSprite spr;

            using (FileStream spritestream = File.OpenRead(filePath))
                spr = SkaaSprite.FromSprStream(spritestream, pal);

            this.GameObject = spr;
            SetFrames();
            return this.GameObject;
        }

        public void SetActiveFrame(int index)
        {
            this.ActiveFrame = this.Frames?[index];
        }
        public Stream GetActiveSpriteStream()
        {
            var spr = new MemoryStream();
            var sprBytes = this.GameObject.GetSpriteFrameByteArrays();
            foreach (byte[] ba in sprBytes)
                spr.Write(ba, 0, ba.Length);

            return spr;
        }
    }
}
