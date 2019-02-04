using log4net;
using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPIPware
{
    partial class MainWindow
    {
        List<CheckBox> checkBoxes = new List<CheckBox>();
        List<TextBlock> textBlocks = new List<TextBlock>();
        

        public void UpdatePlateClick(object sender, RoutedEventArgs e)
        {
            if (spCheckboxes != null)
            {
                UpdatePlateCheckboxes();

            }

        }
        public void UpdatePlateCheckboxes()
        {
            Dispatcher.Invoke(() =>
            {
                _log.Debug("Updating Plate Checkboxes");
                int numBoxes = Properties.Settings.Default.NumLocations;
                spCheckboxes.Children.Clear();
                checkBoxes.Clear();
                textBlocks.Clear();
                for (int i = 0; i < numBoxes; i++)
                {
                    //StackPanel stackPanel = new StackPanel();
                    //stackPanel.Orientation = Orientation.Vertical;
                    //stackPanel.Margin = new Thickness(5);

                    CheckBox tempCB = new CheckBox();
                    tempCB.VerticalAlignment = VerticalAlignment.Center;

                    if (cycle.ImagePositions.Contains(i) || Properties.Settings.Default.SelectAll)
                    {
                        tempCB.IsChecked = true;
                    }
                    else
                    {
                        tempCB.IsChecked = false;
                    }

                    checkBoxes.Add(tempCB);

                    TextBlock tempText = new TextBlock();
                    tempText.Text = (i + 1).ToString();
                    tempText.VerticalAlignment = VerticalAlignment.Center;
                    tempText.Margin = new Thickness(5);
                    textBlocks.Add(tempText);

                    //stackPanel.Children.Add(checkBoxes[i]);
                    //stackPanel.Children.Add(textBlocks[i]);

                    //spCheckboxes.Children.Add(stackPanel);

                    spCheckboxes.Children.Add(checkBoxes[i]);
                    spCheckboxes.Children.Add(textBlocks[i]);

                }
            });
            _log.Debug("Number of plate checkboxes: " + checkBoxes.Count);
            
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
        public void LoadDefaults()
        {
            Experiment.LoadDefaults();
            Dispatcher.Invoke(() => ExperimentUpdated());

        }
        public void SaveDefaults()
        {
            SaveSettingsToFile(Experiment.DEFAULT_SETTINGS_PATH);
            //Properties.Settings.Default.ExperimentPath = Experiment.DEFAULT_SETTINGS_PATH;

        }
        public void SaveSettingsToFile()
        {
            SaveSettingsToFile(Properties.Settings.Default.ExperimentPath);
        }
        public void SaveSettingsToFile( string filePath)
        {
            Dispatcher.Invoke(() => cycle.UpdatePositionList(checkBoxes));
            
            Experiment experiment = new Experiment();
            experiment.SaveExperiment(filePath);
        }
        public System.Windows.Forms.SaveFileDialog openSaveDialog()
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "JSON|*.json";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return dialog;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        return null;
                }
            }

        }
        public void SaveAsSettingsToFile(System.Windows.Forms.SaveFileDialog dialog)
        {
            if (dialog != null)
            {
                Experiment experiment = new Experiment();
                experiment.SaveExperiment(dialog.FileName);
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
                _log.Debug(Properties.Settings.Default.ExperimentPath);
            }
        }
        public void LoadSettingsFromFile()
        {
            var dialog = getFileResult();
            if (dialog != null)
            {
                _log.Debug(dialog.FileName);
                LoadSettings(dialog.FileName);
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
                //UpdateClrCanvas();
            }
        }
        public void ExperimentUpdated(object sender, EventArgs e)
        {
            ExperimentUpdated();
        }
        public void ExperimentUpdated()
        {
            UpdatePlateCheckboxes();
            UpdateClrCanvas();
            UpdateBacklightColor();
        }
        public void LoadSettings(string fileName)
        {

            //Experiment experiment = new Experiment(fileName);
            Experiment experiment = Experiment.LoadExperiment(fileName);
            ExperimentUpdated();
        }
        private string GetFolderResult()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return dialog.SelectedPath;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        return null;
                }
            }
        }
        private System.Windows.Forms.OpenFileDialog getFileResult()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "JSON|*.json";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return dialog;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        return null;
                }
            }
        }
        private string getFileResultName()
        {
            var dialog = getFileResult();
            if (dialog != null)
            {
                return dialog.FileName;
            }
            else { return null; }

        }
        private string getFileResultName(System.Windows.Forms.OpenFileDialog dialog)
        {
            if (dialog != null)
            {
                return dialog.FileName;
            }
            else { return null; }
        }
        private void BtnSaveFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            string folderResult = GetFolderResult();
            if (folderResult != null)
            {
                Properties.Settings.Default.SaveFolderPath = folderResult;
            }

        }

        private void BtnAcquisitionFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = getFileResult();
            if (dialog != null)
            {
                Properties.Settings.Default.AcquisitionExperimentPath = getFileResultName(dialog);
            }
        }
        private void BtnExperimentFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = openSaveDialog();
            SaveAsSettingsToFile(dialog);
            if (dialog != null)
            {
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
            }

        }
        private void BtnTlExperimentFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = getFileResult();
            if (dialog != null)
            {
                Properties.Settings.Default.tlExperimentPath = getFileResultName(dialog);
            }

        }
        private void ButtonSaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => SaveSettingsToFile());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        private void ButtonSaveExperimentDefaults_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => SaveDefaults());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        private void ButtonLoadExperimentDefaults_Click(object sender, RoutedEventArgs e)
        {
            LoadDefaults();

        }
        private void ButtonSaveAsExperiment_Click(object sender, RoutedEventArgs e)
        {
            var dialog = openSaveDialog();
            SaveAsSettingsToFile(dialog);
        }
        private void ButtonLoadExperiment_Click(object sender, RoutedEventArgs e)
        {
            LoadSettingsFromFile();

        }
    }
}
