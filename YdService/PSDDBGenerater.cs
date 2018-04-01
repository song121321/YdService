using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Model;
using System.Data;
using YdService.Db;
using YdService.Util;

namespace YDIOTService
{
    public class PSDDBGenerater
    {

        private DateTime startTime;//start time of this period
        private DateTime endTime;//end time of this period ,caculated
        private DateTime proceeTime;//the actual time of the program prcessing , same to datetime.now
        private string dbName;
        private SqlHelper sqlHelper;
        private readonly static string TIMEFORMAT = "yyyy-MM-dd HH:mm:ss";

        //  private  String log = "";

        public PSDDBGenerater()
        {
        }
        public PSDDBGenerater(string dbName, DateTime startTime, DateTime proceeTime)
        {
            this.dbName = dbName;
            this.proceeTime = proceeTime;
            sqlHelper = new SqlHelper(dbName);
            this.startTime = startTime;
            if ((proceeTime - startTime).Days > 3)
            {
                endTime = startTime.AddDays(3);
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

            LogUtil.log("start processing data from " + startTime.ToString(TIMEFORMAT) + " to " + endTime.ToString(TIMEFORMAT) + "");

            DataTable sourceDt = generateSourceTable(startTime);
            DataTable destTable = generatePollingStaDayTable(sourceDt);

            write2Db(destTable, "Polling_Log_Sta_Day");
            LogUtil.logFinishPeroid("finish processing data from  " + startTime.ToString(TIMEFORMAT) + " to " + endTime.ToString(TIMEFORMAT) + "", startTime, endTime);
            return endTime;
        }


        private void write2Db(DataTable plsd, string tableName)
        {

            string deleteSql = "delete from Polling_Log_Sta_Day where occurtime >='" + startTime.Year + "-" + startTime.Month + "-" + startTime.Day + "'";
            sqlHelper.ExecteNonQueryText(deleteSql);
            sqlHelper.DataTableToSQLServer(plsd, tableName);
            LogUtil.log("finish writing data to table Polling_Log_Sta_Day");


        }

        private DataTable generateSourceTable(DateTime startTime)
        {
            string sql = " select * from Polling_Log where  PL_Time>= '" + startTime.ToShortDateString() + "' and PL_Time< '" + endTime.ToShortDateString() + "' and Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ('正累积流量','正累计流量'))) ";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            LogUtil.log("finished generating source tables from polling_log from " + startTime.ToString(TIMEFORMAT) + " to " + endTime.ToString(TIMEFORMAT) + " ,source rows :" + dataSet.Tables[0].Rows.Count);
            return dataSet.Tables[0];
        }

        public DataTable generatePollingStaDayTable(DataTable sourceDT)
        {

            DataTable insertDT = CommonUtil.createEmptyPollingStaDayDataTable();
          //  LogUtil.log("finished creating a empty pollingstadaydatatable ");
            Dictionary<String, float> maxValueDic = getAllMsgIdAndMaxValueUntillStartTime(startTime);
            List<string> mscList = getStaMscIds();
            createInitTable(mscList, insertDT);

            setTotalValue(sourceDT, maxValueDic, mscList, insertDT);

            setNetValue(insertDT);
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
                    DateTime occurTime = Convert.ToDateTime(sourceDT.Rows[i][1]);
                    int mscid = Convert.ToInt32(sourceDT.Rows[i][2]);
                    float value = Convert.ToSingle(sourceDT.Rows[i][3]);
                    string select = " mscid = " + mscid + " and year = " + occurTime.Year + " and month=" + occurTime.Month + " and day = " + occurTime.Day;
                    DataRow[] drs = insertDT.Select(select);
                    foreach (DataRow item in drs)
                    {
                        item["t" + (occurTime.Hour + 1)] = value;
                    }

                }
                for (int i = 0; i < mscList.Count; i++)
                {
                    string mscid = mscList[i];
                    string select = " mscid = " + mscid;
                    DataRow[] drs = insertDT.Select(select);
                    float toValue = -1f;
                    if (maxValueDic.ContainsKey(mscid))
                    {
                        toValue = maxValueDic[mscid];
                    }
                    foreach (DataRow item in drs)
                    {

                        for (int j = 1; j <= 24; j++)
                        {
                            float actualValue = Convert.ToSingle(item["t" + j]);
                            if (actualValue > 0)
                            {
                                toValue = actualValue;
                            }
                            else
                            {
                                item["t" + j] = toValue;
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
                for (int j = 1; j < 24; j++)
                {
                    float tj = Convert.ToSingle(singleRow["t" + j]); ;
                    float tjplus = Convert.ToSingle(singleRow["t" + (j + 1)]);
                    singleRow["n" + j] = tjplus - tj;
                }

            }
         //   LogUtil.log("finish settingNetValue , starttime:" + startTime.ToString() + ", rows :" + insertDT.Rows.Count);
        }


        private Dictionary<String, float> getAllMsgIdAndMaxValueUntillStartTime(DateTime startTime)
        {
            Dictionary<String, float> result = new Dictionary<string, float>();
            string sql = "select Msc_ID, max(VALUE) as maxValue from Polling_Log where  PL_Time < cast('" + startTime.ToString() + "' as DateTime)  and Msc_ID in(select  Msc_ID from Facility_Config  where Usage_ID in (select Usage_ID from [Usage]  where Usage_Name in ('正累积流量','正累计流量')))  GROUP BY Msc_ID ";
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
            string sql = "select   DISTINCT Msc_ID  from Facility_Config where Usage_ID in ( select  Usage_id from  [Usage] where Usage_Name  in ('正累计流量','正累积流量'))";
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
            DataSet dataSet = sqlHelper.ExecuteDataSet("select  Usage_id from  [Usage] where Usage_Name  in ('正累计流量','正累积流量');");
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
