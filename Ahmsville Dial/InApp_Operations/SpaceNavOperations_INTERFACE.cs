using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    interface SpaceNavOperations_INTERFACE
    {
        bool SpaceNav(int G_P, float x, float y, float rad);
        string SpaceNav(string operationclass, string operationname);
    }
}
