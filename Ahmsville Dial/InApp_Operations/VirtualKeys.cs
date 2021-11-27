using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class VirtualKeys : VirtualKeys_INTERFACE
    {
        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_UP = 0xAF;
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        private const byte VK_MEDIA_PREV_TRACK = 0xB1;
        private const byte VK_MEDIA_STOP = 0xB2;
        private const UInt32 KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const UInt32 KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

        [DllImport("user32.dll")]
        static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        public void keyPress(byte VIRTUAL_KEY)
        {
            keybd_event(VIRTUAL_KEY, MapVirtualKey(VIRTUAL_KEY, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VIRTUAL_KEY, MapVirtualKey(VIRTUAL_KEY, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        public byte getByte(string opname)
        {
            switch (opname)
            {
                case "numpad_multiply":
                    return 0x6A;
                case "F1":
                    return 0x70;
                case "F2":
                    return 0x71;
                case "F3":
                    return 0x72;
                case "F4":
                    return 0x73;
                case "F5":
                    return 0x74;
                case "F6":
                    return 0x75;
                case "F7":
                    return 0x76;
                case "F8":
                    return 0x77;
                case "F9":
                    return 0x78;
                case "F10":
                    return 0x79;
                case "F11":
                    return 0x7A;
                case "F12":
                    return 0x7B;
                case "F13":
                    return 0x7C;
                case "F14":
                    return 0x7D;
                case "F15":
                    return 0x7E;
                case "F16":
                    return 0x7F;
                case "F17":
                    return 0x80;
                case "F18":
                    return 0x81;
                case "F19":
                    return 0x82;
                case "F20":
                    return 0x83;
                case "F21":
                    return 0x84;
                case "F22":
                    return 0x85;
                case "F23":
                    return 0x86;
                case "F24":
                    return 0x87;



                default:
                    break;
            }
            return 0;
        }
        public string virtualkeypress(string operationclass, string operationname)
        {
            keyPress(getByte(operationname));
            //_MethodInfo _Method = this.GetType().GetMethod(operationname);
            //_Method.Invoke(this, null);
            return operationclass + "  yes  " + operationname;
        }
    }
}
