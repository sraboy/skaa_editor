#region Copyright Notice
/***************************************************************************
*   Copyright (C) 2015 Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkaaEditorUI
{
    public partial class ObjectListViewContainer : Form
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
