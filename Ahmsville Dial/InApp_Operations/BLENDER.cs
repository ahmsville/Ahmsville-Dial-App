using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Timers;

namespace Ahmsville_Dial.InApp_Operations
{
    class BLENDER
    {

        public static BLENDER blenderObj = new BLENDER();
        public static NamedPipeClientStream pipeClient;
        public static StreamString writer;

        bool constate = false;
       
        public string blender(string operationclass, string operationname)
        {
            if (operationname.StartsWith("_BLENDER"))
            {
                try
                {
                    _MethodInfo _Method = blenderObj.GetType().GetMethod(operationname);
                    _Method.Invoke(blenderObj, null);
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
                    //System.Console.WriteLine(operationname);
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
                //System.Console.WriteLine(dataout);
                //Thread.Sleep(2000);
            }
            
            
        }

        public static void startPipe()
        {
            
            if (pipeClient == null)
            {
                pipeClient = new NamedPipeClientStream(".", "blender", PipeDirection.Out);

             
                pipeClient.Connect(1000);

                writer = new StreamString(pipeClient);
            }
            else if(!pipeClient.IsConnected)
            {
                //pipeClient.Flush();
                pipeClient.Close();
                pipeClient.Dispose();
                pipeClient = null;
            }
        }
       
        public void _BLENDER_auto_orientationLock()
        {
            Ahmsville_Dial.rawCalculations.orientationlock = !Ahmsville_Dial.rawCalculations.orientationlock;
            if (Ahmsville_Dial.rawCalculations.orientationlock == false)
            {
                Ahmsville_Dial.rawCalculations.gyrolocked = 0;
                Ahmsville_Dial.rawCalculations.planelocked = 0;
            }
        }

        public void _BLENDER_change_viewmode()
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

    }
    

}
