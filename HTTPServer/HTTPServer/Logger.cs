using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log");
        public static void LogException(Exception ex)
        {
            // Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            DateTime currentDate = DateTime.Now;
            string Datetime = "Datetime: " + currentDate.ToString("ddd, dd MMMM yyy HH':'mm':'ss' EST'");
            string message = "message: " + ex.Message;

            StreamWriter log = File.AppendText("log.txt");

            log.WriteLine(Datetime);
            log.WriteLine(message);
            log.Close();
        }
    }
}
