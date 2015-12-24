using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkaaEditorUI.Utilities
{
    public static class FileDialogExtensions
    {
        public static T ShowDialog<T>(this OpenFileDialog dlg, Func<T> loadFileDelegate) where T : class
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return loadFileDelegate();
            }
            else
                return null;
        }
    }
}
