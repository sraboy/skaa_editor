﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkaaEditorUI
{
    public partial class SkaaEditorMainForm
    {
        private List<DebugArgs> _debugArgs;

        private class DebugArgs
        {
            public string MethodName;
            public object Arg;
        }
        /// <summary>
        /// When debugging, creates and adds a new <see cref="DebugArgs"/> object 
        /// to the <see cref="SkaaEditorMainForm._debugArgs"/> List. This is 
        /// essentially a dirty hack for easy debugging with a global variable.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arg"></param>
        [Conditional("DEBUG")]
        private void AddDebugArg(string methodName, object arg)
        {
            if (this._debugArgs == null)
                this._debugArgs = new List<DebugArgs>();

            this._debugArgs.Add(new DebugArgs() { MethodName = methodName, Arg = arg });
        }
        /// <summary>
        /// Configures settings for debugging/development.
        /// </summary>
        [Conditional("DEBUG")]
        private void ConfigSettingsDebug()
        {
            this._debugArgs = new List<DebugArgs>();
            this.btnDebugAction.Visible = true;
            this.btnDebugAction.Click += btnDebugAction_Click;
            this.lbDebugActions.Visible = true;

            //////////////////////// sraboy-targets on my dev boxes ////////////////////////
            string skaaMilkEnvy = @"E:\Programming\GitHubVisualStudio\7kaa\data\";
            string skaaLemmiwinks = @"E:\Nerd\c_and_c++\7kaa\data";
            if (Directory.Exists(skaaMilkEnvy))
                props.SkaaDataDirectory = skaaMilkEnvy;
            else if (Directory.Exists(skaaLemmiwinks))
                props.SkaaDataDirectory = skaaLemmiwinks;
            if (props.SkaaDataDirectory != string.Empty)
                this.lbDebugActions.Items.Add("SaveAndCopyProject");
            ////////////////////////////////////////////////////////////////////////////////

            this.lbDebugActions.Items.Add("SimpleOpenBallistaAndGameSet");
            //this.lbDebugActions.Items.Add("GetFileListing");
            //this.lbDebugActions.Items.Add("SaveProjectToDateTimeDirectory");
            //this.lbDebugActions.Items.Add("OpenDefaultButtonResource"); 
        }
        [Conditional("DEBUG")]
        private void SimpleOpenBallistaAndGameSet()
        {
            ConfigSettings();
            NewProject();
            this.ActiveProject.ActiveSprite = Project.LoadSprite(props.DataDirectory + "ballista.spr", this.ActiveProject.ActivePalette);
            this.ActiveProject.SetActiveSpriteSframeDbfDataView();

            if (this.ActiveProject.ActiveSprite != null)
            {
                this.ActiveProject.ActiveSprite.SpriteUpdated += ActiveSprite_SpriteUpdated;
                this.timelineControl.SetFrameList(this.ActiveProject.ActiveSprite.GetFrameImages());
                this.exportPngToolStripMenuItem.Enabled = true;
            }

            if (this.ActiveProject.OpenGameSet(props.DataDirectory + "std.set"))
                this.saveGameSetToolStripMenuItem.Enabled = true;
        }
        [Conditional("DEBUG")]
        private static void WriteCsv(List<KeyValuePair<string, string>> files)
        {
            using (StreamWriter sw = new StreamWriter("file_types.csv", false))
            {
                sw.WriteLine("filename,filepath,extension,format");
                foreach (KeyValuePair<string, string> kv in files)
                {
                    if (Path.GetExtension(kv.Key) != ".bak") //skip my local backup files
                        sw.WriteLine($"{Path.GetFileName(kv.Key)},{kv.Key},{Path.GetExtension(kv.Key)},{kv.Value}");
                }
            }
        }
        private void btnDebugAction_Click(object sender, EventArgs e)
        {
            foreach (string debugAction in this.lbDebugActions.SelectedItems)
            {
                Type thisType = this.GetType();
                MethodInfo debugMethod = thisType.GetMethod(debugAction, BindingFlags.Instance | BindingFlags.NonPublic);
                debugMethod.Invoke(this, null);
            }

            this._debugArgs = null;
        }
    }
}