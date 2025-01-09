using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSDesign.Entity;
using OSDesign.Service;

namespace OSDesign.View
{
    public partial class AddProcess : Form
    {
        private Process process = new Process();
        private ProcessService processService = new ProcessService();

        public AddProcess()
        {
            InitializeComponent();
        }

        private void AddProcess_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            process.id=int.Parse(textBox7.Text.Trim());
            process.jobs = textBox1.Text;
            process.statusNum = int.Parse(textBox2.Text.Trim()); ;
            process.lengthTLB = int.Parse(textBox3.Text.Trim());
            process.TLBtime = int.Parse(textBox4.Text.Trim());
            process.shortTime = int.Parse(textBox5.Text.Trim());
            process.stopTime = int.Parse(textBox6.Text.Trim());
            processService.InsertProcess(process);
            this.Close();

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
