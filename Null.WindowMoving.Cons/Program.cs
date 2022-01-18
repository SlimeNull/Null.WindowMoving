using Null.WindowMoving.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Null.WindowMoving.Cons
{
    internal class Program
    {
        static bool hotkeyDown = false;

        static IntPtr currentWindow;
        static int windowStartPointX, windowStartPointY;
        static int mouseStartPointX, mouseStartPointY;

        static void TestHotkey()
        {
            const int LWIN = 0x5B, LSHIFT = 0xA0;
            bool valueBefore = hotkeyDown;
            hotkeyDown = (WinApis.GetAsyncKeyState(LWIN) & WinApis.GetAsyncKeyState(LSHIFT)) != 0;

            if (hotkeyDown && !valueBefore)
            {
                currentWindow = WinApis.GetForegroundWindow();
                if (WinApis.GetCursorPos(out Point mouseP) && WinApis.GetWindowRect(currentWindow, out Rect rect))
                {
                    mouseStartPointX = mouseP.X;
                    mouseStartPointY = mouseP.Y;
                    windowStartPointX = rect.X;
                    windowStartPointY = rect.Y;
                }
                else
                {
                    hotkeyDown = false;
                }
            }
        }

        static void ProcessWindowMoving()
        {
            if (hotkeyDown)
            {
                WinApis.GetCursorPos(out Point currentMousePoint);
                int offsetX = currentMousePoint.X - mouseStartPointX,
                    offsetY = currentMousePoint.Y - mouseStartPointY;
                if (WinApis.GetForegroundWindow().Equals(currentWindow))
                {
                    Point newWindowPoint = new Point(windowStartPointX + offsetX, windowStartPointY + offsetY);
                    WinApis.SetWindowPos(currentWindow, IntPtr.Zero, newWindowPoint.X, newWindowPoint.Y, -1, -1, 1 | 4);
                }
            }
        }

        static void Main(string[] args)
        {
            while(true)
            {
                TestHotkey();
                ProcessWindowMoving();
                Thread.Sleep(1);
            }
        }
    }
}
