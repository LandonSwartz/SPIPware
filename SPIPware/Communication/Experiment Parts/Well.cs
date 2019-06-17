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

        private bool active; //is the well being used or not
        public bool Active { get => active; set => active = value; }


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
