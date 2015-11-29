using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkaaEditorUI;

namespace SkaaEditorUnitTester
{
    [TestClass]
    public class ProjectTester
    {
        string ApplicationDirectory, DataDirectory, TempDirectory, ProjectsDirectory;
        Project proj;

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

        [TestMethod]
        public void MakeProjectGameSetPaletteSprite()
        {
            this.proj = new Project();
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
            var check =
                this.proj.OpenSprite(this.ProjectsDirectory + "edited\\ballista.spr");
            Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
            this.proj.OpenSprite(this.DataDirectory + "ballista.spr");
        }

        [TestMethod]
        public void MakeProjectGameSetSpritePalette()
        {
            this.proj = new Project();
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
            var check = 
                this.proj.OpenSprite(this.ProjectsDirectory + "edited\\ballista.spr");
            Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
        }

        [TestMethod]
        public void MakeProjectPaletteSpriteGameSet()
        {
            this.proj = new Project();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
            this.proj.OpenSprite(this.DataDirectory + "ballista.spr");
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
        }

        [TestMethod]
        public void MakeProjectPaletteGameSetSprite()
        {
            this.proj = new Project();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
            this.proj.OpenSprite(this.ProjectsDirectory + "edited\\ballista.spr");
        }

        [TestMethod]
        public void MakeProjectSpritePaletteGameSet()
        {
            this.proj = new Project();
            var check = 
                this.proj.OpenSprite(this.ProjectsDirectory + "edited\\ballista.spr");
            Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
        }

        [TestMethod]
        public void MakeProjectSpriteGameSetPalette()
        {
            this.proj = new Project();
            var check =
                this.proj.OpenSprite(this.ProjectsDirectory + "edited\\ballista.spr");
            Debug.Assert(check == null, "Sprite should be null when no palette is specified!");
            this.proj.LoadGameSet(this.DataDirectory + "std.set");
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
        }

        [TestMethod]
        public void ProjectChangeGameSet()
        {
            MakeProjectGameSetPaletteSprite();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");

            MakeProjectGameSetSpritePalette();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");

            MakeProjectPaletteSpriteGameSet();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");

            MakeProjectPaletteGameSetSprite();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");

            MakeProjectSpritePaletteGameSet();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");

            MakeProjectSpriteGameSetPalette();
            this.proj.LoadGameSet(this.ProjectsDirectory + "edited\\std.set");
        }

        [TestMethod]
        public void ProjectChangePalette()
        {
            MakeProjectGameSetPaletteSprite();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");

            MakeProjectGameSetSpritePalette();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");

            MakeProjectPaletteSpriteGameSet();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");

            MakeProjectPaletteGameSetSprite();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");

            MakeProjectSpritePaletteGameSet();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");

            MakeProjectSpriteGameSetPalette();
            this.proj.LoadPalette(this.DataDirectory + "pal_std.res");
        }
    }
}
