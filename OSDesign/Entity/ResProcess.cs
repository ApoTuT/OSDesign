using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDesign.Entity
{
    public class ResProcess
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string timeNow { get; set; }
        public int resId { get; set; }
        public string message { get; set; }
        public ResProcess() { }
        public ResProcess(string timeNow,int resId,string message)
        {
           this.timeNow = timeNow;
           this.resId = resId;
           this.message = message;
        }
    }
}
