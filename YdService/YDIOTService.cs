using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using YdService.Util;
using YDIOTService;
using System.Threading;
using System.Configuration;

namespace YdService
{
    public partial class YDIOTService : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        private static DateTime startTime = new DateTime(2016,7,11,0,0,0);

        private static bool finishedFlag = true;
        public YDIOTService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            startTime = Convert.ToDateTime(ConfigurationManager.AppSettings["startTime"].Trim().ToString());
            Thread.Sleep(5000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 30000;
            timer.Enabled = true;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (finishedFlag)
            {
                finishedFlag = false;
                Process p = new Process(startTime);
                if (p.start())
                {
                    finishedFlag = true;
                    startTime = DateTime.Now;
                }
                else
                {
                    LogUtil.log2Enter("[attention! processing failed ] occur time : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else {
                LogUtil.log2Enter("attempt to tick when last haven't finished! occur time : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

        }

        protected override void OnStop()
        {
        }
    }
}
