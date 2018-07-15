using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDIOTService;
using YdService.Util;
using System.Configuration;

namespace ConsoleApplication1
{
    class Program
    {
       // private static DateTime startTime = new DateTime(2017, 7, 11, 0, 0, 0);
        //private static DateTime endtime = new DateTime(2017, 8, 2, 0, 0, 0);
        System.Timers.Timer timer = new System.Timers.Timer();
        private static bool finishedFlag = true;
        private static int tickTimes = 0;
        static void Main(string[] args)
        {

            // startTime = new PSDDBGenerater(startTime).run();
           //   Process p = new Process();
           //  Boolean b = p.startDayE();
            //  Boolean b = p.startYearE();
            //  Boolean b = p.startMonthE();
            // List<string> list =new SQLUtil().getAvailableDataBaseList();
            //DateTime start = new DateTime(2017, 8, 1);
            //  DateTime end = DateTime.Now;

            //PSMDBGenerater pp = new PSMDBGenerater("ydupdate", startTime, endtime);
            // pp.run();

            // PSYDBGenerater pp = new PSYDBGenerater("ydupdate", startTime, endtime);
            //  pp.run();

           // Console.WriteLine(CommonUtil. getUsageMatchStrFromConfig());

             int y = 0;
           

           

            new Program().init();

            Console.Read();
            

        }

        private void init()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"].Trim().ToString());
            timer.Enabled = true;
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            object o = new object();
            if (finishedFlag)
            {
                lock (o)
                {
                    finishedFlag = false;
                }
                Process p = new Process();
                if (p.start())
                {

                    lock (o)
                    {
                        finishedFlag = true;
                    }
                    LogUtil.logFinishTicked(++tickTimes);
                }
                else
                {
                    LogUtil.log2Enter("[attention! processing failed ] occur time : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else
            {
                LogUtil.log2Enter("attempt to tick when last haven't finished! occur time : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

        }



    }
}
