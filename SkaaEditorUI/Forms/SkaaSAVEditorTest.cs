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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkaaEditorUI.Forms
{
    public partial class SkaaSAVEditorTest : Form
    {
        public SkaaSAVEditorTest()
        {
            InitializeComponent();
        }

        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            SaveGame game = new SaveGame();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //follow GameFile::read_file_2() in OGFILE2.cpp @ line 532

                /* BOOKMARK = 4096
                 * + 101 = 4197
                 * -- read file size
                 * + 102 = 4198
                 * -- config.read_file()
                 * + 103 = 4199
                 * -- sys.read_file()
                 * + 104 = 4200
                 * -- info.read_file()
                 * + 105 = 4201
                 * -- power.read_file()
                 * + 106 = 4202
                 * -- weather.read_file()
                 */

                FileStream savfile_stream = File.OpenRead(dlg.FileName);
// Maintained the Byte[] config line from the old branch.
// hadn't yet merged that into master.
// <<<<<<< HEAD
                Byte[] header = new Byte[302]; 
                Byte[] duo = new Byte[2];       //for getting sizes and bookmarks
                Byte[] config = new Byte[144];
                Byte[] game_dot_read_file = new Byte[2060];
// =======
                // byte[] header = new byte[304];
                // byte[] duo = new byte[2];       //for getting sizes and bookmarks
                // byte[] config = new byte[144];
// >>>>>>> master
                //Byte[] sys = new Byte[];
                //Byte[] info = new Byte[];
                //Byte[] power = new Byte[];
                //Byte[] weather = new Byte[];

                // *** Read header ***
                savfile_stream.Read(duo, 0, 2);  //read header size        (0x012e = 302)
                savfile_stream.Read(header, 0, 302);

                // *** Read version ***
                savfile_stream.Read(duo, 0, 2);  //read game version       (0x00d4 = 212)
                game.Version = BitConverter.ToInt16(duo, 0);
                savfile_stream.Read(duo, 0, 2);  //read bookmark+101           (0x1065 = 4197)

                // *** Read GameFile ***
                savfile_stream.Read(duo, 0, 2);  //GameFile object's size      (0x80c0 = 2060d)
                savfile_stream.Read(game_dot_read_file, 0, BitConverter.ToInt16(duo, 0));
                savfile_stream.Read(duo, 0, 2);  //read bookmark+102           (0x1066 = 4198)

                // *** Read config ***
                savfile_stream.Read(duo, 0, 2);  //read config recordSize  (0x0090 = 144)
                savfile_stream.Read(config, 0, BitConverter.ToInt16(duo, 0));
                savfile_stream.Read(duo, 0, 2);  //read bookmark           (0x1067 = 4199)


                savfile_stream.Close();

            }

        }

        public class SaveGame
        {
            private int _version, _recordSize;

            public int Version
            {
                get
                {
                    return this._version;
                }
                set
                {
                    if (this._version != value)
                        this._version = value;
                }
            }

            public int RecordSize
            {
                get
                {
                    return this._recordSize;
                }
                set
                {
                    if (this._recordSize != value)
                        this._recordSize = value;
                }
            }
        }
    }
}
