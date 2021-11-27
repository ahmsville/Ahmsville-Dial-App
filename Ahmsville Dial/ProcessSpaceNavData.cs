using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ahmsville_Dial
{
    class ProcessSpaceNavData
    {
        public static Stopwatch stopwatch = new Stopwatch();

        public static ProcessSpaceNavData ProcessSpaceNavDataOBJ = new ProcessSpaceNavData();
        public static Task activetask;
        public static void process(string activeapp, string[] gyrodata, string[] planedata)
        {
            //rawCalculations.stopwatch.Stop();
            //System.Console.WriteLine("spot0   " + rawCalculations.stopwatch.ElapsedTicks +  " mS: " + rawCalculations.stopwatch.ElapsedMilliseconds);
           // rawCalculations.stopwatch.Reset();
            //rawCalculations.stopwatch.Start();
            if (activeapp.Contains("SOLIDWORKS"))
            {
                rawCalculations.activeapp = "SOLIDWORKS";
                double[] Gdata = { 0, 0, 0 };
                double[] Pdata = { 0, 0, 0 };
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Gdata[i] = double.Parse(gyrodata[i]) / 57.296;
                        Pdata[i] = double.Parse(planedata[i]);
                    }
                    //System.Console.WriteLine("planar =  " + Gdata[0] + "   " + Gdata[1] + "   " + Gdata[2]);
                    rawCalculations.setOriginalRawValues(Gdata, Pdata);

                    if (activetask == null)
                    {

                        //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                        //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                    }
                    else
                    {
                        if (activetask.IsCompleted)
                        {
                            //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                        else
                        {
                           //activetask.Wait();
                           //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                    }


                }
                catch (Exception)
                {


                }


            }
            else if (activeapp.Contains("Fusion 360"))
            {
                rawCalculations.activeapp = "Fusion 360";
                double[] Gdata = { 0, 0, 0 };
                double[] Pdata = { 0, 0, 0 };
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Gdata[i] = double.Parse(gyrodata[i]) / 57.296;
                        Pdata[i] = double.Parse(planedata[i]);
                    }
                    //System.Console.WriteLine("planar =  " + Gdata[0] + "   " + Gdata[1] + "   " + Gdata[2]);
                    rawCalculations.setOriginalRawValues(Gdata, Pdata);

                    if (activetask == null)
                    {

                        //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                        //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                    }
                    else
                    {
                        if (activetask.IsCompleted)
                        {
                            //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                        else
                        {
                            // activetask.Wait();
                            //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                    }


                }
                catch (Exception)
                {


                }


            }
            else if (activeapp.Contains("Blender"))
            {
                rawCalculations.activeapp = "Blender";
                double[] Gdata = { 0, 0, 0 };
                double[] Pdata = { 0, 0, 0 };
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Gdata[i] = double.Parse(gyrodata[i]) / 57.296;
                        Pdata[i] = double.Parse(planedata[i]);
                    }
                    //System.Console.WriteLine("planar =  " + Gdata[0] + "   " + Gdata[1] + "   " + Gdata[2]);
                    rawCalculations.setOriginalRawValues(Gdata, Pdata);

                    if (activetask == null)
                    {

                        //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                        //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                    }
                    else
                    {
                        if (activetask.IsCompleted)
                        {
                            //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                        else
                        {
                            // activetask.Wait();
                            //activetask = Task.Run(() => rawCalculations.setOriginalRawValues(Gdata, Pdata));
                            //activetask = Task.Run(() => InApp_Operations.SOLIDWORKS.setRawValues(Gdata, Pdata));
                        }
                    }


                }
                catch (Exception)
                {


                }


            }
        }
        public static void closeallpipes()
        {
            InApp_Operations.FUSION360.pipeClient.Close();
            InApp_Operations.FUSION360.pipeClient.Dispose();
            InApp_Operations.FUSION360.pipeClient = null;
        }
    }
    class rawCalculations
    {
        public static Stopwatch stopwatch = new Stopwatch();
        public static rawCalculations spacenavRawObj = new rawCalculations();
        public static string activeapp = "";
        public static double[] inGyrodata = { 0, 0, 0 };
        public static double[] inPlanedata = { 0, 0, 0 };

        public static double[] previnGyrodata = { 0, 0, 0 };
        public static double[] previnPlanedata = { 0, 0, 0 };

        public static double[] finalGyrodata = { 0, 0, 0 };
        public static double[] finalPlanedata = { 0, 0, 0 };

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
        public static int gyrolocked = 0;

        public static int fixedplaneposcounter = 0;
        public static int planelocked = 0;

        public static bool orientationlock = false;

        public static string activeFunctionID = "";

        public static Timer spacenavMasterTimer;
        public static Timer spacenavGyroTimer;
        public static Timer spacenavPlaneTimer;
        public static int viewmode = 0;
        public static bool timersActive = false;

        #region setcalcparam

        static int maxgyrocnt = 15;
        static double gyrojitterrange = 0.01;
        static double gyrofixedrange = 0.02;
        static double gyrodamprate = 1;

        static int maxplanecnt = 8;
        static double planejitterrange = 0.03;
        static double planefixedrange = 0.05;
        static double planedamprate = 1;

        
        public static void setcalcparameters(int mgcnt, double gjr, double gfr, double gdr, int mpcnt, double pjr, double pfr, double pdr)
        {
             maxgyrocnt = mgcnt;
            gyrojitterrange = gjr;
            gyrofixedrange = gfr;
            gyrodamprate = gdr;

             maxplanecnt = mpcnt;
             planejitterrange = pjr;
             planefixedrange = pfr;
             planedamprate = pdr;
        }
        #endregion
        private static void SetMasterTimer()
        {
            // Create a timer for com port query
            spacenavMasterTimer = new System.Timers.Timer(5000);
            spacenavMasterTimer.AutoReset = true;
            spacenavMasterTimer.Enabled = true;
            // Hook up the Elapsed event for the timer. 
            spacenavMasterTimer.Elapsed += spacenavMasterTimerEvent;

        }
        private static void SetGyroTimer()
        {
            // Create a timer for com port query
            spacenavGyroTimer = new System.Timers.Timer(250);
            spacenavGyroTimer.AutoReset = true;
            spacenavGyroTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            spacenavGyroTimer.Elapsed += spacenavGyroTimerEvent;

        }

        private static void SetPlaneTimer()
        {
            // Create a timer for com port query
            spacenavPlaneTimer = new System.Timers.Timer(300);
            spacenavPlaneTimer.AutoReset = true;
            spacenavPlaneTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            spacenavPlaneTimer.Elapsed += spacenavPlaneTimerEvent;

        }
        public static void setTimersActive()
        {
            if (!spacenavGyroTimer.Enabled && !spacenavPlaneTimer.Enabled && !spacenavMasterTimer.Enabled)
            {
                timersActive = false;
            }
        }
        private static void spacenavMasterTimerEvent(Object source, ElapsedEventArgs e)
        {
            spacenavMasterTimer.Enabled = false;
            setTimersActive();
            //System.Console.WriteLine("Master timer elapsed");
        }
        private static void spacenavGyroTimerEvent(Object source, ElapsedEventArgs e)
        {
            //gyrolocked = false;
            fixedgyroposcounter = 0;
            //prevgyroQuadrant = 0;
            spacenavGyroTimer.Enabled = false;
            setTimersActive();
            //System.Console.WriteLine("gyro timer elapsed");
        }
        private static void spacenavPlaneTimerEvent(Object source, ElapsedEventArgs e)
        {
            //planelocked = false;
            fixedplaneposcounter = 0;
            //prevplaneQuadrant = 0;
            //System.Console.WriteLine("plane timer elapsed");
            spacenavPlaneTimer.Enabled = false;
            setTimersActive();

        }
        public static double dampChange(double newval, double rate)
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

        public static double dampChange(double newval, double rate, double range)
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
        public static bool setOriginalRawValues(double[] gdata, double[] pdata)
        {
            try
            {

                inGyrodata = gdata;
                inPlanedata = pdata;
                if (spacenavMasterTimer != null && spacenavGyroTimer != null)
                {
                    spacenavMasterTimer.Enabled = true;
                    //spacenavGyroTimer.Enabled = true;
                    //spacenavGyroTimer.Stop();
                    //spacenavGyroTimer.Start();
                    spacenavMasterTimer.Stop();
                    spacenavMasterTimer.Start();
                }

                if (viewmode == 0 || viewmode == 1)
                {
                    processedGyro = false;
                }

                if (viewmode == 0 || viewmode == 2)
                {
                    processedPlane = false;
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
            
                RotateModel();
              
                
                
                timersActive = forwardFinalData();
                resetfinalvalues();
               
            }
            catch (Exception)
            {


            }
         
            return true;
            
        }

        public static void RotateModel()
        {
            #region MyRegion

            //gyro control
            if (!processedGyro)
            {
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

                    deltaX = dampChange(deltaX, gyrodamprate, 30);
                    deltaY = dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > gyrojitterrange || Math.Abs(deltaY) > gyrojitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > gyrojitterrange || Math.Abs(acumgyrodeltaY) > gyrojitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (gyrolocked == 0)
                        {
                            finalGyrodata[0] = deltaY * (-1);
                            finalGyrodata[1] = deltaX;
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(gyrolocked + "====" + fixedgyroposcounter);
                        if (Math.Abs(deltaX) <= gyrofixedrange && Math.Abs(deltaY) <= gyrofixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (gyrolocked == 0)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    
                                    spacenavGyroTimer.Stop();
                                    gyrolocked = 1;
                                    spacenavGyroTimer.Start();
                                    spacenavGyroTimer.Enabled = true;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                            else if (!spacenavGyroTimer.Enabled && gyrolocked == 1)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    gyrolocked = 2;
                                    fixedgyroposcounter = 0;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                else
                                {
                                    fixedgyroposcounter += 1;
                                }
                                
                            }

                        }
                        else
                        {
                            if (gyrolocked == 2)
                            {
                                gyrolocked = 0;
                            }
                            else if (gyrolocked == 0)
                            {
                                fixedgyroposcounter = 0;
                            }
                            
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


                    deltaX = dampChange(deltaX, gyrodamprate, 30);
                    deltaY = dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > gyrojitterrange || Math.Abs(deltaY) > gyrojitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > gyrojitterrange || Math.Abs(acumgyrodeltaY) > gyrojitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (gyrolocked == 0)
                        {
                            finalGyrodata[0] = deltaY * (-1);
                            finalGyrodata[1] = deltaX * (-1);

                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(gyrolocked + "====" + fixedgyroposcounter);
                        if (Math.Abs(deltaX) <= gyrofixedrange && Math.Abs(deltaY) <= gyrofixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (gyrolocked == 0)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {

                                    spacenavGyroTimer.Stop();
                                    gyrolocked = 1;
                                    spacenavGyroTimer.Start();
                                    spacenavGyroTimer.Enabled = true;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                            else if (!spacenavGyroTimer.Enabled && gyrolocked == 1)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    gyrolocked = 2;
                                    fixedgyroposcounter = 0;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                else
                                {
                                    fixedgyroposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (gyrolocked == 2)
                            {
                                gyrolocked = 0;
                            }
                            else if (gyrolocked == 0)
                            {
                                fixedgyroposcounter = 0;
                            }
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


                    deltaX = dampChange(deltaX, gyrodamprate, 30);
                    deltaY = dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > gyrojitterrange || Math.Abs(deltaY) > gyrojitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > gyrojitterrange || Math.Abs(acumgyrodeltaY) > gyrojitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (gyrolocked == 0)
                        {
                            finalGyrodata[0] = deltaY;
                            finalGyrodata[1] = deltaX * (-1);
                            //myModelView.RotateAboutCenter(deltaY, deltaX * (-1));
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(gyrolocked + "====" + fixedgyroposcounter);
                        if (Math.Abs(deltaX) <= gyrofixedrange && Math.Abs(deltaY) <= gyrofixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (gyrolocked == 0)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {

                                    spacenavGyroTimer.Stop();
                                    gyrolocked = 1;
                                    spacenavGyroTimer.Start();
                                    spacenavGyroTimer.Enabled = true;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                            else if (!spacenavGyroTimer.Enabled && gyrolocked == 1)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    gyrolocked = 2;
                                    fixedgyroposcounter = 0;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                else
                                {
                                    fixedgyroposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (gyrolocked == 2)
                            {
                                gyrolocked = 0;
                            }
                            else if (gyrolocked == 0)
                            {
                                fixedgyroposcounter = 0;
                            }
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


                    deltaX = dampChange(deltaX, gyrodamprate, 30);
                    deltaY = dampChange(deltaY, gyrodamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > gyrojitterrange || Math.Abs(deltaY) > gyrojitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumgyrodeltaX) > gyrojitterrange || Math.Abs(acumgyrodeltaY) > gyrojitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumgyrodeltaX;
                            deltaY += acumgyrodeltaY;
                            acumgyrodeltaX = 0;
                            acumgyrodeltaY = 0;
                        }
                        if (gyrolocked == 0)
                        {
                            finalGyrodata[0] = deltaY;
                            finalGyrodata[1] = deltaX;
                            //myModelView.RotateAboutCenter(deltaY, deltaX);
                        }

                    }
                    else
                    {
                        acumgyrodeltaX += deltaX;
                        acumgyrodeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(gyrolocked + "====" + fixedgyroposcounter);
                        if (Math.Abs(deltaX) <= gyrofixedrange && Math.Abs(deltaY) <= gyrofixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("gyrocounter  " + fixedgyroposcounter.ToString());
                            if (gyrolocked == 0)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {

                                    spacenavGyroTimer.Stop();
                                    gyrolocked = 1;
                                    spacenavGyroTimer.Start();
                                    spacenavGyroTimer.Enabled = true;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                fixedgyroposcounter += 1;
                            }
                            else if (!spacenavGyroTimer.Enabled && gyrolocked == 1)
                            {
                                if (fixedgyroposcounter >= maxgyrocnt)//fixed long anough to auto lock
                                {
                                    gyrolocked = 2;
                                    fixedgyroposcounter = 0;
                                    //System.Console.WriteLine("gyro locked");
                                }
                                else
                                {
                                    fixedgyroposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (gyrolocked == 2)
                            {
                                gyrolocked = 0;
                            }
                            else if (gyrolocked == 0)
                            {
                                fixedgyroposcounter = 0;
                            }
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

                    deltaX = dampChange(deltaX, planedamprate, 30);
                    deltaY = dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > planejitterrange || Math.Abs(deltaY) > planejitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > planejitterrange || Math.Abs(acumplanedeltaY) > planejitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (planelocked == 0)
                        {
                            finalPlanedata[0] = deltaY * (-1);
                            finalPlanedata[1] = deltaX;
                            //myModelView.ZoomByFactor(1 + (deltaY * (-1)));
                            //myModelView.TranslateBy(deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(planelocked + "====" + fixedplaneposcounter);
                        if (Math.Abs(deltaX) <= planefixedrange && Math.Abs(deltaY) <= planefixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (planelocked == 0)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {

                                    spacenavPlaneTimer.Stop();
                                    planelocked = 1;
                                    spacenavPlaneTimer.Start();
                                    spacenavPlaneTimer.Enabled = true;
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                            else if (!spacenavPlaneTimer.Enabled && planelocked == 1)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    planelocked = 2;
                                    fixedplaneposcounter = 0;
                                    //System.Console.WriteLine("plane locked");
                                }
                                else
                                {
                                    fixedplaneposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (planelocked == 2)
                            {
                                planelocked = 0;
                            }
                            else if (planelocked == 0)
                            {
                                fixedplaneposcounter = 0;
                            }

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


                    deltaX = dampChange(deltaX, planedamprate, 30);
                    deltaY = dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > planejitterrange || Math.Abs(deltaY) > planejitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > planejitterrange || Math.Abs(acumplanedeltaY) > planejitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (planelocked == 0)
                        {
                            finalPlanedata[0] = (deltaY * (-1));
                            finalPlanedata[1] = (-1) * deltaX;
                            //myModelView.ZoomByFactor(1 + (deltaY * (-1)));
                            // myModelView.TranslateBy((-1) * deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(planelocked + "====" + fixedplaneposcounter);
                        if (Math.Abs(deltaX) <= planefixedrange && Math.Abs(deltaY) <= planefixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (planelocked == 0)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {

                                    spacenavPlaneTimer.Stop();
                                    planelocked = 1;
                                    spacenavPlaneTimer.Start();
                                    spacenavPlaneTimer.Enabled = true;
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                            else if (!spacenavPlaneTimer.Enabled && planelocked == 1)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    planelocked = 2;
                                    fixedplaneposcounter = 0;
                                    //System.Console.WriteLine("plane locked");
                                }
                                else
                                {
                                    fixedplaneposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (planelocked == 2)
                            {
                                planelocked = 0;
                            }
                            else if (planelocked == 0)
                            {
                                fixedplaneposcounter = 0;
                            }

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


                    deltaX = dampChange(deltaX, planedamprate, 30);
                    deltaY = dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > planejitterrange || Math.Abs(deltaY) > planejitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > planejitterrange || Math.Abs(acumplanedeltaY) > planejitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (planelocked == 0)
                        {
                            finalPlanedata[0] = deltaY;
                            finalPlanedata[1] = (-1) * deltaX;
                            //myModelView.ZoomByFactor(1 + deltaY);
                            // myModelView.TranslateBy((-1) * deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(planelocked + "====" + fixedplaneposcounter);
                        if (Math.Abs(deltaX) <= planefixedrange && Math.Abs(deltaY) <= planefixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (planelocked == 0)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {

                                    spacenavPlaneTimer.Stop();
                                    planelocked = 1;
                                    spacenavPlaneTimer.Start();
                                    spacenavPlaneTimer.Enabled = true;
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                            else if (!spacenavPlaneTimer.Enabled && planelocked == 1)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    planelocked = 2;
                                    fixedplaneposcounter = 0;
                                    //System.Console.WriteLine("plane locked");
                                }
                                else
                                {
                                    fixedplaneposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (planelocked == 2)
                            {
                                planelocked = 0;
                            }
                            else if (planelocked == 0)
                            {
                                fixedplaneposcounter = 0;
                            }

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


                    deltaX = dampChange(deltaX, planedamprate, 30);
                    deltaY = dampChange(deltaY, planedamprate, 30);
                    //System.Console.WriteLine("    " + deltaX.ToString() + " , " + deltaY.ToString());

                    if (Math.Abs(deltaX) > planejitterrange || Math.Abs(deltaY) > planejitterrange)//removes jitter
                    {
                        //small changes accumulation is now large anough to matter
                        if (Math.Abs(acumplanedeltaX) > planejitterrange || Math.Abs(acumplanedeltaY) > planejitterrange)
                        {
                            //System.Console.WriteLine("acum added");
                            deltaX += acumplanedeltaX;
                            deltaY += acumplanedeltaY;
                            acumplanedeltaX = 0;
                            acumplanedeltaY = 0;
                        }
                        if (planelocked == 0)
                        {
                            finalPlanedata[0] = deltaY;
                            finalPlanedata[1] = deltaX;
                            //myModelView.ZoomByFactor(1 + deltaY);
                            //myModelView.TranslateBy(deltaX, 0);
                        }

                    }
                    else
                    {
                        acumplanedeltaX += deltaX;
                        acumplanedeltaY += deltaY;
                    }
                    if (orientationlock)
                    {
                        //System.Console.WriteLine(planelocked + "====" + fixedplaneposcounter);
                        if (Math.Abs(deltaX) <= planefixedrange && Math.Abs(deltaY) <= planefixedrange)//auto orientation lock
                        {
                            //System.Console.WriteLine("planecounter  " + fixedplaneposcounter.ToString());
                            if (planelocked == 0)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {

                                    spacenavPlaneTimer.Stop();
                                    planelocked = 1;
                                    spacenavPlaneTimer.Start();
                                    spacenavPlaneTimer.Enabled = true;
                                    //System.Console.WriteLine("plane locked");
                                }
                                fixedplaneposcounter += 1;
                            }
                            else if (!spacenavPlaneTimer.Enabled && planelocked == 1)
                            {
                                if (fixedplaneposcounter >= maxplanecnt)//fixed long anough to auto lock
                                {
                                    planelocked = 2;
                                    fixedplaneposcounter = 0;
                                    //System.Console.WriteLine("plane locked");
                                }
                                else
                                {
                                    fixedplaneposcounter += 1;
                                }

                            }

                        }
                        else
                        {
                            if (planelocked == 2)
                            {
                                planelocked = 0;
                            }
                            else if (planelocked == 0)
                            {
                                fixedplaneposcounter = 0;
                            }

                        }
                    }

                    prevplaneQuadrant = activeplaneQuadrant;
                }

                processedPlane = true;


            }

            /////////////////////////////////////// 
            #endregion



        }
        public static bool forwardFinalData()
        {
            try
            {
                if (finalGyrodata[0] != 0 || finalGyrodata[1] != 0 || finalPlanedata[0] != 0 || finalPlanedata[1] != 0)
                {

                    if (activeapp == "SOLIDWORKS")
                    {

                        InApp_Operations.SOLIDWORKS.setRawValues(finalGyrodata, finalPlanedata);

                    }
                    else if (activeapp == "Fusion 360")
                    {

                        InApp_Operations.FUSION360.setRawValues(finalGyrodata, finalPlanedata);

                    }
                    else if (activeapp == "Blender")
                    {

                        InApp_Operations.BLENDER.setRawValues(finalGyrodata, finalPlanedata);

                    }


                }
            }
            catch (Exception)
            {

               
            }
           
            return spacenavMasterTimer.Enabled | spacenavGyroTimer.Enabled | spacenavPlaneTimer.Enabled;
        }
        public static void resetfinalvalues()
        {
            for (int i = 0; i < 3; i++)
            {
                finalGyrodata[i] = 0;
                finalPlanedata[i] = 0;
            }
            
        }
    }
}
