using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YdService.Db;
using System.Data;
using YdService.Util;

namespace YdService.Util
{
    public class SQLUtil
    {
        private SqlHelper sqlHelper;
        public SQLUtil()
        {
            sqlHelper = new SqlHelper("master");
        }

        public List<string> getAvailableDataBaseList()
        {
            List<string> result = new List<string>();
            string sql = "select name from master..sysdatabases where name not in ('ydxx','master','tempdb','model','msdb') ";
            DataSet ds = sqlHelper.ExecuteDataSet(sql);
            if (CommonUtil.firstTableHaveRow(ds))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string DbName = ds.Tables[0].Rows[i][0].ToString();

                    string asumeTableSql = "select count(*) from sysobjects where id in( object_id('Polling_Log'),object_id('Polling_Log_Sta_Day'))";

                    sqlHelper = new SqlHelper(DbName);

                    object obj = sqlHelper.ExecuteScalarText(asumeTableSql);

                    int number = obj != DBNull.Value ? (int)obj : 0;
                    if (number == 2)
                    {
                        result.Add(DbName);
                    }

                }

            }
            return result;
        }
    
    
    
    }
}
