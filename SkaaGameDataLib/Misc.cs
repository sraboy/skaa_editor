using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SkaaGameDataLib
{
    public static class Misc
    {
        public static uint? GetNullableUInt32FromIndex(this DataRow dr, int itemArrayIndex)
        {
            string x = dr[itemArrayIndex].ToString();

            /* Codepage 437 is needed due to the use of Extended ASCII.
             *
             * For example, in SFRAME.dbf, the BITMAPPTR is labeled as a
             * CHAR type (which is just a string in dBase III parlance).
             * Because of this, it's not possible to read the bytes in as
             * numbers, even though they're just used as pointers in the 
             * original code.
             */
            byte[] bytes = Encoding.GetEncoding(437).GetBytes(x);

            uint? offset = null;

            switch (bytes.Length)
            {
                case 0:
                    offset = 0;
                    break;
                case 1:
                    offset = bytes[0];
                    break;
                case 2:
                    offset = BitConverter.ToUInt16(bytes, 0);
                    break;
                case 3:
                    byte[] copy = new byte[4];
                    bytes.CopyTo(copy, 0);
                    offset = BitConverter.ToUInt32(copy, 0);
                    break;
                case 4:
                    offset = BitConverter.ToUInt32(bytes, 0);
                    break;
                default:
                    throw new ArgumentNullException("There was an issue reading offsets from the sprite's DataTable that will cause the variable \'offset\' to remain null.");
            }

            return offset;
        }
        public static int CompareFrameOffset(SpriteFrame a, SpriteFrame b)
        {
            if (a.SprBitmapOffset <= b.SprBitmapOffset)
                return (int) a.SprBitmapOffset;
            else
                return (int) b.SprBitmapOffset;

            //uint? aLen = a.GameSetDataRow.GetNullableUInt32FromIndex(9);
            //uint? bLen = b.GameSetDataRow.GetNullableUInt32FromIndex(9);

            //if (aLen.Value <= bLen.Value)
            //    return (int) aLen.Value;
            //else
            //    return (int) bLen.Value;
        }
    }
}
