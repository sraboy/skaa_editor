﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkaaGameDataLib;

namespace SkaaEditorUI
{
    public class Project
    {
        private static readonly TraceSource Logger = new TraceSource($"{typeof(Project)}", SourceLevels.All);

        private List<SpritePresenter> _openSprites;
        private SpritePresenter _activeSprite;
        private ColorPalette _activePalette;
        private DataSet _gameSet;

        public SpritePresenter ActiveSprite
        {
            get
            {
                return _activeSprite;
            }

            private set
            {
                this._activeSprite = value;
            }
        }
        public ColorPalette ActivePalette
        {
            get
            {
                return _activePalette;
            }

            private set
            {
                this._activePalette = value;
            }
        }
        public List<SpritePresenter> OpenSprites
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

            private set
            {
                this._gameSet = value;
            }
        }

        

        public Project()
        {
            this.ActiveSprite.SpriteUpdated += ActiveSprite_SpriteUpdated;
        }

        private void ActiveSprite_SpriteUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public List<Stream> GetProjectStreams()
        {
            List<Stream> save = new List<Stream>();

            // ActiveSprite //
            save.Add(GetActiveSpriteStream());
            // GameSet //
            
        }

        public Stream GetActiveSpriteStream()
        {
            var spr = new MemoryStream();
            var sprBytes = this.ActiveSprite.GetSpriteFrameByteArrays();
            foreach (byte[] ba in sprBytes)
                spr.Write(ba, 0, ba.Length);

            return spr;
        }
    }
}
