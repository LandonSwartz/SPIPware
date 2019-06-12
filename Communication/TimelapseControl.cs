using log4net;
using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware.Communication
{
    class TimelapseControl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource tokenSource;
        PeripheralControl peripheral = PeripheralControl.Instance;
        CycleControl cycle = CycleControl.Instance;
        Machine machine = Machine.Instance;
        
        public bool runningTimeLapse = false;
        public bool runningSingleCycle = false;

        public string tlEnd;
        public string tlCount;
        public double totalMinutes;
        private Experiment tempExperiment;
        private Experiment timeLapseExperiment;
        private bool growLightsOn = false;

        public delegate void TimeLapseUpdate();
        public event EventHandler TimeLapseStatus;

        public delegate void ExperimentUpdate();
        public event EventHandler ExperimentStatus;

        public TimelapseControl()
        {
            machine.StatusChanged += Machine_StatusChanged;
            cycle.StatusUpdate += CycleStatusUpdated;
        }
        public void Start()
        {
            growLightsOn = false;
            _log.Info("Timelapse Starting");
      
            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(Properties.Settings.Default.tlInterval * Properties.Settings.Default.tlIntervalType);
            _log.Debug("Calculated Milliseconds: "+ Properties.Settings.Default.tlInterval * Properties.Settings.Default.tlIntervalType);
            _log.Debug("Actual Milliseconds: " +timeLapseInterval.TotalSeconds);

            if (Properties.Settings.Default.StartNow)
            {
                Properties.Settings.Default.tlStartDate = DateTime.Now;
            }
            double endTime = Properties.Settings.Default.tlEndIntervalType * Properties.Settings.Default.tlEndInterval;
            DateTime endDate = Properties.Settings.Default.tlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            tlCount = Properties.Settings.Default.tlStartDate.ToString();
            TimeLapseStatus.Raise(this, new EventArgs());
            HandleTimelapseCalculations(timeLapseInterval, endTime);
            //timeLapseCount.Text = "Not Running";


        }
        private void Machine_StatusChanged()
        {

            if (machine.Status == "Alarm")
            {
                //("Timelapse canclled because hard limit encountered");
                _log.Error("Machine in Alarm State. Cancelling Timelapse");
                Stop();
            }
       
        }
        async Task WaitForStartNow()
        {
            await Task.Delay(5000);
        }
        async Task RunSingleTimeLapse(TimeSpan duration, CancellationToken token)
        {
            _log.Debug("Awaiting timelapse");
            while (duration.TotalSeconds > 0)
            {
                totalMinutes = duration.TotalMinutes;
                tlCount = duration.TotalMinutes.ToString() + " minute(s)";
                TimeLapseStatus.Raise(this, new EventArgs());
                if (!cycle.runningCycle)
                {
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
                }
                //_log.Debug("Waiting 1 Minute");
                await Task.Delay(60 * 1000, token);
                //_log.Debug("1 minute elapsed");
                duration = duration.Subtract(TimeSpan.FromMinutes(1));
            }

        }
        public void CycleStatusUpdated(object sender, EventArgs e)
        {
            if(!cycle.runningCycle && runningSingleCycle)
            {
                runningSingleCycle = false;
                if(timeLapseExperiment != null && cycle.runningCycle == false)
                {
                    timeLapseExperiment.SaveExperiment(Properties.Settings.Default.tlExperimentPath);
                }
                tempExperiment.SaveExperimentToSettings();
                ExperimentStatus.Raise(this, new EventArgs());
            }
        }

        public async void HandleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        {

            if (((Properties.Settings.Default.StartNow || Properties.Settings.Default.tlStartDate <= DateTime.Now))
             && endDuration > 0)
            {
                _log.Info("Running single timelapse cycle");
                tokenSource = new CancellationTokenSource();

                tempExperiment = new Experiment();
                tempExperiment.LoadExperiment();

                
                timeLapseExperiment  = Experiment.LoadExperimentAndSave(Properties.Settings.Default.tlExperimentPath);
                ExperimentStatus.Raise(this, new EventArgs());
                runningSingleCycle = true;
                _log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);
                cycle.Start();

                try
                {
                    await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    _log.Error("TimeLapse Cancelled: " + e);
                    Stop();
                    TimeLapseStatus.Raise(this, new EventArgs());
                    return;
                }
                catch (Exception e)
                {
                    _log.Error("Unknown timelapse error: "+ e);
                }

                HandleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
            }
            else if (Properties.Settings.Default.tlStartDate > DateTime.Now)
            {
                await WaitForStartNow();
                HandleTimelapseCalculations(timeLapseInterval, endDuration);
            }
            else
            {
                _log.Info("TimeLapse Finished");
                runningTimeLapse = false;
                TimeLapseStatus.Raise(this, new EventArgs());
                return;
            }

        }
        public void Stop()
        {
          
            cycle.Stop();
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            growLightsOn = true;
            runningTimeLapse = false;
            TimeLapseStatus.Raise(this, new EventArgs());

        }
    }
}
