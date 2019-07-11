using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ExampleTest.Src.Wrapper
{
    public class ExampleWrapper
    {

        [DllImport("libTensorFlowARM.so", CharSet = CharSet.Ansi)]
        internal static extern int predictDigit(IntPtr data, int height, int width);
    }
}