using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Debug
{
    internal static class MossPhysics2DNativeDebug
    {
        public const string Module = "Physics2D";

        public static string NativeLibraryName => MossNativeLibrary.Name;

        public static bool TryLoad(out IntPtr handle) => NativeLibrary.TryLoad(MossNativeLibrary.Name, out handle);

        public static void Free(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                NativeLibrary.Free(handle);
            }
        }
    }
}
