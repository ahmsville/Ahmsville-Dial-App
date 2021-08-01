using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO.Ports;
using System.Timers;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Ahmsville_Dial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer reconTimer;
        //private static System.Timers.Timer COMportQTimer;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);


        static SerialPort _serialPort = new SerialPort();
        List<string> filteredSerialData = new List<string>();
        // Get a list of serial port names.
        static string[] ports = SerialPort.GetPortNames();
        static string inputstring = "";
        static string inputstring2 = "";
        static bool dialfound = false;
        static string[] dialnames = { };
        static string processedport = "";
        static int pos = 0;
        static int con_try = 3;

        static bool is_wireless = false;
        static bool intentionaldisconnect = false;
        static int receivedlineindex = 0;
        bool configfound = false;
        int defaultconfigID = 0;
        //bool timedeventhandlingisdone = true;
        string activeapp;
        string prevactiveapp;
        //path for Dial's config files and library files 
        string[] dialconfigpath = { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Base_Variant_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\MacroKey_Variant_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\SpaceNav_Variant_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Absolute_Variant_main\"};

        string connecteddialconfigpath = "";
        string[] dialvariants = { "Base", "MAcroKey", "SpaceNav", "Absolute" };
        char[] detectedwirelessdials = { '0', '0', '0', '0' };
        char[] prevdetectedwirelessdials = { '0', '0', '0', '0' };
        string[] dialmain_arduinofileByVersion = {"Base_Variant_main.ino",
            "MacroKey_Variant_main.ino",
            "SpaceNav_Variant_main.ino",
            "Absolute_Variant_main.ino" };
        string[] availableconfigs = { };

        struct availableconfigsforapp
        {
            public string[] linesforappfunctions;
        }
        static availableconfigsforapp[] loadedappfunctions = new availableconfigsforapp[0];
        static int active_loaded_inappfunctions_index = 0;

        private IntPtr windowHandle;
        string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;
        int querycnt = 0;
        int querynum = 10; //number of query attempts
        int activeQueryportnum = 0;

        //public AhmsvilleDialViewModel model { get; set; }

        InApp_Operations.SOLIDWORKS_INTERFACE solidworksInstance = new InApp_Operations.SOLIDWORKS();
       // InApp_Operations.SOLIDWORKS_INTERFACE fusion360Instance = new InApp_Operations.FUSION360();
        private bool deviceremoved;
        private bool deviceadded;

        public MainWindow()
        {
            DataContext = new AhmsvilleDialViewModel();
            InitializeComponent();
            AutoSW.IsChecked = true;
            SetTimer();
            AhmsvilleDialViewModel.Instance.constate = 0;


            goGetDevices();

            //ConnectDial();






        }

        private void ConnectDial()
        {
            if (AhmsvilleDialViewModel.Instance.constate != 1)
            {
                if (wired_diallist.Items.Count == 1) //connect to the only available device
                {
                    wired_diallist.SelectedIndex = 0;
                    string selecteddialport = wired_diallist.SelectedItem.ToString();

                    var splitcominfo = selecteddialport.Split(';');
                    SwitchSerialPort(splitcominfo[0]); //get selected port name
                    try
                    {
                        int tryturn = 0;
                        while (!_serialPort.IsOpen && tryturn < con_try) //open connection on selected port
                        {
                            //AhmsvilleDialViewModel.Instance.connectionstate = ("connecting to port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                            _serialPort.Open();
                            tryturn++;
                        }

                    }
                    catch (Exception)
                    {
                        // AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");
                    }





                    if (_serialPort.IsOpen)  //is connected to a directly connected device
                    {

                        if (wired_diallist.SelectedItem.ToString().Contains("Wireless"))
                        {
                            AhmsvilleDialViewModel.Instance.connectionstate += "Connected to Wireless adapter" + "\n";
                            AhmsvilleDialViewModel.Instance.constate = 3;
                            is_wireless = true;
                            ConnectedDialType(); //get dial type and load config for connected dial
                            if (wireless_diallist.Items.Count == 1)
                            {
                                AhmsvilleDialViewModel.Instance.constate = 1;
                                wireless_diallist.SelectedIndex = 0;
                            }


                        }
                        else
                        {
                            AhmsvilleDialViewModel.Instance.connectionstate += "Connected to Dial" + "\n";
                            AhmsvilleDialViewModel.Instance.constate = 1;
                            is_wireless = false;
                            ConnectedDialType(); //get dial type and load config for connected dial


                        }
                    }
                    else
                    {

                        AhmsvilleDialViewModel.Instance.connectionstate += "Failed to connect to dial" + "\n";
                        AhmsvilleDialViewModel.Instance.constate = 2;
                        wired_diallist.IsEnabled = true;

                    }

                }
                else
                {
                    if (!wired_diallist.Items.IsEmpty)
                    {
                        wired_diallist.IsEnabled = true;
                        AhmsvilleDialViewModel.Instance.connectionstate += "choose your dial" + "\n";
                        AhmsvilleDialViewModel.Instance.constate = 3;
                    }
                    else
                    {
                        AhmsvilleDialViewModel.Instance.connectionstate += "no Dial connected" + "\n";
                    }

                }

            }
        }

        private void wiredDialList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                int tryturn = 0;
                while (_serialPort.IsOpen && tryturn < con_try)
                {
                    //AhmsvilleDialViewModel.Instance.connectionstate = ("closing port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                    _serialPort.Close();
                    tryturn++;
                }
            }
            catch (Exception)
            {
                //AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");
            }
            wired_diallist.IsEnabled = true;
            AhmsvilleDialViewModel.Instance.constate = 2;
            intentionaldisconnect = true;

            AhmsvilleDialViewModel.Instance.connectionstate += "disconnected from dial" + "\n";



        }

        private void Connect_Button(object sender, RoutedEventArgs e)
        {

            if (wired_diallist.Items.IsEmpty)
            {

                AhmsvilleDialViewModel.Instance.constate = 2;
                //AhmsvilleDialViewModel.Instance.connectionstate = ("no dial Connected");
                activeQueryportnum = 0;
                //getavailablePorts();
                goGetDevices();
                //RefreshPortList();

            }
            else
            {
                if (AhmsvilleDialViewModel.Instance.constate == 1)
                {
                    try
                    {
                        int tryturn = 0;
                        while (_serialPort.IsOpen && tryturn < con_try)
                        {
                            //AhmsvilleDialViewModel.Instance.connectionstate = ("closing port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                            _serialPort.Close();
                            tryturn++;
                        }
                    }
                    catch (Exception)
                    {
                        //AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");
                    }
                    wired_diallist.IsEnabled = true;
                    AhmsvilleDialViewModel.Instance.constate = 2;
                    intentionaldisconnect = true;

                    AhmsvilleDialViewModel.Instance.connectionstate += "disconnected from dial" + "\n";

                }
                else if (AhmsvilleDialViewModel.Instance.constate == 2 || AhmsvilleDialViewModel.Instance.constate == 3)
                {
                    if (!wired_diallist.Items.IsEmpty)
                    {
                        try
                        {
                            int tryturn = 0;
                            while (_serialPort.IsOpen && tryturn < con_try)
                            {
                                //AhmsvilleDialViewModel.Instance.connectionstate = ("closing port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                                _serialPort.Close();
                                tryturn++;
                            }
                        }
                        catch (Exception)
                        {
                            // AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");
                        }
                        if (wired_diallist.SelectedItem != null)
                        {
                            string selecteddialport = wired_diallist.SelectedItem.ToString();
                            var splitcom = selecteddialport.Split(';');

                            SwitchSerialPort(splitcom[0]);
                        }

                        try
                        {
                            int tryturn = 0;
                            while (!_serialPort.IsOpen && tryturn < con_try)
                            {
                                // AhmsvilleDialViewModel.Instance.connectionstate = ("connecting to port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                                _serialPort.Open();
                                tryturn++;
                            }
                        }
                        catch (Exception)
                        {
                            // AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");
                        }

                        //MessageBox.Show(selecteddialport);
                        if (_serialPort.IsOpen)
                        {
                            intentionaldisconnect = false;
                            if (wired_diallist.SelectedItem != null)
                            {
                                if (wired_diallist.SelectedItem.ToString().Contains("Wireless"))
                                {
                                    AhmsvilleDialViewModel.Instance.connectionstate += "Connected to Wireless adapter" + "\n";
                                    AhmsvilleDialViewModel.Instance.constate = 1;
                                    is_wireless = true;
                                    ConnectedDialType(); //get dial type and load config for connected dial


                                }
                                else
                                {
                                    AhmsvilleDialViewModel.Instance.connectionstate += "Connected to Dial" + "\n";
                                    AhmsvilleDialViewModel.Instance.constate = 1;
                                    is_wireless = false;
                                    ConnectedDialType(); //get dial type and load config for connected dial


                                }
                            }

                        }
                        else
                        {
                            AhmsvilleDialViewModel.Instance.connectionstate += "Failed to connect to dial" + "\n";

                        }

                    }
                }
                else if (AhmsvilleDialViewModel.Instance.constate == 0)
                {
                    // getavailablePorts();
                    goGetDevices();
                }
            }

        }
        private void SwitchSerialPort(string portname)
        {

            _serialPort = new SerialPort();
            _serialPort.PortName = portname;
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.DataReceived += DataReceivedHandler;
            // _serialPort.ReadBufferSize = 19 * 2;


        }

        private void connectSerialPort(string portname)
        {

            _serialPort = new SerialPort();
            _serialPort.PortName = portname;
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.DataReceived += DataReceivedHandler;

            //open port
            try
            {
                int tryturn = 0;
                while (!_serialPort.IsOpen && tryturn < con_try)
                {
                    _serialPort.Open();
                    tryturn++;

                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Failed to connect to PnPWheel");
                AhmsvilleDialViewModel.Instance.constate = 2;
            }
            if (_serialPort.IsOpen)
            {

            }
        }

        #region Serial Data Received Event
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            char[] inChar = new char[19];
            filteredSerialData.Clear();
            if (_serialPort.IsOpen)
            {
                string gy = "";
                string pl = "";
                while (_serialPort.BytesToRead > 0)
                {
                    try
                    {
                        _serialPort.Read(inChar, 0, 19);
                        inputstring = new string(inChar);

                        if (inputstring.Contains("<"))
                        {
                            filteredSerialData.Remove(gy);
                            gy = inputstring;
                        }
                        else if (inputstring.Contains(">"))
                        {
                            filteredSerialData.Remove(pl);
                            pl = inputstring;
                        }
                        filteredSerialData.Add(inputstring);

                    }
                    catch (Exception)
                    {
                        //connectionstate.Text = "ConnectDialion closed";
                    }
                }
                filteredSerialData = filteredSerialData.Distinct().ToList();

                //filter for the most current spacenav data
                
               

                for (int i = 0; i < filteredSerialData.Count(); i++)
                {
                    inputstring = filteredSerialData.ElementAt(i);
                    if (inputstring.Contains("*******Base Variant") || inputstring.Contains("***MacroKey Variant") || inputstring.Contains("***SpaceNav Variant") || inputstring.Contains("***Absolute Variant") || inputstring.Contains("***Wireless Adapter"))
                    {
                        if (AhmsvilleDialViewModel.Instance.constate == 3 && availableconfigs.Length > 0)
                        {
                            intentionaldisconnect = false;
                            AhmsvilleDialViewModel.Instance.constate = 1;
                            AhmsvilleDialViewModel.Instance.connectionstate += "back to life" + "\n";
                        }
                        else if (AhmsvilleDialViewModel.Instance.constate == 0)
                        {
                            if (processedport != _serialPort.PortName)
                            {
                                string dialreg = inputstring + " at -" + _serialPort.PortName;

                                Array.Resize<string>(ref dialnames, pos + 1);
                                dialnames[pos] = dialreg.Replace("*", "");
                                processedport = _serialPort.PortName;
                                pos += 1;
                                inputstring = "";
                                dialfound = true;


                            }
                        }
                    }
                    else if (inputstring.Contains("|||"))  // detected wireless devices update
                    {
                        Array.Clear(detectedwirelessdials, 0, detectedwirelessdials.Length);  // clear detectedwirelessdials array
                        inputstring = inputstring.Replace("*", "");
                        detectedwirelessdials = (inputstring.Replace("|", "")).ToArray();
                    }

                    else if (inputstring.StartsWith("app is in charge"))  //in app operation data
                    {
                        // _serialPort.DiscardInBuffer();
                        receivedlineindex = Int32.Parse(inputstring.Replace("app is in charge", ""));
                        //MessageBox.Show(inputstring);
                        InApp_Operation(receivedlineindex);


                    }
                    else if (inputstring.StartsWith("<")) //spacenav raw gyro data
                    {
                        inputstring = inputstring.Replace("<", "");
                        int floatstrpos = 0;
                        string[] G_xyrad = new string[3];
                        string[] P_xyrad = new string[3];
                        foreach (char c in inputstring)
                        {
                            if (c == '|')
                            {
                                floatstrpos += 1;
                            }
                            else
                            {
                                G_xyrad[floatstrpos] += c;
                            }
                        }
                        if (!G_xyrad.Contains(null))
                        {
                            
                            if (i < filteredSerialData.Count - 1)
                            {
                                inputstring = filteredSerialData.ElementAt(i + 1);
                                if (inputstring.StartsWith(">"))
                                {   
                                    inputstring = inputstring.Replace(">", "");
                                    floatstrpos = 0;

                                    foreach (char c in inputstring)
                                    {
                                        if (c == '|')
                                        {
                                            floatstrpos += 1;
                                        }
                                        else
                                        {
                                            P_xyrad[floatstrpos] += c;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        P_xyrad[k] = "0";
                                    }
                                }

                            }
                            
                            
                            
                            ProcessSpaceNavData.ProcessSpaceNavDataOBJ.process(activeapp, G_xyrad, P_xyrad);
                            
                        }
                        //System.Console.WriteLine(activeapp);
                       //System.Console.WriteLine("gyro =  " + G_xyrad[0] + "   " + G_xyrad[1] + "   " + G_xyrad[2]);
                        //System.Console.WriteLine("planar =  " + P_xyrad[0] + "   " + P_xyrad[1] + "   " + P_xyrad[2]);
                        // MessageBox.Show(G_xyrad[2] + "   " + P_xyrad[2]);
                    }
                    else if (inputstring.StartsWith(">")) //spacenav raw planar data
                    {
                        inputstring = inputstring.Replace(">", "");
                        int floatstrpos = 0;
                        string[] P_xyrad = new string[3];
                        foreach (char c in inputstring)
                        {
                            if (c == '|')
                            {
                                floatstrpos += 1;
                            }
                            else
                            {
                                P_xyrad[floatstrpos] += c;
                            }
                        }
                        if (!P_xyrad.Contains(null))
                        {
                            string[] G_xyrad = { "0", "0", "0" };
                            //ProcessSpaceNavData.ProcessSpaceNavDataOBJ.process(activeapp, G_xyrad, P_xyrad);
                            //i = filteredSerialData.Count;
                        }
                        //System.Console.WriteLine(activeapp);
                        //System.Console.WriteLine("planar =  " + P_xyrad[0] + "   " + P_xyrad[1] + "   " + P_xyrad[2]);
                        // MessageBox.Show(G_xyrad[2] + "   " + P_xyrad[2]);
                    }
                }
            }







        }
        #endregion

        #region External code Interface
        private void InApp_Operation(int receivedindex)
        {
            string operationtoperform = "";
            try
            {
                operationtoperform = loadedappfunctions[active_loaded_inappfunctions_index].linesforappfunctions[receivedindex];
            }
            catch (Exception)
            {

            }

            var inappoperation_classGroup = AhmsvilleDialViewModel.Instance.inappoperations.GroupBy(x => x.operationclass); //group inbuilt operation into appropriate class
            string classname = "";
            foreach (var OpClass in inappoperation_classGroup)
            {

                foreach (var OP in OpClass)
                {
                    if (OP.operationname == operationtoperform)
                    {
                        classname = OpClass.Key;
                        //MessageBox.Show(classname);
                    }
                }

            }
            if (classname != "") //if operation is built into app and not the location of a file
            {
                if (classname == "MediaControl") //medial control interface
                {
                    Type stringtotype = Type.GetType("Ahmsville_Dial.InApp_Operations." + classname);
                    if (stringtotype != null)
                    {
                        var temptype = (InApp_Operations.MediaControl_INTERFACE)Activator.CreateInstance(stringtotype);
                        temptype.mediacontrol(classname, operationtoperform); //call appropriate inapp operation class

                    }
                }
                else if (classname == "SpaceNavOperations") //spacenav operation interface
                {
                    Type stringtotype = Type.GetType("Ahmsville_Dial.InApp_Operations." + classname);
                    if (stringtotype != null)
                    {
                        var temptype = (InApp_Operations.SpaceNavOperations_INTERFACE)Activator.CreateInstance(stringtotype);
                        temptype.SpaceNav(classname, operationtoperform); //call appropriate inapp operation class
                        //MessageBox.Show(temptype.SpaceNav(classname, operationtoperform)); //call appropriate inapp operation class

                    }
                }
                else if (classname == "SOLIDWORKS") //spacenav operation interface
                {
                    //_MethodInfo classmethod = solidworksInstance.GetType().GetMethod(operationtoperform);
                    //Type classfunc = Type.GetType(operationtoperform);
                    //classmethod.Invoke(this, null);
                    //solidworksInstance.SW_rotatemodel_xpos();

                    Type stringtotype = Type.GetType("Ahmsville_Dial.InApp_Operations." + classname);
                    if (stringtotype != null)
                    {
                        InApp_Operations.SOLIDWORKS.swObj.solidworks(classname, operationtoperform); //call appropriate inapp operation class
                        //solidworksInstance.solidworks(classname, operationtoperform); //call appropriate inapp operation class
                                                                                      //MessageBox.Show(temptype.solidworks(classname, operationtoperform)); //call appropriate inapp operation class

                    }

                }
                else if (classname == "FUSION360") //spacenav operation interface
                {
                    //_MethodInfo classmethod = solidworksInstance.GetType().GetMethod(operationtoperform);
                    //Type classfunc = Type.GetType(operationtoperform);
                    //classmethod.Invoke(this, null);
                    //solidworksInstance.SW_rotatemodel_xpos();

                    Type stringtotype = Type.GetType("Ahmsville_Dial.InApp_Operations." + classname);
                    if (stringtotype != null)
                    {
                        InApp_Operations.FUSION360.f360Obj.fusion360(classname, operationtoperform); //call appropriate inapp operation class
                                                                                     

                    }

                }

            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(operationtoperform);
                }
                catch (Exception)
                {
                    //MessageBox.Show("File/Script @ " +operationtoperform+ " not Found","Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion




        private void goGetDevices()
        {
            //close port
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        int tryturn = 0;
                        while (_serialPort.IsOpen && tryturn < con_try)
                        {

                            _serialPort.Close();
                            tryturn++;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            wired_diallist.Items.Clear();
            wireless_diallist.Items.Clear();
            Array.Clear(detectedwirelessdials, 0, detectedwirelessdials.Length);
            Array.Clear(prevdetectedwirelessdials, 0, prevdetectedwirelessdials.Length);

            AhmsvilleDialViewModel.Instance.connectionstate += "Querying..." + "\n";

            List<string> tomatch = new List<string>();

            tomatch.Add("*******Base Variant");
            tomatch.Add("***MacroKey Variant");
            tomatch.Add("***SpaceNav Variant");
            tomatch.Add("***Absolute Variant");
            tomatch.Add("***Wireless Adapter");

            Action action = getSerialDevicesCallback;

            if (GetSerialDevices._getSerialDevices.getDevices(tomatch, 19, "a", action))
            {
                // MessageBox.Show("released");
            }
        }
        public void getSerialDevicesCallback()
        {

            if (GetSerialDevices.serialdevicelist.Count != 0)
            {
                //AhmsvilleDialViewModel.Instance.constate = 3;
                //_serialPort = null;
                wired_diallist.Items.Clear();
                foreach (string s in GetSerialDevices.serialdevicelist)
                {

                    wired_diallist.Items.Add(s.Replace("*", ""));

                }
                populateWirelessDiallist();
                ConnectDial();
                processedport = "";

            }
            else
            {
                AhmsvilleDialViewModel.Instance.connectionstate += "No Dial Found" + "\n";
            }
            reconTimer.Enabled = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Adds the windows message processing hook and registers USB device add/removal notification.
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null)
            {
                windowHandle = source.Handle;
                source.AddHook(HwndHandler);
                UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }

        /// <summary>
        /// Method that receives window messages.
        /// </summary>
        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == UsbNotification.WmDevicechange)
            {
                switch ((int)wparam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        Usb_DeviceRemovedAsync(); // this is where you do your magic
                        break;
                    case UsbNotification.DbtDevicearrival:
                        Usb_DeviceAddedAsync(); // this is where you do your magic
                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

        private void Usb_DeviceRemovedAsync()
        {
            reconTimer.Enabled = false;
            deviceadded = false;
            deviceremoved = true;
            reconTimer.Enabled = true;
        }
        private void Usb_DeviceAddedAsync()
        {

            reconTimer.Enabled = false;
            deviceremoved = false;
            deviceadded = true;
            reconTimer.Enabled = true;

        }


        private void RadioButton_Switchmode(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null)
            {
                string mode = rb.Tag.ToString();
                switch (mode)
                {
                    case "AutoSW":
                        AutoSwitching.Visibility = System.Windows.Visibility.Visible;
                        ManualSwitching.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "ManualSW":
                        AutoSwitching.Visibility = System.Windows.Visibility.Collapsed;
                        ManualSwitching.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
        }

        private void Button_Dialconfig(object sender, RoutedEventArgs e)
        {
            DialConfiguration dialConfiguration = new DialConfiguration();
            dialConfiguration.Show();
        }

        /******************************************************************************************************************/
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var handle = GetForegroundWindow();
            // Obtain the length of the text   
            var intLength = GetWindowTextLength(handle) + 1;
            var stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) > 0)
            {
                activeapp = "";
                activeapp = stringBuilder.ToString();
                if (prevactiveapp != activeapp) //app has changed
                {
                    try
                    {
                        if (_serialPort != null)
                        {
                            if (!_serialPort.IsOpen)
                            {
                                AhmsvilleDialViewModel.Instance.constate = 2;
                            }
                            else
                            {
                                if (AhmsvilleDialViewModel.Instance.constate == 1)
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        ChangeActiveDialConfig();
                                        prevactiveapp = activeapp;
                                    });
                                }

                            }
                        }
                        

                    }
                    catch (Exception)
                    {

                    }
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                populateWirelessDiallist();
                if (!intentionaldisconnect)
                {
                    ConnectDial();
                }

            });
            try
            {
                if (_serialPort != null)
                {
                    _serialPort.DtrEnable = true;
                    _serialPort.RtsEnable = true;
                }
               
            }
            catch (Exception)
            {

            }


        }

        private void reconnectTimerEvent(Object source, ElapsedEventArgs e)
        {
            if (deviceadded)
            {
                AhmsvilleDialViewModel.Instance.constate = 0;
                is_wireless = false;
                this.Dispatcher.Invoke(() =>
                {
                    goGetDevices();
                });
                deviceadded = false;
            }
            else if (deviceremoved)
            {
                if (_serialPort != null)
                {
                    if (!_serialPort.IsOpen)
                    {
                        AhmsvilleDialViewModel.Instance.constate = 2;
                        AhmsvilleDialViewModel.Instance.connectionstate += "Disconnected" + "\n";

                        //getavailablePorts();
                        this.Dispatcher.Invoke(() =>
                        {
                            goGetDevices();
                        });
                        deviceremoved = false;
                        // RefreshPortList();
                        //reconTimer.Enabled = false;
                    }
                }
            }

        }
        private void SetTimer()
        {
            // Create a timer for normal id update.
            aTimer = new System.Timers.Timer(500);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;


            //set reconnection timer
            reconTimer = new System.Timers.Timer(8000);
            reconTimer.AutoReset = true;
            reconTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            reconTimer.Elapsed += reconnectTimerEvent;
        }


        private void ChangeActiveDialConfig()
        {
            if (AutoSW.IsChecked == true)
            {
                AhmsvilleDialViewModel.Instance.activeappname = activeapp;
                configfound = false;
                foreach (string s in availableconfigs)
                {

                    //MessageBox.Show(i.ToString());
                    if (activeapp.Contains(s.Replace("_", " ")))
                    {
                        if (AhmsvilleDialViewModel.Instance.constate == 1)
                        {

                            try
                            {

                                _serialPort.Write((Array.IndexOf(availableconfigs, s)).ToString()); //change dial's active application index
                                active_loaded_inappfunctions_index = Array.IndexOf(availableconfigs, s); //change inApp active application index
                                _serialPort.DtrEnable = true;
                                _serialPort.RtsEnable = true;
                                //_serialPort.DiscardInBuffer();

                            }
                            catch (Exception)
                            {
                                //AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");

                            }
                        }


                        AhmsvilleDialViewModel.Instance.activeappid = (Array.IndexOf(availableconfigs, s)).ToString();
                        configfound = true;



                    }
                    else if (configfound == false && Array.IndexOf(availableconfigs, s) == (availableconfigs.Length - 1))
                    {
                        try
                        {
                            if (AhmsvilleDialViewModel.Instance.constate == 1)
                            {
                                _serialPort.Write(defaultconfigID.ToString()); //set dial active application to default 
                                active_loaded_inappfunctions_index = defaultconfigID; //set inapp active application to default index (0)
                                _serialPort.DtrEnable = true;
                                _serialPort.RtsEnable = true;
                                //_serialPort.
                            }
                        }
                        catch (Exception)
                        {
                            intentionaldisconnect = true;
                            AhmsvilleDialViewModel.Instance.constate = 3;
                            AhmsvilleDialViewModel.Instance.connectionstate += "no one's home" + "\n";
                            //AhmsvilleDialViewModel.Instance.connectionstate = ("Unauthorized Access");

                        }


                        AhmsvilleDialViewModel.Instance.activeappid = "no config found for active app";
                    }
                }
            }
        }

        private void DiscoverAvailableAppConfigs()
        {
            bool defaultset = false;
            int cindex = 0;
            string cname = "";
            // Put all config files in root directory into array.
            string[] configfiles = Directory.GetFiles(connecteddialconfigpath, "*_.ino"); // <-- Case-insensitive

            loadedappfunctions = new availableconfigsforapp[configfiles.Length];



            Array.Resize<string>(ref availableconfigs, configfiles.Length);

            for (int i = 0; i < configfiles.Length; i++)
            {
                loadedappfunctions[i].linesforappfunctions = new string[40];
            }

            int cnt = 0;
            foreach (string s in configfiles)
            {

                string[] configsettings = System.IO.File.ReadAllLines(configfiles[cnt]);


                string configline = configsettings[2];  //line for name
                int substringindex = configline.LastIndexOf("=");
                if (substringindex > 0)
                {
                    string sub = configline.Substring(substringindex);
                    sub = sub.Replace("=", "");
                    sub = sub.Replace(";", "");
                    sub = sub.Replace("\"", "");
                    sub = sub.Replace("'", "");
                    sub = sub.Replace("CRGB::", "");

                    cname = sub;

                }
                configline = configsettings[3];  //line for index
                substringindex = configline.LastIndexOf("=");
                if (substringindex > 0)
                {
                    string sub = configline.Substring(substringindex);
                    sub = sub.Replace("=", "");
                    sub = sub.Replace(";", "");
                    sub = sub.Replace("\"", "");
                    sub = sub.Replace("'", "");
                    sub = sub.Replace("CRGB::", "");

                    if (sub.Contains("//default"))
                    {
                        if (defaultset == false)
                        {
                            sub = sub.Replace("//default", "");
                            defaultconfigID = Int32.Parse(sub) - 1;
                            defaultset = true;
                        }
                        else
                        {
                            AhmsvilleDialViewModel.Instance.connectionstate += "Multiple Defaults Set, Correct this in the configurations" + "\n";
                            sub = sub.Replace("//default", "");
                        }

                    }

                    cindex = Int32.Parse(sub);

                }

                if (cindex <= availableconfigs.Length)
                {
                    //add read config name to availableconfigs array
                    availableconfigs[cindex - 1] = cname;

                    //load inapp functions by line from configsettings
                    int linecnt = 0;
                    foreach (string line in configsettings)
                    {
                        if (line.Contains("functionscombobox"))
                        {
                            configline = line;
                            substringindex = configline.LastIndexOf("=");
                            if (substringindex > 0)
                            {
                                string sub = configline.Substring(substringindex);
                                sub = sub.Replace("=", "");
                                sub = sub.Replace(";", "");
                                sub = sub.Replace("\"", "");
                                sub = sub.Replace("'", "");
                                sub = sub.Replace("CRGB::", "");
                                loadedappfunctions[cindex - 1].linesforappfunctions[linecnt] = sub;

                                linecnt = linecnt + 1;
                            }
                        }
                    }
                    cnt = cnt + 1;
                    //MessageBox.Show(cname + "@" + cindex.ToString());
                }
                else
                {
                    MessageBox.Show("Invalid ID among configurations", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                    // reorder config ID
                    /* PopulateConfigList();
                     for (int i = 0; i < configlist.Items.Count; i++)
                     {
                         configlist.SelectedIndex = i;
                         ReOrderConfigID(i + 1);
                     }*/
                    //Rewrite_ConfigLoader();

                    // MessageBox.Show("Re-order configurations ???");

                }


            }



        }

        private void ConnectedDialType()
        {
            if (is_wireless) //connectec device is wireless adapter
            {
                if (wireless_diallist.SelectedItem != null)
                {
                    if (wireless_diallist.SelectedItem.ToString().Contains("Base"))
                    {
                        connecteddialconfigpath = dialconfigpath[0];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("MacroKey"))
                    {
                        connecteddialconfigpath = dialconfigpath[1];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("SpaceNav"))
                    {
                        connecteddialconfigpath = dialconfigpath[2];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("Absolute"))
                    {
                        connecteddialconfigpath = dialconfigpath[3];
                        DiscoverAvailableAppConfigs();
                    }
                }

            }
            else
            {
                string connecteddial = wired_diallist.SelectedItem.ToString();
                for (int i = 0; i < dialvariants.Length; i++)
                {
                    if (connecteddial.Contains(dialvariants[i]))  //dial connected via cable
                    {
                        connecteddialconfigpath = dialconfigpath[i];
                        //MessageBox.Show(connecteddialconfigpath);
                        DiscoverAvailableAppConfigs();
                    }
                }
            }

        }

        private void populateWirelessDiallist()
        {
            if (detectedwirelessdials.Length == 4)
            {
                int addeddevices = 0;
                if (!detectedwirelessdials.SequenceEqual(prevdetectedwirelessdials)) //continue if a new dial is detected wirelessly
                {

                    if (detectedwirelessdials.Contains('1')) //array is not all zeros
                    {

                        wireless_diallist.Items.Clear();
                        if (detectedwirelessdials[0] == '1')
                        {
                            wireless_diallist.Items.Add("Base Variant");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[1] == '1')
                        {
                            wireless_diallist.Items.Add("MacroKey Variant");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[2] == '1')
                        {
                            wireless_diallist.Items.Add("SpaceNav Variant");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[3] == '1')
                        {
                            wireless_diallist.Items.Add("Absolute Variant");
                            addeddevices += 1;
                        }
                        if (prevdetectedwirelessdials.Length != 0)
                        {
                            Array.Copy(detectedwirelessdials, prevdetectedwirelessdials, 4);
                        }

                        AhmsvilleDialViewModel.Instance.connectionstate += "New wireless Dial detected, select dial from list" + "\n";
                        if (wireless_diallist.Items.Count == 1)
                        {
                            AhmsvilleDialViewModel.Instance.constate = 1;
                            wireless_diallist.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        wireless_diallist.Items.Clear();
                        prevdetectedwirelessdials = detectedwirelessdials;
                    }
                }
            }

        }

        private void Reprogram_dial_Click(object sender, RoutedEventArgs e)
        {
            modifyvariantfile();
            //MessageBox.Show(wireless_diallist.Text);
            if (AhmsvilleDialViewModel.Instance.constate == 1)
            {
                try
                {
                    int tryturn = 0;
                    while (_serialPort.IsOpen && tryturn < con_try)
                    {
                        AhmsvilleDialViewModel.Instance.connectionstate += "closing port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString() + "\n";
                        _serialPort.Close();
                        tryturn++;
                    }
                }
                catch (Exception)
                {
                    AhmsvilleDialViewModel.Instance.connectionstate += "Unauthorized Access" + "\n";
                }
                wired_diallist.IsEnabled = true;
                AhmsvilleDialViewModel.Instance.constate = 2;
                AhmsvilleDialViewModel.Instance.connectionstate += "disconnected from dial" + "\n";

            } //disconnect from dial so arduino can program the dial

            if (AutoSW.IsChecked == true)
            {
                try
                {
                    selectdialversionFromLibrary();
                    System.Diagnostics.Process.Start(connecteddialconfigpath + dialmain_arduinofileByVersion[Array.IndexOf(dialconfigpath, connecteddialconfigpath)]);

                }
                catch (Exception)
                {
                    if (connecteddialconfigpath != "")
                    {
                        if (wired_diallist.SelectedItem == null)
                        {
                            MessageBox.Show("Connect to Dial first", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);

                        }
                        else
                        {
                            MessageBox.Show("Main *ino file for connected Dial version is missing", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (ports.Length == 0)
                    {
                        MessageBox.Show("Connect a Dial first", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Connect to Dial first", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //var Result = MessageBox.Show("No Dial is Connected, \nProceed by selecting your dial variant", "Ahmsville Dial", MessageBoxButton, MessageBoxImage.Question);

                    }

                } //open main arduino file for connected dial
            }
            else
            {
                if (manualSWdiallist.SelectedItem != null)//a dial is selected
                {
                    try
                    {
                        selectdialversionFromLibrary(manualSWdiallist.SelectedIndex);
                        System.Diagnostics.Process.Start(dialconfigpath[manualSWdiallist.SelectedIndex] + dialmain_arduinofileByVersion[manualSWdiallist.SelectedIndex]);

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Main *ino file for connected Dial variant not found", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Select a Dial Variant", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }


        private void selectdialversionFromLibrary()
        {

            //checked for directly connected dial
            if (wired_diallist.SelectedItem.ToString().Contains("Absolute"))
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 4 //v2 Samd21 Absolute";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (wired_diallist.SelectedItem.ToString().Contains("Base"))
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 1 //v2 Samd21 Base";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (wired_diallist.SelectedItem.ToString().Contains("MacroKey"))
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 2 //v2 Samd21 Macro";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (wired_diallist.SelectedItem.ToString().Contains("SpaceNav"))
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 3 //v2 Samd21 SpaceNav";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

        }
        private void selectdialversionFromLibrary(int index)
        {

            //checked for directly connected dial
            if (index == 3)
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 4 //v2 Samd21 Absolute";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (index == 0)
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 1 //v2 Samd21 Base";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (index == 1)
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 2 //v2 Samd21 Macro";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (index == 2)
            {
                //modify header file
                try
                {
                    string headerfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\";
                    string[] headerfile = System.IO.File.ReadAllLines(headerfilepath + "AhmsvilleDial_v2.h");
                    // string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                    //  string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(headerfilepath + "AhmsvilleDial_v2.h"))
                    {
                        for (int i = 0; i < headerfile.Length; i++)
                        {
                            if (i == 5) //Write to line 5
                            {
                                headerfile[i] = "";

                                headerfile[i] = "#define DIAL_VERSION 3 //v2 Samd21 SpaceNav";
                            }
                            file.WriteLine(headerfile[i]);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

        }

        private void modifyvariantfile()
        {
            try
            {
                string variantpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Arduino15\packages\arduino\hardware\samd";
                string[] foldersinpath = Directory.GetDirectories(variantpath);
                if (foldersinpath.Length == 1)//folders in path is == 1
                {
                    foldersinpath[0] += @"\variants\arduino_zero";
                    File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + @"\Configfiles\variant.cpp", foldersinpath[0] + @"\variant.cpp", true);
                }
                //MessageBox.Show(foldersinpath[0]);
            }
            catch (Exception)
            {

            }


        }

        private void wireless_diallist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (wireless_diallist.SelectedItem != null)
            {
                ConnectedDialType();
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            ManualID_Update();
        }

        private void manualid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ManualID_Update();
            }
        }
        private void ManualID_Update()
        {
            string m_ID = manualid.Text;
            int IDint = 0;
            try
            {
                IDint = Int32.Parse(m_ID);
                if (IDint < availableconfigs.Length)
                {
                    if (AhmsvilleDialViewModel.Instance.constate == 1)
                    {

                        try
                        {

                            _serialPort.Write(m_ID); //change dial's active application index
                            active_loaded_inappfunctions_index = IDint; //change inApp active application index
                            _serialPort.DtrEnable = true;
                            _serialPort.RtsEnable = true;
                            //_serialPort.DiscardInBuffer();

                        }
                        catch (Exception)
                        {
                            AhmsvilleDialViewModel.Instance.connectionstate += "Unauthorized Access" + "\n";

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Configuration Identifier", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                bool strconffound = false;
                foreach (string s in availableconfigs)
                {

                    if (s.Contains(m_ID))
                    {
                        if (AhmsvilleDialViewModel.Instance.constate == 1)
                        {

                            try
                            {

                                _serialPort.Write((Array.IndexOf(availableconfigs, s)).ToString()); //change dial's active application index
                                active_loaded_inappfunctions_index = Array.IndexOf(availableconfigs, s); //change inApp active application index
                                _serialPort.DtrEnable = true;
                                _serialPort.RtsEnable = true;
                                //_serialPort.DiscardInBuffer();

                            }
                            catch (Exception)
                            {
                                AhmsvilleDialViewModel.Instance.connectionstate += "Unauthorized Access" + "\n";

                            }
                        }
                        strconffound = true;
                    }

                }
                if (!strconffound)
                {
                    MessageBox.Show("Invalid Configuration Identifier", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            manualid.Clear();
        }

        private void connectionstate_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (connectionstate.LineCount >= 15)
            {

                AhmsvilleDialViewModel.Instance.connectionstate = AhmsvilleDialViewModel.Instance.connectionstate.Remove(0, AhmsvilleDialViewModel.Instance.connectionstate.IndexOf("\n") + 1);

            }
            connectionstate.ScrollToEnd();
        }

        private void connectionstate_MouseEnter(object sender, MouseEventArgs e)
        {
            connectionstate.ScrollToEnd();
            connectionstate.Height = 200;

        }

        private void connectionstate_MouseLeave(object sender, MouseEventArgs e)
        {
            connectionstate.ScrollToEnd();
            connectionstate.Height = 30;

        }
        private void updateLibrary_Click(object sender, RoutedEventArgs e)
        {
            var answer = MessageBox.Show("This will overite any custom changes to all associated libraries \n Are you sure you want to continue?", "Ahmsville Dial", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"\Libraries\Extracted"); //create temp directory
                    ZipFile.ExtractToDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"\Libraries\Ahmsville Dial Libraries.zip", System.AppDomain.CurrentDomain.BaseDirectory + @"\Libraries\Extracted"); //extract libraries to temp directory
                    string librarylocation = System.AppDomain.CurrentDomain.BaseDirectory + @"\Libraries\Extracted";
                    string libraryfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries";
                    string[] libraries = Directory.GetDirectories(librarylocation);


                    foreach (string s in libraries)
                    {
                        string dest = libraryfolder + s.Replace(librarylocation, "");
                        Directory.CreateDirectory(dest);
                        copyfilesRecursively(s, dest);
                        // MessageBox.Show(System.IO.Path.GetFileName(s));
                        //File.Copy(s, libraryfolder);
                    }
                    Directory.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\Libraries\Extracted", true);
                    MessageBox.Show("Libraries Updated", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Run the App as Administrator to use this function", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {

            }

        }
        private void copyfilesRecursively(string source, string dest)
        {
            try
            {
                string[] files = Directory.GetFiles(source);
                string[] folders = Directory.GetDirectories(source);
                foreach (string s in files)
                {
                    string filename = dest + @"\" + System.IO.Path.GetFileName(s);
                    File.Copy(s, filename, true); //copy files to dest
                }
                foreach (string s in folders)
                {
                    string newdest = dest + @"\" + System.IO.Path.GetFileName(s);
                    Directory.CreateDirectory(newdest); //create folder in dest
                    copyfilesRecursively(s, newdest);

                }
            }
            catch (Exception)
            {

            }
        }

        private void reset_dial_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.Write("r");
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }
        }
    }
    /*************************************************************************************************************************************/
    internal static class UsbNotification
    {
        public const int DbtDevicearrival = 0x8000; // system detected a new device        
        public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
        public const int WmDevicechange = 0x0219; // device change event      
        private const int DbtDevtypDeviceinterface = 5;
        private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
        private static IntPtr notificationHandle;

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = GuidDevinterfaceUSBDevice,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        /// <summary>
        /// Unregisters the window for USB device notifications
        /// </summary>
        public static void UnregisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }


        /***************************************************************************************************************************************************/

    }
}

