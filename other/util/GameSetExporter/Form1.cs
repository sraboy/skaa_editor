using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaGameDataLib;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GameSetExporter
{
    public partial class Form1 : Form
    {
        GameSet set;
        Dictionary<string, string> tableJson;

        public Form1()
        {
            InitializeComponent();
            this.btnSave.Enabled = false;
        }

        private void btnLoadSet_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".set";
            dlg.FileName = "std.set";
            string json;
            tableJson = new Dictionary<string, string>();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                set = new GameSet(dlg.FileName);

                foreach(DataTable dt in set.Databases.Tables)
                {
                    json = JsonConvert.SerializeObject(dt);
                    JToken jt = JToken.Parse(json);
                    tableJson.Add(dt.TableName, jt.ToString(Formatting.Indented));
                }

                //json = JsonConvert.SerializeObject(set.Databases.Tables["SFRAME"]);
                //JToken jt = JToken.Parse(json);
                //this.txtOutput.Text = jt.ToString(Formatting.Indented);
            }

            this.btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                foreach (KeyValuePair<string, string> kv in tableJson)
                {
                    using (FileStream fs = new FileStream(Path.GetDirectoryName(dlg.FileName) + '\\' + kv.Key.ToString() + ".dbf.json.txt", FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                            sw.Write(kv.Value);
                    }
                }
            }

        }
    }
}
