using System.ComponentModel;
using PropertyChanged;

namespace SPIPware
{
    [AddINotifyPropertyChangedInterface]
    public class ExperimentViewModel : INotifyPropertyChanged
    {
        //event handler
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Properties
      //  private int radius = Properties.Settings.Default.RadiusOfWell;
      //  private int numRows = Properties.Settings.Default.NumLocationsRow;
      //  private int numColumns = Properties.Settings.Default.NumLocationsColumns;

        public int Radius { get; set; }
        public int NumRows { get; set; }
        public int NumColumns { get; set; }

        #endregion
    }
}
