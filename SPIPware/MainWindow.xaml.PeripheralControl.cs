﻿using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware
{
    partial class MainWindow
    {
        PeripheralControl peripheral = Instance;
        private void ButtonGrowLightOn_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => peripheral.SetLight(Peripheral.GrowLight, true));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        private void ButtonGrowLightOff_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => peripheral.SetLight(Peripheral.GrowLight, false));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        private void ButtonBackLightOn_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => peripheral.SetLight(Peripheral.Backlight, true));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        private void ButtonBackightOff_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => peripheral.SetLight(Peripheral.Backlight, false));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        public void UpdatePeripheralStatus(object sender, EventArgs e)
        {
            PeripheralEventArgs p = (PeripheralEventArgs) e;
            if(p.Periperal == Peripheral.Backlight)
            {
                Properties.Settings.Default.BacklightStatus = p.Status;
            }
            else if (p.Periperal == Peripheral.GrowLight)
            {
                Properties.Settings.Default.GrowlightStatus = p.Status;
            }
        }
        public void UpdateClrCanvas()
        {
            Dispatcher.Invoke(()=> ClrCanvas.SelectedColor = Properties.Settings.Default.BacklightColor);
            
        }
        private void Button_UpdateBacklightColor(object sender, RoutedEventArgs e)
        {
            UpdateBacklightColor();
        } 
        private void UpdateBacklightColor()
        {
            peripheral.SetBacklightColor(Properties.Settings.Default.BacklightColor);
        }
        private void ClrCanvas_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            //TextBox.Text = "#" + ClrPcker_Background.SelectedColor.R.ToString() + ClrPcker_Background.SelectedColor.G.ToString() + ClrPcker_Background.SelectedColor.B.ToString();
            Properties.Settings.Default.BacklightColor = ClrCanvas.SelectedColor.GetValueOrDefault();
            //_log.Debug(Properties.Settings.Default.BacklightColor);
        }
        private void ButtonPeripheralConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                peripheral.Connect();
                toggleButtonVisibility(ButtonPeripheralConnect, ButtonPeripheralDisconnect, true);
            }
            catch (Exception ex)
            {
                _log.Error("Unable to connect" + ex);
            }
        }

        private void ButtonPeripheralDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                peripheral.Disconnect();
                toggleButtonVisibility(ButtonPeripheralConnect, ButtonPeripheralDisconnect, false);
            }
            catch (Exception ex)
            {
                _log.Error("Unable to disconnect" + ex);
            }
        }

    }
}
