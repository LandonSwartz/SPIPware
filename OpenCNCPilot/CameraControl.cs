using SynchronousGrab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenCNCPilot
{

    class CameraControl
    {
        private VimbaHelper m_VimbaHelper = null;
        public ComboBox m_CameraList;
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

            //int index = m_LogList.Items.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, message));
            //m_LogList.TopIndex = index;
        }

        //Add an error log message and show an error message box
        public void LogError(string message)
        {
            LogMessage(message);
            //runningCycle = false;
            //MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void UpdateCameraList()
        {
            //Remember the old selection (if there was any)y
            CameraInfo oldSelectedItem = m_CameraList.SelectedItem as CameraInfo;
            m_CameraList.Items.Clear();

            List<CameraInfo> cameras = VimbaHelper.CameraList;

            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
                m_CameraList.Items.Add(cameraInfo);
               

                if (null == newSelectedItem)
                {
                    //At least select the first camera
                    newSelectedItem = cameraInfo;
                }
                else if (null != oldSelectedItem)
                {
                    //If the previous selected camera is still available
                    //then prefer this camera.
                    if (string.Compare(newSelectedItem.ID, cameraInfo.ID, StringComparison.Ordinal) == 0)
                    {
                        newSelectedItem = cameraInfo;
                    }
                }
            }
            Console.WriteLine("New Selected Camera" + newSelectedItem);

            //If available select a camera.
            if (null != newSelectedItem)
            {

                m_CameraList.SelectedItem = newSelectedItem;
            }
        }

        public void OnCameraListChanged(object sender, EventArgs args)
        {
            //Start an async invoke in case this method was not
            //called by the GUI thread.
            if (! m_CameraList.Dispatcher.CheckAccess())
            {
                m_CameraList.Dispatcher.Invoke(new CameraListChangedHandler(this.OnCameraListChanged), sender, args);
                return;
            }

            if (null != VimbaHelper)
            {
                try
                {
                    UpdateCameraList();
                    Console.WriteLine("Camera List Updated");
                    LogMessage("Camera list updated.");
                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
        }
    }
}
