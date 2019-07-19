using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    /// <summary>
    /// Tray class is a collection of Plate objects. Basically a simplified Plate class.
    /// </summary>
    public class Tray
    {
        #region Properties
        private Plate[,] plates; //need array size but maybe hard coded
        private int numRows;
        private int numColumns;


        public Plate[,] Plates
        {
            get { return plates; }
            set { plates = value; } //may need to iterate through each value
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


        #endregion

        #region Methods
        /// <summary>
        /// Activates all plates on a tray. Returns 1 when sucessful
        /// </summary>
        /// <returns></returns>
        public int ActivateTrays()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    this.plates[i, j].ActivatePlates();
                }
            }


            return 1;
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
