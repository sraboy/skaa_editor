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
        public ObjectListViewContainer()
        {
            InitializeComponent();
        }

        public void SetDataSource<T>(T dataSource) where T : IListSource
        {
            this.dataListView1.DataSource = dataSource;
        }
    }
}
