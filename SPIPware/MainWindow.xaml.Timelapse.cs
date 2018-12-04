using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware
{
    partial class MainWindow
    {
        private CancellationTokenSource tokenSource;

        public static bool runningTimeLapse = false;
        bool growLightsOn = false;

        public  void startTimeLapse()
        {
            btnRunTimeLapse.IsEnabled = false;
            Console.WriteLine("Timelapse Starting");
            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(Properties.Settings.Default.tlInterval * Properties.Settings.Default.tlIntervalType);
            //Console.WriteLine(Properties.Settings.Default.tlInterval * Properties.Settings.Default.tlIntervalType);
            //Console.WriteLine(timeLapseInterval.Seconds);
            updateTimeLapseStatus(runningTimeLapse);
            if (Properties.Settings.Default.StartNow)
            {
                Properties.Settings.Default.tlStartDate = DateTime.Now;
            }
            double endTime = Properties.Settings.Default.tlEndIntervalType * Properties.Settings.Default.tlEndInterval;
            DateTime endDate = Properties.Settings.Default.tlStartDate.AddMilliseconds(endTime);
            timeLapseEnd.Text = endDate.ToString();

            timeLapseCount.Text = Properties.Settings.Default.tlStartDate.ToString();
            handleTimelapseCalculations(timeLapseInterval, endTime);
            //timeLapseCount.Text = "Not Running";


        }
       
        async Task waitForStartNow()
        {
            await Task.Delay(5000);
        }
        async Task runSingleTimeLapse(TimeSpan duration, CancellationToken token)
        {

            while (duration.TotalSeconds > 0)
            {
                timeLapseCount.Text = duration.TotalMinutes.ToString() + " minute(s)";
                if (duration.TotalMinutes <= 1)
                {
                    DisableMachineControlButtons();
                }
                if (!peripheral.IsNightTime() && !growLightsOn)
                {
                    peripheral.SetLight(Peripheral.GrowLight, true, true);
                    growLightsOn = true;
                }
                else if (peripheral.IsNightTime() && growLightsOn)
                {
                    peripheral.SetLight(Peripheral.GrowLight, false, false);
                    growLightsOn = false;
                }
                await Task.Delay(60 * 1000, token);
                duration = duration.Subtract(TimeSpan.FromMinutes(1));
            }

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
        public async void handleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        {

            if ((Properties.Settings.Default.StartNow || Properties.Settings.Default.tlStartDate <= DateTime.Now)
             && endDuration > 0)
            {
                tokenSource = new CancellationTokenSource();
                Experiment.loadExperimentToSettings(Properties.Settings.Default.tlExperimentPath);
                peripheral.SetLight(Peripheral.Backlight, true);
                Thread.Sleep(300);
                StartCycle();
                try
                {
                    await runSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine("TimeLapse Cancelled: " + e);
                    //runningTimeLapse = false;
                    stopTimeLapse();
                    updateTimeLapseStatus(runningTimeLapse);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine("TimeLapse Executed at: " + DateTime.Now);

                handleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
            }
            else if (Properties.Settings.Default.tlStartDate > DateTime.Now)
            {
                await waitForStartNow();
                handleTimelapseCalculations(timeLapseInterval, endDuration);
            }
            else
            {
                Console.WriteLine("TimeLapse Finished");
                runningTimeLapse = false;
                updateTimeLapseStatus(runningTimeLapse);
                return;
            }

        }
        public void updateTimeLapseStatus(bool timeLapseRunning)
        {
            timeLapseStatus.Text = parseStatus(timeLapseRunning);
            if (timeLapseRunning)
            {
                //btnStopTimeLapse.IsEnabled = true;
                //btnRunTimeLapse.IsEnabled = false;
                timeLapseStatusIcon.Source = GREEN_IMAGE;
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
        private void stopTimeLapse()
        {
            StopCycle();
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            runningTimeLapse = false;
            updateTimeLapseStatus(runningTimeLapse);

        }
    }
}
