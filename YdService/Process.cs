using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Util;
using YDIOTService;

namespace YdService
{
    public class Process
    {
        List<string> dbList;
        private DateTime startTime;
        public Process( DateTime startTime)
        {
            dbList = new SQLUtil().getAvailableDataBaseList();
            this.startTime = startTime;
        }

        public bool start()
        {
            DateTime processTime = DateTime.Now;
            try
            {
                for (int i = 0; i < dbList.Count; i++)
                {
                    LogUtil.logDbStart(dbList[i]);
                    LogUtil.logStarWith2EmptyLine("start processing data base: " + dbList[i]);
                    DateTime startTimeCopy = startTime;
                    while (startTimeCopy < processTime)
                    {
                        startTimeCopy = new PSDDBGenerater(dbList[i], startTimeCopy, DateTime.Now).run();
                    }
                    LogUtil.logDbFinish(dbList[i]);
                    LogUtil.logStarWith2EmptyLine("start processing data base: " + dbList[i]);
                }

                return true;
            }
            catch (Exception e)
            {


                return false;
            }

        }

    }
}
