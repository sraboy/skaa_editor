using System;
using System.Windows.Forms;

namespace SkaaEditorUI.Misc
{
    public class PresenterFileDialogEventArgs<T> : EventArgs where T : class
    {
        /// <summary>
        /// The results from a FileDialog like OpenFileDialog or SaveFileDialog
        /// </summary>
        public DialogResult FileDialogResults { get; set; }
        T LoadedObject { get; set; }
    }
}
