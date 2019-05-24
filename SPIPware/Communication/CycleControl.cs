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

/*                                          *
 *  SPIPware.Communication.CycleControl.cs  *
 *                                          *
 *  This is the main class for the cycle    *
 *  control of the machine. It contains the *
 *  functions and methods for moving the    *
 *  machine automatically.                  *
 *                                          */

/// <summary>
/// This is the main class for the cycle
/// control of the machine.It contains the
/// functions and methods for moving the
/// machine automatically.
/// </summary>

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
        /// <summary>
        /// A list ADT of the image positions for the cycle control
        /// </summary>
        public List<int> ImagePositions { get => _imagePositions; set => _imagePositions = value; }

        public event EventHandler StatusUpdate;



        CameraControl camera = CameraControl.Instance;
        Machine machine = Machine.Instance;
        PeripheralControl peripheral = PeripheralControl.Instance;

        private List<int> _imagePositions = new List<int>();
        public bool runningCycle = false;
        private double targetLocation = 0;

        /// <summary>
        /// Position Index of Camera, starts at 0
        /// </summary>
        private static int posIndex = 0;

        CycleControl()
        {
            camera.ImageAcquiredEvent += OnImageAcquired;
            machine.StatusChanged += Machine_StatusChanged;
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
    
        }

        /// <summary>
        /// IsCurrentIndex() checks whether at correct/current index or not then returns
        /// a boolean truth/false
        /// </summary>
        /// <param name="index">The position index of the camera on its cycle</param>
        /// <returns>Boolean truth or false if at current index or not</returns>
        private bool IsCurrentIndex(int index)
        {
            _log.Debug("IsNextIndex Index: " + index);
            _log.Debug("IsNextIndex+ImagePositions.count " + ImagePositions.Count);
            return (index <= ImagePositions.Count && index < Properties.Settings.Default.NumLocations);
        }

        /// <summary>
        /// UpdatePositionList updates the Position list on the left side menu of SPIPware.
        /// It depends on the number of checked boxes in the list <c>CheckBox</c>.
        /// </summary>
        /// <param name="checkBoxes">The List of Checked Boxes on the left menu</param>
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

        /// <summary>
        /// Machine_StatusChanged() checks whether <c>Machine.Status</c> has changed then does
        /// correspnding functions depending on found status
        /// </summary>
        /// <list>
        /// <item>
        /// <term>Alarm</term>
        /// <description>Machine has encountered an alarm error</description>
        /// </item>
        /// <item>
        /// <term>Home</term>
        /// <description>The machine is at home position</description>
        /// </item>
        /// <item>Idle</item>
        /// <description>The machine is idle, waiting for next command in cycle</description>
        /// </list>
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
        /// <summary>
        /// Start() starts a cycle for the machine
        /// </summary>
        /// <remark>First the function checks if both the machine ardunio
        /// and the peripheral ardunio is connected. Then updates status.
        /// Tells the peripherals to set the lighting. Finishes with homing
        /// the machine</remark>
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
        /// <summary>
        /// End() declares what to do at the end of a cycle
        /// </summary>
        /// <remarks>First logs ending the cycle. The function
        /// then increaes the cycle count of <c>Properties.Settings.Default.CycleCount</c>
        /// It updates the running cycle variables for the class. The index is reset, as is the target location
        /// Peripheral lights are returned to the respective original status. Machine homes a final time</remarks>
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
        /// <summary>
        /// Stop() is a handler function for End(). Logs that a stop occured.
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
        public delegate void ImageUpdated();
        public event EventHandler ImageUpdatedEvent;
        public BitmapImage bi;

        /// <summary>
        /// IsHome() determines what to do when <c>Machine.Status == "Home"</c>.
        /// </summary>
        /// <remark>This function could be updated for the y-axis update</remark>
        public void IsHome()
        {
            _log.Debug("Machine Home");
            if (runningCycle)
            {
                HandleNextPosition(posIndex);
                posIndex++;
            }

        }
        /// <summary>
        /// IsIdle() determines what to do when <c>Machine.Status == "Idle"</c>
        /// </summary>
        /// <remarks>The function coudl be important for y-axis updates</remarks>
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
        /// <summary>
        /// HandleNextPosition() is the handler function for GoToPosition(). Continues the cycle.
        /// </summary>
        /// <param name="index">Index is the current position index (<c>posIndex</c>)</param>
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
        /// <summary>
        /// GoToPosition() goes to the next image position of the cycle
        /// </summary>
        /// <param name="index">The index of the position that the function will go to</param>
        public void GoToPosition(int index)
        {
            _log.Debug("ImagePositions[index]: " + ImagePositions[index]);
            targetLocation = machine.sendMotionCommand(ImagePositions[index]);
            _log.Debug("Going to target location: " + targetLocation);
        }
    }
}
