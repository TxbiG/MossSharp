using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Debug
{
    internal readonly record struct MossNativeModuleStatus(string Module, bool Loaded, string NativeLibraryName);

    internal static class MossNativeDebug
    {
        public static IReadOnlyList<MossNativeModuleStatus> ProbeAll()
        {
            string libraryName = MossNativeLibrary.Name;
            bool loaded = NativeLibrary.TryLoad(libraryName, out IntPtr handle);
            if (loaded && handle != IntPtr.Zero)
            {
                NativeLibrary.Free(handle);
            }

            return new[]
            {
                new MossNativeModuleStatus(MossAssetsNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossAudioNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossComponentsNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossGPUNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossGUINativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossNavigationNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossNetworkNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossPhysics2DNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossPhysics3DNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossPlatformNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossRendererNativeDebug.Module, loaded, libraryName),
                new MossNativeModuleStatus(MossXRNativeDebug.Module, loaded, libraryName)
            };
        }
    }
}