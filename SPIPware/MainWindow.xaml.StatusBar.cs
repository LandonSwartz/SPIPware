using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPIPware
{
	partial class MainWindow
	{
        public const string GREEN_STATUS = "pack://application:,,,/Resources/Icons/Button_Icon_Green.png";
        public const string YELLOW_STATUS = "pack://application:,,,/Resources/Icons/Button_Icon_Yellow.png";
        public const string RED_STATUS = "pack://application:,,,/Resources/Icons/Button_Icon_Red.png";

        BitmapImage GREEN_IMAGE = new BitmapImage(new Uri(GREEN_STATUS, UriKind.Absolute));
        BitmapImage YELLOW_IMAGE = new BitmapImage(new Uri(YELLOW_STATUS, UriKind.Absolute));
        BitmapImage RED_IMAGE = new BitmapImage(new Uri(RED_STATUS, UriKind.Absolute));

        private void ButtonFeedHold_Click(object sender, RoutedEventArgs e)
		{
			machine.FeedHold();
		}

		//private void ButtonCycleStart_Click(object sender, RoutedEventArgs e)
		//{
		//	machine.CycleStart();
		//}

		private void ButtonSoftReset_Click(object sender, RoutedEventArgs e)
		{
			new Thread(()=>machine.SoftReset()).Start();
		}
        public void toggleButtonVisibility(Button connectButton, Button disconnectButton, bool connected)
        {
            if (connected)
            {
                connectButton.Visibility = Visibility.Collapsed;
                disconnectButton.Visibility = Visibility.Visible;
            }
            else
            {
                connectButton.Visibility = Visibility.Visible;
                disconnectButton.Visibility = Visibility.Collapsed;
            }

        }
    }
}
