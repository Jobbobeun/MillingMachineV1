using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobboMilling
{
    class SerialCommunication
    {
        public static SerialPort serialPort;
        public static List<Activity> Activity = new List<Activity>();
        public static string LastSendUartCommand { get; set; }
        public static string LastUartMessage { get; set; }
        public static string ReceivedUartMessage { get; set; }
        public static string ReceivedControllerInformation { get; set; }
        public static string ReceivedPositions { get; set; }
        public static string ReceivedStatus { get; set; }
        public static string ReceivedControllerAjust { get; set; }
        public static bool PortBusy { get; set; }
        public static bool PortActive { get; set; }
        public static int BaudRate { get; set; }
        public static string COMPort { get; set; }

        public static bool StartUart()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                try
                {
                    Log.AddToLog("Try to start serial communication on COMport: " + COMPort + " - " + BaudRate);
                    serialPort = new SerialPort();
                    serialPort.PortName = COMPort;
                    serialPort.BaudRate = BaudRate;
                    serialPort.Parity = Parity.None;
                    serialPort.StopBits = StopBits.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = Handshake.None;
                    serialPort.Open();
                    Log.AddToLog("Serial communication started at port: " + COMPort);
                    PortBusy = false;
                    PortActive = true;
                   
                    return true;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Serial port.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.AddToLog("Serial communication not possible: " + e.Message);
                    Program.Status = false;
                    return false;
                }
               
            }
            else if(serialPort.IsOpen)
            {
                MessageBox.Show("Serial port already running. Please disconnect before reconnecting.", "Serial port.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Status = false;
                return false;
            }
            else
            {
                Program.Status = false;
                return false;
            }
        }
        public static void StopUart()
        {
            Random rnd = new Random();
            int ClaimId = rnd.Next(1000);
            Activity.Add(new Activity() { PartId = ClaimId, PartName = "Closing Uart."});
            while (PortBusy == true) { Console.WriteLine("Wait for closing UART connection."); }
            serialPort.Close();
            //Program.Form.UpdateActiveCOMLabel("-");
            //Program.Form.UpdateStatusCOMLabel("Closed");
            //Program.Form.SetSystemStatus("Not connected");
            //LogWindow.AddToLog("Serial communication disconnected.");
            PortActive = false;
            PortBusy = true;
            Activity.Remove(new Activity() { PartId = ClaimId, PartName = "Closing Uart."});
            MessageBox.Show("Uart connection closed.", "Serial port.", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public static int BytesToRead()
        {
            return serialPort.BytesToRead;
        }
        public static bool SendUart(string Message)
        {
            if (serialPort.IsOpen)
            {
                Message = Message.Trim();
                Console.WriteLine("Send uart string: " + Message);
                try
                {
                    serialPort.Write(Message);
                    LastSendUartCommand = Message;
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Not possible to send the uart command.", "Serial port.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //LogWindow.AddToLog("Not possible to send the uart command.");
                    return false;
                }
            }
            else
            {
                //LogWindow.AddToLog("Not possible to send a message, port is not conneced.");
                MessageBox.Show("Serial port is not running. Please start before sending commands.", "Serial port.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static string[] GetExistingCOMports()
        {
            return SerialPort.GetPortNames();
        }
 
        public static string ReadSerial()
        {            
            return serialPort.ReadLine();
        }
        public static List<string> ReadSerialMultiple(string Message)
        {
            Random rnd = new Random();
            int ClaimId = rnd.Next(1000);
            Activity.Add(new Activity() { PartId = ClaimId, PartName = Message });
            while (PortBusy == true) { Console.WriteLine("Wait with messages for multiple response: " + Message); }
            PortBusy = true;
            SerialCommunication.ClearBuffer();
            //LogWindow.AddToLog(Convert.ToString(ClaimId) + ": Send Messages: " + Message);
            SerialCommunication.SendUart(Message);
            List<string> ReadDate = new List<string>();
            string ReadLine = serialPort.ReadLine();
            while (serialPort.BytesToRead > 0)
            {                 
                ReadLine = serialPort.ReadLine();
                ReadDate.Add(ReadLine);
                Thread.Sleep(1);
            }
            //LogWindow.AddToLog(Convert.ToString(ClaimId) + ": Respons " + Convert.ToString(ReadDate.Count) + " sampels.");
            Activity.Remove(new Activity() { PartId = ClaimId, PartName = Message });
            PortBusy = false;

            return ReadDate;
        }
        public static void ClearBuffer()
        {
            serialPort.DiscardInBuffer();
        }

        public static string SendAndReceive(string Message, bool Log = true)
        {
            Random rnd = new Random();
            int ClaimId = rnd.Next(1000);
            Activity.Add(new Activity() { PartId = ClaimId, PartName = Message });

            while (PortBusy == true) { Console.WriteLine("Wait with messages for single response: " + Message); }
            PortBusy = true;
            ClearBuffer();
            //if (Log){LogWindow.AddToLog(Convert.ToString(ClaimId) + ": Send Messages: " + Message);}
            SendUart(Message);
            string Received = ReadSerial();
            //if (Log){LogWindow.AddToLog(Convert.ToString(ClaimId) + ": Respons: " + Received);}
            Activity.Remove(new Activity() { PartId = ClaimId, PartName = Message });
            PortBusy = false;
            return Received;
        }

        public static bool ConnectionActive()
        {
            if (PortActive)
            {
                return true;
            }
            else
            {
                MessageBox.Show("No active serial port. Please connect first with the hardware", "Serial port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static void InitializeCOMports()
        {

            string[] COMPorts = SerialCommunication.GetExistingCOMports();
            if (COMPorts.Length > 0)
            {
                string ComList = "";
                foreach (string COMPort in COMPorts)
                {
                    if (ComList == "")
                    {
                        ComList = "Available COM ports: " + COMPort;
                    }
                    else
                    {
                        ComList = ComList + " & " + COMPort;
                    }
                }

                Log.AddToLog(ComList);
            }
            else
            {
                Log.AddToLog("No COM ports available.");
            }
        }
    }

    public class Activity : IEquatable<Activity>
    {
        public string PartName { get; set; }

        public int PartId { get; set; }

        public override string ToString()
        {
            return "ID: " + PartId + " Request: " + PartName;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Activity objAsPart = obj as Activity;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return PartId;
        }
        public bool Equals(Activity other)
        {
            if (other == null) return false;
            return (this.PartId.Equals(other.PartId));
        }
        // Should also override == and != operators.
    }
}
