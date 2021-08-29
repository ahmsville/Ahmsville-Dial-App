using CodeStack.SwEx.AddIn;
using CodeStack.SwEx.AddIn.Attributes;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CodeStack.SwEx.Common.Attributes;
using System.ComponentModel;
using Ahmsville_Dial_SOLIDWORKS_AddIn.Properties;
using CodeStack.SwEx.AddIn.Enums;
using System.Threading;
using System.Timers;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace Ahmsville_Dial_SOLIDWORKS_AddIn
{
    [ComVisible(true), Guid("2EEAB3C8-95A0-41A7-96A3-F3CC17F937C2")]
    [AutoRegister("Ahmsville_Dial", "Ahmsville Dial SolidWorks AddIn")]
    [ProgId("AhmsvilleDial.SWAddIn")]
    public class AhmsvilleDial_AddIn : SwAddInEx
    {
        [Title("Ahmsville Dial")]
        [Description("Ahmsville Dial SolidWorks Add_In")]
        [Icon(typeof(Resources), nameof(Resources.DialLogo))]

        public static StreamString reader;
        NamedPipeServerStream pipeServer;

        public ITo_AhmDialApp to_callback;
        static AhmsvilleDial_AddIn addInObj = new AhmsvilleDial_AddIn();

        static ModelDoc model = null;
        static ModelView myModelView = null;

        double rollmodelStep = 0.05;

        private enum addinCommands  //create list of addin functions/commands
        {
            [Title("Ahmsville Dial")]
            [Description("Ahmsville Dial SolidWorks AddIn")]
            [Icon(typeof(Resources), nameof(Resources.DialLogo))]
            // [CommandItemInfo(true,true,swWorkspaceTypes_e.All, true)]
            AhmsvilleDial
        }
        public override bool OnConnect()  //detect when functions are clicked
        {
            AddCommandGroup<addinCommands>(OnAddInButtonClick); //add commands for detection
            (App as SldWorks).OnIdleNotify += OnIdleNotify;
            return base.OnConnect();
        }
        private void OnAddInButtonClick(addinCommands command) //command/function clicked event 
        {
            switch (command)
            {
                case addinCommands.AhmsvilleDial:
                    ahmsvilleDial();
                    break;
            }
        }
        public void ahmsvilleDial()
        {

            model = App.IActiveDoc;
            if (model != null)
            {
                if (myModelView == null)
                {
                    myModelView = ((ModelView)model.IActiveView);
                    myModelView.RotateAboutCenter(-0.110483068706364271, 0);
                }
                else
                {

                    myModelView.RotateAboutCenter(-0.110483068706364271, 0);
                }

            }


        }


        public bool registerCallback_InSW(ITo_AhmDialApp callreturn, string functionIdentifier)
        {
            if (to_callback != callreturn)
            {
                to_callback = callreturn;
            }


            return true;
        }

        static bool loopstarted = false;
        private bool queueIN()
        {
            loopstarted = true;
            while (to_callback != null)
            {
                testfunc();
                try
                {
                    to_callback.processInApp_Queue();
                }
                catch (Exception)
                {
                    to_callback = null;
                    loopstarted = false;
                }
            }

            return true;
        }

        Task<bool> invokeQueue;
        public async void breakoffOperationAsync()
        {

            if (!loopstarted)
            {
                invokeQueue = new Task<bool>(createpipe);
                invokeQueue.Start();
                bool ret = await invokeQueue;
            }

        }
        public bool testfunc()
        {
            if (to_callback != null)
            {
                ModelDoc2 swDoc = null;
                PartDoc swPart = null;
                DrawingDoc swDrawing = null;
                AssemblyDoc swAssembly = null;
                bool boolstatus = false;
                int longstatus = 0;
                int longwarnings = 0;
                swDoc = ((ModelDoc2)(App.ActiveDoc));
                ModelView myModelView = null;
                myModelView = ((ModelView)(swDoc.ActiveView));
                //myModelView.EnableGraphicsUpdate = false;
                myModelView.RotateAboutCenter(-1.8730103729387098, -3.5845057162270511);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.25921125696919645, -0.71844618881274935);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.01672330690123848, -0.023175683510088689);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.00706, 0.0084200000000000056);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0075200000000000111, -0.00312000000000001);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.13378645520990784, -0.772522783669623);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.10452066813274051, -0.91930211256685135);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0, -0.10815318971374722);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.01672330690123848, -0.18540546808070951);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.02926578707716734, -0.20085592375410197);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0081799999999999876, -0.0060999999999999934);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.00049999999999998657, 0.00354000000000001);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.13378645520990784, 0);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.17141389573769442, 0.054076594856873612);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.15050976211114633, 0.069527050530266074);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.38463605872848505, 0.32445956914124169);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.17141389573769442, 0.16222978457062084);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.21740298971610025, 0.20085592375410197);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.14632893538583672, 0.13132887322383593);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.10452066813274051, 0.16995501240731706);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0836165345061924, 0.13905410106053215);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0418082672530962, 0.084977506203658529);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.03344661380247696, 0.06180182269356984);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.02926578707716734, 0.054076594856873612);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.01672330690123848, 0.01545045567339246);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.00418082672530962, 0);
                // 
                // Roll View
                ModelView swModelView = null;
                swModelView = ((ModelView)(swDoc.ActiveView));
                swModelView.RollBy(0);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.41808267253096204, -1.6918248962364746);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.03344661380247696, -0.29355865779445678);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.079435707780882783, -0.55621640424212859);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.05853157415433468, -0.44806321452838138);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.037627440527786583, -0.20858115159079821);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.00836165345061924, -0.03090091134678492);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0, -0.00772522783669623);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.010099999999999998, -0.00052000000000000939);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.026940000000000013, 0.00516);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0082600000000000017, -0.0062799999999999974);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0062400000000000016, 0.0081199999999999935);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0022800000000000155, -0.0092400000000000034);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0041799999999999858, 0.00064000000000000688);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.013660000000000005, 0.0016800000000000148);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.00023999999999999578, -0.011639999999999996);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0057599999999999874, 0.011639999999999996);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0062799999999999974, -0.008080000000000009);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.12668000000000001, 0.0037999999999999813);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.01432, 0.0016399999999999971);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.01714, 0.016480000000000005);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.026500000000000013, 0.012819999999999988);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.036240000000000008, -0.0057599999999999986);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.049139999999999996, 0.002100000000000002);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.028120000000000003, 0.012419999999999999);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0082000000000000024, 0.0043599999999999976);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.02142, -0.00232);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.00948, 0.0036599999999999966);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.00928, 0.0022800000000000042);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.007040000000000002, 0.0096999999999999986);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.013519999999999989, 0.011260000000000004);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.0053999999999999829, -0.010500000000000002);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.00855999999999999, -0.0014399999999999997);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.034780000000000012, -0.00426);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.019459999999999991, -0.004859999999999998);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.04596, -0.03056);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0093200000000000071, -0.00076000000000000514);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.063319999999999987, 0.0038200000000000013);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.046540000000000005, -0.007619999999999994);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.06898, 0.0074799999999999979);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.03384, -0.0016399999999999971);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.03262, -0.022619999999999998);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.016080000000000007, -0.012280000000000003);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.011099999999999978, -0.011020000000000009);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0077600000000000342, -0.010139999999999995);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.012539999999999996, -0.0053400000000000114);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.013660000000000005, -0.0012599999999999946);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0066599999999999776, 0.011280000000000002);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.019240000000000014, 0.034980000000000011);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.022619999999999998, 0.0012600000000000057);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.03752, 0.0055999999999999826);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.0082600000000000225, -0.0019199999999999996);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.031099999999999996, -0.0072000000000000067);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.042779999999999957, -0.0006000000000000006);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.08504000000000006, 0.0033800000000000054);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.07382, -0.013080000000000003);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.08804, -0.015599999999999992);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.062159999999999993, -0.011020000000000009);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.024339999999999942, -0.0043199999999999905);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.021600000000000109, -0.0038200000000000013);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.011139999999999973, -0.01342000000000001);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.014200000000000124, -0.012480000000000003);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.045639999999999951, -0.019419999999999993);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.072780000000000025, -0.0087400000000000151);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.075859999999999955, 0.00960000000000001);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.24926, 0.068840000000000012);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.27616000000000007, 0.0096999999999999986);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(0.00875999999999999, 0.0032999999999999922);
                myModelView = ((ModelView)(swDoc.ActiveView));
                myModelView.RotateAboutCenter(-0.00875999999999999, -0.0032999999999999922);

                myModelView.EnableGraphicsUpdate = true;
            }
            return true;
        }
        public bool createpipe()
        {
            if (pipeServer == null)
            {
                pipeServer = new NamedPipeServerStream("SOLIDWORKS", PipeDirection.In);
                // Wait for a client to connect
                pipeServer.WaitForConnection();
            }
            if (pipeServer.IsConnected)
            {
                reader = new StreamString(pipeServer);

                while (pipeServer.IsConnected)
                {
                    //startreading
                    loopstarted = true;
                    string output = reader.ReadStringUntil("*");
                    //App.SendMsgToUser(output);
                    processdata(output);
                }

                pipeServer.Disconnect();
                pipeServer.Dispose();
                pipeServer = null;
                loopstarted = false;
            }
            return true;


        }

        public void processdata(string finaloutput)
        {
            if (finaloutput.StartsWith(">>"))
            {
                string[] datastr = { "", "", "", "" };
                double[] dataflt = { 0, 0, 0, 0 };
                int floatstrpos = 0;
                for (int i = 0; i < finaloutput.Length; i++)
                {
                    char c = finaloutput[i];
                    if (c != '\0' && c != '*' && c != '>')
                    {
                        if (c == '|')
                        {
                            floatstrpos += 1;
                        }
                        else
                        {
                            datastr[floatstrpos] += c;
                        }
                    }

                }
                for (int i = 0; i < dataflt.Length; i++)
                {
                    //debug(datastr[i]);
                    dataflt[i] = Convert.ToDouble(datastr[i]);

                }
                if (connectSwDoc())
                {
                    myModelView.RotateAboutCenter(dataflt[0], dataflt[1]);
                    myModelView.EnableGraphicsUpdate = false;
                    myModelView.ZoomByFactor(1 + (dataflt[2]/3));
                    myModelView.TranslateBy(dataflt[3]/4, 0);
                    myModelView.EnableGraphicsUpdate = true;

                }
            }
            else if (finaloutput.StartsWith("//"))
            {
                if (connectSwDoc())
                {
                    finaloutput = finaloutput.Replace("//", "");
                    finaloutput = finaloutput.Replace("*", "");
                    try
                    {
                        _MethodInfo _Method = addInObj.GetType().GetMethod(finaloutput);
                        _Method.Invoke(addInObj, null);
                    }
                    catch (Exception)
                    {


                    }
                }

            }
        }
        public bool connectSwDoc()
        {
            ModelDoc activemodel = App.IActiveDoc;
            if (activemodel != null)
            {
                if (model != activemodel)
                {
                    model = activemodel;

                    myModelView = ((ModelView)model.IActiveView);
                    return true;


                }
            }
            else
            {
                return false;
            }
            return true;

        }
        private int OnIdleNotify()
        {

            breakoffOperationAsync();


            return 0;
            // App.SendMsgToUser($"debug");

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
            model.ViewZoomtofit();

        }
        public void SW_measure()
        {
            //myModelView
        }


        #endregion
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
                if (inbyte > 0)
                {
                    instr += Convert.ToChar(inbyte);
                }
                else if (inbyte == -1)
                {
                    break;
                }

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
