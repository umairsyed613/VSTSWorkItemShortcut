using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VSTSWorkItemShortcut
{
    class MemoryHelper
    {
        [DllImport("kernel32")]
        static extern bool SetProcessWorkingSetSize(IntPtr handle, int minSize, int maxSize);

        public static void MemoryCleanup()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}
