using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkaaEditorUI.Utilities
{
    public static class FileDialogExtensions
    {
        public static T Open<T>(this OpenFileDialog dlg, string initialDirectory, Func<T> openFileMethod) where T : class
        {
            T obj = null;
            obj = ShowDialog(dlg, openFileMethod);
            return obj;
        }

        //public static void OpenColorPalette(this OpenFileDialog dlg, string initialDirectory, Action openFileMethod)
        //{
        //    dlg.InitialDirectory = initialDirectory;
        //    dlg.Filter = $"7KAA Palette Files (*{ResFileExtension}) (*{ColFileExtension})|*{ResFileExtension};*{ColFileExtension}| All Files (*.*)|*.*";
        //    dlg.DefaultExt = ResFileExtension;
        //    ShowDialog(dlg, openFileMethod);
        //}

        //public static void OpenSprite(this OpenFileDialog dlg, string initialDirectory, Action openFileMethod)
        //{
        //    dlg.InitialDirectory = initialDirectory;
        //    dlg.Filter = $"7KAA Sprite Files (*.spr)|*{SprFileExtension}|All Files (*.*)|*.*";
        //    dlg.DefaultExt = SprFileExtension;
        //    ShowDialog(dlg, openFileMethod);
        //}

        private static T ShowDialog<T>(OpenFileDialog dlg, Func<T> openFileMethod) where T : class
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return openFileMethod();
            }
            else
                return null;
        }
    }
}
