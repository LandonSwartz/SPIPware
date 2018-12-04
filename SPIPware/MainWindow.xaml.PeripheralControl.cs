using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware
{
    partial class MainWindow
    {
        PeripheralControl peripheralControl = new PeripheralControl();
        private void ButtonGrowLightOn_Click(object sender, RoutedEventArgs e)
        {
           Thread t = new Thread (() => peripheralControl.SetLight(Peripheral.GrowLight, true));
            t.Start();
        }
        private void ButtonGrowLightOff_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() => peripheralControl.SetLight(Peripheral.GrowLight, false));
            t.Start();
        }
        private void ButtonBackLightOn_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() => peripheralControl.SetLight(Peripheral.Backlight, true));
            t.Start();
        }
        private void ButtonBackightOff_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() => peripheralControl.SetLight(Peripheral.Backlight, false));
            t.Start();
        }
        private void ButtonPeripheralConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                peripheralControl.Connect();
                toggleButtonVisibility(ButtonPeripheralConnect, ButtonPeripheralDisconnect, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonPeripheralDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                peripheralControl.Disconnect();
                toggleButtonVisibility(ButtonPeripheralConnect, ButtonPeripheralDisconnect, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
