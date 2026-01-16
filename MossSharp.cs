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

public enum Moss_WindowMode : int {
    WINDOWED = 0,
    MINIMIZED = 1,
    MAXIMIZED = 2,
    FULLSCREEN = 3,
    EXCLUSIVE_FULLSCREEN = 4
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

    
namespace Moss {
    // Platform
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowHandle { public IntPtr Ptr; }
    [StructLayout(LayoutKind.Sequential)]
    public struct VideoMode { public int width, height, redBits, greenBits, blueBits, refreshRate;  }
    [StructLayout(LayoutKind.Sequential)]
    public class Image() { public int width; int height; unsigned char* pixels;}
    [StructLayout(LayoutKind.Sequential)]
    public class Haptic() { }
    [StructLayout(LayoutKind.Sequential)]
    public class Events() { }

    [StructLayout(LayoutKind.Sequential)]
    public struct Moss_Locale() { public char language, country; }
    [StructLayout(LayoutKind.Sequential)]
    public struct GammaRamp { public int size; public IntPtr red, green, blue; }

    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern WindowHandle Moss_CreateWindow([MarshalAs(UnmanagedType.LPStr)] string title, int width, int height);
    
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Moss_DestroyWindow(WindowHandle window);


    // Audio
    [StructLayout(LayoutKind.Sequential)]
    public class AudioStream() { }
    [StructLayout(LayoutKind.Sequential)]
    public class AudioStream2D() { }
    [StructLayout(LayoutKind.Sequential)]
    public class AudioStream3D() { }

    [StructLayout(LayoutKind.Sequential)]
    public class Speaker() { }
    [StructLayout(LayoutKind.Sequential)]
    public class Microphone() { }

    [StructLayout(LayoutKind.Sequential)]
    public class AudioListener2D() { }
    [StructLayout(LayoutKind.Sequential)]
    public class AudioListener3D() { }

    [StructLayout(LayoutKind.Sequential)]
    public class RayAudioListener2D() { }

    [StructLayout(LayoutKind.Sequential)]
    public class RayAudioListener3D() { }

    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Moss_Init_Audio();
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Moss_Terminate_Audio();



    // Variants
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2 { public float x, y; }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3 { public float x, y, z; }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4 { public float x, y, z, w; }

    [StructLayout(LayoutKind.Sequential)]
    class iVec2() { public int x, y; }
    [StructLayout(LayoutKind.Sequential)]
    class iVec3() { public int x, y, z; }
    [StructLayout(LayoutKind.Sequential)]
    class iVec4() { public int x, y, z, w; }

    [StructLayout(LayoutKind.Sequential)]
    class dVec2() { public double x, y; }
    [StructLayout(LayoutKind.Sequential)]
    class dVec3() { public double x, y, z; }
    [StructLayout(LayoutKind.Sequential)]
    class dVec4() { public double x, y, z, w; }

    [StructLayout(LayoutKind.Sequential)]
    class Float2() { public float x, y; }
    [StructLayout(LayoutKind.Sequential)]
    class Float3() { public float x, y, z; }
    [StructLayout(LayoutKind.Sequential)]
    class Float4() { public float x, y, z, w; }

    [StructLayout(LayoutKind.Sequential)]
    class Double2() { }
    [StructLayout(LayoutKind.Sequential)]
    class Double3() { }
    [StructLayout(LayoutKind.Sequential)]
    class Double4() { }

    [StructLayout(LayoutKind.Sequential)]
    class Timer() { }
    [StructLayout(LayoutKind.Sequential)]
    class Tween() { }

    // Resources



    // Renderer
    [StructLayout(LayoutKind.Sequential)]
    class Renderer() { }


    // Physics


    // Network

    // XR
    
    // Maths

}


