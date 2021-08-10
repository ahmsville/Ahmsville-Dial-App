using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class PipeManager
    {
        public static PipeManager PipeManagerOBJ = new PipeManager(); 
        static NamedPipeServerStream pipeServer;
        static string activepipename = "";
        static StreamString ss;
        public void transmitdata(string data, string app_pipe)
        {
            if (app_pipe == activepipename)
            {
                //send data
                if (pipeServer != null)
                {
                    if (pipeServer.IsConnected)
                    {
                        if (ss != null)
                        {
                            ss.WriteString(data);
                        }
                    }
                }
            }
            else
            {
                createserver(app_pipe);
                //send data
                if (pipeServer != null)
                {
                    if (pipeServer.IsConnected)
                    {
                        if (ss != null)
                        {
                            ss.WriteString(data);
                        }
                    }
                }
            }
        }
        void createserver(string pipename)
        {
            if (activepipename != pipename)
            {
                if (pipeServer != null)
                {
                    pipeServer.Close();
                }
                try
                {
                    pipeServer = new NamedPipeServerStream(pipename, PipeDirection.InOut);
                    ss = new StreamString(pipeServer);
                    pipeServer.WaitForConnection();
                    activepipename = pipename;
                }
                catch (Exception)
                {
                    activepipename = "";
                    
                }
                
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

            return streamEncoding.GetString(inBuffer);
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
                ioStream.WriteByte((byte)(len / 256));
                ioStream.WriteByte((byte)(len & 255));
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();
            }
            catch (Exception)
            {

                throw;
            }

            return outBuffer.Length + 2;
        }
    }


}
