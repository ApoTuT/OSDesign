using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDesign.View
{
    internal class Request
    {
        public static ArrayList FIFOArrayList = new ArrayList();
        public static ArrayList LRUArrayList = new ArrayList();
        public static ArrayList OPTArrayList = new ArrayList();
        public static double FIFO = 0;
        public static double LRU = 0;
        public static double OPT = 0;
    }
}
