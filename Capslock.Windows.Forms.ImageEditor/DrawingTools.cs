#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
* Copyright © 2013-2015 Cyotek Ltd.
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

namespace Capslock.Windows.Forms.ImageEditor
{
    /// <summary>
    /// The various ways in which user mouse actions can interact with the image
    /// </summary>
    public enum DrawingTools
    {
        /// <summary>
        /// Defer to OS/application default
        /// </summary>
        None,
        /// <summary>
        /// Use a hand to "grab" the image and drag it up or down to pan while zoomed in
        /// </summary>
        Pan,
        /// <summary>
        /// Draw, pixel-by-pixel, or click and drag to scribble
        /// </summary>
        Pencil,
        /// <summary>
        /// Draw a straight line between two points
        /// </summary>
        Line,
        /// <summary>
        /// Fill an area with the specified color. The area is defined by bounds, 
        /// traditionally, by only filling pixels that are of the same color as 
        /// the pixel clicked on by the user.
        /// </summary>
        PaintBucket,
        /// <summary>
        /// Select a rectangular area of the image, given two points defining two diagonally-opposing corners
        /// </summary>
        SelectRectangle
    };
}
