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
using System.Diagnostics;
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

            SpriteSprPresenter spr = new SpriteSprPresenter();
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

            SpriteSprPresenter spr = new SpriteSprPresenter();
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
