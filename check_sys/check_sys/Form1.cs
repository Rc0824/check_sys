using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Design;

namespace check_sys
{
    public partial class Form1 : Form
    {

        static PerformanceCounter cpu = new PerformanceCounter(
            "Processor", "% Processor Time", "_Total");
        static PerformanceCounter memory = new PerformanceCounter(
            "Memory", "% Committed Bytes in Use");
        Chart chart;
        Series cpuSeries;
        Series memorySeries;
        Thread updateThread;
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "選擇保存位置",
            Filter = "文本文件|*.txt",
            DefaultExt = "txt",
            AddExtension = true
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void CreateChart(int time)
        {


            chart = new Chart
            {
                Dock = DockStyle.Fill
            };

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            cpuSeries = new Series("CPU")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue
            };

            memorySeries = new Series("Memory")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red
            };

            chart.Series.Add(cpuSeries);
            chart.Series.Add(memorySeries);


            panel1.Controls.Add(chart);

            updateThread = new Thread(() =>
            {
                while (true)
                {
                    double cpuValue = cpu.NextValue();
                    double memoryValue = memory.NextValue();
                    string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    chart.BeginInvoke(new Action(() =>
                    {
                        chart.ChartAreas[0].AxisY.Minimum = 0;
                        chart.ChartAreas[0].AxisY.Maximum = 100;
                        cpuSeries.Points.AddY(cpuValue);
                        memorySeries.Points.AddY(memoryValue);
                    }));

                    
                     string fileName = saveFileDialog.FileName;

                     using (StreamWriter writer = new StreamWriter(fileName, true))
                     {
                        string formattedLine = $"Time: {currentTime,-20} CPU: {cpuValue.ToString("0.00"),-10} Memory: {memoryValue.ToString("0.00"),-10}";
                        writer.WriteLine(formattedLine);
                    }      
                    Thread.Sleep(time * 1000);
                }
            });

            updateThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            int time = 0;
            if (int.TryParse(textBox1.Text, out time))
            {
                CreateChart(time);
                textBox1.Text = "";
            }
            else
            {
                MessageBox.Show("請輸入有效的數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog()== DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                MessageBox.Show("已保存到指定路徑！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


    }
}