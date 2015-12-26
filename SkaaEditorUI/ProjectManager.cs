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
using System.Diagnostics;
using System.IO;
using SkaaGameDataLib;
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
        /// Creates a new <see cref="TempDirectory"/> in <see cref="Path.GetTempPath()"/>
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
        #endregion

        #region Public Methods
        public void SetMainForm(MDISkaaEditorMainForm form)
        {
            this._mainForm = form;
            this._mainForm.FormClosed += _mainForm_FormClosed;
        }
        public void CleanTempFiles()
        {
            foreach (string dir in this._tempFiles)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    Trace.WriteLine($"Temp directory deleted: {dir}");
                }
            }
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
            SaveSprites(/*FileFormats.SpriteSpr*/);

            using (FileStream fs = new FileStream(this.SaveDirectory, FileMode.Create))
            {
                var str = this.ActiveProject.GameSet.GetStandardGameSetStream();
                str.Position = 0;
                str.CopyTo(fs);
            }

                Logger.TraceInformation($"Saved {typeof(Project)} in {filePath}.");
            return true;
        }
        public void SaveSprites(/*FileFormats format*/)
        {
            //call Save on each MultiImagePresenter
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
        /// Calls the <see cref="IPresenterBase{T}.Open{T1}(object)"/> method of the specified type
        /// </summary>
        /// <typeparam name="T">A SkaaGameDataLib object</typeparam>
        /// <typeparam name="T1">An <see cref="IPresenterBase{T}"/> object on which to call Open()</typeparam>
        /// <returns></returns>
        public IPresenterBase<T> Open<T, T1>(params object[] param) where T : class where T1 : IPresenterBase<T>, new()
        {
            //param[0] is FileFormat
            //param[1] is bool merge for GameSetPresenter
            //param[2] is GameSet for SpritePresenter
            T1 presenter = new T1();

            if (presenter is MultiImagePresenterBase)
                (presenter as MultiImagePresenterBase).PalettePresenter = this._mainForm.ActivePalette;

            presenter.Open<T>(param);
            return presenter;
        }

        //public MultiImagePresenterBase Open<T, T1>() where T : class where T1 : MultiImagePresenterBase, new()
        //{
        //    T1 presenter = new T1();
        //    (presenter as MultiImagePresenterBase).PalettePresenter = this._mainForm.ActivePalette;
        //    presenter.Open();
        //    return presenter;
        //}
        //public IMultiImagePresenter<T> Open<T, T1>(ColorPalettePresenter pal) where T : SkaaSprite where T1 : MultiImagePresenterBase, new()
        //{
        //    T1 spr = new T1();
        //    spr.PalettePresenter = pal;
        //    spr.Open();
        //    return (IMultiImagePresenter<T>) spr;
        //}

        public static FileFormats CheckFileType(string filePath)
        {
            return FileTypeChecks.CheckFileType(filePath);
        }
        #endregion

        #region Event Handlers
        private void _mainForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            CleanTempFiles();
            //throw new NotImplementedException();
        }
        #endregion
    }
}
