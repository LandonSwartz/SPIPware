using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;

namespace SPIPware.Entities
{
    [Serializable]
    class Experiment
    {
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
        private Color BacklightColor;

        public static void LoadExperimentToSettings(string filePath)
        {
            Experiment experiment = LoadExperiment(filePath);
            if(experiment != null)
            {
                Properties.Settings.Default.TotalPlates = experiment.totalPlates;
                Properties.Settings.Default.CurrentPlate = experiment.currentPlate;
                Properties.Settings.Default.PlateOffset = experiment.plateOffset;
                Properties.Settings.Default.BetweenDistance = experiment.betweenDistance;
                Properties.Settings.Default.CameraSettingsPath = experiment.cameraSettingsPath;
                Properties.Settings.Default.FileName = experiment.fileName;
                Properties.Settings.Default.SaveFolderPath = experiment.saveFolderPath;
                Properties.Settings.Default.CurrentPlateSave = experiment.currentPlateSave;
                Properties.Settings.Default.NumLocations = experiment.numLocations;
                Properties.Settings.Default.SelectAll = experiment.selectAll;
                Properties.Settings.Default.CurrentLocation = experiment.currentLocation;
                Properties.Settings.Default.CameraName = experiment.cameraName;
                Properties.Settings.Default.BacklightColor = experiment.BacklightColor;
                //Properties.Settings.Default.tlStartDate = experiment.tlStartDate;
                //Properties.Settings.Default.tlEndDate = experiment.tlEndDate;
                //Properties.Settings.Default.StartNow = experiment.startNow;
                //Properties.Settings.Default.tlEndInterval = experiment.tlEndInterval;
                //Properties.Settings.Default.tlEndIntervalType = experiment.tlEndIntervalType;
                //Properties.Settings.Default.tlIntervalType = experiment.tlIntervalType;

                Properties.Settings.Default.Save();
            }
     
        }
        public static void CreateExperimentFromSettings(string filePath)
        {
            Experiment experiment = new Experiment();
            experiment.totalPlates = Properties.Settings.Default.TotalPlates;
            experiment.currentPlate = Properties.Settings.Default.CurrentPlate;
            experiment.plateOffset = Properties.Settings.Default.PlateOffset;
            experiment.betweenDistance = Properties.Settings.Default.BetweenDistance;
            experiment.cameraSettingsPath = Properties.Settings.Default.CameraSettingsPath;
            experiment.fileName = Properties.Settings.Default.FileName;
            experiment.saveFolderPath = Properties.Settings.Default.SaveFolderPath;
            experiment.currentPlateSave = Properties.Settings.Default.CurrentPlateSave;
            experiment.numLocations = Properties.Settings.Default.NumLocations;
            experiment.selectAll = Properties.Settings.Default.SelectAll;
            experiment.currentLocation = Properties.Settings.Default.CurrentLocation;
            experiment.cameraName = Properties.Settings.Default.CameraName;
            experiment.tlStartDate = Properties.Settings.Default.tlStartDate;
            experiment.tlEndDate = Properties.Settings.Default.tlEndDate;
            experiment.startNow = Properties.Settings.Default.StartNow;
            experiment.tlEndInterval = Properties.Settings.Default.tlEndInterval;
            experiment.tlEndIntervalType = Properties.Settings.Default.tlEndIntervalType;
            experiment.tlIntervalType = Properties.Settings.Default.tlIntervalType;
            experiment.BacklightColor = Properties.Settings.Default.BacklightColor;

            SaveExperiment(experiment, filePath);
        }
        private static Experiment LoadExperiment(string filePath)
        {
            if (FileExists(filePath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filePath,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
                Experiment experiment = (Experiment)formatter.Deserialize(stream);
                stream.Close();
                return experiment;
            }
            else
            {
                return null;
            }

        }
        private static bool SaveExperiment(Experiment experiment, string filePath)
        {
            
                IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream(filePath,
                                                        FileMode.Create,
                                                        FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, experiment);
                stream.Close();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Unable to save file: " + e);
                return false;
            }
               
       
        
        }
        internal static bool FileExists(string name)
        {
            return  File.Exists(name);
        }
    }
 
}
