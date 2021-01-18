using System;
using System.IO;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Ahmsville_Dial
{
    /// <summary>
    /// Interaction logic for DialConfiguration.xaml
    /// </summary>
    public partial class DialConfiguration : Window
    {
       

        double faded = 0.5, opaque = 1;
        string selecteddialfilepath; //path for Dial's config files and library files 
        static string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;  //path for software related files
        int configcount;
        string nullchar = "128"; //classified as null for arduino

        //dial imagepath array
        string selecteddialimagepath; //path to dial images
        


        //static string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;  //path for software related files
        public DialConfiguration()
        {
            
            InitializeComponent();
            /*baseimage.Source = new BitmapImage(new Uri(softwarepath + @"\base\default.png"));
            
            macroimage.Source = new BitmapImage(new Uri(softwarepath + @"\macro\default.png"));
            spacenavimage.Source = new BitmapImage(new Uri(softwarepath + @"\spacenav\default.png"));
            absoluteimage.Source = new BitmapImage(new Uri(softwarepath + @"\absolute\default.png"));
            */
            DataContext = new AhmsvilleDialViewModel();

            //AhmsvilleDialViewModel.Instance.diallistindex = 3;
           diallist.SelectedIndex = 3;
           // loaddialinfo();
            // dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath[10]));
            // DataContext = new AhmsvilleDialViewModel();

            spacenav_manual.IsChecked = true;   //select default space navigator mode
            configname.Text = "NewAppConfig";
            PopulateConfigList();  //load configuration selection
            configid.IsEnabled = false;
        }

        

        public void loaddialinfo()
        {
      
            var dialinfolist = AhmsvilleDialViewModel.Instance.dialinfo;
            dialinfolist = dialinfolist.GetRange(diallist.SelectedIndex,1);
           // dialinfolist = dialinfolist.Where(x => x.name == diallist.Text).ToList();

            foreach (var selecteddial in dialinfolist)
            {
                selecteddialfilepath = selecteddial.filepath;
                //selecteddialimagepath = selecteddialimagepath.cl
                selecteddialimagepath = selecteddial.imagepath.Replace(@"\default.jpg","");

            }
            PopulateConfigList();  //load configuration selection

        }

       
        private void OpenSettingsPage(Object sender, SelectionChangedEventArgs e)
        {
            loaddialinfo();
            string mode = diallist.Items.IndexOf(diallist.SelectedItem).ToString();
            if (mode != null)
            {
                switch (mode)
                {
                    case "0":
                        knob1.Visibility = System.Windows.Visibility.Visible;
                        knob2.Visibility = System.Windows.Visibility.Collapsed;
                        capacitivetouch.Visibility = System.Windows.Visibility.Visible;
                        mk1.Visibility = System.Windows.Visibility.Collapsed;
                        mk2.Visibility = System.Windows.Visibility.Collapsed;
                        mk3.Visibility = System.Windows.Visibility.Collapsed;
                        mk4.Visibility = System.Windows.Visibility.Collapsed;
                        mk5.Visibility = System.Windows.Visibility.Collapsed;
                        spacenav.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "1":
                        knob1.Visibility = System.Windows.Visibility.Visible;
                        knob2.Visibility = System.Windows.Visibility.Collapsed;
                        capacitivetouch.Visibility = System.Windows.Visibility.Visible;
                        mk1.Visibility = System.Windows.Visibility.Visible;
                        mk2.Visibility = System.Windows.Visibility.Visible;
                        mk3.Visibility = System.Windows.Visibility.Visible;
                        mk4.Visibility = System.Windows.Visibility.Visible;
                        mk5.Visibility = System.Windows.Visibility.Visible;
                        spacenav.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "2":
                        knob1.Visibility = System.Windows.Visibility.Visible;
                        knob2.Visibility = System.Windows.Visibility.Visible;
                        capacitivetouch.Visibility = System.Windows.Visibility.Visible;
                        mk1.Visibility = System.Windows.Visibility.Collapsed;
                        mk2.Visibility = System.Windows.Visibility.Collapsed;
                        mk3.Visibility = System.Windows.Visibility.Collapsed;
                        mk4.Visibility = System.Windows.Visibility.Collapsed;
                        mk5.Visibility = System.Windows.Visibility.Collapsed;
                        spacenav.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "3":
                        knob1.Visibility = System.Windows.Visibility.Visible;
                        knob2.Visibility = System.Windows.Visibility.Visible;
                        capacitivetouch.Visibility = System.Windows.Visibility.Visible;
                        mk1.Visibility = System.Windows.Visibility.Visible;
                        mk2.Visibility = System.Windows.Visibility.Visible;
                        mk3.Visibility = System.Windows.Visibility.Visible;
                        mk4.Visibility = System.Windows.Visibility.Visible;
                        mk5.Visibility = System.Windows.Visibility.Visible;
                        spacenav.Visibility = System.Windows.Visibility.Visible;
                        break;
                    

                }
            }
           
        }


        private void Configlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //check if an existing config is selected and load the previous shortcut setting
            if (configlist.SelectedValue != null)
            {
                configname.Text = ((ComboBoxItem)this.configlist.SelectedItem).Name.ToString();
                LoadSettingsFromConfigFile();
            }
            else
            {
                configlist.SelectedIndex = 0;
            }

        }

        private void PopulateConfigList()
        {
            // Put all config files in root directory into array.
            configlist.Items.Clear();
            string[] configfiles = { };
            try
            {
                configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
            }
            catch (Exception)
            {

            }
            //string[] configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
                                                                           // fill configuration name combobox
            foreach (string fullconfigpath in configfiles)
            {
                string configname = fullconfigpath.Replace(selecteddialfilepath, "");
                configname = configname.Replace("_.ino", "");
                bool contains = false;
                for (int i = 0; i < configlist.Items.Count; i++)
                {
                    if (configlist.Items.GetItemAt(i).ToString().EndsWith(configname) == true)
                    {
                        contains = true;
                    }
                }
                if (contains == false)
                {
                    ComboBoxItem temp = new ComboBoxItem();
                    temp.Name = configname;
                    temp.Content = configname;
                    temp.Tag = fullconfigpath;
                    configlist.Items.Add(temp);
                }
            }
            configcount = configfiles.Length;

            if (ValidateExistingConfig() != true)
            {
                MessageBox.Show("Invalid ID among configurations", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);

                for (int i = 0; i < configfiles.Length; i++)
                {
                    string thisconfigisdefault = "\n";
                    try
                    {
                        string[] tempconfig = System.IO.File.ReadAllLines(configfiles[i]); // <-- Case-insensitive
                        if (tempconfig[3].Contains("//default"))
                        {
                            thisconfigisdefault = "//default" + "\n";
                        }
                    }
                    catch (Exception)
                    {

                    }
                    configlist.SelectedIndex = i;
                    ReOrderConfigID(i + 1,thisconfigisdefault);
                }
                Rewrite_ConfigLoader();
                MessageBox.Show("ID has been Re-ordered", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        private void Button_CreateConfiguration(object sender, RoutedEventArgs e)
        {

            if (ValidateConfigName(configname.Text.ToString()) == false)
            {  //validate config name for special characters
                string filename = configname.Text.ToString().Replace(" ", ""); //remove space from name
                configname.Text = filename;
                string configpath = selecteddialfilepath + filename + "_.ino"; //generate config's absolute path

                try
                {
                    int Id = Int32.Parse(GetControlContent(configid.Text));
                    // Create the file, or overwrite if the file exists.
                    using (FileStream fs = File.Create(configpath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("void " + GetControlContent(configname.Text) + "() {" + "\n"
                            + "int index=" + (Id - 1).ToString() + ";" + "\n"
                            + ConstuctCodeLine("appname", GetControlContent(configname.Text), 1)
                            + ConstuctCodeLine("ID", Id.ToString(), 0).Replace("\n","") + Is_defaultConfig()
                            + ConstuctCodeLine("appcolor", GetControlContent_Led(appledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("appanimation", GetControlContent_LedAnimation(appledanimation.SelectedItem), 5)
                            + "\n"
                            + ConstuctCodeLine("knob1_res", knob1res.Value.ToString(), 0)
                            + ConstuctCodeLine("useapp_knob1_CW", GetControlContent(use_APP_knob1_CW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob1_CW =" + inbuiltfunc_knob1_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob1_CW =" + file_knob1_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob1_CW =" + functionscombobox_knob1_CW.Text + "\n"
                            + ConstuctCodeLine("knob1_CW[0]", GetControlContent(knob1CW_1.Text), 3)
                            + ConstuctCodeLine("knob1_CW[1]", GetControlContent(knob1CW_2.Text), 3)
                            + ConstuctCodeLine("knob1_CW[2]", GetControlContent(knob1CW_3.Text), 3)
                            + ConstuctCodeLine("useapp_knob1_CCW", GetControlContent(use_APP_knob1_CCW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob1_CCW =" + inbuiltfunc_knob1_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob1_CCW =" + file_knob1_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob1_CCW =" + functionscombobox_knob1_CCW.Text + "\n"
                            + ConstuctCodeLine("knob1_CCW[0]", GetControlContent(knob1CCW_1.Text), 3)
                            + ConstuctCodeLine("knob1_CCW[1]", GetControlContent(knob1CCW_2.Text), 3)
                            + ConstuctCodeLine("knob1_CCW[2]", GetControlContent(knob1CCW_3.Text), 3)
                            + fileversionModifier("secenc", true)
                            + ConstuctCodeLine("knob2_res", knob2res.Value.ToString(), 0)
                            + ConstuctCodeLine("useapp_knob2_CW", GetControlContent(use_APP_knob2_CW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob2_CW =" + inbuiltfunc_knob2_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob2_CW =" + file_knob2_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob2_CW =" + functionscombobox_knob2_CW.Text + "\n"
                            + ConstuctCodeLine("knob2_CW[0]", GetControlContent(knob2CW_1.Text), 3)
                            + ConstuctCodeLine("knob2_CW[1]", GetControlContent(knob2CW_2.Text), 3)
                            + ConstuctCodeLine("knob2_CW[2]", GetControlContent(knob2CW_3.Text), 3)
                            + ConstuctCodeLine("useapp_knob2_CCW", GetControlContent(use_APP_knob2_CCW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob2_CCW =" + inbuiltfunc_knob2_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob2_CCW =" + file_knob2_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob2_CCW =" + functionscombobox_knob2_CCW.Text + "\n"
                            + ConstuctCodeLine("knob2_CCW[0]", GetControlContent(knob2CCW_1.Text), 3)
                            + ConstuctCodeLine("knob2_CCW[1]", GetControlContent(knob2CCW_2.Text), 3)
                            + ConstuctCodeLine("knob2_CCW[2]", GetControlContent(knob2CCW_3.Text), 3)
                            + fileversionModifier("secenc", false)
                            + ConstuctCodeLine("captouch_dualtapfunc[0]", GetControlContent(singletap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("captouch_dualtapfunc[1]", GetControlContent(doubletap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("useapp_captouch_singletap", GetControlContent(use_APP_captouch_singletap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_singletap =" + inbuiltfunc_captouch_singletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_singletap =" + file_captouch_singletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_singletap =" + functionscombobox_captouch_singletap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("singletap[0]", GetControlContent(singletap_1.Text), 3)
                            + ConstuctCodeLine("singletap[1]", GetControlContent(singletap_2.Text), 3)
                            + ConstuctCodeLine("singletap[2]", GetControlContent(singletap_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_singletap2", GetControlContent(use_APP_captouch_singletap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_singletap2 =" + inbuiltfunc_captouch_singletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_singletap2 =" + file_captouch_singletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_singletap2 =" + functionscombobox_captouch_singletap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("singletap2[0]", GetControlContent(singletap_21.Text), 3)
                            + ConstuctCodeLine("singletap2[1]", GetControlContent(singletap_22.Text), 3)
                            + ConstuctCodeLine("singletap2[2]", GetControlContent(singletap_23.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_doubletap", GetControlContent(use_APP_captouch_doubletap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_doubletap =" + inbuiltfunc_captouch_doubletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_doubletap =" + file_captouch_doubletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_doubletap =" + functionscombobox_captouch_doubletap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("doubletap[0]", GetControlContent(doubletap_1.Text), 3)
                            + ConstuctCodeLine("doubletap[1]", GetControlContent(doubletap_2.Text), 3)
                            + ConstuctCodeLine("doubletap[2]", GetControlContent(doubletap_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_doubletap2", GetControlContent(use_APP_captouch_doubletap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_doubletap2 =" + inbuiltfunc_captouch_doubletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_doubletap2 =" + file_captouch_doubletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_doubletap2 =" + functionscombobox_captouch_doubletap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("doubletap2[0]", GetControlContent(doubletap_21.Text), 3)
                            + ConstuctCodeLine("doubletap2[1]", GetControlContent(doubletap_22.Text), 3)
                            + ConstuctCodeLine("doubletap2[2]", GetControlContent(doubletap_23.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_shortpress", GetControlContent(use_APP_captouch_shortpress.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_shortpress =" + inbuiltfunc_captouch_shortpress.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_shortpress =" + file_captouch_shortpress.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_shortpress =" + functionscombobox_captouch_shortpress.Text + "\n"
                            + ConstuctCodeLine("shortpress[0]", GetControlContent(shortpress_1.Text), 3)
                            + ConstuctCodeLine("shortpress[1]", GetControlContent(shortpress_2.Text), 3)
                            + ConstuctCodeLine("shortpress[2]", GetControlContent(shortpress_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_longpress", GetControlContent(use_APP_captouch_longpress.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_longpress =" + inbuiltfunc_captouch_longpress.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_longpress =" + file_captouch_longpress.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_longpress =" + functionscombobox_captouch_longpress.Text + "\n"
                            + ConstuctCodeLine("longpress[0]", GetControlContent(longpress_1.Text), 3)
                            + ConstuctCodeLine("longpress[1]", GetControlContent(longpress_2.Text), 3)
                            + ConstuctCodeLine("longpress[2]", GetControlContent(longpress_3.Text), 3)
                            + fileversionModifier("mk", true)
                            + ConstuctCodeLine("MK_dualtapfunc[0]", GetControlContent(mk1tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[1]", GetControlContent(mk2tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[2]", GetControlContent(mk3tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[3]", GetControlContent(mk4tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[4]", GetControlContent(mk5tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[0]", GetControlContent(mk1tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[1]", GetControlContent(mk2tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[2]", GetControlContent(mk3tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[3]", GetControlContent(mk4tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[4]", GetControlContent(mk5tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[0]", GetControlContent(mk1hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[1]", GetControlContent(mk2hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[2]", GetControlContent(mk3hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[3]", GetControlContent(mk4hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[4]", GetControlContent(mk5hold_shortcuts.IsEnabled).ToLower(), 0)
                            + "\n"
                            + ConstuctCodeLine("MK_colors[0]", GetControlContent_Led(mk1ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[1]", GetControlContent_Led(mk2ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[2]", GetControlContent_Led(mk3ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[3]", GetControlContent_Led(mk4ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[4]", GetControlContent_Led(mk5ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_animation[0]", GetControlContent_LedAnimation(mk1ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[1]", GetControlContent_LedAnimation(mk2ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[2]", GetControlContent_LedAnimation(mk3ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[3]", GetControlContent_LedAnimation(mk4ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[4]", GetControlContent_LedAnimation(mk5ledanimation.SelectedItem), 5)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK1_tap", GetControlContent(use_APP_MK1tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1tap_SHORTCUT1 =" + inbuiltfunc_MK1tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1tap_SHORTCUT1 =" + file_MK1tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1tap_SHORTCUT1 =" + functionscombobox_MK1tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK1_tap[0]", GetControlContent(mk1tap_1.Text), 3)
                            + ConstuctCodeLine("MK1_tap[1]", GetControlContent(mk1tap_2.Text), 3)
                            + ConstuctCodeLine("MK1_tap[2]", GetControlContent(mk1tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK1_tap2", GetControlContent(use_APP_MK1tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1tap_SHORTCUT2 =" + inbuiltfunc_MK1tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1tap_SHORTCUT2 =" + file_MK1tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1tap_SHORTCUT2 =" + functionscombobox_MK1tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK1_tap2[0]", GetControlContent(mk1tap_21.Text), 3)
                            + ConstuctCodeLine("MK1_tap2[1]", GetControlContent(mk1tap_22.Text), 3)
                            + ConstuctCodeLine("MK1_tap2[2]", GetControlContent(mk1tap_23.Text), 3)
                            + ConstuctCodeLine("MK1_taptext", GetControlContent(mk1tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK1_hold", GetControlContent(use_APP_MK1hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1_hold =" + inbuiltfunc_MK1hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1_hold =" + file_MK1hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1hold =" + functionscombobox_MK1hold.Text + "\n"
                            + ConstuctCodeLine("MK1_hold[0]", GetControlContent(mk1hold_1.Text), 3)
                            + ConstuctCodeLine("MK1_hold[1]", GetControlContent(mk1hold_2.Text), 3)
                            + ConstuctCodeLine("MK1_hold[2]", GetControlContent(mk1hold_3.Text), 3)
                            + ConstuctCodeLine("MK1_holdtext", GetControlContent(mk1hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK2_tap", GetControlContent(use_APP_MK2tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2tap_SHORTCUT1 =" + inbuiltfunc_MK2tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2tap_SHORTCUT1 =" + file_MK2tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2tap_SHORTCUT1 =" + functionscombobox_MK2tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK2_tap[0]", GetControlContent(mk2tap_1.Text), 3)
                            + ConstuctCodeLine("MK2_tap[1]", GetControlContent(mk2tap_2.Text), 3)
                            + ConstuctCodeLine("MK2_tap[2]", GetControlContent(mk2tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK2_tap2", GetControlContent(use_APP_MK2tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2tap_SHORTCUT2 =" + inbuiltfunc_MK2tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2tap_SHORTCUT2 =" + file_MK2tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2tap_SHORTCUT2 =" + functionscombobox_MK2tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK2_tap2[0]", GetControlContent(mk2tap_21.Text), 3)
                            + ConstuctCodeLine("MK2_tap2[1]", GetControlContent(mk2tap_22.Text), 3)
                            + ConstuctCodeLine("MK2_tap2[2]", GetControlContent(mk2tap_23.Text), 3)
                            + ConstuctCodeLine("MK2_taptext", GetControlContent(mk2tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK2_hold", GetControlContent(use_APP_MK2hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2_hold =" + inbuiltfunc_MK2hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2_hold =" + file_MK2hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2hold =" + functionscombobox_MK2hold.Text + "\n"
                            + ConstuctCodeLine("MK2_hold[0]", GetControlContent(mk2hold_1.Text), 3)
                            + ConstuctCodeLine("MK2_hold[1]", GetControlContent(mk2hold_2.Text), 3)
                            + ConstuctCodeLine("MK2_hold[2]", GetControlContent(mk2hold_3.Text), 3)
                            + ConstuctCodeLine("MK2_holdtext", GetControlContent(mk2hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK3_tap", GetControlContent(use_APP_MK3tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3tap_SHORTCUT1 =" + inbuiltfunc_MK3tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3tap_SHORTCUT1 =" + file_MK3tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3tap_SHORTCUT1 =" + functionscombobox_MK3tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK3_tap[0]", GetControlContent(mk3tap_1.Text), 3)
                            + ConstuctCodeLine("MK3_tap[1]", GetControlContent(mk3tap_2.Text), 3)
                            + ConstuctCodeLine("MK3_tap[2]", GetControlContent(mk3tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK3_tap2", GetControlContent(use_APP_MK3tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3tap_SHORTCUT2 =" + inbuiltfunc_MK3tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3tap_SHORTCUT2 =" + file_MK3tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3tap_SHORTCUT2 =" + functionscombobox_MK3tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK3_tap2[0]", GetControlContent(mk3tap_21.Text), 3)
                            + ConstuctCodeLine("MK3_tap2[1]", GetControlContent(mk3tap_22.Text), 3)
                            + ConstuctCodeLine("MK3_tap2[2]", GetControlContent(mk3tap_23.Text), 3)
                            + ConstuctCodeLine("MK3_taptext", GetControlContent(mk3tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK3_hold", GetControlContent(use_APP_MK3hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3_hold =" + inbuiltfunc_MK3hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3_hold =" + file_MK3hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3hold =" + functionscombobox_MK3hold.Text + "\n"
                            + ConstuctCodeLine("MK3_hold[0]", GetControlContent(mk3hold_1.Text), 3)
                            + ConstuctCodeLine("MK3_hold[1]", GetControlContent(mk3hold_2.Text), 3)
                            + ConstuctCodeLine("MK3_hold[2]", GetControlContent(mk3hold_3.Text), 3)
                            + ConstuctCodeLine("MK3_holdtext", GetControlContent(mk3hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK4_tap", GetControlContent(use_APP_MK4tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4tap_SHORTCUT1 =" + inbuiltfunc_MK4tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4tap_SHORTCUT1 =" + file_MK4tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4tap_SHORTCUT1 =" + functionscombobox_MK4tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK4_tap[0]", GetControlContent(mk4tap_1.Text), 3)
                            + ConstuctCodeLine("MK4_tap[1]", GetControlContent(mk4tap_2.Text), 3)
                            + ConstuctCodeLine("MK4_tap[2]", GetControlContent(mk4tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK4_tap2", GetControlContent(use_APP_MK4tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4tap_SHORTCUT2 =" + inbuiltfunc_MK4tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4tap_SHORTCUT2 =" + file_MK4tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4tap_SHORTCUT2 =" + functionscombobox_MK4tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK4_tap2[0]", GetControlContent(mk4tap_21.Text), 3)
                            + ConstuctCodeLine("MK4_tap2[1]", GetControlContent(mk4tap_22.Text), 3)
                            + ConstuctCodeLine("MK4_tap2[2]", GetControlContent(mk4tap_23.Text), 3)
                            + ConstuctCodeLine("MK4_taptext", GetControlContent(mk4tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK4_hold", GetControlContent(use_APP_MK4hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4_hold =" + inbuiltfunc_MK4hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4_hold =" + file_MK4hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4hold =" + functionscombobox_MK4hold.Text + "\n"
                            + ConstuctCodeLine("MK4_hold[0]", GetControlContent(mk4hold_1.Text), 3)
                            + ConstuctCodeLine("MK4_hold[1]", GetControlContent(mk4hold_2.Text), 3)
                            + ConstuctCodeLine("MK4_hold[2]", GetControlContent(mk4hold_3.Text), 3)
                            + ConstuctCodeLine("MK4_holdtext", GetControlContent(mk4hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK5_tap", GetControlContent(use_APP_MK5tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5tap_SHORTCUT1 =" + inbuiltfunc_MK5tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5tap_SHORTCUT1 =" + file_MK5tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5tap_SHORTCUT1 =" + functionscombobox_MK5tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK5_tap[0]", GetControlContent(mk5tap_1.Text), 3)
                            + ConstuctCodeLine("MK5_tap[1]", GetControlContent(mk5tap_2.Text), 3)
                            + ConstuctCodeLine("MK5_tap[2]", GetControlContent(mk5tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK5_tap2", GetControlContent(use_APP_MK5tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5tap_SHORTCUT2 =" + inbuiltfunc_MK5tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5tap_SHORTCUT2 =" + file_MK5tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5tap_SHORTCUT2 =" + functionscombobox_MK5tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK5_tap2[0]", GetControlContent(mk5tap_21.Text), 3)
                            + ConstuctCodeLine("MK5_tap2[1]", GetControlContent(mk5tap_22.Text), 3)
                            + ConstuctCodeLine("MK5_tap2[2]", GetControlContent(mk5tap_23.Text), 3)
                            + ConstuctCodeLine("MK5_taptext", GetControlContent(mk5tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK5_hold", GetControlContent(use_APP_MK5hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5_hold =" + inbuiltfunc_MK5hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5_hold =" + file_MK5hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5hold =" + functionscombobox_MK5hold.Text + "\n"
                            + ConstuctCodeLine("MK5_hold[0]", GetControlContent(mk5hold_1.Text), 3)
                            + ConstuctCodeLine("MK5_hold[1]", GetControlContent(mk5hold_2.Text), 3)
                            + ConstuctCodeLine("MK5_hold[2]", GetControlContent(mk5hold_3.Text), 3)
                            + ConstuctCodeLine("MK5_holdtext", GetControlContent(mk5hold_4.Text), 1)
                            + fileversionModifier("mk", false)
                            + fileversionModifier("spacenav", true)
                            + ConstuctCodeLine("spacenav_func", SpaceNav_Mode().ToString(), 0)
                            + ConstuctCodeLine("spacenav_continuousmode[0]", GetControlContent(continuoustilt.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("spacenav_continuousmode[1]", GetControlContent(continuousslide.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("useapp_spacenav_tiltup", GetControlContent(use_APP_Spacenav_tiltup.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltup =" + inbuiltfunc_Spacenav_tiltup.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltup =" + file_Spacenav_tiltup.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltup =" + functionscombobox_Spacenav_tiltup.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltup[0]", GetControlContent(tiltup_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltup[1]", GetControlContent(tiltup_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltup[2]", GetControlContent(tiltup_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltdown", GetControlContent(use_APP_Spacenav_tiltdown.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltdown =" + inbuiltfunc_Spacenav_tiltdown.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltdown =" + file_Spacenav_tiltdown.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltdown =" + functionscombobox_Spacenav_tiltdown.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltdown[0]", GetControlContent(tiltdown_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltdown[1]", GetControlContent(tiltdown_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltdown[2]", GetControlContent(tiltdown_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltright", GetControlContent(use_APP_Spacenav_tiltright.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltright =" + inbuiltfunc_Spacenav_tiltright.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltright =" + file_Spacenav_tiltright.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltright =" + functionscombobox_Spacenav_tiltright.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltright[0]", GetControlContent(tiltright_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltright[1]", GetControlContent(tiltright_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltright[2]", GetControlContent(tiltright_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltleft", GetControlContent(use_APP_Spacenav_tiltleft.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltleft =" + inbuiltfunc_Spacenav_tiltleft.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltleft =" + file_Spacenav_tiltleft.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltleft =" + functionscombobox_Spacenav_tiltleft.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltleft[0]", GetControlContent(tiltleft_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltleft[1]", GetControlContent(tiltleft_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltleft[2]", GetControlContent(tiltleft_3.Text), 3)
                            + "\n"
                            + ConstuctCodeLine("useapp_spacenav_slideup", GetControlContent(use_APP_Spacenav_slideup.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideup =" + inbuiltfunc_Spacenav_slideup.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideup =" + file_Spacenav_slideup.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideup =" + functionscombobox_Spacenav_slideup.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideup[0]", GetControlContent(slideup_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideup[1]", GetControlContent(slideup_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideup[2]", GetControlContent(slideup_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slidedown", GetControlContent(use_APP_Spacenav_slidedown.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slidedown =" + inbuiltfunc_Spacenav_slidedown.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slidedown =" + file_Spacenav_slidedown.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slidedown =" + functionscombobox_Spacenav_slidedown.Text + "\n"
                            + ConstuctCodeLine("spacenav_slidedown[0]", GetControlContent(slidedown_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slidedown[1]", GetControlContent(slidedown_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slidedown[2]", GetControlContent(slidedown_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slideright", GetControlContent(use_APP_Spacenav_slideright.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideright =" + inbuiltfunc_Spacenav_slideright.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideright =" + file_Spacenav_slideright.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideright =" + functionscombobox_Spacenav_slideright.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideright[0]", GetControlContent(slideright_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideright[1]", GetControlContent(slideright_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideright[2]", GetControlContent(slideright_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slideleft", GetControlContent(use_APP_Spacenav_slideleft.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideleft =" + inbuiltfunc_Spacenav_slideleft.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideleft =" + file_Spacenav_slideleft.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideleft =" + functionscombobox_Spacenav_slideleft.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideleft[0]", GetControlContent(slideleft_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideleft[1]", GetControlContent(slideleft_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideleft[2]", GetControlContent(slideleft_3.Text), 3)
                            + fileversionModifier("spacenav", false)
                            + "}" + "\n"
                            );
                        // Add config settings to the configfile.
                        fs.Write(info, 0, info.Length);
                        MessageBox.Show("Configuration Created", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Rewrite_ConfigLoader();
            }
            else
            {
                MessageBox.Show("Enter A Valid Configname \n (No Special Characters Allowed)", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            PopulateConfigList(); //refresh existing configlist
        }

        private void Update_MaxconfigValue(int numberofconfigs)
        {
            try
            {
                string[] maincodefile = Directory.GetFiles(selecteddialfilepath, "*main.ino"); // <-- Case-insensitive
                string[] maincode = System.IO.File.ReadAllLines(maincodefile[0]);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(maincodefile[0]))
                {
                    for (int i = 0; i < maincode.Length; i++)
                    {
                        if (maincode[i].Contains("#define maxconfignum"))
                        {
                            maincode[i] = "";
                            if (numberofconfigs <= 10) //sets the max number of configs usable
                            {  
                                maincode[i] = "#define maxconfignum " + numberofconfigs.ToString();
                            }
                            else
                            {
                                maincode[i] = "#define maxconfignum 10" ;
                            }
                            
                        }
                        file.WriteLine(maincode[i]);
                    }
                }
            }
            catch (Exception)
            {

            }
           

        }
        private void Rewrite_ConfigLoader()
        {
            int cindex = 0;
            string cname = "";
            // Put all config files in root directory into array.
            string[] configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
            Update_MaxconfigValue(configfiles.Length); //update MaxconfigValue
            string[] configorder = { "", "" };
            Array.Resize<string>(ref configorder, configfiles.Length);

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
                        sub = sub.Replace("//default", "");
                    }
                    cindex = Int32.Parse(sub);

                }
                if (cindex <= configorder.Length)
                {
                    configorder[cindex - 1] = cname;
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
            string carray = "";
            for (int i = 0; i < configorder.Length; i++)
            {
                if (i == (configorder.Length - 1))
                {
                    carray = carray + configorder[i] + "\n";
                }
                else
                {
                    carray = carray + configorder[i] + "," + "\n";
                }
            }
            try
            {

                using (FileStream fs = File.Create(selecteddialfilepath + @"load_configurations.ino"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("void Load_Configurations() {" + "\n"
                        + "void (*config_arr[])() = {" + "\n"
                        + carray
                        + "};" + "\n"
                        + "for (int i = 0; i < maxconfignum; i++) {" + "\n"
                        + "(*config_arr[i])();" + "\n"
                        + " }" + "\n"
                        + " }");

                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void Disable_SpaceNavConfigMode(object sender, RoutedEventArgs e)
        {
            //disable Manual Space nav settings if predefined settings are selected
            if (spacenav_manual.IsChecked == false)
            {
                spacenav_manualconfig.IsEnabled = false;
                spacenav_manualconfig.Opacity = faded;
            }
            else
            {
                spacenav_manualconfig.IsEnabled = true;
                spacenav_manualconfig.Opacity = opaque;
            }
        }

        private void MacroKey_ConfigMode(object sender, TextChangedEventArgs e)
        {
            //set Macro keys text printing function based on input
            if (mk1tap_4.Text == " " || mk1tap_4.Text == "0" || mk1tap_4.Text == "")
            {
                mk1tap_shortcuts.IsEnabled = true;
                mk1tap_shortcuts.Opacity = opaque;
            }
            else
            {
                mk1tap_shortcuts.IsEnabled = false;
                mk1tap_shortcuts.Opacity = faded;
            }
            if (mk1hold_4.Text == " " || mk1hold_4.Text == "0" || mk1hold_4.Text == "")
            {
                mk1hold_shortcuts.IsEnabled = true;
                mk1hold_shortcuts.Opacity = opaque;
            }
            else
            {
                mk1hold_shortcuts.IsEnabled = false;
                mk1hold_shortcuts.Opacity = faded;
            }
            /*******************************************************************************************************/
            if (mk2tap_4.Text == " " || mk2tap_4.Text == "0" || mk2tap_4.Text == "")
            {
                mk2tap_shortcuts.IsEnabled = true;
                mk2tap_shortcuts.Opacity = opaque;
            }
            else
            {
                mk2tap_shortcuts.IsEnabled = false;
                mk2tap_shortcuts.Opacity = faded;
            }
            if (mk2hold_4.Text == " " || mk2hold_4.Text == "0" || mk2hold_4.Text == "")
            {
                mk2hold_shortcuts.IsEnabled = true;
                mk2hold_shortcuts.Opacity = opaque;
            }
            else
            {
                mk2hold_shortcuts.IsEnabled = false;
                mk2hold_shortcuts.Opacity = faded;
            }
            /*******************************************************************************************************/
            if (mk3tap_4.Text == " " || mk3tap_4.Text == "0" || mk3tap_4.Text == "")
            {
                mk3tap_shortcuts.IsEnabled = true;
                mk3tap_shortcuts.Opacity = opaque;
            }
            else
            {
                mk3tap_shortcuts.IsEnabled = false;
                mk3tap_shortcuts.Opacity = faded;
            }
            if (mk3hold_4.Text == " " || mk3hold_4.Text == "0" || mk3hold_4.Text == "")
            {
                mk3hold_shortcuts.IsEnabled = true;
                mk3hold_shortcuts.Opacity = opaque;
            }
            else
            {
                mk3hold_shortcuts.IsEnabled = false;
                mk3hold_shortcuts.Opacity = faded;
            }
            /*******************************************************************************************************/
            if (mk4tap_4.Text == " " || mk4tap_4.Text == "0" || mk4tap_4.Text == "")
            {
                mk4tap_shortcuts.IsEnabled = true;
                mk4tap_shortcuts.Opacity = opaque;
            }
            else
            {
                mk4tap_shortcuts.IsEnabled = false;
                mk4tap_shortcuts.Opacity = faded;
            }
            if (mk4hold_4.Text == " " || mk4hold_4.Text == "0" || mk4hold_4.Text == "")
            {
                mk4hold_shortcuts.IsEnabled = true;
                mk4hold_shortcuts.Opacity = opaque;
            }
            else
            {
                mk4hold_shortcuts.IsEnabled = false;
                mk4hold_shortcuts.Opacity = faded;
            }
            /*******************************************************************************************************/
            if (mk5tap_4.Text == " " || mk5tap_4.Text == "0" || mk5tap_4.Text == "")
            {
                mk5tap_shortcuts.IsEnabled = true;
                mk5tap_shortcuts.Opacity = opaque;
            }
            else
            {
                mk5tap_shortcuts.IsEnabled = false;
                mk5tap_shortcuts.Opacity = faded;
            }
            if (mk5hold_4.Text == " " || mk5hold_4.Text == "0" || mk5hold_4.Text == "")
            {
                mk5hold_shortcuts.IsEnabled = true;
                mk5hold_shortcuts.Opacity = opaque;
            }
            else
            {
                mk5hold_shortcuts.IsEnabled = false;
                mk5hold_shortcuts.Opacity = faded;
            }

        }
        private string GetControlContent(object control)
        {
            //return a specified character if settings are empty
            string newname;
            if (control == null || control.ToString() == "" || control.ToString() == ("null") || control.ToString() == " ")
            {
                newname = nullchar;
            }
            else
            {
                newname = control.ToString();
            }
            return newname;
        }
        private string GetControlContent_Led(object control)
        {
            //return a specified character if Led settings are empty
            string newname;
            if (control == null || control.ToString() == "" || control.ToString() == ("null"))
            {
                newname = "Snow";
            }
            else
            {
                newname = control.ToString();
            }
            return newname;
        }

        private string GetControlContent_LedAnimation(object control)
        {
            //return a specified character if Led settings are empty
            string newname;
            if (control == null || control.ToString() == "" || control.ToString() == ("null"))
            {
                newname = "solid";
            }
            else
            {
                newname = control.ToString();
            }
            return newname;
        }

        private bool ValidateConfigName(string value)
        {
            //checks for special characters in configname
            return !(Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$"));
        }
        private void ValidateCharacter(object sender, TextCompositionEventArgs e)
        {

            string textkey = e.OriginalSource.ToString().Replace("System.Windows.Controls.TextBox: ", "");

            if (textkey.Length > 0 && textkey != "System.Windows.Controls.TextBox")
            {
                MessageBox.Show("Enter A Single Character", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;

            }
        }

        private string ConstuctCodeLine(string control, string content, int mode)
        {
            //code line constructing function
            string codeline = "";
            switch (mode)
            {
                case 0:
                    codeline = "appconfig[index]." + control + "=" + content + ";" + "\n";
                    break;
                case 1:
                    string quotation = "\"";
                    if (content != "' '")
                    {
                        codeline = "appconfig[index]." + control + "=" + quotation + content + quotation + ";" + "\n";
                        codeline = codeline.Replace(Environment.NewLine, "#");
                    }
                    else {
                        codeline = "appconfig[index]." + control + "=" + quotation + quotation + ";" + "\n";
                    }
                    break;
                case 2:
                    if (content != "' '")
                    {
                        codeline = "appconfig[index]." + control + "=" + "'" + content + "'" + ";" + "\n";
                    }
                    else
                    {
                        codeline = "appconfig[index]." + control + "=" + content + ";" + "\n";
                    }
                    break;
                case 3:
                    if (content.Length <= 1)
                    {
                        codeline = "appconfig[index]." + control + "=" + "'" + content + "'" + ";" + "\n";
                    }
                    else
                    {
                        codeline = "appconfig[index]." + control + "=" + content + ";" + "\n";
                    }
                    break;
                case 4:
                        codeline = "appconfig[index]." + control + "=CRGB::" + content + ";" + "\n";
                    break;
                case 5:
                        codeline = "appconfig[index]." + control + "=" + content + ";" + "\n";
                    break;
            }
            return codeline;

        }

        private string Is_defaultConfig()
        {
                if (DefaultConfig.IsChecked == true)
                {
                    return "//default" + "\n";
                }
                else
                {
                    return "\n";
                }      
            
        }

        private void Set_ConfigID(object sender, TextChangedEventArgs e)
        {
            int cindex = 0;
            // Put all config files in root directory into array.
            string[] configfiles = { };  
            try
            {
                configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
            }
            catch (Exception)
            {

            }
            
            if (configfiles.Length > 0)
            {
                int[] idarray = { };
                Array.Resize<int>(ref idarray, configfiles.Length);

                int cnt = 0;
                foreach (string s in configfiles)
                {
                    string[] configsettings = System.IO.File.ReadAllLines(configfiles[cnt]);

                    string configline = configsettings[3];
                    int substringindex = configline.LastIndexOf("=");
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
                            sub = sub.Replace("//default", "");
                        }

                        cindex = Int32.Parse(sub);

                    }
                    idarray[cnt] = cindex;
                    cnt += 1;
                }
                //find the last assigned config ID
                int maxnum = idarray[0];
                for (int i = 0; i < idarray.Length; i++)
                {
                    if (idarray[i] > maxnum)
                    {
                        maxnum = idarray[i];
                    }
                }


                //set config id based on config name
                int selectedconfigindex = configlist.SelectedIndex;
                if (configlist.SelectedItem != null)
                {
                    if (((ComboBoxItem)this.configlist.SelectedItem).Name.ToString() == configname.Text.Replace(" ", ""))
                    {
                        configid.Text = maxnum.ToString();
                    }
                    else
                    {
                        selectedconfigindex = maxnum + 1;
                        configid.Text = selectedconfigindex.ToString();
                    }
                }
                else
                {
                    selectedconfigindex = maxnum + 1;
                    configid.Text = selectedconfigindex.ToString();
                }
            }
            else
            {
                configid.Text = "1";
            }
        }

        private void ConfigID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //disables alphabets in config id textbox
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
        private void Update_ConfigID(object sender, TextChangedEventArgs e)
        {
            //validates manual config id 
            if (configid.Text != "")
            {
                if (configid.Text.Contains(" "))
                {
                    configid.Text = configid.Text.Replace(" ", "");
                    if (configid.Text.Any(x => char.IsLetter(x)))
                    {
                        //MessageBox.Show("onlyhjjjjjj");
                        configid.Text = (configcount + 1).ToString();
                    }
                }
                else
                {
                    if (configid.Text.Any(x => char.IsLetter(x)))
                    {
                        //MessageBox.Show("onlyhjjjjjj");
                        configid.Text = (configcount + 1).ToString();
                    }
                }
            }
            else
            {
                configid.Text = (configcount + 1).ToString();
            }
        }

        private void ResetConfig(object sender, RoutedEventArgs e)
        {
            resetbylevel(settings);
            spacenav_manual.IsChecked = true;
        }

        private void resetbylevel(DependencyObject startpoint)  //recursive function for reseting all control from startpoint
        {
            foreach (Object child1 in LogicalTreeHelper.GetChildren(startpoint))
            {
                if (child1 is StackPanel || child1 is ScrollViewer || child1 is Grid) //if child is a container
                {
                    DependencyObject tempchild = (DependencyObject)child1;
                    resetbylevel(tempchild);  //recursion call

                }
                else if (child1 is ComboBox)
                {
                    ((ComboBox)child1).SelectedItem = null;
                    ((ComboBox)child1).Text = "";
                }
                else if (child1 is TextBox)
                {
                    ((TextBox)child1).Text = "";
                }
                else if (child1 is CheckBox)
                {
                    ((CheckBox)child1).IsChecked = false;
                }
                else if (child1 is RadioButton)
                {
                    ((RadioButton)child1).IsChecked = false;
                }

            }
        }

        private void DeleteConfig(object sender, RoutedEventArgs e)
        {
            if (configlist.SelectedValue != null)
            {
                string[] configfiles = { };
                File.Delete(((ComboBoxItem)this.configlist.SelectedItem).Tag.ToString());
                MessageBox.Show(configname.Text + " Configuration Deleted", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Information);

                configlist.Items.Remove(((ComboBoxItem)this.configlist.SelectedItem));
                PopulateConfigList();
                try
                {
                    configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
                }
                catch (Exception)
                {

                }
                for (int i = 0; i < configlist.Items.Count; i++)
                {
                    string thisconfigisdefault = "\n";
                    try
                    {
                        string[] tempconfig = System.IO.File.ReadAllLines(configfiles[i]); // <-- Case-insensitive
                        if (tempconfig[3].Contains("//default"))
                        {
                            thisconfigisdefault = "//default" + "\n";
                        }
                    }
                    catch (Exception)
                    {

                    }
                    configlist.SelectedIndex = i;
                    ReOrderConfigID(i + 1, thisconfigisdefault);
                }
                configlist.SelectedIndex = 0;
                Rewrite_ConfigLoader();

            }
            else
            {
                MessageBox.Show("This Configuration File Does Not Exist", "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ReOrderConfigID(int newid, string defaulttxt)
        {
            if (ValidateConfigName(configname.Text.ToString()) == false)
            {  //validate config name for special characters
                string filename = configname.Text.ToString().Replace(" ", ""); //remove space from name
                configname.Text = filename;
                string configpath = selecteddialfilepath + filename + "_.ino"; //generate config's absolute path

                try
                {
                    // Create the file, or overwrite if the file exists.
                    using (FileStream fs = File.Create(configpath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("void " + GetControlContent(configname.Text) + "() {" + "\n"
                            + "int index=" + (newid - 1).ToString() + ";" + "\n"
                            + ConstuctCodeLine("appname", GetControlContent(configname.Text), 1)
                            + ConstuctCodeLine("ID", newid.ToString(), 0).Replace("\n","") + defaulttxt
                            + ConstuctCodeLine("appcolor", GetControlContent_Led(appledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("appanimation", GetControlContent_LedAnimation(appledanimation.SelectedItem), 5)
                            + "\n"
                            + ConstuctCodeLine("knob1_res", knob1res.Value.ToString(), 0)
                            + ConstuctCodeLine("useapp_knob1_CW", GetControlContent(use_APP_knob1_CW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob1_CW =" + inbuiltfunc_knob1_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob1_CW =" + file_knob1_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob1_CW =" + functionscombobox_knob1_CW.Text + "\n"
                            + ConstuctCodeLine("knob1_CW[0]", GetControlContent(knob1CW_1.Text), 3)
                            + ConstuctCodeLine("knob1_CW[1]", GetControlContent(knob1CW_2.Text), 3)
                            + ConstuctCodeLine("knob1_CW[2]", GetControlContent(knob1CW_3.Text), 3)
                            + ConstuctCodeLine("useapp_knob1_CCW", GetControlContent(use_APP_knob1_CCW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob1_CCW =" + inbuiltfunc_knob1_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob1_CCW =" + file_knob1_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob1_CCW =" + functionscombobox_knob1_CCW.Text + "\n"
                            + ConstuctCodeLine("knob1_CCW[0]", GetControlContent(knob1CCW_1.Text), 3)
                            + ConstuctCodeLine("knob1_CCW[1]", GetControlContent(knob1CCW_2.Text), 3)
                            + ConstuctCodeLine("knob1_CCW[2]", GetControlContent(knob1CCW_3.Text), 3)
                            + fileversionModifier("secenc", true)
                            + ConstuctCodeLine("knob2_res", knob2res.Value.ToString(), 0)
                            + ConstuctCodeLine("useapp_knob2_CW", GetControlContent(use_APP_knob2_CW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob2_CW =" + inbuiltfunc_knob2_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob2_CW =" + file_knob2_CW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob2_CW =" + functionscombobox_knob2_CW.Text + "\n"
                            + ConstuctCodeLine("knob2_CW[0]", GetControlContent(knob2CW_1.Text), 3)
                            + ConstuctCodeLine("knob2_CW[1]", GetControlContent(knob2CW_2.Text), 3)
                            + ConstuctCodeLine("knob2_CW[2]", GetControlContent(knob2CW_3.Text), 3)
                            + ConstuctCodeLine("useapp_knob2_CCW", GetControlContent(use_APP_knob2_CCW.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_knob2_CCW =" + inbuiltfunc_knob2_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// file_knob2_CCW =" + file_knob2_CCW.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_knob2_CCW =" + functionscombobox_knob2_CCW.Text + "\n"
                            + ConstuctCodeLine("knob2_CCW[0]", GetControlContent(knob2CCW_1.Text), 3)
                            + ConstuctCodeLine("knob2_CCW[1]", GetControlContent(knob2CCW_2.Text), 3)
                            + ConstuctCodeLine("knob2_CCW[2]", GetControlContent(knob2CCW_3.Text), 3)
                            + fileversionModifier("secenc", false)
                            + ConstuctCodeLine("captouch_dualtapfunc[0]", GetControlContent(singletap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("captouch_dualtapfunc[1]", GetControlContent(doubletap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("useapp_captouch_singletap", GetControlContent(use_APP_captouch_singletap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_singletap =" + inbuiltfunc_captouch_singletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_singletap =" + file_captouch_singletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_singletap =" + functionscombobox_captouch_singletap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("singletap[0]", GetControlContent(singletap_1.Text), 3)
                            + ConstuctCodeLine("singletap[1]", GetControlContent(singletap_2.Text), 3)
                            + ConstuctCodeLine("singletap[2]", GetControlContent(singletap_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_singletap2", GetControlContent(use_APP_captouch_singletap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_singletap2 =" + inbuiltfunc_captouch_singletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_singletap2 =" + file_captouch_singletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_singletap2 =" + functionscombobox_captouch_singletap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("singletap2[0]", GetControlContent(singletap_21.Text), 3)
                            + ConstuctCodeLine("singletap2[1]", GetControlContent(singletap_22.Text), 3)
                            + ConstuctCodeLine("singletap2[2]", GetControlContent(singletap_23.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_doubletap", GetControlContent(use_APP_captouch_doubletap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_doubletap =" + inbuiltfunc_captouch_doubletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_doubletap =" + file_captouch_doubletap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_doubletap =" + functionscombobox_captouch_doubletap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("doubletap[0]", GetControlContent(doubletap_1.Text), 3)
                            + ConstuctCodeLine("doubletap[1]", GetControlContent(doubletap_2.Text), 3)
                            + ConstuctCodeLine("doubletap[2]", GetControlContent(doubletap_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_doubletap2", GetControlContent(use_APP_captouch_doubletap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_doubletap2 =" + inbuiltfunc_captouch_doubletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_doubletap2 =" + file_captouch_doubletap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_doubletap2 =" + functionscombobox_captouch_doubletap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("doubletap2[0]", GetControlContent(doubletap_21.Text), 3)
                            + ConstuctCodeLine("doubletap2[1]", GetControlContent(doubletap_22.Text), 3)
                            + ConstuctCodeLine("doubletap2[2]", GetControlContent(doubletap_23.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_shortpress", GetControlContent(use_APP_captouch_shortpress.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_shortpress =" + inbuiltfunc_captouch_shortpress.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_shortpress =" + file_captouch_shortpress.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_shortpress =" + functionscombobox_captouch_shortpress.Text + "\n"
                            + ConstuctCodeLine("shortpress[0]", GetControlContent(shortpress_1.Text), 3)
                            + ConstuctCodeLine("shortpress[1]", GetControlContent(shortpress_2.Text), 3)
                            + ConstuctCodeLine("shortpress[2]", GetControlContent(shortpress_3.Text), 3)
                            + ConstuctCodeLine("useapp_captouch_longpress", GetControlContent(use_APP_captouch_longpress.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_captouch_longpress =" + inbuiltfunc_captouch_longpress.IsChecked.ToString().ToLower() + "\n"
                            + "// file_captouch_longpress =" + file_captouch_longpress.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_captouch_longpress =" + functionscombobox_captouch_longpress.Text + "\n"
                            + ConstuctCodeLine("longpress[0]", GetControlContent(longpress_1.Text), 3)
                            + ConstuctCodeLine("longpress[1]", GetControlContent(longpress_2.Text), 3)
                            + ConstuctCodeLine("longpress[2]", GetControlContent(longpress_3.Text), 3)
                            + fileversionModifier("mk", true)
                            + ConstuctCodeLine("MK_dualtapfunc[0]", GetControlContent(mk1tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[1]", GetControlContent(mk2tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[2]", GetControlContent(mk3tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[3]", GetControlContent(mk4tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_dualtapfunc[4]", GetControlContent(mk5tap_dualshortcut.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[0]", GetControlContent(mk1tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[1]", GetControlContent(mk2tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[2]", GetControlContent(mk3tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[3]", GetControlContent(mk4tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_tapfunc[4]", GetControlContent(mk5tap_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[0]", GetControlContent(mk1hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[1]", GetControlContent(mk2hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[2]", GetControlContent(mk3hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[3]", GetControlContent(mk4hold_shortcuts.IsEnabled).ToLower(), 0)
                            + ConstuctCodeLine("MK_holdfunc[4]", GetControlContent(mk5hold_shortcuts.IsEnabled).ToLower(), 0)
                            + "\n"
                            + ConstuctCodeLine("MK_colors[0]", GetControlContent_Led(mk1ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[1]", GetControlContent_Led(mk2ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[2]", GetControlContent_Led(mk3ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[3]", GetControlContent_Led(mk4ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_colors[4]", GetControlContent_Led(mk5ledcolor.SelectedItem), 4)
                            + ConstuctCodeLine("MK_animation[0]", GetControlContent_LedAnimation(mk1ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[1]", GetControlContent_LedAnimation(mk2ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[2]", GetControlContent_LedAnimation(mk3ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[3]", GetControlContent_LedAnimation(mk4ledanimation.SelectedItem), 5)
                            + ConstuctCodeLine("MK_animation[4]", GetControlContent_LedAnimation(mk5ledanimation.SelectedItem), 5)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK1_tap", GetControlContent(use_APP_MK1tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1tap_SHORTCUT1 =" + inbuiltfunc_MK1tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1tap_SHORTCUT1 =" + file_MK1tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1tap_SHORTCUT1 =" + functionscombobox_MK1tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK1_tap[0]", GetControlContent(mk1tap_1.Text), 3)
                            + ConstuctCodeLine("MK1_tap[1]", GetControlContent(mk1tap_2.Text), 3)
                            + ConstuctCodeLine("MK1_tap[2]", GetControlContent(mk1tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK1_tap2", GetControlContent(use_APP_MK1tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1tap_SHORTCUT2 =" + inbuiltfunc_MK1tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1tap_SHORTCUT2 =" + file_MK1tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1tap_SHORTCUT2 =" + functionscombobox_MK1tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK1_tap2[0]", GetControlContent(mk1tap_21.Text), 3)
                            + ConstuctCodeLine("MK1_tap2[1]", GetControlContent(mk1tap_22.Text), 3)
                            + ConstuctCodeLine("MK1_tap2[2]", GetControlContent(mk1tap_23.Text), 3)
                            + ConstuctCodeLine("MK1_taptext", GetControlContent(mk1tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK1_hold", GetControlContent(use_APP_MK1hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK1_hold =" + inbuiltfunc_MK1hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK1_hold =" + file_MK1hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK1hold =" + functionscombobox_MK1hold.Text + "\n"
                            + ConstuctCodeLine("MK1_hold[0]", GetControlContent(mk1hold_1.Text), 3)
                            + ConstuctCodeLine("MK1_hold[1]", GetControlContent(mk1hold_2.Text), 3)
                            + ConstuctCodeLine("MK1_hold[2]", GetControlContent(mk1hold_3.Text), 3)
                            + ConstuctCodeLine("MK1_holdtext", GetControlContent(mk1hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK2_tap", GetControlContent(use_APP_MK2tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2tap_SHORTCUT1 =" + inbuiltfunc_MK2tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2tap_SHORTCUT1 =" + file_MK2tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2tap_SHORTCUT1 =" + functionscombobox_MK2tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK2_tap[0]", GetControlContent(mk2tap_1.Text), 3)
                            + ConstuctCodeLine("MK2_tap[1]", GetControlContent(mk2tap_2.Text), 3)
                            + ConstuctCodeLine("MK2_tap[2]", GetControlContent(mk2tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK2_tap2", GetControlContent(use_APP_MK2tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2tap_SHORTCUT2 =" + inbuiltfunc_MK2tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2tap_SHORTCUT2 =" + file_MK2tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2tap_SHORTCUT2 =" + functionscombobox_MK2tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK2_tap2[0]", GetControlContent(mk2tap_21.Text), 3)
                            + ConstuctCodeLine("MK2_tap2[1]", GetControlContent(mk2tap_22.Text), 3)
                            + ConstuctCodeLine("MK2_tap2[2]", GetControlContent(mk2tap_23.Text), 3)
                            + ConstuctCodeLine("MK2_taptext", GetControlContent(mk2tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK2_hold", GetControlContent(use_APP_MK2hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK2_hold =" + inbuiltfunc_MK2hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK2_hold =" + file_MK2hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK2hold =" + functionscombobox_MK2hold.Text + "\n"
                            + ConstuctCodeLine("MK2_hold[0]", GetControlContent(mk2hold_1.Text), 3)
                            + ConstuctCodeLine("MK2_hold[1]", GetControlContent(mk2hold_2.Text), 3)
                            + ConstuctCodeLine("MK2_hold[2]", GetControlContent(mk2hold_3.Text), 3)
                            + ConstuctCodeLine("MK2_holdtext", GetControlContent(mk2hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK3_tap", GetControlContent(use_APP_MK3tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3tap_SHORTCUT1 =" + inbuiltfunc_MK3tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3tap_SHORTCUT1 =" + file_MK3tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3tap_SHORTCUT1 =" + functionscombobox_MK3tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK3_tap[0]", GetControlContent(mk3tap_1.Text), 3)
                            + ConstuctCodeLine("MK3_tap[1]", GetControlContent(mk3tap_2.Text), 3)
                            + ConstuctCodeLine("MK3_tap[2]", GetControlContent(mk3tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK3_tap2", GetControlContent(use_APP_MK3tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3tap_SHORTCUT2 =" + inbuiltfunc_MK3tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3tap_SHORTCUT2 =" + file_MK3tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3tap_SHORTCUT2 =" + functionscombobox_MK3tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK3_tap2[0]", GetControlContent(mk3tap_21.Text), 3)
                            + ConstuctCodeLine("MK3_tap2[1]", GetControlContent(mk3tap_22.Text), 3)
                            + ConstuctCodeLine("MK3_tap2[2]", GetControlContent(mk3tap_23.Text), 3)
                            + ConstuctCodeLine("MK3_taptext", GetControlContent(mk3tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK3_hold", GetControlContent(use_APP_MK3hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK3_hold =" + inbuiltfunc_MK3hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK3_hold =" + file_MK3hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK3hold =" + functionscombobox_MK3hold.Text + "\n"
                            + ConstuctCodeLine("MK3_hold[0]", GetControlContent(mk3hold_1.Text), 3)
                            + ConstuctCodeLine("MK3_hold[1]", GetControlContent(mk3hold_2.Text), 3)
                            + ConstuctCodeLine("MK3_hold[2]", GetControlContent(mk3hold_3.Text), 3)
                            + ConstuctCodeLine("MK3_holdtext", GetControlContent(mk3hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK4_tap", GetControlContent(use_APP_MK4tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4tap_SHORTCUT1 =" + inbuiltfunc_MK4tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4tap_SHORTCUT1 =" + file_MK4tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4tap_SHORTCUT1 =" + functionscombobox_MK4tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK4_tap[0]", GetControlContent(mk4tap_1.Text), 3)
                            + ConstuctCodeLine("MK4_tap[1]", GetControlContent(mk4tap_2.Text), 3)
                            + ConstuctCodeLine("MK4_tap[2]", GetControlContent(mk4tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK4_tap2", GetControlContent(use_APP_MK4tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4tap_SHORTCUT2 =" + inbuiltfunc_MK4tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4tap_SHORTCUT2 =" + file_MK4tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4tap_SHORTCUT2 =" + functionscombobox_MK4tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK4_tap2[0]", GetControlContent(mk4tap_21.Text), 3)
                            + ConstuctCodeLine("MK4_tap2[1]", GetControlContent(mk4tap_22.Text), 3)
                            + ConstuctCodeLine("MK4_tap2[2]", GetControlContent(mk4tap_23.Text), 3)
                            + ConstuctCodeLine("MK4_taptext", GetControlContent(mk4tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK4_hold", GetControlContent(use_APP_MK4hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK4_hold =" + inbuiltfunc_MK4hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK4_hold =" + file_MK4hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK4hold =" + functionscombobox_MK4hold.Text + "\n"
                            + ConstuctCodeLine("MK4_hold[0]", GetControlContent(mk4hold_1.Text), 3)
                            + ConstuctCodeLine("MK4_hold[1]", GetControlContent(mk4hold_2.Text), 3)
                            + ConstuctCodeLine("MK4_hold[2]", GetControlContent(mk4hold_3.Text), 3)
                            + ConstuctCodeLine("MK4_holdtext", GetControlContent(mk4hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("useapp_MK5_tap", GetControlContent(use_APP_MK5tap_SHORTCUT1.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5tap_SHORTCUT1 =" + inbuiltfunc_MK5tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5tap_SHORTCUT1 =" + file_MK5tap_SHORTCUT1.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5tap_SHORTCUT1 =" + functionscombobox_MK5tap_SHORTCUT1.Text + "\n"
                            + ConstuctCodeLine("MK5_tap[0]", GetControlContent(mk5tap_1.Text), 3)
                            + ConstuctCodeLine("MK5_tap[1]", GetControlContent(mk5tap_2.Text), 3)
                            + ConstuctCodeLine("MK5_tap[2]", GetControlContent(mk5tap_3.Text), 3)
                            + ConstuctCodeLine("useapp_MK5_tap2", GetControlContent(use_APP_MK5tap_SHORTCUT2.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5tap_SHORTCUT2 =" + inbuiltfunc_MK5tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5tap_SHORTCUT2 =" + file_MK5tap_SHORTCUT2.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5tap_SHORTCUT2 =" + functionscombobox_MK5tap_SHORTCUT2.Text + "\n"
                            + ConstuctCodeLine("MK5_tap2[0]", GetControlContent(mk5tap_21.Text), 3)
                            + ConstuctCodeLine("MK5_tap2[1]", GetControlContent(mk5tap_22.Text), 3)
                            + ConstuctCodeLine("MK5_tap2[2]", GetControlContent(mk5tap_23.Text), 3)
                            + ConstuctCodeLine("MK5_taptext", GetControlContent(mk5tap_4.Text), 1)
                            + ConstuctCodeLine("useapp_MK5_hold", GetControlContent(use_APP_MK5hold.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_MK5_hold =" + inbuiltfunc_MK5hold.IsChecked.ToString().ToLower() + "\n"
                            + "// file_MK5_hold =" + file_MK5hold.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_MK5hold =" + functionscombobox_MK5hold.Text + "\n"
                            + ConstuctCodeLine("MK5_hold[0]", GetControlContent(mk5hold_1.Text), 3)
                            + ConstuctCodeLine("MK5_hold[1]", GetControlContent(mk5hold_2.Text), 3)
                            + ConstuctCodeLine("MK5_hold[2]", GetControlContent(mk5hold_3.Text), 3)
                            + ConstuctCodeLine("MK5_holdtext", GetControlContent(mk5hold_4.Text), 1)
                            + fileversionModifier("mk", false)
                            + fileversionModifier("spacenav", true)
                            + ConstuctCodeLine("spacenav_func", SpaceNav_Mode().ToString(), 0)
                            + ConstuctCodeLine("spacenav_continuousmode[0]", GetControlContent(continuoustilt.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("spacenav_continuousmode[1]", GetControlContent(continuousslide.IsChecked).ToLower(), 0)
                            + ConstuctCodeLine("useapp_spacenav_tiltup", GetControlContent(use_APP_Spacenav_tiltup.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltup =" + inbuiltfunc_Spacenav_tiltup.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltup =" + file_Spacenav_tiltup.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltup =" + functionscombobox_Spacenav_tiltup.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltup[0]", GetControlContent(tiltup_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltup[1]", GetControlContent(tiltup_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltup[2]", GetControlContent(tiltup_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltdown", GetControlContent(use_APP_Spacenav_tiltdown.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltdown =" + inbuiltfunc_Spacenav_tiltdown.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltdown =" + file_Spacenav_tiltdown.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltdown =" + functionscombobox_Spacenav_tiltdown.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltdown[0]", GetControlContent(tiltdown_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltdown[1]", GetControlContent(tiltdown_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltdown[2]", GetControlContent(tiltdown_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltright", GetControlContent(use_APP_Spacenav_tiltright.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltright =" + inbuiltfunc_Spacenav_tiltright.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltright =" + file_Spacenav_tiltright.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltright =" + functionscombobox_Spacenav_tiltright.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltright[0]", GetControlContent(tiltright_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltright[1]", GetControlContent(tiltright_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltright[2]", GetControlContent(tiltright_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_tiltleft", GetControlContent(use_APP_Spacenav_tiltleft.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_tiltleft =" + inbuiltfunc_Spacenav_tiltleft.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_tiltleft =" + file_Spacenav_tiltleft.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_tiltleft =" + functionscombobox_Spacenav_tiltleft.Text + "\n"
                            + ConstuctCodeLine("spacenav_tiltleft[0]", GetControlContent(tiltleft_1.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltleft[1]", GetControlContent(tiltleft_2.Text), 3)
                            + ConstuctCodeLine("spacenav_tiltleft[2]", GetControlContent(tiltleft_3.Text), 3)
                            + "\n"
                            + ConstuctCodeLine("useapp_spacenav_slideup", GetControlContent(use_APP_Spacenav_slideup.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideup =" + inbuiltfunc_Spacenav_slideup.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideup =" + file_Spacenav_slideup.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideup =" + functionscombobox_Spacenav_slideup.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideup[0]", GetControlContent(slideup_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideup[1]", GetControlContent(slideup_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideup[2]", GetControlContent(slideup_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slidedown", GetControlContent(use_APP_Spacenav_slidedown.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slidedown =" + inbuiltfunc_Spacenav_slidedown.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slidedown =" + file_Spacenav_slidedown.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slidedown =" + functionscombobox_Spacenav_slidedown.Text + "\n"
                            + ConstuctCodeLine("spacenav_slidedown[0]", GetControlContent(slidedown_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slidedown[1]", GetControlContent(slidedown_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slidedown[2]", GetControlContent(slidedown_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slideright", GetControlContent(use_APP_Spacenav_slideright.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideright =" + inbuiltfunc_Spacenav_slideright.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideright =" + file_Spacenav_slideright.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideright =" + functionscombobox_Spacenav_slideright.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideright[0]", GetControlContent(slideright_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideright[1]", GetControlContent(slideright_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideright[2]", GetControlContent(slideright_3.Text), 3)
                            + ConstuctCodeLine("useapp_spacenav_slideleft", GetControlContent(use_APP_Spacenav_slideleft.IsChecked).ToLower(), 0)
                            + "// inbuiltfunc_Spacenav_slideleft =" + inbuiltfunc_Spacenav_slideleft.IsChecked.ToString().ToLower() + "\n"
                            + "// file_Spacenav_slideleft =" + file_Spacenav_slideleft.IsChecked.ToString().ToLower() + "\n"
                            + "// functionscombobox_Spacenav_slideleft =" + functionscombobox_Spacenav_slideleft.Text + "\n"
                            + ConstuctCodeLine("spacenav_slideleft[0]", GetControlContent(slideleft_1.Text), 3)
                            + ConstuctCodeLine("spacenav_slideleft[1]", GetControlContent(slideleft_2.Text), 3)
                            + ConstuctCodeLine("spacenav_slideleft[2]", GetControlContent(slideleft_3.Text), 3)
                            + fileversionModifier("spacenav", false)
                            + "}" + "\n"
                            );
                        // Add config settings to the configfile.
                        fs.Write(info, 0, info.Length);
                        //MessageBox.Show("Configuration Created");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ahmsville Dial", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateExistingConfig()
        {
            bool validity = true;
            int cindex = 0;
            // Put all config files in root directory into array.
            string[] configfiles = { };
            try
            {
                configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
            }
            catch (Exception)
            {

            }
            //string[] configfiles = Directory.GetFiles(selecteddialfilepath, "*_.ino"); // <-- Case-insensitive
            int[] idarray = { 0 };
            Array.Resize<int>(ref idarray, configfiles.Length);

            int cnt = 0;
            foreach (string s in configfiles)
            {
                string[] configsettings = System.IO.File.ReadAllLines(configfiles[cnt]);

                string configline = configsettings[3];
                int substringindex = configline.LastIndexOf("=");
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
                        sub = sub.Replace("//default", "");
                    }

                    cindex = Int32.Parse(sub);

                }
                if (idarray.Contains(cindex) || cindex > idarray.Length)
                {
                    validity = false;
                }
                idarray[cnt] = cindex;
                cnt += 1;
            }
            return validity;
        }
        private int SpaceNav_Mode()
        {
            //sets space nav mode identifiers
            int mode = 1;
            if (spacenav_manual.IsChecked == true)
            {
                mode = 1;
            }
            else if (spacenav_1.IsChecked == true)
            {
                mode = 2;
            }
            else if (spacenav_2.IsChecked == true)
            {
                mode = 3;
            }
            else if (spacenav_3.IsChecked == true)
            {
                mode = 4;
            }
            else if (spacenav_4.IsChecked == true)
            {
                mode = 5;
            }
            else if (spacenav_5.IsChecked == true)
            {
                mode = 6;
            }
            else if (spacenav_0.IsChecked == true)
            {
                mode = 0;
            }
            return mode;
        }

        //*****************************************************Dial Image Changers*********************************************************//
        private void Appledcolor_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\led.jpg"));
        }
        private void Knob1_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\knob1.jpg"));
        }

        private void Knob2_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\knob2.jpg"));
        }

        private void Capacitivetouch_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\captouch.jpg"));
        }

        private void Mk1_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\mk1.jpg"));
        }

        private void Mk2_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\mk2.jpg"));
        }

        private void Mk3_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\mk3.jpg"));
        }

        private void Mk4_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\mk4.jpg"));
        }

        private void Mk5_GotFocus(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\mk5.jpg"));
        }

        private void Spacenav_GotFocus(object sender, MouseEventArgs e)
        {
            // dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\led.jpg"));
        }

        private void tiltup_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\tiltup.jpg"));
        }

        private void tiltdown_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\tiltdown.jpg"));
        }

        private void tiltright_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\tiltright.jpg"));
        }

        private void tiltleft_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\tiltleft.jpg"));
        }

        private void slideup_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\slideup.jpg"));
        }
        
        private void slidedown_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\slidedown.jpg"));
        }

        private void slideright_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\slideright.jpg"));
        }

        private void slideleft_MouseEnter(object sender, MouseEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\slideleft.jpg"));
        }


        private void Settings_LostFocus(object sender, MouseEventArgs e)
        {
           dialimage.Source = new BitmapImage(new Uri(selecteddialimagepath + @"\default.jpg"));
        }


        //***********************************************************************************************************************************************//
        private void singletap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                singletap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                SingleTap.Text = "Single Tap       Shortcut 1";
            }
            else
            {
                singletap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                SingleTap.Text = "Single Tap";
            }
        }

        private void doubletap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                doubletap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                DoubleTap.Text = "Double Tap       Shortcut 1";
            }
            else
            {
                doubletap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                DoubleTap.Text = "Double Tap";
            }
        }

        private void mk1tap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                mk1tap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                Mk1Tap.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mk1tap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                Mk1Tap.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void mk2tap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                mk2tap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                Mk2Tap.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mk2tap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                Mk2Tap.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void mk3tap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                mk3tap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                Mk3Tap.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mk3tap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                Mk3Tap.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void mk4tap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                mk4tap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                Mk4Tap.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mk4tap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                Mk4Tap.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void mk5tap_dualfunc(object sender, RoutedEventArgs e)
        {
            if (sender.ToString().Contains("True"))
            {
                mk5tap_shortcut2.Visibility = System.Windows.Visibility.Visible;
                Mk5Tap.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mk5tap_shortcut2.Visibility = System.Windows.Visibility.Collapsed;
                Mk5Tap.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ContinuousSpaceNavMode(object sender, RoutedEventArgs e)
        {
            if (continuoustilt.IsChecked == true)
            {
                tiltdown_1.SelectedIndex = tiltright_1.SelectedIndex = tiltleft_1.SelectedIndex = tiltup_1.SelectedIndex;
                tiltdown_2.SelectedIndex = tiltright_2.SelectedIndex = tiltleft_2.SelectedIndex = tiltup_2.SelectedIndex;
            }
            if (continuousslide.IsChecked == true)
            {
                slidedown_1.SelectedIndex = slideright_1.SelectedIndex = slideleft_1.SelectedIndex = slideup_1.SelectedIndex;
                slidedown_2.SelectedIndex = slideright_2.SelectedIndex = slideleft_2.SelectedIndex = slideup_2.SelectedIndex;
            }

        }

        /************************************************************************************************************************************************************************************/
        //knob 1 use app
        private void Use_APP_knob1_CW_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_knob1_CW.IsChecked == true)
            {
                APP_knob1_CW.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_knob1_CW.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_knob1_CW.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_knob1_CW.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_knob1_CW_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_knob1_CW.IsChecked == true)
            {
                browsebutton_knob1_CW.IsEnabled = false;
                functionscombobox_knob1_CW.IsEditable = false;
            }
            else
            {
                browsebutton_knob1_CW.IsEnabled = true;
                functionscombobox_knob1_CW.IsEditable = true;
            }

        }

        private void Use_APP_knob1_CCW_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_knob1_CCW.IsChecked == true)
            {
                APP_knob1_CCW.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_knob1_CCW.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_knob1_CCW.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_knob1_CCW.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_knob1_CCW_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_knob1_CCW.IsChecked == true)
            {
                browsebutton_knob1_CCW.IsEnabled = false;
                functionscombobox_knob1_CCW.IsEditable = false;
            }
            else
            {
                browsebutton_knob1_CCW.IsEnabled = true;
                functionscombobox_knob1_CCW.IsEditable = true;
            }

        }

        private void Browsebutton_knob1_CW_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_knob1_CW.Text = filename;
            }
        }

        private void Browsebutton_knob1_CCW_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_knob1_CCW.Text = filename;
            }
        }

        //knob 2 use app
        private void Use_APP_knob2_CW_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_knob2_CW.IsChecked == true)
            {
                APP_knob2_CW.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_knob2_CW.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_knob2_CW.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_knob2_CW.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_knob2_CW_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_knob2_CW.IsChecked == true)
            {
                browsebutton_knob2_CW.IsEnabled = false;
                functionscombobox_knob2_CW.IsEditable = false;
            }
            else
            {
                browsebutton_knob2_CW.IsEnabled = true;
                functionscombobox_knob2_CW.IsEditable = true;
            }

        }

        private void Use_APP_knob2_CCW_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_knob2_CCW.IsChecked == true)
            {
                APP_knob2_CCW.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_knob2_CCW.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_knob2_CCW.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_knob2_CCW.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_knob2_CCW_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_knob2_CCW.IsChecked == true)
            {
                browsebutton_knob2_CCW.IsEnabled = false;
                functionscombobox_knob2_CCW.IsEditable = false;
            }
            else
            {
                browsebutton_knob2_CCW.IsEnabled = true;
                functionscombobox_knob2_CCW.IsEditable = true;
            }

        }

        private void Browsebutton_knob2_CW_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_knob2_CW.Text = filename;
            }
        }

        private void Browsebutton_knob2_CCW_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_knob2_CCW.Text = filename;
            }
        }


        //captouch use app
        private void Use_APP_captouch_singletap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_singletap_SHORTCUT1.IsChecked == true)
            {
                APP_captouch_singletap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_captouch_singletap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_singletap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_captouch_singletap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_captouch_singletap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_singletap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_captouch_singletap_SHORTCUT1.IsEnabled = false;
                functionscombobox_captouch_singletap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_singletap_SHORTCUT1.IsEnabled = true;
                functionscombobox_captouch_singletap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_captouch_singletap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_singletap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_captouch_singletap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_singletap_SHORTCUT2.IsChecked == true)
            {
                APP_captouch_singletap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_captouch_singletap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_singletap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_captouch_singletap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_captouch_singletap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_singletap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_captouch_singletap_SHORTCUT2.IsEnabled = false;
                functionscombobox_captouch_singletap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_singletap_SHORTCUT2.IsEnabled = true;
                functionscombobox_captouch_singletap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_captouch_singletap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_singletap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_captouch_doubletap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_doubletap_SHORTCUT1.IsChecked == true)
            {
                APP_captouch_doubletap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_captouch_doubletap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_doubletap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_captouch_doubletap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_captouch_doubletap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_doubletap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_captouch_doubletap_SHORTCUT1.IsEnabled = false;
                functionscombobox_captouch_doubletap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_doubletap_SHORTCUT1.IsEnabled = true;
                functionscombobox_captouch_doubletap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_captouch_doubletap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_doubletap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_captouch_doubletap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_doubletap_SHORTCUT2.IsChecked == true)
            {
                APP_captouch_doubletap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_captouch_doubletap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_doubletap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_captouch_doubletap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_captouch_doubletap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_doubletap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_captouch_doubletap_SHORTCUT2.IsEnabled = false;
                functionscombobox_captouch_doubletap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_doubletap_SHORTCUT2.IsEnabled = true;
                functionscombobox_captouch_doubletap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_captouch_doubletap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_doubletap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_captouch_shortpress_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_shortpress.IsChecked == true)
            {
                APP_captouch_shortpress.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_captouch_shortpress.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_shortpress.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_captouch_shortpress.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_captouch_shortpress_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_shortpress.IsChecked == true)
            {
                browsebutton_captouch_shortpress.IsEnabled = false;
                functionscombobox_captouch_shortpress.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_shortpress.IsEnabled = true;
                functionscombobox_captouch_shortpress.IsEditable = true;
            }

        }
        
        private void Browsebutton_captouch_shortpress_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_shortpress.Text = filename;
            }
        }

        private void Use_APP_captouch_longpress_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_captouch_longpress.IsChecked == true)
            {
                APP_captouch_longpress.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_captouch_longpress.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_captouch_longpress.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_captouch_longpress.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_captouch_longpress_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_captouch_longpress.IsChecked == true)
            {
                browsebutton_captouch_longpress.IsEnabled = false;
                functionscombobox_captouch_longpress.IsEditable = false;
            }
            else
            {
                browsebutton_captouch_longpress.IsEnabled = true;
                functionscombobox_captouch_longpress.IsEditable = true;
            }

        }

        private void Browsebutton_captouch_longpress_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_captouch_longpress.Text = filename;
            }
        }


        //MK1 use app
        private void Use_APP_MK1tap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK1tap_SHORTCUT1.IsChecked == true)
            {
                APP_MK1tap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_MK1tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK1tap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_MK1tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK1tap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK1tap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_MK1tap_SHORTCUT1.IsEnabled = false;
                functionscombobox_MK1tap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_MK1tap_SHORTCUT1.IsEnabled = true;
                functionscombobox_MK1tap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_MK1tap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK1tap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_MK1tap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK1tap_SHORTCUT2.IsChecked == true)
            {
                APP_MK1tap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_MK1tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK1tap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_MK1tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK1tap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK1tap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_MK1tap_SHORTCUT2.IsEnabled = false;
                functionscombobox_MK1tap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_MK1tap_SHORTCUT2.IsEnabled = true;
                functionscombobox_MK1tap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_MK1tap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK1tap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_MK1hold_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK1hold.IsChecked == true)
            {
                APP_MK1hold.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_MK1hold.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK1hold.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_MK1hold.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_MK1hold_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK1hold.IsChecked == true)
            {
                browsebutton_MK1hold.IsEnabled = false;
                functionscombobox_MK1hold.IsEditable = false;
            }
            else
            {
                browsebutton_MK1hold.IsEnabled = true;
                functionscombobox_MK1hold.IsEditable = true;
            }

        }

        private void Browsebutton_MK1hold_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK1hold.Text = filename;
            }
        }

        //MK2 use app
        private void Use_APP_MK2tap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK2tap_SHORTCUT1.IsChecked == true)
            {
                APP_MK2tap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_MK2tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK2tap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_MK2tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK2tap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK2tap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_MK2tap_SHORTCUT1.IsEnabled = false;
                functionscombobox_MK2tap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_MK2tap_SHORTCUT1.IsEnabled = true;
                functionscombobox_MK2tap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_MK2tap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK2tap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_MK2tap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK2tap_SHORTCUT2.IsChecked == true)
            {
                APP_MK2tap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_MK2tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK2tap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_MK2tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK2tap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK2tap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_MK2tap_SHORTCUT2.IsEnabled = false;
                functionscombobox_MK2tap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_MK2tap_SHORTCUT2.IsEnabled = true;
                functionscombobox_MK2tap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_MK2tap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK2tap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_MK2hold_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK2hold.IsChecked == true)
            {
                APP_MK2hold.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_MK2hold.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK2hold.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_MK2hold.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_MK2hold_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK2hold.IsChecked == true)
            {
                browsebutton_MK2hold.IsEnabled = false;
                functionscombobox_MK2hold.IsEditable = false;
            }
            else
            {
                browsebutton_MK2hold.IsEnabled = true;
                functionscombobox_MK2hold.IsEditable = true;
            }

        }

        private void Browsebutton_MK2hold_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK2hold.Text = filename;
            }
        }

        //MK3 use app
        private void Use_APP_MK3tap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK3tap_SHORTCUT1.IsChecked == true)
            {
                APP_MK3tap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_MK3tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK3tap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_MK3tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK3tap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK3tap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_MK3tap_SHORTCUT1.IsEnabled = false;
                functionscombobox_MK3tap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_MK3tap_SHORTCUT1.IsEnabled = true;
                functionscombobox_MK3tap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_MK3tap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK3tap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_MK3tap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK3tap_SHORTCUT2.IsChecked == true)
            {
                APP_MK3tap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_MK3tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK3tap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_MK3tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK3tap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK3tap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_MK3tap_SHORTCUT2.IsEnabled = false;
                functionscombobox_MK3tap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_MK3tap_SHORTCUT2.IsEnabled = true;
                functionscombobox_MK3tap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_MK3tap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK3tap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_MK3hold_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK3hold.IsChecked == true)
            {
                APP_MK3hold.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_MK3hold.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK3hold.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_MK3hold.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_MK3hold_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK3hold.IsChecked == true)
            {
                browsebutton_MK3hold.IsEnabled = false;
                functionscombobox_MK3hold.IsEditable = false;
            }
            else
            {
                browsebutton_MK3hold.IsEnabled = true;
                functionscombobox_MK3hold.IsEditable = true;
            }

        }

        private void Browsebutton_MK3hold_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK3hold.Text = filename;
            }
        }

        //MK4 use app
        private void Use_APP_MK4tap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK4tap_SHORTCUT1.IsChecked == true)
            {
                APP_MK4tap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_MK4tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK4tap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_MK4tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK4tap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK4tap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_MK4tap_SHORTCUT1.IsEnabled = false;
                functionscombobox_MK4tap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_MK4tap_SHORTCUT1.IsEnabled = true;
                functionscombobox_MK4tap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_MK4tap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK4tap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_MK4tap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK4tap_SHORTCUT2.IsChecked == true)
            {
                APP_MK4tap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_MK4tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK4tap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_MK4tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK4tap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK4tap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_MK4tap_SHORTCUT2.IsEnabled = false;
                functionscombobox_MK4tap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_MK4tap_SHORTCUT2.IsEnabled = true;
                functionscombobox_MK4tap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_MK4tap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK4tap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_MK4hold_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK4hold.IsChecked == true)
            {
                APP_MK4hold.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_MK4hold.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK4hold.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_MK4hold.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_MK4hold_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK4hold.IsChecked == true)
            {
                browsebutton_MK4hold.IsEnabled = false;
                functionscombobox_MK4hold.IsEditable = false;
            }
            else
            {
                browsebutton_MK4hold.IsEnabled = true;
                functionscombobox_MK4hold.IsEditable = true;
            }

        }

        private void Browsebutton_MK4hold_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK4hold.Text = filename;
            }
        }

        //MK5 use app
        private void Use_APP_MK5tap_SHORTCUT1_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK5tap_SHORTCUT1.IsChecked == true)
            {
                APP_MK5tap_SHORTCUT1.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT1_MK5tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK5tap_SHORTCUT1.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT1_MK5tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK5tap_SHORTCUT1_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK5tap_SHORTCUT1.IsChecked == true)
            {
                browsebutton_MK5tap_SHORTCUT1.IsEnabled = false;
                functionscombobox_MK5tap_SHORTCUT1.IsEditable = false;
            }
            else
            {
                browsebutton_MK5tap_SHORTCUT1.IsEnabled = true;
                functionscombobox_MK5tap_SHORTCUT1.IsEditable = true;
            }
        }

        private void Browsebutton_MK5tap_SHORTCUT1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK5tap_SHORTCUT1.Text = filename;
            }
        }

        private void Use_APP_MK5tap_SHORTCUT2_checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK5tap_SHORTCUT2.IsChecked == true)
            {
                APP_MK5tap_SHORTCUT2.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT2_MK5tap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK5tap_SHORTCUT2.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT2_MK5tap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Inbuiltfunc_MK5tap_SHORTCUT2_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK5tap_SHORTCUT2.IsChecked == true)
            {
                browsebutton_MK5tap_SHORTCUT2.IsEnabled = false;
                functionscombobox_MK5tap_SHORTCUT2.IsEditable = false;
            }
            else
            {
                browsebutton_MK5tap_SHORTCUT2.IsEnabled = true;
                functionscombobox_MK5tap_SHORTCUT2.IsEditable = true;
            }
        }

        private void Browsebutton_MK5tap_SHORTCUT2_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK5tap_SHORTCUT2.Text = filename;
            }
        }

        private void Use_APP_MK5hold_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_MK5hold.IsChecked == true)
            {
                APP_MK5hold.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_MK5hold.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_MK5hold.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_MK5hold.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_MK5hold_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_MK5hold.IsChecked == true)
            {
                browsebutton_MK5hold.IsEnabled = false;
                functionscombobox_MK5hold.IsEditable = false;
            }
            else
            {
                browsebutton_MK5hold.IsEnabled = true;
                functionscombobox_MK5hold.IsEditable = true;
            }

        }

        private void Browsebutton_MK5hold_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_MK5hold.Text = filename;
            }
        }


        //spacenav use app
        private void Use_APP_Spacenav_tiltup_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_tiltup.IsChecked == true)
            {
                APP_Spacenav_tiltup.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_tiltup.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_tiltup.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_tiltup.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_tiltup_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_tiltup.IsChecked == true)
            {
                browsebutton_Spacenav_tiltup.IsEnabled = false;
                functionscombobox_Spacenav_tiltup.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_tiltup.IsEnabled = true;
                functionscombobox_Spacenav_tiltup.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_tiltup_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_tiltup.Text = filename;
            }
        }

        private void Use_APP_Spacenav_tiltdown_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_tiltdown.IsChecked == true)
            {
                APP_Spacenav_tiltdown.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_tiltdown.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_tiltdown.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_tiltdown.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_tiltdown_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_tiltdown.IsChecked == true)
            {
                browsebutton_Spacenav_tiltdown.IsEnabled = false;
                functionscombobox_Spacenav_tiltdown.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_tiltdown.IsEnabled = true;
                functionscombobox_Spacenav_tiltdown.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_tiltdown_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_tiltdown.Text = filename;
            }
        }

        private void Use_APP_Spacenav_tiltright_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_tiltright.IsChecked == true)
            {
                APP_Spacenav_tiltright.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_tiltright.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_tiltright.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_tiltright.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_tiltright_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_tiltright.IsChecked == true)
            {
                browsebutton_Spacenav_tiltright.IsEnabled = false;
                functionscombobox_Spacenav_tiltright.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_tiltright.IsEnabled = true;
                functionscombobox_Spacenav_tiltright.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_tiltright_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_tiltright.Text = filename;
            }
        }

        private void Use_APP_Spacenav_tiltleft_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_tiltleft.IsChecked == true)
            {
                APP_Spacenav_tiltleft.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_tiltleft.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_tiltleft.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_tiltleft.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_tiltleft_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_tiltleft.IsChecked == true)
            {
                browsebutton_Spacenav_tiltleft.IsEnabled = false;
                functionscombobox_Spacenav_tiltleft.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_tiltleft.IsEnabled = true;
                functionscombobox_Spacenav_tiltleft.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_tiltleft_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_tiltleft.Text = filename;
            }
        }

        private void Use_APP_Spacenav_slideup_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_slideup.IsChecked == true)
            {
                APP_Spacenav_slideup.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_slideup.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_slideup.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_slideup.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_slideup_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_slideup.IsChecked == true)
            {
                browsebutton_Spacenav_slideup.IsEnabled = false;
                functionscombobox_Spacenav_slideup.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_slideup.IsEnabled = true;
                functionscombobox_Spacenav_slideup.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_slideup_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_slideup.Text = filename;
            }
        }

        private void Use_APP_Spacenav_slidedown_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_slidedown.IsChecked == true)
            {
                APP_Spacenav_slidedown.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_slidedown.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_slidedown.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_slidedown.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_slidedown_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_slidedown.IsChecked == true)
            {
                browsebutton_Spacenav_slidedown.IsEnabled = false;
                functionscombobox_Spacenav_slidedown.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_slidedown.IsEnabled = true;
                functionscombobox_Spacenav_slidedown.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_slidedown_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_slidedown.Text = filename;
            }
        }

        private void Use_APP_Spacenav_slideright_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_slideright.IsChecked == true)
            {
                APP_Spacenav_slideright.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_slideright.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_slideright.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_slideright.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_slideright_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_slideright.IsChecked == true)
            {
                browsebutton_Spacenav_slideright.IsEnabled = false;
                functionscombobox_Spacenav_slideright.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_slideright.IsEnabled = true;
                functionscombobox_Spacenav_slideright.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_slideright_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_slideright.Text = filename;
            }
        }

        private void Use_APP_Spacenav_slideleft_Checked(object sender, RoutedEventArgs e)
        {
            if (use_APP_Spacenav_slideleft.IsChecked == true)
            {
                APP_Spacenav_slideleft.Visibility = System.Windows.Visibility.Visible;
                SHORTCUT_Spacenav_slideleft.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                APP_Spacenav_slideleft.Visibility = System.Windows.Visibility.Collapsed;
                SHORTCUT_Spacenav_slideleft.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void Inbuiltfunc_Spacenav_slideleft_Checked(object sender, RoutedEventArgs e)
        {
            if (inbuiltfunc_Spacenav_slideleft.IsChecked == true)
            {
                browsebutton_Spacenav_slideleft.IsEnabled = false;
                functionscombobox_Spacenav_slideleft.IsEditable = false;
            }
            else
            {
                browsebutton_Spacenav_slideleft.IsEnabled = true;
                functionscombobox_Spacenav_slideleft.IsEditable = true;
            }

        }

        private void Browsebutton_Spacenav_slideleft_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                functionscombobox_Spacenav_slideleft.Text = filename;
            }
        }

        private void Pagecontent_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            MessageBox.Show(e.NewFocus.ToString());
        }


        private void knob1res_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (knob1res.Value == 0)
            {
                knob1resvalue.Text = "Default";
            }
            else
            {
                knob1resvalue.Text = knob1res.Value.ToString() + " %";
            }

        }
        private void knob2res_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (knob2res.Value == 0)
            {
                knob2resvalue.Text = "Default";
            }
            else
            {
                knob2resvalue.Text = knob2res.Value.ToString() + " %";
            }

        }

        private string fileversionModifier(string feature, bool begin_or_end)
        {
            string commentline = "\n";
            if (feature == "secenc")  //secondary encoder
            {
                if (diallist.SelectedIndex != 2 && diallist.SelectedIndex != 3) 
                {
                    if (begin_or_end == true)
                    {
                        commentline = "/*" + commentline; //begin comment
                    }
                    else
                    {
                        commentline = "*/" + commentline; //end comment
                    }
                }
            }
            else if (feature == "mk") //macro key
            {
                if (diallist.SelectedIndex != 1 && diallist.SelectedIndex != 3) 
                {
                    if (begin_or_end == true)
                    {
                        commentline = "/*" + commentline; //begin comment
                    }
                    else
                    {
                        commentline = "*/" + commentline; //end comment
                    }
                }
            }
            else if (feature == "spacenav") //spacenav
            {
                if (diallist.SelectedIndex != 2 && diallist.SelectedIndex != 3) 
                {
                    if (begin_or_end == true)
                    {
                        commentline = "/*" + commentline; //begin comment
                    }
                    else
                    {
                        commentline = "*/" + commentline; //end comment
                    }
                }
            }
            return commentline;
        }

        private void previewtextboxstring(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;
            MessageBox.Show(textBox.Text);
        }

        

       






        //****************************************************************************************************************************************************//

        //************************************************************************************************************************************//

        private void LoadSettingsFromConfigFile()
        {
            //translate setting in config file to UI setting.
            DefaultConfig.IsChecked = false;
            int[] lines = { 3, 10, 17, 24, 37, 48, 54, 63, 72, 81, 90, 99, 113 };
            string selectedconfig = ((ComboBoxItem)this.configlist.SelectedItem).Tag.ToString();
            string[] configsettings = System.IO.File.ReadAllLines(selectedconfig);
            for (int i = 0; i < configsettings.Length; i++)
            {
                string configline = configsettings[i];
                int substringindex = configline.LastIndexOf("=");
                if (substringindex > 0)
                {
                    string sub = configline.Substring(substringindex);
                    if (sub.StartsWith("=\""))  //load strings differently
                    {
                        sub = sub.Replace("=", "");
                        sub = sub.Replace("\";", "");
                        sub = sub.Replace("\"", "");
                        sub = sub.Replace(nullchar, "");
                        sub = sub.Replace("#", Environment.NewLine);
                    }
                    else
                    {
                        sub = sub.Replace("=", "");
                        sub = sub.Replace(";", "");
                        sub = sub.Replace("\"", "");
                        sub = sub.Replace("'", "");
                        sub = sub.Replace(nullchar, "");
                        sub = sub.Replace("CRGB::", "");
                    }
                    
                    if (sub.Contains("//default"))
                    {
                        DefaultConfig.IsChecked = true;
                        sub = sub.Replace("//default","");
                    }
                    
                    configsettings[i] = sub;
                }
            }
            int currentline = lines[0];
            configname.Text = configsettings[currentline - 1];
            configid.Text = configsettings[currentline++]; 
            appledcolor.SelectedValue = configsettings[currentline++];
            appledanimation.SelectedValue = configsettings[currentline++];
            currentline++;
            knob1res.Value = Int32.Parse((configsettings[currentline++]));
            use_APP_knob1_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_knob1_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            file_knob1_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_knob1_CW.Text = configsettings[currentline++];
            knob1CW_1.Text = configsettings[currentline++];
            knob1CW_2.Text = configsettings[currentline++];
            knob1CW_3.Text = configsettings[currentline++];
            use_APP_knob1_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_knob1_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            file_knob1_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_knob1_CCW.Text = configsettings[currentline++];
            knob1CCW_1.Text = configsettings[currentline++];
            knob1CCW_2.Text = configsettings[currentline++];
            knob1CCW_3.Text = configsettings[currentline++];
            currentline++;
            knob2res.Value = Int32.Parse((configsettings[currentline++]));
            use_APP_knob2_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_knob2_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            file_knob2_CW.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_knob2_CW.Text = configsettings[currentline++];
            knob2CW_1.Text = configsettings[currentline++];
            knob2CW_2.Text = configsettings[currentline++];
            knob2CW_3.Text = configsettings[currentline++];
            use_APP_knob2_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_knob2_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            file_knob2_CCW.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_knob2_CCW.Text = configsettings[currentline++];
            knob2CCW_1.Text = configsettings[currentline++];
            knob2CCW_2.Text = configsettings[currentline++];
            knob2CCW_3.Text = configsettings[currentline++];
            currentline++;
            singletap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            doubletap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            use_APP_captouch_singletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_singletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_singletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_singletap_SHORTCUT1.Text = configsettings[currentline++];
            singletap_1.Text = configsettings[currentline++];
            singletap_2.Text = configsettings[currentline++];
            singletap_3.Text = configsettings[currentline++];
            use_APP_captouch_singletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_singletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_singletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_singletap_SHORTCUT2.Text = configsettings[currentline++];
            singletap_21.Text = configsettings[currentline++];
            singletap_22.Text = configsettings[currentline++];
            singletap_23.Text = configsettings[currentline++];
            use_APP_captouch_doubletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_doubletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_doubletap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_doubletap_SHORTCUT1.Text = configsettings[currentline++];
            doubletap_1.Text = configsettings[currentline++];
            doubletap_2.Text = configsettings[currentline++];
            doubletap_3.Text = configsettings[currentline++];
            use_APP_captouch_doubletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_doubletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_doubletap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_doubletap_SHORTCUT2.Text = configsettings[currentline++];
            doubletap_21.Text = configsettings[currentline++];
            doubletap_22.Text = configsettings[currentline++];
            doubletap_23.Text = configsettings[currentline++];
            use_APP_captouch_shortpress.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_shortpress.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_shortpress.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_shortpress.Text = configsettings[currentline++];
            shortpress_1.Text = configsettings[currentline++];
            shortpress_2.Text = configsettings[currentline++];
            shortpress_3.Text = configsettings[currentline++];
            use_APP_captouch_longpress.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_captouch_longpress.IsChecked = bool.Parse(configsettings[currentline++]);
            file_captouch_longpress.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_captouch_longpress.Text = configsettings[currentline++];
            longpress_1.Text = configsettings[currentline++];
            longpress_2.Text = configsettings[currentline++];
            longpress_3.Text = configsettings[currentline++];
            currentline++;
            mk1tap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            mk2tap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            mk3tap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            mk4tap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            mk5tap_dualshortcut.IsChecked = bool.Parse(configsettings[currentline++]);
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            currentline++;
            mk1ledcolor.SelectedValue = configsettings[currentline++];
            mk2ledcolor.SelectedValue = configsettings[currentline++];
            mk3ledcolor.SelectedValue = configsettings[currentline++];
            mk4ledcolor.SelectedValue = configsettings[currentline++];
            mk5ledcolor.SelectedValue = configsettings[currentline++];
            mk1ledanimation.SelectedValue = configsettings[currentline++];
            mk2ledanimation.SelectedValue = configsettings[currentline++];
            mk3ledanimation.SelectedValue = configsettings[currentline++];
            mk4ledanimation.SelectedValue = configsettings[currentline++];
            mk5ledanimation.SelectedValue = configsettings[currentline++];
            currentline++;
            use_APP_MK1tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK1tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK1tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK1tap_SHORTCUT1.Text = configsettings[currentline++];
            mk1tap_1.Text = configsettings[currentline++];
            mk1tap_2.Text = configsettings[currentline++];
            mk1tap_3.Text = configsettings[currentline++];
            use_APP_MK1tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK1tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK1tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK1tap_SHORTCUT2.Text = configsettings[currentline++];
            mk1tap_21.Text = configsettings[currentline++];
            mk1tap_22.Text = configsettings[currentline++];
            mk1tap_23.Text = configsettings[currentline++];
            mk1tap_4.Text = configsettings[currentline++];
            use_APP_MK1hold.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK1hold.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK1hold.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK1hold.Text = configsettings[currentline++];
            mk1hold_1.Text = configsettings[currentline++];
            mk1hold_2.Text = configsettings[currentline++];
            mk1hold_3.Text = configsettings[currentline++];
            mk1hold_4.Text = configsettings[currentline++];
            currentline++;
            use_APP_MK2tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK2tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK2tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK2tap_SHORTCUT1.Text = configsettings[currentline++];
            mk2tap_1.Text = configsettings[currentline++];
            mk2tap_2.Text = configsettings[currentline++];
            mk2tap_3.Text = configsettings[currentline++];
            use_APP_MK2tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK2tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK2tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK2tap_SHORTCUT2.Text = configsettings[currentline++];
            mk2tap_21.Text = configsettings[currentline++];
            mk2tap_22.Text = configsettings[currentline++];
            mk2tap_23.Text = configsettings[currentline++];
            mk2tap_4.Text = configsettings[currentline++];
            use_APP_MK2hold.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK2hold.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK2hold.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK2hold.Text = configsettings[currentline++];
            mk2hold_1.Text = configsettings[currentline++];
            mk2hold_2.Text = configsettings[currentline++];
            mk2hold_3.Text = configsettings[currentline++];
            mk2hold_4.Text = configsettings[currentline++];
            currentline++;
            use_APP_MK3tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK3tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK3tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK3tap_SHORTCUT1.Text = configsettings[currentline++];
            mk3tap_1.Text = configsettings[currentline++];
            mk3tap_2.Text = configsettings[currentline++];
            mk3tap_3.Text = configsettings[currentline++];
            use_APP_MK3tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK3tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK3tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK3tap_SHORTCUT2.Text = configsettings[currentline++];
            mk3tap_21.Text = configsettings[currentline++];
            mk3tap_22.Text = configsettings[currentline++];
            mk3tap_23.Text = configsettings[currentline++];
            mk3tap_4.Text = configsettings[currentline++];
            use_APP_MK3hold.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK3hold.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK3hold.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK3hold.Text = configsettings[currentline++];
            mk3hold_1.Text = configsettings[currentline++];
            mk3hold_2.Text = configsettings[currentline++];
            mk3hold_3.Text = configsettings[currentline++];
            mk3hold_4.Text = configsettings[currentline++];
            currentline++;
            use_APP_MK4tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK4tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK4tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK4tap_SHORTCUT1.Text = configsettings[currentline++];
            mk4tap_1.Text = configsettings[currentline++];
            mk4tap_2.Text = configsettings[currentline++];
            mk4tap_3.Text = configsettings[currentline++];
            use_APP_MK4tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK4tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK4tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK4tap_SHORTCUT2.Text = configsettings[currentline++];
            mk4tap_21.Text = configsettings[currentline++];
            mk4tap_22.Text = configsettings[currentline++];
            mk4tap_23.Text = configsettings[currentline++];
            mk4tap_4.Text = configsettings[currentline++];
            use_APP_MK4hold.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK4hold.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK4hold.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK4hold.Text = configsettings[currentline++];
            mk4hold_1.Text = configsettings[currentline++];
            mk4hold_2.Text = configsettings[currentline++];
            mk4hold_3.Text = configsettings[currentline++];
            mk4hold_4.Text = configsettings[currentline++];
            currentline++;
            use_APP_MK5tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK5tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK5tap_SHORTCUT1.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK5tap_SHORTCUT1.Text = configsettings[currentline++];
            mk5tap_1.Text = configsettings[currentline++];
            mk5tap_2.Text = configsettings[currentline++];
            mk5tap_3.Text = configsettings[currentline++];
            use_APP_MK5tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK5tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK5tap_SHORTCUT2.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK5tap_SHORTCUT2.Text = configsettings[currentline++];
            mk5tap_21.Text = configsettings[currentline++];
            mk5tap_22.Text = configsettings[currentline++];
            mk5tap_23.Text = configsettings[currentline++];
            mk5tap_4.Text = configsettings[currentline++];
            use_APP_MK5hold.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_MK5hold.IsChecked = bool.Parse(configsettings[currentline++]);
            file_MK5hold.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_MK5hold.Text = configsettings[currentline++];
            mk5hold_1.Text = configsettings[currentline++];
            mk5hold_2.Text = configsettings[currentline++];
            mk5hold_3.Text = configsettings[currentline++];
            mk5hold_4.Text = configsettings[currentline++];
            currentline++;
            currentline++;
            int templine = currentline++;
            if (configsettings[templine] == "1")
            {
                spacenav_manual.IsChecked = true;
            }
            else if (configsettings[templine] == "2")
            {
                spacenav_1.IsChecked = true;
            }
            else if (configsettings[templine] == "3")
            {
                spacenav_2.IsChecked = true;
            }
            else if (configsettings[templine] == "4")
            {
                spacenav_3.IsChecked = true;
            }
            else if (configsettings[templine] == "5")
            {
                spacenav_4.IsChecked = true;
            }
            else if (configsettings[templine] == "6")
            {
                spacenav_5.IsChecked = true;
            }
            else if (configsettings[templine] == "0")
            {
                spacenav_0.IsChecked = true;
            }
            continuoustilt.IsChecked = bool.Parse(configsettings[currentline++]);
            continuousslide.IsChecked = bool.Parse(configsettings[currentline++]);
            use_APP_Spacenav_tiltup.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_tiltup.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_tiltup.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_tiltup.Text = configsettings[currentline++];
            tiltup_1.Text = configsettings[currentline++];
            tiltup_2.Text = configsettings[currentline++];
            tiltup_3.Text = configsettings[currentline++];
            use_APP_Spacenav_tiltdown.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_tiltdown.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_tiltdown.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_tiltdown.Text = configsettings[currentline++];
            tiltdown_1.Text = configsettings[currentline++];
            tiltdown_2.Text = configsettings[currentline++];
            tiltdown_3.Text = configsettings[currentline++];
            use_APP_Spacenav_tiltright.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_tiltright.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_tiltright.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_tiltright.Text = configsettings[currentline++];
            tiltright_1.Text = configsettings[currentline++];
            tiltright_2.Text = configsettings[currentline++];
            tiltright_3.Text = configsettings[currentline++];
            use_APP_Spacenav_tiltleft.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_tiltleft.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_tiltleft.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_tiltleft.Text = configsettings[currentline++];
            tiltleft_1.Text = configsettings[currentline++];
            tiltleft_2.Text = configsettings[currentline++];
            tiltleft_3.Text = configsettings[currentline++];
            currentline++;
            use_APP_Spacenav_slideup.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_slideup.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_slideup.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_slideup.Text = configsettings[currentline++];
            slideup_1.Text = configsettings[currentline++];
            slideup_2.Text = configsettings[currentline++];
            slideup_3.Text = configsettings[currentline++];
            use_APP_Spacenav_slidedown.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_slidedown.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_slidedown.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_slidedown.Text = configsettings[currentline++];
            slidedown_1.Text = configsettings[currentline++];
            slidedown_2.Text = configsettings[currentline++];
            slidedown_3.Text = configsettings[currentline++];
            use_APP_Spacenav_slideright.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_slideright.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_slideright.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_slideright.Text = configsettings[currentline++];
            slideright_1.Text = configsettings[currentline++];
            slideright_2.Text = configsettings[currentline++];
            slideright_3.Text = configsettings[currentline++];
            use_APP_Spacenav_slideleft.IsChecked = bool.Parse(configsettings[currentline++]);
            inbuiltfunc_Spacenav_slideleft.IsChecked = bool.Parse(configsettings[currentline++]);
            file_Spacenav_slideleft.IsChecked = bool.Parse(configsettings[currentline++]);
            functionscombobox_Spacenav_slideleft.Text = configsettings[currentline++];
            slideleft_1.Text = configsettings[currentline++];
            slideleft_2.Text = configsettings[currentline++];
            slideleft_3.Text = configsettings[currentline++];
        }

    }
}
