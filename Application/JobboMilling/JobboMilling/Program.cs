using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace JobboMilling
{
    static class Program
    {
        public static string Version { get; set; }
        public static bool Status { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Ini();

            if (Status)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                MessageBox.Show("Somthing went during boot", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static void Ini()
        {
            Status = true;
            ReadConfigFile();
            Log.InitLogFile();
            SerialCommunication.InitializeCOMports();
            SerialCommunication.StartUart();
        }
        private static void ReadConfigFile()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                Log.LogPath = appSettings["LogPath"];
                SerialCommunication.BaudRate = Int32.Parse(appSettings["BaudRate"]);
                SerialCommunication.COMPort = appSettings["COMport"];
            }
            catch (ConfigurationErrorsException)
            {

                MessageBox.Show("Error reading config file", "Config file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
