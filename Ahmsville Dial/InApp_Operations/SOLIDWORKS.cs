using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ahmsville_Dial_SOLIDWORKS_AddIn;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Ahmsville_Dial.InApp_Operations
{
    class SOLIDWORKS : SOLIDWORKS_INTERFACE
    {

        public static ObservableCollection<string> myList;
        public SOLIDWORKS()
        {
            myList = new ObservableCollection<string>();
            myList.CollectionChanged += myList_CollectionChanged;
        }

        private static SldWorks swApp;
        private static object Part;
        private static bool gotSolidworks = false;
        public static bool connectedtoSolidworksDoc = false;

        double rollmodelStep = 0.05;

        ModelDoc2 swModel = default(ModelDoc2);

        ModelViewManager swModelViewManager = default(ModelViewManager);

        ModelView swModelView = default(ModelView);

        static ModelDoc2 swDoc = null;
        static ModelView myModelView = null;
        private static dynamic addin = null;

        static SOLIDWORKS swObj = new SOLIDWORKS();
        InApp_Queue InApp_Queue_InCall = new InApp_Queue();

        // get solidworks 
        internal static SldWorks getApplication()
        {
            if (swApp == null)
            {
                try
                {
                    swApp = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as SldWorks;
                    swApp.Visible = true;

                }
                catch (Exception)
                {

                }

                return swApp;
            }

            return swApp;
        }
        public void connectToSolidworks()
        {

            PartDoc swPart = null;
            DrawingDoc swDrawing = null;
            AssemblyDoc swAssembly = null;
            bool boolstatus = false;
            int longstatus = 0;
            int longwarnings = 0;
            swDoc = ((ModelDoc2)(swApp.ActiveDoc));
            myModelView = ((ModelView)(swDoc.ActiveView));

            //get ahmsville dial addin

            addin = swApp.GetAddInObject("AhmsvilleDial.SWAddIn");

            if (addin != null)
            {
                addin.registerCallback_InSW(swObj.InApp_Queue_InCall);
            }
        }

        public static List<string> SW_InApp_Op_queue = new List<string>();


        

      
        public static bool invokeMethodsInQueue()
        {
            if (SW_InApp_Op_queue.Count > 0)
            {
                string methodname = SW_InApp_Op_queue.ElementAt(0);
                SW_InApp_Op_queue.RemoveAt(0);
                _MethodInfo _Method = swObj.GetType().GetMethod(methodname);
                try
                {
                    _Method.Invoke(swObj, null);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (SW_InApp_Op_queue.Count == 0)
            {
               
                return true;

            }
            else
            {
                return false;
            }

        }
        #region inapp functions
        public void SW_roll_xpos()
        {
            myModelView.RotateAboutCenter(-(rollmodelStep), 0);
        }
        public void SW_roll_xneg()
        {
            myModelView.RotateAboutCenter((rollmodelStep), 0);
        }
        public void SW_roll_ypos()
        {
            myModelView.RotateAboutCenter(0, -(rollmodelStep));
        }
        public void SW_roll_yneg()
        {
            myModelView.RotateAboutCenter(0, (rollmodelStep));
        }

        public void SW_zoomIn()
        {
            myModelView.ZoomByFactor(1.02);
        }

        public void SW_zoomOut()
        {
            myModelView.ZoomByFactor(0.98);
        }
        public void SW_spinCW()
        {
            myModelView.RollBy(-0.05);
        }
        public void SW_spinCCW()
        {
            myModelView.RollBy(0.05);
        }
        public void SW_move_xpos()
        {
            myModelView.TranslateBy(0.005, 0);
        }
        public void SW_move_xneg()
        {
            myModelView.TranslateBy(-0.005, 0);
        }
        public void SW_move_ypos()
        {
            myModelView.TranslateBy(0, 0.005);
        }
        public void SW_move_yneg()
        {
            myModelView.TranslateBy(0, -0.005);
        }
        public void SW_zoomToFit()
        {
            swDoc.ViewZoomtofit();
        }
        public void SW_measure()
        {
           
        }
        #endregion


        void myList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //list changed - an item was added.
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
              
                //volMethod = myList.ElementAt(0);
               // int threadid = Process.GetCurrentProcess().Id;
                //Trace.WriteLine("app" + "  " + threadid.ToString());
                //Do what ever you want to do when an item is added here...
                //the new items are available in e.NewItems
            }
        }


        public string solidworks(string operationclass, string operationname)
        {
            
            try
            {
                if (swApp != null)
                {
                    
                    connectToSolidworks();
                    SW_InApp_Op_queue.Add(operationname);
                    // _Method.Invoke(this, null);

                    myList.Add(operationname);


                }
                else
                {
                    getApplication();
                }    
               
            }
            catch (Exception)
            {

            }
           
            return operationclass + "  yes  " + operationname;
           
        }
        public bool solidworks(int G_P, float x, float y, float rad)
        {

            return true;
        }
    }

    class InApp_Queue : Ito_AhmDialApp
    {

        
        public void processInApp_Queue()
        {
            while (!SOLIDWORKS.invokeMethodsInQueue())
            {              
                
            }
        }
    }
}
