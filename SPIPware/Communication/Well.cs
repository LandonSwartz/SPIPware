using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication
{
    class Well
    {
        private int radius; //size of well
        public int Radius //set and get function of well
        {
            get { return radius; }
            set { radius = value; }
        }  
        
        private int diameter; //diameter of well
        public int Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }

        public bool filled; //is the well being used or not

        private string contents; //what is in the well?
        public string Contents
        {
            get { return contents; }
            set { } //may need to update in future
        }

        public

            int GetDiameter(int radius)
            {
                int diameter = radius * 2;

                return diameter;
            }


            double WellArea(int radius)
            {
                double area = (radius * radius) * 3.14;

                return area;
            }
    }
}
