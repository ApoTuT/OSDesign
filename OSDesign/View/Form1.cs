using OSDesign.Entity;
using OSDesign.View;
using OSDesign.Service;
using Time=System.Windows.Forms;
using SqlSugar;
using System;
using System.Collections.Generic;
using static System.Threading.Thread;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace OSDesign
{
    public partial class Form1 : Form
    {
        string address;
        int statusNum;
        int lengthTLB;//快表TLB的大小，可以容纳的个数
        int TLBtime;//访问TLB的时间
        int shortTime;//访问内存的时间
        int stopTime;//缺页产生中断的时间
        int FIFOTime = 0;
        int LRUTime = 0;
        int OPTTime = 0;
        double FIFOLackPrecent = 0.0;
        double LRULackPrecent = 0.0;
        double OPTLackPrecent = 0.0;
        int FIFOLackNum = 0;
        int LRULackNum = 0;
        int OPTLackNum = 0;
        int currentTime = 0;
        string resMessage = "";
        string resMessage2 = "";
        string resMessage3 = "";
        public Process processAll = new Process();
        List<Process> processList = new List<Process>();
        ProcessService processService = new ProcessService();
        resProcessService resProcessService1 = new resProcessService();
        private System.Windows.Forms.Timer timer;
        //Time.Timer timer = new Time.Timer();

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; // 设置窗口最大化
            InitializeTimer();


            label33.Text = null;
            label34.Text = null;
            label35.Text = null;
            label36.Text = null;
            label37.Text = null;
            label38.Text = null;
            label4.Text = null;
            label5.Text = null;
            label6.Text = null;

            processList = processService.FindAllProcesses();
            dataGridView4.DataSource = processList;
            List<ResProcess> resProcessList = resProcessService1.FindAllProcesses();
            dataGridView5.DataSource = resProcessList;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        //十六进制数转为二进制数
        public string changeToBinary(string str)
        {
            int decimalValue = Convert.ToInt32(str, 16);
            string binary = Convert.ToString(decimalValue, 2);
            int length = str.Length * 4;
            return binary.PadLeft(length, '0');
        }

        private void InitializeTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // 每秒更新一次
            timer.Tick += Timer_Tick; // 绑定事件
            timer.Start(); // 启动定时器
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshPlot(); // 更新数据
        }
        private void RefreshPlot()
        {
            formsPlot1.Plot.Clear(); // 清空现有图表
            formsPlot2.Plot.Clear();
            // 更新数据
            double[] fifoData = new double[Request.FIFOArrayList.Count];
            double[] fifoX = new double[Request.FIFOArrayList.Count];
            double[] lruData = new double[Request.LRUArrayList.Count];
            double[] lruX = new double[Request.LRUArrayList.Count];
            double[] optData = new double[Request.OPTArrayList.Count];
            double[] optX = new double[Request.OPTArrayList.Count];
            double[] values = { Request.FIFO * 10, Request.LRU * 10, Request.OPT * 10 };
            for (int i = 0; i < Request.FIFOArrayList.Count; i++)
            {
                fifoData[i] = (int)Request.FIFOArrayList[i];
                fifoX[i] = i;
            }
            for (int i = 0; i < Request.LRUArrayList.Count; i++)
            {
                lruData[i] = (int)Request.LRUArrayList[i];
                lruX[i] = i;
            }
            for (int i = 0; i < Request.OPTArrayList.Count; i++)
            {
                optData[i] = (int)Request.OPTArrayList[i];
                optX[i] = i;
            }
            string[] tickLabels = optX.Select(x => x.ToString("G")).ToArray();
            double[] logTicks = optX.Select(x => Math.Log10(x)).ToArray();
            formsPlot1.Plot.Axes.SetLimitsX(0, optX.Length);
            formsPlot1.Plot.Axes.SetLimitsY(0, 5000);
            // 绘制信号
            formsPlot1.Plot.Add.Scatter(fifoX, fifoData);
            formsPlot1.Plot.Add.Scatter(lruX, lruData);
            formsPlot1.Plot.Add.Scatter(optX, optData);
            formsPlot1.Plot.XLabel("costing time");
            formsPlot1.Plot.YLabel("times");
            formsPlot2.Plot.Axes.SetLimitsX(-0.5, 3);
            formsPlot2.Plot.Axes.SetLimitsY(0, 10);
            formsPlot2.Plot.Add.Bars(values);
            int[] num = { 1, 2, 3 };

            
            formsPlot2.Plot.XLabel("缺页率");
            formsPlot1.Refresh(); // 刷新图表
            formsPlot2.Refresh();
        }

        //FIFO算法实现

        public void FIFOMethod()
        {
            if (processAll == null)
            {
                MessageBox.Show("请选择符合条件的地址信息");
            }
            else
            {
                List<Job> jobs = new List<Job>();
                string[] strs = processAll.jobs.Split(",");
                string[] names = new string[strs.Length];
                Queue<Job> myTLB = new Queue<Job>();
                int TLBnum = 0;
                for (int i = 0; i < strs.Length; i++)
                {
                    string tmp = changeToBinary(strs[i]);
                    Job job = new Job();
                    job.address = tmp;
                    string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                    int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                    job.name = num;
                    names[i] = num.ToString();
                    jobs.Add(job);
                }
                List<Job> myList = new List<Job>();
                int maxSize = processAll.statusNum;
                int flag = 0;
                string res = "";
                resMessage = "";
                resMessage += "使用方法为FIFO ";
                resMessage += "作业为：";
                int resNum = 0;
                for (int i = 0; i < jobs.Count; i++)
                {
                    resMessage += jobs[i].name + " ";
                    Sleep(currentTime);
                    bool flag1 = false;
                    if (flag < maxSize)
                    {
                        //检查是否有重复的
                        foreach (Job tmp in myList)
                        {
                            if (tmp.name == jobs[i].name)
                            {
                                flag1 = true;
                                break;
                            }
                        }
                        if (flag1 == false)
                        {
                            myList.Add(jobs[i]);
                            jobs[i].times++;//它使用的次数要加一

                            flag++;
                        }
                        for (int j = 0; j < flag; j++)
                        {
                            if (i > 0 && dataGridView1.Rows[j].Cells[i].Value != null)
                            {

                                dataGridView1.Rows[j].Cells[i + 1].Value = dataGridView1.Rows[j].Cells[i].Value;
                            }
                            else
                            {
                                dataGridView1.Rows[j].Cells[i + 1].Value = jobs[i].name.ToString();
                            }

                        }

                    }
                    else if (flag == maxSize)
                    {
                        //判断缺页情况
                        foreach (Job tmp in myList)
                        {
                            if (tmp.name == jobs[i].name)
                            {
                                flag1 = true;
                                break;
                            }
                        }

                        List<Job> list = new List<Job>();
                        for (int j = 0; j < flag; j++)
                        {

                            bool judge = false;
                            foreach (Job tmp in myList)
                            {
                                if (tmp.name == jobs[i].name)
                                {
                                    judge = true;
                                }
                                list.Add(tmp);
                            }
                            if (judge == false)
                            {
                                //获取到最大的次数和最大次数的名字
                                int timeMax = 0;
                                int nameMax = 0;
                                foreach (Job tmp in myList)
                                {
                                    if (tmp.times > timeMax)
                                    {
                                        timeMax = tmp.times;
                                        nameMax = tmp.name;
                                    }
                                }
                                for (int t = 0; t < flag; t++)
                                {
                                    if (myList[t].name == nameMax)
                                    {
                                        myList[t] = jobs[i];
                                    }
                                }
                                for (int t = 0; t < flag; t++)
                                {
                                    list.Add(myList[t]);
                                }
                            }
                            dataGridView1.Rows[j].Cells[i + 1].Value = myList[j].name.ToString();
                        }
                    }
                    bool TLBstatue = false;
                    if (TLBnum < processAll.lengthTLB)
                    {
                        foreach (Job tmp in myTLB)
                        {
                            if (jobs[i] == tmp)
                            {
                                TLBstatue = true;
                            }
                        }
                        if (TLBstatue == false)
                        {
                            myTLB.Enqueue(jobs[i]);
                            TLBnum++;
                        }
                    }
                    else if (TLBnum == processAll.lengthTLB)
                    {
                        foreach (Job tmp in myTLB)
                        {
                            if (jobs[i] == tmp)
                            {
                                TLBstatue = true;
                            }
                        }
                        if (TLBstatue == false && myTLB != null)
                        {
                            myTLB.Dequeue();
                            myTLB.Enqueue(jobs[i]);
                        }
                    }
                    if (flag1 == true)
                    {
                        dataGridView1.Rows[maxSize].Cells[i + 1].Value = "No";
                        //不缺页但是快表里面没有
                        if (TLBstatue == false)
                        {
                            currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                            FIFOTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                        }
                        else if (TLBstatue == true)
                        {
                            //不缺页但是快表里面有
                            currentTime = processAll.TLBtime + processAll.shortTime;
                            FIFOTime += processAll.TLBtime + processAll.shortTime;
                        }
                    }
                    else if (flag1 == false)
                    {
                        Request.FIFO++;
                        dataGridView1.Rows[maxSize].Cells[i + 1].Value = "Yes";
                        resNum++;
                        FIFOLackNum++;
                        currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;
                        FIFOTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;

                    }
                    //增加前面作业的次数的属性
                    for (int j = 0; j <= i; j++)
                    {
                        jobs[j].times++;
                    }
                    //label24.Text = LRUTime.ToString();
                    if (label22.InvokeRequired)
                    {
                        label22.BeginInvoke(new Action(() =>
                        {
                            label22.Text = FIFOTime.ToString();
                            label22.Text = FIFOTime.ToString();
                            label22.Refresh();
                        }));
                    }
                    else
                    {
                        label22.Text = FIFOTime.ToString();
                        label22.Text = FIFOTime.ToString();
                        label22.Refresh();
                    }
                    double bchu = FIFOLackNum;
                    double chu = jobs.Count;
                    FIFOLackPrecent = Math.Round(bchu / chu, 2);
                    if (label4.InvokeRequired)
                    {
                        label4.BeginInvoke(new Action(() =>
                        {
                            label4.Text = FIFOLackPrecent.ToString();
                            label4.Refresh();
                        }));
                    }
                    else
                    {
                        label4.Text = FIFOLackPrecent.ToString();
                        label4.Refresh();
                    }
                    if (dataGridView1.InvokeRequired)
                    {
                        dataGridView1.BeginInvoke(new Action(() =>
                        {
                            dataGridView1.Refresh();
                        }));
                    }
                    else
                    {
                        dataGridView1.Refresh();
                    }
                    Request.FIFOArrayList.Insert(i, currentTime);
                    Request.FIFO = FIFOLackPrecent;

                }
                resMessage += "缺页次数为：" + resNum + "；";
                resMessage += "消耗的时间为：" + FIFOTime + "；";
                resMessage += "缺页率为：" + FIFOLackPrecent;
            }

        }



        //LRU算法实现
        public void LRUMethod()
        {
            List<Job> jobs = new List<Job>();
            string[] strs = processAll.jobs.Split(",");
            string[] names = new string[strs.Length];
            Queue<Job> myTLB = new Queue<Job>();
            int TLBnum = 0;
            for (int i = 0; i < strs.Length; i++)
            {
                string tmp = changeToBinary(strs[i]);
                Job job = new Job();
                job.address = tmp;
                string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                job.name = num;
                names[i] = num.ToString();
                jobs.Add(job);
            }
            List<Job> myList = new List<Job>();
            int maxSize = processAll.statusNum;
            int flag = 0;
            string res = "";
            resMessage2 = "";
            resMessage2 += "使用方法为LRU ";
            resMessage2 += "作业为：";
            int resNum = 0;
            for (int i = 0; i < jobs.Count; i++)
            {
                resMessage2 += jobs[i].name + " ";
                Sleep(1000);
                bool flag1 = false;
                if (flag < maxSize)
                {
                    //检查是否有重复的
                    foreach (Job tmp in myList)
                    {
                        if (tmp.name == jobs[i].name)
                        {
                            flag1 = true;
                            break;
                        }
                    }
                    if (flag1 == false)
                    {
                        myList.Add(jobs[i]);
                        jobs[i].times++;//它使用的次数要加一

                        flag++;
                    }
                    for (int j = 0; j < flag; j++)
                    {
                        if (i > 0 && dataGridView2.Rows[j].Cells[i].Value != null)
                        {

                            dataGridView2.Rows[j].Cells[i + 1].Value = dataGridView2.Rows[j].Cells[i].Value;
                        }
                        else
                        {
                            dataGridView2.Rows[j].Cells[i + 1].Value = jobs[i].name.ToString();
                        }

                    }

                }
                else if (flag == maxSize)
                {
                    //判断缺页情况
                    foreach (Job tmp in myList)
                    {
                        if (tmp.name == jobs[i].name)
                        {
                            flag1 = true;
                            break;
                        }
                    }

                    List<Job> list = new List<Job>();
                    for (int j = 0; j < flag; j++)
                    {
                        bool judge = false;
                        foreach (Job tmp in myList)
                        {
                            if (tmp.name == jobs[i].name)
                            {
                                judge = true;
                            }
                            list.Add(tmp);
                        }
                        if (judge == false)
                        {
                            for (int m = 0; m < flag; m++)
                            {
                                myList[m].record = i;
                            }
                            for (int m = 0; m < flag; m++)
                            {
                                for (int t = 0; t <= i - 1; t++)
                                {
                                    if (myList[m].name == jobs[t].name)
                                    {
                                        myList[m].record = t;
                                    }
                                }
                            }
                            int minRecord = i;
                            for (int t = 0; t < flag; t++)
                            {
                                if (myList[t].record < minRecord)
                                {
                                    minRecord = myList[t].record;
                                }
                            }
                            int sum = 0;
                            for (int t = 0; t < flag; t++)
                            {
                                if (sum == 1) { break; }
                                if (myList[t].record == i || myList[t].record == minRecord)
                                {
                                    myList[t] = jobs[i];
                                    sum++;
                                }
                            }
                            for (int t = 0; t < flag; t++)
                            {
                                list.Add(myList[t]);
                            }
                        }
                        //bool judge=false;
                        //foreach(Job tmp in myList)
                        //{
                        //    if (tmp.name == jobs[i].name)
                        //    {
                        //        judge= true;
                        //    }
                        //    list.Add(tmp);
                        //}
                        //if (judge == false)
                        //{
                        //    //获取到最大的次数和最大次数的名字
                        //    int timeMax = 0;
                        //    int nameMax = 0;
                        //    foreach(Job tmp in myList)
                        //    {
                        //        if (tmp.times > timeMax)
                        //        {
                        //            timeMax = tmp.times;
                        //            nameMax = tmp.name;
                        //        }
                        //    }
                        //    for(int t = 0; t < flag; t++)
                        //    {
                        //        if (myList[t].name == nameMax)
                        //        {
                        //            myList[t] = jobs[i];
                        //        }
                        //    }
                        //    for (int t = 0; t < flag; t++)
                        //    {
                        //        list.Add(myList[t]);
                        //    }
                        //}
                        dataGridView2.Rows[j].Cells[i + 1].Value = myList[j].name.ToString();
                    }
                }
                bool TLBstatue = false;
                if (TLBnum < processAll.lengthTLB)
                {
                    foreach (Job tmp in myTLB)
                    {
                        if (jobs[i] == tmp)
                        {
                            TLBstatue = true;
                        }
                    }
                    if (TLBstatue == false)
                    {
                        myTLB.Enqueue(jobs[i]);
                        TLBnum++;
                    }
                }
                else if (TLBnum == processAll.lengthTLB)
                {
                    foreach (Job tmp in myTLB)
                    {
                        if (jobs[i] == tmp)
                        {
                            TLBstatue = true;
                        }
                    }
                    if (TLBstatue == false && myTLB != null)
                    {
                        myTLB.Dequeue();
                        myTLB.Enqueue(jobs[i]);
                    }
                }
                if (flag1 == true)
                {

                    dataGridView2.Rows[maxSize].Cells[i + 1].Value = "No";
                    //不缺页但是快表里面没有
                    if (TLBstatue == false)
                    {
                        currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                        LRUTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                    }
                    else if (TLBstatue == true)
                    {
                        //不缺页但是快表里面有
                        currentTime = processAll.TLBtime + processAll.shortTime;
                        LRUTime += processAll.TLBtime + processAll.shortTime;
                    }
                }
                else if (flag1 == false)
                {
                    Request.LRU++;
                    dataGridView2.Rows[maxSize].Cells[i + 1].Value = "Yes";
                    resNum++;
                    LRULackNum++;
                    currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;
                    LRUTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;

                }
                //增加前面作业的次数的属性
                for (int j = 0; j <= i; j++)
                {
                    jobs[j].times++;
                }
                //label24.Text = LRUTime.ToString();
                if (label24.InvokeRequired)
                {
                    label24.BeginInvoke(new Action(() =>
                    {
                        label24.Text = LRUTime.ToString();
                        label24.Text = LRUTime.ToString();
                        label24.Refresh();
                    }));
                }
                else
                {
                    label24.Text = LRUTime.ToString();
                    label24.Text = LRUTime.ToString();
                    label24.Refresh();
                }
                double bchu = LRULackNum;
                double chu = jobs.Count;
                LRULackPrecent = Math.Round(bchu / chu, 2);
                if (label5.InvokeRequired)
                {
                    label5.BeginInvoke(new Action(() =>
                    {
                        label5.Text = LRULackPrecent.ToString();
                        label5.Refresh();
                    }));
                }
                else
                {
                    label5.Text = LRULackPrecent.ToString();
                    label5.Refresh();
                }
                if (dataGridView2.InvokeRequired)
                {
                    dataGridView2.BeginInvoke(new Action(() =>
                    {
                        dataGridView2.Refresh();
                    }));
                }
                else
                {
                    dataGridView2.Refresh();
                }
                Request.LRUArrayList.Insert(i, currentTime);
                Request.LRU = LRULackPrecent;

            }
            resMessage2 += "缺页次数为：" + resNum + "；";
            resMessage2 += "消耗的时间为：" + LRUTime + "；";
            resMessage2 += "缺页率为：" + LRULackPrecent;
        }



        //OPT页面置换算法实现
        public void OPTMethod()
        {
            List<Job> jobs = new List<Job>();
            string[] strs = processAll.jobs.Split(",");
            string[] names = new string[strs.Length];
            Queue<Job> myTLB = new Queue<Job>();
            int TLBnum = 0;
            for (int i = 0; i < strs.Length; i++)
            {
                string tmp = changeToBinary(strs[i]);
                Job job = new Job();
                job.address = tmp;
                string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                job.name = num;
                names[i] = num.ToString();
                jobs.Add(job);
            }
            List<Job> myList = new List<Job>();
            int maxSize = processAll.statusNum;
            int flag = 0;
            string res = "";
            resMessage3 = "";
            resMessage3 += "使用方法为OPT ";
            resMessage3 += "作业为：";
            int resNum = 0;
            OPTLackPrecent = 0;
            for (int i = 0; i < jobs.Count; i++)
            {
                resMessage3 += jobs[i].name + " ";
                Sleep(1000);
                bool flag1 = false;
                //当达不到限制数目时
                if (flag < maxSize)
                {
                    //检查是否有重复的
                    foreach (Job tmp in myList)
                    {
                        if (tmp.name == jobs[i].name)
                        {
                            flag1 = true;
                            break;
                        }
                    }
                    if (flag1 == false)
                    {
                        myList.Add(jobs[i]);
                        flag++;
                    }
                    for (int j = 0; j < flag; j++)
                    {
                        if (i > 0 && dataGridView3.Rows[j].Cells[i].Value != null)
                        {

                            dataGridView3.Rows[j].Cells[i + 1].Value = dataGridView3.Rows[j].Cells[i].Value;
                        }
                        else
                        {
                            dataGridView3.Rows[j].Cells[i + 1].Value = jobs[i].name.ToString();
                        }

                    }

                }
                else if (flag == maxSize)
                {
                    //判断缺页情况
                    foreach (Job tmp in myList)
                    {
                        if (tmp.name == jobs[i].name)
                        {
                            flag1 = true;
                            break;
                        }
                    }

                    List<Job> list = new List<Job>();
                    for (int j = 0; j < flag; j++)
                    {
                        bool judge = false;
                        foreach (Job tmp in myList)
                        {
                            if (tmp.name == jobs[i].name)
                            {
                                judge = true;
                            }
                            list.Add(tmp);
                        }
                        if (judge == false)
                        {
                            for (int m = 0; m < flag; m++)
                            {
                                myList[m].record = 0;
                            }
                            for (int m = 0; m < flag; m++)
                            {
                                for (int t = jobs.Count - 1; t >= i; t--)
                                {
                                    if (myList[m].name == jobs[t].name)
                                    {
                                        myList[m].record = t;
                                    }
                                }
                            }
                            int maxRecord = 0;
                            for (int t = 0; t < flag; t++)
                            {
                                if (myList[t].record > maxRecord)
                                {
                                    maxRecord = myList[t].record;
                                }
                            }
                            int sum = 0;
                            for (int t = 0; t < flag; t++)
                            {
                                if (sum == 1) { break; }
                                if (myList[t].record == 0 || myList[t].record == maxRecord)
                                {
                                    myList[t] = jobs[i];
                                    sum++;
                                }
                            }
                            for (int t = 0; t < flag; t++)
                            {
                                list.Add(myList[t]);
                            }
                        }
                        dataGridView3.Rows[j].Cells[i + 1].Value = myList[j].name.ToString();
                    }
                }
                bool TLBstatue = false;
                if (TLBnum < processAll.lengthTLB)
                {
                    foreach (Job tmp in myTLB)
                    {
                        if (jobs[i] == tmp)
                        {
                            TLBstatue = true;
                        }
                    }
                    if (TLBstatue == false)
                    {
                        myTLB.Enqueue(jobs[i]);
                        TLBnum++;
                    }
                }
                else if (TLBnum == processAll.lengthTLB)
                {
                    foreach (Job tmp in myTLB)
                    {
                        if (jobs[i] == tmp)
                        {
                            TLBstatue = true;
                        }
                    }
                    if (TLBstatue == false)
                    {
                        myTLB.Dequeue();
                        myTLB.Enqueue(jobs[i]);
                    }
                }
                if (flag1 == true)
                {

                    dataGridView3.Rows[maxSize].Cells[i + 1].Value = "No";
                    //不缺页但是快表里面没有
                    if (TLBstatue == false)
                    {
                        currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                        OPTTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime;
                    }
                    else if (TLBstatue == true)
                    {
                        //不缺页但是快表里面有
                        currentTime = processAll.TLBtime + processAll.shortTime;
                        OPTTime += processAll.TLBtime + processAll.shortTime;
                    }
                }
                else if (flag1 == false)
                {
                    Request.OPT++;
                    dataGridView3.Rows[maxSize].Cells[i + 1].Value = "Yes";
                    resNum++;
                    OPTLackNum++;
                    currentTime = processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;
                    OPTTime += processAll.TLBtime + processAll.shortTime + processAll.shortTime + processAll.shortTime + processAll.stopTime;
                }
                //label27.Text = OPTTime.ToString();
                if (label27.InvokeRequired)
                {
                    label27.BeginInvoke(new Action(() =>
                    {
                        label27.Text = OPTTime.ToString();
                        label27.Text = OPTTime.ToString();
                        label27.Refresh();
                    }));
                }
                else
                {
                    label27.Text = OPTTime.ToString();
                    label27.Text = OPTTime.ToString();
                    label27.Refresh();
                }
                double bchu = OPTLackNum;
                double chu = jobs.Count;
                OPTLackPrecent = Math.Round(bchu / chu, 2);


                if (label6.InvokeRequired)
                {
                    label6.BeginInvoke(new Action(() =>
                    {
                        label6.Text = OPTLackPrecent.ToString();
                        label6.Refresh();
                    }));
                }
                else
                {
                    label6.Text = OPTLackPrecent.ToString();
                    label6.Refresh();
                }

                if (dataGridView3.InvokeRequired)
                {
                    dataGridView3.BeginInvoke(new Action(() =>
                    {
                        dataGridView3.Refresh();
                    }));
                }
                else
                {
                    dataGridView3.Refresh();
                }
                Request.OPTArrayList.Insert(i, currentTime);
                Request.OPT = OPTLackPrecent;
            }
            resMessage3 += "缺页次数为：" + resNum + "；";
            resMessage3 += "消耗的时间为：" + OPTTime + "；";
            resMessage3 += "缺页率为：" + OPTLackPrecent;
            //dataGridView3.Refresh();
        }

        //根据输入的地址为第一个dataGridView添加表头的列
        public void View1AddCol()
        {
            dataGridView1.Columns.Clear();
            string[] strs = processAll.jobs.Split(',');
            string[] names = new string[strs.Length];
            DataGridViewTextBoxColumn column0 = new DataGridViewTextBoxColumn();
            column0.Name = "作业的名字"; // 列的名称，用于数据绑定
            column0.HeaderText = "作业的名字"; // 列的表头数据
            dataGridView1.Columns.Add(column0);
            for (int i = 0; i < strs.Length; i++)
            {
                string tmp = changeToBinary(strs[i]);
                string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                names[i] = num.ToString();
                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
                column1.Name = names[i]; // 列的名称，用于数据绑定
                column1.HeaderText = names[i]; // 列的表头数据
                dataGridView1.Columns.Add(column1);
            }

        }
        //根据输入的物理块数为第一个dataGridView添加表头的行
        public void View1AddRow(int num)
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < num; i++)
            {
                string tmp = "物理块" + (i + 1);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = tmp;
                dataGridView1.Rows.Add(row);
            }
            DataGridViewRow rowLast = new DataGridViewRow();
            rowLast.CreateCells(dataGridView1);
            rowLast.Cells[0].Value = "是否缺页";
            dataGridView1.Rows.Add(rowLast);



            //dataGridView1.Rows[0].Cells[1].Value = "测试";
            //数据可以从第0行，第一列写入
        }



        //根据输入的地址为dataGridView2添加表头的列
        public void View2AddCol()
        {
            dataGridView2.Columns.Clear();
            string[] strs = processAll.jobs.Split(',');
            string[] names = new string[strs.Length];
            DataGridViewTextBoxColumn column0 = new DataGridViewTextBoxColumn();
            column0.Name = "作业的名字"; // 列的名称，用于数据绑定
            column0.HeaderText = "作业的名字"; // 列的表头数据
            dataGridView2.Columns.Add(column0);
            for (int i = 0; i < strs.Length; i++)
            {
                string tmp = changeToBinary(strs[i]);
                string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                names[i] = num.ToString();
                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
                column1.Name = names[i]; // 列的名称，用于数据绑定
                column1.HeaderText = names[i]; // 列的表头数据
                dataGridView2.Columns.Add(column1);
            }

        }
        //根据输入的物理块数为dataGridView2添加表头的行
        public void View2AddRow(int num)
        {
            dataGridView2.Rows.Clear();
            for (int i = 0; i < num; i++)
            {
                string tmp = "物理块" + (i + 1);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView2);
                row.Cells[0].Value = tmp;
                dataGridView2.Rows.Add(row);
            }
            DataGridViewRow rowLast = new DataGridViewRow();
            rowLast.CreateCells(dataGridView2);
            rowLast.Cells[0].Value = "是否缺页";
            dataGridView2.Rows.Add(rowLast);



            //dataGridView1.Rows[0].Cells[1].Value = "测试";
            //数据可以从第0行，第一列写入
        }



        //根据输入的地址为dataGridView3添加表头的列
        public void View3AddCol()
        {
            dataGridView3.Columns.Clear();
            string[] strs = processAll.jobs.Split(',');
            string[] names = new string[strs.Length];
            DataGridViewTextBoxColumn column0 = new DataGridViewTextBoxColumn();
            column0.Name = "作业的名字"; // 列的名称，用于数据绑定
            column0.HeaderText = "作业的名字"; // 列的表头数据
            dataGridView3.Columns.Add(column0);
            for (int i = 0; i < strs.Length; i++)
            {
                string tmp = changeToBinary(strs[i]);
                string tmp1 = tmp.Substring(0, 4);//截取前四位用来表示名字
                int num = Convert.ToInt32(tmp1, 2);//将前四位的二进制数转化为十进制数作为名字
                names[i] = num.ToString();
                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
                column1.Name = names[i]; // 列的名称，用于数据绑定
                column1.HeaderText = names[i]; // 列的表头数据
                dataGridView3.Columns.Add(column1);
            }

        }
        //根据输入的物理块数为dataGridView3添加表头的行
        public void View3AddRow(int num)
        {
            dataGridView3.Rows.Clear();
            for (int i = 0; i < num; i++)
            {
                string tmp = "物理块" + (i + 1);
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView3);
                row.Cells[0].Value = tmp;
                dataGridView3.Rows.Add(row);
            }
            DataGridViewRow rowLast = new DataGridViewRow();
            rowLast.CreateCells(dataGridView3);
            rowLast.Cells[0].Value = "是否缺页";
            dataGridView3.Rows.Add(rowLast);



            //dataGridView1.Rows[0].Cells[1].Value = "测试";
            //数据可以从第0行，第一列写入
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FIFOLackNum = 0;
            FIFOTime = 0;
            FIFOMethod();
            ResProcess resProcess = new ResProcess();
            resProcessService resProcessService = new resProcessService();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess.timeNow = time;
            resProcess.resId = processAll.id;
            resProcess.message = resMessage;
            resProcessService.InsertProcess(resProcess);
            List<ResProcess> resProcessList = resProcessService.FindAllProcesses();
            dataGridView5.DataSource = resProcessList;
            //resProcess.message = "......";
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            LRULackNum = 0;
            LRUTime = 0;
            LRUMethod();
            ResProcess resProcess = new ResProcess();
            resProcessService resProcessService = new resProcessService();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess.timeNow = time;
            resProcess.resId = processAll.id;
            resProcess.message = resMessage;
            resProcessService.InsertProcess(resProcess);
            List<ResProcess> resProcessList = resProcessService.FindAllProcesses();
            dataGridView5.DataSource = resProcessList;
            //Thread thread = new Thread(LRUMethod);
            //thread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OPTLackNum = 0;
            OPTTime = 0;
            OPTMethod();
            ResProcess resProcess = new ResProcess();
            resProcessService resProcessService = new resProcessService();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess.timeNow = time;
            resProcess.resId = processAll.id;
            resProcess.message = resMessage;
            resProcessService.InsertProcess(resProcess);
            List<ResProcess> resProcessList = resProcessService.FindAllProcesses();
            dataGridView5.DataSource = resProcessList;
            //Thread thread = new Thread(OPTMethod);
            //thread.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FIFOLackNum = 0;
            LRULackNum = 0;
            OPTLackNum = 0;
            FIFOTime = 0;
            LRUTime = 0;
            OPTTime = 0;
            Thread thread1 = new Thread(FIFOMethod);
            Thread thread2 = new Thread(LRUMethod);
            Thread thread3 = new Thread(OPTMethod);

            thread1.Start();
            thread2.Start();
            thread3.Start();
            ResProcess resProcess1 = new ResProcess();
            resProcessService resProcessService = new resProcessService();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess1.timeNow = "FIFO " + time;
            resProcess1.resId = processAll.id;
            resProcess1.message = resMessage;
            resProcessService.InsertProcess(resProcess1);

            ResProcess resProcess2 = new ResProcess();
            resProcessService resProcessService2 = new resProcessService();
            string time2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess2.timeNow = "LRU " + time2;
            resProcess2.resId = processAll.id;
            resProcess2.message = resMessage2;
            resProcessService.InsertProcess(resProcess2);

            ResProcess resProcess3 = new ResProcess();
            resProcessService resProcessService3 = new resProcessService();
            string time3 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            resProcess3.timeNow = "OPT " + time3;
            resProcess3.resId = processAll.id;
            resProcess3.message = resMessage3;
            resProcessService.InsertProcess(resProcess3);
            List<ResProcess> resProcessList3 = resProcessService3.FindAllProcesses();
            dataGridView5.DataSource = resProcessList3;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            AddProcess addProcess = new AddProcess();
            addProcess.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            processList = processService.FindAllProcesses();
            dataGridView4.DataSource = processList;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label22.Text = "00";
            label24.Text = "00";
            label27.Text = "00";
            label4.Text = "";
            label6.Text = "";
            label5.Text = "";
            FIFOLackNum = 0;
            LRULackNum = 0;
            OPTLackNum = 0;
            FIFOTime = 0;
            LRUTime = 0;
            OPTTime = 0;
            DataGridViewRow dataGridViewRow = dataGridView4.CurrentRow;
            processAll = dataGridViewRow.DataBoundItem as Process;
            label33.Text = processAll.jobs;
            label34.Text = processAll.statusNum.ToString();
            label35.Text = processAll.lengthTLB.ToString();
            label36.Text = processAll.TLBtime.ToString();
            label37.Text = processAll.shortTime.ToString();
            label38.Text = processAll.stopTime.ToString();
            View1AddCol();
            View2AddCol();
            View3AddCol();
            View1AddRow(processAll.statusNum);
            View2AddRow(processAll.statusNum);
            View3AddRow(processAll.statusNum);

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Process deleteProcess = new Process();
            DataGridViewRow dataGridViewRow = dataGridView4.CurrentRow;
            deleteProcess = dataGridViewRow.DataBoundItem as Process;
            processService.DeleteProcess(deleteProcess.id);

            List<Process> processList = processService.FindAllProcesses();
            dataGridView4.DataSource = processList;

        }

        private void resBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            Chart chart = new Chart();
            chart.Show();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            ResProcess deleteResProcess = new ResProcess();
            DataGridViewRow dataGridViewRow = dataGridView5.CurrentRow;
            deleteResProcess = dataGridViewRow.DataBoundItem as ResProcess;
            resProcessService1.DeleteProcess(deleteResProcess.timeNow);

            List<ResProcess> processList = resProcessService1.FindAllProcesses();
            dataGridView5.DataSource = processList;
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            List<ResProcess> processList = resProcessService1.FindAllProcesses();
            dataGridView5.DataSource = processList;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {
            double[] fifoData = new double[Request.FIFOArrayList.Count];
            double[] fifoX = new double[Request.FIFOArrayList.Count];
            double[] lruData = new double[Request.LRUArrayList.Count];
            double[] lruX = new double[Request.LRUArrayList.Count];
            double[] optData = new double[Request.OPTArrayList.Count];
            double[] optX = new double[Request.OPTArrayList.Count];
            for (int i = 0; i < Request.FIFOArrayList.Count; i++)
            {
                fifoData[i] = (int)Request.FIFOArrayList[i];
                fifoX[i] = i;
            }
            for (int i = 0; i < Request.LRUArrayList.Count; i++)
            {
                lruData[i] = (int)Request.LRUArrayList[i];
                lruX[i] = i;
            }
            for (int i = 0; i < Request.OPTArrayList.Count; i++)
            {
                optData[i] = (int)Request.OPTArrayList[i];
                optX[i] = i;
            }
            formsPlot1.Plot.Add.Scatter(fifoX, fifoData);
            formsPlot1.Plot.Add.Scatter(lruX, lruData);
            formsPlot1.Plot.Add.Scatter(optX, optData);
            var plt = formsPlot1.Plot;
            formsPlot1.Refresh();
        }
    }
}