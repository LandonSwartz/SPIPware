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

        //for plate array coordinates
        private int x;
        public int X
        {
            get { return x;  }
            set { x = value; }
        }

        private int y;
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public

            int GetDiameter(int radius) //maybe used for auto distance finding
            {
                int diameter = this.radius * 2;

                return diameter;
            }
    }
}
