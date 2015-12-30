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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SkaaEditorUI.Forms;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib;
using TrulyObservableCollection;

namespace SkaaEditorUI
{
    /// <summary>
    /// A singleton class that will manage a <see cref="Project"/>'s file operations
    /// </summary>
    public class ProjectManager
    {
        private static readonly TraceSource Logger = new TraceSource($"{typeof(ProjectManager)}", SourceLevels.All);


        #region Private Members
        private MDISkaaEditorMainForm _mainForm;
        private TrulyObservableCollection<MultiImagePresenterBase> _openSprites;
        //private GameSetPresenter _gameSetPresenter = new GameSetPresenter();

        private bool _hasUnsavedChanges = false; //todo: track image changes so we know if items are unsaved
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
        public TrulyObservableCollection<MultiImagePresenterBase> OpenSprites
        {
            get
            {
                return _openSprites;
            }

            private set
            {
                this._openSprites = value;
            }
        }
        //public GameSetPresenter GameSet
        //{
        //    get
        //    {
        //        return _gameSetPresenter;
        //    }

        //    set
        //    {
        //        this._gameSetPresenter = value;

        //    }
        //}
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
            if (format == FileFormats.Unknown ||
                format == FileFormats.ResUnknown ||
                format == FileFormats.ResIdxUnknown)
                return true;
            else
                return false;
        }
        #endregion

        #region Public Methods

        //////////////////////////////// Setup/Teardown/Utilities ////////////////////////////////
        public ProjectManager()
        {
            this._tempFiles.Add(this.TempDirectory);
            this.OpenSprites = new TrulyObservableCollection<MultiImagePresenterBase>();
        }
        public void SetMainForm(MDISkaaEditorMainForm form)
        {
            this._mainForm = form;
            this._mainForm.FormClosed += MainForm_FormClosed;
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
        public static FileFormats CheckFileType(string filePath)
        {
            return FileTypeChecks.CheckFileType(filePath);
        }

        /////////////////////////////////// Project Management ///////////////////////////////////

        /// <summary>
        /// Creates a new <see cref="Project"/> in <see cref="TempDirectory"/> 
        /// </summary>
        /// <returns>A new <see cref="Project"/></returns>
        public void CreateNewProject()
        {
            this.IsInTempDirectory = true;
            //return CreateNewProject(this.TempDirectory);
        }

        public bool SaveProject(string filePath)
        {
            //SaveSprites(/*FileFormats.SpriteSpr*/);

            //using (FileStream fs = new FileStream(this.SaveDirectory, FileMode.Create))
            //{
            //    var str = this.GameSet.GetStandardGameSetStream();
            //    str.Position = 0;
            //    str.CopyTo(fs);
            //}

            Logger.TraceInformation($"Saved {typeof(ProjectManager)} in {filePath}.");
            return true;
        }
        /// <summary>
        /// Closes the <see cref="ActiveProject"/>
        /// </summary>
        public void CloseProject()
        {
            SaveProject(this.SaveDirectory ?? this.TempDirectory);
            CleanTempFiles();
        }

        private void MainForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            CloseProject();
        }

        /////////////////////////////////// Presenter Management ///////////////////////////////////
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

            //This method signature is really verbose, which is a pain for the caller, but it allows this one single method
            //to open SpritePresenters, ResIdxMultiBmpPresenters and GameSetPresenters. T1 is necessary to specify either
            //SkaaSprite or DataSet

            T1 presenter = new T1();

            if (presenter is MultiImagePresenterBase)
            {
                (presenter as MultiImagePresenterBase).PalettePresenter = new ColorPalettePresenter(this._mainForm.ActivePalette);
                this.OpenSprites.Add(presenter as MultiImagePresenterBase);
            }

            presenter.Open<T>(param);
            return presenter;
        }

        public void Save<T>(IPresenterBase<T> pres) where T : class
        {
            var result = pres.Save<T>(null);
            if (result == false)
                Debugger.Break(); //still need to refactor the FileDialog stuff so we can get a more useful return value
        }

        public void SetSpriteDataViews(GameSetPresenter gsp)
        {
            if (gsp == null)
                return;

            foreach (var spr in this.OpenSprites)
                spr.SetSpriteDataView(gsp);
        }
        #endregion


    }
}
