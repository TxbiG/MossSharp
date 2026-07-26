using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Debug
{
    internal static class MossRendererNativeDebug
    {
        public const string Module = "Renderer";

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
