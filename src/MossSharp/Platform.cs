using System;
using System.Runtime.InteropServices;
using MossSharp.Native.Platform;

namespace Moss.Platform
{
    public static class PlatformSystem
    {
        public static int AvailableCpuCores => MossPlatformNative.Moss_GetAvailableCPUCores();
        public static int CpuCacheLineSize => MossPlatformNative.Moss_GetCPUCacheLineSize();
        public static int SystemRamMb => MossPlatformNative.Moss_GetSystemRAM();
        public static string ClipboardText => Marshal.PtrToStringUTF8(MossPlatformNative.Moss_GetClipboardText()) ?? string.Empty;

        public static bool OpenUrl(string url) => MossPlatformNative.Moss_OpenURL(url);
        public static bool SetClipboardText(string text) => MossPlatformNative.Moss_SetClipboardText(text);
        public static bool IsWindowFocused(IntPtr window) => MossPlatformNative.Moss_IsWindowFocused(window);
        public static void SetWindowAlwaysOnTop(IntPtr window, bool enabled) => MossPlatformNative.Moss_SetWindowAlwaysOnTop(window, enabled);
        public static void SetWindowBorderless(IntPtr window, bool borderless) => MossPlatformNative.Moss_SetWindowBorderless(window, borderless);
        public static float MouseWheelDelta => MossPlatformNative.Moss_GetMouseWheelDelta();
    }

    public sealed class Storage : IDisposable
    {
        public Storage(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; private set; }
        public bool IsReady => Handle != IntPtr.Zero && MossPlatformNative.Moss_StorageReady(Handle);
        public ulong SpaceRemaining => MossPlatformNative.Moss_GetStorageSpaceRemaining(Handle);

        public static Storage OpenFileStorage(string path) => new(MossPlatformNative.Moss_OpenFileStorage(path));
        public static Storage OpenTitleStorage(string? overridePath = null, ulong properties = 0) => new(MossPlatformNative.Moss_OpenTitleStorage(overridePath, properties));

        public bool CreateDirectory(string path) => MossPlatformNative.Moss_CreateStorageDirectory(Handle, path);
        public bool RemovePath(string path) => MossPlatformNative.Moss_RemoveStoragePath(Handle, path);
        public bool RenamePath(string oldPath, string newPath) => MossPlatformNative.Moss_RenameStoragePath(Handle, oldPath, newPath);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossPlatformNative.Moss_CloseStorage(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }
}