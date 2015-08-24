/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
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

namespace SkaaEditor
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

                Byte[] header = new Byte[302]; 
                Byte[] duo = new Byte[2];       //for getting sizes and bookmarks
                Byte[] config = new Byte[144];
                Byte[] game_dot_read_file = new Byte[2060];
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
