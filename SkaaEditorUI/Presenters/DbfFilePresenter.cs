using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using SkaaGameDataLib;

namespace SkaaEditorUI.Presenters
{
    class DbfFilePresenter : PresenterBase<DataTable>
    {
        public override DataTable Load(string filePath, params object[] param)
        {
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                DbfFile file = new DbfFile();

                if (file.ReadStream(fs) != true)
                    throw new Exception("Failed to read DBF file.");

                file.DataTable.TableName = Path.GetFileNameWithoutExtension(filePath);
                file.DataTable.ExtendedProperties.Add("FileName", filePath);
                dt = file.DataTable;
            }

            return dt;
        }

        public override bool Save(string filePath, params object[] param)
        {
            throw new NotImplementedException();
        }

        protected override void SetupFileDialog(FileDialog dlg)
        {
            throw new NotImplementedException();
        }
    }
}
