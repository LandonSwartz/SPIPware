using SynchronousGrab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SPIPware
{
    partial class MainWindow
    {
        //private static VimbaHelper m_VimbaHelper = null;


        private void updateCameraSettingsOptions()
        {
            string csPath = Properties.Settings.Default.CameraSettingsPath;
            cameraSettingsCB.Items.Clear();

            DirectoryInfo d = new DirectoryInfo("./Resources/CameraSettings/");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
            //string str = "";
  
                int i = 0;
                int selectedIndex = i;
                foreach (FileInfo file in Files)
                {
                    if (string.Equals(file.Name, csPath))
                    {
                        selectedIndex = i;
                    }
                    cameraSettingsCB.Items.Add(file);
                    i++;
                }
                //int index = Files.FindIndxx var match = Files.FirstOrDefault(file => file.Name.Contains(Properties.Settings.Default.CameraSettingsPath));
                Dispatcher.Invoke(() =>
                {//this refer to form in WPF application 
                    cameraSettingsCB.SelectedIndex = selectedIndex;
                    //if (match == null)
                    //{
                    //    //cameraSettingsCB.SelectedItem = 0;

                    //}
                    //else
                    //{
                    //    cameraSettingsCB.SelectedItem = Properties.Settings.Default.CameraSettingsPath;
                    //}
                });

        }
        public BitmapImage bi;
        public void UpdatePictureBox(object sender, EventArgs e)
        {

            //if (Application.Current.Dispatcher.CheckAccess())
            //{
            //    bi = cycle.bi.Clone();
            //    m_PictureBox.Source = camera.bi;
            //}
            //else
            //{
            //    //Other wise re-invoke the method with UI thread access
            //    Application.Current.Dispatcher.Invoke(new System.Action(() =>
            //    {
            //        Thread.Sleep(300);
            //        bi = cycle.bi.Clone();
            //        m_PictureBox.Source = bi;

            //    }));
            //}
        }
        private void cameraStatusUpdated(object sender, DataTransferEventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            updateCameraStatus(text.Text);
        }
        private void disableCameraControlButtons()
        {
            btnCapture.IsEnabled = false;
            //btnCameraSettingsReload.IsEnabled = false;

        }
        private void enableCameraControlButtons()
        {
            btnCapture.IsEnabled = true;
            //btnCameraSettingsReload.IsEnabled = false;
        }
        public void updateCameraStatus(string cameraStatus)
        {
            if (string.Equals("Not Ready", cameraStatus))
            {
                cameraStatusIcon.Source = RED_IMAGE;
            }
            else
            {
                cameraStatusIcon.Source = GREEN_IMAGE;
            }
        }
        private void cbCameraOpen(object sender, EventArgs e)
        {

        }
    }
}
