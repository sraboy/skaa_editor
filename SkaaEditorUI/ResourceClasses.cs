/***************************************************************************
*   This file is part of SkaaEditor, a binary file editor for 7KAA.
*
*   Copyright (C) 2015  Steven Lavoie  steven.lavoiejr@gmail.com
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program; if not, write to the Free Software Foundation,
*   Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*
*   SkaaEditor is capable of viewing and/or editing binary files from 
*   Enlight Software's Seven Kingdoms: Ancient Adversaries (7KAA). All code
*  	is licensed under GPLv3, including any code from Enlight Software. For
*  	information on 7KAA, visit http://www.7kfans.com.
***************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;
//using System.IO;

//namespace SkaaEditor
//{
//    public class Resource
//    {
//        public string resourceName;
//        public string resourceType;
//        public int dataLocation;
//        public int dataSize;
//        public byte[] dataStream;

//        public Resource() { }

//        public Resource(string name, int location)
//        {
//            resourceName = name;
//            dataLocation = location;
//            //initDataStream();
//        }

//        public void initDataStream()
//        {
//            dataStream = new byte[dataSize];
//        }
//    }

//    public class ImageResource : Resource
//    {
//        public Bitmap bmp;

//        public ImageResource(string name, int location)
//        {
//            resourceName = name;
//            dataLocation = location;
//            //initDataStream();            
//        }

//        public Bitmap initIMGStream()
//        {
//            byte[] imgData = new byte[dataStream.Length - 4];
//            int width = (short) (dataStream[0] + dataStream[1]), height = (short) (dataStream[2] + dataStream[3]);

//            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

//            /*********************************
//             * Strip the first four bytes that 
//             * contain the width and height
//             *********************************/
//            for (int b = 4; b < dataStream.Length; b++)
//            {
//                imgData[b - 4] = dataStream[b];
//            }

//            //TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
//            //bmp = (Bitmap)tc.ConvertFrom(imgData);


//            //Create a BitmapData and Lock all pixels to be written 
//            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
//            System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

//            //Copy the data from the byte array into BitmapData.Scan0
//            //System.Runtime.InteropServices.Marshal.Copy(imgData, 0, bmpData.Scan0, imgData.Length);

//            int offset = 0;
//            long ptr = bmpData.Scan0.ToInt64();
//            for (int i = 0; i < height; i++)
//            {
//                System.Runtime.InteropServices.Marshal.Copy(imgData, offset, new IntPtr(ptr), width * 3);
//                offset += width * 3;
//                ptr += bmpData.Stride;
//            }

//            //Unlock the pixels
//            bmp.UnlockBits(bmpData);

//            return bmp;
//        }

//    }


//    /// <summary>
//    /// Summary of RES file:
//    ///1. 2 bytes: number of records
//    ///2. 8 bytes: resource name
//    ///3. 1 byte : padding
//    ///4. 4 bytes: resource location in file
//    ///5. the rest: binary bitmap data
//    /// 
//    /// There is one additional, "virtual" record listed with a blank name. Items
//    /// 2-4 repeat for each record plus the virtual record. The virtual record was
//    /// created to help calculate datasizes easily in C++. With C# byte arrays, it's
//    /// not necessary to read, or so it seems. The 4 bytes following the virtual
//    /// record indicate the size of the file instead of a location.
//    /// </summary>
//    public class ResourceClasses
//    {
//        int headerSize = 2; //2 byte header is a uint16 with the number of resources in the files

//        public ImageResource[] Resources;
//        byte[] fileBytes;

//        /// <summary>
//        /// Builds the class and parses out resources from the file
//        /// </summary>
//        /// <param name="file">RES file to parse</param>
//        public ResourceClasses(string file)
//        {
//            fileBytes = File.ReadAllBytes(file);

//            //parserFileBytes will contain all the data minus the first two bytes
//            byte[] parserFileBytes = new byte[fileBytes.Length];

//            //it's easier to parse the file without worrying about the 2-byte offset
//            //from the recCount
//            Array.Copy(fileBytes, 2, parserFileBytes, 0, fileBytes.Length - 2);

//            int recCount = Convert.ToInt32(fileBytes[0] + fileBytes[1]);
//            Resources = new ImageResource[recCount];


//            /********************************************* 
//             * copies every 13 bytes into a byte array
//             * so they can be parsed out with the first
//             * 9 bytes as the resource name and next 4
//             * bytes as the location in the file of that 
//             * particular resource
//             ********************************************/
//            for (int r = 0; r < recCount * 13; r += 13)
//            {
//                byte[] b = new byte[13];
//                char[] name = new char[9];
//                byte[] loc = new byte[4];

//                Array.Copy(parserFileBytes, r, b, 0, 13);

//                Array.Copy(b, 0, name, 0, 9);
//                Array.Copy(b, 9, loc, 0, 4);

//                Resources[r / 13] = new ImageResource(name.ToString(), BitConverter.ToInt32(loc, 0));
//            }


//            for (int r = 0; r < recCount; r++)
//            {
//                /********************************************* 
//                * Calculate the dataSize for each Resource[r]
//                * by subtracting the offsets from one another
//                ********************************************/
//                if (r == recCount - 1)
//                    Resources[r].dataSize = fileBytes.Length - Resources[r].dataLocation;
//                else
//                    Resources[r].dataSize = Resources[r + 1].dataLocation - Resources[r].dataLocation;

//                /********************************************* 
//                * Copy the BMP data from the RES file to the
//                * appropriate Resource[] element. The BMP data
//                * begins with two short (4 bytes) elements
//                * which are the width and height of the BMP
//                ********************************************/
//                Resources[r].dataStream = new byte[Resources[r].dataSize];
//                Array.Copy(fileBytes, Resources[r].dataLocation, Resources[r].dataStream, 0, Resources[r].dataSize);
//            }

//        }
//    }
//}
