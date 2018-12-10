using SPIPware.Communication;
using SynchronousGrab;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SPIPware
{

    public sealed class CameraControl
    {
        private static readonly CameraControl instance = new CameraControl();
        static CameraControl()
        {

        }
        public static CameraControl Instance
        {
            get
            {
                return instance;
            }
        }
        //public int currentIndex = 0;
        //public static bool runningCycle = false;
        //public bool home = false;
        //public bool goToFirstPlate = true;
        //public bool firstRun = true;
        public bool settingsLoaded = false;

        string previousSettingsDir;
        private VimbaHelper m_VimbaHelper = null;
        public Image m_PictureBox;
        //TODO Make real output
        private CameraInfo selectedItem;
        public VimbaHelper VimbaHelper { get => m_VimbaHelper; set => m_VimbaHelper = value; }
        public CameraInfo SelectedItem { get => selectedItem; set => selectedItem = value; }
        //Add log message to logging list box
        public delegate void ImageAcquired();
        public event EventHandler ImageAcquiredEvent;
        
        public void StartVimba()
        {
            //cameraControl = new CameraControl();
            //TODO Refactor to raise event
            //updateCameraSettingsOptions();
            try
            {
                //Start up Vimba API
                VimbaHelper vimbaHelper = new VimbaHelper();
                vimbaHelper.Startup(OnCameraListChanged);
                //Text += String.Format(" Vimba .NET API Version {0}", vimbaHelper.GetVersion());
                m_VimbaHelper = vimbaHelper;
                VimbaHelper = m_VimbaHelper;

                VimbaHelper.ImageAcquiredEvent += OnImageAcquired;
                try
                {
                    UpdateCameraList();

                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
            catch (Exception exception)
            {
                LogError("Could not startup Vimba API. Reason: " + exception.Message);
            }
        }
        public void OnImageAcquired(object sender, EventArgs e)
        {
            ImageAcquiredEvent.Raise(this, new EventArgs());
        }
        public void ShutdownVimba()
        {
            if (null != m_VimbaHelper)
            {
                try
                {
                    //Shutdown Vimba API when application exits
                    m_VimbaHelper.Shutdown();

                    m_VimbaHelper = null;
                }
                catch (Exception exception)
                {
                    LogError("Could not shutdown Vimba API. Reason: " + exception.Message);
                }
            }
        }
        public void LogMessage(string message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }
            
            Console.WriteLine(message);
            //int index = m_LogList.Items.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, message));
            //m_LogList.TopIndex = index;
        }

        //Add an error log message and show an error message box
        public void LogError(string message)
        {
            LogMessage(message);
            Console.WriteLine(message);
            //runningCycle = false;
            //MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        List<CameraInfo> cameras;
        public void UpdateCameraState(bool cameraReady)
        {
            if (cameraReady)
            {
                Properties.Settings.Default.CameraState = "Ready";
                
            }
            else
            {
                Properties.Settings.Default.CameraState = "Not Ready";
            }
        }
        public void UpdateCameraList()
        {
            //Remember the old selection (if there was any)y
            //CameraInfo oldSelectedItem = m_CameraList.SelectedItem as CameraInfo;
            //m_CameraList.Items.Clear();
            
             cameras = VimbaHelper.CameraList;
            
            if (cameras.Any())
            {
                //CameraInfo newSelectedItem = cameras[0];
                UpdateCameraState(true);
                //Console.WriteLine("New Selected Camera" + newSelectedItem);
            }
            else
            {
                UpdateCameraState(false);
            }
            //foreach (CameraInfo cameraInfo in cameras)
            //{
            //    //m_CameraList.Items.Add(cameraInfo);
               

            //    if (null == newSelectedItem)
            //    {
            //        //At least select the first camera
            //        newSelectedItem = cameraInfo;
            //    }
            //    //else if (null != oldSelectedItem)
            //    //{
            //    //    //If the previous selected camera is still available
            //    //    //then prefer this camera.
            //    //    if (string.Compare(newSelectedItem.ID, cameraInfo.ID, StringComparison.Ordinal) == 0)
            //    //    {
            //    //        newSelectedItem = cameraInfo;
            //    //    }
            //    //}
            //}
            //Console.WriteLine("New Selected Camera" + newSelectedItem);

            ////If available select a camera.
            //if (null != newSelectedItem)
            //{

            //    //m_CameraList.SelectedItem = newSelectedItem;
            //    Properties.Settings.Default.CameraState = "Ready";
            //}
            //else
            //{
            //    Properties.Settings.Default.CameraState = "Not Ready";
            //}
            
        }

        public void OnCameraListChanged(object sender, EventArgs args)
        {
            //Start an async invoke in case this method was not
            //called by the GUI thread.
            //if (!m_CameraList.Dispatcher.CheckAccess())
            //{
            //    m_CameraList.Dispatcher.Invoke(new CameraListChangedHandler(this.OnCameraListChanged), sender, args);
            //    return;
            //}

            if (null != VimbaHelper)
            {
                try
                {
                    UpdateCameraList();
                    
                    LogMessage("Camera list updated.");
                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
        }
        public void loadCameraSettings()
        {
            string cameraSettingsFileName = Directory.GetCurrentDirectory() + @"\Resources\CameraSettings\" + Properties.Settings.Default.CameraSettingsPath;
            if (!String.Equals(cameraSettingsFileName, previousSettingsDir))
            {
                previousSettingsDir = cameraSettingsFileName;
                forceSettingsReload();
            }



        }
        public void forceSettingsReload()
        {
            string cameraSettingsFileName = Directory.GetCurrentDirectory() + @"\Resources\CameraSettings\" + Properties.Settings.Default.CameraSettingsPath;
            if (File.Exists(cameraSettingsFileName))
            {
                LogMessage("Loading camera settings");
                if (VimbaHelper != null && selectedItem != null)
                {
                    Console.WriteLine(selectedItem.ID);
                    VimbaHelper.loadCamSettings(cameraSettingsFileName, selectedItem.ID);
                    LogMessage("Loaded Camera Settings");
                }
                else
                {
                    LogError("Settings not loaded because camera is disconnected");
                }

            }
            else
            {
                LogError(cameraSettingsFileName);
                LogError("Invalid file name for Camera Settings");
            }
            settingsLoaded = true;


        }
        public BitmapImage bi;
        public BitmapImage UpdateImageBox(System.Drawing.Image image)
        {
            
            using (var ms = new MemoryStream())
                {
                    bi = new BitmapImage();
                     image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    ms.Position = 0;

                   
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();


                }

            //Display imageD
            //m_PictureBox.Source = bi;
            //Application.Current.Dispatcher.Invoke(new Action(() => ));
           

            return bi;

        }
        public Task WriteBitmapToFile(BitmapImage image, string filePath)
        {
            Task task = new Task(() =>
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            });
            return task;
        }
        ImageFormat fileType = ImageFormat.Png;
        public String createFilePath()
        {
            StringBuilder sb = new StringBuilder();
            if (Properties.Settings.Default.CurrentPlateSave == true)
            {
                string currentPlateStr = (Properties.Settings.Default.CurrentPlate).ToString();
                sb.Append(currentPlateStr + "_");
            }
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd--H-mm-ss");
    
            sb.Append(currentDate + "_");
            sb.Append(Properties.Settings.Default.FileName);
            sb.Append("." + fileType.ToString().ToLower());

            String filePath = Path.Combine(Properties.Settings.Default.SaveFolderPath, sb.ToString());
            return filePath;
        }
      public Task WriteImageToFile(System.Drawing.Image  image)
        {
            Task task = new Task(()=>{
               
                String filePath = createFilePath();
                //Console.WriteLine("Image written to: " + filePath);
                // Console.WriteLine("File Name: " + sb.ToString());

                image.Save(filePath, fileType);
                Console.WriteLine("Image written to: " + filePath);
                LogMessage("Image acquired synchonously.");
                if (Properties.Settings.Default.CurrentPlate < Properties.Settings.Default.TotalPlates)
                {
                    Properties.Settings.Default.CurrentPlate++;
                }
                else
                {
                    Properties.Settings.Default.CurrentPlate = 1;
                }
            });

            return task;
        }
        public BitmapImage CapSaveImage()
        {
            try
            {
                //Determine selected camera

                selectedItem = cameras[0];
                if (null == SelectedItem)
                {
                    throw new NullReferenceException("No camera selected.");
                }
                

                //Acquire an image synchronously (snap) from selected camera
                System.Drawing.Image image = VimbaHelper.AcquireSingleImage(SelectedItem.ID);
                System.Drawing.Image imageCopy = (System.Drawing.Image)image.Clone();
                BitmapImage img = UpdateImageBox(image);
               

                String filePath = createFilePath();
                if (Directory.Exists(Properties.Settings.Default.SaveFolderPath))
                {
                   
                    Task witf = WriteImageToFile(imageCopy);
                    witf.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                    witf.Start();

                }
                else
                {
                    LogError("Invalid directory selected");
                }
                return img;

            }
            catch (Exception exception)
            {
                LogError("Could not acquire image. Reason: " + exception.Message);
                return null;
            }
        }
        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Console.WriteLine(exception);
        }
    }
   
}
