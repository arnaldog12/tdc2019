using System;
using System.Runtime.InteropServices;

namespace AndroidApp
{
    public class Wrapper
    {
        [DllImport("libTensorFlowARM.so", CharSet = CharSet.Ansi)]
        internal static extern int predictDigit(IntPtr data, int height, int width);
    }
}