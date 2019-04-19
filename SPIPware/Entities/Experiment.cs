using log4net;
using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace SPIPware.Entities
{
    [Serializable]
    public class Experiment
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory; // current folder the program is running in
        public static string DEFAULT_SETTINGS_PATH = BaseDirectory + "Resources\\DefaultSettings.json";

        public static Dictionary<string, string> friendlyNames = new Dictionary<string, string>
        {
            {"TotalPlates","Total Plates" },
            {"CurrentPlate", "Current Plate" },
            {"PlateOffset", "Plate Offset" },
            {"BetweenDistance", "Between Distance" },
            {"CameraSettingsPath", "Camera Settings" },
            {"FileName", "File Name" },
            {"SaveFolderPath", "Save Folder" },
            {"BackgroundColor", "Backlight Color" }
            //{"TlEndDate", "Time Lapse End Date" }
       

        };

        private static CycleControl cycle = CycleControl.Instance;
        private int totalPlates;
        private int currentPlate;
        private int plateOffset;
        private int betweenDistance;
        private string cameraSettingsPath;
        private string fileName;
        private string saveFolderPath;
        private bool currentPlateSave;
        private int numLocations;
        private bool selectAll;
        private int currentLocation;
        private string cameraName;
        private DateTime tlStartDate;
        private DateTime tlEndDate;
        private bool startNow;
        //private int tlInterval;
        private int tlEndInterval;
        private long tlEndIntervalType;
        private long tlIntervalType;
        private int cycleCount;
        private List<int> imagePositions;

        private Color BacklightColor;

        public int TotalPlates { get => totalPlates; set => totalPlates = value; }
        public int CurrentPlate { get => currentPlate; set => currentPlate = value; }
        public int PlateOffset { get => plateOffset; set => plateOffset = value; }
        public int BetweenDistance { get => betweenDistance; set => betweenDistance = value; }
        public string CameraSettingsPath { get => cameraSettingsPath; set => cameraSettingsPath = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string SaveFolderPath { get => saveFolderPath; set => saveFolderPath = value; }
        public bool CurrentPlateSave { get => currentPlateSave; set => currentPlateSave = value; }
        public int NumLocations { get => numLocations; set => numLocations = value; }
        public bool SelectAll { get => selectAll; set => selectAll = value; }
        public int CurrentLocation { get => currentLocation; set => currentLocation = value; }
        public string CameraName { get => cameraName; set => cameraName = value; }
        public DateTime TlStartDate { get => tlStartDate; set => tlStartDate = value; }
        public DateTime TlEndDate { get => tlEndDate; set => tlEndDate = value; }
        public bool StartNow { get => startNow; set => startNow = value; }
        public int TlEndInterval { get => tlEndInterval; set => tlEndInterval = value; }
        public long TlEndIntervalType { get => tlEndIntervalType; set => tlEndIntervalType = value; }
        public long TlIntervalType { get => tlIntervalType; set => tlIntervalType = value; }

        public Color BackgroundColor { get => BacklightColor; set => BacklightColor = value; }
        
        public List<int> ImagePositions { get => imagePositions; set => imagePositions = value; }
        public int CycleCount { get => cycleCount; set => cycleCount = value; }


        public Experiment()
        {
        }
        public static void LoadDefaults()
        { 
            Experiment experiment = LoadExperimentAndSave(DEFAULT_SETTINGS_PATH);

        }
        public void SaveExperimentToSettings()
        {
            if (this != null)
            {
                
                Properties.Settings.Default.TotalPlates = TotalPlates;
                Properties.Settings.Default.CurrentPlate = CurrentPlate;
                Properties.Settings.Default.PlateOffset = PlateOffset;
                Properties.Settings.Default.BetweenDistance = BetweenDistance;
                Properties.Settings.Default.CameraSettingsPath = CameraSettingsPath;
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.SaveFolderPath = SaveFolderPath;
                Properties.Settings.Default.CurrentPlateSave = CurrentPlateSave;
                Properties.Settings.Default.NumLocations = NumLocations;
                Properties.Settings.Default.SelectAll = SelectAll;
                Properties.Settings.Default.CurrentLocation = CurrentLocation;
                Properties.Settings.Default.CameraName = CameraName;
                Properties.Settings.Default.BacklightColor = BacklightColor;
                Properties.Settings.Default.CycleCount = CycleCount;
                //CycleControl.ImagePositions = ImagePositions;
                cycle.ImagePositions = ImagePositions;

                Properties.Settings.Default.Save();

            }

     
        }
        //Loads application settings settings into experiment
        public void LoadExperiment()
        {
            //Experiment experiment = new Experiment();
            TotalPlates = Properties.Settings.Default.TotalPlates;
            CurrentPlate = Properties.Settings.Default.CurrentPlate;
            PlateOffset = Properties.Settings.Default.PlateOffset;
            BetweenDistance = Properties.Settings.Default.BetweenDistance;
            CameraSettingsPath = Properties.Settings.Default.CameraSettingsPath;
            FileName = Properties.Settings.Default.FileName;
            SaveFolderPath = Properties.Settings.Default.SaveFolderPath;
            CurrentPlateSave = Properties.Settings.Default.CurrentPlateSave;
            NumLocations = Properties.Settings.Default.NumLocations;
            SelectAll = Properties.Settings.Default.SelectAll;
            CurrentLocation = Properties.Settings.Default.CurrentLocation;
            CameraName = Properties.Settings.Default.CameraName;
            TlStartDate = Properties.Settings.Default.tlStartDate;
            TlEndDate = Properties.Settings.Default.tlEndDate;
            StartNow = Properties.Settings.Default.StartNow;
            TlEndInterval = Properties.Settings.Default.tlEndInterval;
            TlEndIntervalType = Properties.Settings.Default.tlEndIntervalType;
            TlIntervalType = Properties.Settings.Default.tlIntervalType;
            BacklightColor = Properties.Settings.Default.BacklightColor;
            CycleCount = Properties.Settings.Default.CycleCount;
            if (cycle.ImagePositions != null)
            {
                ImagePositions = new List<int>(cycle.ImagePositions);
            }
            else
            {
                _log.Error("cycle.ImagePositions is null");
            }
        }
        public static Experiment LoadExperimentAndSave(string filePath)
        {
            Experiment experiment = LoadExperiment(filePath);
            if(experiment != null)
            {
                experiment.SaveExperimentToSettings();  
            }
            return experiment;


        }
        public static Experiment LoadExperiment(string filePath)
        {
            try
            {
                if (FileExists(filePath))
                {

                    Experiment experiment = JSONUtil.LoadJSON<Experiment>(filePath);
                    return experiment;
                }
                else
                {
                    _log.Error("Experiment file does not exist");
                    return null;
                }
            }
            catch(Exception e)
            {
                _log.Error("Unable to load experiment file: " + e);
                return null;
            }

        }
        public bool SaveExperiment( string filePath)
        {
            
            try
            {
                JSONUtil.SaveJSON(this, filePath);
                return true;
            }
            catch(Exception e)
            {
                _log.Error("Unable to save file: " + e);
                return false;
            }
               
       
        
        }
        internal static bool FileExists(string name)
        {
            return  File.Exists(name);
        }
    }
 
}
