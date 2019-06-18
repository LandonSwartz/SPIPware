using SPIPware.Util;
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
        public Plate()
        {
            wells = new Well[numRows, numColumns]; //will need to see how this works in practice
            for(int i = 0; i < numRows; i++)
            {
                for( int j = 0; j < numColumns; j++)
                {
                    wells[i, j] = new Well();
                    wells[i, j].X = i;
                    wells[i, j].Y = j;
                }
            }
        }
        //initializing method
        public Plate(int NumWells, int NumRows, int NumColumns, int XOffset, int YOffset)
        {
            numWells = NumWells;
            numRows = NumRows;
            numColumns = NumColumns;
            xOffset = XOffset;
            yOffset = YOffset;
        }

        private int numWells; //how many wells in the plate in total
        public int NumWells
        {
            get { return numWells; }
            set { numWells = value; }
        }
        //for plate array
        int numRows; //auto private because not specified
        public int NumRows
        {
            get { return numRows; }
            set { numRows = value; }
        }
        int numColumns;
        public int NumColumns
        {
            get { return numColumns; }
            set { numColumns = value; }
        }
        //offsets for camera
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


        private Well[,] wells; //need to make variable when can

        //finding point in well array
      /*  private  ptInWellArray(xVal, yVal)
        { 

        }*/
    }
}
