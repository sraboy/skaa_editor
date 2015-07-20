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

namespace SkaaEditor
{
    public partial class SkaaEditorMainForm : Form
    {
        private bool GridOn = true;
        Sprite sprite;

        //hack for when I break these projects' builds and lose the DLLs
        //the Designer will freak out
        //SkaaColorChooser.SkaaColorChooser skaaColorChooser1;
        //MultiplePictureBox.MultiplePictureBox multiplePictureBox1;

        public SkaaEditorMainForm()
        {
            //skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
            //multiplePictureBox1 = new MultiplePictureBox.MultiplePictureBox();
            //pbEdit = new InterpolatedBox();

            InitializeComponent();

            if (skaaColorChooser1.Palette == null)
                btnLoadSPR.Enabled = false;

            this.sprite = new Sprite();
        }

        private void btnLoadSPR_Click(object sender, EventArgs e)
        {
            if (skaaColorChooser1.Palette == null)
                return;

            //ResourceFile resfile = new ResourceFile("i_menu2.res");
            //pictureBox1.Image = resfile.Resources[0].initIMGStream();

            FileStream spritestream = File.OpenRead("../../data/sprite/ballista.spr");
            //ballista.spr length = 82578 (data_buf_size in init_import()

            /************************
             * Get all of the frames
             ************************/
            //Byte[] sprite_data = new Byte[spritestream.Length];
            //spritestream.Read(sprite_data, 0, (int) spritestream.Length);


            sprite.Frames = new List<SpriteFrame>();

            while (spritestream.Position < spritestream.Length)
            {
                ////jump to offset 17537, w=62, h=65 (cur_dir=1) + 4 byte (junk?)
                //spritestream.Seek(17537+4, SeekOrigin.Begin);
                Byte[] frame_size_bytes = new Byte[4];
                spritestream.Seek(4, SeekOrigin.Current); //skip the 32-bit size value
                spritestream.Read(frame_size_bytes, 0, 4);

                short width = BitConverter.ToInt16(frame_size_bytes, 0);// >> 16;
                short height = BitConverter.ToInt16(frame_size_bytes, 2);// >> 16;
                
                //Just a cheap hack to get around VS issues with the Designer
                //SkaaColorChooser.SkaaColorChooser skaaColorChooser1 = new SkaaColorChooser.SkaaColorChooser();
                
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
            editFrame(sprite.Frames[0]);
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
            this.GridOn = !this.GridOn;
        }

        private void pbEdit_Paint(object sender, PaintEventArgs e)
        {

        }

        private void editFrame(SpriteFrame sf)
        {
            int zoomWidth = sf.Image.Width * 10;
            int zoomHeight = sf.Image.Height * 10;
            
            System.Drawing.Bitmap bmp =
                new System.Drawing.Bitmap(sf.Image, new System.Drawing.Size(zoomWidth, zoomHeight));

            pbEdit.Image = bmp;
        }
    }


    ///// <summary>
    ///// A PictureBox control extended to allow a variety of interpolations.
    ///// </summary>
    //class InterpolatedBox:PictureBox
    //{
    //    #region Interpolation Property
    //    /// <summary>Backing Field</summary>
    //    private InterpolationMode interpolation = InterpolationMode.Low;

    //    /// <summary>
    //    /// The interpolation used to render the image.
    //    /// </summary>
    //    [DefaultValue(typeof(InterpolationMode), "Default"),
    //    Description("The interpolation used to render the image.")]
    //    public InterpolationMode Interpolation {
    //        get { return interpolation; }
    //        set {
    //            if(value == InterpolationMode.Invalid)
    //                throw new ArgumentException("\"Invalid\" is not a valid value."); // (Duh!)

    //            interpolation = value;
    //            Invalidate(); // Image should be redrawn when a different interpolation is selected
    //        }
    //    }
    //    #endregion

    //    /// <summary>
    //    /// Overridden to modify rendering behavior.
    //    /// </summary>
    //    /// <param name="pe">Painting event args.</param>
    //    protected override void OnPaint(PaintEventArgs pe) {
    //        // Before the PictureBox renders the image, we modify the
    //        // graphics object to change the interpolation.

    //        // Set the selected interpolation.
    //        pe.Graphics.InterpolationMode = interpolation;
    //        // Certain interpolation modes (such as nearest neighbor) need
    //        // to be offset by half a pixel to render correctly.
    //        pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

    //        // Allow the PictureBox to draw.
    //        base.OnPaint(pe);
    //    }
    //}
    
}
