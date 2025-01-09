using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDesign.Entity
{
    public class Job
    {
        public int name { get; set; }
        public string address { get; set; }
        
        public string status { get; set; }//true表示存在缺页  false表示不缺页
        public int times { get; set; } 

        public int record { get; set; }
        public Job()
        {
            status= "Yes";
            times= 0;
            record= 0;
        }
        public Job(int name, string address,string status,int times)
        {
            this.name = name;
            this.address = address;
            this.status = status;
            this.times = times;
        }
    }
}
