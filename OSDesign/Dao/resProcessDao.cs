using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSDesign.config;
using OSDesign.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OSDesign.Dao
{
    class resProcessDao
    {
        DB dB = new DB();
        public string InsertProcess(ResProcess resProcess)
        {
            return dB.getInstance().Insertable(resProcess).ExecuteReturnEntity().timeNow;
        }
        internal List<ResProcess> FindAllProcesses()
        {
            return dB.getInstance().Queryable<ResProcess>().ToList();
        }
        public void DeleteProcess(string timeNow)
        {
            dB.getInstance().Deleteable<ResProcess>().In(timeNow).ExecuteCommand();
        }

        public ResProcess FindResProcessById(string timeNow)
        {
            return dB.getInstance().Queryable<ResProcess>().First(et => et.timeNow == timeNow);
        }
    }
}
