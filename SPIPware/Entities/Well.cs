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

        public int GetDiameter(radius)
        {
            int diameter = radius * 2;

            return diameter;
        }


        public int WellArea(radius)
        {
            double area = (radius * radius) * 3.14;

            return area;
        }
    }
}
