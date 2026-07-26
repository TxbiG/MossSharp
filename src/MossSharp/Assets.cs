using System;
using System.Runtime.InteropServices;
using MossSharp.Native.Assets;

namespace Moss.Assets
{
    public enum AssetType { Unknown = 0, Binary, Json, Texture, Mesh, Shader, Font, Audio }

    public readonly record struct AssetHandle(uint Value)
    {
        public bool IsNull => Value == 0;
    }

    public sealed class AssetManager : IDisposable
    {
        public AssetManager(IntPtr handle) => Handle = handle;

        public IntPtr Handle { get; private set; }
        public bool IsDisposed => Handle == IntPtr.Zero;

        public static AssetManager Create(string? rootPath = null, bool enableHotReload = false)
        {
            IntPtr nativeRoot = rootPath is null ? IntPtr.Zero : Marshal.StringToCoTaskMemUTF8(rootPath);
            try
            {
                var desc = new MossAssetManagerDesc
                {
                    RootPath = nativeRoot,
                    EnableHotReload = enableHotReload
                };
                return new AssetManager(MossAssetsNative.Moss_AssetManagerCreate(in desc));
            }
            finally
            {
                if (nativeRoot != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(nativeRoot);
                }
            }
        }

        public AssetHandle Load(string path, AssetType type = AssetType.Unknown)
        {
            IntPtr nativePath = Marshal.StringToCoTaskMemUTF8(path);
            try
            {
                var desc = new MossAssetLoadDesc { Path = nativePath, Type = (MossAssetType)type };
                return new AssetHandle(MossAssetsNative.Moss_AssetLoad(Handle, in desc).Value);
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativePath);
            }
        }

        public void Retain(AssetHandle handle) => MossAssetsNative.Moss_AssetRetain(Handle, ToNative(handle));
        public void Release(AssetHandle handle) => MossAssetsNative.Moss_AssetRelease(Handle, ToNative(handle));
        public void Unload(AssetHandle handle) => MossAssetsNative.Moss_AssetUnload(Handle, ToNative(handle));
        public uint GetRefCount(AssetHandle handle) => MossAssetsNative.Moss_AssetGetRefCount(Handle, ToNative(handle));
        public IntPtr Get(AssetHandle handle) => MossAssetsNative.Moss_AssetGet(Handle, ToNative(handle));
        public uint PollHotReload() => MossAssetsNative.Moss_AssetPollHotReload(Handle);
        public string LastError => Marshal.PtrToStringUTF8(MossAssetsNative.Moss_AssetGetLastError(Handle)) ?? string.Empty;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossAssetsNative.Moss_AssetManagerDestroy(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private static MossAssetHandle ToNative(AssetHandle handle) => new(handle.Value);
    }
}