using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using static AutoClickerUT.Form1;

namespace AutoClickerUT
{
    public partial class Form1 : Form
    {

        //mouse event constants
        const int MOUSEEVENTF_LEFTDOWN = 2;
        const int MOUSEEVENTF_LEFTUP = 4;
        //input type constant
        const int INPUT_MOUSE = 0;

        int ScreenWidth;
        int ScreenHeight;

        int clickCounter;

        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        };

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        public class LowLevelKeyboardHook
        {
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private const int WM_SYSKEYDOWN = 0x0104;
            private const int WM_KEYUP = 0x101;
            private const int WM_SYSKEYUP = 0x105;

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

            public event EventHandler<Keys> OnKeyPressed;
            public event EventHandler<Keys> OnKeyUnpressed;

            private LowLevelKeyboardProc _proc;
            private IntPtr _hookID = IntPtr.Zero;

            public LowLevelKeyboardHook()
            {
                _proc = HookCallback;
            }

            public void HookKeyboard()
            {
                _hookID = SetHook(_proc);
            }

            public void UnHookKeyboard()
            {
                UnhookWindowsHookEx(_hookID);
            }

            private IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);

                    OnKeyPressed.Invoke(this, ((Keys)vkCode));
                }
                else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);

                    OnKeyUnpressed.Invoke(this, ((Keys)vkCode));
                }

                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }

        public class LowLevelMouseHook
        {
            private const int WH_MOUSE_LL = 14;
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_LBUTTONUP = 0x0202;

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

            public event EventHandler OnLMousePressed;
            public event EventHandler OnLMouseUnpressed;

            private LowLevelMouseProc _proc;
            private IntPtr _hookID = IntPtr.Zero;

            public LowLevelMouseHook()
            {
                _proc = HookCallback;
            }

            public void HookMouse()
            {
                _hookID = SetHook(_proc);
            }

            public void UnHookMouse()
            {
                UnhookWindowsHookEx(_hookID);
            }

            private IntPtr SetHook(LowLevelMouseProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    OnLMousePressed.Invoke(this, EventArgs.Empty);
                }
                else if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONUP)
                {
                    OnLMouseUnpressed.Invoke(this, EventArgs.Empty);
                }

                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }

        int timer1Duration = 0;
        int curX=0, curY=0;
        LowLevelMouseHook lowLevelMouseHook1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = System.Convert.ToInt32(textBox1.Text);
            int y = System.Convert.ToInt32(textBox2.Text);
            int w = 30;
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            Rectangle mouseNewRect = new Rectangle(new Point(x-w/2, y-w/2), new Size(w, w));
            g.FillRectangle(new SolidBrush(Color.Magenta), mouseNewRect);

            Cursor.Position = new System.Drawing.Point(x, y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1Duration = this.timer1Duration - timer1.Interval;
            if(timer1Duration == 0)
            {
                timer1.Enabled = false;
                button2_Click(null, null);
            }

            //set cursor position to memorized location
            Cursor.Position = new System.Drawing.Point(curX,curY);
            //set up the INPUT struct and fill it for the mouse down
            INPUT i = new INPUT();
            i.type = INPUT_MOUSE;
            i.mi.dx = 0;
            i.mi.dy = 0;
            i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            i.mi.dwExtraInfo = IntPtr.Zero;
            i.mi.mouseData = 0;
            i.mi.time = 0;
            //send the input 
            SendInput(1, ref i, Marshal.SizeOf(i));
            //set the INPUT for mouse up and send it
            i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref i, Marshal.SizeOf(i));

            clickCounter = clickCounter+ 1;
            label10.Text = clickCounter.ToString();

            if(!checkBox1.Checked)
            {
                progressBar1.Value = clickCounter * timer1.Interval;
            }

            DateTime dateTime1 = DateTime.Now;
            DateTime dateTime2 = dateTime1.AddMilliseconds(timer1.Interval);
            label13.Text = dateTime1.ToString();
            label14.Text = dateTime2.ToString();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.textBox1;
            textBox2.Text = Properties.Settings.Default.textBox2;
            textBox3.Text = Properties.Settings.Default.textBox3;
            textBox4.Text = Properties.Settings.Default.textBox4;
            comboBox1.SelectedIndex = Properties.Settings.Default.comboBox1;
            comboBox2.SelectedIndex = Properties.Settings.Default.comboBox2;
            label10.Text = "0";
            label16.Text = "Durduruldu";
            label13.Text = "";
            label14.Text = "";
            label17.Visible = false;

            LowLevelKeyboardHook lowLevelKeyboardHook1 = new LowLevelKeyboardHook();
            lowLevelKeyboardHook1.OnKeyPressed += lowLevelKeyboardHook1_OnKeyPressed;
            lowLevelKeyboardHook1.OnKeyUnpressed += lowLevelKeyboardHook1_OnKeyUnpressed;
            lowLevelKeyboardHook1.HookKeyboard();
        }

        void lowLevelKeyboardHook1_OnKeyPressed(object sender, Keys e)
        {
            if(e == Keys.F6)
            {
                button2_Click(null, null);
            }
            //if (e == Keys.LControlKey)
            //{
            //    lctrlKeyPressed = true;
            //}
            //else if (e == Keys.F1)
            //{
            //    f1KeyPressed = true;
            //}
            //CheckKeyCombo();
        }

        void lowLevelKeyboardHook1_OnKeyUnpressed(object sender, Keys e)
        {
            //if (e == Keys.LControlKey)
            //{
            //    lctrlKeyPressed = false;
            //}
            //else if (e == Keys.F1)
            //{
            //    f1KeyPressed = false;
            //}
        }

        void CheckKeyCombo()
        {
            //if (lctrlKeyPressed && f1KeyPressed)
            //{
            //    //Open Form
            //}
        }

        void lowLevelMouseHook1_OnLMousePressed(object sender, EventArgs e)
        {
            lowLevelMouseHook1.UnHookMouse();

            System.Drawing.Point point1 = System.Windows.Forms.Cursor.Position;
            textBox1.Text = point1.X.ToString();
            textBox2.Text = point1.Y.ToString();

            label17.Visible = false;
        }

        void lowLevelMouseHook1_OnLMouseUnpressed(object sender, EventArgs e)
        {

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.textBox1 = textBox1.Text;
            Properties.Settings.Default.textBox2 = textBox2.Text;
            Properties.Settings.Default.textBox3 = textBox3.Text;
            Properties.Settings.Default.textBox4 = textBox4.Text;
            Properties.Settings.Default.comboBox1 = comboBox1.SelectedIndex;
            Properties.Settings.Default.comboBox2 = comboBox2.SelectedIndex;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            textBox1.Text = hScrollBar1.Value.ToString();
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            textBox2.Text = hScrollBar2.Value.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int int1 = -1;
            try
            {
                int1 = System.Convert.ToInt32(textBox1.Text);
            }
            catch(System.Exception ex)
            {
                textBox1.Text = "0";
                int1 = 0;
            }

            hScrollBar1.Value = int1;

            label18.Text = "X=" + textBox1.Text.ToString() + ", Y=" + textBox2.Text.ToString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int int1 = -1;
            try
            {
                int1 = System.Convert.ToInt32(textBox2.Text);
            }
            catch (System.Exception ex)
            {
                textBox2.Text = "0";
                int1 = 0;
            }

            hScrollBar2.Value = int1;

            label18.Text = "X=" + textBox1.Text.ToString() + ", Y=" + textBox2.Text.ToString();
        }

        private void Form1_Move(object sender, EventArgs e)
        {

            ScreenWidth = Screen.GetBounds(new Point(this.Left, this.Top)).Width;
            ScreenHeight = Screen.GetBounds(new Point(this.Left, this.Top)).Height;
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = ScreenWidth + hScrollBar1.LargeChange - 1;
            hScrollBar2.Minimum = 0;
            hScrollBar2.Maximum = ScreenHeight + hScrollBar2.LargeChange - 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label17.Visible = true;

            lowLevelMouseHook1 = new LowLevelMouseHook();
            lowLevelMouseHook1.OnLMousePressed += lowLevelMouseHook1_OnLMousePressed;
            lowLevelMouseHook1.OnLMouseUnpressed += lowLevelMouseHook1_OnLMouseUnpressed;
            lowLevelMouseHook1.HookMouse();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox4.Enabled = false;
                comboBox2.Enabled = false;
            }
            else
            {
                textBox4.Enabled = true;
                comboBox2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "DURDUR")
            {
                timer1.Enabled = false;
                button2.Text = "BAŞLA";
                label16.Text = "Durduruldu";
                return;
            }


            int int1 = -1;
            int int2 = -1;

            try
            {
                int1 = System.Convert.ToInt32(textBox3.Text);
            }
            catch(System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                int2 = System.Convert.ToInt32(textBox4.Text);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }

            if (comboBox1.SelectedIndex == 0) //milisecond
            {
                //do nothing
            }
            else if (comboBox1.SelectedIndex == 1) //second
            {
                int1 = int1 * 1000;
            }
            else if (comboBox1.SelectedIndex == 2) //minute
            {
                int1 = int1 * 60 * 1000;
            }
            else if (comboBox1.SelectedIndex == 3) //hour
            {
                int1 = int1 * 60 * 60 * 1000;
            }
            else if (comboBox1.SelectedIndex == 4) //day
            {
                int1 = int1 * 24 * 60 * 60 * 1000;
            }

            textBox3.Text = int1.ToString();
            if (!checkBox1.Checked)
            {
                textBox4.Text = int2.ToString();

                if (comboBox2.SelectedIndex == 0) //milisecond
                {
                    //do nothing
                }
                else if (comboBox2.SelectedIndex == 1) //second
                {
                    int2 = int2 * 1000;
                }
                else if (comboBox2.SelectedIndex == 2) //minute
                {
                    int2 = int2 * 60 * 1000;
                }
                else if (comboBox2.SelectedIndex == 3) //hour
                {
                    int2 = int2 * 60 * 60 * 1000;
                }
                else if (comboBox2.SelectedIndex == 4) //day
                {
                    int2 = int2 * 24 * 60 * 60 * 1000;
                }
            }


            timer1.Interval = int1;
            this.timer1Duration = int2;
            this.curX = System.Convert.ToInt32(textBox1.Text);
            this.curY = System.Convert.ToInt32(textBox2.Text);

            clickCounter = 0;
            label10.Text = clickCounter.ToString();

            progressBar1.Minimum = 0;
            if (checkBox1.Checked)
            {
                progressBar1.Maximum = 100;
            }
            else
            {
                progressBar1.Maximum = timer1Duration;
            }
            
            progressBar1.Value = 0;

            button2.Text = "DURDUR";
            label16.Text = "Çalışıyor";
            timer1.Enabled = true;
        }
    }
}