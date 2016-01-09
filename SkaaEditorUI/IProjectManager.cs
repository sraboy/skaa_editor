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
using SkaaEditorUI.Forms;
using SkaaEditorUI.Presenters;
using TrulyObservableCollection;

namespace SkaaEditorUI
{
    public interface IProjectManager
    {
        GameSetPresenter GameSet { get; set; }
        bool IsInTempDirectory { get; }
        TrulyObservableCollection<MultiImagePresenterBase> OpenSprites { get; }
        string SaveDirectory { get; }
        string TempDirectory { get; }

        event EventHandler ActiveSpriteChanged;
        void CloseProject();
        void CleanTempFiles();
        IPresenterBase<T> Open<T, T1>(params object[] param)
            where T : class
            where T1 : IPresenterBase<T>, new();
        void Save<T>(IPresenterBase<T> pres) where T : class;
        void SetMainForm(MDISkaaEditorMainForm form);
        void SetSpriteDataViews();
    }
}