using log4net;
using SPIPware.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using SPIPware.Communication.Experiment_Parts;

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
            {"TotalRows","Total Rows" },
            {"Total Columns", "Total Columns" },
        //    {"CurrentPlate", "Current Plate" },
            {"PlateXOffset", "Plate X Offset" },
            {"PlateYOffset", "Plate Y Offset" },
        //    {"BetweenDistance", "Between Distance" },
            {"CameraSettingsPath", "Camera Settings" },
            {"FileName", "File Name" },
            {"SaveFolderPath", "Save Folder" },
            {"BackgroundColor", "Backlight Color" }
            //{"TlEndDate", "Time Lapse End Date" }
       

        };

        private static CycleControl cycle = CycleControl.Instance;
        // private int totalPlates;
        private int platesPerRow;
        private int platesPerColumn;
     //   private int[] currentPlate;
        private int wellXoffset;
        private int wellYoffset;
        private int plateXOffset;
        private int plateYOffset;
        private int betweenDistance;
        private string cameraSettingsPath;
        private string fileName;
        private string saveFolderPath;
        private bool currentPlateSave;
        private int numLocations; //may need to make array
        private bool selectAll;
        private int currentLocationX;
        private int currentLocationY;
        private int currentLocationZ;
        private string cameraName;
        private DateTime tlStartDate;
        private DateTime tlEndDate;
        private bool startNow;
        //private int tlInterval;
        private int tlEndInterval;
        private long tlEndIntervalType;
        private long tlIntervalType;
        private int cycleCount;
        private int[] imagePositionsX;
        private int[] imagePositionsY;
        private int numTrays;
        private int wellRadius;
        private string experimentTitle; //for title of experiment
        private int totalColumns;
        private int totalRows;

        private Color BacklightColor;

       // public int TotalPlates { get => totalPlates; set => totalPlates = value; }
        public int PlatesPerRow { get => platesPerRow; set => platesPerRow = value; }
        public int PlatesPerColumn{ get => platesPerColumn; set => platesPerColumn = value; }
      //  public int[] CurrentPlate { get => currentPlate; set => currentPlate = value; } //current plate is array of x then y
        private int PlateXOffset { get => plateXOffset; set => plateXOffset = value; }
        private int PlateYOffset { get => plateYOffset; set => plateYOffset = value; }
        public int WellXoffset { get => wellXoffset; set => wellXoffset = value; }
        public int WellYoffset { get => wellYoffset; set => wellYoffset = value; }
        public int BetweenDistance { get => betweenDistance; set => betweenDistance = value; }
        public string CameraSettingsPath { get => cameraSettingsPath; set => cameraSettingsPath = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string SaveFolderPath { get => saveFolderPath; set => saveFolderPath = value; }
        public bool CurrentPlateSave { get => currentPlateSave; set => currentPlateSave = value; }
        public int NumLocations { get => numLocations; set => numLocations = value; }
        public bool SelectAll { get => selectAll; set => selectAll = value; }
        public int CurrentLocationX { get => currentLocationX; set => currentLocationX = value; }
        public int CurrentLocationY { get => currentLocationY; set => currentLocationY = value; }
        public int CurrentLocationZ { get => currentLocationZ; set => currentLocationZ = value; }
        public string CameraName { get => cameraName; set => cameraName = value; }
        public DateTime TlStartDate { get => tlStartDate; set => tlStartDate = value; }
        public DateTime TlEndDate { get => tlEndDate; set => tlEndDate = value; }
        public bool StartNow { get => startNow; set => startNow = value; }
        public int TlEndInterval { get => tlEndInterval; set => tlEndInterval = value; }
        public long TlEndIntervalType { get => tlEndIntervalType; set => tlEndIntervalType = value; }
        public long TlIntervalType { get => tlIntervalType; set => tlIntervalType = value; }
        public int NumTrays { get => numTrays; set => numTrays = value; }
        public int WellRadius { get => wellRadius; set => wellRadius = value; }
        public string ExperimentTitle { get => experimentTitle; set => experimentTitle = value; }
        public int TotalColumns { get => totalColumns; set => totalColumns = value;}
        public int TotalRows { get => totalRows; set => totalRows = value; }

        public Color BackgroundColor { get => BacklightColor; set => BacklightColor = value; }
        
        public int[] ImagePositionsX { get => imagePositionsX; set => imagePositionsX = value; }
        public int[] ImagePositionsY { get => imagePositionsY; set => imagePositionsY = value; }
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
                
                Properties.Settings.Default.TotalRows = PlatesPerRow;
                Properties.Settings.Default.TotalColumns = PlatesPerColumn;
            //    Properties.Settings.Default.CurrentPlate = CurrentPlate;
                Properties.Settings.Default.PlateXOffset = PlateXOffset;
                Properties.Settings.Default.PlateYOffset = PlateYOffset;
                Properties.Settings.Default.WellXOffset = WellXoffset;
                Properties.Settings.Default.WellYOffset = WellYoffset;
                Properties.Settings.Default.XBetweenDistance = BetweenDistance;
                Properties.Settings.Default.CameraSettingsPath = CameraSettingsPath;
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.SaveFolderPath = SaveFolderPath;
                Properties.Settings.Default.CurrentPlateSave = CurrentPlateSave;
                Properties.Settings.Default.NumLocationsRow = NumLocations;
                Properties.Settings.Default.SelectAll = SelectAll;
                Properties.Settings.Default.CurrentLocationX = CurrentLocationX;
                Properties.Settings.Default.CurrentLocationY = CurrentLocationY;
                Properties.Settings.Default.CurrentLocationZ = CurrentLocationZ;
                Properties.Settings.Default.CameraName = CameraName;
                Properties.Settings.Default.BacklightColor = BacklightColor;
                Properties.Settings.Default.CycleCount = CycleCount;
                //CycleControl.ImagePositions = ImagePositions;
                cycle.ImagePositionsX = ImagePositionsX;
                cycle.ImagePositionsY = imagePositionsY;
                Properties.Settings.Default.NumberOfTrays = NumTrays;
                Properties.Settings.Default.RadiusOfWell = WellRadius;
                Properties.Settings.Default.TotalColumns = TotalColumns;
                Properties.Settings.Default.TotalRows = TotalRows;


                Properties.Settings.Default.Save();

            }

     
        }
        //Loads application settings settings into experiment
        public void LoadExperiment()
        {
            //Experiment experiment = new Experiment();
            PlatesPerRow = Properties.Settings.Default.TotalRows;
            PlatesPerColumn = Properties.Settings.Default.TotalColumns;
            //CurrentPlate = Properties.Settings.Default.CurrentPlate;
            PlateXOffset = Properties.Settings.Default.PlateXOffset;
            PlateYOffset = Properties.Settings.Default.PlateYOffset;
            WellXoffset = Properties.Settings.Default.WellXOffset;
            WellYoffset = Properties.Settings.Default.WellYOffset;
            BetweenDistance = Properties.Settings.Default.XBetweenDistance;
            CameraSettingsPath = Properties.Settings.Default.CameraSettingsPath;
            FileName = Properties.Settings.Default.FileName;
            SaveFolderPath = Properties.Settings.Default.SaveFolderPath;
            CurrentPlateSave = Properties.Settings.Default.CurrentPlateSave;
            NumLocations = Properties.Settings.Default.NumLocationsRow;
            SelectAll = Properties.Settings.Default.SelectAll;
            CurrentLocationX = Properties.Settings.Default.CurrentLocationX;
            CurrentLocationY = Properties.Settings.Default.CurrentLocationY;
            CurrentLocationZ = Properties.Settings.Default.CurrentLocationZ;
            CameraName = Properties.Settings.Default.CameraName;
            TlStartDate = Properties.Settings.Default.tlStartDate;
            TlEndDate = Properties.Settings.Default.tlEndDate;
            StartNow = Properties.Settings.Default.StartNow;
            TlEndInterval = Properties.Settings.Default.tlEndInterval;
            TlEndIntervalType = Properties.Settings.Default.tlEndIntervalType;
            TlIntervalType = Properties.Settings.Default.tlIntervalType;
            BacklightColor = Properties.Settings.Default.BacklightColor;
            CycleCount = Properties.Settings.Default.CycleCount;
            NumTrays = Properties.Settings.Default.NumberOfTrays;
            WellRadius = Properties.Settings.Default.RadiusOfWell;
            TotalRows = Properties.Settings.Default.TotalRows;
            TotalColumns = Properties.Settings.Default.TotalColumns;

            if (cycle.ImagePositionsX != null)
            {
                ImagePositionsX = (cycle.ImagePositionsX);
            }
            else
            {
                _log.Error("cycle.ImagePositionsX is null");
            }

            if (cycle.ImagePositionsY != null)
            {
                ImagePositionsY = (cycle.ImagePositionsY);
            }
            else
            {
                _log.Error("cycle.ImagePositionsY is null");
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
