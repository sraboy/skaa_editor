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
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using SkaaColorChooser;
using System.Drawing.Imaging;
using SkaaGameDataLib;
using System.Collections.Generic;

namespace SkaaEditor
{
    public partial class SkaaEditorMainForm : Form
    {
        private Sprite activeSprite;
        private SpriteFrame activeFrame;

        public SkaaEditorMainForm()
        {
            InitializeComponent();

            if (skaaColorChooser1.Palette == null)
                btnLoadSPR.Enabled = false;

            skaaColorChooser1.ActiveColorChanged += skaaColorChooser1_ActiveColorChanged;
            showGridToolStripMenuItem.Checked = skaaImageBox1.ShowPixelGrid;

            skaaImageBox1.ImageUpdated += skaaImageBox1_ImageUpdated;
        }
        void skaaImageBox1_ImageUpdated(object sender, EventArgs e)
        {
            //todo: need to change multiplePictureBox to SpriteFrame instead of image
            //multiplePictureBox1.


            //throw new NotImplementedException();
        }
        private void skaaColorChooser1_ActiveColorChanged(object sender, EventArgs e)
        {
            skaaImageBox1.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
        }


        private void btnLoadSPR_Click(object sender, EventArgs e)
        {
            if (skaaColorChooser1.Palette == null)
                return;

            FileStream spritestream = File.OpenRead("../../data/sprite/ballista.spr");
            activeSprite = new Sprite(skaaColorChooser1.Palette);

            var x = spritestream.Length;


            while (spritestream.Position < spritestream.Length)
            {
                Byte[] frame_size_bytes = new Byte[8];

                spritestream.Read(frame_size_bytes, 0, 8);
                
                int size = BitConverter.ToInt32(frame_size_bytes, 0);
                short width = BitConverter.ToInt16(frame_size_bytes, 4);
                short height = BitConverter.ToInt16(frame_size_bytes, 6);

                SpriteFrame frame = new SpriteFrame(size, width, height, skaaColorChooser1.Palette);

                frame.GetPixels(spritestream);

                //debugging: gives an ASCII representation of image 
                //(add \n after every 62d character). Verified alignment of pixels as read.
                //var hex = BitConverter.ToString(frame.FrameData);

                frame.BuildBitmap32bpp();

                activeSprite.Frames.Add(frame);
            }

            exportAsToolStripMenuItem.Enabled = true;
            spritestream.Close();

            //todo: figure out the UX for editing individual frames
            activeFrame = activeSprite.Frames[0];
            skaaImageBox1.Image = activeFrame.ImageBmp;
            skaaFrameViewer1.ActiveSprite = this.activeSprite;
            skaaFrameViewer1.ActiveFrame = this.activeFrame;
        }
        private void cbEdit_CheckedChanged(object sender, EventArgs e)
        {
            skaaImageBox1.EditMode = !skaaImageBox1.EditMode;
        }
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "pal_std.res";
            dlg.DefaultExt = ".res";
            dlg.SupportMultiDottedExtensions = true;

            if (dlg.ShowDialog() == DialogResult.OK)
                this.activeSprite = new Sprite(skaaColorChooser1.LoadPalette(dlg.FileName));

            btnLoadSPR.Enabled = true;
        }
        private void skaaEditorMainForm_Load(object sender, EventArgs e)
        {
            exportAsToolStripMenuItem.Enabled = false;

            //turn on the Export capability only after the ImageChanged event fires, when an image is (re)loaded
            skaaImageBox1.ImageChanged += (s, a) =>
            {
                exportAsToolStripMenuItem.Enabled = (skaaImageBox1.Image == null) ? true : false;
            };
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm abt = new AboutForm();
            abt.Show();
        }
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.skaaImageBox1.ShowPixelGrid = !this.skaaImageBox1.ShowPixelGrid;
            (sender as ToolStripMenuItem).Checked = this.skaaImageBox1.ShowPixelGrid;
        }
        private void exportBmp32bppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate);

                skaaImageBox1.Image.Save(fs, ImageFormat.Bmp);
                fs.Close();
            }
        }
        private void saveFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skaaImageBox1.Image == null)
                return;

            //todo: need to overwrite entire file if file exists
            //      right now, it just overwrites by byte so if the
            //      original is larger than what I'm writing, its
            //      data will still be there.
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate);
                    Byte[] save = activeFrame.BuildBitmap8bppIndexed();
                    fs.Write(save, 0, Buffer.ByteLength(save));
                    fs.Close();
                }
                catch (IOException ioex)
                {
                    throw new Exception(ioex.ToString());
                }
                
            }
        }

        private void saveAllFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (skaaImageBox1.Image == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate);
                Byte[] save = activeSprite.BuildSPR();
                fs.Write(save, 0, Buffer.ByteLength(save));
                fs.Close();
            }
        }


    }
}
