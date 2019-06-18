using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts

{
    class Well
    {
        #region Properties
        private int radius; //size of well
        private int diameter; //diameter of well
        private bool active; //is the well being used or not
        //for coordinates in well array of plate
        private int x;
        private int y;

        public int Radius //set and get function of well
        {
            get { return radius; }
            set { radius = value; }
        }  
        
        public int Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }

        public bool Active { get => active; set => active = value; }

        //for plate array coordinates
        public int X
        {
            get { return x;  }
            set { x = value; }
        }

       
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        #endregion

        #region Constructors
        //constructor function
        public Well() //if no parameters passed
        {
            radius = 0;
            active = false;
            x = 0;
            y = 0;
        }

        public Well(int radius, bool active, int x, int y) //manual constructor with well parameters passed
        {
            this.radius = radius;
            this.active = active;
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Misc
        public

            int GetDiameter(int radius) //maybe used for auto distance finding
            {
                int diameter = this.radius * 2;

                return diameter;
            }
        #endregion
    }
}
