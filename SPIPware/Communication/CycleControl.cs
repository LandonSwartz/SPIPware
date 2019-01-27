using log4net;
using System;
using System.Collections.Generic;
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

        public delegate void CycleUpdate();
        public event EventHandler StatusUpdate;

        

        CameraControl camera = CameraControl.Instance;
        Machine machine = Machine.Instance;
        PeripheralControl peripheral = PeripheralControl.Instance;

        private List<int> imagePositions = new List<int>();
        public bool runningCycle = false;
        public bool firstRun = true;
        private decimal targetLocation = 0;
        bool foundPlate = false;

    
        public List<int> ImagePositions { get => imagePositions; set => imagePositions = value; }


        private int posIndex = 0;

        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
            //if (runningCycle)
            //{

            //    HandleNextPosition(posIndex);
            //}
        }
        private bool IsNextIndex(int index)
        {
            return index+1 < imagePositions.Count;
        }
        public void UpdatePositionList(List<CheckBox> checkBoxes)
        {
            ImagePositions = new List<int>();
            for (var i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    ImagePositions.Add(i);
                }
            }
            //ImagePositions.ForEach((position) => Console.Write(position + ","));
            //Console.WriteLine();
        }
        //protected void OnStatusUpdateEvent(EventArgs e)
        //{
        //    StatusUpdate?.Invoke(this, e);
        //}
        public void Start()
        {
            if (machine.Connected && peripheral.Connected)
            {
                if (runningCycle) Stop();

                _log.Info("Starting Cycle");
                runningCycle = true;
              
                StatusUpdate.Raise(this, new EventArgs());
                

                camera.loadCameraSettings();
                posIndex = 0;
                //currentIndex = 0;
                
                //bool isPlateFound = FindCheckedBox(currentIndex, true);
                if (!ImagePositions.Any()) return;

                peripheral.SetLight(Peripheral.Backlight, true);
                peripheral.SetLight(Peripheral.GrowLight, false, false);
                peripheral.SetBacklightColor(Properties.Settings.Default.BacklightColor);
                if (firstRun)
                {
                    machine.SendLine("$H");
                    
                    //Properties.Settings.Default.CurrentPlate = 1;
                    firstRun = false;
                }
                else
                {
                    //Thread.Sleep(300);
                    targetLocation = machine.sendMotionCommand(imagePositions[posIndex]);
                }
            }
            //else
            //{
            //    Machine_NonFatalException("Machine not connected");
            //}


        }
        public void End()
        {
            _log.Debug("Ending Cycle.");
            runningCycle = false;
            StatusUpdate.Raise(this, EventArgs.Empty);

            //machine.sendMotionCommand(0);
            machine.SendLine("$H");
            peripheral.SetLight(Peripheral.Backlight, false);
            peripheral.SetLight(Peripheral.GrowLight, true, !peripheral.IsNightTime());
            posIndex = 0;
        }
        public void Stop()
        {
            End();
            if (machine.Connected)
            {
                machine.SoftReset();
            }
        }
        public delegate void ImageUpdated();
        public event EventHandler ImageUpdatedEvent;
        public BitmapImage bi;
        public void Check()
        {
            _log.Info("Checking cycle");
            if (runningCycle)
            {
                if (machine.Status == "Home")
                {
                    //cameraControl.home = true;
                    //cameraControl.firstRun = false;
                    //peripheral.SetLight(Peripheral.Backlight, true);
                    //Thread.Sleep(300);
                    targetLocation = machine.sendMotionCommand(imagePositions[posIndex]);
                }

                else if (machine.WorkPosition.X == (double)targetLocation && machine.Status == "Idle")
                {
                    _log.Debug("Current Index: " + imagePositions[posIndex]);
                    //peripheral.SetLight(Peripheral.Backlight, true);
                    bi =  camera.CapSaveImage().Clone();

                    bi.Freeze();

                    ImageUpdatedEvent.Raise(this, new EventArgs());

                    HandleNextPosition();
                }

            }
            else
            {
                return;
            }
        }
        public void HandleNextPosition()
        {
            foundPlate = IsNextIndex(posIndex);
            if (foundPlate)
            {
                _log.Debug("Found Next Plate: " + foundPlate);
                GoToNextPosition(posIndex++);
                _log.Debug("Next Position: " + posIndex);
            }
            else End();
        }
        public void GoToNextPosition(int index)
        {
            targetLocation = machine.sendMotionCommand(imagePositions[index]);
        }
    }
}
