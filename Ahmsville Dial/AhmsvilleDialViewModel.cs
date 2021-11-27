using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ahmsville_Dial
{
    public class AhmsvilleDialViewModel : INotifyPropertyChanged
    {
        public List<string> specialkeys { get; set; }
        public List<string> ledcolors { get; set; }

        public List<string> ledanimations { get; set; }

        static string softwarepath = System.AppDomain.CurrentDomain.BaseDirectory;  //path for software related files

        public class Dial_information
        {
            public string name { get; set; }
            public string filepath { get; set; }
            public string imagepath { get; set; }
        }

        public List<Dial_information> dialinfo { get; set; }

        public class inapp
        {
            public string operationname { get; set; }
            public string operationclass { get; set; }
        }

        public static string[] availableconfigs = { };
       
        public struct availableconfigsforapp
        {
            public string[] linesforappfunctions;
            public List<string> comments;
        }
        public static availableconfigsforapp[] loadedappfunctions = new availableconfigsforapp[0];
        
        private static int _active_loaded_inappfunctions_index = 0;
        public static int active_loaded_inappfunctions_index
        {
            get { return _active_loaded_inappfunctions_index; }

            set
            {
                _active_loaded_inappfunctions_index = value;
                instance.OnPropertyChanged();
                instance.changePopup();    
            }
        }
        private void changePopup()
        {
            popupConfigID = active_loaded_inappfunctions_index.ToString();
            popupConfigColor = loadedappfunctions[active_loaded_inappfunctions_index].comments.ElementAt(0);
            popupConfigName = availableconfigs[active_loaded_inappfunctions_index];
            popupConfigComment = "";
            for (int i = 1; i < loadedappfunctions[active_loaded_inappfunctions_index].comments.Count; i++)
            {
                popupConfigComment += loadedappfunctions[active_loaded_inappfunctions_index].comments.ElementAt(i) + "\n";
            }
        }

       private string _geometrydata = "M12 13c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm6-5h-1V6c0-2.76-2.24-5-5-5-2.28 0-4.27 1.54-4.84 3.75-.14.54.18 1.08.72 1.22.53.14 1.08-.18 1.22-.72C9.44 3.93 10.63 3 12 3c1.65 0 3 1.35 3 3v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm0 11c0 .55-.45 1-1 1H7c-.55 0-1-.45-1-1v-8c0-.55.45-1 1-1h10c.55 0 1 .45 1 1v8z";
        public string geometrydata
        {
            get { return _geometrydata; }

            set
            {
                _geometrydata = value;
                OnPropertyChanged();
            }
        }
        public List<string> inappOperationClasses { get; set; }


        public List<inapp> inappoperations { get; set; }



        public List<inapp> GetInapps()
        {
            return inappoperations;
        }

        private string _popupConfigID;
        public string popupConfigID
        {
            get { return _popupConfigID; }

            set
            {
                _popupConfigID = value;
                OnPropertyChanged();
            }
        }

        private string _popupConfigName;
        public string popupConfigName
        {
            get { return _popupConfigName; }

            set
            {
                _popupConfigName = value;
                OnPropertyChanged();
            }
        }
        private string _popupConfigColor;
        public string popupConfigColor
        {
            get { return _popupConfigColor; }

            set
            {
                _popupConfigColor = value;
                OnPropertyChanged();
            }
        }

        private string _popupConfigComment;
        public string popupConfigComment
        {
            get { return _popupConfigComment; }

            set
            {
                _popupConfigComment = value;
                OnPropertyChanged();
            }
        }

        private string _activeappname;
        public string activeappname
        {
            get { return _activeappname; }

            set
            {
                _activeappname = value;
                OnPropertyChanged();
            }
        }
        private string _activeappid;
        public string activeappid
        {
            get { return _activeappid; }

            set
            {
                _activeappid = value;
                OnPropertyChanged();
            }
        }

        private void conStateChanged()
        {
            if (constate == 1)
            {
                connectionstatecolor = "Lime";
                conbuttontext = "Disconnect";
            }
            else if(constate == 2)
            {
                connectionstatecolor = "Red";
                conbuttontext = "Connect";
            }
            else if (constate == 3)
            {
                connectionstatecolor = "Blue";
                conbuttontext = "Available";
            }
            else if (constate == 0)
            {
                connectionstatecolor = "Gold";
                conbuttontext = "Query";
            }
        }

        public int _constate = 0;
        public int constate
        {
            get { return _constate; }
            set { _constate = value; conStateChanged(); }
        }

        public string _conbuttontext = "Query";
        public string conbuttontext
        {
            get { return _conbuttontext; }
            set { _conbuttontext = value; AhmsvilleDialViewModel.Instance.OnPropertyChanged(); }
        }

        public string _connectionstate;
        public string connectionstate
        {
            get { return _connectionstate; }
            set { _connectionstate = value; AhmsvilleDialViewModel.Instance.OnPropertyChanged(); }
        }
        public string _connectionstatecolor;
        public string connectionstatecolor
        {
            get { return _connectionstatecolor; }
            set { _connectionstatecolor = value; AhmsvilleDialViewModel.Instance.OnPropertyChanged(); }
        }
        /*
        public class connectionVariables
        {
            public string _numberofdialsfound = "hhhhhhhhhhhhhh";
            public string numberofdialsfound
            {
                get { return _numberofdialsfound; }
                set { _numberofdialsfound = value; AhmsvilleDialViewModel.Instance.OnPropertyChanged(); }
            }
        }
        public connectionVariables connVariable;
       */


        private static readonly AhmsvilleDialViewModel instance = new AhmsvilleDialViewModel();

        public static AhmsvilleDialViewModel Instance { get { return instance; } }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
        public AhmsvilleDialViewModel()
        {
            specialkeys = new List<string>
            {
             "null","KEY_LEFT_CTRL","KEY_LEFT_SHIFT","KEY_LEFT_ALT","KEY_LEFT_GUI", "KEY_RIGHT_CTRL", "KEY_RIGHT_SHIFT", "KEY_RIGHT_ALT", "KEY_RIGHT_GUI"
            ,"KEY_UP_ARROW","KEY_DOWN_ARROW","KEY_RIGHT_ARROW","KEY_LEFT_ARROW","KEY_BACKSPACE","KEY_SPACEBAR","NUMPAD_MULTIPLY","KEY_TAB","KEY_RETURN","KEY_ESC","KEY_INSERT","KEY_DELETE","KEY_PAGE_UP","KEY_PAGE_DOWN"
            ,"KEY_HOME","KEY_END","KEY_CAPS_LOCK","KEY_F1","KEY_F2","KEY_F3","KEY_F4","KEY_F5","KEY_F6","KEY_F7","KEY_F8","KEY_F9","KEY_F10","KEY_F11","KEY_F12","KEY_F13","KEY_F14","KEY_F15","KEY_F16","KEY_F17","KEY_F18","KEY_F19","KEY_F20","KEY_F21","KEY_F22","KEY_F23","KEY_F24"
            ,"MOUSE_LEFT","MOUSE_RIGHT","MOUSE_MIDDLE","MOUSEMOVE_UP","MOUSEMOVE_DOWN","MOUSEMOVE_RIGHT","MOUSEMOVE_LEFT"
            };

            ledcolors = new List<string>
            {
                //"null","ALLColors","White","Lime","Ivory","Yellow","Gold","Goldenrod","Brown","Orange","Sienna","Coral","Red","Maroon","Pink","Magenta","Violet","Purple","Indigo","SkyBlue","Blue","Aqua","PaleGreen","Green","ForestGreen"
                //,"Gray","Black"

               "null","ALLColors","Black", "AliceBlue","Amethyst","AntiqueWhite","Aqua","Aquamarine","Azure","Beige","Bisque","BlanchedAlmond","Blue","BlueViolet","Brown","BurlyWood","CadetBlue","Chartreuse","Chocolate","Coral","CornflowerBlue","Cornsilk","Crimson","Cyan","DarkBlue","DarkCyan","DarkGoldenrod","DarkGray","DarkGrey","DarkGreen","DarkKhaki","DarkMagenta","DarkOliveGreen","DarkOrange","DarkOrchid","DarkRed","DarkSalmon","DarkSeaGreen","DarkSlateBlue","DarkSlateGray","DarkSlateGrey","DarkTurquoise","DarkViolet","DeepPink","DeepSkyBlue","DimGray","DimGrey","DodgerBlue","FireBrick","FloralWhite","ForestGreen","Fuchsia","Gainsboro","GhostWhite","Gold","Goldenrod","Gray","Grey","Green","GreenYellow","Honeydew","HotPink","IndianRed","Indigo","Ivory","Khaki","Lavender","LavenderBlush","LawnGreen","LemonChiffon","LightBlue","LightCoral","LightCyan","LightGoldenrodYellow","LightGreen","LightGrey","LightPink","LightSalmon","LightSeaGreen","LightSkyBlue","LightSlateGray","LightSlateGrey","LightSteelBlue","LightYellow","Lime","LimeGreen","Linen","Magenta","Maroon","MediumAquamarine","MediumBlue","MediumOrchid","MediumPurple","MediumSeaGreen","MediumSlateBlue","MediumSpringGreen","MediumTurquoise","MediumVioletRed","MidnightBlue","MintCream","MistyRose","Moccasin","NavajoWhite","Navy","OldLace","Olive","OliveDrab","Orange","OrangeRed","Orchid","PaleGoldenrod","PaleGreen","PaleTurquoise","PaleVioletRed","PapayaWhip","PeachPuff","Peru","Pink","Plaid","Plum","PowderBlue","Purple","Red","RosyBrown","RoyalBlue","SaddleBrown","Salmon","SandyBrown","SeaGreen","Seashell","Sienna","Silver","SkyBlue","SlateBlue","SlateGray","SlateGrey","Snow","SpringGreen","SteelBlue","Tan","Teal","Thistle","Tomato","Turquoise","Violet","Wheat","White","WhiteSmoke","Yellow","YellowGreen","FairyLight","FairyLightNCC"
            };

            ledanimations = new List<string>
            {
                "null","ColorWheel","Breath","Cardio","Rainbow","SpacklingRainbow","Confetti","Jungle"
            };

            inappoperations = new List<inapp>
            {
            new inapp {operationname = "Open_PopUp", operationclass = "PopUp"},
            //new inapp {operationname = "Close_PopUp", operationclass = "PopUp"},

            new inapp {operationname = "volume_up", operationclass = "MediaControl"},
            new inapp {operationname = "volume_down", operationclass = "MediaControl"},
            new inapp {operationname = "volume_mute", operationclass = "MediaControl"},
            new inapp {operationname = "media_PlayPause", operationclass = "MediaControl"},
            new inapp {operationname = "media_next", operationclass = "MediaControl"},
            new inapp {operationname = "media_prev", operationclass = "MediaControl"},
            new inapp {operationname = "media_stop", operationclass = "MediaControl"},
            
            new inapp {operationname = "SW_roll_xpos", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_roll_xneg", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_roll_ypos", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_roll_yneg", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_zoomIn", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_zoomOut", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_spinCW", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_spinCCW", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_move_xpos", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_move_xneg", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_move_ypos", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_move_yneg", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "_SW_auto_orientationLock", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "_SW_change_viewmode", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_zoomToFit", operationclass = "SOLIDWORKS"},
            new inapp {operationname = "SW_translate_zoom", operationclass = "SOLIDWORKS"},

            new inapp {operationname = "_FU360_auto_orientationLock", operationclass = "FUSION360"},
            new inapp {operationname = "_FU360_change_viewmode", operationclass = "FUSION360"},
            new inapp {operationname = "FU360_zoomToFit", operationclass = "FUSION360"},
            new inapp {operationname = "FU360_translate_zoom", operationclass = "FUSION360"},

            new inapp {operationname = "_BLENDER_auto_orientationLock", operationclass = "BLENDER"},
            new inapp {operationname = "_BLENDER_change_viewmode", operationclass = "BLENDER"},
            new inapp {operationname = "BLENDER_zoomToFit", operationclass = "BLENDER"},
            new inapp {operationname = "BLENDER_translate_zoom", operationclass = "BLENDER"},

            new inapp {operationname = "numpad_multiply", operationclass = "VirtualKeys"},
            new inapp {operationname = "F1", operationclass = "VirtualKeys"},
            new inapp {operationname = "F2", operationclass = "VirtualKeys"},
            new inapp {operationname = "F3", operationclass = "VirtualKeys"},
            new inapp {operationname = "F4", operationclass = "VirtualKeys"},
            new inapp {operationname = "F5", operationclass = "VirtualKeys"},
            new inapp {operationname = "F6", operationclass = "VirtualKeys"},
            new inapp {operationname = "F7", operationclass = "VirtualKeys"},
            new inapp {operationname = "F8", operationclass = "VirtualKeys"},
            new inapp {operationname = "F9", operationclass = "VirtualKeys"},
            new inapp {operationname = "F10", operationclass = "VirtualKeys"},
            new inapp {operationname = "F11", operationclass = "VirtualKeys"},
            new inapp {operationname = "F12", operationclass = "VirtualKeys"},
            new inapp {operationname = "F13", operationclass = "VirtualKeys"},
            new inapp {operationname = "F14", operationclass = "VirtualKeys"},
            new inapp {operationname = "F15", operationclass = "VirtualKeys"},
            new inapp {operationname = "F16", operationclass = "VirtualKeys"},
            new inapp {operationname = "F17", operationclass = "VirtualKeys"},
            new inapp {operationname = "F18", operationclass = "VirtualKeys"},
            new inapp {operationname = "F19", operationclass = "VirtualKeys"},
            new inapp {operationname = "F20", operationclass = "VirtualKeys"},
            new inapp {operationname = "F21", operationclass = "VirtualKeys"},
            new inapp {operationname = "F22", operationclass = "VirtualKeys"},
            new inapp {operationname = "F23", operationclass = "VirtualKeys"},
            new inapp {operationname = "F24", operationclass = "VirtualKeys"}
            };

            inappOperationClasses = new List<string>
            {

            };
            var inappoperation_classGroup = inappoperations.GroupBy(x => x.operationclass); //group inbuilt operation into appropriate class
            foreach (var opClass in inappoperation_classGroup)
            {

                inappOperationClasses.Add(opClass.Key);
            }

            dialinfo = new List<Dial_information>
            {
            new Dial_information {name = "Base Variant", filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Base_Variant_main\", imagepath = softwarepath + @"\base\default.jpg" },
            new Dial_information {name = "MacroKey Variant", filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\MacroKey_Variant_main\", imagepath = softwarepath + @"\macro\default.jpg" },
            new Dial_information {name = "SpaceNav Variant", filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\SpaceNav_Variant_main\", imagepath = softwarepath + @"\spacenav\default.jpg" },
            new Dial_information {name = "Absolute Variant", filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Arduino\libraries\AhmsvilleDial_v2\examples\Absolute_Variant_main\", imagepath = softwarepath + @"\absolute\default.jpg" }
            };

            popupConfigID = "0";
            popupConfigName = "No Active Config";
            popupConfigColor = "Purple";
            popupConfigComment = "Hello";
           
            
        }

        
    }

    
}
