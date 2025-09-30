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

using System.Runtime.InteropServices;

namespace Moss {
    // Platform
    public class Window() { }
    public class Monitor() { }
    public class Image() { }
    public class Haptic() { }
    public class Events() { }


    public struct Moss_Locale() { }
    public struct Moss_VideoMode() { }
    public struct Moss_Image() { }
    public struct Moss_GammaRamp() { }


    // Audio
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Moss_Init_Audio() { }
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Moss_Terminate_Audio() { }
    public class AudioStream() { }
    public class AudioStream2D() { }
    public class AudioStream3D() { }

    public class Speaker() { }
    public class Microphone() { }

    public class AudioListener2D() { }
    public class AudioListener3D() { }

    public class RayAudioListener2D() { }
    public class RayAudioListener3D() { }



    // Variants
    class Vec2() { }
    class Vec3() { }
    class Vec4() { }

    class iVec2() { }
    class iVec3() { }
    class iVec4() { }

    class dVec2() { }
    class dVec3() { }
    class dVec4() { }

    class Float2() { }
    class Float3() { }
    class Float4() { }

    class Double2() { }
    class Double3() { }
    class Double4() { }

    class Timer() { }
    class Tween() { }

    // Resources



    // Renderer
    class Renderer() { }


    // Physics


    // Network

    // XR
    
    // Maths
}