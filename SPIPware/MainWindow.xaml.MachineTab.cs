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
            
            cb.Items.Clear();
            foreach (string port in System.IO.Ports.SerialPort.GetPortNames())
                cb.Items.Add(port);
            if(string.Equals(cb.Name, "PeripheralSerialPortSelect") &&
               !cb.Items.Contains(Properties.Settings.Default.PeripheralSP))
            {
                cb.SelectedItem = 0;
                cb.SelectedIndex = 0;
            }
            else if (string.Equals(cb.Name, "SerialPortSelect") && 
                !cb.Items.Contains(Properties.Settings.Default.P))
            {
                cb.SelectedItem = 0;
                cb.SelectedIndex = 0;
            }


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
                peripheralControl.Connect();
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
                peripheralControl.Disconnect();
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
            if (null != m_VimbaHelper)
            {
                try
                {
                    //Shutdown Vimba API when application exits
                    m_VimbaHelper.Shutdown();

                    m_VimbaHelper = null;
                }
                catch (Exception exception)
                {
                    cameraControl.LogError("Could not shutdown Vimba API. Reason: " + exception.Message);
                }
            }
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
            new Thread(() =>
            {
                Experiment.loadExperimentToSettings(Properties.Settings.Default.AcquisitionExperimentPath);
                startCycle();
            }).Start();
        }
    }
    }
