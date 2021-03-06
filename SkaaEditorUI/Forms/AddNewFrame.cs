﻿#region Copyright Notice
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
using System.Windows.Forms;

namespace SkaaEditorUI.Forms
{
    public partial class AddNewFrame : Form
    {
        public string FrameName = string.Empty;
        public int FrameHeight = 0;
        public int FrameWidth = 0;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text != string.Empty && this.txtHeight.Text != string.Empty && this.txtHeight.Text != string.Empty)
            {
                this.FrameName = this.txtName.Text.ToUpper();
                try
                {
                    this.FrameHeight = Convert.ToInt32(this.txtHeight.Text);
                    this.FrameWidth = Convert.ToInt32(this.txtWidth.Text);
                    this.Close();
                }
                catch
                {
                    this.DialogResult = DialogResult.Abort;
                }
            }
        }

        public AddNewFrame()
        {
            InitializeComponent();
        }
    }
}
