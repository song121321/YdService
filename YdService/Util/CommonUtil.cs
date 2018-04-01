using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YdService.Model;

namespace YdService.Util
{
    class CommonUtil
    {
        public static bool firstTableHaveRow(DataSet ds)
        {

            if (null == ds)
            {
                return false;
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;

        }

        public static List<DateTime> generateDateUntillNowList(DateTime startTime)
        {
            List<DateTime> dateList = new List<DateTime>();
            while (startTime < DateTime.Now)
            {
                dateList.Add(startTime);
                startTime = startTime.AddDays(1);

            }
            return dateList;
        }

        public static DataTable createEmptyPollingStaDayDataTable()
        {
            DataTable dt = new DataTable("Polling_Log_Sta_Day");
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("year", typeof(int));
            dt.Columns.Add("month", typeof(int));
            dt.Columns.Add("day", typeof(int));
            dt.Columns.Add("mscid", typeof(int));
            dt.Columns.Add("occurtime", typeof(DateTime));
            for (int i = 0; i < 24; i++)
            {
                dt.Columns.Add("t" + (i + 1), typeof(float));
                dt.Columns.Add("n" + (i + 1), typeof(float));
            }
            return dt;
        }

        public static void addNewRowForDataTable(DataTable dt,PollingStaDay psd) {
            DataRow newRow = dt.NewRow();
            newRow["id"] = psd.id;
            newRow["year"] = psd.year;
            newRow["month"] = psd.month;
            newRow["day"] = psd.day;
            newRow["mscid"] = psd.mscid;
            newRow["occurtime"] = psd.year + "-" + psd.month + "-" + psd.day;
            for (int i = 0; i < 24; i++)
            {
                newRow["t" + (i + 1)] = psd.totalColumn[i];
                newRow["n" + (i + 1)] = psd.netColumn[i];
            }
            dt.Rows.Add(newRow);
        }




    }
}
