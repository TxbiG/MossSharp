using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Platform
{
    internal static class MossPlatformNative
    {
        [StructLayout(LayoutKind.Sequential)] internal struct MossImage { public int Width, Height; public IntPtr Pixels; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossMonitorRect { public int X, Y, Width, Height; }
        [StructLayout(LayoutKind.Sequential)] internal unsafe struct MossGammaRamp { public uint Size; public fixed ushort Red[256]; public fixed ushort Green[256]; public fixed ushort Blue[256]; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossVideoMode { public int Width, Height, RedBits, GreenBits, BlueBits, RefreshRate; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossLocale { public IntPtr Country, Language; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossFinger { public uint Id; public float X, Y, Pressure; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossVideoCaptureFrame { public IntPtr Pixels; public int Width, Height, Stride, Format; public ulong TimestampNs; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossCameraSpec { public int Format, Colorspace, Width, Height, FramerateNumerator, FramerateDenominator; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossDialogFileFilter { public IntPtr Name, Pattern; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossPathInfo { public int Type; public ulong Size, CreatedTime, ModifiedTime, AccessedTime; [MarshalAs(UnmanagedType.I1)] public bool Readable; [MarshalAs(UnmanagedType.I1)] public bool Writable; [MarshalAs(UnmanagedType.I1)] public bool Executable; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossFramebufferResizeCallback(int width, int height);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossWindowSizeCallback(int width, int height);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossWindowPositionCallback(int x, int y);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossWindowFocusCallback([MarshalAs(UnmanagedType.I1)] bool focused);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossWindowContentScaleCallback(float xscale, float yscale);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossWindowResizeCallback(int width, int height);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossMonitorCallback(IntPtr monitorName, [MarshalAs(UnmanagedType.I1)] bool connected);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossDialogFileCallback(IntPtr userdata, IntPtr filelist, int filter);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal delegate bool MossDirectoryIterateFn(in MossPathInfo info, IntPtr path, IntPtr userData);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateWindow([MarshalAs(UnmanagedType.LPUTF8Str)] string title, int width, int height, IntPtr monitor, IntPtr share);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerminateWindow(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CreateMessageBox([MarshalAs(UnmanagedType.LPUTF8Str)] string title, [MarshalAs(UnmanagedType.LPUTF8Str)] string message, int flags, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ShouldWindowClose(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PollEvents();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetWindowWidth();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetWindowHeight();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowTitle(IntPtr window, [MarshalAs(UnmanagedType.LPUTF8Str)] string title);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowIcon(IntPtr window, MossImage image);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CloseWindow(IntPtr window);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MonitorGetPrimary();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MonitorGetSecondary();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MonitorGetPhysicalSize(IntPtr monitor, out int widthMm, out int heightMm);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MonitorGetContentScale(IntPtr monitor, out float xscale, out float yscale);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MonitorGetPosition(IntPtr monitor, out int x, out int y);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_MonitorGetRect(IntPtr monitor, out MossMonitorRect rect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_MonitorGetWorkArea(IntPtr monitor, out MossMonitorRect rect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MonitorGetName(IntPtr monitor);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MonitorSetGammaRamp(IntPtr monitor, in MossGammaRamp gammaRamp);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MonitorGetGammaRamp(IntPtr monitor);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MonitorSetGamma(IntPtr monitor, float gamma);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsKeyPressed(int key);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsReleased(int key);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsKeyJustPressed(int key);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsKeyJustReleased(int key);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_InputGetKey();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMousePressed(int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMouseReleased(int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMouseJustPressed(int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMouseJustReleased(int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_InputGetMouseButton();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GetMousePosition(out int x, out int y);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetMousePosition(int x, int y);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetMouseVisible([MarshalAs(UnmanagedType.I1)] bool visible);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowAlwaysOnTop(IntPtr window, [MarshalAs(UnmanagedType.I1)] bool onTop);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowBorderless(IntPtr window, [MarshalAs(UnmanagedType.I1)] bool borderless);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_GetMouseWheelDelta();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetTextInput();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsWindowFocused(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetPenDeviceType(uint penId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetTouchDeviceName(ulong touchId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetTouchDevices(out int count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetTouchDeviceType(ulong touchId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetTouchFingers(ulong touchId, out int count);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetNumGamepads();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenGamepad(uint id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CloseGamepad(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GamepadConnected(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UpdateGamepads();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsGamepadButtonPressed(IntPtr gamepad, int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsGamepadButtonJustPressed(IntPtr gamepad, int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsGamepadButtonJustReleased(IntPtr gamepad, int button);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_GetGamepadAxis(IntPtr gamepad, int axis);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGamepadAxisDeadzone(int axis, float deadzone);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGamepadAxisInverted(int axis, [MarshalAs(UnmanagedType.I1)] bool inverted);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RumbleGamepad(IntPtr gamepad, ushort low, ushort high, uint durationMs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GamepadRumble(IntPtr gamepad, float lowFrequency, float highFrequency, uint durationMs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RumbleGamepadTriggers(IntPtr gamepad, ushort left, ushort right, uint durationMs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetGamepadLED(IntPtr gamepad, byte r, byte g, byte b);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGamepadName(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetGamepadID(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetGamepadPlayerIndex(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetGamepadPowerInfo(IntPtr gamepad, out int percent);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetNumGamepadTouchpads(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetNumGamepadTouchpadFingers(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetGamepadTouchpadFinger(IntPtr gamepad, int pad, int finger, [MarshalAs(UnmanagedType.I1)] out bool down, out float x, out float y, out float pressure);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGamepadMapping(IntPtr gamepad);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetGamepadMapping(IntPtr gamepad, [MarshalAs(UnmanagedType.LPUTF8Str)] string mapping);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReloadGamepadMappings();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_InputGetGamepadButton();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_InputGetGamepadAxis();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenHaptic(uint deviceIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CloseHaptic(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_CreateHapticEffect(IntPtr haptic, IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DestroyHapticEffect(IntPtr haptic, int effectId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetHapticEffectStatus(IntPtr haptic, int effectId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetHapticFeatures(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetHapticFromID(uint instanceId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetHapticID(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetHapticName(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetHapticNameForID(uint deviceIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetHaptics(out int count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetMaxHapticEffects(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetMaxHapticEffectsPlaying(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetNumHapticAxes(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_HapticEffectSupported(IntPtr haptic, IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_HapticRumbleSupported(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_InitHapticRumble(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsJoystickHaptic(IntPtr joystick);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMouseHaptic();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenHapticFromJoystick(IntPtr joystick);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenHapticFromMouse();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PauseHaptic(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_PlayHapticRumble(IntPtr haptic, float strength, uint length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_HapticRumble(IntPtr haptic, float strength, uint length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResumeHaptic(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_RunHapticEffect(IntPtr haptic, int effectId, uint iterations);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_SetHapticAutocenter(IntPtr haptic, int autocenter);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_SetHapticGain(IntPtr haptic, int gain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_StopHapticEffect(IntPtr haptic, int effectId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_StopHapticEffects(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_StopHapticRumble(IntPtr haptic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_UpdateHapticEffect(IntPtr haptic, int effectId, IntPtr effect);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetAvailableCPUCores();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetCPUCacheLineSize();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetSystemRAM();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetPlatformTheme();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_OpenURL([MarshalAs(UnmanagedType.LPUTF8Str)] string url);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetLocale();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetPowerInfo(out int seconds, out int percent);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsProcessRunningByName([MarshalAs(UnmanagedType.LPUTF8Str)] string executablePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_LoadDynamicLibrary([MarshalAs(UnmanagedType.LPUTF8Str)] string libPath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetLibrarySymbol(IntPtr handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string symbolName);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UnloadDynamicLibrary(IntPtr handle);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetFramebufferResizeCallback(MossFramebufferResizeCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowSizeCallback(MossWindowSizeCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowResizeCallback(MossWindowResizeCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowPositionCallback(MossWindowPositionCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowFocusCallback(MossWindowFocusCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetWindowContentScaleCallback(MossWindowContentScaleCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetMonitorCallback(MossMonitorCallback callback);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetCameras(out int count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetCameraName(uint cameraId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetCameraPosition(uint cameraId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetCurrentCameraDriver();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetNumCameraDrivers();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetCameraSupportedFormats(uint cameraId, out int count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AcquireCameraFrame(IntPtr camera, out ulong timestampNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseCameraFrame(IntPtr camera, IntPtr frame);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetCameraFormat(IntPtr camera, out MossCameraSpec spec);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetCameraPermissionState(IntPtr camera);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ulong Moss_GetCameraProperties(IntPtr camera);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CloseCamera(IntPtr camera);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetCameraID(IntPtr camera);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenCamera(uint instanceId, in MossCameraSpec spec);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenVideoCapture(uint captureId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CloseVideoCapture(IntPtr capture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VideoCaptureReadFrame(IntPtr capture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_VideoCaptureReadFrameEx(IntPtr capture, out MossVideoCaptureFrame frame);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CopyFile([MarshalAs(UnmanagedType.LPUTF8Str)] string sourcePath, [MarshalAs(UnmanagedType.LPUTF8Str)] string destinationPath, [MarshalAs(UnmanagedType.I1)] bool overwrite);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CreateDirectory([MarshalAs(UnmanagedType.LPUTF8Str)] string path, [MarshalAs(UnmanagedType.I1)] bool recursive);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RemovePath([MarshalAs(UnmanagedType.LPUTF8Str)] string path, [MarshalAs(UnmanagedType.I1)] bool recursive);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RenamePath([MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath, [MarshalAs(UnmanagedType.I1)] bool overwrite);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetPathInfo([MarshalAs(UnmanagedType.LPUTF8Str)] string path, out MossPathInfo info);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetCurrentDirectory(IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetBasePath(IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetUserFolder(int folder, IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetPrefPath([MarshalAs(UnmanagedType.LPUTF8Str)] string orgName, [MarshalAs(UnmanagedType.LPUTF8Str)] string appName, IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetAppDataPath([MarshalAs(UnmanagedType.LPUTF8Str)] string orgName, [MarshalAs(UnmanagedType.LPUTF8Str)] string appName, IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetCachePath([MarshalAs(UnmanagedType.LPUTF8Str)] string orgName, [MarshalAs(UnmanagedType.LPUTF8Str)] string appName, IntPtr outPath, int maxLen);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_EnumerateDirectory([MarshalAs(UnmanagedType.LPUTF8Str)] string path, [MarshalAs(UnmanagedType.I1)] bool recursive, MossDirectoryIterateFn callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GlobDirectory([MarshalAs(UnmanagedType.LPUTF8Str)] string pattern, MossDirectoryIterateFn callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetClipboardText([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetClipboardText();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetClipboardImage(in MossImage image);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetClipboardImage(out MossImage image);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FreeClipboardImage(ref MossImage image);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetCursorMode(IntPtr window, int mode);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetCursorMode(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GetWindowContentScale(IntPtr window, out float xscale, out float yscale);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShowFileDialogWithProperties(MossDialogFileCallback callback, IntPtr userdata, IntPtr window, IntPtr filters, int nfilters, [MarshalAs(UnmanagedType.LPUTF8Str)] string? defaultLocation);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShowOpenFileDialog(MossDialogFileCallback callback, IntPtr userdata, IntPtr window, [MarshalAs(UnmanagedType.LPUTF8Str)] string? defaultLocation, [MarshalAs(UnmanagedType.I1)] bool allowMany);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShowOpenFolderDialog(MossDialogFileCallback callback, IntPtr userdata, IntPtr window, IntPtr filters, int nfilters, [MarshalAs(UnmanagedType.LPUTF8Str)] string? defaultLocation, [MarshalAs(UnmanagedType.I1)] bool allowMany);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShowSaveFileDialog(int type, MossDialogFileCallback callback, IntPtr userdata, ulong props);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CloseStorage(IntPtr storage);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenFileStorage([MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_StorageReady(IntPtr storage);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ulong Moss_GetStorageSpaceRemaining(IntPtr storage);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ReadStorageFile(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr destination, ulong length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WriteStorageFile(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr source, ulong length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CopyStorageFile(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_CreateStorageDirectory(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_EnumerateStorageDirectory(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, MossDirectoryIterateFn callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetStorageFileSize(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, out ulong length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetStoragePathInfo(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, out MossPathInfo info);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GlobStorageDirectory(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string pattern, MossDirectoryIterateFn callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenStorage(IntPtr iface, IntPtr userdata);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenTitleStorage([MarshalAs(UnmanagedType.LPUTF8Str)] string? overridePath, ulong props);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenUserStorage(ulong userId, ulong props);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RemoveStoragePath(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RenameStoragePath(IntPtr storage, [MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MakeContextCurrent(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SwapBuffers();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SwapBuffersInterval(int interval);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetProcAddress([MarshalAs(UnmanagedType.LPUTF8Str)] string procname);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateEmbeddedWindow();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Terminate_EmbeddedWindow(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EmbeddedWindow_OnOrientationChanged(IntPtr window, int orientation);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EmbeddedWindow_OnResume(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EmbeddedWindow_OnPause(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_CreateWindowSurface(IntPtr window, IntPtr vkInstance, IntPtr allocator, out ulong vkSurface);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_VulkanSupported();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetRequiredInstanceExtensions(out uint count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetInstanceProcAddress(IntPtr vkInstance, [MarshalAs(UnmanagedType.LPUTF8Str)] string procName);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetPhysicalDevicePresentationSupport(IntPtr vkInstance, IntPtr physicalDevice, uint queueFamilyIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_Metal_CreateView(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Metal_DestroyView(IntPtr view);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_Metal_GetLayer(IntPtr view);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Metal_Resize(IntPtr view, int width, int height);
    }
}


