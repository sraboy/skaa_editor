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