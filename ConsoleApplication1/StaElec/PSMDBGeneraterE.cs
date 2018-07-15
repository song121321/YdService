using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDIOTService.Generater;

namespace YDIOTService.StaElec
{
    class PSMDBGeneraterE: PSMDBGenerater
    {
        public PSMDBGeneraterE(string dbName, DateTime startTime, DateTime proceeTime)
            : base(dbName, startTime, proceeTime)
        {
            preTable = "Polling_Log_Sta_Day_Electricity";
            updateTable = "Polling_Log_Sta_Month_Electricity";
        }
    }
}
