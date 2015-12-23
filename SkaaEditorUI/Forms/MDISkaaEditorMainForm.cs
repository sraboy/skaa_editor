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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaEditorUI.Forms.DockPanels;
using WeifenLuo.WinFormsUI.Docking;
using static SkaaEditorUI.SProjectManager;

namespace SkaaEditorUI.Forms
{
    public partial class MDISkaaEditorMainForm : Form
    {
        //private ProjectManager _projectManager;

        public MDISkaaEditorMainForm()
        {
            InitializeComponent();
            AddDockPanels();
        }

        private void AddDockPanels()
        {
            ImageEditorContainer iec = new ImageEditorContainer();
            ToolboxContainer tc = new ToolboxContainer();
            SpriteViewerContainer svc = new SpriteViewerContainer();
            iec.Show(dockPanel, DockState.Document);
            tc.Show(dockPanel, DockState.DockLeft);
            svc.Show(dockPanel, DockState.DockRight);
        }

        private void toolStripBtnNewProject_Click(object sender, EventArgs e)
        {
            if (TrySaveCloseProject())
                ProjectManager.CreateNewProject();
        }

        /// <summary>
        /// Closes the current project and saves changes, if needed.
        /// </summary>
        /// <returns>True if the project was closed (whether or not saved). False otherwise.</returns>
        private bool TrySaveCloseProject()
        {
            DialogResult saveChanges = UserShouldSaveChanges();

            if (saveChanges == DialogResult.Yes)
            {
                ProjectManager.CloseProject();
                return true;
            }
            else if (saveChanges == DialogResult.No)
            {
                ProjectManager.CloseProject();
                return true;
            }
            else // (DialogResult.Cancel)
                return false;
        }

        private DialogResult UserShouldSaveChanges()
        {
            bool spriteHasChanges = CheckSpriteForPendingChanges(this.ActiveProject?.ActiveSprite);

            if (!spriteHasChanges)// && this.ActiveProject?.UnsavedSprites?.Count == 0)
                return DialogResult.No;
            else
                return MessageBox.Show("You have unsaved changes. Do you want to save these changes?", "Save?", MessageBoxButtons.YesNoCancel);
        }
    }
}
