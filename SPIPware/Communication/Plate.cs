using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication
{
    //for collection of wells
    class Plate
    {
        private int numWells; //how many wells in the plate
        public int NumWells
        {
            get { return numWells; }
            set { numWells = value; }
        }
        int numRows; //auto private because not specified
        public int NumRows
        {
            get { return length; }
            set { length = value; }
        }
        int numColumns;
        public int NumColumns
        {
            get { return numColumns; }
            set { numColumns = value; }
        }

        private int xOffset;
        public int XOffset
        {
            get { return xOffset; }
            set { xOffset = value; }
        }

        private int yOffset;
        public int YOffset
        {
            get { return yOffset; }
            set { yOffset = value; }
        }


        public Well[,] wells = new Well[3,4];//need to make variable when can

        //may need a method to initalize all the wells arrays
    }
}
