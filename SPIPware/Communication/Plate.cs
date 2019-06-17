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
        int length; //auto private because not specified
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        int distanceBtwnWells; //distance between wells on plate
        public int DistanceBtwn
        {
            get { return distanceBtwnWells; }
            set { distanceBtwnWells = value; }
        }
    }
}
