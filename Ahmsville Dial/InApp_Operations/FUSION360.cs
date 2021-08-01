using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class FUSION360 : FUSION360_INTERFACE
    {

        public static FUSION360 f360Obj = new FUSION360();
        public string fusion360(string operationclass, string operationname)
        {
            startPipe();
            System.Console.WriteLine("fusion360 started");
            return "yes";
        }

        public void setRawValues(double[] gdata, double[] pdata)
        {

        }

        public void startPipe()
        {
            using (var server = new NamedPipeServerStream("Demo"))
            {
                server.WaitForConnection();

                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write("\"hello\"");
                    server.Write(stream.ToArray(), 0, stream.ToArray().Length);
                }

                server.Disconnect();
            }
        }
    }
}
