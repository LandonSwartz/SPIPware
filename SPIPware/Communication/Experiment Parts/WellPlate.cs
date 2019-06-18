using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{ //Special case class where the well is the actual plate instead of wells in plate
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
            xOffset = 1; //will chnage later
            yOffset = 1;
        }
        #endregion

    }
}
