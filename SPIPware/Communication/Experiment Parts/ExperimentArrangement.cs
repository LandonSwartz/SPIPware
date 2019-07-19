using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    /// <summary>
    /// Class to contain total arragement of experiment parts. Includes an array of Tray objects. 
    /// One method to activate all trays in the experiment (for quicker experiment building).
    /// Constructor makes array of Trays when initialized.
    /// </summary>
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

        #region Methods

        /// <summary>
        /// Activates all the wells in the trays of plates
        /// </summary>
        /// <returns>Returns 1 if success, 0 if fail</returns>
        public int ActivateAllTrays()
        {
            for(int i=0; i < 3; i++) //hard coded to three rn'
            {
                this.trays[i].ActivateTrays();
            }

            return 1;
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
