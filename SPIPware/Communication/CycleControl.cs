using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware.Communication
{
    class CycleControl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly CycleControl instance = new CycleControl();

        static CycleControl()
        {
        }
        public static CycleControl Instance
        {
            get
            {
                return instance;
            }
        }
        public List<int> ImagePositions { get => _imagePositions; set => _imagePositions = value; }

        public event EventHandler StatusUpdate;



        CameraControl camera = CameraControl.Instance;
        Machine machine = Machine.Instance;
        PeripheralControl peripheral = PeripheralControl.Instance;

        private List<int> _imagePositions = new List<int>();
        public bool runningCycle = false;
        private double targetLocation = 0;

        private static int posIndex = 0;

        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
            machine.StatusChanged += Machine_StatusChanged;
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
    
        }
        private bool IsCurrentIndex(int index)
        {
            _log.Debug("IsNextIndex Index: " + index);
            _log.Debug("IsNextIndex+ImagePositions.count " + ImagePositions.Count);
            return (index <= ImagePositions.Count && index < Properties.Settings.Default.NumLocations);
        }
        public void UpdatePositionList(List<CheckBox> checkBoxes)
        {
            ImagePositions.Clear();
            _log.Debug("Number of Checkboxes: " + checkBoxes.Count);
            //ImagePositions.ForEach((position) => _log.Debug(position + ","));
            for (var i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    ImagePositions.Add(i);
                }
            }
            //_imagePositions.ForEach(i => _log.Debug)
            _log.Debug("UpdatePositionList+ImagePositions.Count: " + ImagePositions.Count);
        }
        private void Machine_StatusChanged()
        {

            if (machine.Status == "Alarm")
            {
                Stop();
            }
            else if (machine.Status == "Home")
            {
                IsHome();
            }
            else if (machine.Status == "Idle")
            {
                IsIdle();
            }

        }
        public void Start()
        {
            if (machine.Connected && peripheral.Connected)
            {
                if (runningCycle)
                {
                    Stop();
                    _log.Error("Attempted to start a cycle while another was running");
                }


                _log.Info("Starting Cycle");
                runningCycle = true;

                StatusUpdate.Raise(this, new EventArgs());


                camera.loadCameraSettings();
                posIndex = 0;
           
                if (!ImagePositions.Any())
                {
                    _log.Error("No positions selected");
                    return;
                }
                ;

                peripheral.SetLight(Peripheral.Backlight, true);
                peripheral.SetLight(Peripheral.GrowLight, false, false);
                peripheral.SetBacklightColor(Properties.Settings.Default.BacklightColor);
                machine.SendLine("$H");
      
            }
            else
            {
                _log.Debug("Machine not connected");
            }


        }
        public void End()
        {
            _log.Debug("Ending Cycle.");
            Properties.Settings.Default.CycleCount++;
            runningCycle = false;
            posIndex = 0;
            targetLocation = 0;
            StatusUpdate.Raise(this, EventArgs.Empty);

            peripheral.SetLight(Peripheral.Backlight, false);
            peripheral.SetLight(Peripheral.GrowLight, true, !peripheral.IsNightTime());

            //machine.sendMotionCommand(0);
            _log.Debug("Sending machine home to end cycle");
            machine.SendLine("$H");


        }
        public void Stop()
        {
            _log.Error("Stop Occured");
            End();
            //if (machine.Connected)
            //{
            //    machine.SoftReset();
            //}
        }
        public delegate void ImageUpdated();
        public event EventHandler ImageUpdatedEvent;
        public BitmapImage bi;
        public void IsHome()
        {
            _log.Debug("Machine Home");
            if (runningCycle)
            {
                HandleNextPosition(posIndex);
                posIndex++;
            }

        }
        public void IsIdle()
        {
            _log.Debug("Machine Idle");
            if (runningCycle && machine.WorkPosition.X == targetLocation)
            {
                //peripheral.SetLight(Peripheral.Backlight, true);
                bi = camera.CapSaveImage().Clone();

                bi.Freeze();

                ImageUpdatedEvent.Raise(this, new EventArgs());

                
                HandleNextPosition(posIndex);
                posIndex++;

            }
        }
        public void HandleNextPosition(int index)
        {
            bool foundPlate = IsCurrentIndex(index);
            _log.Debug("Found Current Plate: " + foundPlate);
            if (foundPlate )
            {
                _log.Debug("local index: " + index);
                GoToPosition(index);
            }
            else End();
        }
        public void GoToPosition(int index)
        {
            _log.Debug("ImagePositions[index]: " + ImagePositions[index]);
            targetLocation = machine.sendMotionCommand(ImagePositions[index]);
            _log.Debug("Going to target location: " + targetLocation);
        }
    }
}
