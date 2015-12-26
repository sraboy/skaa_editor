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
using System.Diagnostics;
using System.Drawing.Imaging;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class ColorPalettePresenter : PresenterBase<ColorPalette>
    {
        private static readonly Dictionary<string, string> _fileTypes = new Dictionary<string, string>() { { "Skaa General Palette", ".res" }, { "Skaa Encyclopedia Palette", ".col" } };
        protected override Dictionary<string, string> FileTypes
        {
            get
            {
                return _fileTypes;
            }
        }

        public ColorPalettePresenter() { }
        public ColorPalettePresenter(ColorPalette pal)
        {
            this.GameObject = pal;
        }

        public override ColorPalette Load(string filePath, params object[] param)
        {
            var pal = PaletteLoader.FromResFile(filePath);

            if (pal == null)
                Logger.TraceEvent(TraceEventType.Error, 0, $"Failed to load palette: {filePath}");

            return pal;
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}
