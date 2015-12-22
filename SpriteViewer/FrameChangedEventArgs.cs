using System;

namespace SpriteViewer
{
    public class FrameChangedEventArgs : EventArgs
    {
        private Guid _frameGuid;
        //private Frame _selectedFrame;

        //public Frame SelectedFrame
        //{
        //    get
        //    {
        //        return _selectedFrame;
        //    }

        //    private set
        //    {
        //        this._selectedFrame = value;
        //    }
        //}
        public Guid FrameGuid
        {
            get
            {
                return _frameGuid;
            }

            set
            {
                this._frameGuid = value;
            }
        }

        //public FrameChangedEventArgs(Frame SelectedFrame)
        //{
        //    this.SelectedFrame = SelectedFrame;
        //    this.FrameGuid = SelectedFrame.Guid;
        //}
        public FrameChangedEventArgs(Guid SelectedFrameGuid)
        {
            this.FrameGuid = SelectedFrameGuid;
        }
    }
}
