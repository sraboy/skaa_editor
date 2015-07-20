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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkaaColorChooser;

namespace SkaaEditor
{
    public partial class SkaaEditorMainForm : Form
    {
        Sprite sprite;

        public SkaaEditorMainForm()
        {
            InitializeComponent();

            if (skaaColorChooser1.Palette == null)
                btnLoadSPR.Enabled = false;

            this.sprite = new Sprite();

            skaaColorChooser1.ActiveColorChanged += skaaColorChooser1_ActiveColorChanged;
        }

        void skaaColorChooser1_ActiveColorChanged(object sender, EventArgs e)
        {
            skaaImageBox1.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
            //this.imageBox1.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
        }

        private void btnLoadSPR_Click(object sender, EventArgs e)
        {
            if (skaaColorChooser1.Palette == null)
                return;

            //ResourceFile resfile = new ResourceFile("i_menu2.res");
            //pictureBox1.Image = resfile.Resources[0].initIMGStream();

            FileStream spritestream = File.OpenRead("../../data/sprite/ballista.spr");

            sprite.Frames = new List<SpriteFrame>();

            while (spritestream.Position < spritestream.Length)
            {
                Byte[] frame_size_bytes = new Byte[4];
                spritestream.Seek(4, SeekOrigin.Current); //skip the 32-bit size value
                spritestream.Read(frame_size_bytes, 0, 4);

                short width = BitConverter.ToInt16(frame_size_bytes, 0);
                short height = BitConverter.ToInt16(frame_size_bytes, 2);
                
                SpriteFrame frame = new SpriteFrame(width, height, skaaColorChooser1.Palette);

                frame.GetPixels(spritestream);

                //debugging: gives an ASCII representation of image 
                //(add \n after every 62d character). Verified alignment of pixels as read.
                //var hex = BitConverter.ToString(frame.FrameData);

                frame.BuildBitmap32bpp();
                
                sprite.Frames.Add(frame);

                // TODO: Just a hack since we skip pixels that are preset to 0x00.
                // Will need to write those pixels as the actual Color.Transparent
                // so we can have black in our images.
                // this also has to be removed if we stick with the 8bpp indexed image
                frame.Image.MakeTransparent(System.Drawing.Color.Black);
            }

            foreach(SpriteFrame sf in sprite.Frames)
                multiplePictureBox1.AddImage(sf.Image);
            
            spritestream.Close();

            //todo: figure out the UX for editing individual frames
            skaaImageBox1.Image = sprite.Frames[0].Image;
            //imageBox1.Image = sprite.Frames[0].Image;
        }

        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "pal_std.res";
            dlg.DefaultExt = ".res";
            dlg.SupportMultiDottedExtensions = true;

            if (dlg.ShowDialog() == DialogResult.OK)
                skaaColorChooser1.LoadPalette(dlg.FileName);

            btnLoadSPR.Enabled = true;
        }

        private void SkaaEditorMainForm_Load(object sender, EventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm abt = new AboutForm();
            abt.Show();
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.GridOn = !this.GridOn;
            this.skaaImageBox1.ShowPixelGrid = !this.skaaImageBox1.ShowPixelGrid;
        }

        private void cbEdit_CheckedChanged(object sender, EventArgs e)
        {
            skaaImageBox1.EditMode = !skaaImageBox1.EditMode;
            //imageBox1.EditMode = !imageBox1.EditMode;
        }
    }    
}
