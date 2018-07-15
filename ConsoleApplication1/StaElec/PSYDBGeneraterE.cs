using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDIOTService.Generater;

namespace YDIOTService.StaElec
{
    class PSYDBGeneraterE: PSYDBGenerater
    {
        public PSYDBGeneraterE(string dbName, DateTime startTime, DateTime proceeTime)
            : base(dbName, startTime, proceeTime)
        {
            preTable = "Polling_Log_Sta_Month_Electricity";
            updateTable = "Polling_Log_Sta_Year_Electricity";
        }
    }
}
