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
        private List<List<int>> _imagePositions = new List<List<int>>(); //chnaged to 2D checkboxes
        public List<List<int> >ImagePositions { get => _imagePositions; set => _imagePositions = value; }

        public event EventHandler StatusUpdate;



        CameraControl camera = CameraControl.Instance;
        Machine machine = Machine.Instance;
        PeripheralControl peripheral = PeripheralControl.Instance;

        
        public bool runningCycle = false;

        //target locations for next position in cycle
        private double targetLocationX = 0;
        private double targetLocationY = 0;
   //     private double targetLocationZ = 0; TO USE LATER IF NEEDED

        //position index for each axis
        private static int posIndexX = 0;
        private static int posIndexY = 0; //y position index
       // private static int posIndexZ = 0; TO USE LATER IF NEEDED

        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
            machine.StatusChanged += Machine_StatusChanged;
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
    
        }
        //finds if currenet index (but only for x?)
        private bool IsCurrentIndexX(int index)
        {
            _log.Debug("IsNextIndex IndexX: " + index);
            _log.Debug("IsNextIndex+ImagePositionsX.count " + ImagePositions.Count);
            return (index <= ImagePositions.Count && index < Properties.Settings.Default.NumLocationsRow);
        }

        private bool IsCurrentIndexY(int index)
        {
            _log.Debug("IsNextIndex IndexX: " + index);
            _log.Debug("IsNextIndex+ImagePositionsX.count " + ImagePositions.Count);
            return (index <= ImagePositions.Count && index < Properties.Settings.Default.NumLocationsColumns);
        }

        //NOT enabled
        private bool IsCurrentIndexZ(int index)
        {
            return false; //DO NOT USE
        }

        public void UpdatePositionList(List<List<CheckBox>> checkBoxes)
        {
            ImagePositions.Clear();
            _log.Debug("Number of Checkboxes: " + checkBoxes.Count);
            //ImagePositions.ForEach((position) => _log.Debug(position + ","));

            for (var j = 0; j < checkBoxes.Count; j++)
            {
                for (var i = 0; i < checkBoxes[j].Count; i++)
                {
                    if (checkBoxes[j][j].IsChecked == true)
                    {
                        ImagePositions[j].Add(i);
                    }
                }
            }
            //_imagePositions.ForEach(i => _log.Debug)
            _log.Debug("UpdatePositionList+ImagePositions.Count: " + ImagePositions.Count);
        }

        //Machine Status Change function
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

        #region Machine Status Functions
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
                posIndexX = 0;
                posIndexY = 0;
           //     posIndexZ = 0;
           
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
            //reseting all the indexes and target location values
            posIndexX = 0;
            posIndexY = 0;
          //  posIndexZ = 0;
            targetLocationX = 0;
            targetLocationY = 0;
            //targetLocationZ = 0;
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
        {//currently only set up for x-axis
            _log.Debug("Machine Home");
            if (runningCycle)
            {
                    HandleNextPositionX(posIndexX);
                    posIndexX++;
                HandleNextPositionY(posIndexY);
                posIndexY++;
            }

        }

        public void IsIdle()
        {//only set up for x
            _log.Debug("Machine Idle");
            if (runningCycle && (machine.WorkPosition.X == targetLocationX))
            {
                //peripheral.SetLight(Peripheral.Backlight, true);
                bi = camera.CapSaveImage().Clone();

                bi.Freeze();

                ImageUpdatedEvent.Raise(this, new EventArgs());
                                
                HandleNextPositionX(posIndexX);
                
                posIndexX++;

                HandleNextPositionY(posIndexY);

                posIndexY++;

            }
        }
        #endregion

        #region HandleNextPosition and GoToPosition Functions(x,y,z)
        //x-axis
        public void HandleNextPositionX(int index)
        {
            bool foundPlate = IsCurrentIndexX(index);
            _log.Debug("Found Current Plate: " + foundPlate);
            if (foundPlate )
            {
                _log.Debug("local index: " + index);
                GoToPositionX(index);
            }
            else End();
        }
        public void GoToPositionX(int index)
        { //machine will go to x value and not move at y axis, that is why ImagePositions[index][0] has 0
            _log.Debug("ImagePositions[index]: " + ImagePositions[index][0]);
            targetLocationX = machine.sendMotionCommandX(ImagePositions[index][0]); 
            _log.Debug("Going to target location: " + targetLocationX);
        }

        //y-axis
        public void HandleNextPositionY(int index)
        {
            bool foundPlate = IsCurrentIndexY(index);
            _log.Debug("Found Current Plate: " + foundPlate);
            if (foundPlate)
            {
                _log.Debug("local index: " + index);
                GoToPositionY(index);
            }
            else End();
        }
        public void GoToPositionY(int indexY)
        {
            _log.Debug("ImagePositions[index]: " + ImagePositions[0][indexY]);
            targetLocationX = machine.sendMotionCommandX(ImagePositions[0][indexY]);
            _log.Debug("Going to target location: " + targetLocationY);
        }

        //z-axis NOT ENABLED
        /*
        public void HandleNextPositionZ(int index)
        {
            bool foundPlate = IsCurrentIndexZ(index);
            _log.Debug("Found Current Plate: " + foundPlate);
            if (foundPlate)
            {
                _log.Debug("local index: " + index);
                GoToPositionZ(index);
            }
            else End();
        }
        public void GoToPositionZ(int index)
        {
            _log.Debug("ImagePositions[index]: " + ImagePositions[index]);
            targetLocationX = machine.sendMotionCommandX(ImagePositions[index]);
            _log.Debug("Going to target location: " + targetLocationZ);
        }*/
        #endregion
    }
}
