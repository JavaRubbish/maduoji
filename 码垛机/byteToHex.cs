﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 码垛机
{
    class byteToHex
    {
        public static string byteToHexStr(byte[] bytes,int length)
        {
            string returnStr = "";
            if(bytes != null)
            {
                for(int i =0;i < length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;

                
        }
    }
}
