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

namespace SkaaEditorUI.Presenters
{
    public class SpritePresenter : PresenterBase<SkaaSprite>, INotifyPropertyChanged
    {
        private static readonly Dictionary<string, string> _fileTypes = new Dictionary<string, string>() { { "Sprite", ".spr" }, { "Sprite", ".res" } };

        private IFrame _activeFrame;
        private static readonly SkaaSprite _skaaSprite = new SkaaSprite();

        #region PropertyChangedEvent
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Public Members
        public ColorPalette ActivePalette
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
                Misc.SetField(ref this._activeFrame, value, () => OnPropertyChanged(Misc.GetDesignModeValue(() => this.ActiveFrame)));
                this._activeFrame = value;
            }
        }

        protected override Dictionary<string, string> FileTypes
        {
            get
            {
                return _fileTypes;
            }
        }
        #endregion

        #region Constructors
        public SpritePresenter() { }
        public SpritePresenter(SkaaSprite spr)
        {
            this.ActiveFrame = (IFrame)spr.Frames[0];
        }
        #endregion

        public TrulyObservableCollection<IFrame> GetIFrames()
        {
            TrulyObservableCollection<IFrame> frames = new TrulyObservableCollection<IFrame>();// = (SkaaEditorFrame)this.Frames[0];
            foreach (SkaaFrame sf in this.Frames)
            {
                frames.Add(new FramePresenter(sf));
            }

            return frames;
        }

        /// <summary>
        /// Creates a <see cref="SpritePresenter"/> object from an SPR-formatted file
        /// </summary>
        /// <param name="filePath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SpritePresenter"/></returns>
        /// <remarks>
        protected SpritePresenter Load(string filePath, ColorPalette pal)
        {
            if (pal == null)
                throw new ArgumentNullException("pal", "You must specify a ColorPalette to load a sprite.");

            SkaaSprite spr;

            using (FileStream spritestream = File.OpenRead(filePath))
                spr = SkaaSprite.FromSprStream(spritestream, pal);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filePath);

            return new SpritePresenter(spr);
        }
    }
}
