using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using System.Runtime.InteropServices;

namespace Ahmsville_Dial_SOLIDWORKS_AddIn
{
    [ComVisible(true)]
    public interface ITo_AhmDialApp
    {
        //SldWorks swApp, ModelDoc2 swDoc, ModelView myModelView
        void processInApp_Queue();
      
    }
}
