using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthernetService
{
    public static class Library
    {
        public static void deleteLog()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt");
            }
        }
        /// <summary>
        /// Writes an error message to my logfile
        /// </summary>
        /// <param name="ex"></param>
        public static void writeErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": Error occured in '" + ex.Source.ToString().Trim() + "': " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Writes a custom message to the logfile
        /// </summary>
        /// <param name="Message"></param>
        public static void writeErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
