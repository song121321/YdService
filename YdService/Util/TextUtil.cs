using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YdService.Util
{
    public class TextUtil
    {
        #region 读取txt文档
        public static String readFromTxt(String txtPath)
        {
            return File.ReadAllText(txtPath, Encoding.Default);
        }
        #endregion

        public static void writeToTxt(string path, string content)
        {

            if (!File.Exists(path))
            {
                FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(content);
                sw.Close();
                fs1.Close();
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(content);
                sr.Close();
                fs.Close();
            }


        }

        public static void appendToTxt(string path, string content)
        {
            if (!File.Exists(path))
            {
                FileInfo myfile = new FileInfo(path);
                FileStream fs = myfile.Create();
                fs.Close();
            }
            StreamWriter sw = File.AppendText(path);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }


    }
}
