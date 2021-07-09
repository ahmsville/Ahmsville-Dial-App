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
using System.Timers;

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

        public static double[] inGyrodata = { 0, 0, 0 };
        public static double[] inPlanedata = { 0, 0, 0 };

        public static double[] previnGyrodata = { 0, 0, 0 };
        public static double[] previnPlanedata = { 0, 0, 0 };

        public static int activegyroQuadrant = 0;
        public static int prevgyroQuadrant = 0;

        public static int activeplaneQuadrant = 0;
        public static int prevplaneQuadrant = 0;


        public static double mingyroradii = 0;
        public static double maxgyroradii = 0;

        public static double minplaneradii = 0;
        public static double maxplaneradii = 0;

        public static bool processedGyro = true;
        public static bool processedPlane = true;

        public static double deltaX = 0;
        public static double deltaY = 0;

        public static double acumgyrodeltaX = 0;
        public static double acumgyrodeltaY = 0;

        public static double acumplanedeltaX = 0;
        public static double acumplanedeltaY = 0;

        public static int fixedgyroposcounter = 0;
        public static bool gyrolocked = false;

        public static int fixedplaneposcounter = 0;
        public static bool planelocked = false;

        public static bool orientationlock = false;

        public static string activeFunctionID = "";

        double rollmodelStep = 0.05;

        ModelDoc2 swModel = default(ModelDoc2);

        ModelViewManager swModelViewManager = default(ModelViewManager);

        ModelView swModelView = default(ModelView);

        static ModelDoc2 swDoc = null;
        static ModelView myModelView = null;
        private static dynamic addin = null;

        public static SOLIDWORKS swObj = new SOLIDWORKS();
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
        public void connectToSolidworks(Ito_AhmDialApp callbackfunction, string funcid)
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
                addin.registerCallback_InSW(callbackfunction, funcid);
                activeFunctionID = funcid;
            }

        }

        public static List<string> SW_InApp_Op_queue = new List<string>();
        public static Timer spacenavMasterTimer;
        public static Timer spacenavGyroTimer;
        public static Timer spacenavPlaneTimer;
        public static Timer defaultAddinTimer;

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

            return defaultAddinTimer.Enabled;
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

        public void SW_auto_orientationLock()
        {
            orientationlock = !orientationlock;

            if (orientationlock)
            {

                //System.Console.WriteLine("Auto locking enabled");
            }
            else
            {
                prevgyroQuadrant = 0;
                prevplaneQuadrant = 0;
                //System.Console.WriteLine("Auto locking disabled");
            }
        }
        #endregion

        private void SetTimer()
        {
            // Create a timer for com port query
            defaultAddinTimer = new System.Timers.Timer(5000);
            defaultAddinTimer.AutoReset = true;
            defaultAddinTimer.Enabled = true;
            // Hook up the Elapsed event for the timer. 
            defaultAddinTimer.Elapsed += defaultAddinTimerEvent;

        }
        private void SetMasterTimer()
        {
            // Create a timer for com port query
            spacenavMasterTimer = new System.Timers.Timer(5000);
            spacenavMasterTimer.AutoReset = true;
            spacenavMasterTimer.Enabled = true;
            // Hook up the Elapsed event for the timer. 
            spacenavMasterTimer.Elapsed += spacenavMasterTimerEvent;

        }
        private void SetGyroTimer()
        {
            // Create a timer for com port query
            spacenavGyroTimer = new System.Timers.Timer(700);
            spacenavGyroTimer.AutoReset = true;
            spacenavGyroTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            spacenavGyroTimer.Elapsed += spacenavGyroTimerEvent;

        }

        private void SetPlaneTimer()
        {
            // Create a timer for com port query
            spacenavPlaneTimer = new System.Timers.Timer(1000);
            spacenavPlaneTimer.AutoReset = true;
            spacenavPlaneTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            spacenavPlaneTimer.Elapsed += spacenavPlaneTimerEvent;

        }

        private void defaultAddinTimerEvent(Object source, ElapsedEventArgs e)
        {
            //System.Console.WriteLine("timer elapsed");
            defaultAddinTimer.Enabled = false;
        }

        private void spacenavMasterTimerEvent(Object source, ElapsedEventArgs e)
        {
            spacenavMasterTimer.Enabled = false;
            //System.Console.WriteLine("Master timer elapsed");
        }
        private void spacenavGyroTimerEvent(Object source, ElapsedEventArgs e)
        {
            gyrolocked = false;
            fixedgyroposcounter = 0;
            prevgyroQuadrant = 0;
            spacenavGyroTimer.Enabled = false;
            //System.Console.WriteLine("gyro timer elapsed");
        }
        private void spacenavPlaneTimerEvent(Object source, ElapsedEventArgs e)
        {
            planelocked = false;
            fixedplaneposcounter = 0;
            prevplaneQuadrant = 0;
            //System.Console.WriteLine("plane timer elapsed");
            spacenavPlaneTimer.Enabled = false;

        }

        public static bool RotateModel()
        {

            //gyro control
            if (!processedGyro)
            {
                int maxgyrocnt = 5;
                double jitterrange = 0.008;
                double fixedrange = 0.02;
                double gyrodamprate = 0.2;
                if (inGyrodata[0] > 0 && inGyrodata[1] > 0)//Q1
                {
                    //System.Console.WriteLine(">>>Q1 " + previnGyrodata[0].ToString() + " , " + previnGyrodata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activegyroQuadrant = 1;
                    if (activegyroQuadrant == prevgyroQuadrant)
                    {
                        deltaX = inGyrodata[0] - previnGyrodata[0];
                        deltaY = inGyrodata[1] - previnGyrodata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                        fixedgyroposcounter = 0;
                        acumgyrodeltaX = 0;
                        acumgyrodeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q1 " + inGyrodata[0].ToString() + " , " + inGyrodata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    deltaX = swObj.dampChange(deltaX, gyrodamprate, 30);
                    deltaY = swObj.dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrange || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > jitterrange || Math.Abs(acumgyrodeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (!gyrolocked)
                        {
                            myModelView.RotateAboutCenter(deltaY * (-1), deltaX);
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (!gyrolocked)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    spacenavGyroTimer.Enabled = true;
                                    spacenavGyroTimer.Stop();
                                    gyrolocked = true;
                                    spacenavGyroTimer.Start();
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                            
                        }
                        else
                        {
                            fixedgyroposcounter = 0;
                        }
                    }



                    prevgyroQuadrant = activegyroQuadrant;
                }

                else if (inGyrodata[0] < 0 && inGyrodata[1] > 0) //Q2
                {
                    //System.Console.WriteLine(">>>Q2 " + previnGyrodata[0].ToString() + " , " + previnGyrodata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activegyroQuadrant = 2;
                    if (activegyroQuadrant == prevgyroQuadrant)
                    {
                        deltaX = previnGyrodata[0] - inGyrodata[0];
                        deltaY = inGyrodata[1] - previnGyrodata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                        fixedgyroposcounter = 0;
                        acumgyrodeltaX = 0;
                        acumgyrodeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q2 " + inGyrodata[0].ToString() + " , " + inGyrodata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, gyrodamprate, 30);
                    deltaY = swObj.dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrange || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > jitterrange || Math.Abs(acumgyrodeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (!gyrolocked)
                        {
                            myModelView.RotateAboutCenter(deltaY * (-1), deltaX * (-1));
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (!gyrolocked)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    spacenavGyroTimer.Enabled = true;
                                    spacenavGyroTimer.Stop();
                                    gyrolocked = true;
                                    spacenavGyroTimer.Start();
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                        }
                        else
                        {
                            fixedgyroposcounter = 0;
                        }
                    }

                    prevgyroQuadrant = activegyroQuadrant;
                }

                else if (inGyrodata[0] < 0 && inGyrodata[1] < 0) //Q3
                {
                    //System.Console.WriteLine(">>>Q3 " + previnGyrodata[0].ToString() + " , " + previnGyrodata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activegyroQuadrant = 3;
                    if (activegyroQuadrant == prevgyroQuadrant)
                    {
                        deltaX = previnGyrodata[0] - inGyrodata[0];
                        deltaY = previnGyrodata[1] - inGyrodata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                        fixedgyroposcounter = 0;
                        acumgyrodeltaX = 0;
                        acumgyrodeltaY = 0;
                    }


                    //System.Console.WriteLine(">=>Q3 " + inGyrodata[0].ToString() + " , " + inGyrodata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, gyrodamprate, 30);
                    deltaY = swObj.dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrange || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > jitterrange || Math.Abs(acumgyrodeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (!gyrolocked)
                        {
                            myModelView.RotateAboutCenter(deltaY, deltaX * (-1));
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                            {
                                spacenavGyroTimer.Enabled = true;
                                spacenavGyroTimer.Stop();
                                gyrolocked = true;
                                spacenavGyroTimer.Start();
                                //System.Console.WriteLine("gyro locked");
                            }
                            fixedgyroposcounter += 1;
                        }
                        else
                        {
                            fixedgyroposcounter = 0;
                        }
                    }

                    prevgyroQuadrant = activegyroQuadrant;
                }
                else if (inGyrodata[0] > 0 && inGyrodata[1] < 0) //Q4
                {
                    //System.Console.WriteLine(">>>Q4 " + previnGyrodata[0].ToString() + " , " + previnGyrodata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activegyroQuadrant = 4;
                    if (activegyroQuadrant == prevgyroQuadrant)
                    {
                        deltaX = inGyrodata[0] - previnGyrodata[0];
                        deltaY = previnGyrodata[1] - inGyrodata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnGyrodata[i] = inGyrodata[i];
                        }
                        fixedgyroposcounter = 0;
                        acumgyrodeltaX = 0;
                        acumgyrodeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q4 " + inGyrodata[0].ToString() + " , " + inGyrodata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, gyrodamprate, 30);
                    deltaY = swObj.dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrange || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > jitterrange || Math.Abs(acumgyrodeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (!gyrolocked)
                        {
                            myModelView.RotateAboutCenter(deltaY, deltaX);
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (!gyrolocked)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    spacenavGyroTimer.Enabled = true;
                                    spacenavGyroTimer.Stop();
                                    gyrolocked = true;
                                    spacenavGyroTimer.Start();
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                        }
                        else
                        {
                            fixedgyroposcounter = 0;
                        }
                    }

                    prevgyroQuadrant = activegyroQuadrant;
                }

                processedGyro = true;


            }
            ///////////////////////////////////////


            //planar control
            if (!processedPlane)
            {
                int maxplanecnt = 4;
                double jitterrange = 0.01;
                double jitterrangetranslate = 0.01;
                double fixedrange = 0.02;
                double planedamprate = 0.07;
                if (inPlanedata[0] > 0 && inPlanedata[1] > 0)//Q1
                {
                    //System.Console.WriteLine(">>>Q1 " + previnPlanedata[0].ToString() + " , " + previnPlanedata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activeplaneQuadrant = 1;
                    if (activeplaneQuadrant == prevplaneQuadrant)
                    {
                        deltaX = inPlanedata[0] - previnPlanedata[0];
                        deltaY = inPlanedata[1] - previnPlanedata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                        fixedplaneposcounter = 0;
                        acumplanedeltaX = 0;
                        acumplanedeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q1 " + inPlanedata[0].ToString() + " , " + inPlanedata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    deltaX = swObj.dampChange(deltaX, (planedamprate * 0.5), 30);
                    deltaY = swObj.dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrangetranslate || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > jitterrangetranslate || Math.Abs(acumplanedeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (!planelocked)
                        {
                            myModelView.ZoomByFactor(1 + (deltaY * (-1)));
                            myModelView.TranslateBy(deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (!planelocked)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    spacenavPlaneTimer.Enabled = true;
                                    spacenavPlaneTimer.Stop();
                                    planelocked = true;
                                    spacenavPlaneTimer.Start();
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }

                        }
                        else
                        {
                            fixedplaneposcounter = 0;
                        }
                    }



                    prevplaneQuadrant = activeplaneQuadrant;
                }

                else if (inPlanedata[0] < 0 && inPlanedata[1] > 0) //Q2
                {
                    //System.Console.WriteLine(">>>Q2 " + previnPlanedata[0].ToString() + " , " + previnPlanedata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activeplaneQuadrant = 2;
                    if (activeplaneQuadrant == prevplaneQuadrant)
                    {
                        deltaX = previnPlanedata[0] - inPlanedata[0];
                        deltaY = inPlanedata[1] - previnPlanedata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                        fixedplaneposcounter = 0;
                        acumplanedeltaX = 0;
                        acumplanedeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q2 " + inPlanedata[0].ToString() + " , " + inPlanedata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, (planedamprate * 0.5), 30);
                    deltaY = swObj.dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrangetranslate || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > jitterrangetranslate || Math.Abs(acumplanedeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (!planelocked)
                        {
                            myModelView.ZoomByFactor(1 + (deltaY * (-1)));
                            myModelView.TranslateBy((-1) * deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (!planelocked)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    spacenavPlaneTimer.Enabled = true;
                                    spacenavPlaneTimer.Stop();
                                    planelocked = true;
                                    spacenavPlaneTimer.Start();
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                        }
                        else
                        {
                            fixedplaneposcounter = 0;
                        }
                    }

                    prevplaneQuadrant = activeplaneQuadrant;
                }

                else if (inPlanedata[0] < 0 && inPlanedata[1] < 0) //Q3
                {
                    //System.Console.WriteLine(">>>Q3 " + previnPlanedata[0].ToString() + " , " + previnPlanedata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activeplaneQuadrant = 3;
                    if (activeplaneQuadrant == prevplaneQuadrant)
                    {
                        deltaX = previnPlanedata[0] - inPlanedata[0];
                        deltaY = previnPlanedata[1] - inPlanedata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                        fixedplaneposcounter = 0;
                        acumplanedeltaX = 0;
                        acumplanedeltaY = 0;
                    }


                    //System.Console.WriteLine(">=>Q3 " + inPlanedata[0].ToString() + " , " + inPlanedata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, (planedamprate * 0.5), 30);
                    deltaY = swObj.dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrangetranslate || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > jitterrangetranslate || Math.Abs(acumplanedeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (!planelocked)
                        {
                            myModelView.ZoomByFactor(1 + deltaY);
                            myModelView.TranslateBy((-1) * deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (!planelocked)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    spacenavPlaneTimer.Enabled = true;
                                    spacenavPlaneTimer.Stop();
                                    planelocked = true;
                                    spacenavPlaneTimer.Start();
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                        }
                        else
                        {
                            fixedplaneposcounter = 0;
                        }
                    }

                    prevplaneQuadrant = activeplaneQuadrant;
                }
                else if (inPlanedata[0] > 0 && inPlanedata[1] < 0) //Q4
                {
                    //System.Console.WriteLine(">>>Q4 " + previnPlanedata[0].ToString() + " , " + previnPlanedata[1].ToString());
                    deltaX = 0;
                    deltaY = 0;
                    activeplaneQuadrant = 4;
                    if (activeplaneQuadrant == prevplaneQuadrant)
                    {
                        deltaX = inPlanedata[0] - previnPlanedata[0];
                        deltaY = previnPlanedata[1] - inPlanedata[1];
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            previnPlanedata[i] = inPlanedata[i];
                        }
                        fixedplaneposcounter = 0;
                        acumplanedeltaX = 0;
                        acumplanedeltaY = 0;
                    }



                    //System.Console.WriteLine(">=>Q4 " + inPlanedata[0].ToString() + " , " + inPlanedata[1].ToString());
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());


                    deltaX = swObj.dampChange(deltaX, (planedamprate * 0.5), 30);
                    deltaY = swObj.dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > jitterrangetranslate || Math.Abs(deltaY) > jitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > jitterrangetranslate || Math.Abs(acumplanedeltaY) > jitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (!planelocked)
                        {
                            myModelView.ZoomByFactor(1 + deltaY);
                            myModelView.TranslateBy(deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        if (Math.Abs(deltaX) <= fixedrange && Math.Abs(deltaY) <= fixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (!planelocked)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    spacenavPlaneTimer.Enabled = true;
                                    spacenavPlaneTimer.Stop();
                                    planelocked = true;
                                    spacenavPlaneTimer.Start();
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                        }
                        else
                        {
                            fixedplaneposcounter = 0;
                        }
                    }

                    prevplaneQuadrant = activeplaneQuadrant;
                }

                processedPlane = true;

            }
            ///////////////////////////////////////
            

            return spacenavMasterTimer.Enabled | spacenavGyroTimer.Enabled | spacenavPlaneTimer.Enabled;
        }

        public double dampChange(double newval, double rate)
        {

            if (newval >= -1 && newval <= 1)
            {
                return (rate * newval);
            }
            else
            {
                return newval;
            }

        }

        public double dampChange(double newval, double rate, double range)
        {

            if (newval >= (-1) * range && newval <= range)
            {
                return (rate * newval);
            }
            else
            {
                return newval;
            }

        }
        public void setRawValues(double[] gdata, double[] pdata)
        {

            if (processedGyro)
            {
                inGyrodata = gdata;

                if (spacenavMasterTimer != null && spacenavGyroTimer != null)
                {
                    spacenavMasterTimer.Enabled = true;
                    //spacenavGyroTimer.Enabled = true;
                    //spacenavGyroTimer.Stop();
                    //spacenavGyroTimer.Start();
                    spacenavMasterTimer.Stop();
                    spacenavMasterTimer.Start();
                }

                if (inGyrodata[2] == 0)
                {
                    //activegyroQuadrant = 0;
                    prevgyroQuadrant = 0;
                }
                else
                {
                    processedGyro = false;

                    if (gyrolocked)
                    {
                        if (inGyrodata[2] < 1)
                        {
                            gyrolocked = false;
                            fixedgyroposcounter = 0;
                            prevgyroQuadrant = 0;
                            spacenavGyroTimer.Enabled = false;
                            //System.Console.WriteLine("gyro timer elapsed");
                        }

                    }


                }

            }

            if (processedPlane)
            {
                inPlanedata = pdata;

                if (spacenavMasterTimer != null && spacenavPlaneTimer != null)
                {
                    spacenavMasterTimer.Enabled = true;
                    //spacenavPlaneTimer.Stop();
                    //spacenavPlaneTimer.Start();
                    spacenavMasterTimer.Stop();
                    spacenavMasterTimer.Start();
                }
                if (inPlanedata[2] == 0)
                {
                    // activeplaneQuadrant = 0;
                    prevplaneQuadrant = 0;
                }
                else
                {
                    processedPlane = false;
                    if (planelocked)
                    {
                        if (inPlanedata[2] <= 1)
                        {
                            planelocked = false;
                            fixedplaneposcounter = 0;
                            prevplaneQuadrant = 0;
                            //System.Console.WriteLine("plane timer elapsed");
                            spacenavPlaneTimer.Enabled = false;
                        }
                    }

                }

            }

            if (!processedGyro || !processedPlane)
            {
                try
                {
                    if (swApp != null)
                    {
                        connectToSolidworks(swObj.InApp_Queue_InCall, "raw");
                    }
                    else
                    {
                        getApplication();
                    }

                }
                catch (Exception)
                {

                }
            }
            if (spacenavMasterTimer == null)
            {
                SetMasterTimer();
            }
            if (spacenavGyroTimer == null)
            {
                SetGyroTimer();

            }
            if (spacenavPlaneTimer == null)
            {
                SetPlaneTimer();

            }




        }
        public void spacenavRaw()
        {

        }

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
                    if (operationname == "SW_auto_orientationLock")
                    {
                        SW_auto_orientationLock();
                    }
                    else
                    {
                        if (defaultAddinTimer != null)
                        {
                            defaultAddinTimer.Enabled = true;
                            defaultAddinTimer.Stop();
                            defaultAddinTimer.Start();
                        }
                        else
                        {
                            SetTimer();
                        }
                        connectToSolidworks(swObj.InApp_Queue_InCall, "default");
                        SW_InApp_Op_queue.Add(operationname);
                        // _Method.Invoke(this, null);

                        //myList.Add(operationname);
                    }



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

    }

    class InApp_Queue : Ito_AhmDialApp
    {
        public void processInApp_Queue()
        {
            while (SOLIDWORKS.invokeMethodsInQueue())
            {

            }
        }

        public void processInApp_Raw()
        {
            ////System.Console.WriteLine("loop opened");
            while (SOLIDWORKS.RotateModel())
            {

            }
            ////System.Console.WriteLine("loop closed");
        }
    }
}
