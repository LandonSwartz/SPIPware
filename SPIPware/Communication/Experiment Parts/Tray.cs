using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Communication
{
    class Tray
    {
        public Plate[,] plates; //need array size but maybe hard coded

        public Tray()
        {
            plates = new Plate[3, 4]; 
        }
    }
}
