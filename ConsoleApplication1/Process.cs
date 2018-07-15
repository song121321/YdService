using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Util;
using YDIOTService;
using ConsoleApplication1.Generater;
using System.Configuration;

namespace ConsoleApplication1
{
    public class Process
    {
        List<string> dbList;
        private static DateTime configStartTime = Convert.ToDateTime(ConfigurationManager.AppSettings["startTime"].Trim().ToString());
        private static readonly string lastExcuteTimeStr = ConfigurationManager.AppSettings["lastExcuteTime"].Trim().ToString();
        private static DateTime startTime = string.IsNullOrWhiteSpace(lastExcuteTimeStr) ? configStartTime : Convert.ToDateTime(lastExcuteTimeStr);
        private static DateTime startTimeMonth = new DateTime(startTime.Year, startTime.Month, 1);
        private static DateTime startTimeYear = startTime;
        private static HashSet<string> daysHaveCounted = new HashSet<string>();
        private static HashSet<string> monthsHaveCounted = new HashSet<string>();

        public Process()
        {
            dbList = new SQLUtil().getAvailableDataBaseList();
        }

        public bool start()
        {

            DateTime proTime = DateTime.Now;
            if (proTime.Year == startTime.Year && proTime.Month == startTime.Month)
            {
                //执行到当月了，不能每月1号统计month了，需要每天都统计。
                startDay();
                if (!daysHaveCounted.Contains(startTime.AddDays(-1).ToShortDateString()))
                {
                    startMonth();
                    daysHaveCounted.Add(startTime.AddDays(-1).ToShortDateString());
                }

                if (startTime.Day == 2)//实际上是1号因为startDay已经执行完了，这里变成了2号。
                {
                    //今天是1号，那么统计下上个月的year值。
                    if (!monthsHaveCounted.Contains(startTime.Year + "_" + startTime.Month))
                    {
                        startYear();
                        monthsHaveCounted.Add(startTime.Year + "_" + startTime.Month);
                    }

                }

                return true;

            }

            if (startTime.Day == 1 && startTime.Month == 1)
            {
                startMonth();
                startYear();
                startDay();
                return true;
            }

            if (startTime.Day == 1)
            {

                startMonth();
                startDay();
                return true;
            }
            startDay();
            return true;

        }

        public bool startDay()
        {
            DateTime processTime = DateTime.Now;
            try
            {
                if (startTime < processTime)
                {
                    DateTime startTimeCopy = startTime;
                    for (int i = 0; i < dbList.Count; i++)
                    {
                        startTimeCopy = new PSDDBGenerater(dbList[i], startTime, processTime).run();
                        new PSUpdateFcidGenerater(dbList[i], "Day").run();
                        LogUtil.logFinishDataBase(dbList[i]);
                    }
                    startTime = startTimeCopy;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool startMonth()
        {
            DateTime processTime = DateTime.Now;

            try
            {
                LogUtil.starLog(startTimeMonth.ToString());
                if (startTimeMonth < processTime)
                {
                    DateTime startTimeCopy = startTimeMonth;
                    for (int i = 0; i < dbList.Count; i++)
                    {
                        startTimeCopy = new PSMDBGenerater(dbList[i], startTimeMonth, processTime).run();
                        new PSUpdateFcidGenerater(dbList[i], "Month").run();
                        LogUtil.logFinishDataBase(dbList[i]);
                    }
                    startTimeMonth = startTimeCopy;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool startYear()
        {
            DateTime processTime = DateTime.Now;
            if (startTimeYear < processTime)
            {
                DateTime startTimeCopy = startTimeYear;
                for (int i = 0; i < dbList.Count; i++)
                {
                    startTimeCopy = new PSYDBGenerater(dbList[i], startTimeYear, processTime).run();
                    new PSUpdateFcidGenerater(dbList[i],"Year").run();
                    LogUtil.logFinishDataBase(dbList[i]);
                }
                startTimeYear = startTimeCopy;
            }
            return true;
        }

    }
}
