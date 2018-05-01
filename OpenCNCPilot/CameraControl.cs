using SynchronousGrab;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OpenCNCPilot
{

    class CameraControl
    {
        public int currentIndex = 0;
        public static bool runningCycle = false;
        public bool home = false;
        public bool goToFirstPlate = true;
        public bool firstRun = true;
        public bool settingsLoaded = false;


        private VimbaHelper m_VimbaHelper = null;
        public ComboBox m_CameraList;
        public Image m_PictureBox;
        //TODO Make real output
        private CameraInfo selectedItem;
        public VimbaHelper VimbaHelper { get => m_VimbaHelper; set => m_VimbaHelper = value; }
        public CameraInfo SelectedItem { get => selectedItem; set => selectedItem = value; }
        //Add log message to logging list box
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
        public void UpdateCameraList()
        {
            //Remember the old selection (if there was any)y
            //CameraInfo oldSelectedItem = m_CameraList.SelectedItem as CameraInfo;
            //m_CameraList.Items.Clear();
            
             cameras = VimbaHelper.CameraList;
            
            if (cameras.Any())
            {
                //CameraInfo newSelectedItem = cameras[0];

                Properties.Settings.Default.CameraState = "Ready";
                //Console.WriteLine("New Selected Camera" + newSelectedItem);
            }
            else
            {
                Properties.Settings.Default.CameraState = "Not Ready";

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
            //if (! m_CameraList.Dispatcher.CheckAccess())
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
        public void CapSaveImage()
        {

            if (!settingsLoaded)
            {
                string cameraSettingsFileName = Properties.Settings.Default.CameraSettingsPath;
                if (File.Exists(cameraSettingsFileName))
                {
                    LogMessage("Loading camera settings");
                    VimbaHelper.loadCamSettings(cameraSettingsFileName, selectedItem.ID);
                    settingsLoaded = true;
                }
                else
                {
                    LogError("Invalid file name for Camera Settings");
                }
            }
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


                BitmapImage bi;
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                   
                    ms.Position = 0;

                    bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    
                }

                //Display image
                m_PictureBox.Source = bi;
                StringBuilder sb = new StringBuilder();
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd--H-mm-ss");
                ImageFormat fileType = ImageFormat.Png;


                sb.Append(currentDate + "--");
                sb.Append(Properties.Settings.Default.FileName);
                sb.Append("." + fileType.ToString().ToLower());

                String filePath = Path.Combine(Properties.Settings.Default.SaveFolderPath, sb.ToString());
                //Console.WriteLine("Image written to: " + filePath);
                // Console.WriteLine("File Name: " + sb.ToString());
                image.Save(filePath, fileType);
                Console.WriteLine("Image written to: " + filePath);
                LogMessage("Image acquired synchonously.");

                // System.Threading.Thread.Sleep(200);


            }
            catch (Exception exception)
            {
                LogError("Could not acquire image. Reason: " + exception.Message);
            }
        }
    }
}
