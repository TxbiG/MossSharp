using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Assets
{
    internal enum MossAssetType
    {
        Unknown = 0,
        Binary,
        Json,
        Texture,
        Mesh,
        Shader,
        Font,
        Audio
    }

    internal readonly record struct MossAssetHandle(uint Value);

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossAssetManagerDesc
    {
        public IntPtr RootPath;
        [MarshalAs(UnmanagedType.I1)] public bool EnableHotReload;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossAssetMountDesc
    {
        public IntPtr VirtualRoot;
        public IntPtr PhysicalRoot;
        public IntPtr PckPath;
        public IntPtr PckKey32;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossAssetLoadDesc
    {
        public MossAssetType Type;
        public IntPtr Path;
        public IntPtr Asset;
        public IntPtr UserData;
        public MossAssetDestroyFn? Destroy;
        public MossAssetReloadFn? Reload;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossAssetData
    {
        public IntPtr Data;
        public nuint Size;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossAssetDestroyFn(IntPtr asset, IntPtr userData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal delegate bool MossAssetReloadFn(IntPtr manager, MossAssetHandle handle, IntPtr asset, IntPtr userData);

    internal static class MossAssetsNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AssetManagerCreate(in MossAssetManagerDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetManagerDestroy(IntPtr manager);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AssetManagerMount(IntPtr manager, in MossAssetMountDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetManagerUnmountAll(IntPtr manager);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AssetResolvePath(IntPtr manager, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr outPath, nuint outCapacity);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossAssetData Moss_AssetReadFile(IntPtr manager, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetFreeData(MossAssetData data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossAssetHandle Moss_AssetLoad(IntPtr manager, in MossAssetLoadDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AssetReload(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetRetain(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetRelease(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetUnload(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_AssetGetRefCount(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossAssetType Moss_AssetGetType(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AssetGetPath(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AssetGet(IntPtr manager, MossAssetHandle handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AssetSet(IntPtr manager, MossAssetHandle handle, IntPtr asset, MossAssetDestroyFn? destroy, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_AssetPollHotReload(IntPtr manager);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AssetGetLastError(IntPtr manager);
    }
}
