using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoClickerUT
{
    //class InterceptKeys
    //{
    //    private const int WH_KEYBOARD_LL = 13;
    //    private const int WM_KEYDOWN = 0x0100;
    //    private static LowLevelKeyboardProc _proc = HookCallback;
    //    private static IntPtr _hookID = IntPtr.Zero;

    //    public static void InitializeComponent()
    //    {
    //        _hookID = SetHook(_proc);
    //        Application.Run();
    //        UnhookWindowsHookEx(_hookID);
    //    }

    //    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    //    {
    //        using (Process curProcess = Process.GetCurrentProcess())
    //        using (ProcessModule curModule = curProcess.MainModule)
    //        {
    //            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
    //                GetModuleHandle(curModule.ModuleName), 0);
    //        }
    //    }

    //    private delegate IntPtr LowLevelKeyboardProc(
    //        int nCode, IntPtr wParam, IntPtr lParam);

    //    private static IntPtr HookCallback(
    //        int nCode, IntPtr wParam, IntPtr lParam)
    //    {
    //        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
    //        {
    //            int vkCode = Marshal.ReadInt32(lParam);
    //            Console.WriteLine((Keys)vkCode);
    //        }
    //        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    //    }

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr SetWindowsHookEx(int idHook,
    //        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
    //        IntPtr wParam, IntPtr lParam);

    //    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr GetModuleHandle(string lpModuleName);
    //}

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}