using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class SpaceNavOperations : SpaceNavOperations_INTERFACE
    {


        public void up()
        {
            
        }
        public void down()
        {

        }
        public void left()
        {

        }
        public void right()
        {

        }
        public string SpaceNav(string operationclass, string operationname)
        {
            _MethodInfo _Method = this.GetType().GetMethod(operationname);
            _Method.Invoke(this, null);
            return operationclass + "  yes  " + operationname;
           
        }
        public bool SpaceNav(int G_P, float x, float y, float rad)
        {

            return true;
        }
    }
}
