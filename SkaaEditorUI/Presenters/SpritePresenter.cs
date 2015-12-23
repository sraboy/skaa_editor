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

namespace SkaaEditorUI.Presenters
{
    public class SpritePresenter : SkaaSprite, ICustomOpenFileDialog, INotifyPropertyChanged
    {
        private static readonly string _fileExtension = ".spr";

        private IFrame _activeFrame;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


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

        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }
        }



        public SpritePresenter() { }
        public SpritePresenter(SkaaSprite sgs)
        {
            this.Frames = sgs.Frames;
            this.SpriteId = sgs.SpriteId;
            this.ActiveFrame = this.GetIFrames()[0];
        }

        public List<IFrame> GetIFrames()
        {
            List<IFrame> frames = new List<IFrame>();// = (SkaaEditorFrame)this.Frames[0];
            foreach (SkaaFrame sf in this.Frames)
            {
                frames.Add(new FramePresenter(sf));
            }

            return frames;
        }

        public OpenFileDialog GetOpenFileDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = $"7KAA Sprite Files (*{FileExtension})|*{FileExtension}|All Files (*.*)|*.*";
            dlg.DefaultExt = FileExtension;
            return dlg;
        }
    }
}
