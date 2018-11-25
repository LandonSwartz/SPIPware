using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SPIPware
{
    partial class MainWindow
    {
        PeripheralControl peripheralControl = new PeripheralControl();
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
