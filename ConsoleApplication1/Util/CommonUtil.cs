using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YdService.Model;
using System.Configuration;
using YdService.Db;

namespace YdService.Util
{
    class CommonUtil
    {
        private static Dictionary<string, string> dicMscAndFcid = new Dictionary<string, string>();

        public static Dictionary<string, string> generateMscAndFcidMap(SqlHelper sqlHelper)
        {
            if (dicMscAndFcid.Count > 0) {
                return dicMscAndFcid;
            }
            string sql = "SELECT DISTINCT Msc_ID,Facility_ID from Facility_Config; ";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            if (CommonUtil.firstTableHaveRow(dataSet))
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    string mscid = dataSet.Tables[0].Rows[i][0].ToString().Trim();
                    string fcid = dataSet.Tables[0].Rows[i][1].ToString().Trim();
                    if (!dicMscAndFcid.ContainsKey(mscid))
                    {
                        dicMscAndFcid.Add(mscid, fcid);
                    }
                }
            }
            return dicMscAndFcid;
        }

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

        public static DataTable createEmptyPollingStaDayTable()
        {
            DataTable table = createEmptyPollingStaTable(true, "Polling_Log_Sta_Day", 24);
            table.Columns.Add("month", typeof(int));
            table.Columns.Add("day", typeof(int));
            return table;
        }

        public static DataTable createEmptyPollingStaMonthTable()
        {
            DataTable table = createEmptyPollingStaTable(false, "Polling_Log_Sta_Month", 32);
            table.Columns.Add("month", typeof(int));
            return table;
        }

        public static DataTable createEmptyPollingStaYearTable()
        {
            return createEmptyPollingStaTable(false, "Polling_Log_Sta_Year", 13);
        }

        public static DataTable createEmptyPollingStaTable(bool from0, string tableName, int width)
        {
            char columnLabel = 'H';

            switch (width)
            {
                case 32:
                    columnLabel = 'D';
                    break;
                case 13:
                    columnLabel = 'M';
                    break;
                default:
                    break;
            }

            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("year", typeof(int));
            dt.Columns.Add("mscid", typeof(int));
            dt.Columns.Add("occurtime", typeof(DateTime));
            dt.Columns.Add("fcid", typeof(int));
            dt.Columns.Add("haveData", typeof(byte));
            int startindex = 0;
            if (!from0) { startindex = 1; }
            for (int i = startindex; i < width; i++)
            {

                dt.Columns.Add(columnLabel + "" + i, typeof(float));
                dt.Columns.Add(columnLabel + "n" + i, typeof(float));
            }
            return dt;
        }


        public static void addNewRowForDataTable(DataTable dt, PollingStaYear psy)
        {
            char colLabel = 'M';
            DataRow newRow = dt.NewRow();
            newRow["id"] = psy.id;
            newRow["year"] = psy.year;
            newRow["mscid"] = psy.mscid;
            newRow["fcid"] = 0;
            newRow["haveData"] = 0;
            if (psy is PollingStaDay)
            {
                psy = (PollingStaDay)psy;
                colLabel = 'H';
                newRow["month"] = ((PollingStaDay)psy).month;
                newRow["day"] = ((PollingStaDay)psy).day;
                newRow["occurtime"] = psy.year + "-" + ((PollingStaDay)psy).month + "-" + ((PollingStaDay)psy).day;
                for (int i =  0; i <= 23; i++)
                {
                    newRow[colLabel + "" + i] = psy.totalColumn[i];
                    newRow[colLabel + "n" + i] = psy.netColumn[i];
                }
            
            
            }
            else if (psy is PollingStaMonth)
            {
                psy = (PollingStaMonth)psy;
                colLabel = 'D';
                newRow["month"] = ((PollingStaMonth)psy).month;
                newRow["occurtime"] = psy.year + "-" + ((PollingStaMonth)psy).month + "-01";

                for (int i = 1; i <= 31; i++)
                {
                    newRow[colLabel + "" + i] = psy.totalColumn[i-1];
                    newRow[colLabel + "n" + i] = psy.netColumn[i-1];
                }
            
            }
            else
            {
                newRow["occurtime"] = psy.year + "-01-01";
                for (int i = 1; i <= 12; i++)
                {
                    newRow[colLabel + "" + i] = psy.totalColumn[i-1];
                    newRow[colLabel + "n" + i] = psy.netColumn[i-1];
                }
            }

          
            dt.Rows.Add(newRow);
        }

        public static int getLastDayOfMonth(DateTime dateTime)
        {
            return DateTime.Parse(dateTime.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).Day;
        }

        public static void deleteUnusefulData(DataTable insertDT, int width)
        {
            int interval = 0;

            char colLabel = 'M';
            if (width == 24) {
                colLabel = 'H';
            }
            else if (width == 31)
            {
                colLabel = 'D';
            }

            string select = colLabel+""+1+" <= "+interval;
            for (int i = 2; i < width; i++)
            {
                select += " and " + colLabel + i + " <= " + interval;
            }
            DataRow[] drs = insertDT.Select(select);
            foreach (DataRow item in drs)
            {
                insertDT.Rows.Remove(item);
            }
        }

        /// <summary>
         /// 修改AppSettings中配置
         /// </summary>
         /// <param name="key">key值</param>
         /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
         {
             try
             {
                 Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                 if (config.AppSettings.Settings[key] != null)
                     config.AppSettings.Settings[key].Value = value;
                 else
                     config.AppSettings.Settings.Add(key, value);
                 config.Save(ConfigurationSaveMode.Modified);
                 ConfigurationManager.RefreshSection("appSettings");
                 return true;
             }
             catch
             {
                 return false;
             }
         }

        public static String getUsageMatchStrFromConfig() {
            return "'" + ConfigurationManager.AppSettings["usageMatchStr"].Trim().ToString().Replace(",", "','") + "'";
        }
    }
}
