using System;
using System.Runtime.InteropServices;



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

enum class Moss_Gesture {
    NONE        = 0<<0,     // No gesture
    TAP         = 1<<0,     // Tap gesture
    DOUBLETAP   = 2<<0,     // Double tap gesture
    HOLD        = 3<<0,     // Hold gesture
    DRAG        = 4<<0,     // Drag gesture
    SWIPE_RIGHT = 5<<0,     // Swipe right gesture
    SWIPE_LEFT  = 6<<0,     // Swipe left gesture
    SWIPE_UP    = 7<<0,     // Swipe up gesture
    SWIPE_DOWN  = 8<<0,     // Swipe down gesture
    PINCH_IN    = 9<<0,     // Pinch in gesture
    PINCH_OUT   = 10<<0     // Pinch out gesture
};

// Cursor
enum class Moss_CursorMode {
    VISIBLE = 0,             //
    HIDDEN = 0,              //
    CAPTURED = 0,            //
    CONFINED = 0,            //
    CONFINED_HIDDEN = 0,     //
};

enum class Moss_CursorShape {
    ARROW = 0,           //
    IBEAM = 1,           //
    POINTING_HAND = 2,   //
    CROSS = 3,           //
    WAIT = 4,            //
    BUSY = 5,            //
    DRAG = 6,            //
    CAN_DROP = 7,        //
    FORBIDDEN = 8,       //
    VSIZE = 9,           //
    HSIZE = 10,          //
    BDIAGSIZE = 11,      //
    FDIAGSIZE = 12,      //
    MOVE = 13,           //
    VSPLIT = 14,         //
    HSPLIT = 15,         //
    HELP = 16            //
};


enum class Moss_HapticFeedbackType {
    AUTOCENTER,
    CARTESIAN,
    CONSTANT,
    CUSTOM,
    DAMPER,
    FRICTION,
    GAIN,
    INERTIA,
    INFINITY,
    LEFTRIGHT,
    PAUSE,
    POLAR,
    RAMP,
    RESERVED1,
    RESERVED2,
    RESERVED3,
    SAWTOOTHDOWN,
    SAWTOOTHUP,
    SINE,
    SPHERICAL,
    SPRING,
    SQUARE,
    STATUS,
    STEERING_AXIS,
    TRIANGLE
}


enum class Moss_HapticEffectType {
    HAPTIC_CONSTANT,
    HAPTIC_PERIODIC,
    HAPTIC_CONDITION,
    HAPTIC_RAMP,
    HAPTIC_LEFTRIGHT,
    HAPTIC_CUSTOM 
};



enum class Moss_PlatformTheme { UKNOWN = 0, LIGHT, DARK };

enum class Moss_PowerState : int8_t {
    ERROR,   // error determining power status */
    UNKNOWN,      // cannot determine power status */
    ON_BATTERY,   // Not plugged in, running on the battery */
    NO_BATTERY,   // Plugged in, no battery available */
    CHARGING,     // Plugged in, charging battery */
    CHARGED       // Plugged in, battery charged */
};

enum class Moss_FileDialogType {
    OPENFILE,
    SAVEFILE,
    OPENFOLDER
};

enum class Moss_PenAxis {
    PRESSURE,  // Pen pressure.  Unidirectional: 0 to 1.0 */
    XTILT,     // Pen horizontal tilt angle.  Bidirectional: -90.0 to 90.0 (left-to-right). */
    YTILT,     // Pen vertical tilt angle.  Bidirectional: -90.0 to 90.0 (top-to-down). */
    DISTANCE,  // Pen distance to drawing surface.  Unidirectional: 0.0 to 1.0 */
    ROTATION,  // Pen barrel rotation.  Bidirectional: -180 to 179.9 (clockwise, 0 is facing up, -180.0 is facing down). */
    SLIDER,    // Pen finger wheel or slider (e.g., Airbrush Pen).  Unidirectional: 0 to 1.0 */
    TANGENTIAL_PRESSURE,    // Pressure from squeezing the pen ("barrel pressure"). */
    COUNT       // Total known pen axis types in this version of Moss. This number may grow in future releases! */
};

enum class Moss_PenDeviceType {
    INVALID = -1, // Not a valid pen device. */
    UNKNOWN,      // Don't know specifics of this pen. */
    DIRECT,       // Pen touches display. */
    INDIRECT      // Pen touches something that isn't the display. */
};
enum class Moss_TouchDeviceType {
    INVALID = -1,
    DIRECT,            // touch screen with window-relative coordinates */
    INDIRECT_ABSOLUTE, // trackpad with absolute device coordinates */
    INDIRECT_RELATIVE  // trackpad with screen cursor-relative coordinates */
};

