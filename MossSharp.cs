//                        MIT License
//
//                  Copyright (c) 2026 Toby
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Runtime.InteropServices;


internal static class MossNative
{
#if WINDOWS
    private const string LIB = "moss.dll";
#elif LINUX
    private const string LIB = "libmoss.so";
#elif MACOS
    private const string LIB = "libmoss.dylib";
#endif

using Moss_Window  = System.IntPtr;
using Moss_Monitor = System.IntPtr;
using Moss_Gamepad = System.IntPtr;
using Moss_Haptic  = System.IntPtr;
using Moss_Camera  = System.IntPtr;


public enum Moss_Keyboard : ushort {
    KEY_0, 
    KEY_1, 
    KEY_2, 
    KEY_3, 
    KEY_4, 
    KEY_5, 
    KEY_6, 
    KEY_7, 
    KEY_8, 
    KEY_9, 
    KEY_A, 
    KEY_B, 
    KEY_C, 
    KEY_D, 
    KEY_E, 
    KEY_F, 
    KEY_G, 
    KEY_H, 
    KEY_I, 
    KEY_J, 
    KEY_K, 
    KEY_L, 
    KEY_M,
    KEY_N, 
    KEY_O, 
    KEY_P, 
    KEY_Q, 
    KEY_R, 
    KEY_S, 
    KEY_T, 
    KEY_U, 
    KEY_V, 
    KEY_W, 
    KEY_X, 
    KEY_Y, 
    KEY_Z,
    KEY_APOSTROPHE, 
    KEY_BACKSLASH, 
    KEY_COMMA, 
    KEY_EQUAL,
    KEY_GRAVE_ACCENT, 
    KEY_LEFT_BRACKET, 
    KEY_MINUS, 
    KEY_PERIOD, 
    KEY_RIGHT_BRACKET, 
    KEY_SEMICOLON, 
    KEY_SLASH, 
    KEY_WORLD_1, 
    KEY_WORLD_2,     
    KEY_BACKSPACE,
    KEY_DELETE, 
    KEY_END, 
    KEY_ENTER, 
    KEY_ESCAPE, 
    KEY_HOME, 
    KEY_INSERT, 
    KEY_MENU, 
    KEY_PAGE_DOWN, 
    KEY_PAGE_UP, 
    KEY_PAUSE, 
    KEY_SPACE, 
    KEY_TAB, 
    KEY_CAPS_LOCK, 
    KEY_NUM_LOCK, 
    KEY_SCROLL_LOCK,
    KEY_F1, 
    KEY_F2, 
    KEY_F3,  
    KEY_F4,  
    KEY_F5,  
    KEY_F6,  
    KEY_F7,  
    KEY_F8,  
    KEY_F9, 
    KEY_F10, 
    KEY_F11, 
    KEY_F12, 
    KEY_F13, 
    KEY_F14, 
    KEY_F15, 
    KEY_F16, 
    KEY_F17, 
    KEY_F18, 
    KEY_F19, 
    KEY_F20, 
    KEY_F21, 
    KEY_F22, 
    KEY_F23, 
    KEY_F24,
    KEY_LEFT_ALT, 
    KEY_LEFT_CONTROL, 
    KEY_LEFT_SHIFT, 
    KEY_LEFT_SUPER,
    KEY_PRINT_SCREEN, 
    KEY_RIGHT_ALT, 
    KEY_RIGHT_CONTROL, 
    KEY_RIGHT_SHIFT, 
    KEY_RIGHT_SUPER, 
    KEY_DOWN, 
    KEY_LEFT, 
    KEY_RIGHT, 
    KEY_UP,
    KEY_KP_0, 
    KEY_KP_1, 
    KEY_KP_2, 
    KEY_KP_3, 
    KEY_KP_4, 
    KEY_KP_5, 
    KEY_KP_6, 
    KEY_KP_7, 
    KEY_KP_8, 
    KEY_KP_9, 
    KEY_KP_ADD, 
    KEY_KP_DECIMAL, 
    KEY_KP_DIVIDE, 
    KEY_KP_ENTER, 
    KEY_KP_EQUAL, 
    KEY_KP_MULTIPLY, 
    KEY_KP_SUBTRACT,
    MOSS_LAST_KEY = KEY_KP_SUBTRACT
};

// Mouse
public enum Moss_Mouse { 
    LEFT, 
    RIGHT, 
    MIDDLE, 
    BUTTON_4, 
    BUTTON_5, 
    BUTTON_6, 
    BUTTON_7, 
    BUTTON_8, 
    COUNT = 8 
};

public enum Gamepad {
    GAMEPAD_BUTTON_A, 
    GAMEPAD_BUTTON_B, 
    GAMEPAD_BUTTON_X, 
    GAMEPAD_BUTTON_Y,
    GAMEPAD_BUTTON_LEFT_BUMPER, 
    GAMEPAD_BUTTON_RIGHT_BUMPER,
    GAMEPAD_BUTTON_BACK, 
    GAMEPAD_BUTTON_START, 
    GAMEPAD_BUTTON_GUIDE,
    GAMEPAD_BUTTON_LEFT_THUMB, 
    GAMEPAD_BUTTON_RIGHT_THUMB,
    GAMEPAD_BUTTON_DPAD_UP, 
    GAMEPAD_BUTTON_DPAD_RIGHT, 
    GAMEPAD_BUTTON_DPAD_DOWN, 
    GAMEPAD_BUTTON_DPAD_LEFT,

