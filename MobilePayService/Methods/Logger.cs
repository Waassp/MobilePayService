using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace MobilePayService.Methods
{
    public class Logger
    {
        public static void Log(string message)
        {
            string logFile = ConfigurationManager.AppSettings["LogFile"];

            if (File.Exists(logFile))
            {

                using (StreamWriter sw = File.AppendText(logFile))
                {
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
        }
    }
}