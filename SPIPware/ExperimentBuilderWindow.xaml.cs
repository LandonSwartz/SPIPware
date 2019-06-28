﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SPIPware.Communication;
using SPIPware.Entities; //for experiment class
using System.IO; //for file opening
using log4net;
using System.Reflection;
using SPIPware.Communication.Experiment_Parts;

namespace SPIPware
{
    /// <summary>
    /// Interaction logic for ExperimentBuilderWindow.xaml
    /// </summary>
    public partial class ExperimentBuilderWindow : Window
    {
        ExperimentArrangement experiment = new ExperimentArrangement();

        public ExperimentBuilderWindow()
        {
            InitializeComponent();

            this.DataContext = new ExperimentViewModel();

         //   Experiment newExperimet = new Experiment();

            //filling out plate thingy
            for(int i = 0; i < Properties.Settings.Default.TotalColumns; i++)
            {
                for(int j = 0; j < Properties.Settings.Default.TotalRows; j++)
                {
                    Image greySquare = new Image();

                    Grid.SetColumn(greySquare, i);
                    Grid.SetRow(greySquare, j);
                    PlateArrangementGrid.Children.Add(greySquare);
                }
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void BtnSaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingsToFile(Properties.Settings.Default.ExperimentPath);
        }

        private void BtnApplyExperiment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void XOffsetOfPlate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void YOffsetOfPlate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadiusOfWell_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OpenPastExperiments_Click(object sender, RoutedEventArgs e)
        {
            var dialog = openSaveDialog();
            SaveAsSettingsToFile(dialog);
            if (dialog != null)
            {
                Properties.Settings.Default.ExperimentPath = dialog.FileName;
            }
        }

        //to say all wells are active
        private void selectAll_Change(object sender, RoutedEventArgs e)
        {
            SelectAll_Change();

        }
        private void SelectAll_Change()
        { //will need to make experiment class
           /* foreach (Tray tray in experiment)
            {
                foreach ( in plateList)
                {
                    if (well != null)
                    {
                        if (SelectAllValue == true)
                        {
                            well.Active = true;
                        }
                        else if (SelectAllValue == false)
                        {
                            well.Active = false;
                        }
                    }
                    // Dispatcher.Invoke(() => cycle.UpdatePositionList(checkBoxes2D));
                }
            }*/

            //making method in plate classes to make all wells active then calling those for each plate in tray

        }




        #region Past SPIPware functions
        List<List<CheckBox>> checkBoxes2D = new List<List<CheckBox>>();
        List<TextBlock> textBlocks = new List<TextBlock>();


        public void UpdatePlateClick(object sender, RoutedEventArgs e)
        {
            if (spCheckboxes != null)
            {
                UpdatePlateCheckboxes();

            }
            else
            {
                _log.Error("Could not update plate checkboxes, spCheckboxes null");
            }

        }

        public void UpdatePlateCheckboxes(bool value)
        {
            if (checkBoxes2D != null)
            {
                //1D version
                //checkBoxes2D.ForEach(c => c.IsChecked = value);

                foreach (List<CheckBox> boxList in checkBoxes2D)
                {
                    foreach (CheckBox box in boxList)
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
        public void UpdatePlateCheckboxes()
        { //will redo checkboxes with expeirment builder so commenting below out FOR NOW    
            Dispatcher.Invoke(() =>
            {
                _log.Debug("Updating Plate Checkboxes");
                int numBoxes = Properties.Settings.Default.NumLocationsRow;
                spCheckboxes.Children.Clear();
                foreach (List<CheckBox> list in checkBoxes2D)
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

                    /* if (cycle.ImagePositions!= null && cycle.ImagePositions.Contains(i))
                     {
                         tempCB.IsChecked = true;
                     }
                     else
                     {
                         cbSelectAll.IsChecked = null;
                         tempCB.IsChecked = false;
                         //IsThreeState = true;
                     }*/

                    foreach (List<CheckBox> list in checkBoxes2D)
                    {
                        list.Add(tempCB);
                    }


                    TextBlock tempText = new TextBlock();
                    tempText.Text = (i + 1).ToString();
                    tempText.VerticalAlignment = VerticalAlignment.Center;
                    tempText.Margin = new Thickness(5);
                    textBlocks.Add(tempText);

                    foreach (List<CheckBox> list in checkBoxes2D)
                    {
                        foreach (CheckBox box in list)
                        {
                            stackPanel.Children.Add(box);
                        }

                    }

                    stackPanel.Children.Add(textBlocks[i]);

                    spCheckboxes.Children.Add(stackPanel);

                    //spCheckboxes.Children.Add(checkBoxes[i]);
                    //spCheckboxes.Children.Add(textBlocks[i]);

                }
                //cycle.UpdatePositionList(checkBoxes);
            });
            //checkBoxes.ForEach(c => c.Click += new RoutedEventHandler(PlateCheck_Change));
            _log.Debug("Number of plate checkboxes: " + checkBoxes2D.Count);


        }

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
            UpdatePlateCheckboxes();
       //     UpdateClrCanvas();
       //     UpdateBacklightColor();
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
        //may remove
    /*    private void UpdateTLParams()
        {
            Experiment tlExperiment = Experiment.LoadExperiment(Properties.Settings.Default.tlExperimentPath);
            GenerateCurrentParametersList(tlExperiment, TLCurrentParameters);

        }*/

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
                //UpdateTLParams();
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

            //UpdateTLParams();



        }

    }

    #endregion
}

