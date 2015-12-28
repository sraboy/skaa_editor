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
using System.Data;
using System.Windows.Forms;
using BrightIdeasSoftware;
using SkaaEditorUI.Presenters;
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
                this._gameSetPresenter = value;
                SetDataSource(this._gameSetPresenter?.GameObject);
            }
        }

        public GameSetViewerContainer()
        {
            InitializeComponent();
            this.dataListView1.ItemChecked += DataListView1_ItemChecked;
        }

        private void DataListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }

        private void SetDataSource(DataSet ds)
        {
            this.dataListView1.DataSource = ds;
            this.dataListView1.AutoSizeColumns();
            //this.dataListView1.CheckBoxes = true;
            this.dataListView1.ShowGroups = true;
            this.dataListView1.DataMember = "SFRAME";
            //this.dataListView1.CheckedAspectName = "Save";
            foreach (OLVColumn c in this.dataListView1.Columns)
                c.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
    }
}
