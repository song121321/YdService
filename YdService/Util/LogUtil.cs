using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace YdService.Util
{
    public class LogUtil
    {
        private static string logPath = ConfigurationManager.AppSettings["logPath"].Trim().ToString();// "d:/log/log.txt";
        public static void log(string content)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(1);
        }

        public static void logFinishPeroid(string content,DateTime startTtime,DateTime endtime)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(1);
            TextUtil.appendToTxt(logPath, "------------------------------- peroid ["+startTtime.ToString()+"] to ["+endtime.ToString()+"]   finished-------------------------------------------------[" + DateTime.Now+"]");
            addEnter(1);
        }



        public static void logDbStart(string dbName)
        {
            string content = "******************************* dataBase["+dbName+"] start to process****************************************************************************************************[" + DateTime.Now + "]";
            logStarWith2EmptyLine(content);
        }

        public static void logStarWith2EmptyLine(string content)
        {
            addEnter(1);
            TextUtil.appendToTxt(logPath, content);
            addEnter(1);
        }

        public static void logDbFinish(string dbName)
        {
            string content = "******************************* dataBase[" + dbName + "] finished to process*********************************************************************************************[" + DateTime.Now + "]";
            logStarWith2EmptyLine(content);
        }

        public static void log2Enter(string content)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(2);

        }

        public static void log3Enter(string content)
        {
            TextUtil.appendToTxt(logPath, content);
            addEnter(3);
        }

        private static void addEnter(int number) {

            for (int i = 0; i < number; i++)
            {
                TextUtil.appendToTxt(logPath, " ");
            }
        }

    }
}
