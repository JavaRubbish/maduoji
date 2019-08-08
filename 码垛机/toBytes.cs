using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 码垛机
{
    class toBytes
    {
        public static byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)(value & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[3] = (byte)((value >> 24) & 0xFF);
            return src;
        }

    }
}
