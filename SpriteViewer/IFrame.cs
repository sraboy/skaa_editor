using System;
using System.Drawing;

namespace SpriteViewer
{
    public interface IFrame
    {
        long BitmapOffset { get; }
        string Name { get; }
        Bitmap Bitmap { get; }
        Guid Guid { get; }
    }
}