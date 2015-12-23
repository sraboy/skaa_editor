using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkaaGameDataLib;
using SkaaEditorUI.Utilities;
using SkaaEditorUI.Forms;

namespace SkaaEditorUI
{
    /// <summary>
    /// A singleton class that will manage a <see cref="Project"/>'s file operations
    /// </summary>
    public sealed class SProjectManager
    {
        private static readonly TraceSource Logger = new TraceSource($"{typeof(SProjectManager)}", SourceLevels.All);

        #region Lazy Singleton
        private static readonly Lazy<SProjectManager> lazyPm = new Lazy<SProjectManager>(() => new SProjectManager());
        public static SProjectManager ProjectManager { get { return lazyPm.Value; } }
        private SProjectManager() { this._tempFiles.Add(this.TempDirectory); }
        #endregion

        #region Private Members
        private Project _activeProject = new Project();
        private MDISkaaEditorMainForm _mainForm;

        private bool _hasUnsavedChanges = false;
        private bool _isInTempDirectory = false;

        private List<string> _tempFiles = new List<string>();
        private string _tempDirectory = GetTemporaryDirectory();
        private string _saveDirectory;
        #endregion

        #region Public Members
        public bool IsInTempDirectory
        {
            get
            {
                return _isInTempDirectory;
            }

            private set
            {
                this._isInTempDirectory = value;
            }
        }
        public string TempDirectory
        {
            get
            {
                return _tempDirectory;
            }

            private set
            {
                this._tempDirectory = value;
            }
        }
        public string SaveDirectory
        {
            get
            {
                return _saveDirectory;
            }

            private set
            {
                this._saveDirectory = value;
            }
        }
        public Project ActiveProject
        {
            get
            {
                return _activeProject;
            }

            private set
            {
                this._activeProject = value;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new <see cref="TempDirectory"/> in <see cref="Path.GetTempPath()"/>\<see cref="Path.GetRandomFileName()"/>
        /// </summary>
        private static string GetTemporaryDirectory()
        {
            var path = Path.GetTempPath() + '\\' + Path.GetRandomFileName();
            var d = Directory.CreateDirectory(path);

            if (d.Exists != false)
                Logger.TraceInformation($"Created new temporary directory in: {path}.");
            else
                Logger.TraceInformation($"Failed to create a new temporary directory in: {path}.");

            return path;
        }


        public void SetMainForm(MDISkaaEditorMainForm form)
        {
            this._mainForm = form;
        }


        /// <summary>
        /// Creates a new <see cref="Project"/> in <see cref="TempDirectory"/> 
        /// </summary>
        /// <returns>A new <see cref="Project"/></returns>
        public Project CreateNewProject()
        {
            this.IsInTempDirectory = true;
            return CreateNewProject(this.TempDirectory);
        }
        /// <summary>
        /// Creates a new <see cref="Project"/> in a the specified directory
        /// </summary>
        /// <returns>A new <see cref="Project"/></returns>
        public Project CreateNewProject(string filePath)
        {
            var p = new Project();
            Logger.TraceInformation($"Created new {typeof(Project)} in {filePath}.");
            return p;
        }


        public bool SaveProject(Project project, string filePath)
        {
            Logger.TraceInformation($"Saved {typeof(Project)} in {filePath}.");
            return true;
        }
        /// <summary>
        /// Closes the <see cref="ActiveProject"/> and unsubscribes from all events
        /// </summary>
        public void CloseProject()
        {
            this.ActiveProject = null;
            //Unsubscribe();
        }

        /// <summary>
        /// This function will open the specified 7KAA <see cref="GameSetFile"/>
        /// </summary>
        /// <param name="filePath">The complete path to the SET file.</param>
        /// <remarks>
        ///  A SET file, like 7KAA's std.set, simply contains multiple dBase III databases stitched together.
        /// </remarks>
        public bool OpenStandardSet(string filePath)
        {
            DataSet ds = new DataSet();

            if (ds.OpenStandardGameSet(filePath) == false)
                return false;
            else
            {
                this.ActiveProject.GameSet = ds;
                return true;
            }
        }

        /// <summary>
        /// Loads a 7KAA-formatted palette file.
        /// </summary>
        /// <param name="filePath">The specific palette file to load.</param>
        /// <returns>A ColorPalette built from the palette file</returns>
        public bool OpenPalette(string filePath)
        {
            var pal = PaletteLoader.FromResFile(filePath);

            if (pal == null)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"{typeof(PaletteLoader)} returned null. Failed to load palette: {filePath}");
                return false;
            }
            else
            {
                this.ActiveProject.ActivePalette = pal;
                return true;
            }
        }

        public void OpenSprite(OpenFileDialog dlg)
        {
            SpritePresenter spr = null;
            dlg.OpenAs<SpritePresenter>(this.SaveDirectory, () => { spr = LoadSprite(dlg.FileName, this._mainForm.ActivePalette); });
            this.ActiveProject.AddSprite(spr);
        }

        /// <summary>
        /// Creates a <see cref="SpritePresenter"/> object from an SPR-formatted file
        /// </summary>
        /// <param name="filePath">The absolute path to the SPR file to open</param>
        /// <returns>The newly-created <see cref="SpritePresenter"/></returns>
        /// <remarks>
        private static SpritePresenter LoadSprite(string filePath, ColorPalette pal)
        {
            if (pal == null)
                throw new ArgumentNullException("pal", "You must specify a ColorPalette to load a sprite.");

            SkaaSprite spr;

            using (FileStream spritestream = File.OpenRead(filePath))
                spr = SkaaSprite.FromSprStream(spritestream, pal);

            spr.SpriteId = Path.GetFileNameWithoutExtension(filePath);

            return new SpritePresenter(spr);
        }

        public static FileFormats CheckFileType(string filePath)
        {
            return FileTypeChecks.CheckFileType(filePath);
        }
        private static bool IsFileFormatUnknown(FileFormats format)
        {
            if (format == FileFormats.Unknown || format == FileFormats.ResUnknown || format == FileFormats.ResIdxUnknown)
                return true;
            else
                return false;
        }
    }
}
