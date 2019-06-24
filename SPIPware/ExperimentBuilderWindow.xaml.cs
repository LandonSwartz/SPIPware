using System;
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

namespace SPIPware
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ExperimentBuilderWindow : Window
    {
        public ExperimentBuilderWindow()
        {
            InitializeComponent();

          //  Experiment newExperiment; //don't know if needed
        } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void BtnSaveExperiment_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
