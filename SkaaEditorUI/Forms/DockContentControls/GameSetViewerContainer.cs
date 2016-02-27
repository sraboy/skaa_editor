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
using SkaaEditorUI.Presenters;
using SkaaGameDataLib.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockContentControls
{
    public partial class GameSetViewerContainer : DockContent
    {
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

        #region Constructor
        public GameSetViewerContainer()
        {
            InitializeComponent();
            this._gameSetPresenter = new GameSetPresenter();

            this.cbTables.SelectedIndexChanged += CbTables_SelectedIndexChanged;
            this.cbDataSources.SelectedIndexChanged += CbDataSources_SelectedIndexChanged;
            this.GameSetPresenter.PropertyChanged += GameSetPresenter_PropertyChanged;
        }
        #endregion

        #region Event Handlers
        private void GameSetPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameObject")
            {
                PopulateComboBoxDataSourcesList();
                PopulateComboBoxTablesList(this.cbDataSources.SelectedItem?.ToString());
                SetDataSource();
            }
        }
        private void CbDataSources_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // No need to call SetDataSource() since the PopulateComboBoxTablesList() 
            // will trigger cbTables' SelectedIndexChanged event, which calls SetDataSource()
            PopulateComboBoxTablesList(this.cbDataSources.SelectedItem?.ToString());
        }
        private void CbTables_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            /*
             * SetDataSource() will call PopulateComboBoxTablesList() if cbTables hasn't
             * been set up yet. When that's the case, dataListView1.DataSource is still 
             * null so we can use this to skip making a second call to SetDataSource().
             */
            if (this.dataListView1.DataSource == null)
                return;

            SetDataSource();
        }
        #endregion

        #region Private Methods
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
            return this.cbTables.SelectedItem?.ToString();
        }
        private string PopulateComboBoxDataSourcesList()
        {
            DataSet ds = this._gameSetPresenter.GameObject;
            List<string> list = new List<string>();

            var sources = ds.GetDataSourceList();

            if (sources != null)    //AddRange() blows up if passed null
            {
                if (sources.Count > 1)
                    list.Add("All");

                list.AddRange(sources);
            }

            this.cbDataSources.DataSource = list;
            return this.cbDataSources.SelectedItem?.ToString();
        }
        private void SetDataSource()
        {
            this.dataListView1.AllColumns = null;
            string dataSource = this.cbDataSources.SelectedItem?.ToString() ?? PopulateComboBoxDataSourcesList();
            string tableName = this.cbTables.SelectedItem?.ToString() ?? PopulateComboBoxTablesList(dataSource);

            DataSet ds = this._gameSetPresenter.GameObject;
            DataView dv = new DataView(ds.Tables[tableName]);
            this.dataListView1.DataSource = dv;
            this.dataListView1.AutoResizeColumns();

            this.TabText = tableName;
        }
        #endregion
    }
}
