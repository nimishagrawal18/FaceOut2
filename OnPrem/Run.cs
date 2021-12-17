using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnPrem
{
    class Run
    {
        static void Main(string[] args)
        {
            Capture CapObj = new Capture();
            CapObj.startCapture(0);  // Starting capture using Camera 0
            
            
        }
    }
}
