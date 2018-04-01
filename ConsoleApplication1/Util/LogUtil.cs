using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace YdService.Util
{
    public class LogUtil
    {
        private static string logPath ="log\\"+ ConfigurationManager.AppSettings["logName"].Trim().ToString()+"-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        
        public static void starLog(string  content){
            addEnter(1, "%%%%%%%%%%%%%%" + content + "%%%%%%%%%%%%%%%%%%%%%%%");
           Console.WriteLine("%%%%%%%%%%%%%%"+content+"%%%%%%%%%%%%%%%%%%%%%%%");
        }
        
        
        public static void log(string content)
        {
            //  TextUtil.appendToTxt(logPath, content);
            addEnter(1, content);
        }

        public static void logGenerateSource(string tableName, string startTtime, string endtime, int count)
        {
            string content = "finished generating source tables from " + tableName + ", time from   [" + startTtime.ToString() + "] to [" + endtime.ToString() + "] , count:  " + count + "--logDate: [" + DateTime.Now + "]";
            addEnter(1, content);
        }

        public static void logFinishTicked(int tickTimes)
        {
            string content = "################## ticks [" + tickTimes + "] #####[" + DateTime.Now + "]";
            addEnter(6, content);
        }


        public static void logFinishDataBase(string dbName)
        {
            string content = "***************** dataBase[" + dbName + "] finished*****[" + DateTime.Now + "]";
            addEnter(1, content);
        }

        public static void log2Enter(string content)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(2, content);

        }

        public static void log3Enter(string content)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(3, content);
        }

        private static void addEnter(int number, string content)
        {
            TextUtil.mkLogDir();
            for (int i = 0; i < number; i++)
            {
                TextUtil.appendToTxt(logPath, " ");
                if (number % 2 == 1)
                {
                    Console.WriteLine("\n");
                }
            }
            TextUtil.appendToTxt(logPath, content);
            Console.WriteLine(content);
        }

    }
}
