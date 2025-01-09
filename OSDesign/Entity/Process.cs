using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDesign.Entity
{
    public class Process
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int id { get; set; }
        public string jobs { get; set; }
        public int statusNum { get; set; }
        public int lengthTLB { get; set; }
        public int TLBtime { get; set; }
        public int shortTime { get; set; }
        public int stopTime { get; set; }


        public Process() { }
        public Process(int id,string jobs,int statusNum, int lengthTLB, int tLBtime, int shortTime, int stopTime)
        {
            this.id = id;
            this.jobs = jobs;
            this.statusNum = statusNum;
            this.lengthTLB = lengthTLB;
            TLBtime = tLBtime;
            this.shortTime = shortTime;
            this.stopTime = stopTime;
        }
    }
}
