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

        private Ito_AhmDialApp to_callback;
        private Ito_AhmDialApp to_callback2;

        ModelDoc model = null;
        ModelView myModelView = null;

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

        string activeFunctionID = "";
        public bool registerCallback_InSW(Ito_AhmDialApp callreturn, string functionIdentifier)
        {
            activeFunctionID = functionIdentifier;
            if (activeFunctionID == "default")
            {
                if (to_callback != callreturn)
                {
                    poolingStarted = false;
                    to_callback = callreturn;

                }
            }
            else if (activeFunctionID == "raw")
            {
                if (to_callback2 != callreturn)
                {
                    poolingStarted2 = false;
                    to_callback2 = callreturn;

                }
            }
            
           
            return true;
        }

        bool poolingStarted = false;
        bool poolingStarted2 = false;

        public bool queueIN()
        {
            while (poolingStarted)
            {
                while (to_callback != null)
                {
                    try
                    {
                        to_callback.processInApp_Queue();
                    }
                    catch (Exception)
                    {
                        poolingStarted = false;
                        // to_callback = null;
                    }
                }
                
            }
            
            
           
            return true;
        }

        public bool queueIN2()
        {
            while (poolingStarted2)
            {
                while (to_callback2 != null)
                {
                    try
                    {
                        to_callback2.processInApp_Raw();

                    }
                    catch (Exception)
                    {
                        poolingStarted2 = false;
                        // to_callback = null;
                    }
                }
                
            }



            return true;
        }

        public async void breakoffOperationAsync()
        {
            if (to_callback != null)
            {
                Task<bool> invokeQueue = new Task<bool>(queueIN);
                poolingStarted = true;
                invokeQueue.Start();
                //App.SendMsgToUser(poolingStarted.ToString());
                bool ret = await invokeQueue;
                if (ret)
                {
                    to_callback = null;
                    poolingStarted = false;
                    //App.SendMsgToUser("endedloop");
                }
            }
        }

        public async void breakoffOperationAsync2()
        {
            if (to_callback2 != null)
            {
                Task<bool> invokeQueue2 = new Task<bool>(queueIN2);
                poolingStarted2 = true;
                invokeQueue2.Start();
                //App.SendMsgToUser(poolingStarted.ToString());
                bool ret2 = await invokeQueue2;
                if (ret2)
                {
                    to_callback2 = null;
                    poolingStarted2 = false;
                    //App.SendMsgToUser("endedloop");
                }
            }
        }

        private int OnIdleNotify()
        {
            if (!poolingStarted)
            {
                breakoffOperationAsync();
                
            }
            if (!poolingStarted2)
            {
                
                breakoffOperationAsync2();
            }

            return 0;
            // App.SendMsgToUser($"debug");
            
        }
    }

}
