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

        public bool registerCallback_InSW(Ito_AhmDialApp callreturn)
        {
            if (to_callback != callreturn)
            {
                poolingStarted = false;
                to_callback = callreturn;

            }
            return true;
        }

        bool poolingStarted = false;

        public bool queueIN()
        {
            while (poolingStarted)
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
            
            
           
            return true;
        }
      
        public async void breakoffOperationAsync()
        {
            if (to_callback != null)
            {
                Task<bool> invokeQueue = new Task<bool>(queueIN);
                invokeQueue.Start();
                poolingStarted = true;
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

        private int OnIdleNotify()
        {
            if (!poolingStarted)
            {
                breakoffOperationAsync();     
                
            }
           
            return 0;
            // App.SendMsgToUser($"debug");
            
        }
    }

}
