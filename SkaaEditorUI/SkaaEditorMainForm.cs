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
using System.Drawing.Imaging;

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

            //this.activeFrame = new SpriteFrame();

            skaaColorChooser1.ActiveColorChanged += skaaColorChooser1_ActiveColorChanged;
            showGridToolStripMenuItem.Checked = skaaImageBox1.ShowPixelGrid;

            skaaImageBox1.ImageUpdated += skaaImageBox1_ImageUpdated;
        }

        void skaaImageBox1_ImageUpdated(object sender, EventArgs e)
        {
            //todo: need to change multiplePictureBox to SpriteFrame instead of image
            //multiplePictureBox1.
            throw new NotImplementedException();
        }

        private void skaaColorChooser1_ActiveColorChanged(object sender, EventArgs e)
        {
            skaaImageBox1.ActiveColor = (e as ActiveColorChangedEventArgs).NewColor;
        }
        private void btnLoadSPR_Click(object sender, EventArgs e)
        {
            if (skaaColorChooser1.Palette == null)
                return;

            //ResourceFile resfile = new ResourceFile("i_menu2.res");
            //pictureBox1.Image = resfile.Resources[0].initIMGStream();

            FileStream spritestream = File.OpenRead("../../data/sprite/ballista.spr");

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
                
                activeSprite.Frames.Add(frame);

                //todo: Just a hack since we skip pixels that are preset to 0x00.
                // Will need to write those pixels as the actual Color.Transparent
                // so we can have black in our images.
                frame.Image.MakeTransparent(System.Drawing.Color.Black);

                exportAsToolStripMenuItem.Enabled = true;
            }

            foreach(SpriteFrame sf in activeSprite.Frames)
                multiplePictureBox1.AddImage(sf.Image);
            
            spritestream.Close();

            //todo: figure out the UX for editing individual frames
            activeFrame = activeSprite.Frames[0];
            skaaImageBox1.Image = activeFrame.Image;
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

            skaaImageBox1.ImageChanged += (s, a) => {
                    exportAsToolStripMenuItem.Enabled = 
                        (skaaImageBox1.Image == null) ? true : false;
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

            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate);
                
                skaaImageBox1.Image.Save(fs, ImageFormat.Bmp);
                fs.Close();
            }
        }
        private void saveFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeFrame.SaveChanges(skaaImageBox1.Image as Bitmap, activeSprite.Palette);

            //Byte palColorByte; 
            //Bitmap bmp = skaaImageBox1.Image as Bitmap;
            //SpriteFrame sf = new SpriteFrame(bmp.Width, bmp.Height, skaaColorChooser1.Palette);
            //byte transparentByte = 0xf8;
            //int transparentByteCount = 0;
            //int realOffset = 0;
            //sf.Height = bmp.Height;
            //sf.Width = bmp.Width;
            ////todo: should probably just convert this to a List at the source
            //List<Color> Palette = new List<Color>();
            //foreach(Color c in skaaColorChooser1.Palette.Entries)
            //{
            //    Palette.Add(c);
            //}
            ////the below is pretty much the same as GetPixel() but reversed(ish)
            //for (int y = 0; y < bmp.Height; ++y)
            //{
            //    for (int x = 0; x < bmp.Width; ++x)
            //    {
            //        Color pixel = bmp.GetPixel(x, y);
            //        palColorByte = Convert.ToByte(Palette.FindIndex(c => c == Color.FromArgb(255, pixel)));
            //        if(palColorByte == 0)
            //        {
            //            transparentByteCount++;
            //        }
            //        else
            //        {
            //            if (transparentByteCount > 0)
            //            {
            //                    sf.FrameData[realOffset] = transparentByte;
            //                    realOffset++;
            //                    sf.FrameData[realOffset] = Convert.ToByte(transparentByteCount);
            //                    realOffset++;
            //                    sf.FrameData[realOffset] = palColorByte;
            //                    realOffset++;
            //                    transparentByteCount = 0;
            //            }
            //            else
            //            {
            //                sf.FrameData[realOffset] = palColorByte;
            //                realOffset++;
            //            }
            //        }
            //    }//end inner for
            //}//end outer for
        }
    }    
}
