using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    public class Tray
    {
        #region Properties
        private Plate[,] plates; //need array size but maybe hard coded
        private int numRow;
        private int numColumns;


        public Plate[,] Plates
        {
            get { return plates; }
            set { plates = value; } //may need to iterate through each value
        }
        public int NumRows
        {
            get { return numRow; }
            set { numRow = value; }
        }
        public int NumColumns
        {
            get { return numColumns; }
            set { numColumns = value; }
        }


        #endregion

        #region Constructor
        public Tray() //constructor
        {
            plates = new Plate[3, 4]; //right now plate array is hard coded but will put numRows and columns later

            for(int i = 0; i < 3; i++)
            {
                for(int j =0; j < 4; j++)
                {
                    plates[i, j] = new Plate();
                }
            }
        }
        #endregion

        #region Misc
        
        #endregion
    }
}
