using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPIPware
{
    partial class MainWindow
    {
        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Communication.Machine.OperatingMode.Disconnected)
                return;

            new SettingsWindow().ShowDialog();
        }
        private void updateSerialPortComboBox(ComboBox cb)
        {
            string sp = Properties.Settings.Default.P;
            string psp = Properties.Settings.Default.PeripheralSP;
            cb.Items.Clear();
            String[] ports = System.IO.Ports.SerialPort.GetPortNames();
            int i = 0;
            int selectedIndex = 0;
           
            foreach (string port in ports )
            {
                if (string.Equals(cb.Name, "PeripheralSerialPortSelect") && string.Equals(port, psp)){
                        selectedIndex = i;
                }
                if (string.Equals(cb.Name, "SerialPortSelect") && string.Equals(port, sp))
                {
                    selectedIndex = i;
                }
               
                cb.Items.Add(port);
                i++;
            }
            cb.SelectedIndex = selectedIndex;

            
            //if (string.Equals(cb.Name, "PeripheralSerialPortSelect")){
            //    var match = ports.FirstOrDefault(stringToCheck => stringToCheck.Contains(Properties.Settings.Default.PeripheralSP));
            //    if (match == null)
            //    {
            //        //cb.SelectedItem = 0;
            //        cb.SelectedIndex = 0;
            //    }
            //    else
            //    {
            //        cb.SelectedItem = Properties.Settings.Default.PeripheralSP;
            //    }
            //}
            //if (string.Equals(cb.Name, "SerialPortSelect"))
            //{
            //    var match = ports.FirstOrDefault(stringToCheck => stringToCheck.Contains(Properties.Settings.Default.P));
            //    if (match == null)
            //    {
            //        //cb.SelectedItem = 0;
            //        cb.SelectedIndex = 0;
            //    }
            //    else
            //    {
            //        cb.SelectedItem = Properties.Settings.Default.P;
            //    }
            //}
        }

        private void cbPeripheralSerialOpen(object sender, EventArgs e)
        {
            updateSerialPortComboBox(PeripheralSerialPortSelect);

        }
        private void cbSerialOpen(object sender, EventArgs e)
        {
            updateSerialPortComboBox(SerialPortSelect);

        }
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                machine.Connect();
                peripheral.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                machine.Disconnect();
                peripheral.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (machine.Connected)
            {
                machine.Disconnect();
                //MessageBox.Show("Can't close while connected!");
                e.Cancel = true;
                //return;
            }

            settingsWindow.Close();
            camera.ShutdownVimba();
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void ButtonSyncBuffer_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Communication.Machine.OperatingMode.Manual)
                return;

            machine.SyncBuffer = true;
        }

        private void ShowGrblSettings_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Communication.Machine.OperatingMode.Manual)
                return;

            machine.SendLine("$$");
            settingsWindow.ShowDialog();
        }
        private void ButtonStartCycle_Click(object sender, RoutedEventArgs e)
        {
                //Experiment.LoadExperimentToSettings(Properties.Settings.Default.AcquisitionExperimentPath);
                StartCycle();
        }
    }
    }
