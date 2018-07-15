using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Model;
using System.Data;
using YdService.Db;
using YdService.Util;
using System.Configuration;

namespace YDIOTService
{
    public class PSDDBGenerater
    {

        private DateTime startTime;//start time of this period
        private DateTime endTime;//end time of this period ,caculated
        private DateTime proceeTime;//the actual time of the program prcessing , same to datetime.now
        private string dbName;
        private SqlHelper sqlHelper;
        private string colLabel = "H";
        private int dayLength = 1;
       
        private readonly static string TIMEFORMAT = "yyyy-MM-dd HH:mm:ss";

       // private static Dictionary<string, string> dicMscAndFcid = new Dictionary<string, string>();


        public PSDDBGenerater()
        {
        }
        public PSDDBGenerater(string dbName, DateTime startTime, DateTime proceeTime)
        {
            this.dbName = dbName;
            this.proceeTime = proceeTime;
            sqlHelper = new SqlHelper(dbName);
            this.startTime = startTime;
            if ((proceeTime - startTime).Days > dayLength)
            {
                endTime = startTime.AddDays(dayLength);
            }
            else
            {
                endTime = proceeTime;// new DateTime(proceeTime.Year, proceeTime.Month, proceeTime.Day);
            }
        }

        public DateTime run()
        {
            if (endTime <= startTime)
            {
                LogUtil.log("endtime is greater or equal than starttime ,process will not go on,startTime:" + startTime.ToString(TIMEFORMAT) + ",endtime:" + endTime.ToString(TIMEFORMAT));
                return endTime;
            }
            //startTime = new DateTime(2017, 12, 30, 0, 0, 0);
            //endTime = new DateTime(2017, 12, 30, 14, 33, 0);
            // LogUtil.log("start processing data from " + startTime.ToString(TIMEFORMAT) + " to " + endTime.ToString(TIMEFORMAT) + "");
            preDealSourceTable();
            DataTable sourceDt = generateSourceTable(startTime);
            DataTable destTable = generatePollingStaDayTable(sourceDt);

            write2Db(destTable, "Polling_Log_Sta_Day");
            // LogUtil.logFinishPeroid("finish processing data from  " + startTime.ToString(TIMEFORMAT) + " to " + endTime.ToString(TIMEFORMAT) + "", startTime, endTime);
            if (proceeTime == endTime)//当天执行了，返回今天0点
            {
                endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day);
            }

            //  ConfigurationManager.AppSettings["lastExcuteTime"] = endTime.ToShortDateString();
            CommonUtil.SetConfigValue("lastExcuteTime", endTime.ToShortDateString());

            return endTime;
        }
       


        private void write2Db(DataTable plsd, string tableName)
        {

            string deleteSql = "delete from Polling_Log_Sta_Day where occurtime >='" + startTime.Year + "-" + startTime.Month + "-" + startTime.Day + "'";
            sqlHelper.ExecteNonQueryText(deleteSql);
            sqlHelper.DataTableToSQLServer(plsd, tableName);
            //  LogUtil.log("finish writing data to table Polling_Log_Sta_Day");


        }