enum class Moss_JoystickConnectionState {
    INVALID = -1,
    UNKNOWN,
    WIRED,
    WIRELESS
};


enum class Moss_UserFolder {
    HOME,
    DESKTOP,
    DOCUMENTS,
    DOWNLOADS,
    PICTURES,
    MUSIC,
    VIDEOS,
    APPDATA,
    CACHE
};



enum class Moss_MessageBoxFlags {
    ABORT_ENTRY_IGNORE = 0,  // The message box contains three push buttons: Abort, Retry, and Ignore.
    CANCEL_TRY_CONTINUE = 1, // The message box contains three push buttons: Cancel, Try Again, Continue.
    HELP = 3,              // Adds a Help button to the message box.
    OK = 4,                // The message box contains one push button: OK. This is the default.
    OK_CANCEL = 5,          // The message box contains two push buttons: OK and Cancel.
    RETRY_CANCEL = 6,       // The message box contains two push buttons: Retry and Cancel.
    YES_NO = 7,             // The message box contains two push buttons: Yes and No.
    YES_NO_CANCEL = 8,       // The message box contains three push buttons: Yes, No, and Cancel.
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

struct Moss_Locale {
    public char* country, language;
};

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
/*! @brief X. @param X X @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowTitle(Moss_Window* window, const char* title);
/*! @brief X. @param X X @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowIcon(Moss_Window* window, Moss_Image image);
/*! @brief This is to call for closing a window. @param Moss_Window* window @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_CloseWindow(Moss_Window* window);

public static extern bool Moss_CreateMessageBox(const char* title, const char* message, Moss_MessageBoxFlags flags, Moss_Window* window);


/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API Moss_Monitor* Moss_MonitorGetPrimary();
/*! @brief X. @param X X @ingroup Monitor @returns returns monitor unless if theres not a second will return nullptr. */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API Moss_Monitor* Moss_MonitorGetSecondary();
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API void Moss_MonitorGetPhysicalSize(Moss_Monitor monitor, int* width_mm, int* height_mm);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API void Moss_MonitorGetContentScale(Moss_Monitor monitor, float* xscale, float* yscale);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API void Moss_MonitorGetPosition(Moss_Monitor monitor, int* x, int* y);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API const char* Moss_MonitorGetName(Moss_Monitor monitor);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API void Moss_MonitorSetGammaRamp(Moss_Monitor monitor, const Moss_GammaRamp* gammaRamp);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API Moss_GammaRamp* Moss_MonitorGetGammaRamp(Moss_Monitor monitor);
/*! @brief X. @param X X @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
MOSS_API void Moss_MonitorSetGamma(Moss_Monitor monitor, float gamma);



[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsKeyPressed(Moss_Keyboard key);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsKeyJustPressed(Moss_Key key);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsKeyJustReleased(Moss_Key key);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_Keyboard Moss_InputGetKey();
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsMousePressed(Moss_MouseButton button);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsMouseJustPressed(Moss_MouseButton button);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern bool Moss_IsMouseJustReleased(Moss_MouseButton button);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_Keyboard Moss_InputGetMouse();
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_GetMousePosition(int x, int y);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetMousePosition(int x, int y);
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetMouseVisible(bool visible);

// Gamepad management
public static extern int Moss_GetNumGamepads(void);
public static extern Moss_Gamepad* Moss_OpenGamepad(Moss_GamepadID id);
public static extern void Moss_CloseGamepad(Moss_Gamepad* gp);
public static extern bool Moss_GamepadConnected(Moss_Gamepad* gp);
public static extern void Moss_UpdateGamepads(void); // poll / refresh all gamepads

// Button & axis
public static extern bool Moss_IsGamepadButtonPressed(Moss_Gamepad* gp, Moss_GamepadButton button);
public static extern bool Moss_IsGamepadButtonJustPressed(Moss_Gamepad* gp, Moss_GamepadButton button);
public static extern bool Moss_IsGamepadButtonJustReleased(Moss_Gamepad* gp, Moss_GamepadButton button);
public static extern float Moss_GetGamepadAxis(Moss_Gamepad* gp, Moss_Joystick axis);

// Rumble / LED
public static extern bool Moss_RumbleGamepad(Moss_Gamepad* gp, uint16_t low, uint16_t high, uint32_t duration_ms);
public static extern bool Moss_RumbleGamepadTriggers(Moss_Gamepad* gp, uint16_t left, uint16_t right, uint32_t duration_ms);
public static extern bool Moss_SetGamepadLED(Moss_Gamepad* gp, uint8_t r, uint8_t g, uint8_t b);

// Metadata
public static extern const char* Moss_GetGamepadName(Moss_Gamepad* gp);
public static extern Moss_GamepadID Moss_GetGamepadID(Moss_Gamepad* gp);
public static extern int Moss_GetGamepadPlayerIndex(Moss_Gamepad* gp);
public static extern Moss_PowerState Moss_GetGamepadPowerInfo(Moss_Gamepad* gp, int* percent);
public static extern int Moss_GetNumGamepadTouchpads(Moss_Gamepad* gp);
public static extern int Moss_GetNumGamepadTouchpadFingers(Moss_Gamepad* gp);
public static extern bool Moss_GetGamepadTouchpadFinger(Moss_Gamepad* gp, int pad, int finger, bool* down, float* x, float* y, float* pressure);

// Mapping & type
public static extern const char* Moss_GetGamepadMapping(Moss_Gamepad* gp);
public static extern bool Moss_SetGamepadMapping(Moss_Gamepad* gp, const char* mapping);
Mpublic static extern void Moss_ReloadGamepadMappings(void);

public static extern Moss_GamepadButton Moss_InputGetGamepadButton();
public static extern Moss_GamepadAxis Moss_InputGetGamepadAxis();




/*              CPU Info          */
/*! @brief Get the number of logical CPU cores available. @ingroup Platform Misc. */
public static extern int Moss_GetAvailableCPUCores(void);
/*! @brief Determine the L1 cache line size of the CPU. @ingroup Platform Misc. */
public static extern int Moss_GetCPUCacheLineSize(void);
/*! @brief Get the amount of RAM configured in the system. @ingroup Platform Misc. */
public static extern int Moss_GetSystemRAM(void);


/*             OS Spesific        */
/*! @brief URL to a website link. Supported on PC and mobile. @param url URL link. @ingroup Platform Misc. */
public static extern bool Moss_OpenURL(const char *url);
/*! @brief X. @param X X @ingroup Platform Misc. */
public static extern Moss_PlatformTheme Moss_GetPlatformTheme();
/*! @brief Get Locale of the Operating system. @return Country "UK" for United Kingdom and language "en" for English. @ingroup Platform Misc. */
public static extern Moss_Locale* Moss_GetLocale();
/*! @brief */
public static extern Moss_PowerState Moss_GetPowerInfo(int *seconds, int *percent);
/*! @brief Detct if an executible is running. @param executable_path exe name - e.g. obs.exe. */
public static extern bool Moss_IsProcessRunningByName(const char* executable_path);
/*! @brief Loads a dynamic library from the given path. @param lib_path Path to the library file. @return Handle to the loaded library, or NULL on failure. @note The returned handle must be released using Moss_UnloadDynamicLibrary(). @ingroup Platform Misc.*/
public static extern void* Moss_LoadDynamicLibrary(const char* lib_path);
/*! @brief Retrieves a symbol from a loaded library. @param handle Handle to the loaded library. @param symbol_name Name of the symbol to retrieve. @return Pointer to the symbol, or NULL if not found.  @return A pointer to the requested symbol, or NULL if not found.  @warning The returned pointer must be cast to the appropriate function or data type. @ingroup Platform Misc.*/
public static extern void* Moss_GetLibrarySymbol(void* handle, const char* symbol_name);
/*! @brief Unloads a previously loaded dynamic library. @param handle Handle to the library to unload. @note After unloading, the handle should not be used again. @ingroup Platform Misc. */
public static extern void Moss_UnloadDynamicLibrary(void* handle);





//! @brief Callback for framebuffer resize events. @param width  New framebuffer width, in pixels. @param height New framebuffer height, in pixels.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_FramebufferResizeCallback)(int width, int height);
//! @brief Callback for logical window size changes. @param width  New window width, in screen coordinates. @param height New window height, in screen coordinates.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_WindowSizeCallback)(int width, int height);
//! @brief Callback for window position changes on screen. @param xpos New X coordinate of the window’s top-left corner. @param ypos New Y coordinate of the window’s top-left corner.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_WindowPositionCallback)(int xpos, int ypos);
//! @brief Callback for window focus events. @param focused True if the window gained focus; false if it lost focus.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_WindowFocusCallback)(bool focused);
//! @brief Callback for content scale changes (e.g., HiDPI scaling). @param xscale X-axis content scale factor. @param yscale Y-axis content scale factor.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_WindowContentScaleCallback)(float xscale, float yscale);
//! @brief Callback for general window resize notifications (platform-driven). @param width  New window width in pixels. @param height New window height in pixels.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_WindowResizeCallback)(int width, int height);
//! @brief Callback for monitor configuration changes (e.g. hotplug events). @param monitorName Name or ID of the monitor that changed. @param connected True if the monitor was connected; false if disconnected.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (*Moss_MonitorCallback)(const char* monitorName, bool connected);
//! @brief X. @param width X. @param X. */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef void (MOSS_CALL* Moss_DialogFileCallback)(void *userdata, const char * const *filelist, int filter);
//! @brief X. @param width X. @param X. */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
typedef bool (*Moss_DirectoryIterateFn)(const Moss_PathInfo* info, const char* path, void* user_data);


/*! @brief Sets the framebuffer resize callback. @param callback Pointer to a function to be invoked when framebuffer size changes. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetFramebufferResizeCallback(Moss_FramebufferResizeCallback callback);
/*! @brief Sets the window size callback. @param callback Pointer to a function invoked when the window size changes. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowSizeCallback(Moss_WindowSizeCallback callback);
/*! @brief Sets the window resize callback (platform-level, e.g. minimize/maximize). @param callback Pointer to a function invoked when window resizing events occur. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowResizeCallback(Moss_WindowResizeCallback callback);
/*! @brief Sets the window position callback. @param callback Pointer to a function invoked when the window position changes. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowPositionCallback(Moss_WindowPositionCallback callback);
/*! @brief Sets the window focus callback. @param callback Pointer to a function invoked when the window focus changes. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowFocusCallback(Moss_WindowFocusCallback callback);
/*! @brief Sets the window content scale callback (for HiDPI / Retina support). @param callback Pointer to a function invoked when the content scale changes. @ingroup Window */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetWindowContentScaleCallback(Moss_WindowContentScaleCallback callback);
/*! @brief Sets the monitor connection or configuration callback. @param callback Pointer to a function invoked when a monitor is connected or disconnected. @ingroup Monitor */
[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_SetMonitorCallback(Moss_MonitorCallback callback);




// OpenGL / OpenGL-ES
#if defined(MOSS_GRAPHICS_OPENGL) || defined(MOSS_GRAPHICS_OPENGLES)
/*! @brief Sets window as current.  @param X X. @ingroup window */
public static extern void Moss_MakeContextCurrent(Moss_Window* window);
/*! @brief Swapbuffers used to swapbuffers for opengl. @ingroup window */
public static extern void Moss_SwapBuffers();
/*! @brief Swapbuffers but with a delay in seconds. @param X X. @ingroup window. */
public static extern void Moss_SwapBuffersInterval(int interval);
/*! @brief X. @param X X. @ingroup window. */
public static extern void* Moss_GetProcAddress(const char* procname);
#endif // MOSS_USE_OPENGL

// Vulkan
#if defined(MOSS_GRAPHICS_VULKAN)
/*! @brief Creates the window for vulkan @param X X @ingroup Vulkan. */
public static extern VkResult Moss_CreateWindowSurface(Moss_Window* window, VkInstance vk_instance, const VkAllocationCallbacks *allocator, VkSurfaceKHR* vk_surface);
/*! @brief X @param X X @ingroup Vulkan. */
public static extern int Moss_VulkanSupported(void);
/*! @brief X @param X X @ingroup Vulkan. */
//MOSS_API void Moss_InitVulkanLoader(PFN_vkGetInstanceProcAddr loader);
/*! @brief X @param X X @ingroup Vulkan. */
public static extern const char** Moss_GetRequiredInstanceExtensions(uint32_t* count);
/*! @brief X @param X X @ingroup Vulkan. */
public static extern void* Moss_GetInstanceProcAddress(VkInstance instance, const char* procname);
/*! @brief X @param X X @ingroup Vulkan. */
public static extern int Moss_GetPhysicalDevicePresentationSupport(Moss_Window* window, VkPhysicalDevice device, uint32_t queuefamily);
#endif // MOSS_USE_VULKAN

