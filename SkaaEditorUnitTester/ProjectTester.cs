using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkaaEditorUI;
using SkaaGameDataLib;
using System.Data;
using System.Drawing;

namespace SkaaEditorUnitTester
{
    [TestClass]
    public class ProjectTester
    {
        string ApplicationDirectory, DataDirectory, TempDirectory, ProjectsDirectory;

        //[STAThread]
        //static void Main()
        //{
        //    ProjectTester pj = new ProjectTester();
        //    pj.MakeNewProject();
        //}

        public ProjectTester()
        {
            this.ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
            //Trace.AutoFlush = true;
            //Logger.TraceInformation.TraceEvent(TraceEventType.Start, 0, $"Log started: {string.Concat(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())}");

            this.DataDirectory = ApplicationDirectory + "data\\";
            this.TempDirectory = ApplicationDirectory + "temp\\";
            this.ProjectsDirectory = ApplicationDirectory + "projects\\";

            Debug.Assert(Directory.CreateDirectory(this.ProjectsDirectory) != null, $"Failed to create ProjectsDirectory: {this.ProjectsDirectory}");
            Debug.Assert(Directory.CreateDirectory(this.TempDirectory) != null, $"Failed to create TempDirectory: {this.TempDirectory}");
        }

        private Project GetNewProject(string palettePath = null)
        {
            Project proj = new Project();
            if (palettePath == null)
                palettePath = this.DataDirectory + "pal_std.res";

            proj.OpenPalette(palettePath);
            return proj;
        }

        [TestMethod]
        public void LoadResDbf()
        {
            var proj = GetNewProject();

            string primaryDbf = @"E:\Nerd\c_and_c++\7kaa\data\resource\rock1.res";
            string animationDbf = @"E:\Nerd\c_and_c++\7kaa\data\resource\rockani1.res";
            string rockblockDbf = @"E:\Nerd\c_and_c++\7kaa\data\resource\rockblk1.res";
            string rockBmpDbf = @"E:\Nerd\c_and_c++\7kaa\data\resource\rockbmp1.res";

            Tuple<Sprite, DataSet> tuple = Project.LoadResDbf(rockBmpDbf, proj.ActivePalette);
            Debug.Assert(tuple.Item1 != null, "Failed to load sprite data.");
            Debug.Assert(tuple.Item2.Tables.Count > 0, "Failed to load data tables.");
        }

        //[TestMethod]
        //public void MakeProjectGameSetPaletteSprite()
        //{
        //    this.proj = new Project();
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //    var check =
        //        this.proj.LoadSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        //    Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
        //    this.proj.LoadSprite(this.DataDirectory + "ballista.spr");
        //}

        //[TestMethod]
        //public void MakeProjectGameSetSpritePalette()
        //{
        //    this.proj = new Project();
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //    var check = 
        //        this.proj.LoadSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        //    Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //}

        //[TestMethod]
        //public void MakeProjectPaletteSpriteGameSet()
        //{
        //    this.proj = new Project();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //    this.proj.LoadSprite(this.DataDirectory + "ballista.spr");
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //}

        //[TestMethod]
        //public void MakeProjectPaletteGameSetSprite()
        //{
        //    this.proj = new Project();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //    this.proj.LoadSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        //}

        //[TestMethod]
        //public void MakeProjectSpritePaletteGameSet()
        //{
        //    this.proj = new Project();
        //    var check = 
        //        this.proj.LoadSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        //    Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //}

        //[TestMethod]
        //public void MakeProjectSpriteGameSetPalette()
        //{
        //    this.proj = new Project();
        //    var check =
        //        this.proj.LoadSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        //    Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
        //    this.proj.OpenGameSet(this.DataDirectory + "std.set");
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //}

        //[TestMethod]
        //public void ProjectChangeGameSet()
        //{
        //    MakeProjectGameSetPaletteSprite();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");

        //    MakeProjectGameSetSpritePalette();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");

        //    MakeProjectPaletteSpriteGameSet();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");

        //    MakeProjectPaletteGameSetSprite();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");

        //    MakeProjectSpritePaletteGameSet();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");

        //    MakeProjectSpriteGameSetPalette();
        //    this.proj.OpenGameSet(this.ProjectsDirectory + "edited\\std.set");
        //}

        //[TestMethod]
        //public void ProjectChangePalette()
        //{
        //    MakeProjectGameSetPaletteSprite();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");

        //    MakeProjectGameSetSpritePalette();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");

        //    MakeProjectPaletteSpriteGameSet();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");

        //    MakeProjectPaletteGameSetSprite();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");

        //    MakeProjectSpritePaletteGameSet();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");

        //    MakeProjectSpriteGameSetPalette();
        //    this.proj.OpenPalette(this.DataDirectory + "pal_std.res");
        //}
    }
}
