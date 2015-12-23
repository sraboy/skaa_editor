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
using Capslock.WinForms.ImageEditor;
using SkaaEditorControls;
using WeifenLuo.WinFormsUI.Docking;

namespace SkaaEditorUI.Forms.DockPanels
{
    public partial class ToolboxContainer : DockContent
    {
        private DrawingToolbox drawingToolbox;
        private SkaaColorChooser colorGridChooser;

        public ToolboxContainer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.drawingToolbox = new DrawingToolbox();
            this.colorGridChooser = new SkaaColorChooser();
            this.SuspendLayout();
            // 
            // drawingToolbox
            // 
            this.drawingToolbox.Location = new System.Drawing.Point(1, 1);
            this.drawingToolbox.Margin = new System.Windows.Forms.Padding(2);
            this.drawingToolbox.Name = "drawingToolbox";
            this.drawingToolbox.Size = new System.Drawing.Size(175, 69);
            this.drawingToolbox.TabIndex = 18;
            // 
            // colorGridChooser
            // 
            this.colorGridChooser.AutoAddColors = false;
            this.colorGridChooser.CellSize = new System.Drawing.Size(18, 18);
            this.colorGridChooser.Columns = 8;
            this.colorGridChooser.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            this.colorGridChooser.Location = new System.Drawing.Point(0, 76);
            this.colorGridChooser.Name = "colorGridChooser";
            this.colorGridChooser.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            this.colorGridChooser.ShowCustomColors = false;
            this.colorGridChooser.Size = new System.Drawing.Size(175, 679);
            this.colorGridChooser.TabIndex = 17;
            // 
            // ToolboxContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 756);
            this.Controls.Add(this.drawingToolbox);
            this.Controls.Add(this.colorGridChooser);
            this.Name = "ToolboxContainer";
            this.Text = "ToolboxContainer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
