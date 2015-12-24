using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    public class ColorPalettePresenter : IPresenter
    {
        private static readonly string ResFileExtension = ".res";
        private static readonly string ColFileExtension = ".col";

        private ColorPalette _colorPalette;

        public ColorPalette ColorPalette
        {
            get
            {
                return _colorPalette;
            }

            set
            {
                this._colorPalette = value;
            }
        }

        public string FileExtension
        {
            get
            {
                return null;
            }
        }

        public ColorPalettePresenter(ColorPalette pal)
        {
            this.ColorPalette = pal;
        }

        public OpenFileDialog GetOpenFileDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = $"7KAA Palette Files (*{ResFileExtension}) (*{ColFileExtension})|*{ResFileExtension};*{ColFileExtension}| All Files (*.*)|*.*";
            dlg.DefaultExt = ResFileExtension;
            return dlg;
        }
    }
}
