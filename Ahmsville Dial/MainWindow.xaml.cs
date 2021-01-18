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


namespace Ahmsville_Dial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer COMportQTimer;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);


        static SerialPort _serialPort;
        // Get a list of serial port names.
        static string[] ports = SerialPort.GetPortNames();
        static string inputstring = "";
        static string inputstring2 = "";
        static bool dialfound = false;
        static string[] dialnames = { };
        static string processedport = "";
        static int pos = 0;
        static int con_try = 10;

        static bool is_wireless = false;
        static bool intentionaldisconnect = false;
        static int receivedlineindex = 0;
        bool configfound = false;
        int defaultconfigID = 0;
        //bool timedeventhandlingisdone = true;
        string activeapp;
        string prevactiveapp;
        //path for Dial's config files and library files 
        string[] dialconfigpath = { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Ahmsville_Dial_Base_version_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Ahmsville_Dial_Macro_version_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Ahmsville_Dial_SpaceNav_version_main\",
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Ahmsville_Dial_Absolute_version_main\"};
       
        string connecteddialconfigpath = "";
        string[] dialversions = { "base", "mk", "spacenav", "absolute" };
        char[] detectedwirelessdials = { '0', '0', '0', '0' };
        char[] prevdetectedwirelessdials = { '0', '0', '0', '0' };
        string[] dialmain_arduinofileByVersion = {"Ahmsville_Dial_Base_version_main.ino",
            "Ahmsville_Dial_Macro_version_main.ino",
            "Ahmsville_Dial_SpaceNav_version_main.ino",
            "Ahmsville_Dial_Absolute_version_main.ino" };
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

        public MainWindow()
        {
            DataContext = new AhmsvilleDialViewModel();
            InitializeComponent();
            AutoSW.IsChecked = true;
            SetTimer();
            AhmsvilleDialViewModel.Instance.constate = 0;
            getavailablePorts();
            //RefreshPortList();
            //ConnectDial();






        }

        private void ConnectDial()
        {
            if (AhmsvilleDialViewModel.Instance.constate != 1)
            {
                if (dialnames.Length == 1) //connect to the only available device
                {
                    wired_diallist.SelectedIndex = 0;

                    //AhmsvilleDialViewModel.Instance.connectionstate = (wired_diallist.SelectedItem.ToString() + " ---selected automatically");
                    if (!_serialPort.IsOpen)
                    {
                        string selecteddialport = wired_diallist.SelectedItem.ToString();
                        selecteddialport = selecteddialport.Substring(selecteddialport.IndexOf('-') + 1);
                        SwitchSerialPort(selecteddialport); //get selected port name
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
                getavailablePorts();
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
                            selecteddialport = selecteddialport.Substring(selecteddialport.IndexOf('-') + 1);
                            SwitchSerialPort(selecteddialport);
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
                    getavailablePorts();
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
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
            _serialPort.DataReceived += DataReceivedHandler;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            char[] inChar = new char[19];
            try
            {
                _serialPort.Read(inChar, 0, 19);
                inputstring = new string(inChar);


            }
            catch (Exception)
            {
                //connectionstate.Text = "ConnectDialion closed";
            }
            if (inputstring.Contains("***************base") || inputstring.Contains("*****************mk") || inputstring.Contains("***********spacenav") || inputstring.Contains("***********absolute") || inputstring.Contains("***********Wireless"))
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
                        string dialreg = inputstring + " Version" + " at -" + _serialPort.PortName;
                      
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

                char[] inChar2 = new char[19];
                try
                {
                    _serialPort.Read(inChar2, 0, 19);  //grab second set of spacenav data (planar)
                    inputstring2 = new string(inChar2);


                }
                catch (Exception)
                {
                    //connectionstate.Text = "ConnectDialion closed";
                }

                if (inputstring2.StartsWith(">")) //spacenav raw gyro data
                {
                    inputstring2.Replace(">", "");
                    floatstrpos = 0;

                    foreach (char c in inputstring2)
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
                //MessageBox.Show(G_xyrad[2] + "   " + P_xyrad[2]);
            }





        }

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

                Type stringtotype = Type.GetType("Ahmsville_Dial.InApp_Operations." + classname);
                if (stringtotype != null)
                {
                    var temptype = (InApp_Operations.InAppOperations_Interface)Activator.CreateInstance(stringtotype);
                    temptype.PerformInAppOperation(classname, operationtoperform); //call appropriate inapp operation class

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
        private void getavailablePorts()
        {
            /********************disconnect from connected serial device********************/
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

            /*************************refresh connection variables***************************************/
            string[] portsrefresh = SerialPort.GetPortNames();
            portsrefresh = portsrefresh.Distinct().ToArray();
            Array.Resize<string>(ref ports, portsrefresh.Length);
            Array.Copy(portsrefresh, ports, portsrefresh.Length);
            Array.Clear(portsrefresh, 0, portsrefresh.Length);
            processedport = "";
            pos = 0;
            Array.Resize<string>(ref dialnames, 0);
            wired_diallist.Items.Clear();
            //wireless_diallist.Items.Clear();
            Array.Resize<char>(ref detectedwirelessdials, 0);
            AhmsvilleDialViewModel.Instance.connectionstate += "querying..." + "\n";
            AhmsvilleDialViewModel.Instance.constate = 0;
            activeQueryportnum = 0;
            aTimer.Enabled = false;
            RefreshPortList();
        }
        private void RefreshPortList()
        {
            /*********************query available ports*****************************/

            if (activeQueryportnum < ports.Length)
            {
                //activeQueryportnum += 1;



                try  //close active port
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
                if (!AhmsvilleDialViewModel.Instance.connectionstate.EndsWith("donewithquery" + "\n"))
                {
                    try //open next port to query
                    {
                        SwitchSerialPort(ports[activeQueryportnum]);
                        int tryturn = 0;
                        while (!_serialPort.IsOpen && tryturn < con_try)
                        {
                            //AhmsvilleDialViewModel.Instance.connectionstate = ("connecting to port- " + _serialPort.PortName.ToString() + " try - " + tryturn.ToString());
                            _serialPort.Open();
                            tryturn++;
                        }

                    }
                    catch (Exception)
                    {
                        AhmsvilleDialViewModel.Instance.connectionstate += "this serial port no longer exists" + "\n";
                    }
                }

                COMportQTimer.Enabled = true;
                if (dialfound)  //if dial is found after quering coonected serial devices
                {
                    AhmsvilleDialViewModel.Instance.connectionstate += "new Dial found" + "\n";
                    dialfound = false;
                }
            }
            else //disable query timer
            {
                COMportQTimer.Enabled = false;
                AhmsvilleDialViewModel.Instance.connectionstate += "donewithquery" + "\n";
                try  //close active port
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
                populatedDeviceList();
                aTimer.Enabled = true;
            }


        }

        private void populatedDeviceList()
        {
            if (AhmsvilleDialViewModel.Instance.connectionstate.EndsWith("donewithquery" + "\n"))
            {
                if (dialnames.Count() != 0)
                {     
                    wired_diallist.Items.Clear();
                    foreach (string s in dialnames)
                    {
                        wired_diallist.Items.Add(s);

                    }
                    populateWirelessDiallist();
                    ConnectDial();
                    processedport = "";
                }
                /**********************************************************************/
            }
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

        private async Task Usb_DeviceRemovedAsync()
        {
            await Task.Delay(3000);
            if (!_serialPort.IsOpen)
            {

                AhmsvilleDialViewModel.Instance.constate = 2;
                AhmsvilleDialViewModel.Instance.connectionstate += "Disconnected" + "\n";
                getavailablePorts();
                // RefreshPortList();
            }



        }
        private async Task Usb_DeviceAddedAsync()
        {

            await Task.Delay(5000);
            AhmsvilleDialViewModel.Instance.constate = 2;
            is_wireless = false;


            getavailablePorts();
            // RefreshPortList();




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
                _serialPort.DtrEnable = true;
                _serialPort.RtsEnable = true;
            }
            catch (Exception)
            {

            }


        }

        private void COMportQTimerEvent(Object source, ElapsedEventArgs e)
        {
            // int numofportstoquery = ports.Length;
            //int currentqueryportnum = 0;
            //int querycnt = 0;
            //query
            try
            {
                _serialPort.Write("a");
                _serialPort.DtrEnable = true;
                _serialPort.RtsEnable = true;
            }
            catch (Exception)
            {

            }
            if (querycnt < querynum)
            {
                querycnt += 1;
            }
            else //change to next comport
            {
                activeQueryportnum += 1;
                COMportQTimer.Enabled = false; //temporarily disable timer
                querycnt = 0;
                this.Dispatcher.Invoke(() =>
                {
                    RefreshPortList();
                });

            }
        }
        private void SetTimer()
        {
            // Create a timer for normal id update.
            aTimer = new System.Timers.Timer(500);
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Create a timer for com port query
            COMportQTimer = new System.Timers.Timer(200);
            COMportQTimer.AutoReset = true;
            COMportQTimer.Enabled = false;
            // Hook up the Elapsed event for the timer. 
            COMportQTimer.Elapsed += COMportQTimerEvent;

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
                    if (activeapp.Contains(s))
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
                    if (wireless_diallist.SelectedItem.ToString().Contains("base"))
                    {
                        connecteddialconfigpath = dialconfigpath[0];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("mk"))
                    {
                        connecteddialconfigpath = dialconfigpath[1];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("spacenav"))
                    {
                        connecteddialconfigpath = dialconfigpath[2];
                        DiscoverAvailableAppConfigs();
                    }
                    else if (wireless_diallist.SelectedItem.ToString().Contains("absolute"))
                    {
                        connecteddialconfigpath = dialconfigpath[3];
                        DiscoverAvailableAppConfigs();
                    }
                }

            }
            else
            {
                string connecteddial = wired_diallist.SelectedItem.ToString();
                for (int i = 0; i < dialversions.Length; i++)
                {
                    if (connecteddial.Contains(dialversions[i]))  //dial connected via cable
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
                            wireless_diallist.Items.Add("base");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[1] == '1')
                        {
                            wireless_diallist.Items.Add("mk");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[2] == '1')
                        {
                            wireless_diallist.Items.Add("spacenav");
                            addeddevices += 1;
                        }
                        if (detectedwirelessdials[3] == '1')
                        {
                            wireless_diallist.Items.Add("absolute");
                            addeddevices += 1;
                        }
                        if (prevdetectedwirelessdials.Length != 0)
                        {
                            Array.Copy(detectedwirelessdials, prevdetectedwirelessdials, 4);
                        }

                        AhmsvilleDialViewModel.Instance.connectionstate += "New wireless Dial detected, select dial from list" + "\n";


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
                }

            } //nbnbmbmopen main arduino file for connected dial

        }


        private void selectdialversionFromLibrary()
        {
            //checked for directly connected dial
            if (wired_diallist.SelectedItem.ToString().Contains("absolute"))
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
            else if (wired_diallist.SelectedItem.ToString().Contains("base"))
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
            else if (wired_diallist.SelectedItem.ToString().Contains("mk"))
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
            else if (wired_diallist.SelectedItem.ToString().Contains("spacenav"))
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

