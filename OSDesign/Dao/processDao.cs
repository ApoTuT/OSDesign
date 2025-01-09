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
    class processDao
    {
       DB dB = new DB();
        public int InsertProcess(Process process)
        {
            return dB.getInstance().Insertable(process).ExecuteReturnEntity().id;
        }
        internal List<Process> FindAllProcesses()
        {
            return dB.getInstance().Queryable<Process>().ToList();
        }
        public void DeleteProcess(int taskId)
        {
            dB.getInstance().Deleteable<Process>().In(taskId).ExecuteCommand();
        }
    }
}
