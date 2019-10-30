using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static 码垛机.HomeForm;

namespace 码垛机
{
    class CenterPoint
    {
        public static void CaclCenterPoint(byte[] a,byte[] b,byte[] c,byte[] d)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x12;
            BF.sendbuf[2] = 0x0E;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = a[3];
            BF.sendbuf[5] = a[2];
            BF.sendbuf[6] = a[1];
            BF.sendbuf[7] = a[0];
            BF.sendbuf[8] = b[3];
            BF.sendbuf[9] = b[2];
            BF.sendbuf[10] = b[1];
            BF.sendbuf[11] = b[0];
            BF.sendbuf[12] = c[3];
            BF.sendbuf[13] = c[2];
            BF.sendbuf[14] = c[1];
            BF.sendbuf[15] = c[0];
            BF.sendbuf[16] = d[3];
            BF.sendbuf[17] = d[2];
            BF.sendbuf[18] = d[1];
            BF.sendbuf[19] = d[0];
            BF.sendbuf[20] = 0xF5;
            SendMenuCommand(BF.sendbuf, 21);
        }
    }
}
