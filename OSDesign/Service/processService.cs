using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSDesign.config;
using OSDesign.Dao;
using OSDesign.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OSDesign.Service
{
    public class ProcessService
    {
        private processDao processDao = new processDao();
        public List<Process> FindAllProcesses()
        {
            return processDao.FindAllProcesses();
        }
       
        public int InsertProcess(Process process)
        {
            return processDao.InsertProcess(process);
        }
        public void DeleteProcess(int taskId)
        {
            processDao.DeleteProcess(taskId);
        }

    }
}
