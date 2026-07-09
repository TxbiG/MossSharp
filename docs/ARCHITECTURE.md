# MossSharp Architecture

This document describes the overall architecture and design of MossSharp.

## Overview

MossSharp is a .NET binding layer over the native Moss Framework, implemented in C++. The architecture is designed to provide:

- **Type Safety**: Strong typing with C# managed code
- **Performance**: Direct P/Invoke to native code
- **Portability**: Abstraction over platform-specific implementations
- **Usability**: Idiomatic C# API design

## High-Level Architecture

```
┌────────────────────────────────────────────────────────────────┐
│   C# Application Code                   │
│   (Your Game/App)                       │
└────────────────────────┬─────────────────────────────────────┘
                   │
┌────────────────────────┴─────────────────────────────────────┐
│   MossSharp Managed API                 │
│   (Audio, Platform, Physics, etc.)      │
│   - Type-safe wrappers                  │
│   - Resource management                 │
│   - Error handling                      │
└────────────────────────┬──────────────────────────────────────┘
                   │
┌────────────────────────┴──────────────────────────────────────┐
│   P/Invoke Layer (Moss.Native)          │
│   - Native interop declarations         │
│   - Marshal data between managed/native │
│   - Runtime library loading             │
└────────────────────────┬────────────────────────────────────┘
                   │
┌────────────────────────┴───────────────────────────────────┐
│   Native Moss Libraries                 │
│   (C++ Implementation)                  │
│   - moss.dll / libmoss.so / libmoss.dylib
└─────────────────────────────────────────────────────────┘
```

## Directory Structure

```
src/
├── MossSharp/              # Main managed API
│   ├── Audio.cs           # Audio subsystem
│   ├── Platform.cs        # Platform/window management
│   ├── Physics.cs         # Physics engine
│   ├── Renderer.cs        # Graphics rendering
│   ├── GUI.cs             # User interface
│   ├── Navigation.cs      # Pathfinding
│   ├── Network.cs         # Networking
│   ├── XR.cs              # VR/AR support
│   ├── Utility.cs         # Utilities
│   └── Moss.cs            # Core types
│
├── Moss.Native/           # P/Invoke declarations
│   └── [Native interop]   # Platform-specific code
│
└── Moss.Native.Debug/     # Debug-only native bindings
```

## Core Design Patterns

### 1. Subsystem Initialization

Major subsystems follow a common initialization pattern:

```csharp
public static class AudioSystem
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized) return;
        
        int result = Native.Audio.MossAudioNative.Moss_Init_Audio();
        if (result != 0)
            throw new InvalidOperationException("Failed to initialize");
        
        initialized = true;
    }
    
    public static void Shutdown()
    {
        if (!initialized) return;
        Native.Audio.MossAudioNative.Moss_Terminate_Audio();
        initialized = false;
    }
}
```

### 2. Resource Management

Resources use `IDisposable` pattern for proper cleanup:

```csharp
public sealed class AudioStream : IDisposable
{
    internal IntPtr Handle { get; private set; }
    
    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            Native.Audio.MossAudioNative.Moss_AudioStreamRemove(Handle);
            Handle = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }
}
```

### 3. P/Invoke Interop

Native function declarations are isolated in the `Moss.Native` namespace:

```csharp
namespace Moss.Native.Audio
{
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Moss_Init_Audio();
    
    [DllImport("moss", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr Moss_AudioStreamCreate();
}
```

### 4. Error Handling

Errors are propagated through exceptions:

```csharp
IntPtr handle = Native.Audio.MossAudioNative.Moss_AudioStreamCreate();
if (handle == IntPtr.Zero)
    throw new InvalidOperationException("Failed to create audio stream");
```

## Subsystem Overview

### Audio (`Moss.Audio`)
- **Responsibility**: Manage audio playback, recording, and processing
- **Key Classes**: `AudioSystem`, `AudioStream`
- **Lifecycle**: Initialize → Use → Shutdown

### Platform (`Moss.Platform`)
- **Responsibility**: Window management, input handling, system resources
- **Key Classes**: `PlatformBuilder`, `Window`, `Input`
- **Lifecycle**: Builder pattern for initialization

### Physics (`Moss.Physics`)
- **Responsibility**: 2D/3D collision detection and dynamics
- **Scope**: Physics worlds, bodies, constraints, raycasting

### Renderer (`Moss.Renderer`)
- **Responsibility**: Graphics rendering pipeline
- **Features**: Textures, shaders, meshes, cameras, lighting, post-processing
- **Graphics APIs**: OpenGL, Vulkan, DirectX 12

### GUI (`Moss.GUI`)
- **Responsibility**: UI framework for building interactive interfaces
- **Components**: Containers, controls, layouts

### XR (`Moss.XR`)
- **Responsibility**: VR/AR device support and interaction
- **Features**: Device tracking, hand tracking, spatial input

### Navigation (`Moss.Navigation`)
- **Responsibility**: Pathfinding and AI navigation

### Networking (`Moss.Network`)
- **Responsibility**: Multiplayer networking
- **Features**: Client/server, RPC, synchronization

### Utility (`Moss.Utility`)
- **Responsibility**: Common data types and utilities
- **Types**: Vectors, matrices, quaternions, collections

## Native Interop Considerations

### Memory Management
- Native objects are managed through opaque `IntPtr` handles
- C# wrapper classes track ownership and lifetime
- Destructors fall back to finalizers for safety

### Data Marshaling
- Value types (vectors, matrices) are marshaled by value
- Reference types use `IntPtr` for native pointers
- Callbacks use delegates with appropriate calling conventions

### Platform-Specific Binaries
Native libraries are distributed through NuGet:
```
runtimes/
├── win-x64/native/moss.dll
├── linux-x64/native/libmoss.so
├── osx-x64/native/libmoss.dylib
└── osx-arm64/native/libmoss.dylib
```

## Threading Model

- **Main Thread**: Platform loop and rendering run on main thread
- **Audio**: Audio system may use worker threads
- **Physics**: Physics updates typically on main thread
- **Networking**: Network operations may use background threads

## Performance Considerations

### Managed to Native Calls
- P/Invoke calls have some overhead
- Batch operations when possible
- Avoid per-frame allocations

### Memory Layout
- Value types are stack-allocated
- Reference types use managed heap
- Native objects use unmanaged memory with pinned handles

### Garbage Collection
- Use `IDisposable` for deterministic cleanup
- Call `Dispose()` explicitly or use `using` statements
- Avoid finalizers when explicit disposal is available

## Future Architecture Improvements

- Source generators for P/Invoke declarations
- AOT (Ahead-of-Time) compilation support
- Job system for parallelization
- ECS (Entity Component System) framework integration
