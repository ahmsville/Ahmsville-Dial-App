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
    class FUSION360 : FUSION360_INTERFACE
    {

        public static FUSION360 f360Obj = new FUSION360();
        public static NamedPipeClientStream pipeClient;
        public static StreamString writer;

        bool constate = false;
       
        public string fusion360(string operationclass, string operationname)
        {
            if (operationname.StartsWith("_FU360"))
            {
                try
                {
                    _MethodInfo _Method = f360Obj.GetType().GetMethod(operationname);
                    _Method.Invoke(f360Obj, null);
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
                   // System.Console.WriteLine(operationname);
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
                pipeClient = new NamedPipeClientStream(".", "FUSION360", PipeDirection.Out);

             
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
       
        public void _FU360_auto_orientationLock()
        {
            Ahmsville_Dial.rawCalculations.orientationlock = !Ahmsville_Dial.rawCalculations.orientationlock;
            if (Ahmsville_Dial.rawCalculations.orientationlock == false)
            {
                Ahmsville_Dial.rawCalculations.gyrolocked = 0;
                Ahmsville_Dial.rawCalculations.planelocked = 0;
            }

        }

        public void _FU360_change_viewmode()
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
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
           
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return System.Text.Encoding.Default.GetString(inBuffer);
            //return streamEncoding.GetString(inBuffer);
        }

        public string ReadStringUntil(string endstr)
        {
            int inbyte = 0;
            string instr = "";
            while (!instr.EndsWith(endstr))
            {
                inbyte = ioStream.ReadByte();
                instr += Convert.ToChar(inbyte);
            }

            return instr;
            //return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }

            try
            {
                // ioStream.WriteByte((byte)(len / 256));
                //ioStream.WriteByte((byte)(len & 255));
              
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();
            }
            catch (Exception)
            {

                //throw;
            }

            return outBuffer.Length + 2;
        }
    }

}
