using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    /// <summary>
    /// Special case class where the well is the actual plate of the experiment. Inherits the well class
    /// then manual has some of the Plate attributes given to it.
    /// </summary>
    class WellPlate : Well //inheritance bby
    {
        #region Properites
        //offsets for camera
        private int xOffset;
        private int yOffset;
        public int XOffset
        {
            get { return xOffset; }
            set { xOffset = value; }
        }

        public int YOffset
        {
            get { return yOffset; }
            set { yOffset = value; }
        }
        #endregion

        #region Constructors
        public WellPlate()
        {
            xOffset = 1; //will change later
            yOffset = 1;
        }
        #endregion

    }
}
