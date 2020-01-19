using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyTool
{
    class Log
    {
        public static void WriteLog(string msg, string logPath, string type)
        {
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            type = type ?? "0";
            logPath = logPath ?? @"D:\十大股东部署文件\文件解析服务\Config\";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    if (type == "0")
                        sw.WriteLine("************************异常开始**************************");
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("消息：" + msg);
                    if (type == "0")
                        sw.WriteLine("************************异常结束**************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    if (type == "0")
                        sw.WriteLine("************************异常开始**************************");
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (type == "0")
                        sw.WriteLine("消息：" + e.Message);
                    sw.WriteLine("************************异常结束**************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }
}
