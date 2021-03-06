﻿using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static SPIPware.Communication.PeripheralControl;

namespace SPIPware
{
    partial class MainWindow
    {

        
        public void StartCycle()
        {
            _log.Info("        ============= User Starting Cycle  =============        ");
            //cycle.UpdatePositionList(checkBoxes);
            cycle.Start();
            //Task task = new Task(cycle.Start);
            //task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //task.Start();

        }
        public void StopCycle()
        {
            _log.Info("        ============= User Stopping Cycle  =============        ");
            cycle.Stop();
            //Task task = new Task(cycle.Stop);
            //task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //task.Start();
        }

        //public void CheckCycle()
        //{
        //    Task task = new Task(cycle.Check);
        //    task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
        //    task.Start();
        //}
     
        public void UpdateCycleStatus(object sender, EventArgs e)
        {
            UpdateCycleStatus(cycle.runningCycle);
        }
        public void UpdateCycleStatus(bool cycleRunning)
        {
            Dispatcher.Invoke(() =>
            {//this refer to form in WPF application 

                if (!cycleRunning)
                {
                    EnableMachineControlButtons();
                    cycleStatusIcon.Source = YELLOW_IMAGE;
                }
                else
                {
                    btnRunCycle.IsEnabled = false;
                    cycleStatusIcon.Source = GREEN_IMAGE;
                }

                cycleStatus.Text = parseStatus(cycleRunning);
                toggleButtonVisibility(btnRunCycle, btnStopCycle, cycleRunning);
            });

        }
        //public void toggleImageCycleButton(bool running)
        //{
        //    if (running)
        //    {
        //        btnRunCycle.Visibility = Visibility.Collapsed;
        //        btnStopCycle.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        btnRunCycle.Visibility = Visibility.Visible;
        //        btnStopCycle.Visibility = Visibility.Collapsed;
        //    }

        //}
        private void DisableMachineControlButtons()
        {
            btnMove.IsEnabled = false;
            btnRunCycle.IsEnabled = false;
            btnRunTimeLapse.IsEnabled = false;
            btnTestFirst.IsEnabled = false;
            btnTestBetween.IsEnabled = false;
        }
        private void EnableMachineControlButtons()
        {
            btnMove.IsEnabled = true;
            btnRunCycle.IsEnabled = true;
            btnRunTimeLapse.IsEnabled = true;
            btnTestFirst.IsEnabled = true;
            btnTestBetween.IsEnabled = true;
        }
        //x axis
        private void BtnTestFirst_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommandX(0);

        }
        private void btnTestBetween_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommandX(1);
        }

        //y-axis
        private void BtnYTestFirst_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommandY(0);
        }
        private void BtnYTestBetween_Click(object sender, RoutedEventArgs e)
        {
            machine.sendMotionCommandY(1);
        }

        


    }
}
