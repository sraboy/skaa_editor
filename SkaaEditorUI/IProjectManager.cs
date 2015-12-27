using SkaaEditorUI.Forms;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI
{
    public interface IProjectManager
    {
        Project ActiveProject { get; }
        bool IsInTempDirectory { get; }
        string SaveDirectory { get; }
        string TempDirectory { get; }

        void CleanTempFiles();
        void CloseProject();
        Project CreateNewProject();
        Project CreateNewProject(string filePath);
        IPresenterBase<T> Open<T, T1>(params object[] param)
            where T : class
            where T1 : IPresenterBase<T>, new();
        void Save<T>(IPresenterBase<T> pres) where T : class;
        bool SaveProject(Project project, string filePath);
        void SetMainForm(MDISkaaEditorMainForm form);
    }
}