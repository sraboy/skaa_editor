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
using SkaaEditorUI.Presenters;

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

        #region Private Methods
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
        /// <summary>
        /// Checks whether the specified format is any kind of unknown format
        /// </summary>
        private static bool IsFileFormatUnknown(FileFormats format)
        {
            if (format == FileFormats.Unknown || format == FileFormats.ResUnknown || format == FileFormats.ResIdxUnknown)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Loads a <see cref="ColorPalette"/> from the specified file
        /// </summary>
        /// <param name="filePath">The 7KAA-formatted file</param>
        /// <returns>A new <see cref="ColorPalettePresenter"/></returns>
        private static ColorPalettePresenter LoadPalette(string filePath)
        {
            var pal = PaletteLoader.FromResFile(filePath);

            if (pal == null)
                Logger.TraceEvent(TraceEventType.Error, 0, $"{typeof(PaletteLoader)} returned null. Failed to load palette: {filePath}");

            return new ColorPalettePresenter(pal);
        }

        #endregion

        #region Public Methods
        // Set up //
        public void SetMainForm(MDISkaaEditorMainForm form)
        {
            this._mainForm = form;
        }
        /// <summary>
        /// Creates a new <see cref="Project"/> in <see cref="TempDirectory"/> 
        /// </summary>
        /// <returns>A new <see cref="Project"/></returns>
        // Managing the Project //
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
        // Opening Files //
        public T Open<T>() where T : PresenterBase<T>, new()
        {
            T presenter = new T();
            presenter.Open();
            return presenter;
        }
        //public void OpenStandardSet()
        //{
        //    GameSetPresenter gsp = new GameSetPresenter();
        //    using (var dlg = gsp.GetOpenFileDialog())
        //        gsp.GameSet = dlg.Open(this.SaveDirectory, () => LoadStandardGameSet(dlg.FileName));
        //    this.ActiveProject.GameSet = gsp.GameSet;
        //}
        //public void OpenPalette()
        //{
        //    ColorPalettePresenter pal = new ColorPalettePresenter(null);
        //    using (var dlg = pal.GetOpenFileDialog())
        //        pal = dlg.Open(this.SaveDirectory, () => LoadPalette(dlg.FileName));
        //    this.ActiveProject.ActivePalette = pal.ColorPalette;
        //}
        //public void OpenSprite()
        //{
        //    SpritePresenter spr = new SpritePresenter();
        //    using (var dlg = spr.GetOpenFileDialog())
        //        spr = dlg.Open(this.SaveDirectory, () => LoadSprite(dlg.FileName, this._mainForm.ActivePalette));
        //    this.ActiveProject.AddSprite(spr);
        //}
        public static FileFormats CheckFileType(string filePath)
        {
            return FileTypeChecks.CheckFileType(filePath);
        }
        #endregion
    }
}
