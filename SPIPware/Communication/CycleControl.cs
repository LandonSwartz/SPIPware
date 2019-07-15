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
    /// <summary>
    /// Class for the controlling of the cycling of the machine during a timelapse or single cycle run
    /// </summary>
    class CycleControl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly CycleControl instance = new CycleControl();

        #region Constructor and Instance Function
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
        /// <summary>
        /// Function to check for Image acquired event and changes machine status
        /// </summary>
        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
            machine.StatusChanged += Machine_StatusChanged;
        }
        #endregion

        #region Properties


        private int[] _imagePositionsX; // = new int[Properties.Settings.Default.TotalColumns + 1]; //image positions of x axis
        public int[] ImagePositionsX { get => _imagePositionsX; set => _imagePositionsX = value; }

        private int[] _imagePositionsY; // = new int[Properties.Settings.Default.TotalRows + 1]; //image position of y-axis
        public int[] ImagePositionsY { get => _imagePositionsY; set => _imagePositionsY = value; }

        public event EventHandler StatusUpdate;



        CameraControl camera = CameraControl.Instance;
        Machine machine = Machine.Instance;
        PeripheralControl peripheral = PeripheralControl.Instance;

        
        public bool runningCycle = false;

        //target locations for next position in cycle
        private double targetLocationX = 0;
        private double targetLocationY = 0;
   /*    private double targetLocationZ = 0; TO USE LATER IF NEEDED */

        //position index for each axis
        private static int posIndexX = 0;
        private static int posIndexY = 0; //y position index
        /*  private static int posIndexZ = 0; TO USE LATER IF NEEDED  */
        #endregion

        public void OnImageAcquired(object sender, EventArgs e)
        {
    
        }


        #region IsCurrentIndex Functions (x,y,z)
        private bool IsCurrentIndexX(int index)
        {
            _log.Debug("IsNextIndex IndexX: " + index);
            _log.Debug("IsNextIndex+ImagePositionsX[index] " + ImagePositionsX[index]); //because columns is x axis MAY NEED TO CHANGE
            return (index <= ImagePositionsX[index] && index < Properties.Settings.Default.TotalColumns);
        }

        private bool IsCurrentIndexY(int indexY)
        {
            _log.Debug("IsNextIndex IndexY: " + indexY);
            _log.Debug("IsNextIndex+ImagePositionsY[index] " + ImagePositionsY[indexY]);
            return (indexY <= ImagePositionsY[indexY] && indexY < Properties.Settings.Default.TotalRows);
        }

        //NOT enabled
        private bool IsCurrentIndexZ(int index)
        {
            return false; //DO NOT USE
        }
        #endregion

        //Removed 7/8/2019 becuase redudnat with new bugbear updates
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

        //Machine Status Change method
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

        public bool rowCompleted = false; //for testing of cycle control with new y-axis

        #region Cycle Status Functions
        /// <summary>
        /// Start() is what is called when starting a cycle. Intializes properties to zero and begins
        /// log of data
        /// </summary>
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

                ImagePositionsX = new int[Properties.Settings.Default.TotalColumns + 1];
                ImagePositionsY = new int[Properties.Settings.Default.TotalRows + 1] ;

                for(int i = 0; i < ImagePositionsX.Length; i++)
                {
                    ImagePositionsX[i] = i;
                }
                for(int i = 0; i < ImagePositionsY.Length; i++)
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

                machine.SendLine("$H"); //forces machine.status to "HOME"               
      
            }
            else
            {
                _log.Debug("Machine not connected");
            }


        }
        /// <summary>
        /// End() called at end of cycle to reset properties and tie up loose cycle ends.
        /// </summary>
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
        /// <summary>
        /// Stop() stops the currently running cycle and ends the cycle.
        /// </summary>
        public void Stop()
        {
            _log.Error("Stop Occured");
            End();
            //if (machine.Connected)
            //{
            //    machine.SoftReset();
            //}
        }

        //image properties for cycle 
        public delegate void ImageUpdated();
        public event EventHandler ImageUpdatedEvent;
        public BitmapImage bi;

        /// <summary>
        /// IsHome() is called when machine.status is home after a $H call. It is split into two main parts.
        /// The first is about what to do during the first start of the entire cycle. The second
        /// is about when moving to the next row for a new X-axis cycle.
        /// </summary>
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
            
        }
        //for next row
        public void IsHomeRow()
        {
            if (runningCycle && rowCompleted) //next row shiz
            {
                //resetting x values
                posIndexX = 0;
                targetLocationX = 0;
                rowCompleted = false;
                // machine.Status = "Home";

                GoToPositionX(posIndexX); //go to home position

                System.Threading.Thread.Sleep(3000); //wait five seconds

                HandleNextPositionY(posIndexY); 
                posIndexY++;
                //posIndexX = 1; FOR TESTING
                
            }
        }

        /// <summary>
        /// IsIdle() is called when machine is idle during a run and captures a picture before moving on to new position.
        /// </summary>
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

                if(rowCompleted == true)
                {
                    posIndexX++;
                }
                
            }
        }
        #endregion


        #region HandleNextPosition and GoToPosition Functions(x,y,z)
        /// <summary>
        /// These Functions find the next position for their respective axis then goes to it.
        /// </summary>
        /// <param name="index">Each axis has own index based off of their respective machine position index</param>

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
            else IsHomeRow(); //changed from end() to isHome() to try to make 2D, 7/8/2019
        }

        public void GoToPositionX(int index)
        {
            _log.Debug("ImagePositionsX[indexX]: " + ImagePositionsX[index]);
            targetLocationX = machine.sendMotionCommandX(ImagePositionsX[index]); 
            _log.Debug("Going to target  X: " + targetLocationX);
        }

        //y-axis
        public void HandleNextPositionY(int indexY)
        {
            bool foundPlate = IsCurrentIndexY(indexY);
            _log.Debug("Found Current Plate Y: " + foundPlate);
            if (foundPlate)
            {
                _log.Debug("local index Y: " + indexY);
                GoToPositionY(indexY);
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
