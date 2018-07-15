using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDIOTService.StaElec
{
    public class PSDDBGeneraterE : PSDDBGenerater
    {
        public PSDDBGeneraterE(string dbName, DateTime startTime, DateTime proceeTime)
            : base(dbName, startTime, proceeTime)
        {
            updateTable = "Polling_Log_Sta_Day_Electricity";
        }
    }
}
