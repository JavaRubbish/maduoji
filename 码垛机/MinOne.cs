using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 码垛机
{
    class MinOne
    {
        public static int findMin(int[] arr)
        {
            Array.Sort(arr);
            return arr[0];
        } 
    }
}
