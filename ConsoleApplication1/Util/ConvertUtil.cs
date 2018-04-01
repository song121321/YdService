using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace YdService.Util
{
    public class ConvertUtil
    {
        public static string dataTable2String(DataTable dt){

            string result = "";

            for (int i = 0; i < dt.Columns.Count; i++)
			{
                result += "   " + dt.Columns[i].ColumnName;
			}
            result += "\n";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    result += "   " + dt.Rows[i][j].ToString();
                }

                result += "\n";
            }

            return result;
        
        }

      
       


    }
}
