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
        private static readonly string SprFileExtension = ".spr";

        public static void OpenAs<SpritePresenter>(this OpenFileDialog dlg, string initialDirectory, Action openFileMethod)
        {
            dlg.InitialDirectory = initialDirectory;//ProjectManager.SaveDirectory;
            dlg.Filter = $"7KAA Sprite Files (*.spr)|*{SprFileExtension}|All Files (*.*)|*.*";
            dlg.DefaultExt = SprFileExtension;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                openFileMethod();
            }


            //switch (format)
            //{
            //    case FileFormats.GameSet: //set file
            //        dlg.Filter = $"7KAA Game Set Files (*.set)|*{props.SetFileExtension}|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.SetFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () => this.ActiveProject.OpenGameSet(dlg.FileName));
            //        break;
            //    //case FileFormat.SpritePNG:
            //    //    dlg.DefaultExt = ".png";
            //    //    dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
            //    //    dlg.FileName = filepath;
            //    //    //ShowOpenFileDialog(dlg, format, () => this.ActiveProject.LoadSprite(dlg.FileName));
            //    //    break;
            //    //case FileFormat.FramePNG:
            //    //    dlg.DefaultExt = ".png";
            //    //    dlg.Filter = $"Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
            //    //    dlg.FileName = filepath;
            //    //    //ShowOpenFileDialog(dlg, () => Project.Export(dlg.FileName, this.ActiveProject.ActiveFrame));
            //    //    break;
            //    case FileFormats.SpriteSpr:
            //        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.SprFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () => { this.ActiveProject.ActiveSprite = (SpritePresenter)oldProject.LoadSprite(dlg.FileName, this.ActiveProject.ActivePalette); });
            //        this.ActiveProject.SetActiveSpriteSframeDbfDataView();
            //        break;
            //    case FileFormats.SpriteFrameSpr:
            //        dlg.Filter = $"7KAA Sprite Files (*.spr)|*{props.SprFileExtension}|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.SprFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () =>
            //        {
            //            SpritePresenter spr;
            //            if (this.ActiveProject.ActiveSprite == null)
            //                spr = new SpritePresenter();
            //            else
            //                spr = this.ActiveProject.ActiveSprite;
            //            spr.Frames.Add(oldProject.LoadFrame(dlg.FileName, this.ActiveProject.ActivePalette));
            //            this.ActiveProject.ActiveSprite = spr;
            //        });
            //        break;
            //    case FileFormats.DbaseIII:
            //        dlg.Filter = $"7KAA Resource Files (*.res)|*{props.ResFileExtension}|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.ResFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () =>
            //        {
            //            Tuple<SpritePresenter, DataTable> tup = oldProject.LoadResDbf(dlg.FileName, this.ActiveProject.ActivePalette);
            //            //this.ActiveProject.ActiveSprite = tup.Item1;
            //            this.ActiveProject.ActiveGameSet = this.ActiveProject.ActiveGameSet ?? new DataSet();
            //            this.ActiveProject.ActiveGameSet.Tables.Add(tup.Item2);
            //            this.ActiveProject.ActiveGameSet.AddDataSource(Path.GetFileName(dlg.FileName));
            //        });
            //        break;
            //    case FileFormats.Palette:
            //        dlg.Filter = $"7KAA Palette Files (*.res) (*.col)|*{props.ResFileExtension};*.col|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.ResFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () => this.ActiveProject.OpenPalette(dlg.FileName));
            //        break;
            //    case FileFormats.ResIdxMultiBmp:
            //        dlg.Filter = $"7KAA Resource Files (*.res)|*{props.ResFileExtension}|All Files (*.*)|*.*";
            //        dlg.DefaultExt = props.ResFileExtension;
            //        dlg.FileName = filepath;
            //        OpenFile(dlg, format, () =>
            //        {
            //            Tuple<SpritePresenter, DataTable> tup = oldProject.LoadResIdxMultiBmp(dlg.FileName, this.ActiveProject.ActivePalette);
            //            this.ActiveProject.ActiveGameSet = this.ActiveProject.ActiveGameSet ?? new DataSet();
            //            this.ActiveProject.ActiveGameSet.Tables.Add(tup.Item2);
            //            this.ActiveProject.ActiveGameSet.AddDataSource(Path.GetFileName(dlg.FileName));
            //            this.ActiveProject.ActiveSprite = tup.Item1;
            //        });
            //        break;
            //    case FileFormats.Any: //user did not specify file type via UI menus (drag/drop or generic Open File)
            //        OpenFile(dlg, format, null);
            //        break;
            //}
        }
    }
}
