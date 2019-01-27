using SPIPware.Entities;
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
        public void UpdateTimeLapseStatus(object sender, EventArgs e)
        {
            UpdateTimeLapseStatus(timelapse.runningTimeLapse);
        }
        public void UpdateTimeLapseStatus(bool timeLapseRunning)
        {
            Dispatcher.Invoke(() =>
            {//this refer to form in WPF application 

                timeLapseCount.Text = timelapse.tlCount;
                timeLapseEnd.Text = timelapse.tlEnd;
                timeLapseStatus.Text = parseStatus(timeLapseRunning);
                if (timeLapseRunning)
                {
                    //btnStopTimeLapse.IsEnabled = true;
                    btnRunTimeLapse.IsEnabled = false;
                    timeLapseStatusIcon.Source = GREEN_IMAGE;
                    if(timelapse.totalMinutes <= 1 && timelapse.totalMinutes != 0.0)
                    {
                        DisableMachineControlButtons();
                    }
                }
                else
                {
                    //btnStopTimeLapse.IsEnabled = false;
                    //btnRunTimeLapse.IsEnabled = true;
                    timeLapseCount.Text = parseStatus(timeLapseRunning);
                    timeLapseEnd.Text = timeLapseCount.Text;
                    EnableMachineControlButtons();
                    timeLapseStatusIcon.Source = YELLOW_IMAGE;
                }
                toggleButtonVisibility(btnRunTimeLapse, btnStopTimeLapse, timeLapseRunning);
            });
        }
        private static readonly KeyValuePair<long, string>[] intervalList = {
            //new KeyValuePair<long, string>(1000, "seconds(s)"),
            new KeyValuePair<long, string>(60000, "minute(s)"),
            new KeyValuePair<long, string>(3600000, "hour(s)"),
            new KeyValuePair<long, string>(86400000, "day(s)"),
            new KeyValuePair<long, string>(604800000, "week(s)")
        };
        public KeyValuePair<long, string>[] IntervalList
        {
            get
            {
                return intervalList;
            }
        }
        private void ButtonStartTimeLapse_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("Starting Timpelapse Cycle");
            cycle.UpdatePositionList(checkBoxes);
            SetNoSleep();
            timelapse.Start();

            //Task task = new Task(() => timelapse.Start());
            //task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //task.Start();
        }
        private void ButtonStopTimeLapse_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("Stopping Timpelapse Cycle");
            Task task = new Task(() => timelapse.Stop());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        //public void toggleTimeLapseButton(bool running)
        //{
        //    if (running)
        //    {
        //        btnRunTimeLapse.Visibility = Visibility.Collapsed;
        //        btnStopTimeLapse.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        btnRunTimeLapse.Visibility = Visibility.Visible;
        //        btnStopTimeLapse.Visibility = Visibility.Collapsed;
        //    }

        //}

    }
}
