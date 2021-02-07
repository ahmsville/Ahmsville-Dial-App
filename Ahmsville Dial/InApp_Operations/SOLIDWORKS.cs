using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class SOLIDWORKS : SOLIDWORKS_INTERFACE
    {

        private static SldWorks swApp;
        private static object Part;
        private static bool gotSolidworks = false;

        // get solidworks 
        internal static SldWorks getApplication()
        {
            if (swApp == null)
            {
                try
                {
                    swApp = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as SldWorks;
                    swApp.Visible = true;


                    

                    gotSolidworks = true;
                   
                }
                catch (Exception)
                {
                    gotSolidworks = false;
                }
                //SldWorks swApp = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as SldWorks;



                return swApp;
            }

            return swApp;
        }

        public void SW_rotatemodel_xpos()
        {
            getApplication();
            if (gotSolidworks)  //got the solidworks instance
            {
                ModelDoc2 swDoc = null;
                PartDoc swPart = null;
                DrawingDoc swDrawing = null;
                AssemblyDoc swAssembly = null;
                bool boolstatus = false;
                int longstatus = 0;
                int longwarnings = 0;
                
                swDoc = ((ModelDoc2)(swApp.ActiveDoc));
                ModelView myModelView = null;
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.36766451524043131, 0.01545045567339246);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.4948008429403935, -0.023175683510088689);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.051541754472957661, 0);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.017180584824319219, 0);
                // 
                // Roll View
                ModelView swModelView = null;
                swModelView = ((ModelView)(swDoc.ActiveView));
                swModelView.RollBy(0);


            }
        }
        public void SW_rotatemodel_xneg()
        {

        }
        public void SW_rotatemodel_ypos()
        {

        }
        public void SW_rotatemodel_yneg()
        {

        }
        public string solidworks(string operationclass, string operationname)
        {
            _MethodInfo _Method = this.GetType().GetMethod(operationname);
            _Method.Invoke(this, null);
            return operationclass + "  yes  " + operationname;
           
        }
        public bool solidworks(int G_P, float x, float y, float rad)
        {

            return true;
        }
    }
}
