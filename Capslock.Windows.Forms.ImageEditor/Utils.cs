using System;
using System.Drawing;
using System.IO;

namespace Capslock.Windows.Forms
{
    public static class Utils
    {
        /// <summary>
        /// Compares the two specified images by converting each to a Base64 
        /// representation of the images (as PNGs) and comparing the strings
        /// </summary>
        /// <returns>The result of <see cref="string.Equals(string, string)"/></returns>
        /// <remarks>
        /// This idea and code is from http://codereview.stackexchange.com/a/39989. 
        /// It is licensed under CC-By-SA 3.0 (http://creativecommons.org/licenses/by-sa/3.0)
        /// </remarks>
        public static bool Compare(Image img1, Image img2)
        {
            if (img1 == null && img2 == null)       //if they're both null, they're equal
                return true;
            else if (img1 == null || img2 == null)  //if only one is null, they're not equal (and the code below would fail)
                return false;

            byte[] image1Bytes;
            byte[] image2Bytes;

            using (var mstream = new MemoryStream())
            {
                img1.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
                image1Bytes = mstream.ToArray();
            }

            using (var mstream = new MemoryStream())
            {
                img2.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
                image2Bytes = mstream.ToArray();
            }

            var image164 = Convert.ToBase64String(image1Bytes);
            var image264 = Convert.ToBase64String(image2Bytes);

            return string.Equals(image164, image264);
        }
    }
    //todo: Compare performance of unsafe vs string-based Bitmap comparison if there are any performance issues in setting the Image property
    //public static unsafe class FastBitmapComparer
    //{
    //    public static unsafe bool Compare(Bitmap bmp1, Bitmap bmp2)
    //    {
    //        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    //        sw.Start();

    //        if (bmp1 == null && bmp2 == null)       //if they're both null, they're equal
    //            return true;
    //        else if (bmp1 == null || bmp2 == null)  //if only one is null, they're not equal and the code below will fail
    //            return false;

    //        bool equals = true;

    //        Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
    //        BitmapData bmpData1 = bmp1.LockBits(rect, ImageLockMode.ReadOnly, bmp1.PixelFormat);
    //        BitmapData bmpData2 = bmp2.LockBits(rect, ImageLockMode.ReadOnly, bmp2.PixelFormat);
    //        unsafe
    //        {
    //            byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
    //            byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
    //            int width = rect.Width * 3; // for 24bpp pixel data
    //            for (int y = 0; equals && y < rect.Height; y++)
    //            {
    //                for (int x = 0; x < width; x++)
    //                {
    //                    if (*ptr1 != *ptr2)
    //                    {
    //                        equals = false;
    //                        break;
    //                    }
    //                    ptr1++;
    //                    ptr2++;
    //                }
    //                ptr1 += bmpData1.Stride - width;
    //                ptr2 += bmpData2.Stride - width;
    //            }
    //        }
    //        bmp1.UnlockBits(bmpData1);
    //        bmp2.UnlockBits(bmpData2);

    //        sw.Stop();
    //        System.Diagnostics.Debug.WriteLine($"Unsafe code time: {sw.Elapsed}");

    //        sw.Restart();

    //        byte[] image1Bytes;
    //        byte[] image2Bytes;

    //        using (var mstream = new MemoryStream())
    //        {
    //            bmp1.Save(mstream, bmp1.RawFormat);
    //            image1Bytes = mstream.ToArray();
    //        }

    //        using (var mstream = new MemoryStream())
    //        {
    //            bmp2.Save(mstream, bmp2.RawFormat);
    //            image2Bytes = mstream.ToArray();
    //        }

    //        var image164 = Convert.ToBase64String(image1Bytes);
    //        var image264 = Convert.ToBase64String(image2Bytes);

    //        equals = string.Equals(image164, image264);

    //        sw.Stop();
    //        System.Diagnostics.Debug.WriteLine($"String compare code time: {sw.Elapsed}");

    //        return equals;
    //    }
    //}
}
