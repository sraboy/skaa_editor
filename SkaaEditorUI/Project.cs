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
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using SkaaEditorUI.Misc;
using SkaaEditorUI.Presenters;

namespace SkaaEditorUI
{
    public class Project
    {
        private static readonly TraceSource Logger = new TraceSource($"{typeof(Project)}", SourceLevels.All);

        #region Events
        [NonSerialized]
        private EventHandler _activeSpriteChanged;
        public event EventHandler ActiveSpriteChanged
        {
            add
            {
                if (_activeSpriteChanged == null || !_activeSpriteChanged.GetInvocationList().Contains(value))
                {
                    _activeSpriteChanged += value;
                }
            }
            remove
            {
                _activeSpriteChanged -= value;
            }
        }
        protected virtual void OnActiveSpriteChanged(EventArgs e)
        {
            EventHandler handler = _activeSpriteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private TrulyObservableCollection<SpritePresenter> _openSprites;
        private DataSet _gameSet;
 
        public TrulyObservableCollection<SpritePresenter> OpenSprites
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
        public DataSet GameSet
        {
            get
            {
                return _gameSet;
            }

            set
            {
                this._gameSet = value;
            }
        }

        

        public Project()
        {
        }

        public void AddSprite(SpritePresenter spr)
        {
            this.OpenSprites = this.OpenSprites ?? new TrulyObservableCollection<SpritePresenter>();
            this.OpenSprites.Add(spr);
        }

        //public List<Stream> GetProjectStreams()
        //{
        //    List<Stream> save = new List<Stream>();

        //    // ActiveSprite //
        //    save.Add(GetActiveSpriteStream());
        //    // GameSet //
            
        //}


    }
}
