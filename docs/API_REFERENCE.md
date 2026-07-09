# MossSharp API Reference

Detailed documentation of the MossSharp public API.

## Table of Contents

- [Audio](#audio)
- [Platform](#platform)
- [Physics](#physics)
- [Renderer](#renderer)
- [GUI](#gui)
- [XR](#xr)
- [Navigation](#navigation)
- [Networking](#networking)
- [Utility](#utility)

## Audio

Namespace: `Moss.Audio`

### AudioSystem

Manages the audio subsystem lifecycle.

```csharp
public static class AudioSystem
{
    /// <summary>Initialize the audio system.</summary>
    public static void Initialize();
    
    /// <summary>Shutdown the audio system.</summary>
    public static void Shutdown();
    
    /// <summary>Update audio processing (call each frame).</summary>
    /// <param name="deltaTime">Time elapsed since last frame in seconds</param>
    public static void Update(float deltaTime);
}
```

### AudioStream

Represents an audio playback stream.

```csharp
public sealed class AudioStream : IDisposable
{
    /// <summary>Create a new audio stream.</summary>
    public static AudioStream CreateStream();
    
    /// <summary>Start audio playback.</summary>
    public void Play();
    
    /// <summary>Stop audio playback.</summary>
    public void Stop();
    
    /// <summary>Set audio callback for real-time processing.</summary>
    /// <param name="onAudio">Callback receiving audio buffer</param>
    public void SetCallback(Action<float[]> onAudio);
    
    /// <summary>Distance model for 3D audio attenuation.</summary>
    public enum DistanceModel
    {
        Linear,      // Linear attenuation
        Inverse,     // Inverse square law
        Exponential  // Exponential attenuation
    }
}
```

## Platform

Namespace: `Moss.Platform`

### PlatformBuilder

Builder for creating platform instances.

```csharp
public class PlatformBuilder
{
    /// <summary>Build the platform instance.</summary>
    public IPlatform Build();
}
```

### IPlatform

Main platform interface.

```csharp
public interface IPlatform
{
    /// <summary>Check if the platform is running.</summary>
    bool IsRunning { get; }
    
    /// <summary>Update platform and process events.</summary>
    void Update();
}
```

## Physics

Namespace: `Moss.Physics`

The Physics module provides 2D and 3D physics simulation. Specific implementation details depend on native library functionality.

```csharp
public static class PhysicsSystem
{
    // Physics initialization and update methods
    // See native Moss Framework documentation for details
}
```

## Renderer

Namespace: `Moss.Renderer`

Handles graphics rendering with support for multiple graphics APIs.

```csharp
public static class RendererSystem
{
    // Renderer initialization and rendering methods
    // Supports OpenGL, Vulkan, and DirectX 12
    // See native documentation for specific features
}
```

## GUI

Namespace: `Moss.GUI`

User interface framework for building interactive layouts.

```csharp
public static class GUISystem
{
    // GUI initialization and control creation
    // Includes containers, buttons, text fields, etc.
}
```

## XR

Namespace: `Moss.XR`

Virtual and Augmented Reality support.

### XRSystem

```csharp
public static class XRSystem
{
    /// <summary>Initialize the XR subsystem.</summary>
    /// <param name="settings">XR initialization settings</param>
    public static void Initialize(XRInitSettings settings);
}
```

### XRInitSettings

```csharp
public struct XRInitSettings
{
    /// <summary>Renderer type (e.g., "OpenGL", "Vulkan", "DirectX12")</summary>
    public string Renderer { get; set; }
    
    /// <summary>Graphics device pointer</summary>
    public IntPtr GraphicsDevice { get; set; }
    
    /// <summary>Graphics context pointer</summary>
    public IntPtr GraphicsContext { get; set; }
}
```

## Navigation

Namespace: `Moss.Navigation`

Pathfinding and AI navigation utilities.

```csharp
public static class NavigationSystem
{
    // Navigation mesh generation and pathfinding
    // See native documentation for detailed API
}
```

## Networking

Namespace: `Moss.Network`

Multiplayer and networking capabilities.

```csharp
public static class NetworkSystem
{
    // Client/Server implementation
    // Remote Procedure Calls (RPC)
    // Network synchronization
    // See native documentation for details
}
```

## Utility

Namespace: `Moss.Utility`

Common utilities and data types.

### Vector Types

```csharp
// 2D Vectors
public struct Vec2 { public float x, y; }
public struct iVec2 { public int x, y; }
public struct uVec2 { public uint x, y; }
public struct bVec2 { public bool x, y; }

// 3D Vectors
public struct Vec3 { public float x, y, z; }
public struct iVec3 { public int x, y, z; }
public struct uVec3 { public uint x, y, z; }
public struct bVec3 { public bool x, y, z; }

// 4D Vectors
public struct Vec4 { public float x, y, z, w; }
public struct iVec4 { public int x, y, z, w; }
public struct uVec4 { public uint x, y, z, w; }
public struct bVec4 { public bool x, y, z, w; }

// Double precision
public struct Double2 { public double x, y; }
public struct Double3 { public double x, y, z; }
public struct Double4 { public double x, y, z, w; }
```

### Matrix Types

```csharp
public struct Mat2x2 { /* 2x2 matrix */ }
public struct Mat2x3 { /* 2x3 matrix */ }
public struct Mat2x4 { /* 2x4 matrix */ }
public struct Mat3x2 { /* 3x2 matrix */ }
public struct Mat3x3 { /* 3x3 matrix */ }
public struct Mat3x4 { /* 3x4 matrix */ }
public struct Mat4x2 { /* 4x2 matrix */ }
public struct Mat4x3 { /* 4x3 matrix */ }
public struct Mat4x4 { /* 4x4 matrix */ }
```

### Other Types

```csharp
public struct Quat { public float x, y, z, w; }  // Quaternion
public struct Basis { /* Rotation basis */ }     // 3D rotation matrix
public struct Color { public float r, g, b, a; } // RGBA color
public struct Rect { public float x, y, width, height; }
public struct iRect { public int x, y, width, height; }
public struct AABB2 { /* 2D bounding box */ }
public struct AABB3 { /* 3D bounding box */ }
```

### Collections

```csharp
public class TArray<T> { /* Dynamic array */ }
public class TMap<K, V> { /* Hash map */ }
public class TSet<T> { /* Hash set */ }
public class TStaticArray<T> { /* Fixed-size array */ }
public class TMultiMap<K, V> { /* Multi-value map */ }
public struct TPair<K, V> { public K Key; public V Value; }
```

### Tweening

```csharp
public static class TweenSystem
{
    // Tween animations for smooth transitions
}
```

### Noise

```csharp
public static class NoiseGenerator
{
    // Procedural noise generation (Perlin, Simplex, etc.)
}
```

## Common Patterns

### Using Statements

For safe resource cleanup, use `using` statements:

```csharp
using var stream = AudioStream.CreateStream();
stream.Play();
// Automatically disposed when leaving scope
```

### Error Handling

```csharp
try
{
    AudioSystem.Initialize();
    var stream = AudioStream.CreateStream();
    stream.Play();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Lifecycle Management

```csharp
// Initialize
AudioSystem.Initialize();

try
{
    // Use subsystem
    // ...
}
finally
{
    // Always cleanup
    AudioSystem.Shutdown();
}
```

## See Also

- [Getting Started](./GETTING_STARTED.md)
- [Architecture](./ARCHITECTURE.md)
- [Examples](./EXAMPLES.md)
- [Moss Framework Documentation](https://github.com/TxbiG/MossFramework)
