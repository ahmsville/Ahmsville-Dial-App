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
using System.IO.Pipes;
using System.IO;
using System.Threading;

namespace Ahmsville_Dial.InApp_Operations
{
    class SOLIDWORKS : SOLIDWORKS_INTERFACE
    {
        public static NamedPipeClientStream pipeClient;
        public static StreamString writer;
      

        public static bool orientationlock = false;


        public static SOLIDWORKS swObj = new SOLIDWORKS();
        InApp_Queue InApp_Queue_InCall = new InApp_Queue();
        private static Object mylock = new object();


        public static List<string> SW_InApp_Op_queue = new List<string>();


        public static bool newvalueSet = false;
       




        public void _SW_auto_orientationLock()
        {
            Ahmsville_Dial.rawCalculations.orientationlock = !Ahmsville_Dial.rawCalculations.orientationlock;

            if (Ahmsville_Dial.rawCalculations.orientationlock)
            {

                //System.Console.WriteLine("Auto locking enabled");
            }
            else
            {
                //prevgyroQuadrant = 0;
                //prevplaneQuadrant = 0;
                //System.Console.WriteLine("Auto locking disabled");
            }
        }

        public void _SW_change_viewmode()
        {
            if (Ahmsville_Dial.rawCalculations.viewmode < 3)
            {
                Ahmsville_Dial.rawCalculations.viewmode += 1;
            }
            else
            {
                Ahmsville_Dial.rawCalculations.viewmode = 0;
            }
        }








        public string solidworks(string operationclass, string operationname)
        {

            if (operationname.StartsWith("_SW"))
            {
                try
                {
                    _MethodInfo _Method = swObj.GetType().GetMethod(operationname);
                    _Method.Invoke(swObj, null);
                }
                catch (Exception)
                {


                }
            }
            else
            {
                operationname = "//" + operationname + "*";

                startPipe();
                if (writer != null)
                {
                    writer.WriteString(operationname);
                    System.Console.WriteLine(operationname);
                    //Thread.Sleep(2000);
                }
            }



            return operationclass + "  yes  " + operationname;


        }
        public static void setRawValues(double[] gdata, double[] pdata)
        {
            string dataout =
                ">>"
                + gdata[0].ToString("0.0000") + "|"
                + gdata[1].ToString("0.0000") + "|"
                + pdata[0].ToString("0.0000") + "|"
                + pdata[1].ToString("0.0000") + "*";




            startPipe();
            if (writer != null)
            {
                writer.WriteString(dataout);
                System.Console.WriteLine(dataout);
                //Thread.Sleep(2000);
            }


        }
        public static void startPipe()
        {

            if (pipeClient == null)
            {
                pipeClient = new NamedPipeClientStream(".", "SOLIDWORKS", PipeDirection.Out);


                pipeClient.Connect(1000);

                writer = new StreamString(pipeClient);
            }
            else if (!pipeClient.IsConnected)
            {
                //pipeClient.Flush();
                pipeClient.Close();
                pipeClient.Dispose();
                pipeClient = null;
            }
        }
    }

    class InApp_Queue : ITo_AhmDialApp
    {

        public void processInApp_Queue()
        {


        }


    }

}
