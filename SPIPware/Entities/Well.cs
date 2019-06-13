using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Entities
{
    class Well
    {
        public int radius; //size of well
        public int diameter; //diameter of well
        public bool filled; //is the well being used or not

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
