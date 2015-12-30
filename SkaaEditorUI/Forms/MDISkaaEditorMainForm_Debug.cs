using System.Diagnostics;
using System.IO;
using System.Reflection;
using SkaaEditorUI.Forms.DockContentControls;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI.Forms
{
    partial class MDISkaaEditorMainForm
    {
        [Conditional("DEBUG")]
        public void OpenBallistaSpriteAndStandardGameSet()
        {
            string dataDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";

            //OpenSprite<SpritePresenter>();
            //ColorPalettePresenter pal = new ColorPalettePresenter();
            //pal.Load(dataDir + "pal_std.res");
            SpritePresenter spr = new SpritePresenter();
            spr.LoadPalette(dataDir + "pal_std.res");
            spr.Load(dataDir + "ballista.spr");

            ProjectManager.OpenSprites.Add(spr);

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }

            GameSetPresenter gsp = new GameSetPresenter();
            gsp.Load(dataDir + "std.set", null, true);
            this._gameSetViewerContainer.GameSetPresenter = gsp;

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
        }
    }
}
