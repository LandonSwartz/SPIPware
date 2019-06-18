using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication.Experiment_Parts
{
    class Tray
    {
        #region Properties
        private Plate[,] plates; //need array size but maybe hard coded
        public Plate[,] Plates
        {
            get { return plates; }
            set { plates = value; } //may need to iterate through each value
        }
        #endregion

        #region Constructor
        public Tray() //constructor
        {
            plates = new Plate[3, 4]; //right now plate array is hard coded but will figure out later how to not

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
        //array initalizing function attempt, may salavage later
        /*  T[,] InitializeArray<T,S>(int rows, int columns) where Plate : new()
          {
              T[,] array = new T[rows, columns];
              for (int i = 0; i < length; ++i)
              {
                  for(int j = 0; j < columns; j++)
                  {
                      array[i, j] = new Plate();
                  }

              }

              return array;
          }*/
    }
}
