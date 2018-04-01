using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Db;
using System.Data;
using YdService.Util;
using YdService.Model;

namespace ConsoleApplication1.Generater
{
    class PSYDBGenerater
    {
        private class YearSrcBean
        {
            public int year { set; get; }
            public int month { set; get; }
            public int mscid { set; get; }
            public int fcid { set; get; }
            public byte haveData { set; get; }
            public float net { set; get; }
            public float total { set; get; }

            public string getKey()
            {
                return year + "-" + month + "-" + mscid;
            }
        }
        private DateTime startTime;//start time of this period
        private DateTime proceeTime;//the actual time of the program prcessing , same to datetime.now
        private string dbName;
        private SqlHelper sqlHelper;
        private readonly static string TIMEFORMAT = "yyyy-MM-dd HH:mm:ss";

        public PSYDBGenerater()
        {
        }

        public PSYDBGenerater(string dbName, DateTime startTime, DateTime proceeTime)
        {
            this.dbName = dbName;
            this.proceeTime = proceeTime;
            sqlHelper = new SqlHelper(dbName);
            this.startTime = startTime;
         
        }

        public DateTime run()
        {
            Dictionary<string, YearSrcBean> sDic = generateSourceDic(startTime);
            DataTable psyTable = CommonUtil.createEmptyPollingStaYearTable();
            List<string> mscList = getStaMscIds();
            createInitTable(mscList, psyTable);
            setValue(sDic, psyTable);
            CommonUtil.deleteUnusefulData(psyTable, 12);
            write2Db(psyTable, "Polling_Log_Sta_Year");

            if (startTime.Year == proceeTime.Year)
            {
                // current year and not the last day,return jan 1
                return new DateTime(startTime.Year, 1, 1);
            }
            else
            {
                // not current month ,return the next jan 1.
                return new DateTime(startTime.Year + 1, 1, 1);
            }
          
        }

        private Dictionary<string, YearSrcBean> generateSourceDic(DateTime startTime)
        {
            Dictionary<string, YearSrcBean> result = new Dictionary<string, YearSrcBean>();
            string netColum = "(";
            for (int i = 1; i <= 31; i++)
            {
                netColum += "Dn" + i;
                if (i != 31)
                {
                    netColum += "+";
                }
            }
            netColum += ") as monthNet";
            string sql = " select year,month,mscid,D28,D29,D30,D31,fcid,haveData," + netColum + " from Polling_Log_Sta_Month where  occurtime>= '" + startTime.ToString("yyyy-01-01") + "' and occurtime<= '" + startTime.ToString("yyyy-12-31 23:59:59") + "'";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
           // LogUtil.log(" finished generating source tables from Polling_Log_Sta_Month from " + startTime.ToString("yyyy - 01 - 01") + " to " + startTime.ToString("yyyy-12-31") + " ,source rows :" + dataSet.Tables[0].Rows.Count);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                YearSrcBean ysb = new YearSrcBean();
                ysb.year = Convert.ToInt32(dataSet.Tables[0].Rows[i][0]);
                ysb.month = Convert.ToInt32(dataSet.Tables[0].Rows[i][1]);
                int lastday = CommonUtil.getLastDayOfMonth(new DateTime(ysb.year, ysb.month, 1));
                ysb.mscid = Convert.ToInt32(dataSet.Tables[0].Rows[i][2]);
                ysb.total = Convert.ToSingle(dataSet.Tables[0].Rows[i]["D" + lastday]);
                ysb.net = Convert.ToSingle(dataSet.Tables[0].Rows[i]["monthNet"]);
                ysb.fcid = Convert.ToInt32(dataSet.Tables[0].Rows[i]["fcid"]);
                ysb.haveData = Convert.ToByte(dataSet.Tables[0].Rows[i]["haveData"]);
                result.Add(ysb.getKey(), ysb);
            }

            LogUtil.logGenerateSource("Polling_Log_Sta_Month", startTime.ToString("yyyy-01-01 00:00:00"), startTime.ToString("yyyy-12-31 23:59:59"), dataSet.Tables[0].Rows.Count);

            return result;
        }

        private void createInitTable(List<string> mscList, DataTable insertDT)
        {
            DateTime startTimeCopy = startTime;
            while (new DateTime(startTimeCopy.Year, 1, 1) < proceeTime)
            {
                for (int i = 0; i < mscList.Count; i++)
                {
                    PollingStaYear psm = new PollingStaYear();
                    psm.id = 0;
                    psm.year = startTimeCopy.Year;
                    psm.mscid = int.Parse(mscList[i]);
                    CommonUtil.addNewRowForDataTable(insertDT, psm);
                }
                startTimeCopy = startTimeCopy.AddYears(1);
            }
        }

        private void setValue(Dictionary<string, YearSrcBean> sDic, DataTable insertDT)
        {
            try
            {
                for (int i = 0; i < insertDT.Rows.Count; i++)
                {
                    int year = Convert.ToInt32(insertDT.Rows[i]["year"]);
                    int mscid = Convert.ToInt32(insertDT.Rows[i]["mscid"]);
                    for (int j = 1; j < 13; j++)
                    {
                        int month = j;
                        string key = year + "-" + month + "-" + mscid;
                        float iNet = 0f;
                        float iTotal = 0f;
                        int fcid = 0;
                        byte haveData = 0;
                        if (sDic.ContainsKey(key))
                        {
                            YearSrcBean ysb = sDic[key];
                            iTotal = ysb.total;
                            iNet = ysb.net;
                            fcid = ysb.fcid;
                            haveData = ysb.haveData;
                        }
                        insertDT.Rows[i]["M" + j] = iTotal;
                        insertDT.Rows[i]["Mn" + j] = iNet;
                        if (fcid != 0)
                        {
                            insertDT.Rows[i]["fcid"] = fcid;
                        }
                        if (haveData != 0)
                        {
                            insertDT.Rows[i]["haveData"] = haveData;
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

        private void write2Db(DataTable plsd, string tableName)
        {
            string deleteSql = "delete from Polling_Log_Sta_Year where occurtime >='" + startTime.Year + "-01-01';" ;
            sqlHelper.ExecteNonQueryText(deleteSql);
            sqlHelper.DataTableToSQLServer(plsd, tableName);
            LogUtil.log("finish writing data to table Polling_Log_Sta_Year : " + startTime.Year);
        }

    }





}
