# MossSharp Cheatsheet

> Quick reference for the idiomatic C# public API. The companion [API Reference](API_Reference.md) defines the complete feature map and all exported native-to-language projections. The public calls below are the target design, not a claim that the current binding has implemented every member yet.

## Language design at a glance

The public surface is a .NET library, not a P/Invoke header. Use PascalCase members, typed descriptor records, properties for stable state, events for notifications, exceptions for exceptional failures, and deterministic disposal.

## Subsystem map

| Moss subsystem | Public module / facade | Responsibility |
|---|---|---|
| Core | `Moss.Core` | Foundational value types, logging, memory, profiler, jobs, streams and utilities. |
| Platform | `Moss.Platform` | Windows/surfaces, monitors, events, input, gamepads, sensors, cameras, storage and dialogs. |
| GPU | `Moss.GPU` | Devices, buffers, textures, samplers, shaders, pipelines, command recording, swapchains and synchronization. |
| Renderer | `Moss.Renderer` | Frame lifecycle, materials, resource sets, framebuffers, meshes, models, fonts, sprites, cameras and debug draw. |
| Audio | `Moss.Audio` | Audio engine, sources, streams, channels, effects, listeners, acoustics, speaker and microphone capture. |
| GUI | `Moss.GUI` | Contexts, windows, containers, widgets, text/scalar input and drawing. |
| Physics | `Moss.Physics` | Bodies, shapes, filters, collisions, constraints, characters, vehicles, soft bodies and navigation integration. |
| Network | `Moss.Network` | Hosts, peers, packets, transports, HTTP/TLS, WebSocket and WebRTC where enabled. |
| XR | `Moss.XR` | Runtime/session lifecycle, views, actions, tracking, haptics, hand/body/face tracking and composition. |

## 1. Import Moss

```csharp
using Moss;
using Moss.Platform;
using Moss.Renderer;
using Moss.Audio;
using Moss.GPU;
using Moss.GUI;
```

## 2. Create a window and pump events

```csharp
using var window = Platform.CreateWindow(new WindowOptions
{
    Title = "Moss application",
    Width = 1280,
    Height = 720
});

while (!window.ShouldClose)
{
    Platform.PollEvents();
    // Update application state.
}

// `using` calls Window.Dispose() exactly once.
```

## 3. Create and upload a GPU buffer

```csharp
using var device = GpuDevice.Create(new GpuDeviceDesc());
using var buffer = device.CreateBuffer(new GpuBufferDesc
{
    Size = (nuint)vertices.Length,
    Usage = GpuBufferUsage.Vertex
});
buffer.Upload(vertices);

// The buffer is tied to its creating device and is disposed before it.
```

## 4. Render a frame

```csharp
using var renderer = Renderer.Create(window);
renderer.BeginFrame();
renderer.SetCamera(camera);
renderer.DrawSprite(sprite);
renderer.DrawText("Hello Moss", position, style);
renderer.EndFrame();
```

## 5. Play audio

```csharp
Audio.Initialize();
try
{
    using var source = Audio.LoadWav("assets/theme.wav");
    source.Play();
    Audio.Update(deltaTime);
}
finally
{
    Audio.Shutdown();
}
```

## 6. Build a GUI

```csharp
using var ui = GUI.CreateContext();
ui.Window("Inspector", panel =>
{
    panel.Text("Player");
    bool enabled = panel.Checkbox("Enabled", currentValue);
});
```

## 7. Simulate physics

```csharp
using var physics = PhysicsSystem.Create(settings);
using var body = physics.Bodies.Create(new BodyOptions
{
    Shape = Shape.Box(size),
    MotionType = MotionType.Dynamic
});
physics.Step(deltaTime);
var contacts = physics.Query.Contacts(body);
```

## 8. Networking

```csharp
using var host = Network.CreateHost(new HostOptions
{
    BindAddress = "0.0.0.0",
    PeerLimit = 32
});
foreach (var networkEvent in host.PollEvents())
{
    // Handle connect, receive and disconnect events.
}
```

## 9. XR session

```csharp
using var session = XR.CreateSession(renderer);
while (session.PollEvents())
{
    if (session.BeginFrame())
    {
        var views = session.LocateViews();
        // Render one view per active XR view.
        session.EndFrame();
    }
}
```

## 10. Common type families

| Category | Public types |
|---|---|
| Vectors | `Vec2`, `Vec3`, `Vec4`, float/double/integer variants |
| Matrices and transforms | `Mat22`, `Mat33`, `Mat44`, `DMat44`, `Quat`, `Basis` |
| Geometry | `AABB2`, `AABB3`, `OBB2`, `OBB3`, `Plane`, `Rect`, `Triangle` |
| Colour | `Color` with explicit component range and storage precision |
| Containers | language-native lists/maps/sets or typed Moss containers when allocator semantics matter |
| Results and references | target-language result/error types and explicit owned/borrowed resource wrappers |
| Jobs and synchronization | job system, thread pool, barrier, mutex, semaphore and scoped locks |

## 11. Resource-management rules

Every owned native resource is a sealed `IDisposable` class backed by `SafeHandle`. `Dispose()` releases it once; a finalizer is only a leak-safety net. Borrowed views are non-owning and must not expose `Dispose()`. Keep delegates rooted until callback unregistration has completed. Use `Span<T>`/`ReadOnlySpan<T>` for synchronous buffers; do not retain a span after the call.

## 12. Error-handling rules

Use `MossException` for failed resource creation, invalid state and native operation errors. Keep boolean query results as `bool`; a legitimate `false` is not an exception. Provide `TryCreate...` only for APIs where absence is an ordinary branch. Native entry points remain internal `LibraryImport`/`DllImport` declarations.

## 13. Feature and platform checks

- A declaration in the reference does not mean every backend implements it. Check the selected GPU backend, platform, XR runtime and optional networking/codec features.
- Keep GPU resources associated with the device that created them; release child resources before the device.
- Pair renderer frame begin/end, GUI begin/end, render-pass begin/end and XR frame begin/end operations. Prefer scoped/structured helpers so early returns cannot leave a subsystem unbalanced.
- Poll platform and network events at the cadence required by the subsystem. Do not block a browser/UI thread waiting for asynchronous I/O.
- Use the API Reference's subsystem and header inventory when looking for advanced Navigation, Geometry, Physics, Resources, Skeleton or Variants APIs.

## 14. Where to look next

See [API_Reference.md](API_Reference.md) for the full native symbol crosswalk, target-language signatures, resource models, threading rules, feature coverage and the complete 270-header / 19-inline-header inventory.
