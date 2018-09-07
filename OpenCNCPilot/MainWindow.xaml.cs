using System;
using System.Windows;
using OpenCNCPilot.Communication;
using OpenCNCPilot.Util;
using Microsoft.Win32;
using OpenCNCPilot.GCode;

using System.ComponentModel;
using System.Windows.Controls.Ribbon;
using System.Windows.Controls;
using System.Collections.Generic;
using SynchronousGrab;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace OpenCNCPilot
{
    public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
    {
        Machine machine = new Machine();

        OpenFileDialog openFileDialogGCode = new OpenFileDialog() { Filter = Constants.FileFilterGCode };
        SaveFileDialog saveFileDialogGCode = new SaveFileDialog() { Filter = Constants.FileFilterGCode };
        OpenFileDialog openFileDialogHeightMap = new OpenFileDialog() { Filter = Constants.FileFilterHeightMap };
        SaveFileDialog saveFileDialogHeightMap = new SaveFileDialog() { Filter = Constants.FileFilterHeightMap };

        GCodeFile ToolPath { get; set; } = GCodeFile.Empty;
        //HeightMap Map { get; set; }

        private decimal targetLocation = 0;
        private int currentIndex = 0;
        public static bool runningCycle = false;
        public static bool runningTimeLapse = false;


        private bool goToFirstPlate = true;
        private bool firstRun = true;


        private CancellationTokenSource tokenSource;

        private static VimbaHelper m_VimbaHelper = null;
        GrblSettingsWindow settingsWindow = new GrblSettingsWindow();
        private CameraControl cameraControl = new CameraControl();
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            InitializeComponent();
            updateSerialPortComboBox();

            Properties.Settings.Default.tlStartDate = DateTime.Now;
            Properties.Settings.Default.tlEndDate = DateTime.Now.AddHours(1);

        


            //SerialPortSelect.SelectedValue = port[0];
            openFileDialogGCode.FileOk += OpenFileDialogGCode_FileOk;
            saveFileDialogGCode.FileOk += SaveFileDialogGCode_FileOk;
            //openFileDialogHeightMap.FileOk += OpenFileDialogHeightMap_FileOk;
            //saveFileDialogHeightMap.FileOk += SaveFileDialogHeightMap_FileOk;

            machine.ConnectionStateChanged += Machine_ConnectionStateChanged;

            machine.NonFatalException += Machine_NonFatalException;
            machine.Info += Machine_Info;
            machine.LineReceived += Machine_LineReceived;
            machine.LineReceived += settingsWindow.LineReceived;
            machine.StatusReceived += Machine_StatusReceived;
            machine.LineSent += Machine_LineSent;

            machine.PositionUpdateReceived += Machine_PositionUpdateReceived;
            machine.StatusChanged += Machine_StatusChanged;
            machine.DistanceModeChanged += Machine_DistanceModeChanged;
            machine.UnitChanged += Machine_UnitChanged;
            machine.PlaneChanged += Machine_PlaneChanged;
            machine.BufferStateChanged += Machine_BufferStateChanged;
            machine.OperatingModeChanged += Machine_OperatingMode_Changed;
            machine.FileChanged += Machine_FileChanged;
            //machine.FilePositionChanged += Machine_FilePositionChanged;
            //machine.ProbeFinished += Machine_ProbeFinished;
            machine.OverrideChanged += Machine_OverrideChanged;
            machine.PinStateChanged += Machine_PinStateChanged;

            Machine_OperatingMode_Changed();
            Machine_PositionUpdateReceived();

            Properties.Settings.Default.SettingChanging += Default_SettingChanging;
            FileRuntimeTimer.Tick += FileRuntimeTimer_Tick;

            machine.ProbeFinished += Machine_ProbeFinished_UserOutput;

            LoadMacros();

            settingsWindow.SendLine += machine.SendLine;

            CheckBoxUseExpressions_Changed(null, null);
            updatePlateCheckboxes();
            UpdateCheck.CheckForUpdate();
            //cameraControl.m_CameraList = m_CameraList;
            cameraControl.m_PictureBox = m_PictureBox;
            updateCameraSettingsOptions();
            try
            {
                //Start up Vimba API
                VimbaHelper vimbaHelper = new VimbaHelper();
                vimbaHelper.Startup(cameraControl.OnCameraListChanged);
                //Text += String.Format(" Vimba .NET API Version {0}", vimbaHelper.GetVersion());
                m_VimbaHelper = vimbaHelper;
                cameraControl.VimbaHelper = m_VimbaHelper;


                try
                {
                    cameraControl.UpdateCameraList();

                }
                catch (Exception exception)
                {
                    cameraControl.LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
            catch (Exception exception)
            {
                cameraControl.LogError("Could not startup Vimba API. Reason: " + exception.Message);
            }

        }

        public Vector3 LastProbePosMachine { get; set; }
        public Vector3 LastProbePosWork { get; set; }

        private void Machine_ProbeFinished_UserOutput(Vector3 position, bool success)
        {
            LastProbePosMachine = machine.LastProbePosMachine;
            LastProbePosWork = machine.LastProbePosWork;

            RaisePropertyChanged("LastProbePosMachine");
            RaisePropertyChanged("LastProbePosWork");
        }
        private void BtnSaveFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        var path = dialog.SelectedPath;
                        Properties.Settings.Default.SaveFolderPath = path;
                        //TxtFile.ToolTip = file;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        Properties.Settings.Default.SaveFolderPath = null;
                        //TxtFile.ToolTip = null;
                        break;
                }
            }
        }
        private void UnhandledException(object sender, UnhandledExceptionEventArgs ea)
        {
            Exception e = (Exception)ea.ExceptionObject;

            string info = "Unhandled Exception:\r\nMessage:\r\n";
            info += e.Message;
            info += "\r\nStackTrace:\r\n";
            info += e.StackTrace;
            info += "\r\nToString():\r\n";
            info += e.ToString();

            MessageBox.Show(info);
            Console.WriteLine(info);

            try
            {
                System.IO.File.WriteAllText("OpenCNCPilot_Crash_Log.txt", info);
            }
            catch { }

            Environment.Exit(1);
        }

        private void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            if (e.SettingName.Equals("JogFeed") ||
                e.SettingName.Equals("JogDistance") ||
                e.SettingName.Equals("ProbeFeed") ||
                e.SettingName.Equals("ProbeSafeHeight") ||
                e.SettingName.Equals("ProbeMinimumHeight") ||
                e.SettingName.Equals("ProbeMaxDepth") ||
                e.SettingName.Equals("SplitSegmentLength") ||
                e.SettingName.Equals("ViewportArcSplit") ||
                e.SettingName.Equals("ArcToLineSegmentLength") ||
                e.SettingName.Equals("ProbeXAxisWeight") ||
                e.SettingName.Equals("ConsoleFadeTime"))
            {
                if (((double)e.NewValue) <= 0)
                    e.Cancel = true;
            }

            if (e.SettingName.Equals("SerialPortBaud") ||
                e.SettingName.Equals("StatusPollInterval") ||
                e.SettingName.Equals("ControllerBufferSize"))
            {
                if (((int)e.NewValue) <= 0)
                    e.Cancel = true;
            }
        }
        private void btnTestFirst_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommand(0);

        }
        private void btnTestBetween_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommand(1);
        }
        private void updateCameraSettingsOptions()
        {
            cameraSettingsCB.Items.Clear();

            DirectoryInfo d = new DirectoryInfo("./Resources/CameraSettings/");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
            //string str = "";
            foreach (FileInfo file in Files)
            {
                cameraSettingsCB.Items.Add(file);
            }


            if (!cameraSettingsCB.Items.Contains(Properties.Settings.Default.CameraSettingsPath))
            {
                cameraSettingsCB.SelectedItem = 0;
                cameraSettingsCB.SelectedIndex = 0;
            }


        }

        private void selectAll_Change(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in checkBoxes)
            {
                if (box != null)
                {
                    if (Properties.Settings.Default.SelectAll == true)
                    {
                        box.IsChecked = true;
                    }
                    else
                    {
                        box.IsChecked = false;
                    }
                }
            }
        }
        private bool FindCheckedBox(int index, bool firstBox)
        {
            if (!firstBox)
            {
                index++;
            }
            while (index < Properties.Settings.Default.NumLocations)
            {

                if (checkBoxes[index].IsChecked == true)
                {
                    Console.WriteLine("Box " + (index + 1) + " checked");
                    currentIndex = index;
                    return true;
                }
                else
                {
                    index++;
                }

            }
            Console.WriteLine("No more boxes checked");
            return false;
        }
        bool foundPlate = false;
        public void checkCycle()
        {

            if (runningCycle)
            {
                if (machine.Status == "Home") return;
           
                if (machine.WorkPosition.X == (double)targetLocation && machine.Status == "Idle")
                {
                    Console.WriteLine("Current Index" + currentIndex);
                    cameraControl.CapSaveImage();
                    if (Properties.Settings.Default.CurrentPlate < Properties.Settings.Default.TotalPlates)
                    {
                        Properties.Settings.Default.CurrentPlate++;
                    }
                    else
                    {
                        Properties.Settings.Default.CurrentPlate = 1;
                    }

                    Console.WriteLine("Found Plate: " + foundPlate);
                    foundPlate = FindCheckedBox(currentIndex, false);
                    if (foundPlate)
                    {
                        targetLocation = machine.sendMotionCommand(currentIndex);
                    }
                    else endCycle();

                }

            }
            else
            {
                return;
            }
        }
     
     
         
        private void disableMachineControlButtons()
        {
            btnMove.IsEnabled = false;
            btnRunCycle.IsEnabled = false;
            btnRunTimeLapse.IsEnabled = false;
            btnTestFirst.IsEnabled = false;
            btnTestBetween.IsEnabled = false;
        }
        private void enableMachineControlButtons()
        {
            btnMove.IsEnabled = true;
            btnRunCycle.IsEnabled = true;
            btnRunTimeLapse.IsEnabled = true;
            btnTestFirst.IsEnabled = true;
            btnTestBetween.IsEnabled = true;
        }
        private void disableCameraControlButtons()
        {
            btnCapture.IsEnabled = false;
            btnCameraSettingsReload.IsEnabled = false;

        }
        private void enableCameraControlButtons()
        {
            btnCapture.IsEnabled = true;
            btnCameraSettingsReload.IsEnabled = false;
        }

        private void stopTimeLapse()
        {
            stopCycle();
            if(tokenSource != null)
            {
                tokenSource.Cancel();
            }
            runningTimeLapse = false;
        }
        async Task runSingleTimeLapse(TimeSpan duration, CancellationToken token)
        {
         
            while (duration.TotalSeconds > 0)
            {
                timeLapseCount.Text = duration.TotalMinutes.ToString() + " minute(s)";
                if(duration.TotalMinutes <= 1)
                {
                    disableMachineControlButtons();
                }
               
                await Task.Delay(60*1000, token);
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
                startCycle();
                try
                {
                    await runSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine("TimeLapse Cancelled");
                    runningTimeLapse = false;
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
        private string parseStatus(bool status)
        {
           return status ? "Running" : "Not Running";
        }
        public void updateTimeLapseStatus(bool timeLapseRunning)
        {
            timeLapseStatus.Text = parseStatus(timeLapseRunning);
            if (timeLapseRunning)
            {
                btnStopTimeLapse.IsEnabled = true;
                btnRunTimeLapse.IsEnabled = false;
            }
            else
            {
                btnStopTimeLapse.IsEnabled = false;
                btnRunTimeLapse.IsEnabled = true;
                enableMachineControlButtons();
            }
        }
        public void updateCycleStatus(bool cycleRunning)
        {
            if (!cycleRunning)
            {
                enableMachineControlButtons();
            }
          
            cycleStatus.Text = parseStatus(cycleRunning);
        }
     
        public async void startTimeLapse()
        {
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
            Double endTime = Properties.Settings.Default.tlEndIntervalType * Properties.Settings.Default.tlEndInterval;


            handleTimelapseCalculations(timeLapseInterval, endTime);
       
        }
        public void startCycle()
        {
            if (machine.Connected)
            {
                btnRunCycle.IsEnabled = false;
                if (runningCycle) stopCycle();
                runningCycle = true;
                updateCycleStatus(runningCycle);
                currentIndex = 0;
                bool isPlateFound = FindCheckedBox(currentIndex, true);
                if (!isPlateFound) return;
                if (firstRun)
                {
                    machine.SendLine("$H");
                    //Properties.Settings.Default.CurrentPlate = 1;
                    firstRun = false;
                }
                else
                {
                    targetLocation = machine.sendMotionCommand(currentIndex);
                }
            }
            else
            {
                Machine_NonFatalException("Machine not connected");
            }
           

        }
        public void endCycle()
        {
            runningCycle = false;
            updateCycleStatus(runningCycle);
            currentIndex = 0;
            machine.sendMotionCommand(0);
            enableMachineControlButtons();
        }
        public void stopCycle()
        {
            endCycle();
            if (machine.Connected)
            {
                machine.SoftReset();
            }
     
        }
        List<CheckBox> checkBoxes = new List<CheckBox>();
        List<TextBlock> textBlocks = new List<TextBlock>();
        public void updatePlateClick(object sender, RoutedEventArgs e)
        {
            if (spCheckboxes != null)
            {
                spCheckboxes.Children.Clear();
                checkBoxes.Clear();
                textBlocks.Clear();
                updatePlateCheckboxes();

            }

        }

        public void updatePlateCheckboxes()
        {

            int numBoxes = Properties.Settings.Default.NumLocations;
            for (int i = 0; i < numBoxes; i++)
            {
                CheckBox tempCB = new CheckBox();
                tempCB.VerticalAlignment = VerticalAlignment.Center;
                tempCB.IsChecked = true;
                checkBoxes.Add(tempCB);
                TextBlock tempText = new TextBlock();
                tempText.Text = i.ToString();
                tempText.VerticalAlignment = VerticalAlignment.Center;
                tempText.Margin = new Thickness(5);
                textBlocks.Add(tempText);

                spCheckboxes.Children.Add(checkBoxes[i]);
                spCheckboxes.Children.Add(textBlocks[i]);

            }
        }
        private void updateSerialPortComboBox()
        {

            SerialPortSelect.Items.Clear();

            foreach (string port in System.IO.Ports.SerialPort.GetPortNames())
                SerialPortSelect.Items.Add(port);

            if (!SerialPortSelect.Items.Contains(Properties.Settings.Default.SerialPortName))
            {
                SerialPortSelect.SelectedItem = 0;
                SerialPortSelect.SelectedIndex = 0;
            }


        }
        private void cbCameraOpen(object sender, EventArgs e)
        {

        }
        private void cbSerialOpen(object sender, EventArgs e)
        {
            updateSerialPortComboBox();

        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }


        public string Version
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version}";
            }
        }



        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string file = files[0];

                    if (file.EndsWith(".hmap"))
                    {
                        if (machine.Mode == Machine.OperatingMode.Probe)
                            return;

                        //OpenHeightMap(file);
                    }
                    else
                    {
                        if (machine.Mode == Machine.OperatingMode.SendFile)
                            return;

                        try
                        {
                            machine.SetFile(System.IO.File.ReadAllLines(file));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string file = files[0];

                    if (file.EndsWith(".hmap"))
                    {
                        if (machine.Mode != Machine.OperatingMode.Probe)
                        {
                            e.Effects = DragDropEffects.Copy;
                            return;
                        }
                    }
                    else
                    {
                        if (machine.Mode != Machine.OperatingMode.SendFile)
                        {
                            e.Effects = DragDropEffects.Copy;
                            return;
                        }
                    }
                }
            }

            e.Effects = DragDropEffects.None;
        }

        private void viewport_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                machine.FeedHold();
                e.Handled = true;
            }
        }

        private void ButtonRapidOverride_Click(object sender, RoutedEventArgs e)
        {
            //Button b = sender as Button;

            //if (b == null)
            //	return;

            //switch(b.Content as string)
            //{
            //	case "100%":
            //		machine.SendControl(0x95);
            //		break;
            //	case "50%":
            //		machine.SendControl(0x96);
            //		break;
            //	case "25%":
            //		machine.SendControl(0x97);
            //		break;
            //}
        }

        private void ButtonFeedOverride_Click(object sender, RoutedEventArgs e)
        {
            //Button b = sender as Button;

            //if (b == null)
            //	return;

            //switch (b.Tag as string)
            //{
            //	case "100%":
            //		machine.SendControl(0x90);
            //		break;
            //	case "+10%":
            //		machine.SendControl(0x91);
            //		break;
            //	case "-10%":
            //		machine.SendControl(0x92);
            //		break;
            //	case "+1%":
            //		machine.SendControl(0x93);
            //		break;
            //	case "-1%":
            //		machine.SendControl(0x94);
            //		break;
            //}
        }

        private void ButtonResetViewport_Click(object sender, RoutedEventArgs e)
        {
            //viewport.Camera.Position = new System.Windows.Media.Media3D.Point3D(50, -150, 250);
            //viewport.Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(-50, 150, -250);
            //viewport.Camera.UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 1);
        }

        private void ButtonSaveTLOPos_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Machine.OperatingMode.Manual)
                return;

            double Z = (Properties.Settings.Default.TLSUseActualPos) ? machine.MachinePosition.Z : LastProbePosMachine.Z;

            Properties.Settings.Default.ToolLengthSetterPos = Z;
        }

        private void ButtonApplyTLO_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Machine.OperatingMode.Manual)
                return;

            double Z = (Properties.Settings.Default.TLSUseActualPos) ? machine.MachinePosition.Z : LastProbePosMachine.Z;

            double delta = Z - Properties.Settings.Default.ToolLengthSetterPos;

            machine.SendLine($"G43.1 Z{delta.ToString(Constants.DecimalOutputFormat)}");
        }

        private void ButtonClearTLO_Click(object sender, RoutedEventArgs e)
        {
            if (machine.Mode != Machine.OperatingMode.Manual)
                return;

            machine.SendLine("G49");
        }

        private void cbSelectAll_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void btnCameraSettingsReload_Click(object sender, RoutedEventArgs e)
        {
            cameraControl.forceSettingsReload();
        }

        private void cameraSettingsCB_DropDownOpened(object sender, EventArgs e)
        {
            updateCameraSettingsOptions();
        }

        private void cameraSettingsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cameraControl.loadCameraSettings();
        }
        private void ButtonStartTimeLapse_Click(object sender, RoutedEventArgs e)
        {
            startTimeLapse();
        }
        private void ButtonStopTimeLapse_Click(object sender, RoutedEventArgs e)
        {
            stopTimeLapse();
        }
        private void ButtonStopCycle_Click(object sender, RoutedEventArgs e)
        {
            stopCycle();
        }
    }
}
