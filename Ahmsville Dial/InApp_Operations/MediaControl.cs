using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ahmsville_Dial.InApp_Operations
{
    class MediaControl : MediaControl_INTERFACE
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

        public void volume_up()
        {
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public void volume_down()
        {
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public void volume_mute()
        {
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public void media_PlayPause()
        {
            keybd_event(VK_MEDIA_PLAY_PAUSE, MapVirtualKey(VK_MEDIA_PLAY_PAUSE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_PLAY_PAUSE, MapVirtualKey(VK_MEDIA_PLAY_PAUSE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public void media_next()
        {
            keybd_event(VK_MEDIA_NEXT_TRACK, MapVirtualKey(VK_MEDIA_NEXT_TRACK, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_NEXT_TRACK, MapVirtualKey(VK_MEDIA_NEXT_TRACK, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        public void media_prev()
        {
            keybd_event(VK_MEDIA_PREV_TRACK, MapVirtualKey(VK_MEDIA_PREV_TRACK, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_PREV_TRACK, MapVirtualKey(VK_MEDIA_PREV_TRACK, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        public void media_stop()
        {
            keybd_event(VK_MEDIA_STOP, MapVirtualKey(VK_MEDIA_STOP, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_MEDIA_STOP, MapVirtualKey(VK_MEDIA_STOP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        public string mediacontrol(string operationclass, string operationname)
        {
            _MethodInfo _Method = this.GetType().GetMethod(operationname);
            _Method.Invoke(this, null);
            return operationclass +"  yes  "+ operationname;
        }
    }
}
