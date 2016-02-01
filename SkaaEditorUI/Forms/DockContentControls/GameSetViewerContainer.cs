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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockContentControls
{
    public partial class GameSetViewerContainer : DockContent
    {
        private bool _updateRequired = false;

        private GameSetPresenter _gameSetPresenter;
        public GameSetPresenter GameSetPresenter
        {
            get
            {
                return _gameSetPresenter;
            }
            set
            {
                if (this._gameSetPresenter != value)
                {
                    this._gameSetPresenter = value;
                    SetDataSource();
                }
            }
        }

        public GameSetViewerContainer()
        {
            InitializeComponent();
            this._gameSetPresenter = new GameSetPresenter();

            this.dataListView1.ItemChecked += DataListView1_ItemChecked;
            this.cbTables.SelectedIndexChanged += CbTables_SelectedIndexChanged;
            this.cbDataSources.SelectedIndexChanged += CbDataSources_SelectedIndexChanged;
            this.GameSetPresenter.PropertyChanged += GameSetPresenter_PropertyChanged;
        }

        //public void UpdateDataSources()
        //{
        //    PopulateComboBoxDataSourcesList();
        //}

        #region Event Handlers
        private void GameSetPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameObject")
                SetDataSource();
        }
        private void CbDataSources_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            PopulateComboBoxTablesList(this.cbDataSources.SelectedItem.ToString());
            // No need to call SetDataSource() since the above line will trigger
            // cbTables' SelectedIndexChanged event, which calls SetDataSource()
        }
        private void CbTables_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            /*
             * SetDataSource() will call PopulateComboxTablesList() if cbTables hasn't
             * been set up yet. When that's the case, dataListView1.DataSource is still 
             * null. If we don't return, we'd end up calling SetDataSource() in the middle
             * of a call to SetDataSource()
             */
            if (this.dataListView1.DataSource == null)
                return;

            SetDataSource();
        }
        private void DataListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }
        #endregion

        private string PopulateComboBoxTablesList(string dataSource)
        {
            DataSet ds = this._gameSetPresenter.GameObject;
            IList list;
            if (dataSource == "All")
                list = ds.Tables.Cast<DataTable>().ToList();
            else
                list = ds.Tables.Cast<DataTable>().Where(dt => dt.GetDataSource() == dataSource).ToList();

            this.cbTables.DataSource = list;
            this.cbTables.DisplayMember = "TableName";
            return this.cbTables.SelectedItem.ToString();
        }
        private string PopulateComboBoxDataSourcesList()
        {
            DataSet ds = this._gameSetPresenter.GameObject;
            List<string> list = new List<string>() { "All" };

            //AddRange() blows up if passed null
            var sources = ds.GetDataSources();
            if (sources != null)
                list.AddRange(sources);

            this.cbDataSources.DataSource = list;
            return this.cbDataSources.SelectedItem.ToString();
        }
        private void SetDataSource()
        {
            this.dataListView1.AllColumns = null;
            string dataSource = this.cbDataSources.SelectedItem?.ToString() ?? PopulateComboBoxDataSourcesList();
            string tableName = this.cbTables.SelectedItem?.ToString() ?? PopulateComboBoxTablesList(dataSource);

            ///* tableName is set from this.cbTables.SelectedItem?.ToString(). There may not 
            // * be a SelectedItem if Merge() was called when there was no DataSet to merge to.
            // * This could happen if merging is just the default behavior from the UI since 
            // * GameSetPresenter.Merge() will just automatically create the new DataSet if needed.
            // */
            //if (tableName == null)
            //    tableName = PopulateComboBoxTablesList(null);

            DataSet ds = this._gameSetPresenter.GameObject;
            DataView dv = new DataView(ds.Tables[tableName]);

            this.dataListView1.DataSource = dv;
            this.dataListView1.AutoResizeColumns();
            //can't use the below when using FastDataListView
            //re-enable if using DataListView
            //this.dataListView1.ShowGroups = true;

            this.TabText = tableName;
        }
    }
}
