using System;
using System.IO.Ports;
using System.Windows.Media;

namespace SPIPware.Communication
{
    public sealed class PeripheralControl
    {
        private bool connected = false;
        private static readonly PeripheralControl instance = new PeripheralControl();
        static PeripheralControl()
        {

        }
        public static PeripheralControl Instance
        {
            get
            {
                return instance;
            }
        }

        public bool Connected { get => connected; set => connected = value; }

        SerialPort port;
     
        public enum Peripheral {Backlight = 6, GrowLight =1 };
        public void Connect()
        {
            switch (Properties.Settings.Default.PeripheralConType)
            {
                case ConnectionType.Serial:
                    port = new SerialPort(Properties.Settings.Default.PeripheralSP, Properties.Settings.Default.PeripheralBaud, Parity.None,8,StopBits.One);
                    port.DataReceived += new SerialDataReceivedEventHandler(Peripheral_DataReceived);
                    port.Open();
                    //setLight(Peripheral.Backlight, true);
                    Connected = true;
                    break;
                default:
                    Connected = false;
                    throw new Exception("Invalid Connection Type");
            }

        }

        public bool IsNightTime()
        {
            TimeSpan startOfNight = TimeSpan.Parse("23:00:00");
            TimeSpan endOfNight = TimeSpan.Parse("07:00:00");
            TimeSpan now = DateTime.Now.TimeOfDay;

            Console.WriteLine(now.TotalHours);
            return (now >= startOfNight || now <= endOfNight) ? true : false;

        }
        public void SetLight(Peripheral peripheral, bool status, bool daytime)
        {
            if (status && daytime)
            {
                SetLight(peripheral,status);
            }
            else { SetLight(peripheral, false); }
        }
        public void SetLight(Peripheral peripheral, bool value)
        {
            string cmdStr = "";
            if(peripheral == Peripheral.Backlight)
            {
                cmdStr = "S2P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            else if( peripheral == Peripheral.GrowLight)
            {
                cmdStr = "S1P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            Console.WriteLine(cmdStr);
            SendCommand(cmdStr);
            
        }
        public void SetBacklightColor(Color color)
        {
            string cmdStr = "S3P0R" + color.R.ToString() + "G" + color.G.ToString() + "B" + color.B.ToString();
            Console.WriteLine(cmdStr);
            SendCommand(cmdStr);
            //string cmdStr2 = "S4P0L" + color.A.ToString();
            //Console.WriteLine(cmdStr2);
            //SendCommand(cmdStr2);
        }
        private int BtoI(bool value)
        {
            return (value == true) ? 1 : 0;
        }
        private void SendCommand(string commandString)
        {
            //port.DiscardOutBuffer();

            if (port!= null && port.IsOpen)
            {
                port.WriteLine(commandString);
            }
            
        }
        private void Peripheral_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            Console.WriteLine(port.ReadLine());
        }
        public void Disconnect()
        {
            port.Close();
        }
    }
}
