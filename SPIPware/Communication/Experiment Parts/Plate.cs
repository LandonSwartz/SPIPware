using SPIPware.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

//ADD constructor!!!

namespace SPIPware.Communication.Experiment_Parts
{
    //for collection of wells
    public class Plate
    {
        #region Properties
        private int numWells; //how many wells in the plate in total
        //for plate array
        int numRows; //auto private because not specified
        int numColumns;
        //offsets for camera
        private int xOffset;
        private int yOffset;
        private bool active; //all wells are active
        public Well[,] wells; //could change to list of list later

        /* Notes for future...
         * 
         * Right now, the wells are based as a 2D fixed array. The hope is though
         * that in the future, it is a dynamic 2D array. But for current modes of 
         * operation, I am keeping fixed and will work in future. May make a 
         * dictionary of list or a list of lists to make it truly dynamic but 
         * only time will tell if I can learn enough c# to do it. 
         */


        public int NumWells
        {
            get { return numWells; }
            set { numWells = numRows * NumColumns; } //return rows times column
        }

        public int NumRows
        {
            get { return numRows; }
            set { numRows = value; }
        }

        public int NumColumns
        {
            get { return numColumns; }
            set { numColumns = value; }
        }

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

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Activates all wells in a plate. Returns 1 if sucessful, 0 if not
        /// </summary>
        /// <returns></returns>
        public int ActivatePlates()
        {
            for(int i =0; i < numRows; i++)
            {
                for(int j =0; j < numColumns; j++)
                {
                    this.wells[i,j].Active = true;
                }
            }

            this.active = true;

            return 1;
        }
        #endregion



        #region Constructors
        public Plate() //constructor function
        {
            //hardcoded values, will change in future
            numRows = 4; //will change to Properties.Settings.Default.numberofRows later
            numColumns = 6;
            xOffset = 1;//will change
            yOffset = 1;

            wells = new Well[numRows, numColumns]; //will need to see how this works in practice
            for(int i = 0; i < numRows; i++)
            {
                for( int j = 0; j < numColumns; j++)
                {
                    wells[i, j] = new Well();
                    //filling in coordinate of the well
                    wells[i, j].X = i;
                    wells[i, j].Y = j;
                }
            }
        }
        //Manual initializing method (if needed)
        public Plate(int NumWells, int NumRows, int NumColumns, int XOffset, int YOffset)
        {
            numWells = NumWells;
            numRows = NumRows;
            numColumns = NumColumns;
            xOffset = XOffset;
            yOffset = YOffset;
        }
        #endregion

        #region Misc
        //finding point in well array
        /*  private  ptInWellArray(xVal, yVal)
          { 

          }*/
        #endregion
    }
}
