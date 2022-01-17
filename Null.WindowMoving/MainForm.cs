using Gma.System.MouseKeyHook;
using Null.WindowMoving.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Null.WindowMoving
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IKeyboardMouseEvents keyboardMouseEvents = Hook.GlobalEvents();

            mainForm_MouseMove = MainForm_MouseMove;
            keyboardMouseEvents_KeyDown = KeyboardMouseEvents_KeyDown;
            keyboardMouseEvents_KeyUp = KeyboardMouseEvents_KeyUp;
            keyboardMouseEvents.MouseMove += mainForm_MouseMove;
            keyboardMouseEvents.KeyDown += keyboardMouseEvents_KeyDown;
            keyboardMouseEvents.KeyUp += keyboardMouseEvents_KeyUp;
        }

        bool hotkeyDown = false;
        bool noWindowMove = false;

        static MouseEventHandler mainForm_MouseMove;
        static KeyEventHandler keyboardMouseEvents_KeyDown;
        static KeyEventHandler keyboardMouseEvents_KeyUp;

        private void KeyboardMouseEvents_KeyUp(object sender, KeyEventArgs e)
        {
            if (hotkeyDown && featureEnabled.Checked)
            {
                hotkeyDown = false;
            }
        }

        private void KeyboardMouseEvents_KeyDown(object sender, KeyEventArgs e)
        {
            if (!hotkeyDown && e.Shift && e.KeyData.HasFlag(Keys.LWin) && featureEnabled.Checked)
            {
                mouseStartPoint = MousePosition;
                currentWindow = WinApis.GetForegroundWindow();
                if (WinApis.GetWindowRect(currentWindow, out Rectangle rectangle))
                {
                    windowStartPoint = rectangle.Location;

                    hotkeyDown = true;
                    e.Handled = true;
                }
            }
        }

        IntPtr currentWindow;
        Point windowStartPoint;
        Point mouseStartPoint;
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (hotkeyDown && featureEnabled.Checked)
            {
                if (noWindowMove)
                {
                    noWindowMove = false;
                    return;
                }

                Point currentMousePoint = MousePosition;
                int offsetX = currentMousePoint.X - mouseStartPoint.X,
                    offsetY = currentMousePoint.Y - mouseStartPoint.Y;
                if (WinApis.GetForegroundWindow().Equals(currentWindow))
                {
                    Point newWindowPoint = new Point(windowStartPoint.X + offsetX, windowStartPoint.Y + offsetY);
                    WinApis.SetWindowPos(currentWindow, IntPtr.Zero, newWindowPoint.X, newWindowPoint.Y, -1, -1, 1 | 4);
                }
            }
        }
    }
}
