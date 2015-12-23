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
using System.Data;

namespace SkaaGameDataLib
{
    public class SkaaSpriteFrame : SkaaFrame
    {
        private SkaaSprite _parentSprite;
        public string Test = "test";
        public string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }
        public long BitmapOffset
        { 
            get
            {
                return base.BitmapOffset;
            }
            set
            {
                base.BitmapOffset = value;
            }
        }
   
        public SkaaSprite ParentSprite;
        public List<DataRow> GameSetDataRows;

        /// <summary>
        /// Initializes a new <see cref="SkaaSpriteFrame"/>.
        /// </summary>
        /// <param name="parentSprite">The <see cref="SkaaSprite"/> containing this <see cref="SkaaSpriteFrame"/></param>
        /// <param name="stream"></param>
        public SkaaSpriteFrame(SkaaSprite parentSprite)
        {
            this.ParentSprite = parentSprite;
            this.GameSetDataRows = new List<DataRow>();
        }
    }
}