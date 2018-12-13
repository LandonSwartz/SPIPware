using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string DEFAULT_SETTINGS_PATH = @"Resources\defaultSettings.bin";

        public void UpdatePlateClick(object sender, RoutedEventArgs e)
        {
            if (spCheckboxes != null)
            {
                spCheckboxes.Children.Clear();
                checkBoxes.Clear();
                textBlocks.Clear();
                UpdatePlateCheckboxes();

            }

        }
        public void UpdatePlateCheckboxes()
        {

            int numBoxes = Properties.Settings.Default.NumLocations;
            spCheckboxes.Children.Clear();
            for (int i = 0; i < numBoxes; i++)
            {
                //StackPanel stackPanel = new StackPanel();
                //stackPanel.Orientation = Orientation.Vertical;
                //stackPanel.Margin = new Thickness(5);

                CheckBox tempCB = new CheckBox();
                tempCB.VerticalAlignment = VerticalAlignment.Center;
                tempCB.IsChecked = true;
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
            Experiment.LoadExperimentToSettings(DEFAULT_SETTINGS_PATH);
            Properties.Settings.Default.ExperimentPath = DEFAULT_SETTINGS_PATH;
            //UpdateClrCanvas();
        }
        public void SaveDefaults()
        {
            Experiment.CreateExperimentFromSettings(DEFAULT_SETTINGS_PATH);
            Properties.Settings.Default.ExperimentPath = DEFAULT_SETTINGS_PATH;
        }
        public void SaveSettingsToFile()
        {
            Experiment.CreateExperimentFromSettings(Properties.Settings.Default.ExperimentPath);
        }
        public System.Windows.Forms.SaveFileDialog openSaveDialog()
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "Binary|*.bin";
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
                Experiment.CreateExperimentFromSettings(dialog.FileName);
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
                Console.Write(Properties.Settings.Default.ExperimentPath);
            }
        }
        public void LoadSettingsFromFile()
        {
            var dialog = getFileResult();
            if (dialog != null)
            {
                Console.Write(dialog.FileName);
                Experiment.LoadExperimentToSettings(dialog.FileName);
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
                //UpdateClrCanvas();
            }
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
    }
}
