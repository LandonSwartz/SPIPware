using System;
using System.IO.Ports;

namespace SPIPware.Communication
{
    class PeripheralControl
    {
        SerialPort port;
     
        public enum Peripheral {Backlight = 1, GrowLight =6 };
        public void Connect()
        {
            switch (Properties.Settings.Default.PeripheralConType)
            {
                case ConnectionType.Serial:
                    port = new SerialPort(Properties.Settings.Default.PeripheralSP, Properties.Settings.Default.PeripheralBaud, Parity.None,8,StopBits.One);
                    port.DataReceived += new SerialDataReceivedEventHandler(peripheral_DataReceived);
                    port.Open();
                    //setLight(Peripheral.Backlight, true);
                    break;
                default:
                    throw new Exception("Invalid Connection Type");
            }

        }


        public void SetLight(Peripheral peripheral, bool status, bool daytime)
        {
            if (status && daytime)
            {
                SetLight(Peripheral.GrowLight,status);
            }
            else { SetLight(Peripheral.GrowLight, false); }
        }
        public void SetLight(Peripheral peripheral, bool value)
        {
            string cmdStr = "S1P" + peripheral.ToString("D") + "V" + btoi(value);
            Console.WriteLine(cmdStr);
            sendCommand(cmdStr);
            
        }
        private int btoi(bool value)
        {
            return (value == true) ? 1 : 0;
        }
        private void sendCommand(string commandString)
        {
            //port.DiscardOutBuffer();
            port.WriteLine(commandString);
        }
        private void peripheral_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
