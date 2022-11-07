using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.SysInfo
{
    public class Keyboard
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal int type;
            internal short wVk;
            internal short wScan;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
            int dummy1;
            int dummy2;
            internal int type1;
            internal short wVk1;
            internal short wScan1;
            internal int dwFlags1;
            internal int time1;
            internal IntPtr dwExtraInfo1;
            int dummy3;
            int dummy4;
        }
        [DllImport("user32.dll")]
        static extern int SendInput(uint nInputs, IntPtr pInputs, int cbSize);

        public static void SetNumlockOn()
        {
            const int mouseInpSize = 28;//Hardcoded size of the MOUSEINPUT tag !!!
            INPUT input = new INPUT();
            input.type = 0x01; //INPUT_KEYBOARD
            input.wVk = 0x90; //VK_NUMLOCK
            input.wScan = 0;
            input.dwFlags = 0; //key-down
            input.time = 0;
            input.dwExtraInfo = IntPtr.Zero;

            input.type1 = 0x01;
            input.wVk1 = 0x90;
            input.wScan1 = 0;
            input.dwFlags1 = 2; //key-up
            input.time1 = 0;
            input.dwExtraInfo1 = IntPtr.Zero;

            IntPtr pI = Marshal.AllocHGlobal(mouseInpSize * 2);
            Marshal.StructureToPtr(input, pI, false);
            int result = SendInput(2, pI, mouseInpSize); //Hardcoded size of the MOUSEINPUT tag !!!
            Marshal.FreeHGlobal(pI);
        }
    }
}
