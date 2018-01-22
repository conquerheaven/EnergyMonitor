using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Interpolation.Model.Implement;
using Interpolation.Model.Interface;
using Interpolation.Tools;
using Interpolation.RedisTool;
using System.Threading;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using Interpolation.Model.Entities;
using Interpolation.Controller.Interface;
using Interpolation.Controller;

namespace Interpolation
{
    public partial class Form1 : Form
    {
        private Time timeTool;
        private IScanController scanController;

        public Form1()
        {
            InitializeComponent();
            //设置检测定时器时间间隔为1分钟
            this.scanner.Interval = 60000;

            timeTool = new Time();
            scanController = new ScanController();
        }

        private void open_Click(object sender, EventArgs e)
        {
            open.Enabled = false;
            close.Enabled = true;
            hour.Enabled = false;
            minute.Enabled = false;
            scanner.Enabled = true;
            IRedisClient redis = RedisManager.redis;
            /*try
            {
                for(int i = 0; i < 4000; i++)
                {
                    redis.Set<int>(i.ToString(), i);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */
            
            //检测定时器启动，马上对前三周数据进行检测
            DateTime startTime = timeTool.YYMMDD_hh0000(DateTime.Now).AddDays(-21);
            //终点为定时时间点前1天
            DateTime endTime = new DateTime(DateTime.Now.Year , DateTime.Now.Month , DateTime.Now.Day , Convert.ToInt32(hour.Text) , Convert.ToInt32(minute.Text) , 0);
            endTime = endTime.AddDays(-1);

            TimeGap timeGap = new TimeGap(startTime, endTime);

            scanController.setTimeGap(timeGap);
            Thread scanThread = new Thread(new ThreadStart(scanController.scanTask));

            scanThread.IsBackground = true;
            scanThread.Start();
        }

        private void close_Click(object sender, EventArgs e)
        {
            open.Enabled = true;
            close.Enabled = false;
            hour.Enabled = true;
            minute.Enabled = true;
        }

        private void scanner_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (hour.Text == now.Hour.ToString() && minute.Text == now.Minute.ToString())
            {
                //将当前时间往前推24小时，作为起始时间点
                DateTime startTime = timeTool.YYMMDD_hh0000(DateTime.Now).AddHours(-24);
                //终点为当前时间小时减1秒
                DateTime endTime = timeTool.YYMMDD_hh0000(DateTime.Now).AddSeconds(-1);

                TimeGap timeGap = new TimeGap(startTime, endTime);

                scanController.setTimeGap(timeGap);
                Thread scanThread = new Thread(new ThreadStart(scanController.scanTask));

                scanThread.IsBackground = true;
                scanThread.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'energyMonitorDataSet.InterpolationState' table. You can move, or remove it, as needed.
            this.interpolationStateTableAdapter.Fill(this.energyMonitorDataSet.InterpolationState);

        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.interpolationStateTableAdapter.FillBy(this.energyMonitorDataSet.InterpolationState);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void interpolateOpen_Click(object sender, EventArgs e)
        {

        }
    }
}