    MOSS_GAMEPAD_BUTTON_LAST = GAMEPAD_BUTTON_DPAD_LEFT

    GAMEPAD_BUTTON_CROSS  =  GAMEPAD_BUTTON_A,
    GAMEPAD_BUTTON_CIRCLE =  GAMEPAD_BUTTON_B,
    GAMEPAD_BUTTON_SQUARE =  GAMEPAD_BUTTON_X,
    GAMEPAD_BUTTON_TRIANGLE = GAMEPAD_BUTTON_Y,
};

public enum Moss_Joystick {
    GAMEPAD_AXIS_LEFT_X,            // Left Stick X Axis
    GAMEPAD_AXIS_LEFT_Y,            // Left Stick Y Axis
    GAMEPAD_AXIS_RIGHT_X,           // Right Stick X Axis
    GAMEPAD_AXIS_RIGHT_Y,           // Right Stick Y Axis
    GAMEPAD_AXIS_LEFT_TRIGGER,      // Left Trigger
    GAMEPAD_AXIS_RIGHT_TRIGGER,     // Right Trigger
    GAMEPAD_AXIS_TOUCHPAD_X,        // (PS4/PS5)
    GAMEPAD_AXIS_TOUCHPAD_Y,
    GAMEPAD_AXIS_GYRO_X,
    GAMEPAD_AXIS_GYRO_Y,
    GAMEPAD_AXIS_GYRO_Z,
    MOSS_JOY_AXIS_LAST = GAMEPAD_AXIS_GYRO_Z
};

public enum Moss_WindowMode : int {
    WINDOWED = 0,
    MINIMIZED = 1,
    MAXIMIZED = 2,
    FULLSCREEN = 3,
    EXCLUSIVE_FULLSCREEN = 4
}

[StructLayout(LayoutKind.Explicit)]
public struct Moss_HapticEffect
{
    [FieldOffset(0)] public ushort type;
    [FieldOffset(0)] public Moss_HapticConstant constant;
    [FieldOffset(0)] public Moss_HapticPeriodic periodic;
    [FieldOffset(0)] public Moss_HapticRamp ramp;
}


[StructLayout(LayoutKind.Sequential)]
public struct Moss_Image { public int width; public int height; public IntPtr pixels; }

[StructLayout(LayoutKind.Sequential)]
public struct Moss_Locale() { public char language, country; }

[StructLayout(LayoutKind.Sequential)]
public struct GammaRamp { 
    public int size; 
    public IntPtr red,  green, blue; 
}
    
[StructLayout(LayoutKind.Sequential)]
public struct Moss_VideoMode {
    public int width, height, redBits, greenBits, blueBits, refreshRate;
}

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_Window Moss_CreateWindow(string title, int width, int height, Moss_Monitor monitor, Moss_Window share);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_TerminateWindow(Moss_Window window);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_ShouldWindowClose(Moss_Window window);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_PollEvents();


[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsKeyPressed(Moss_Keyboard key);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_GetMousePosition(out int x, out int y);


[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void Moss_WindowSizeCallback(int w, int h);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowSizeCallback(Moss_WindowSizeCallback callback);
