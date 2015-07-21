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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkaaFrameViewer
{
    public partial class SkaaFrameViewer : UserControl
    {

        bool zoom = false;
        int imageOnDisplay = 0;

        private List<Bitmap> Frames;

        public SkaaFrameViewer()
        {
            InitializeComponent();
            Frames = new List<Bitmap>();
        }

        public void AddImage(Bitmap bmp)
        {
            Frames.Add(bmp);

            if (Frames.Count == 1) //first image
                picBoxFrame.Image = this.Frames[0];
        }

        private void picBoxFrame_Click(object sender, MouseEventArgs e) 
        {
            if (this.Frames.Count == 0)
                return;

            if (e.Button == MouseButtons.Left)
            {
                //hack to reset zoom
                zoom = false;
                picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(Frames[imageOnDisplay].Width, Frames[imageOnDisplay].Height));

                imageOnDisplay++;
                imageOnDisplay %= (Frames.Count - 1);
                picBoxFrame.Image = Frames[imageOnDisplay];
            }
            else if (e.Button == MouseButtons.Right)
            {
                //hack to reset zoom
                zoom = false;
                picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(Frames[imageOnDisplay].Width, Frames[imageOnDisplay].Height));

                imageOnDisplay--;
                imageOnDisplay = (imageOnDisplay % (Frames.Count - 1) + (Frames.Count - 1)) % (Frames.Count - 1);
                // special mod() function above to actually cycle negative numbers around. Turns out % isn't 
                // a real mod() function, just remainder.
                picBoxFrame.Image = Frames[imageOnDisplay];
            }
            else if (e.Button == MouseButtons.Middle)
            {
                int zoomWidth, zoomHeight;

                //todo: zoom is still a bit wonky. zoom = false above is a temp hack
                switch (zoom)
                {
                    case true:
                        zoomWidth = Frames[imageOnDisplay].Width;
                        zoomHeight = Frames[imageOnDisplay].Height;
                        picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(zoomWidth, zoomHeight));
                        zoom = false;
                        break;
                    case false:
                        zoomWidth = picBoxFrame.Image.Width * 2;
                        zoomHeight = picBoxFrame.Image.Height * 2;
                        picBoxFrame.Image = new Bitmap(picBoxFrame.Image, new Size(zoomWidth, zoomHeight));
                        zoom = true;
                        break;
                }

            }
        }

        
    }
}
