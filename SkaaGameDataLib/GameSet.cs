/****************************************************************************
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
****************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    [Serializable]
    public class SkaaGameSet
    {
        public GameSetFile SetFile;
        private DataSet _gameDataSet;
        public DataSet GameDataSet
        {
            get
            {
                return this._gameDataSet;
            }
            set
            {
                if (this._gameDataSet != value)
                { 
                    this._gameDataSet = value;
                }
            }
        }        

        public SkaaGameSet() { }
        public SkaaGameSet(string setFileDirectory, string tempDirectory)
        {
            this.SetFile = new GameSetFile(setFileDirectory, tempDirectory);
            this.SetFile.OpenAndRead();
            this.GameDataSet = this.SetFile.GameDataSet;
        }

        public DataView GetSpriteDataView(string spriteId)
        {
            DataView dv = new DataView(this.GameDataSet.Tables["SFRAME"]);
            dv.RowFilter = string.Format("SPRITE = '{0}'", spriteId);
            
            return dv;
        }
        public void SaveGameSet(string filename)
        {
            this.SetFile.SaveGameSetToFile(filename);
        }

    }//end GameSet
}
