using System;
using System.IO;
using System.Windows.Forms;

namespace JobboMilling
{
    class Log
    {
        public static string LogPath { get; set; }
        public static bool LogChartData { get; set; }
        public static void AddToLog(string LogItem)
        {

            try
            {
                using (StreamWriter LogFile = new StreamWriter(LogPath, true))
                {
                    string Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
                    LogFile.WriteLine(Time + ": " + LogItem);
 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Log error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void InitLogFile()
        {
            
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            string LogPathFile = LogPath + "\\Log_JobboMilling_" + DateTime.Now.ToString("MM/dd/yyyy") + ".txt";
            if (!File.Exists(LogPathFile))
            {
                try
                {
                    using (StreamWriter LogFile = File.CreateText(LogPathFile))
                    {
                        LogFile.WriteLine("-----------------------------------------------------");
                        LogFile.WriteLine("-----------------------------------------------------");
                        LogFile.WriteLine("JobboMilling version: " + Program.Version);
                        LogFile.WriteLine("Start log: " + DateTime.Now.ToString("MM/dd/yyyy H:mm"));
                        LogFile.WriteLine("Username: " + Environment.UserName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Log error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    using (StreamWriter LogFile = new StreamWriter(LogPathFile, true))
                    {
                        LogFile.WriteLine("-----------------------------------------------------");
                        LogFile.WriteLine("-----------------------------------------------------");
                        LogFile.WriteLine("JobboMilling version: " + Program.Version);
                        LogFile.WriteLine("Start new log: " + DateTime.Now.ToString("MM/dd/yyyy H:mm"));
                        LogFile.WriteLine("Username: " + Environment.UserName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Log error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LogPath = LogPathFile;

        }
    }
}
