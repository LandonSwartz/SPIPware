using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    class ExperimentArrangement
    {
        #region Properties
        private Tray[] trays;

        public Tray[] Trays
        {
            get => trays;
            set => trays = value;
        }
        #endregion


        #region Constructors
        public ExperimentArrangement()
        {
            Trays = new Tray[3];
            
            for(int i =0; i < 3; i++)
            {
                Trays[i] = new Tray();
            }
        }

        #endregion
    }
}