        private void preDealSourceTable()
        {
            string sql = "if not exists (select * from syscolumns where id=object_id('Polling_Log') and name='check_time') ALTER TABLE Polling_Log add check_time datetime ";
            sqlHelper.ExecteNonQueryText(sql);
            sql = @"
            UPDATE Polling_Log
            SET check_time = 
            (CASE 
            WHEN (Datepart(hh, pl_time)!=23 and Datepart(n, pl_time)>50) THEN Dateadd(MINUTE, 10, pl_time)
            WHEN (Datepart(hh, pl_time) =23 and Datepart(n, pl_time)>50) THEN CAST(DATENAME(YEAR,Dateadd(MINUTE, 10, pl_time))+'-'+DATENAME(MONTH,Dateadd(MINUTE, 10, pl_time))+'-'+DATENAME(DAY,Dateadd(MINUTE, 10, pl_time))+' 00:00:00' as DATETIME) 
            ELSE pl_time END) 
            WHERE
            check_time is null and
            Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ("+CommonUtil. getUsageMatchStrFromConfig()+")))";
            sqlHelper.ExecteNonQueryText(sql);
        }

        private DataTable generateSourceTable(DateTime startTime)
        {
            string sql = "select pl.*,fc.Facility_id as fcid from polling_log pl LEFT JOIN Facility_Config fc on pl.msc_id = fc.msc_id  where  check_Time>= '" + startTime.ToShortDateString() + "' and check_Time< '" + endTime.ToString() + "' and pl.Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ("+CommonUtil. getUsageMatchStrFromConfig()+"))) order by check_time asc ";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            HashSet<string> set = new HashSet<string>();
            DataTable sourceTable = dataSet.Tables[0].Copy();
            sourceTable.Clear();
            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string key = Convert.ToDateTime(dr["check_time"]).ToString("yyyy-MM-dd HH") + "-" + dr["msc_id"].ToString();
                if (!set.Contains(key))
                {
                    set.Add(key);
                    DataRow newRow = sourceTable.NewRow();
                    for (int i = 0; i < sourceTable.Columns.Count; i++)
                    {
                        newRow[i] = dr[i];
                    }
                    sourceTable.Rows.Add(newRow);

                }
            }
            LogUtil.logGenerateSource("Polling_Log", startTime.ToString(TIMEFORMAT), endTime.ToString(TIMEFORMAT), dataSet.Tables[0].Rows.Count);
            return sourceTable;
        }

        public DataTable generatePollingStaDayTable(DataTable sourceDT)
        {
            //dicMscAndFcid = CommonUtil.generateMscAndFcidMap(sqlHelper);
            DataTable insertDT = CommonUtil.createEmptyPollingStaDayTable();
            //  LogUtil.log("finished creating a empty pollingstadaydatatable ");
            Dictionary<string, float> maxValueDic = getAllMsgIdAndMaxValueUntillStartTime(startTime);
            List<string> mscList = getStaMscIds();

            createInitTable(mscList, insertDT);

            setTotalValue(sourceDT, maxValueDic, mscList, insertDT);

            setNetValue(insertDT);

            setFirstNetValue(insertDT, mscList);

            CommonUtil.deleteUnusefulData(insertDT, 24);
            //  LogUtil.log("finish generating PollingStaDay Table ");

            return insertDT;
        }

        private void createInitTable(List<string> mscList, DataTable insertDT)
        {
            DateTime startTimeCopy = startTime;
            while (startTimeCopy < endTime)
            {
                for (int i = 0; i < mscList.Count; i++)
                {
                    PollingStaDay psd = new PollingStaDay();
                    psd.id = 0;
                    psd.year = startTimeCopy.Year;
                    psd.month = startTimeCopy.Month;
                    psd.day = startTimeCopy.Day;
                    psd.mscid = int.Parse(mscList[i]);
                    CommonUtil.addNewRowForDataTable(insertDT, psd);
                }
                startTimeCopy = startTimeCopy.AddDays(1);

            }
            // LogUtil.log("finish creating InitTable, insertDT rows:" + insertDT.Rows.Count);
        }

        private void setTotalValue(DataTable sourceDT, Dictionary<String, float> maxValueDic, List<string> mscList, DataTable insertDT)
        {
            try
            {
                for (int i = 0; i < sourceDT.Rows.Count; i++)
                {
                    DateTime occurTime = Convert.ToDateTime(sourceDT.Rows[i]["check_time"]);
                    int mscid = Convert.ToInt32(sourceDT.Rows[i][2]);
                    float value = Convert.ToSingle(sourceDT.Rows[i][3]);
                
                    string select = " mscid = " + mscid + " and year = " + occurTime.Year + " and month=" + occurTime.Month + " and day = " + occurTime.Day;
                    DataRow[] drs = insertDT.Select(select);
                    foreach (DataRow item in drs)
                    {
                        item[colLabel + (occurTime.Hour)] = value;
                        item["haveData"] = 1;
                    }

                }
                for (int i = 0; i < mscList.Count; i++)
                {
                    string mscid = mscList[i];
                    string select = " mscid = " + mscid;
                    DataRow[] drs = insertDT.Select(select);
                    float toValue = 0;
                    if (maxValueDic.ContainsKey(mscid))
                    {
                        toValue = maxValueDic[mscid];
                    }
                    foreach (DataRow item in drs)
                    {
                        int hourBound = 23;
                        int year = Convert.ToInt32(item["year"]);
                        int month = Convert.ToInt32(item["month"]);
                        int day = Convert.ToInt32(item["day"]);
                        DateTime now = DateTime.Now;
                        if (now.Year == year && now.Month == month && now.Day == day)
                        {
                            hourBound = now.Hour;
                        }
                        for (int j = 0; j <= hourBound; j++)
                        {
                            float actualValue = Convert.ToSingle(item[colLabel + j]);
                            if (actualValue > 0)
                            {
                                toValue = actualValue;
                            }
                            else
                            {
                                item[colLabel + j] = toValue;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {

                LogUtil.log(e.StackTrace.ToString());
                LogUtil.log(e.Message);
            }

            //  LogUtil.log("finish setting total values");
        }

        private void setNetValue(DataTable insertDT)
        {
            for (int i = 0; i < insertDT.Rows.Count; i++)
            {
                DataRow singleRow = insertDT.Rows[i];
                int hourBound = 23;
                int year = Convert.ToInt32(singleRow["year"]);
                int month = Convert.ToInt32(singleRow["month"]);
                int day = Convert.ToInt32(singleRow["day"]);
                int mscid = Convert.ToInt32(singleRow["mscid"]);

                DateTime now = DateTime.Now;
                int fcid = 0;
                //if (dicMscAndFcid.ContainsKey(mscid + ""))
                //{
                //    fcid = Convert.ToInt32(dicMscAndFcid["" + mscid]);
                //}
                insertDT.Rows[i]["fcid"] = fcid;
                if (now.Year == year && now.Month == month && now.Day == day)
                {
                    hourBound = now.Hour;
                }

                for (int j = 0; j < hourBound; j++)
                {
                    float tj = Convert.ToSingle(singleRow[colLabel + j]);
                    float tjplus = Convert.ToSingle(singleRow[colLabel + (j + 1)]);
                    singleRow[colLabel + "n" + (j + 1)] = (tjplus - tj) < 0 ? 0 : tjplus - tj;
                }

            }
            //   LogUtil.log("finish settingNetValue , starttime:" + startTime.ToString() + ", rows :" + insertDT.Rows.Count);
        }

        private void setFirstNetValue(DataTable insertDT, List<string> mscList)
        {
            Dictionary<string, float> lastDayDic = new Dictionary<string, float>();
            //获取前一天t23的值，用今天的t0减去，就会得到今天的n0
            string sql = "select mscid, " + colLabel + "23  from Polling_Log_Sta_Day where occurtime = '" + startTime.AddDays(-1).ToString() + "'";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            if (CommonUtil.firstTableHaveRow(dataSet))
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    string mscid = dataSet.Tables[0].Rows[i][0].ToString();
                    float t23 = Convert.ToSingle(dataSet.Tables[0].Rows[i][1].ToString());
                    lastDayDic.Add(mscid, t23);
                }
                for (int i = 0; i < mscList.Count; i++)
                {
                    string select = " mscid = " + mscList[i];
                    DataRow[] dr = insertDT.Select(select);

                    if (lastDayDic.ContainsKey(mscList[i]))
                    {
                        float t23 = lastDayDic[mscList[i]];
                        float t0 = Convert.ToSingle(dr[0][colLabel + "0"]);
                        float n0 = t0 - t23;
                        if (n0 < 0.01) { n0 = 0; }
                        dr[0][colLabel + "n0"] = n0;
                    }
                }
            }

            for (int i = 0; i < insertDT.Rows.Count; i++)
            {
                for (int j = 0; j <= 23; j++)
                {
                    if (Convert.ToSingle(insertDT.Rows[i][colLabel + "n" + j]) < 0)
                    {
                        insertDT.Rows[i][colLabel + "n" + j] = 0f;
                    }
                    if (Convert.ToSingle(insertDT.Rows[i][colLabel + "" + j]) < 0)
                    {
                        insertDT.Rows[i][colLabel + "" + j] = 0f;
                    }
                }
            }
        }

        private Dictionary<String, float> getAllMsgIdAndMaxValueUntillStartTime(DateTime startTime)
        {
            Dictionary<String, float> result = new Dictionary<string, float>();
            string sql = "select Msc_ID,Value from (SELECT * , Row_Number() OVER (partition by Msc_ID ORDER BY check_Time desc) rank FROM Polling_Log where  check_Time < cast('" + startTime.ToString() + "' as DateTime)  and Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ("+CommonUtil. getUsageMatchStrFromConfig()+"))))T  where T.rank = 1";
            //string sql = "select Msc_ID, max(VALUE) as maxValue from Polling_Log where  check_Time < cast('" + startTime.ToString() + "' as DateTime)  and Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ("+CommonUtil. getUsageMatchStrFromConfig()+")))  GROUP BY Msc_ID ";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            if (CommonUtil.firstTableHaveRow(dataSet))
            {

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    string mscid = dataSet.Tables[0].Rows[i][0].ToString();
                    float maxValue = Convert.ToSingle(dataSet.Tables[0].Rows[i][1]);
                    result.Add(mscid, maxValue);
                }
            }
            // LogUtil.log("finish getting  maxValue until starttime:" + startTime.ToString() + ", rows :" + dataSet.Tables[0].Rows.Count);
            return result;

        }

        private List<string> getStaMscIds()
        {
            List<string> idList = new List<string>();
            string sql = "select   DISTINCT Msc_ID  from Facility_Config where Usage_ID in ( select  Usage_id from  [Usage] where Usage_Name  in ("+CommonUtil. getUsageMatchStrFromConfig()+"))";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            if (CommonUtil.firstTableHaveRow(dataSet))
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    idList.Add(dataSet.Tables[0].Rows[i][0].ToString());
                }
            }
            // LogUtil.log("finish getting mscids  rows :" + idList.Count);
            return idList;
        }

        private List<string> getStaUsageIds()
        {
            List<string> idList = new List<string>();
            DataSet dataSet = sqlHelper.ExecuteDataSet("select  Usage_id from  [Usage] where Usage_Name  in ("+CommonUtil. getUsageMatchStrFromConfig()+");");
            if (CommonUtil.firstTableHaveRow(dataSet))
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    idList.Add(dataSet.Tables[0].Rows[i][0].ToString());
                }
            }
            return idList;
        }
    }
}





