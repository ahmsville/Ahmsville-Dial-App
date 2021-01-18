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
    /// Interaction logic for Dial_Ultimate.xaml
    /// </summary>
    public partial class Dial_MacroKey : Page
    {
        double faded = 0.5, opaque = 1;
        string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\Ahmsville_dial_v2\examples\Ahmsville_Dial_Macro_version\";  //path for Dial's config files and library files 
        static string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;  //path for software related files
        int configcount;

        //dial imagepath array
        string[] dialimagepaths = {softwarepath + @"\macro\led.png", softwarepath + @"\macro\knob1.png", softwarepath + @"\macro\knob2.png", softwarepath + @"\macro\captouch.png", softwarepath + @"\macro\mk1.png",
            softwarepath + @"\macro\mk2.png", softwarepath + @"\macro\mk3.png", softwarepath + @"\macro\mk4.png", softwarepath + @"\macro\mk5.png", softwarepath + @"\macro\spacenav.png", softwarepath + @"\macro\default.png"};


        public Dial_MacroKey()
        {
            InitializeComponent();
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[10]));
            DataContext = new AhmsvilleDialViewModel();
            configname.Text = "NewAppConfig";
            PopulateConfigList();  //load configuration selection
            configid.IsEnabled = false;

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
            string[] configfiles = Directory.GetFiles(filepath, "*_.ino"); // <-- Case-insensitive
            // fill configuration name combobox
            foreach (string fullconfigpath in configfiles)
            {
                string configname = fullconfigpath.Replace(filepath, "");
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
        }

        private void Button_CreateConfiguration(object sender, RoutedEventArgs e)
        {

            if (ValidateConfigName(configname.Text.ToString()) == false)
            {  //validate config name for special characters
                string filename = configname.Text.ToString().Replace(" ", ""); //remove space from name
                configname.Text = filename;
                string configpath = filepath + filename + "_.ino"; //generate config's absolute path

                try
                {
                    int Id = Int32.Parse(GetControlContent(configid.Text));
                    // Create the file, or overwrite if the file exists.
                    using (FileStream fs = File.Create(configpath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("void " + GetControlContent(configname.Text) + "() {" + "\n"
                            + "int index=" + (Id - 1).ToString() + ";" + "\n"
                            + ConstuctCodeLine("appname", GetControlContent(configname.Text), 1)
                            + ConstuctCodeLine("ID", Id.ToString(), 0)
                            + ConstuctCodeLine("appcolor", "CRGB::" + GetControlContent_Led(appledcolor.SelectedItem), 0)
                            + "\n"
                            + ConstuctCodeLine("knob1_CW[0]", GetControlContent(knob1CW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CW[1]", GetControlContent(knob1CW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CW[2]", GetControlContent(knob1CW_3.Text), 2)
                            + ConstuctCodeLine("knob1_CCW[0]", GetControlContent(knob1CCW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CCW[1]", GetControlContent(knob1CCW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CCW[2]", GetControlContent(knob1CCW_3.Text), 2)
                            + "\n"
                            + ConstuctCodeLine("singletap[0]", GetControlContent(singletap_1.SelectedItem), 0)
                            + ConstuctCodeLine("singletap[1]", GetControlContent(singletap_2.SelectedItem), 0)
                            + ConstuctCodeLine("singletap[2]", GetControlContent(singletap_3.Text), 2)
                            + ConstuctCodeLine("doubletap[0]", GetControlContent(doubletap_1.SelectedItem), 0)
                            + ConstuctCodeLine("doubletap[1]", GetControlContent(doubletap_2.SelectedItem), 0)
                            + ConstuctCodeLine("doubletap[2]", GetControlContent(doubletap_3.Text), 2)
                            + ConstuctCodeLine("shortpress[0]", GetControlContent(shortpress_1.SelectedItem), 0)
                            + ConstuctCodeLine("shortpress[1]", GetControlContent(shortpress_2.SelectedItem), 0)
                            + ConstuctCodeLine("shortpress[2]", GetControlContent(shortpress_3.Text), 2)
                            + ConstuctCodeLine("longpress[0]", GetControlContent(longpress_1.SelectedItem), 0)
                            + ConstuctCodeLine("longpress[1]", GetControlContent(longpress_2.SelectedItem), 0)
                            + ConstuctCodeLine("longpress[2]", GetControlContent(longpress_3.Text), 2)
                            + "\n"
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
                            + ConstuctCodeLine("MK_colors[0]", "CRGB::" + GetControlContent_Led(mk1ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[1]", "CRGB::" + GetControlContent_Led(mk2ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[2]", "CRGB::" + GetControlContent_Led(mk3ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[3]", "CRGB::" + GetControlContent_Led(mk4ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[4]", "CRGB::" + GetControlContent_Led(mk5ledcolor.SelectedItem), 0)
                            + "\n"
                            + ConstuctCodeLine("MK1_tap[0]", GetControlContent(mk1tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_tap[1]", GetControlContent(mk1tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_tap[2]", GetControlContent(mk1tap_3.Text), 2)
                            + ConstuctCodeLine("MK1_taptext", GetControlContent(mk1tap_4.Text), 1)
                            + ConstuctCodeLine("MK1_hold[0]", GetControlContent(mk1hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_hold[1]", GetControlContent(mk1hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_hold[2]", GetControlContent(mk1hold_3.Text), 2)
                            + ConstuctCodeLine("MK1_holdtext", GetControlContent(mk1hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK2_tap[0]", GetControlContent(mk2tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_tap[1]", GetControlContent(mk2tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_tap[2]", GetControlContent(mk2tap_3.Text), 2)
                            + ConstuctCodeLine("MK2_taptext", GetControlContent(mk2tap_4.Text), 1)
                            + ConstuctCodeLine("MK2_hold[0]", GetControlContent(mk2hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_hold[1]", GetControlContent(mk2hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_hold[2]", GetControlContent(mk2hold_3.Text), 2)
                            + ConstuctCodeLine("MK2_holdtext", GetControlContent(mk2hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK3_tap[0]", GetControlContent(mk3tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_tap[1]", GetControlContent(mk3tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_tap[2]", GetControlContent(mk3tap_3.Text), 2)
                            + ConstuctCodeLine("MK3_taptext", GetControlContent(mk3tap_4.Text), 1)
                            + ConstuctCodeLine("MK3_hold[0]", GetControlContent(mk3hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_hold[1]", GetControlContent(mk3hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_hold[2]", GetControlContent(mk3hold_3.Text), 2)
                            + ConstuctCodeLine("MK3_holdtext", GetControlContent(mk3hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK4_tap[0]", GetControlContent(mk4tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_tap[1]", GetControlContent(mk4tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_tap[2]", GetControlContent(mk4tap_3.Text), 2)
                            + ConstuctCodeLine("MK4_taptext", GetControlContent(mk4tap_4.Text), 1)
                            + ConstuctCodeLine("MK4_hold[0]", GetControlContent(mk4hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_hold[1]", GetControlContent(mk4hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_hold[2]", GetControlContent(mk4hold_3.Text), 2)
                            + ConstuctCodeLine("MK4_holdtext", GetControlContent(mk4hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK5_tap[0]", GetControlContent(mk5tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_tap[1]", GetControlContent(mk5tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_tap[2]", GetControlContent(mk5tap_3.Text), 2)
                            + ConstuctCodeLine("MK5_taptext", GetControlContent(mk5tap_4.Text), 1)
                            + ConstuctCodeLine("MK5_hold[0]", GetControlContent(mk5hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_hold[1]", GetControlContent(mk5hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_hold[2]", GetControlContent(mk5hold_3.Text), 2)
                            + ConstuctCodeLine("MK5_holdtext", GetControlContent(mk5hold_4.Text), 1)
                            + "\n"
                            + "}" + "\n"
                            );
                        // Add config settings to the configfile.
                        fs.Write(info, 0, info.Length);
                        MessageBox.Show("Configuration Created");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                Rewrite_ConfigLoader();
            }
            else
            {
                MessageBox.Show("Enter A Valid Configname \n (No Special Characters Allowed)");
            }
            PopulateConfigList(); //refresh existing configlist
        }

        private void Rewrite_ConfigLoader()
        {
            int cindex = 0;
            string cname = "";
            // Put all config files in root directory into array.
            string[] configfiles = Directory.GetFiles(filepath, "*_.ino"); // <-- Case-insensitive
            string[] configorder = { "", "" };
            Array.Resize<string>(ref configorder, configfiles.Length);

            int cnt = 0;
            foreach (string s in configfiles)
            {
                string[] configsettings = System.IO.File.ReadAllLines(configfiles[cnt]);

                string configline = configsettings[2];
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
                configline = configsettings[3];
                substringindex = configline.LastIndexOf("=");
                if (substringindex > 0)
                {
                    string sub = configline.Substring(substringindex);
                    sub = sub.Replace("=", "");
                    sub = sub.Replace(";", "");
                    sub = sub.Replace("\"", "");
                    sub = sub.Replace("'", "");
                    sub = sub.Replace("CRGB::", "");

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
                    MessageBox.Show("Invalid ID among configurations");
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

                using (FileStream fs = File.Create(filepath + @"load_configurations.ino"))
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
                MessageBox.Show(ex.ToString());
            }

        }

        private void MacroKey_ConfigMode(object sender, TextChangedEventArgs e)
        {
            //set Macro keys function based on input
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
            if (control == null || control.ToString() == "" || control.ToString() == ("null"))
            {
                newname = "' '";
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

        private bool ValidateConfigName(string value)
        {
            //checks for special characters in configname
            return !(Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$"));
        }

        private void ValidateCharacter(object sender, TextCompositionEventArgs e)
        {
            string textkey = e.Source.ToString().Replace("System.Windows.Controls.TextBox: ", "");

            if (textkey.Length > 0 && textkey != "System.Windows.Controls.TextBox")
            {
                MessageBox.Show("Enter A Single Character");
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
                    codeline = "appconfig[index]." + control + "=" + quotation + content + quotation + ";" + "\n";
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
            }
            return codeline;

        }

        private void Set_ConfigID(object sender, TextChangedEventArgs e)
        {
            int cindex = 0;
            // Put all config files in root directory into array.
            string[] configfiles = Directory.GetFiles(filepath, "*_.ino"); // <-- Case-insensitive
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
            //configname.Text = configsettings[currentline - 1];
            //configid.Text = configsettings[currentline++];
            appledcolor.SelectedValue = null;

            knob1CW_1.SelectedValue = null;
            knob1CW_2.SelectedValue = null;
            knob1CW_3.Text = null;
            knob1CCW_1.SelectedValue = null;
            knob1CCW_2.SelectedValue = null;
            knob1CCW_3.Text = null;

            singletap_1.SelectedValue = null;
            singletap_2.SelectedValue = null;
            singletap_3.Text = null;
            doubletap_1.SelectedValue = null;
            doubletap_2.SelectedValue = null;
            doubletap_3.Text = null;
            shortpress_1.SelectedValue = null;
            shortpress_2.SelectedValue = null;
            shortpress_3.Text = null;
            longpress_1.SelectedValue = null;
            longpress_2.SelectedValue = null;
            longpress_3.Text = null;

            mk1ledcolor.SelectedValue = null;
            mk2ledcolor.SelectedValue = null;
            mk3ledcolor.SelectedValue = null;
            mk4ledcolor.SelectedValue = null;
            mk5ledcolor.SelectedValue = null;

            mk1tap_1.SelectedValue = null;
            mk1tap_2.SelectedValue = null;
            mk1tap_3.Text = null;
            mk1tap_4.Text = null;
            mk1hold_1.SelectedValue = null;
            mk1hold_2.SelectedValue = null;
            mk1hold_3.Text = null;
            mk1hold_4.Text = null;

            mk2tap_1.SelectedValue = null;
            mk2tap_2.SelectedValue = null;
            mk2tap_3.Text = null;
            mk2tap_4.Text = null;
            mk2hold_1.SelectedValue = null;
            mk2hold_2.SelectedValue = null;
            mk2hold_3.Text = null;
            mk2hold_4.Text = null;

            mk3tap_1.SelectedValue = null;
            mk3tap_2.SelectedValue = null;
            mk3tap_3.Text = null;
            mk3tap_4.Text = null;
            mk3hold_1.SelectedValue = null;
            mk3hold_2.SelectedValue = null;
            mk3hold_3.Text = null;
            mk3hold_4.Text = null;

            mk4tap_1.SelectedValue = null;
            mk4tap_2.SelectedValue = null;
            mk4tap_3.Text = null;
            mk4tap_4.Text = null;
            mk4hold_1.SelectedValue = null;
            mk4hold_2.SelectedValue = null;
            mk4hold_3.Text = null;
            mk4hold_4.Text = null;

            mk5tap_1.SelectedValue = null;
            mk5tap_2.SelectedValue = null;
            mk5tap_3.Text = null;
            mk5tap_4.Text = null;
            mk5hold_1.SelectedValue = null;
            mk5hold_2.SelectedValue = null;
            mk5hold_3.Text = null;
            mk5hold_4.Text = null;

        }

        private void DeleteConfig(object sender, RoutedEventArgs e)
        {
            if (configlist.SelectedValue != null)
            {
                File.Delete(((ComboBoxItem)this.configlist.SelectedItem).Tag.ToString());
                MessageBox.Show(configname.Text + " Configuration Deleted");
                PopulateConfigList();
                configlist.Items.Remove(((ComboBoxItem)this.configlist.SelectedItem));
                for (int i = 0; i < configlist.Items.Count; i++)
                {
                    configlist.SelectedIndex = i;
                    ReOrderConfigID(i + 1);
                }
                Rewrite_ConfigLoader();

            }
            else
            {
                MessageBox.Show("This Configuration File Does Not Exist");
            }

        }

        private void ReOrderConfigID(int newid)
        {
            if (ValidateConfigName(configname.Text.ToString()) == false)
            {  //validate config name for special characters
                string filename = configname.Text.ToString().Replace(" ", ""); //remove space from name
                configname.Text = filename;
                string configpath = filepath + filename + "_.ino"; //generate config's absolute path

                try
                {
                    // Create the file, or overwrite if the file exists.
                    using (FileStream fs = File.Create(configpath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("void " + GetControlContent(configname.Text) + "() {" + "\n"
                            + "int index=" + (newid - 1).ToString() + ";" + "\n"
                            + ConstuctCodeLine("appname", GetControlContent(configname.Text), 1)
                            + ConstuctCodeLine("ID", newid.ToString(), 0)
                            + ConstuctCodeLine("appcolor", "CRGB::" + GetControlContent_Led(appledcolor.SelectedItem), 0)
                            + "\n"
                            + ConstuctCodeLine("knob1_CW[0]", GetControlContent(knob1CW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CW[1]", GetControlContent(knob1CW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CW[2]", GetControlContent(knob1CW_3.Text), 2)
                            + ConstuctCodeLine("knob1_CCW[0]", GetControlContent(knob1CCW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CCW[1]", GetControlContent(knob1CCW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob1_CCW[2]", GetControlContent(knob1CCW_3.Text), 2)
                            + "\n"
                            + ConstuctCodeLine("singletap[0]", GetControlContent(singletap_1.SelectedItem), 0)
                            + ConstuctCodeLine("singletap[1]", GetControlContent(singletap_2.SelectedItem), 0)
                            + ConstuctCodeLine("singletap[2]", GetControlContent(singletap_3.Text), 2)
                            + ConstuctCodeLine("doubletap[0]", GetControlContent(doubletap_1.SelectedItem), 0)
                            + ConstuctCodeLine("doubletap[1]", GetControlContent(doubletap_2.SelectedItem), 0)
                            + ConstuctCodeLine("doubletap[2]", GetControlContent(doubletap_3.Text), 2)
                            + ConstuctCodeLine("shortpress[0]", GetControlContent(shortpress_1.SelectedItem), 0)
                            + ConstuctCodeLine("shortpress[1]", GetControlContent(shortpress_2.SelectedItem), 0)
                            + ConstuctCodeLine("shortpress[2]", GetControlContent(shortpress_3.Text), 2)
                            + ConstuctCodeLine("longpress[0]", GetControlContent(longpress_1.SelectedItem), 0)
                            + ConstuctCodeLine("longpress[1]", GetControlContent(longpress_2.SelectedItem), 0)
                            + ConstuctCodeLine("longpress[2]", GetControlContent(longpress_3.Text), 2)
                            + "\n"
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
                            + ConstuctCodeLine("MK_colors[0]", "CRGB::" + GetControlContent_Led(mk1ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[1]", "CRGB::" + GetControlContent_Led(mk2ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[2]", "CRGB::" + GetControlContent_Led(mk3ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[3]", "CRGB::" + GetControlContent_Led(mk4ledcolor.SelectedItem), 0)
                            + ConstuctCodeLine("MK_colors[4]", "CRGB::" + GetControlContent_Led(mk5ledcolor.SelectedItem), 0)
                            + "\n"
                            + ConstuctCodeLine("MK1_tap[0]", GetControlContent(mk1tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_tap[1]", GetControlContent(mk1tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_tap[2]", GetControlContent(mk1tap_3.Text), 2)
                            + ConstuctCodeLine("MK1_taptext", GetControlContent(mk1tap_4.Text), 1)
                            + ConstuctCodeLine("MK1_hold[0]", GetControlContent(mk1hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_hold[1]", GetControlContent(mk1hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK1_hold[2]", GetControlContent(mk1hold_3.Text), 2)
                            + ConstuctCodeLine("MK1_holdtext", GetControlContent(mk1hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK2_tap[0]", GetControlContent(mk2tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_tap[1]", GetControlContent(mk2tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_tap[2]", GetControlContent(mk2tap_3.Text), 2)
                            + ConstuctCodeLine("MK2_taptext", GetControlContent(mk2tap_4.Text), 1)
                            + ConstuctCodeLine("MK2_hold[0]", GetControlContent(mk2hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_hold[1]", GetControlContent(mk2hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK2_hold[2]", GetControlContent(mk2hold_3.Text), 2)
                            + ConstuctCodeLine("MK2_holdtext", GetControlContent(mk2hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK3_tap[0]", GetControlContent(mk3tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_tap[1]", GetControlContent(mk3tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_tap[2]", GetControlContent(mk3tap_3.Text), 2)
                            + ConstuctCodeLine("MK3_taptext", GetControlContent(mk3tap_4.Text), 1)
                            + ConstuctCodeLine("MK3_hold[0]", GetControlContent(mk3hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_hold[1]", GetControlContent(mk3hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK3_hold[2]", GetControlContent(mk3hold_3.Text), 2)
                            + ConstuctCodeLine("MK3_holdtext", GetControlContent(mk3hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK4_tap[0]", GetControlContent(mk4tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_tap[1]", GetControlContent(mk4tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_tap[2]", GetControlContent(mk4tap_3.Text), 2)
                            + ConstuctCodeLine("MK4_taptext", GetControlContent(mk4tap_4.Text), 1)
                            + ConstuctCodeLine("MK4_hold[0]", GetControlContent(mk4hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_hold[1]", GetControlContent(mk4hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK4_hold[2]", GetControlContent(mk4hold_3.Text), 2)
                            + ConstuctCodeLine("MK4_holdtext", GetControlContent(mk4hold_4.Text), 1)
                            + "\n"
                            + ConstuctCodeLine("MK5_tap[0]", GetControlContent(mk5tap_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_tap[1]", GetControlContent(mk5tap_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_tap[2]", GetControlContent(mk5tap_3.Text), 2)
                            + ConstuctCodeLine("MK5_taptext", GetControlContent(mk5tap_4.Text), 1)
                            + ConstuctCodeLine("MK5_hold[0]", GetControlContent(mk5hold_1.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_hold[1]", GetControlContent(mk5hold_2.SelectedItem), 0)
                            + ConstuctCodeLine("MK5_hold[2]", GetControlContent(mk5hold_3.Text), 2)
                            + ConstuctCodeLine("MK5_holdtext", GetControlContent(mk5hold_4.Text), 1)
                            + "\n"
                            + "}" + "\n"
                            );
                        // Add config settings to the configfile.
                        fs.Write(info, 0, info.Length);
                        //MessageBox.Show("Configuration Created");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }


        //*****************************************************Dial Image Changers*********************************************************//
        private void Appledcolor_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[0]));
        }
        private void Knob1_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[1]));
        }

  
        private void Capacitivetouch_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[3]));
        }

        private void Mk1_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[4]));
        }

        private void Mk2_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[5]));
        }

        private void Mk3_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[6]));
        }

        private void Mk4_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[7]));
        }

        private void Mk5_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[8]));
        }

     
        private void Settings_LostFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[10]));
        }
        //************************************************************************************************************************************//

        private void LoadSettingsFromConfigFile()
        {
            //translate setting in config file to UI setting.
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
                    sub = sub.Replace("=", "");
                    sub = sub.Replace(";", "");
                    sub = sub.Replace("\"", "");
                    sub = sub.Replace("'", "");
                    sub = sub.Replace("CRGB::", "");
                    configsettings[i] = sub;
                }
            }

            int currentline = lines[0];
            configname.Text = configsettings[currentline - 1];
            configid.Text = configsettings[currentline++];
            appledcolor.SelectedValue = configsettings[currentline++];
            currentline++;
            knob1CW_1.SelectedValue = configsettings[currentline++];
            knob1CW_2.SelectedValue = configsettings[currentline++];
            knob1CW_3.Text = configsettings[currentline++];
            knob1CCW_1.SelectedValue = configsettings[currentline++];
            knob1CCW_2.SelectedValue = configsettings[currentline++];
            knob1CCW_3.Text = configsettings[currentline++];
            currentline++;
            singletap_1.SelectedValue = configsettings[currentline++];
            singletap_2.SelectedValue = configsettings[currentline++];
            singletap_3.Text = configsettings[currentline++];
            doubletap_1.SelectedValue = configsettings[currentline++];
            doubletap_2.SelectedValue = configsettings[currentline++];
            doubletap_3.Text = configsettings[currentline++];
            shortpress_1.SelectedValue = configsettings[currentline++];
            shortpress_2.SelectedValue = configsettings[currentline++];
            shortpress_3.Text = configsettings[currentline++];
            longpress_1.SelectedValue = configsettings[currentline++];
            longpress_2.SelectedValue = configsettings[currentline++];
            longpress_3.Text = configsettings[currentline++];
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
            currentline++;
            mk1ledcolor.SelectedValue = configsettings[currentline++];
            mk2ledcolor.SelectedValue = configsettings[currentline++];
            mk3ledcolor.SelectedValue = configsettings[currentline++];
            mk4ledcolor.SelectedValue = configsettings[currentline++];
            mk5ledcolor.SelectedValue = configsettings[currentline++];
            currentline++;
            mk1tap_1.SelectedValue = configsettings[currentline++];
            mk1tap_2.SelectedValue = configsettings[currentline++];
            mk1tap_3.Text = configsettings[currentline++];
            mk1tap_4.Text = configsettings[currentline++];
            mk1hold_1.SelectedValue = configsettings[currentline++];
            mk1hold_2.SelectedValue = configsettings[currentline++];
            mk1hold_3.Text = configsettings[currentline++];
            mk1hold_4.Text = configsettings[currentline++];
            currentline++;
            mk2tap_1.SelectedValue = configsettings[currentline++];
            mk2tap_2.SelectedValue = configsettings[currentline++];
            mk2tap_3.Text = configsettings[currentline++];
            mk2tap_4.Text = configsettings[currentline++];
            mk2hold_1.SelectedValue = configsettings[currentline++];
            mk2hold_2.SelectedValue = configsettings[currentline++];
            mk2hold_3.Text = configsettings[currentline++];
            mk2hold_4.Text = configsettings[currentline++];
            currentline++;
            mk3tap_1.SelectedValue = configsettings[currentline++];
            mk3tap_2.SelectedValue = configsettings[currentline++];
            mk3tap_3.Text = configsettings[currentline++];
            mk3tap_4.Text = configsettings[currentline++];
            mk3hold_1.SelectedValue = configsettings[currentline++];
            mk3hold_2.SelectedValue = configsettings[currentline++];
            mk3hold_3.Text = configsettings[currentline++];
            mk3hold_4.Text = configsettings[currentline++];
            currentline++;
            mk4tap_1.SelectedValue = configsettings[currentline++];
            mk4tap_2.SelectedValue = configsettings[currentline++];
            mk4tap_3.Text = configsettings[currentline++];
            mk4tap_4.Text = configsettings[currentline++];
            mk4hold_1.SelectedValue = configsettings[currentline++];
            mk4hold_2.SelectedValue = configsettings[currentline++];
            mk4hold_3.Text = configsettings[currentline++];
            mk4hold_4.Text = configsettings[currentline++];
            currentline++;
            mk5tap_1.SelectedValue = configsettings[currentline++];
            mk5tap_2.SelectedValue = configsettings[currentline++];
            mk5tap_3.Text = configsettings[currentline++];
            mk5tap_4.Text = configsettings[currentline++];
            mk5hold_1.SelectedValue = configsettings[currentline++];
            mk5hold_2.SelectedValue = configsettings[currentline++];
            mk5hold_3.Text = configsettings[currentline++];
            mk5hold_4.Text = configsettings[currentline++];
           
        }
    }
}
