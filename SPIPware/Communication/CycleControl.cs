using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private bool cycleStatus = false;

        public List<int> ImagePositions { get => imagePositions; set => imagePositions = value; }
        public bool CycleStatus { get => cycleStatus; set => cycleStatus = value; }

        private int posIndex = 0;

        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
            if (runningCycle)
            {
                GoToNextPosition();
            }
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
            //if (machine.Connected && peripheral.Connected)
            {
                if (runningCycle) Stop();


                runningCycle = true;
                CycleStatus = runningCycle;
              
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
            runningCycle = false;
            CycleStatus = runningCycle;
            StatusUpdate.Raise(this, EventArgs.Empty);

            //currentIndex = 0;
            posIndex = 0;
            //machine.sendMotionCommand(0);
            machine.SendLine("$H");
            peripheral.SetLight(Peripheral.Backlight, false);
            peripheral.SetLight(Peripheral.GrowLight, true, !peripheral.IsNightTime());
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
                    //Console.WriteLine("Current Index" + currentIndex);
                    //peripheral.SetLight(Peripheral.Backlight, true);
                    bi =  camera.CapSaveImage().Clone();
                    
                    bi.Freeze();

                    ImageUpdatedEvent.Raise(this, new EventArgs());

                    //Console.WriteLine("Found Plate: " + foundPlate);
                    //foundPlate = FindCheckedBox(currentIndex, false);
                    //foundPlate = IsNextIndex(posIndex);
                    //if (foundPlate)
                    //{
                    //    posIndex++;
                    //    targetLocation = machine.sendMotionCommand(imagePositions[posIndex]);
                    //}
                    //else End();

                }

            }
            else
            {
                return;
            }
        }
        public void GoToNextPosition()
        {
           
            foundPlate = IsNextIndex(posIndex);
            if (foundPlate)
            {
                posIndex++;
                targetLocation = machine.sendMotionCommand(imagePositions[posIndex]);
            }
            else End();
        }
    }
}
