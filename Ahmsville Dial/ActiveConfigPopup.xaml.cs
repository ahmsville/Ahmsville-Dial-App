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
using System.Windows.Shapes;

namespace Ahmsville_Dial
{
    /// <summary>
    /// Interaction logic for ActiveConfigPopup.xaml
    /// </summary>
    public partial class ActiveConfigPopup : Window
    {
        static Action defaultaction;
        static Action manualSW;
        static Action autoSW;
        public static bool lockstate = false;
        public ActiveConfigPopup()
        {
            InitializeComponent();
        }
        public static void setActions(Action def, Action manual, Action auto)
        {
            defaultaction = def;
            manualSW = manual;
            autoSW = auto;
        }

        private void popupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.popupWindowOpened = true;
            defaultaction();
            manualSW();
        }
        private void popupWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.popupWindowOpened = false;
            if (lockstate)
            {
                manualSW();
            }
            else
            {
                autoSW();
            }
            defaultaction();
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            switchlock();
            
        }
        public static void switchlock()
        {
            lockstate = !lockstate;
            if (lockstate)
            {
                AhmsvilleDialViewModel.Instance.geometrydata = "M18 8h-1V6c0-2.76-2.24-5-5-5S7 3.24 7 6v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm-6 9c-1.1 0-2-.9-2-2s.9-2 2-2 2 .9 2 2-.9 2-2 2zM9 8V6c0-1.66 1.34-3 3-3s3 1.34 3 3v2H9z";
            }
            else
            {
                AhmsvilleDialViewModel.Instance.geometrydata = "M12 13c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm6-5h-1V6c0-2.76-2.24-5-5-5-2.28 0-4.27 1.54-4.84 3.75-.14.54.18 1.08.72 1.22.53.14 1.08-.18 1.22-.72C9.44 3.93 10.63 3 12 3c1.65 0 3 1.35 3 3v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm0 11c0 .55-.45 1-1 1H7c-.55 0-1-.45-1-1v-8c0-.55.45-1 1-1h10c.55 0 1 .45 1 1v8z";
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
