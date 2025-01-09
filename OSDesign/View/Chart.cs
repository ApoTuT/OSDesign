using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDesign.View
{
    public partial class Chart : Form
    {
        public string Title => "Multi-Axis";

        public string Description => "Display data which visually overlaps but is plotted on different axes";

        readonly ScottPlot.IYAxis YAxis1;

        readonly ScottPlot.IYAxis YAxis2;

        readonly ScottPlot.IYAxis YAxis3;

        readonly ScottPlot.IYAxis YAxis4;

        readonly ScottPlot.IYAxis YAxis5;

        readonly ScottPlot.IYAxis YAxis6;
        double[] fifoData = new double[Request.FIFOArrayList.Count];
        double[] lruData = new double[Request.LRUArrayList.Count];
        double[] optData = new double[Request.OPTArrayList.Count];
        int[] fifoX = new int[Request.FIFOArrayList.Count];
        int[] lruX = new int[Request.LRUArrayList.Count];
        int[] optX = new int[Request.OPTArrayList.Count];
        public Chart()
        {
            InitializeComponent();
            // Store the primary Y axis so we can refer to it later
            YAxis1 = formsPlot1.Plot.Axes.Left;

            // Create a second Y axis, add it to the plot, and save it for later
            YAxis2 = formsPlot1.Plot.Axes.AddLeftAxis();

            YAxis3 = formsPlot1.Plot.Axes.AddLeftAxis();
            YAxis1.MinimumSize = 0;
            YAxis2.MinimumSize = 0;
            YAxis3.MinimumSize = 0;
            formsPlot1_Load(null, null);


            // plot random data to start
            PlotRandomData();
        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {
            formsPlot1.Plot.Clear();
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
        }

        private void PlotRandomData()
        {
            formsPlot1.Plot.Clear();

            fifoData[0] = 510.2;
            lruData[0] = 314.5;
            double[] data1 = ScottPlot.RandomDataGenerator.Generate.RandomWalk(count: 51, mult: 1);
            var sig1 = formsPlot1.Plot.Add.Signal(fifoData);
            sig1.Axes.YAxis = YAxis1;

            YAxis1.Label.Text = "FIFO";
            YAxis1.Label.ForeColor = sig1.Color;

            double[] data2 = ScottPlot.RandomDataGenerator.Generate.RandomWalk(count: 51, mult: 1000);
            var sig2 = formsPlot1.Plot.Add.Signal(lruData);
            sig2.Axes.YAxis = YAxis2;
            YAxis2.Label.Text = "LRU";
            YAxis2.Label.ForeColor = sig2.Color;

            double[] data3 = ScottPlot.RandomDataGenerator.Generate.RandomWalk(count: 51, mult: 1000);
            var sig3 = formsPlot1.Plot.Add.Signal(optData);
            sig3.Axes.YAxis = YAxis3;
            YAxis3.Label.Text = "OPT";
            YAxis3.Label.ForeColor = sig3.Color;



            formsPlot1.Plot.Axes.AutoScale();
            formsPlot1.Plot.Axes.Zoom(.8, .1); // zoom out slightly
            formsPlot1.Refresh();
        }

        private void Chart_Load(object sender, EventArgs e)
        {

        }

        private void formsPlot1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
