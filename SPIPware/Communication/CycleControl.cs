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
        private int[] _imagePositionsX = new int[Properties.Settings.Default.TotalColumns + 1]; //changed to 2D
        public int[] ImagePositionsX { get => _imagePositionsX; set => _imagePositionsX = value; }

        private int[] _imagePositionsY = new int[Properties.Settings.Default.TotalRows + 1];
        public int[] ImagePositionsY { get => _imagePositionsY; set => _imagePositionsY = value; }

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
            _log.Debug("IsNextIndex+ImagePositionsX[index] " + ImagePositionsX[index]); //because columns is x axis MAY NEED TO CHANGE
            return (index <= ImagePositionsX[index] && index < Properties.Settings.Default.TotalColumns);
        }

        private bool IsCurrentIndexY(int index)
        {
            _log.Debug("IsNextIndex IndexY: " + index);
            _log.Debug("IsNextIndex+ImagePositionsY[index] " + ImagePositionsY[index]);
            return (index <= ImagePositionsY[index] && index < Properties.Settings.Default.TotalRows);
        }

        //NOT enabled
        private bool IsCurrentIndexZ(int index)
        {
            return false; //DO NOT USE
        }

        //removing because probs don't need
       /* public void UpdatePositionList(List<List<CheckBox>> checkBoxes)
        {
            Array.Clear(ImagePositions, 0, ImagePositions.Length); //remade into array.clear, hopefully will work
            _log.Debug("Number of Checkboxes: " + checkBoxes.Count);
            //ImagePositions.ForEach((position) => _log.Debug(position + ","));

            for (var i = 0; i < Properties.Settings.Default.TotalRows; i++)
            {
                for (var j = 0; j < Properties.Settings.Default.TotalColumns; j++)
                {
                    if (checkBoxes[i][j].IsChecked == true)
                    {
                        ImagePositions[i, j] = i + j;
                    }
                }
            }
            //_imagePositions.ForEach(i => _log.Debug)
            _log.Debug("UpdatePositionList+ImagePositions.Count: " + ImagePositions.Length);
        }*/

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

        public bool rowCompleted = false;

        #region Cycle Status Functions
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
                rowCompleted = false; //hasn't complete a row yet so false 
                //posIndexZ = 0;

                for(int i = 0; i < ImagePositionsX.Length; i++)
                {
                    ImagePositionsX[i] = i;
                }
                for(int i = 0; i <ImagePositionsY.Length; i++)
                {
                    ImagePositionsY[i] = i;
                }


                //removed 7/9/2019 for testing of two axis cycle control and redunant now because of phased out image position checkboxes
                /*  if (!ImagePositions.Any())
                  {
                      _log.Error("No positions selected");
                      return;
                  } */
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
            rowCompleted = false;
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

            
            _log.Debug("Row Complete Value is " + rowCompleted);
            //first run
            if (runningCycle && rowCompleted != true)
            {
              //  rowCompleted = false;
                _log.Debug("Position Index X is " + posIndexX);
                _log.Debug("Position Index Y is " + posIndexY);

               HandleNextPositionY(posIndexY); //move to next y-axis then move x then continue with x axis
               posIndexY++;
               HandleNextPositionX(posIndexX);
               posIndexX++;
               
            }
            else if (runningCycle && rowCompleted) //next row shiz
            {
                //resetting x values
                posIndexX = 0;
                targetLocationX = 0;
                for (int i = 0; i < ImagePositionsX.Length; i++)
                {
                    ImagePositionsX[i] = i;
                }

                HandleNextPositionX(posIndexX);
                posIndexX++;

                System.Threading.Thread.Sleep(5000); //wait five seconds

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
            }
        }
        #endregion


        #region HandleNextPosition and GoToPosition Functions(x,y,z)

        //x-axis
        public void HandleNextPositionX(int index)
        {
            bool foundPlate = IsCurrentIndexX(index);
            _log.Debug("Found Current Plate X: " + foundPlate);
            if (foundPlate )
            {
                _log.Debug("local index X: " + index);
                GoToPositionX(index);
                rowCompleted = true;
            }
            else IsHome(); //changed from end() to isHome() to try to make 2D
        }

        public void GoToPositionX(int index)
        { //image position is [0,index] because columns is second value which is x axis
            _log.Debug("ImagePositionsX[indexX]: " + ImagePositionsX[index]);
            targetLocationX = machine.sendMotionCommandX(ImagePositionsX[index]); 
            _log.Debug("Going to target  X: " + targetLocationX);
        }

        //y-axis
        public void HandleNextPositionY(int index)
        {
            bool foundPlate = IsCurrentIndexY(index);
            _log.Debug("Found Current Plate Y: " + foundPlate);
            if (foundPlate)
            {
                _log.Debug("local index Y: " + index);
                GoToPositionY(index);
            }
            else
            {
                End();
            }
        }
        public void GoToPositionY(int indexY)
        {
            //moving by rows
            _log.Debug("ImagePositionsY[IndexY]: " + ImagePositionsY[indexY]);
            targetLocationY = machine.sendMotionCommandY(ImagePositionsY[indexY]);
            _log.Debug("Going to target location Y: " + targetLocationY);
        }

        //z-axis NOT ENABLED
        //Will have to update image positions to a triple array to accomdate
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
