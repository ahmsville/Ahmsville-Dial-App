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
    public partial class Dial_SpaceNav : Page
    {
        double faded = 0.5, opaque = 1;
        string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\Ahmsville_dial_v2\examples\Ahmsville_Dial_SpaceNav_version\";  //path for Dial's config files and library files 
        static string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;  //path for software related files
        int configcount;

        //dial imagepath array
        string[] dialimagepaths = {softwarepath + @"\spacenav\led.png", softwarepath + @"\spacenav\knob1.png", softwarepath + @"\spacenav\knob2.png", softwarepath + @"\spacenav\captouch.png", softwarepath + @"\spacenav\mk1.png",
            softwarepath + @"\spacenav\mk2.png", softwarepath + @"\spacenav\mk3.png", softwarepath + @"\spacenav\mk4.png", softwarepath + @"\spacenav\mk5.png", softwarepath + @"\spacenav\spacenav.png", softwarepath + @"\spacenav\default.png"};

        public Dial_SpaceNav()
        {
            InitializeComponent();
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[10]));
            DataContext = new AhmsvilleDialViewModel();
            spacenav_manual.IsChecked = true;   //select default space navigator mode
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
                            + ConstuctCodeLine("knob2_CW[0]", GetControlContent(knob2CW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CW[1]", GetControlContent(knob2CW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CW[2]", GetControlContent(knob2CW_3.Text), 2)
                            + ConstuctCodeLine("knob2_CCW[0]", GetControlContent(knob2CCW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CCW[1]", GetControlContent(knob2CCW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CCW[2]", GetControlContent(knob2CCW_3.Text), 2)
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
                            + ConstuctCodeLine("spacenav_func", SpaceNav_Mode().ToString(), 0)
                            + ConstuctCodeLine("spacenav_tiltup[0]", GetControlContent(tiltup_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltup[1]", GetControlContent(tiltup_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltup[2]", GetControlContent(tiltup_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltdown[0]", GetControlContent(tiltdown_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltdown[1]", GetControlContent(tiltdown_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltdown[2]", GetControlContent(tiltdown_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltright[0]", GetControlContent(tiltright_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltright[1]", GetControlContent(tiltright_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltright[2]", GetControlContent(tiltright_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltleft[0]", GetControlContent(tiltleft_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltleft[1]", GetControlContent(tiltleft_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltleft[2]", GetControlContent(tiltleft_3.Text), 2)
                            + "\n"
                            + ConstuctCodeLine("spacenav_slideup[0]", GetControlContent(slideup_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideup[1]", GetControlContent(slideup_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideup[2]", GetControlContent(slideup_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slidedown[0]", GetControlContent(slidedown_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slidedown[1]", GetControlContent(slidedown_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slidedown[2]", GetControlContent(slidedown_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slideright[0]", GetControlContent(slideright_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideright[1]", GetControlContent(slideright_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideright[2]", GetControlContent(slideright_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slideleft[0]", GetControlContent(slideleft_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideleft[1]", GetControlContent(slideleft_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideleft[2]", GetControlContent(slideleft_3.Text), 2)
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

            knob2CW_1.SelectedValue = null;
            knob2CW_2.SelectedValue = null;
            knob2CW_3.Text = null;
            knob2CCW_1.SelectedValue = null;
            knob2CCW_2.SelectedValue = null;
            knob2CCW_3.Text = null;

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

            spacenav_manual.IsChecked = true;
            tiltup_1.SelectedValue = null;
            tiltup_2.SelectedValue = null;
            tiltup_3.Text = null;
            tiltdown_1.SelectedValue = null;
            tiltdown_2.SelectedValue = null;
            tiltdown_3.Text = null;
            tiltright_1.SelectedValue = null;
            tiltright_2.SelectedValue = null;
            tiltright_3.Text = null;
            tiltleft_1.SelectedValue = null;
            tiltleft_2.SelectedValue = null;
            tiltleft_3.Text = null;

            slideup_1.SelectedValue = null;
            slideup_2.SelectedValue = null;
            slideup_3.Text = null;
            slidedown_1.SelectedValue = null;
            slidedown_2.SelectedValue = null;
            slidedown_3.Text = null;
            slideright_1.SelectedValue = null;
            slideright_2.SelectedValue = null;
            slideright_3.Text = null;
            slideleft_1.SelectedValue = null;
            slideleft_2.SelectedValue = null;
            slideleft_3.Text = null;
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
                            + ConstuctCodeLine("knob2_CW[0]", GetControlContent(knob2CW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CW[1]", GetControlContent(knob2CW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CW[2]", GetControlContent(knob2CW_3.Text), 2)
                            + ConstuctCodeLine("knob2_CCW[0]", GetControlContent(knob2CCW_1.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CCW[1]", GetControlContent(knob2CCW_2.SelectedItem), 0)
                            + ConstuctCodeLine("knob2_CCW[2]", GetControlContent(knob2CCW_3.Text), 2)
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
                            + ConstuctCodeLine("spacenav_func", SpaceNav_Mode().ToString(), 0)
                            + ConstuctCodeLine("spacenav_tiltup[0]", GetControlContent(tiltup_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltup[1]", GetControlContent(tiltup_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltup[2]", GetControlContent(tiltup_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltdown[0]", GetControlContent(tiltdown_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltdown[1]", GetControlContent(tiltdown_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltdown[2]", GetControlContent(tiltdown_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltright[0]", GetControlContent(tiltright_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltright[1]", GetControlContent(tiltright_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltright[2]", GetControlContent(tiltright_3.Text), 2)
                            + ConstuctCodeLine("spacenav_tiltleft[0]", GetControlContent(tiltleft_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltleft[1]", GetControlContent(tiltleft_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_tiltleft[2]", GetControlContent(tiltleft_3.Text), 2)
                            + "\n"
                            + ConstuctCodeLine("spacenav_slideup[0]", GetControlContent(slideup_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideup[1]", GetControlContent(slideup_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideup[2]", GetControlContent(slideup_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slidedown[0]", GetControlContent(slidedown_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slidedown[1]", GetControlContent(slidedown_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slidedown[2]", GetControlContent(slidedown_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slideright[0]", GetControlContent(slideright_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideright[1]", GetControlContent(slideright_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideright[2]", GetControlContent(slideright_3.Text), 2)
                            + ConstuctCodeLine("spacenav_slideleft[0]", GetControlContent(slideleft_1.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideleft[1]", GetControlContent(slideleft_2.SelectedItem), 0)
                            + ConstuctCodeLine("spacenav_slideleft[2]", GetControlContent(slideleft_3.Text), 2)
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
        private int SpaceNav_Mode()
        {
            //sets space nav mode identifiers
            int mode = 0;
            if (spacenav_manual.IsChecked == true)
            {
                mode = 0;
            }
            else if (spacenav_1.IsChecked == true)
            {
                mode = 1;
            }
            else if (spacenav_2.IsChecked == true)
            {
                mode = 2;
            }
            else if (spacenav_3.IsChecked == true)
            {
                mode = 3;
            }
            else if (spacenav_4.IsChecked == true)
            {
                mode = 4;
            }
            else if (spacenav_5.IsChecked == true)
            {
                mode = 5;
            }
            return mode;
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

        private void Knob2_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[2]));
        }

        private void Capacitivetouch_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[3]));
        }

        private void Spacenav_GotFocus(object sender, RoutedEventArgs e)
        {
            dialimage.Source = new BitmapImage(new Uri(dialimagepaths[9]));
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
            knob2CW_1.SelectedValue = configsettings[currentline++];
            knob2CW_2.SelectedValue = configsettings[currentline++];
            knob2CW_3.Text = configsettings[currentline++];
            knob2CCW_1.SelectedValue = configsettings[currentline++];
            knob2CCW_2.SelectedValue = configsettings[currentline++];
            knob2CCW_3.Text = configsettings[currentline++];
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
            int templine = currentline++;
            if (configsettings[templine] == "0")
            {
                spacenav_manual.IsChecked = true;
            }
            else if (configsettings[templine] == "1")
            {
                spacenav_1.IsChecked = true;
            }
            else if (configsettings[templine] == "2")
            {
                spacenav_2.IsChecked = true;
            }
            else if (configsettings[templine] == "3")
            {
                spacenav_3.IsChecked = true;
            }
            else if (configsettings[templine] == "4")
            {
                spacenav_4.IsChecked = true;
            }
            else if (configsettings[templine] == "5")
            {
                spacenav_5.IsChecked = true;
            }
            tiltup_1.SelectedValue = configsettings[currentline++];
            tiltup_2.SelectedValue = configsettings[currentline++];
            tiltup_3.Text = configsettings[currentline++];
            tiltdown_1.SelectedValue = configsettings[currentline++];
            tiltdown_2.SelectedValue = configsettings[currentline++];
            tiltdown_3.Text = configsettings[currentline++];
            tiltright_1.SelectedValue = configsettings[currentline++];
            tiltright_2.SelectedValue = configsettings[currentline++];
            tiltright_3.Text = configsettings[currentline++];
            tiltleft_1.SelectedValue = configsettings[currentline++];
            tiltleft_2.SelectedValue = configsettings[currentline++];
            tiltleft_3.Text = configsettings[currentline++];
            currentline++;
            slideup_1.SelectedValue = configsettings[currentline++];
            slideup_2.SelectedValue = configsettings[currentline++];
            slideup_3.Text = configsettings[currentline++];
            slidedown_1.SelectedValue = configsettings[currentline++];
            slidedown_2.SelectedValue = configsettings[currentline++];
            slidedown_3.Text = configsettings[currentline++];
            slideright_1.SelectedValue = configsettings[currentline++];
            slideright_2.SelectedValue = configsettings[currentline++];
            slideright_3.Text = configsettings[currentline++];
            slideleft_1.SelectedValue = configsettings[currentline++];
            slideleft_2.SelectedValue = configsettings[currentline++];
            slideleft_3.Text = configsettings[currentline++];
        }
    }
}
