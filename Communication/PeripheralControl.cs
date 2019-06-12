using log4net;
using System;
using System.IO.Ports;
using System.Reflection;
using System.Windows.Media;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware.Communication
{
    public class PeripheralEventArgs : EventArgs
    {
        private Peripheral periperal;
        private bool status;

        public PeripheralEventArgs(Peripheral periperal, bool peripheralStatus)
        {
            this.Periperal = periperal;
            this.Status = peripheralStatus;
        }

        public Peripheral Periperal { get => periperal; set => periperal = value; }
        public bool Status { get => status; set => status = value; }
    }
    public sealed class PeripheralControl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler PeripheralUpdate;

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
     
        public enum Peripheral {Backlight = 6, GrowLight = 1 };
        public void Connect()
        {
            switch (Properties.Settings.Default.PeripheralConType)
            {
                case ConnectionType.Serial:
                    _log.Info("Opening Peripheral Serial Port");
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
            TimeSpan startOfNight = Properties.Settings.Default.StartOfNight;
            TimeSpan endOfNight = Properties.Settings.Default.EndOfNight;
            TimeSpan now = DateTime.Now.TimeOfDay;

            //_log.Debug("Current total hours: " + now.TotalHours);
            return (now >= startOfNight || now <= endOfNight) ? true : false;

        }
        public void SetLight(Peripheral peripheral, bool status, bool daytime)
        {
            if (status && daytime)
            {
                _log.Info(peripheral.ToString() + " lights on");
                SetLight(peripheral,status);
            }
            else {
                _log.Info(peripheral.ToString() + " lights off");
                SetLight(peripheral, false); }
        }
        public void SetLight(Peripheral peripheral, bool value)
        {
            PeripheralUpdate.Raise(this, new PeripheralEventArgs(peripheral,value));
            string cmdStr = "";
            if(peripheral == Peripheral.Backlight)
            {
                cmdStr = "S2P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            else if( peripheral == Peripheral.GrowLight)
            {
                cmdStr = "S1P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            SendCommand(cmdStr);
            
        }
        public void SetBacklightColor(Color color)
        {
            string cmdStr = "S3P0R" + color.R.ToString() + "G" + color.G.ToString() + "B" + color.B.ToString();
            SendCommand(cmdStr);
        }
        private int BtoI(bool value)
        {
            return (value == true) ? 1 : 0;
        }
        private void SendCommand(string commandString)
        {
            _log.Debug(commandString);
            if (port!= null && port.IsOpen)
            {
                port.WriteLine(commandString);
            }
            
        }
        private void Peripheral_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            _log.Debug("From Peripheral:" + port.ReadLine());
        }
        public void Disconnect()
        {
            port.Close();
        }
    }
}
