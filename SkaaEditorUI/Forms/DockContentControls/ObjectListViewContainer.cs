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
using System.Data;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockContentControls
{
    public partial class ObjectListViewContainer : DockContent
    {
        private string _tableToSave;

        public string TableToSave
        {
            get
            {
                return this._tableToSave;
            }
            private set
            {
                this._tableToSave = value;
            }
        }

        public ObjectListViewContainer()
        {
            InitializeComponent();
            this.dataListView1.ItemChecked += DataListView1_ItemChecked;
            this.btnDone.Enabled = false;
        }

        private void DataListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.btnDone.Enabled = this.dataListView1.CheckedObject == null ? false : true;
        }

        public void SetDataSource(DataTable dt)
        {
            this.dataListView1.DataSource = dt;
            //this.dataListView1.AutoSizeColumns();
            this.dataListView1.CheckBoxes = true;
            this.dataListView1.ShowGroups = true;
            this.dataListView1.CheckedAspectName = "Save";
            this.dataListView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            DataRowView checkedRow = this.dataListView1.CheckedObject as DataRowView;

            //string t = checkedRow[1].ToString();
            this.TableToSave = checkedRow.Row[1].ToString();
        }

    }
}
