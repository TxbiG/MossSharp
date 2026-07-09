# MossSharp Examples

Practical code examples demonstrating common MossSharp patterns and features.

## Table of Contents

- [Basic Setup](#basic-setup)
- [Audio Examples](#audio-examples)
- [Platform Examples](#platform-examples)
- [XR Examples](#xr-examples)
- [Physics Examples](#physics-examples)

## Basic Setup

### Minimal Application

```csharp
using Moss.Platform;

var platform = new PlatformBuilder().Build();

while (platform.IsRunning)
{
    platform.Update();
    // Game logic here
}
```

### Complete Application Template

```csharp
using Moss.Platform;
using Moss.Audio;
using Moss.Renderer;

class Program
{
    static void Main()
    {
        // Initialize subsystems
        var platform = new PlatformBuilder().Build();
        AudioSystem.Initialize();
        
        try
        {
            // Setup
            Console.WriteLine("Application initialized");
            
            // Main loop
            float frameTime = 0.016f; // ~60 FPS
            while (platform.IsRunning)
            {
                // Update
                platform.Update();
                AudioSystem.Update(frameTime);
                
                // Render
                // ...
            }
        }
        finally
        {
            // Cleanup
            AudioSystem.Shutdown();
        }
    }
}
```

## Audio Examples

### Playing an Audio Stream

```csharp
using Moss.Audio;

AudioSystem.Initialize();

try
{
    // Create and play audio
    using var audioStream = AudioStream.CreateStream();
    audioStream.Play();
    
    // Simulate playback
    System.Threading.Thread.Sleep(2000);
    
    audioStream.Stop();
}
finally
{
    AudioSystem.Shutdown();
}
```

### Audio with Callback

```csharp
using Moss.Audio;

AudioSystem.Initialize();

try
{
    var stream = AudioStream.CreateStream();
    
    // Set up audio processing callback
    stream.SetCallback(audioBuffer =>
    {
        // Process audio data
        for (int i = 0; i < audioBuffer.Length; i++)
        {
            // Apply effects, analyze, etc.
            audioBuffer[i] *= 0.5f; // Example: volume reduction
        }
    });
    
    stream.Play();
    
    // ... play for a duration ...
    
    stream.Stop();
}
finally
{
    AudioSystem.Shutdown();
}
```

### Multi-Stream Audio Management

```csharp
using Moss.Audio;
using System.Collections.Generic;

public class AudioManager
{
    private List<AudioStream> streams = new();
    
    public void Initialize()
    {
        AudioSystem.Initialize();
    }
    
    public AudioStream PlaySound()
    {
        var stream = AudioStream.CreateStream();
        stream.Play();
        streams.Add(stream);
        return stream;
    }
    
    public void Update(float deltaTime)
    {
        AudioSystem.Update(deltaTime);
    }
    
    public void Shutdown()
    {
        foreach (var stream in streams)
        {
            stream.Stop();
            stream.Dispose();
        }
        streams.Clear();
        AudioSystem.Shutdown();
    }
}
```

## Platform Examples

### Window Management

```csharp
using Moss.Platform;

var platform = new PlatformBuilder().Build();

// The platform handles window lifecycle
while (platform.IsRunning)
{
    platform.Update();
    
    // Process window events, input, etc.
}
```

### Input Handling Pattern

```csharp
using Moss.Platform;

public class InputManager
{
    private IPlatform platform;
    
    public InputManager(IPlatform platform)
    {
        this.platform = platform;
    }
    
    public void Update()
    {
        platform.Update();
        // Input events are processed here
    }
    
    public bool IsRunning => platform.IsRunning;
}
```

## XR Examples

### VR Initialization

```csharp
using Moss.XR;

// Initialize with OpenGL
var vrSettings = new XRInitSettings
{
    Renderer = "OpenGL",
    GraphicsDevice = IntPtr.Zero,
    GraphicsContext = IntPtr.Zero
};

try
{
    XRSystem.Initialize(vrSettings);
    Console.WriteLine("VR system initialized");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Failed to initialize VR: {ex.Message}");
}
```

### Vulkan VR Setup

```csharp
using Moss.XR;

var vrSettings = new XRInitSettings
{
    Renderer = "Vulkan",
    GraphicsDevice = GetVulkanDevice(),
    GraphicsContext = GetVulkanContext()
};

XRSystem.Initialize(vrSettings);
```

### DirectX 12 VR Setup

```csharp
using Moss.XR;

var vrSettings = new XRInitSettings
{
    Renderer = "DirectX12",
    GraphicsDevice = GetD3D12Device(),
    GraphicsContext = GetD3D12Context()
};

XRSystem.Initialize(vrSettings);
```

## Physics Examples

### Physics Simulation Pattern

```csharp
using Moss.Physics;
using Moss.Utility;

public class PhysicsScene
{
    private Vec3 gravity = new Vec3(0, -9.81f, 0);
    
    public void Update(float deltaTime)
    {
        // Physics update logic
        // Collision detection
        // Constraint solving
        // Body integration
    }
}
```

### Collision Detection Pattern

```csharp
public class CollisionHandler
{
    public void OnCollision(object bodyA, object bodyB)
    {
        // Handle collision between two bodies
        Console.WriteLine($"Collision between {bodyA} and {bodyB}");
    }
}
```

## Utility Examples

### Vector Operations

```csharp
using Moss.Utility;

// Vector creation
var pos2D = new Vec2 { x = 10, y = 20 };
var pos3D = new Vec3 { x = 1, y = 2, z = 3 };

// Integer vectors
var gridPos = new iVec2 { x = 5, y = 10 };

// Boolean vectors
var mask = new bVec3 { x = true, y = false, z = true };
```

### Matrix Operations

```csharp
using Moss.Utility;

// Create identity matrix
var identity = new Mat4x4();
// ... matrix operations ...
```

### Quaternion Usage

```csharp
using Moss.Utility;

// Rotation quaternion
var rotation = new Quat 
{ 
    x = 0, y = 0, z = 0.7071f, w = 0.7071f 
};
```

### Color Manipulation

```csharp
using Moss.Utility;

// Define colors
var red = new Color { r = 1.0f, g = 0.0f, b = 0.0f, a = 1.0f };
var transparent = new Color { r = 1.0f, g = 1.0f, b = 1.0f, a = 0.5f };
```

## Advanced Patterns

### Resource Pooling

```csharp
using Moss.Audio;
using System.Collections.Generic;

public class AudioStreamPool
{
    private Queue<AudioStream> available = new();
    private HashSet<AudioStream> inUse = new();
    
    public AudioStream Acquire()
    {
        AudioStream stream;
        if (available.TryDequeue(out stream))
        {
            inUse.Add(stream);
            return stream;
        }
        
        stream = AudioStream.CreateStream();
        inUse.Add(stream);
        return stream;
    }
    
    public void Release(AudioStream stream)
    {
        stream.Stop();
        inUse.Remove(stream);
        available.Enqueue(stream);
    }
    
    public void Clear()
    {
        foreach (var stream in available)
            stream.Dispose();
        foreach (var stream in inUse)
            stream.Dispose();
        available.Clear();
        inUse.Clear();
    }
}
```

### State Machine Pattern

```csharp
public abstract class GameState
{
    public abstract void Enter();
    public abstract void Update(float deltaTime);
    public abstract void Exit();
}

public class GameStateManager
{
    private GameState currentState;
    
    public void ChangeState(GameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
    public void Update(float deltaTime)
    {
        currentState?.Update(deltaTime);
    }
}
```

## Troubleshooting Examples

### Handling Initialization Failures

```csharp
using Moss.Audio;

try
{
    AudioSystem.Initialize();
}
catch (InvalidOperationException)
{
    Console.WriteLine("Audio not available on this platform");
    // Handle gracefully without audio
}
```

### Safe Resource Cleanup

```csharp
using Moss.Audio;

AudioStream stream = null;
try
{
    stream = AudioStream.CreateStream();
    stream.Play();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
}
finally
{
    stream?.Dispose();
}
```

## See Also

- [Getting Started](./GETTING_STARTED.md)
- [API Reference](./API_REFERENCE.md)
- [Architecture](./ARCHITECTURE.md)
