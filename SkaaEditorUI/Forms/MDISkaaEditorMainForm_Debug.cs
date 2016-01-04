﻿using System.Diagnostics;
using System.IO;
using System.Reflection;
using SkaaEditorUI.Forms.DockContentControls;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI.Forms
{
    partial class MDISkaaEditorMainForm
    {
        private string dataDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";

        [Conditional("DEBUG")]
        public void dbgOpenIButtonResIdxMultiBmp()
        {
            ResIdxMultiBmpPresenter spr = new ResIdxMultiBmpPresenter();
            spr.PalettePresenter = new ColorPalettePresenter();
            spr.PalettePresenter.Load(this.dataDir + "pal_std.res", null);
            spr.Load(dataDir + "i_button.res", this._gameSetViewerContainer.GameSetPresenter);

            ProjectManager.OpenSprites.Add(spr);

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewImageEditorContainerTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewImageEditorContainerTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
        }

        [Conditional("DEBUG")]
        public void dbgOpenBallistaSprite()
        {
            string dataDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";

            SpritePresenter spr = new SpritePresenter();
            spr.PalettePresenter = new ColorPalettePresenter();
            spr.PalettePresenter.Load(this.dataDir + "pal_std.res", null);
            spr.Load(dataDir + "ballista.spr");

            ProjectManager.OpenSprites.Add(spr);

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewImageEditorContainerTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewImageEditorContainerTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
        }

        [Conditional("DEBUG")]
        public void dbgOpenBallistaSpriteAndStandardGameSet()
        {
            string dataDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";

            SpritePresenter spr = new SpritePresenter();
            spr.PalettePresenter = new ColorPalettePresenter();
            spr.PalettePresenter.Load(this.dataDir + "pal_std.res", null);
            spr.Load(dataDir + "ballista.spr");

            ProjectManager.OpenSprites.Add(spr);

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewImageEditorContainerTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewImageEditorContainerTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }

            GameSetPresenter gsp = new GameSetPresenter();
            gsp.Load(dataDir + "std.set", true);
            this._gameSetViewerContainer.GameSetPresenter = gsp;

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
        }

        [Conditional("DEBUG")]
        public void dbgOpenIButtonResIdxMultiBmpAndStandardGameSet()
        {
            string dataDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";

            GameSetPresenter gsp = new GameSetPresenter();
            gsp.Load(dataDir + "std.set", true);
            this._gameSetViewerContainer.GameSetPresenter = gsp;

            ResIdxMultiBmpPresenter spr = new ResIdxMultiBmpPresenter();
            spr.PalettePresenter = new ColorPalettePresenter();
            spr.PalettePresenter.Load(this.dataDir + "pal_std.res", null);
            spr.Load(dataDir + "i_button.res", this._gameSetViewerContainer.GameSetPresenter);

            ProjectManager.OpenSprites.Add(spr);

            if (spr.Frames.Count > 0)
            {
                var doc = (this._dockPanel.ActiveDocument as ImageEditorContainer) ?? OpenNewImageEditorContainerTab();

                if (doc?.ActiveSprite == null) //no sprite is being viewed in the UI
                    SetActiveSprite(spr);
                else
                {
                    OpenNewImageEditorContainerTab();           //open a new document tab
                    SetActiveSprite(spr);
                }
            }

            SetSpriteDataViews(this._gameSetViewerContainer.GameSetPresenter);
        }
    }
}
