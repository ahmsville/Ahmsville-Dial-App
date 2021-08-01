using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    interface SOLIDWORKS_INTERFACE
    {
        string solidworks(string operationclass, string operationname);
    }

    interface FUSION360_INTERFACE
    {
        string fusion360(string operationclass, string operationname);
    }
}
