using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public interface ICustomOpenFileDialog
    {
        string FileExtension { get; }
        OpenFileDialog GetOpenFileDialog();
    }
}
