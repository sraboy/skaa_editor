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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaGameDataLib;
using SkaaEditorUI.Utilities;

namespace SkaaEditorUI.Presenters
{
    public abstract class PresenterBase<T> where T : class, new()
    {
        private Dictionary<string, string> fileTypes;// = new Dictionary<string, string>() { };
        private static OpenFileDialog _dlg;
        public static implicit operator T(PresenterBase<T> obj) { return obj.GameObject; }

        protected string FileDialogFilter { get { return GetFileDialogFilter(FileTypes); } }
        protected abstract Dictionary<string, string> FileTypes { get; }

        public T GameObject { get; set; }

        //Passed as a delegate in Open() so derived types must implement
        protected abstract T Load(string filePath, params object[] param);

        static PresenterBase()
        {
            _dlg = new OpenFileDialog();
        }

        public string GetFileDialogFilter(Dictionary<string, string> fileTypeDic)
        {
            string fileTypes = "7KAA";
            string fileExtensions = "";
            string allFiles = "All Files (*.*)|*.*";

            foreach (KeyValuePair<string, string> kv in )
            {
                fileTypes += kv.Key + $" Files (*{kv.Value})|";
                fileExtensions += kv.Value + '|';
            }
            
            string filter = $"{fileTypes + fileExtensions}|{allFiles}";
            return filter;
        }

        public T Open(object loadParam = null)
        {
            //T obj;
            var dlg = new OpenFileDialog();
            this.GameObject = dlg.ShowDialog<T>(() => this.Load(dlg.FileName, loadParam));
            return this.GameObject;
        }
    }
}
