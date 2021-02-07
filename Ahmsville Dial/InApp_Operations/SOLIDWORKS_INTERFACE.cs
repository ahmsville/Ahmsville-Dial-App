using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    interface SOLIDWORKS_INTERFACE
    {
        bool solidworks(int G_P, float x, float y, float rad);
        string solidworks(string operationclass, string operationname);
    }
}
