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
    public class PSMDBGenerater
    {
        private class MonthSrcBean
        {
            public int year { set; get; }
            public int month { set; get; }
            public int day { set; get; }
            public int mscid { set; get; }
            public int fcid { set; get; }
            public byte haveData { set; get; }
            public float net { set; get; }
            public float total { set; get; }
          

            public string getKey()
            {
                return year + "-" + month + "-"+day+"-" + mscid;
            }
        }

        private DateTime startTime;//start time of this period
        private DateTime proceeTime;//the actual time of the program prcessing , same to datetime.now
        private DateTime endTime;
        private string dbName;
        private SqlHelper sqlHelper;
        private Dictionary<string, MonthSrcBean> sDic = new Dictionary<string, MonthSrcBean>();
        

        public PSMDBGenerater()
        {

        }


        public PSMDBGenerater(string dbName, DateTime startTime, DateTime proceeTime)
        {
            this.dbName = dbName;
            sqlHelper = new SqlHelper(dbName);
            this.startTime = startTime;
            this.proceeTime = proceeTime;
            int dayBound = CommonUtil.getLastDayOfMonth(startTime);
            int year = startTime.Year;
            int month = startTime.Month;
            if (proceeTime.Year == year && proceeTime.Month == month)
            {
                dayBound = proceeTime.Day;
            }
            this.endTime = new DateTime(year, month, dayBound);
        }

        public DateTime run()
        {
            DataTable dayTable = generateSourceTable(startTime);
            DataTable destTable = generatePollingStaMonthTable(dayTable);
            write2Db(destTable, "Polling_Log_Sta_Month");

            if ((endTime.Year==proceeTime.Year&&endTime.Month==proceeTime.Month))//&&endTime.Day != CommonUtil.getLastDayOfMonth(startTime))
            {
                // current month and not the last day,return current month day 1
                endTime = new DateTime(endTime.Year,endTime.Month,1); // endTime.AddDays((-endTime.Day) + 1);
            }
            else {
                // not current month ,return the next month and day 1.
                endTime = endTime.AddDays(1);
            }

            return endTime;
        }
        public DataTable generatePollingStaMonthTable(DataTable dayTable)
        {
            List<string> mscList = getStaMscIds();
            DataTable psmTable = CommonUtil.createEmptyPollingStaMonthTable();
            createInitTable(mscList, psmTable);
            setValue(psmTable);
            CommonUtil.deleteUnusefulData(psmTable, 31);
            return psmTable;
        }

        private DataTable generateSourceTable(DateTime startTime)
        {

            string netColum = "(";
            for (int i = 0; i < 24; i++)
            {
                netColum += "Hn" + i;
                if (i != 23)
                {
                    netColum += "+";
                }
            }
            netColum += ") as dayNet";


            string sql = " select year,month,day,mscid,H23,fcid,haveData," + netColum + " from Polling_Log_Sta_Day where  occurtime>= '" + startTime.ToShortDateString() + "' and occurtime<= '" + endTime.ToString("yyyy-MM-dd 23:59:59") + "'";
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                MonthSrcBean msb = new MonthSrcBean();
                msb.year = Convert.ToInt32(dataSet.Tables[0].Rows[i]["year"]);
                msb.month = Convert.ToInt32(dataSet.Tables[0].Rows[i]["month"]);
                msb.day = Convert.ToInt32(dataSet.Tables[0].Rows[i]["day"]);
                msb.mscid = Convert.ToInt32(dataSet.Tables[0].Rows[i]["mscid"]);
                msb.total = Convert.ToSingle(dataSet.Tables[0].Rows[i]["H23"]);
                msb.net = Convert.ToSingle(dataSet.Tables[0].Rows[i]["dayNet"]);
                msb.fcid = Convert.ToInt16(dataSet.Tables[0].Rows[i]["fcid"]);
                msb.haveData = Convert.ToByte(dataSet.Tables[0].Rows[i]["haveData"]);
                sDic.Add(msb.getKey(), msb);
            }
            LogUtil.logGenerateSource("Polling_Log_Sta_Day", startTime.ToString(), endTime.ToString("yyyy-MM-dd 23:59:59"), dataSet.Tables[0].Rows.Count);
            return dataSet.Tables[0];// dataSet.Tables[0];
        }
       
        private void createInitTable(List<string> mscList, DataTable insertDT)
        {
            DateTime startTimeCopy = startTime;
            while (new DateTime(startTimeCopy.Year, startTimeCopy.Month, 1) < endTime)
            {
                for (int i = 0; i < mscList.Count; i++)
                {
                    PollingStaMonth psm = new PollingStaMonth();
                    psm.id = 0;
                    psm.year = startTimeCopy.Year;
                    psm.month = startTimeCopy.Month;
                    psm.mscid = int.Parse(mscList[i]);
                    CommonUtil.addNewRowForDataTable(insertDT, psm);
                }
                startTimeCopy = startTimeCopy.AddMonths(1);
            }
            // LogUtil.log("finish creating InitTable, insertDT rows:" + insertDT.Rows.Count);
        }

        private void setValue(DataTable insertDT) {
            try
            {
                for (int i = 0; i < insertDT.Rows.Count; i++)
                {
                    int year = Convert.ToInt32(insertDT.Rows[i]["year"]);
                    int month = Convert.ToInt32(insertDT.Rows[i]["month"]);
                    int mscid = Convert.ToInt32(insertDT.Rows[i]["mscid"]);
                  
                    for (int j = 1; j < 32; j++)
                    {
                        string key = year + "-" + month + "-" +j+"-"+ mscid;
                        float iNet = 0f;
                        float iTotal = 0f;
                        int fcid = 0;
                        byte haveData = 0;
                        if (sDic.ContainsKey(key))
                        {
                            MonthSrcBean msb = sDic[key];
                            iTotal = msb.total;
                            iNet = msb.net;
                            fcid = msb.fcid;
                            haveData = msb.haveData;
                        }
                        insertDT.Rows[i]["D" + j] = iTotal;
                        insertDT.Rows[i]["Dn" + j] = iNet;
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
        }

        private void write2Db(DataTable plsd, string tableName)
        {

            string deleteSql = "delete from Polling_Log_Sta_Month where occurtime >='" + startTime.Year + "-" + startTime.Month + "-01';";
            sqlHelper.ExecteNonQueryText(deleteSql);
            sqlHelper.DataTableToSQLServer(plsd, tableName);
            LogUtil.log("finish writing data to table Polling_Log_Sta_Month, month:"+startTime.Year+"-"+startTime.Month);
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


    }
}
