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
