using log4net;
using SPIPware.Communication;
using SPIPware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

/* Useful Links
 * https://social.msdn.microsoft.com/Forums/vstudio/en-US/2f303aa2-73c8-420b-ab6d-2dde22a9d0bb/wpf-dynamically-adding-controls-to-grid?forum=wpf
 */

namespace SPIPware
{
    partial class MainWindow
    {
        //WILL UPDATE WHEN WORKING ON EXPERIMENT BUILDER WINDOW
        List<List<CheckBox>> checkBoxes2D = new List<List<CheckBox>>();
        List<TextBlock> textBlocks = new List<TextBlock>();

        //removed because plate checkboxes removed from main window for time being
       /* public void UpdatePlateClick(object sender, RoutedEventArgs e)
        {
            if (spCheckboxes != null)
            {
                UpdatePlateCheckboxes();

            }
            else
            {
                _log.Error("Could not update plate checkboxes, spCheckboxes null");
            }

        }*/

        public void UpdatePlateCheckboxes(bool value)
        {
            if (checkBoxes2D != null)
            {
                //1D version
                //checkBoxes2D.ForEach(c => c.IsChecked = value);

                foreach(List<CheckBox> boxList in checkBoxes2D)
                {
                    foreach(CheckBox box in boxList)
                    {
                        box.IsChecked = value;
                    }
                }
            }
        }
        public void GenerateCurrentParametersList(Experiment experiment, Panel panel)
        {
            Dispatcher.Invoke(() =>
            {

                panel.Children.Clear();

                StackPanel leftPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5)
                };
                StackPanel rightPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5)
                };
                if (experiment != null)
                {
                    _log.Debug("Updating Current Paramemters for Experiment: " + experiment.FileName);
                    PropertyInfo[] properties = typeof(Experiment).GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        _log.Info(property.Name);
                        if (Experiment.friendlyNames.TryGetValue(property.Name, out string friendlyName))
                        {
                            friendlyName += " :";
                            TextBlock keyText = new TextBlock
                            {
                                Text = friendlyName,
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(3)
                            };
                            leftPanel.Children.Add(keyText);
                            Object value = property.GetValue(experiment);
                            String strVal = "WARN: Undefined";
                            if (value != null)
                            {
                                strVal = value.ToString();
                            }
                            TextBlock valueText = new TextBlock
                            {
                                Text = strVal,
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(3)
                            };
                            rightPanel.Children.Add(valueText);
                        }
                        else
                        {
                            //_log.Debug("Property name not in frienldy name dictonary");
                        }


                    }
                }
                else
                {
                    TextBlock keyText = new TextBlock
                    {
                        Text = "WARNING: ",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(3)
                      
                    };
                    leftPanel.Children.Add(keyText);
                    TextBlock valueText = new TextBlock
                    {
                        Text = "Experiment not Selected",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(3)

                    };
                    rightPanel.Children.Add(valueText);

                }

                panel.Children.Add(leftPanel);
                panel.Children.Add(rightPanel);

            });
            
        }
        //removed for now because plate checkboxes removed from main window
  /*      public void UpdatePlateCheckboxes()
        { //will redo checkboxes with expeirment builder so commenting below out FOR NOW    
            Dispatcher.Invoke(() =>
            {
                _log.Debug("Updating Plate Checkboxes");
                int numBoxes = Properties.Settings.Default.NumLocationsRow;
                spCheckboxes.Children.Clear();
                foreach(List<CheckBox> list in checkBoxes2D)
                {
                        list.Clear();
                }

                
                textBlocks.Clear();
                for (int i = 0; i < numBoxes; i++)
                {
                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10)
                    };


                    CheckBox tempCB = new CheckBox
                    {
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    tempCB.Click += new RoutedEventHandler(PlateCheck_Change);

                    if (cycle.ImagePositions!= null && cycle.ImagePositions.Contains(i))
                    {
                        tempCB.IsChecked = true;
                    }
                    else
                    {
                        cbSelectAll.IsChecked = null;
                        tempCB.IsChecked = false;
                        //IsThreeState = true;
                    }
                    
                    foreach(List<CheckBox> list in checkBoxes2D)
                    {
                        list.Add(tempCB);
                    }
                    

                    TextBlock tempText = new TextBlock();
                    tempText.Text = (i + 1).ToString();
                    tempText.VerticalAlignment = VerticalAlignment.Center;
                    tempText.Margin = new Thickness(5);
                    textBlocks.Add(tempText);

                    foreach(List<CheckBox> list in checkBoxes2D)
                    {
                        foreach(CheckBox box in list)
                        {
                            stackPanel.Children.Add(box);
                        }
                        
                    }
                    
                    stackPanel.Children.Add(textBlocks[i]);

                    spCheckboxes.Children.Add(stackPanel);

                    foreach(List<CheckBox> list in checkBoxes2D)
                    {
                        foreach(CheckBox box in list)
                        {
                            spCheckboxes.Children.Add(box);
                        }
                        
                    }
                    
                    spCheckboxes.Children.Add(textBlocks[i]);

                }
                cycle.UpdatePositionList(checkBoxes2D);
            });
            //checkBoxes.ForEach(c => c.Click += new RoutedEventHandler(PlateCheck_Change));
            _log.Debug("Number of plate checkboxes: " + checkBoxes2D.Count);
            

        }*/

        private void PlateCheck_Change(object sender, System.EventArgs e)
        {
            //  WILL NEED TO FIX LATER
            /*
            if (checkBoxes2D.All(c => c.IsChecked == true))
            {
                SelectAllValue = true;
            }
            else if (checkBoxes2D.All(c => c.IsChecked == false))
            {
                SelectAllValue = false;
            }
            else
            {
                SelectAllValue = null;
                cbSelectAll.IsChecked = null;
            }
            Dispatcher.Invoke(() => cycle.UpdatePositionList(checkBoxes2D)); */

        }

        bool? _SelectAllValue;
        public bool? SelectAllValue
        {
            get { return _SelectAllValue; }
            set
            {

                if (value == null)
                {
                    IsThreeState = true;
                }
                else
                {
                    IsThreeState = false;
                }
                _SelectAllValue = value;

            }
        }

        bool _IsThreeState = true;
        public bool IsThreeState
        {
            get { return _IsThreeState; }
            private set
            {
                _IsThreeState = value;
            }
        }
        private void selectAll_Change(object sender, RoutedEventArgs e)
        {
            SelectAll_Change();

        }
        private void SelectAll_Change()
        {
            foreach(List<CheckBox> boxList in checkBoxes2D)
            {
                foreach(CheckBox box in boxList)
                {
                    if (box != null)
                    {
                        if (SelectAllValue == true)
                        {
                            box.IsChecked = true;
                        }
                        else if (SelectAllValue == false)
                        {
                            box.IsChecked = false;
                        }
                    }
                  //  Dispatcher.Invoke(() => cycle.UpdatePositionList(checkBoxes2D));
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
        public void SaveSettingsToFile(string filePath)
        {
            //Dispatcher.Invoke(() => cycle.UpdatePositionList(checkBoxes));

            Experiment experiment = new Experiment();
            experiment.LoadExperiment();
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
                experiment.LoadExperiment();
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
           // UpdatePlateCheckboxes();
            UpdateClrCanvas();
            UpdateBacklightColor();
        }
        public void LoadSettings(string fileName)
        {

            //Experiment experiment = new Experiment(fileName);
            Experiment experiment = Experiment.LoadExperimentAndSave(fileName);
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
        private void UpdateTLParams()
        {
            Experiment tlExperiment = Experiment.LoadExperiment(Properties.Settings.Default.tlExperimentPath);
            GenerateCurrentParametersList(tlExperiment, TLCurrentParameters);

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
                UpdateTLParams();
            }

        }
        private void ButtonSaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingsToFile();
            //Task task = new Task(() => SaveSettingsToFile());
            //task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //task.Start();
        }
        private void ButtonSaveExperimentDefaults_Click(object sender, RoutedEventArgs e)
        {
            SaveDefaults();
            //Task task = new Task(() => SaveDefaults());
            //task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //task.Start();
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
        private void UpdateCurrentParams_Click(object sender, RoutedEventArgs e)
        {
            Experiment testExperiment = new Experiment();
            testExperiment.LoadExperiment();
            GenerateCurrentParametersList(testExperiment, SPCurrentParameters);

        }
        private void UpdateTLParams_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Make reload on load
            //TODO: Fix random issues with crashing on parameters loading
          
                UpdateTLParams();
        
 

        }

    }
}
