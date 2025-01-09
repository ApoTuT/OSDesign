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
    public class resProcessService
    {
        private resProcessDao resProcessDao = new resProcessDao();
        public List<ResProcess> FindAllProcesses()
        {
            return resProcessDao.FindAllProcesses();
        }
        public ResProcess FindResProcessById(string timeNow)
        {
            return resProcessDao.FindResProcessById(timeNow);
        }
        public string InsertProcess(ResProcess resProcess)
        {
            return resProcessDao.InsertProcess(resProcess);
        }
        public void DeleteProcess(string  timeNow)
        {
            resProcessDao.DeleteProcess(timeNow);
        }

    }
}
