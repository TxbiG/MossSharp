# MossSharp API Reference

> **Complete-parity public API design for C#.** This document specifies the intended idiomatic API when the binding exposes the full Moss feature set. It is a language-facing design contract, not a statement that every projected member is implemented in the current wrapper.

## 1. API design principles

Moss C++/CMoss is the canonical feature and behaviour baseline. The language binding is a **native C# library**, not a mechanical translation of C++ identifiers. The FFI may use C-compatible handles and exported `Moss_*` symbols internally, but ordinary users should not see those details.

The public surface is a .NET library, not a P/Invoke header. Use PascalCase members, typed descriptor records, properties for stable state, events for notifications, exceptions for exceptional failures, and deterministic disposal.

The binding must preserve feature coverage and semantics, not the number or spelling of native functions. Several native calls may become one managed method, a property, an iterator, a scoped object or a language protocol. Conversely, an overloaded native function may become distinct named methods when the language does not support overloads cleanly.

### Non-negotiable rules

- Keep the complete native feature set: Core, Platform, GPU, Renderer, Audio, GUI, Physics, Networking, XR, Navigation, Geometry, Resources, Skeleton, TriangleSplitter and Variants.
- Expose resource objects rather than asking users to pass opaque pointers repeatedly.
- Hide allocator plumbing, user-data pointers, C calling conventions, struct packing and raw WASM/JNI/P/Invoke handles behind the binding layer.
- Preserve status-query meaning. `false`, `0`, an empty collection and an absent optional value are not interchangeable with an error.
- Use asynchronous operations only when the target runtime requires them. WebAssembly startup and browser fetches are asynchronous; ordinary native API calls are not made asynchronous merely for stylistic consistency.
- Document platform/backend feature gates. A declared API is not proof that a backend or optional transport is available in a particular build.

## 2. Package and module layout

```csharp
using Moss;
using Moss.Platform;
using Moss.Renderer;
using Moss.Audio;
using Moss.GPU;
using Moss.GUI;
```

### Subsystem map

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

The public package/module root should follow the repository's established package metadata. The names in this document are the recommended public design; implementation namespaces can be adjusted to the actual package identifier without changing the member design.

## 3. Core language type system

| Native Moss concept | C# public representation | Design requirement |
|---|---|---|
| `bool` | `bool` | Keep boolean status and query values boolean. |
| Fixed-width signed/unsigned integers | `sbyte/byte, short/ushort, int/uint, long/ulong` | Preserve width and unsigned semantics at the ABI boundary. |
| `size_t` | `nuint` | Must be large enough for the host architecture. |
| `float` / `double` | `float / double` | Do not narrow double precision silently. |
| UTF-8 text | `string` | Copy or borrow according to documented native lifetime; validate encoding. |
| Opaque native resource | `SafeHandle-backed IDisposable class` | Never expose raw pointers as normal user-facing values. |
| Pointer + element count | `Span<T> / ReadOnlySpan<T>` | Validate length, element type and retention duration. |
| Function pointer + user data | `delegate with rooted GCHandle` | Unregister before callback state is released. |

### Value types and descriptors

Vectors, matrices, quaternions, colours, bounds, planes, rectangles and descriptor types should be real C# values. Use value structs/records where copy semantics are cheap and safe; use resource classes only for objects with identity or native lifetime. Descriptor objects should have named fields/options instead of long sequences of positional primitives. Validate invalid enum combinations before crossing FFI.

## 4. Public resource model

| Moss native resource family | Recommended public type | Ownership model |
|---|---|---|
| Window / monitor / gamepad / camera / capture / storage | `Window`, `Monitor`, `Gamepad`, `Camera`, `Capture`, `Storage` | The application owns platform state; each opened resource has explicit close/dispose semantics. |
| GPU device / buffer / texture / sampler / shader / pipeline / command buffer / fence / semaphore | `GpuDevice`, `GpuBuffer`, `GpuTexture`, `GpuSampler`, `GpuShader`, `GpuPipeline`, `GpuCommandBuffer`, `GpuFence`, `GpuSemaphore` | Child objects retain or borrow their creating device; destroy children before device shutdown. |
| Renderer / material / framebuffer / mesh / model / font / sprite / batch | `Renderer`, `Material`, `Framebuffer`, `Mesh`, `Model`, `Font`, `Sprite`, `SpriteBatch` | Renderer-owned resources cannot outlive the renderer unless explicitly reference-counted. |
| Audio source / stream / listener / effect / microphone | `AudioSource`, `AudioStream`, `AudioListener`, `AudioEffect`, `Microphone` | Release sources, streams and listeners before audio shutdown. |
| GUI context | `GuiContext` | Context scopes widget calls; begin/end pairs are balanced by scoped helpers or callbacks. |
| Physics system / body / shape / constraint / character / vehicle / soft body | `PhysicsSystem`, `Body`, `Shape`, `Constraint`, `Character`, `Vehicle`, `SoftBody` | System and job/allocator dependencies outlive bodies and simulation work. |
| Network host / peer / packet / HTTP request / WebSocket / WebRTC | `NetworkHost`, `Peer`, `Packet`, `HttpRequest`, `WebSocket`, `WebRtcConnection` | Explicit ownership of hosts, packets, requests and connections. |
| XR instance / session / action set / action / hand / body / face | `XrRuntime`, `XrSession`, `XrActionSet`, `XrAction`, `HandTracker`, `BodyTracker`, `FaceTracker` | Session-bound objects cannot outlive the session/runtime they depend on. |

The exact implementation may consolidate thin native objects where that is safer (for example, a renderer may own framebuffers or a device may own command pools). The public API must still make ownership unambiguous.

## 5. Lifecycle, errors, callbacks and concurrency

### Resource lifetime

Every owned native resource is a sealed `IDisposable` class backed by `SafeHandle`. `Dispose()` releases it once; a finalizer is only a leak-safety net. Borrowed views are non-owning and must not expose `Dispose()`. Keep delegates rooted until callback unregistration has completed. Use `Span<T>`/`ReadOnlySpan<T>` for synchronous buffers; do not retain a span after the call.

### Errors and results

Use `MossException` for failed resource creation, invalid state and native operation errors. Keep boolean query results as `bool`; a legitimate `false` is not an exception. Provide `TryCreate...` only for APIs where absence is an ordinary branch. Native entry points remain internal `LibraryImport`/`DllImport` declarations.

### Threading and callbacks

- State which thread owns window/event pumping, renderer submission, audio callbacks, physics updates and XR frame operations.
- Do not promise thread safety unless the native subsystem guarantees it.
- Keep callback state alive until native unregistration completes; no callback may run after disposal/close returns.
- Never allow a managed exception, panic or language-runtime longjmp to unwind through a C ABI frame.
- For asynchronous APIs, define cancellation, completion and disposal behaviour. Do not return a view over memory that is freed when the Promise/callback completes.

## 6. Idiomatic quick-start patterns

### Platform and event loop

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

### GPU resource creation

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

### Renderer frame

```csharp
using var renderer = Renderer.Create(window);
renderer.BeginFrame();
renderer.SetCamera(camera);
renderer.DrawSprite(sprite);
renderer.DrawText("Hello Moss", position, style);
renderer.EndFrame();
```

### Audio playback

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

### GUI context and widgets

```csharp
using var ui = GUI.CreateContext();
ui.Window("Inspector", panel =>
{
    panel.Text("Player");
    bool enabled = panel.Checkbox("Enabled", currentValue);
});
```

### Physics world

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

### Networking

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

### XR session

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

The examples show the intended public shape and lifecycle rules. Names and overloads in the index below are the parity mapping target; implementation-specific initialization parameters can be added to the relevant options objects without exposing the FFI.

## 7. Complete public API projection index

The following table maps every exported callable/allocator callback in the supplied CMoss declaration catalogue to a proposed C# public entry point. The native name is included **only for traceability**. It is not the public name that developers should call. Target signatures use idiomatic types and hide receiver/context handles where those belong to an object.

- **1063 indexed entries:** 1,058 callable/constructor declarations and five allocator callback values.
- Duplicate native overloads are retained as separate rows where their parameters differ; the public API should resolve each to a distinct overload or a disambiguated method name.
- Native pointer/out-parameter patterns should become owned resource values, spans/slices, tuples/records or callback objects as appropriate to the language.

### `Moss_Audio.h` → `Moss.Audio`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_Init_Audio` | `Audio.Initialize(...)` | `Audio.Initialize() : void` | function |
| `Moss_Terminate_Audio` | `Audio.Shutdown(...)` | `Audio.Shutdown() : void` | function |
| `Moss_AudioUpdate` | `Audio.Update(...)` | `Audio.Update(float deltaTime) : void` | function |
| `Moss_AudioLoadWavFile` | `Audio.LoadWavFile(...)` | `Audio.LoadWavFile(string path) : AudioSource` | function |
| `Moss_AudioLoadWav` | `Audio.LoadWav(...)` | `Audio.LoadWav() : AudioSource` | function |
| `Moss_AudioLoadOgg` | `Audio.LoadOgg(...)` | `Audio.LoadOgg(string filename, AudioLoadType type) : AudioSource` | function |
| `Moss_AudioLoadMP3` | `Audio.LoadMp3(...)` | `Audio.LoadMp3(string filename) : AudioSource` | function |
| `Moss_AudioSourceDestroy` | `AudioSource.Dispose(...)` | `AudioSource.Dispose() : void` | function |
| `Moss_AudioCaptureMicrophone` | `Audio.CaptureMicrophone(...)` | `public static AudioSource Audio.CaptureMicrophone(Microphone microphone)` | function |
| `Moss_AudioCreateEffect` (overload 1) | `Audio.CreateEffect(...)` | `Audio.CreateEffect(AudioEffectType type) : AudioEffect` | function |
| `Moss_AudioCreateEffect` (overload 2) | `AudioEffect.SetParameter(...)` | `AudioEffect.SetParameter(string paramName, float value) : void` | function |
| `Moss_AudioRemoveEffect` | `AudioEffect.Dispose(...)` | `AudioEffect.Dispose() : void` | function |
| `Moss_AudioCreateChannel` | `Audio.CreateChannel(...)` | `Audio.CreateChannel(ChannelId channel) : ChannelId` | function |
| `Audio_RemoveChannel` | `Audio.RemoveChannel(...)` | `Audio.RemoveChannel(ChannelId channel) : void` | function |
| `Moss_AudioGetMasterChannel` | `Audio.GetMasterChannel(...)` | `Audio.GetMasterChannel() : ChannelId` | function |
| `Moss_AudioSetChannelVolume` | `Audio.SetChannelVolume(...)` | `Audio.SetChannelVolume(ChannelId channel, float volume) : void` | function |
| `Moss_AudioSetChannelMute` | `Audio.SetChannelMute(...)` | `Audio.SetChannelMute(ChannelId channel, bool mute) : void` | function |
| `Moss_AudioAddChannelEffect` | `Audio.AddEffectToChannel(...)` | `Audio.AddEffectToChannel(ChannelId channel, AudioEffect effect) : void` | function |
| `Moss_AudioRemoveChannelEffect` | `Audio.RemoveEffectFromChannel(...)` | `Audio.RemoveEffectFromChannel(ChannelId channel, AudioEffect effect) : void` | function |
| `Moss_AudioRemoveAllChannelEffects` | `Audio.ClearChannelEffects(...)` | `Audio.ClearChannelEffects(ChannelId channel) : void` | function |
| `Moss_AudioStreamCreate` | `Audio.CreateStream(...)` | `Audio.CreateStream() : AudioStream` | function |
| `Moss_AudioStreamPlay` | `AudioStream.Play(...)` | `AudioStream.Play() : void` | function |
| `Moss_AudioStreamStop` | `AudioStream.Stop(...)` | `AudioStream.Stop() : void` | function |
| `Moss_AudioStreamSetVolume` | `AudioStream.SetVolume(...)` | `AudioStream.SetVolume(float volume) : void` | function |
| `Moss_AudioStreamSetPitch` | `AudioStream.SetPitch(...)` | `AudioStream.SetPitch(float pitch) : void` | function |
| `Moss_AudioStreamSetPlaybackRate` | `AudioStream.SetPlaybackRate(...)` | `AudioStream.SetPlaybackRate(float rate) : void` | function |
| `Moss_AudioStreamSetPan` | `AudioStream.SetPan(...)` | `AudioStream.SetPan(float pan) : void` | function |
| `Moss_AudioStreamSetLoop` | `AudioStream.SetLoop(...)` | `AudioStream.SetLoop(bool loop) : void` | function |
| `Moss_AudioStreamRemove` | `AudioStream.Dispose(...)` | `AudioStream.Dispose() : void` | function |
| `Moss_AudioStream2DCreate` | `Audio.CreateStreamDimension2D(...)` | `Audio.CreateStreamDimension2D() : AudioStreamDIMENSION2D` | function |
| `Moss_AudioStream2DPlay` | `AudioStreamDIMENSION2D.Play(...)` | `AudioStreamDIMENSION2D.Play() : void` | function |
| `Moss_AudioStream2DStop` | `AudioStreamDIMENSION2D.Stop(...)` | `AudioStreamDIMENSION2D.Stop() : void` | function |
| `Moss_AudioStream2DSetVolume` | `AudioStreamDIMENSION2D.SetVolume(...)` | `AudioStreamDIMENSION2D.SetVolume(float volume) : void` | function |
| `Moss_AudioStream2DSetPitch` | `AudioStreamDIMENSION2D.SetPitch(...)` | `AudioStreamDIMENSION2D.SetPitch(float pitch) : void` | function |
| `Moss_AudioStream2DSetPan` | `AudioStreamDIMENSION2D.SetPan(...)` | `AudioStreamDIMENSION2D.SetPan(float pan) : void` | function |
| `Moss_AudioStream2DSetLoop` | `AudioStreamDIMENSION2D.SetLoop(...)` | `AudioStreamDIMENSION2D.SetLoop(bool loop) : void` | function |
| `Moss_AudioStream3DSetPosition` (overload 1) | `AudioStreamDIMENSION2D.StreamDimension3SetPosition(...)` | `AudioStreamDIMENSION2D.StreamDimension3SetPosition() : void` | function |
| `Moss_AudioStream3DSetVelocity` (overload 1) | `AudioStreamDIMENSION2D.StreamDimension3SetVelocity(...)` | `AudioStreamDIMENSION2D.StreamDimension3SetVelocity() : void` | function |
| `Moss_AudioStream3DSetMaxDistance` (overload 1) | `AudioStreamDIMENSION2D.StreamDimension3SetMaxDistance(...)` | `AudioStreamDIMENSION2D.StreamDimension3SetMaxDistance() : void` | function |
| `Moss_AudioStream2DRemove` | `AudioStreamDIMENSION2D.Dispose(...)` | `AudioStreamDIMENSION2D.Dispose() : void` | function |
| `Moss_AudioStream3DCreate` | `Audio.CreateStreamDimension3D(...)` | `Audio.CreateStreamDimension3D() : AudioStreamDIMENSION3D` | function |
| `Moss_AudioStream3DPlay` | `AudioStreamDIMENSION3D.Play(...)` | `AudioStreamDIMENSION3D.Play() : void` | function |
| `Moss_AudioStream3DStop` | `AudioStreamDIMENSION3D.Stop(...)` | `AudioStreamDIMENSION3D.Stop() : void` | function |
| `Moss_AudioStream3DSetVolume` | `AudioStreamDIMENSION3D.SetVolume(...)` | `AudioStreamDIMENSION3D.SetVolume(float volume) : void` | function |
| `Moss_AudioStream3DSetPitch` | `AudioStreamDIMENSION3D.SetPitch(...)` | `AudioStreamDIMENSION3D.SetPitch(float pitch) : void` | function |
| `Moss_AudioStream3DSetPlaybackRate` | `AudioStreamDIMENSION3D.SetPlaybackRate(...)` | `AudioStreamDIMENSION3D.SetPlaybackRate(float rate) : void` | function |
| `Moss_AudioStream3DSetPan` | `AudioStreamDIMENSION3D.SetPan(...)` | `AudioStreamDIMENSION3D.SetPan(float pan) : void` | function |
| `Moss_AudioStream3DSetLoop` | `AudioStreamDIMENSION3D.SetLoop(...)` | `AudioStreamDIMENSION3D.SetLoop(bool loop) : void` | function |
| `Moss_AudioStream3DSetPosition` (overload 2) | `AudioStreamDIMENSION3D.SetPosition(...)` | `AudioStreamDIMENSION3D.SetPosition() : void` | function |
| `Moss_AudioStream3DSetVelocity` (overload 2) | `AudioStreamDIMENSION3D.SetVelocity(...)` | `AudioStreamDIMENSION3D.SetVelocity() : void` | function |
| `Moss_AudioStream3DSetMaxDistance` (overload 2) | `AudioStreamDIMENSION3D.SetMaxDistance(...)` | `AudioStreamDIMENSION3D.SetMaxDistance() : void` | function |
| `Moss_AudioStream3DSetDistanceModel` | `AudioStreamDIMENSION3D.SetDistanceModel(...)` | `AudioStreamDIMENSION3D.SetDistanceModel(DistanceModel model) : void` | function |
| `Moss_AudioStream3DRemove` | `AudioStreamDIMENSION3D.Dispose(...)` | `AudioStreamDIMENSION3D.Dispose() : void` | function |
| `Moss_AudioCreateAudioListener2D` | `Audio.CreateAudioListenerDimension2D(...)` | `Audio.CreateAudioListenerDimension2D(Float2 position) : AudioListenerDIMENSION2D` | function |
| `Moss_AudioCreateAudioListener3D` | `Audio.CreateAudioListenerDimension3D(...)` | `Audio.CreateAudioListenerDimension3D(Float3 position) : AudioListenerDIMENSION3D` | function |
| `Moss_AudioCreateRayAudioListener2D` | `Audio.CreateRayAudioListenerDimension2D(...)` | `Audio.CreateRayAudioListenerDimension2D(Float2 position) : RayAudioListenerDIMENSION2D` | function |
| `Moss_AudioCreateRayAudioListener3D` | `Audio.CreateRayAudioListenerDimension3D(...)` | `Audio.CreateRayAudioListenerDimension3D(Float3 position) : RayAudioListenerDIMENSION3D` | function |
| `Moss_AudioRemoveAudioListener2D` | `AudioListenerDIMENSION2D.Dispose(...)` | `AudioListenerDIMENSION2D.Dispose() : void` | function |
| `Moss_AudioRemoveAudioListener3D` | `AudioListenerDIMENSION3D.Dispose(...)` | `AudioListenerDIMENSION3D.Dispose() : void` | function |
| `Moss_AudioRemoveRayAudioListener2D` | `RayAudioListenerDIMENSION2D.Dispose(...)` | `RayAudioListenerDIMENSION2D.Dispose() : void` | function |
| `Moss_AudioRemoveRayAudioListener3D` | `RayAudioListenerDIMENSION3D.Dispose(...)` | `RayAudioListenerDIMENSION3D.Dispose() : void` | function |
| `Moss_AudioActivateAudioListener2D` | `AudioListenerDIMENSION2D.Activate(...)` | `AudioListenerDIMENSION2D.Activate(bool activate) : void` | function |
| `Moss_AudioActivateAudioListener3D` | `AudioListenerDIMENSION3D.Activate(...)` | `AudioListenerDIMENSION3D.Activate(bool activate) : void` | function |
| `Moss_AudioActivateRayAudioListener2D` | `RayAudioListenerDIMENSION2D.Activate(...)` | `RayAudioListenerDIMENSION2D.Activate(bool activate) : void` | function |
| `Moss_AudioActivateRayAudioListener3D` | `RayAudioListenerDIMENSION3D.Activate(...)` | `RayAudioListenerDIMENSION3D.Activate(bool activate) : void` | function |
| `Moss_AudioListenerSetOrientation` | `AudioListenerDIMENSION3D.ListenerSetOrientation(...)` | `AudioListenerDIMENSION3D.ListenerSetOrientation(Vec3 forward, Vec3 up) : void` | function |
| `Moss_AudioRayListener2DSetPhysics` | `RayAudioListenerDIMENSION2D.RaySetPhysics(...)` | `RayAudioListenerDIMENSION2D.RaySetPhysics(PhysicsSystem physics, AudioRaycastDIMENSION2DCallback raycast) : void` | function |
| `Moss_AudioRayListener3DSetPhysics` | `RayAudioListenerDIMENSION3D.RaySetPhysics(...)` | `RayAudioListenerDIMENSION3D.RaySetPhysics(PhysicsSystem physics, AudioRaycastDIMENSION3DCallback raycast) : void` | function |
| `Moss_AudioRayListener2DSetTraceSettings` | `RayAudioListenerDIMENSION2D.RaySetTraceSettings(...)` | `RayAudioListenerDIMENSION2D.RaySetTraceSettings(float maxDistance, uint reflectionRays) : void` | function |
| `Moss_AudioRayListener3DSetTraceSettings` | `RayAudioListenerDIMENSION3D.RaySetTraceSettings(...)` | `RayAudioListenerDIMENSION3D.RaySetTraceSettings(float maxDistance, uint reflectionRays) : void` | function |
| `Moss_AudioRayTrace2D` | `Audio.RayTraceDimension2D(...)` | `Audio.RayTraceDimension2D(AudioRayTraceDIMENSION2DDesc desc, AudioRayTraceResult outResult) : bool` | function |
| `Moss_AudioRayTrace3D` | `Audio.RayTraceDimension3D(...)` | `Audio.RayTraceDimension3D(AudioRayTraceDIMENSION3DDesc desc, AudioRayTraceResult outResult) : bool` | function |
| `Moss_AudioRayTraceFromListener2D` | `RayAudioListenerDIMENSION2D.RayTraceFrom(...)` | `RayAudioListenerDIMENSION2D.RayTraceFrom(Vec2 sourcePosition, AudioRayTraceResult outResult) : bool` | function |
| `Moss_AudioRayTraceFromListener3D` | `RayAudioListenerDIMENSION3D.RayTraceFrom(...)` | `RayAudioListenerDIMENSION3D.RayTraceFrom(Vec3 sourcePosition, AudioRayTraceResult outResult) : bool` | function |
| `Moss_AudioRayTraceComputeGain` | `AudioRayTraceResult.RayTraceComputeGain(...)` | `AudioRayTraceResult.RayTraceComputeGain() : float` | function |
| `Moss_IsSpeakerDeviceReady` | `Audio.IsSpeakerDeviceReady(...)` | `Audio.IsSpeakerDeviceReady() : bool` | function |
| `Moss_AudioSpeakerOpen` | `Audio.SpeakerOpen(...)` | `Audio.SpeakerOpen() : void` | function |
| `Moss_AudioSpeakerPause` | `Audio.SpeakerPause(...)` | `Audio.SpeakerPause() : void` | function |
| `Moss_AudioSpeakerResume` | `Audio.SpeakerResume(...)` | `Audio.SpeakerResume() : void` | function |
| `Moss_AudioSpeakerIsPaused` | `Audio.SpeakerIsPaused(...)` | `Audio.SpeakerIsPaused() : bool` | function |
| `Moss_AudioSelectSpeakerDevice` | `Audio.SelectSpeakerDevice(...)` | `Audio.SelectSpeakerDevice(int id) : bool` | function |
| `Moss_GetCurrentSpeakerDeviceID` | `Audio.GetCurrentSpeakerDeviceId(...)` | `Audio.GetCurrentSpeakerDeviceId() : int` | function |
| `Moss_GetSpeakerDeviceName` | `Audio.GetSpeakerDeviceName(...)` | `Audio.GetSpeakerDeviceName(int id) : string?` | function |
| `Moss_ListSpeakerDevices` | `Audio.ListSpeakerDevices(...)` | `Audio.ListSpeakerDevices() : int` | function |
| `Moss_MicrophoneGetDeviceCount` | `Audio.MicrophoneGetDeviceCount(...)` | `Audio.MicrophoneGetDeviceCount() : uint` | function |
| `Moss_MicrophoneGetDeviceName` | `Audio.MicrophoneGetDeviceName(...)` | `Audio.MicrophoneGetDeviceName(uint deviceIndex) : string?` | function |
| `Moss_MicrophoneOpen` | `Audio.MicrophoneOpen(...)` | `Audio.MicrophoneOpen(MicrophoneDesc desc) : Microphone` | function |
| `Moss_MicrophoneClose` | `Microphone.Dispose(...)` | `Microphone.Dispose() : void` | function |
| `Moss_MicrophoneStart` | `Microphone.Start(...)` | `Microphone.Start() : bool` | function |
| `Moss_MicrophoneStop` | `Microphone.Stop(...)` | `Microphone.Stop() : void` | function |
| `Moss_MicrophoneRead` | `Microphone.Read(...)` | `Microphone.Read(out float outSamples, uint maxFrames) : uint` | function |
| `Moss_MicrophoneSetGain` | `Microphone.SetGain(...)` | `Microphone.SetGain(float gain) : void` | function |
| `Moss_MicrophoneGetSampleRate` | `Microphone.GetSampleRate(...)` | `Microphone.GetSampleRate() : uint` | function |
| `Moss_MicrophoneGetChannels` | `Microphone.GetChannels(...)` | `Microphone.GetChannels() : uint` | function |
| `Moss_MicrophoneSetCallback` | `Microphone.SetCallback(...)` | `Microphone.SetCallback(MicrophoneCallback callback) : void` | function |
| `Moss_MicrophoneGetLevels` | `Microphone.GetLevels(...)` | `Microphone.GetLevels() : MicrophoneLevels` | function |
| `Moss_MicrophoneGetLevelRMS` | `Microphone.GetLevelRms(...)` | `Microphone.GetLevelRms() : float` | function |
| `Moss_MicrophoneGetLevelPeak` | `Microphone.GetLevelPeak(...)` | `Microphone.GetLevelPeak() : float` | function |
| `Moss_MicrophoneGetSmoothedVolume` | `Microphone.GetSmoothedVolume(...)` | `Microphone.GetSmoothedVolume() : float` | function |
| `Moss_MicrophoneGetVoiceActivity` | `Microphone.GetVoiceActivity(...)` | `Microphone.GetVoiceActivity() : float` | function |
| `Moss_IsMicrophoneDeviceReady` | `Audio.IsMicrophoneDeviceReady(...)` | `Audio.IsMicrophoneDeviceReady() : bool` | function |
| `Moss_AudioMicrophoneOpen` | `Audio.MicrophoneOpen(...)` | `Audio.MicrophoneOpen() : int` | function |
| `Moss_AudioMicrophoneClose` | `Audio.MicrophoneClose(...)` | `Audio.MicrophoneClose() : void` | function |
| `Moss_AudioMicrophonePlay` | `Audio.MicrophonePlay(...)` | `Audio.MicrophonePlay() : void` | function |
| `Moss_AudioMicrophoneStop` | `Audio.MicrophoneStop(...)` | `Audio.MicrophoneStop() : void` | function |
| `Moss_AudioMicrophoneID` | `Audio.MicrophoneId(...)` | `Audio.MicrophoneId() : int` | function |
| `Moss_AudioSelectMicrophoneDevice` | `Audio.SelectMicrophoneDevice(...)` | `Audio.SelectMicrophoneDevice(int id) : bool` | function |
| `Moss_GetMicrophoneDeviceName` | `Audio.GetMicrophoneDeviceName(...)` | `Audio.GetMicrophoneDeviceName(int index) : string?` | function |
| `Moss_ListMicrophoneDevices` | `Audio.ListMicrophoneDevices(...)` | `Audio.ListMicrophoneDevices() : int` | function |
| `Moss_AudioMicrophoneSetGain` | `Microphone.SetGain(...)` | `Microphone.SetGain(float gain) : void` | function |
| `Moss_AudioMicrophoneGetSampleRate` | `Microphone.GetSampleRate(...)` | `Microphone.GetSampleRate() : int` | function |
| `Moss_AudioMicrophoneGetChannels` | `Microphone.GetChannels(...)` | `Microphone.GetChannels() : int` | function |
| `Moss_AudioStreamSetCallback` | `AudioStream.SetCallback(...)` | `AudioStream.SetCallback(AudioStreamCallback callback) : void` | function |

### `Moss_GPU.h` → `Moss.GPU`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_AcquireGPUCommandBuffer` | `GpuDevice.AcquireCommandBuffer(...)` | `GpuDevice.AcquireCommandBuffer() : GpuCommandBuffer` | function |
| `Moss_BeginGPUComputePass` | `GpuCommandBuffer.BeginComputePass(...)` | `GpuCommandBuffer.BeginComputePass() : void` | function |
| `Moss_BeginGPUCopyPass` | `GpuCommandBuffer.BeginCopyPass(...)` | `GpuCommandBuffer.BeginCopyPass() : void` | function |
| `Moss_CreateGPUBuffer` | `GpuDevice.CreateBuffer(...)` | `GpuDevice.CreateBuffer(GpuBufferDesc desc) : GpuBuffer` | function |
| `Moss_DestroyGPUDevice` | `GpuDevice.Dispose(...)` | `GpuDevice.Dispose() : void` | function |
| `Moss_BeginGPURenderPass` | `GpuCommandBuffer.BeginRenderPass(...)` | `GpuCommandBuffer.BeginRenderPass(Framebuffer framebuffer) : void` | function |
| `Moss_BindGPUComputeSamplers` | `GpuCommandBuffer.BindComputeSamplers(...)` | `GpuCommandBuffer.BindComputeSamplers(uint firstSlot, ReadOnlySpan<GpuSampler*> samplers) : void` | function |
| `Moss_BindGPUComputeTextures` | `GpuCommandBuffer.BindComputeTextures(...)` | `GpuCommandBuffer.BindComputeTextures(uint firstSlot, ReadOnlySpan<GpuTexture*> textures) : void` | function |
| `Moss_BindGPUComputeUniformBuffers` | `GpuCommandBuffer.BindComputeUniformBuffers(...)` | `GpuCommandBuffer.BindComputeUniformBuffers(uint firstSlot, ReadOnlySpan<GpuBufferBinding> bindings) : void` | function |
| `Moss_BindGPUFragmentSamplers` | `GpuCommandBuffer.BindFragmentSamplers(...)` | `GpuCommandBuffer.BindFragmentSamplers(uint firstSlot, ReadOnlySpan<GpuSampler*> samplers) : void` | function |
| `Moss_BindGPUFragmentTextures` | `GpuCommandBuffer.BindFragmentTextures(...)` | `GpuCommandBuffer.BindFragmentTextures(uint firstSlot, ReadOnlySpan<GpuTexture*> textures) : void` | function |
| `Moss_BindGPUFragmentUniformBuffers` | `GpuCommandBuffer.BindFragmentUniformBuffers(...)` | `GpuCommandBuffer.BindFragmentUniformBuffers(uint firstSlot, ReadOnlySpan<GpuBufferBinding> bindings) : void` | function |
| `Moss_BindGPUIndexBuffer` | `GpuCommandBuffer.BindIndex(...)` | `GpuCommandBuffer.BindIndex(GpuBuffer buffer, ulong offset, GpuIndexType indexType) : void` | function |
| `Moss_BindGPUPipeline` | `GpuCommandBuffer.BindPipeline(...)` | `GpuCommandBuffer.BindPipeline(PipelineState pipeline) : void` | function |
| `Moss_BindGPUResourceSet` | `GpuCommandBuffer.BindResourceSet(...)` | `GpuCommandBuffer.BindResourceSet(uint setIndex, ResourceSet set) : void` | function |
| `Moss_BindGPUVertexBuffers` | `GpuCommandBuffer.BindVertexBuffers(...)` | `GpuCommandBuffer.BindVertexBuffers(uint firstBinding, ReadOnlySpan<GpuBufferBinding> bindings) : void` | function |
| `Moss_BindGPUVertexSamplers` | `GpuCommandBuffer.BindVertexSamplers(...)` | `GpuCommandBuffer.BindVertexSamplers(uint firstSlot, ReadOnlySpan<GpuSampler*> samplers) : void` | function |
| `Moss_BindGPUVertexTextures` | `GpuCommandBuffer.BindVertexTextures(...)` | `GpuCommandBuffer.BindVertexTextures(uint firstSlot, ReadOnlySpan<GpuTexture*> textures) : void` | function |
| `Moss_BindGPUVertexUniformBuffers` | `GpuCommandBuffer.BindVertexUniformBuffers(...)` | `GpuCommandBuffer.BindVertexUniformBuffers(uint firstSlot, ReadOnlySpan<GpuBufferBinding> bindings) : void` | function |
| `Moss_BlitGPUTexture` | `GpuCommandBuffer.BlitTexture(...)` | `GpuCommandBuffer.BlitTexture(Texture src, Texture dst, GpuBlitRegion region) : void` | function |
| `Moss_CalculateGPUTextureFormatSize` | `GPU.CalculateTextureFormatSize(...)` | `GPU.CalculateTextureFormatSize(TextureFormat format, uint width, uint height, uint depth) : uint` | function |
| `Moss_CancelGPUCommandBuffer` | `GpuCommandBuffer.Cancel(...)` | `GpuCommandBuffer.Cancel() : void` | function |
| `Moss_CopyGPUBufferToBuffer` | `GpuCommandBuffer.CopyToBuffer(...)` | `GpuCommandBuffer.CopyToBuffer(GpuBuffer src, GpuBuffer dst, GpuBufferRegion srcRegion, GpuBufferRegion dstRegion) : void` | function |
| `Moss_CopyGPUBufferToTexture` | `GpuCommandBuffer.CopyToTexture(...)` | `GpuCommandBuffer.CopyToTexture(GpuBuffer src, GpuTexture dst, GpuBufferRegion srcRegion, GpuTextureRegion dstRegion) : void` | function |
| `Moss_CopyGPUTextureToBuffer` | `GpuCommandBuffer.CopyTextureTo(...)` | `GpuCommandBuffer.CopyTextureTo(GpuTexture src, GpuBuffer dst, GpuTextureRegion srcRegion, GpuBufferRegion dstRegion) : void` | function |
| `Moss_CopyGPUTextureToTexture` | `GpuCommandBuffer.CopyTextureToTexture(...)` | `GpuCommandBuffer.CopyTextureToTexture(GpuTexture src, GpuTexture dst, GpuTextureRegion srcRegion, GpuTextureRegion dstRegion) : void` | function |
| `Moss_CreateGPUSampler` | `GpuDevice.CreateSampler(...)` | `GpuDevice.CreateSampler(GpuSamplerCreateInfo createInfo) : GpuSampler` | function |
| `Moss_CreateGPUShader` | `GpuDevice.CreateShader(...)` | `GpuDevice.CreateShader(GpuShaderCreateInfo createInfo) : GpuShader` | function |
| `Moss_CreateGPUComputePipeline` | `GpuDevice.CreateComputePipeline(...)` | `GpuDevice.CreateComputePipeline(GpuComputePipelineCreateInfo createInfo) : ComputePipelineState` | function |
| `Moss_CreateGPUTexture` | `GpuDevice.CreateTexture(...)` | `GpuDevice.CreateTexture(GpuTextureCreateInfo createInfo) : GpuTexture` | function |
| `Moss_CreateGPUTextureView` | `GpuDevice.CreateTextureView(...)` | `GpuDevice.CreateTextureView(GpuTextureViewCreateInfo createInfo) : GpuTextureView` | function |
| `Moss_CreateGPUTransferBuffer` | `GpuDevice.CreateTransferBuffer(...)` | `GpuDevice.CreateTransferBuffer(GpuTransferBufferCreateInfo createInfo) : GpuTransferBuffer` | function |
| `Moss_CreateGPUFramebuffer` | `GpuDevice.CreateFramebuffer(...)` | `GpuDevice.CreateFramebuffer(GpuFramebufferCreateInfo createInfo) : Framebuffer` | function |
| `Moss_CreateGPUResourceSetLayout` | `GpuDevice.CreateResourceSetLayout(...)` | `GpuDevice.CreateResourceSetLayout(GpuResourceSetLayoutCreateInfo createInfo) : ResourceSetLayout` | function |
| `Moss_CreateGPUResourceSet` | `GpuDevice.CreateResourceSet(...)` | `GpuDevice.CreateResourceSet(GpuResourceSetCreateInfo createInfo) : ResourceSet` | function |
| `Moss_CreateGPUQueryPool` | `GpuDevice.CreateQueryPool(...)` | `GpuDevice.CreateQueryPool(GpuQueryPoolCreateInfo createInfo) : GpuQueryPool` | function |
| `Moss_DispatchGPUCompute` | `GpuCommandBuffer.DispatchCompute(...)` | `GpuCommandBuffer.DispatchCompute(uint groupCountX, uint groupCountY, uint groupCountZ) : void` | function |
| `Moss_DispatchGPUComputeIndirect` | `GpuCommandBuffer.DispatchComputeIndirect(...)` | `GpuCommandBuffer.DispatchComputeIndirect(GpuBuffer indirectBuffer, ulong offset) : void` | function |
| `Moss_DownloadFromGPUBuffer` | `GpuBuffer.DownloadFrom(...)` | `GpuBuffer.DownloadFrom(Span<byte> dstData, ulong size, ulong srcOffset) : void` | function |
| `Moss_DownloadFromGPUTexture` | `GpuTexture.DownloadFrom(...)` | `GpuTexture.DownloadFrom(GpuTextureRegion srcRegion, Span<byte> dstData, uint dstRowPitch, uint dstSlicePitch) : void` | function |
| `Moss_DrawGPUIndexedPrimitives` | `GpuCommandBuffer.DrawIndexedPrimitives(...)` | `GpuCommandBuffer.DrawIndexedPrimitives(uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance) : void` | function |
| `Moss_DrawGPUIndexedPrimitivesIndirect` | `GpuCommandBuffer.DrawIndexedPrimitivesIndirect(...)` | `GpuCommandBuffer.DrawIndexedPrimitivesIndirect(GpuBuffer indirectBuffer, ulong offset, uint drawCount, uint stride) : void` | function |
| `Moss_DrawGPUPrimitives` | `GpuCommandBuffer.DrawPrimitives(...)` | `GpuCommandBuffer.DrawPrimitives(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance) : void` | function |
| `Moss_DrawGPUPrimitivesIndirect` | `GpuCommandBuffer.DrawPrimitivesIndirect(...)` | `GpuCommandBuffer.DrawPrimitivesIndirect(GpuBuffer indirectBuffer, ulong offset, uint drawCount, uint stride) : void` | function |
| `Moss_EndGPUComputePass` | `GpuCommandBuffer.EndComputePass(...)` | `GpuCommandBuffer.EndComputePass() : void` | function |
| `Moss_EndGPUCopyPass` | `GpuCommandBuffer.EndCopyPass(...)` | `GpuCommandBuffer.EndCopyPass() : void` | function |
| `Moss_EndGPURenderPass` | `GpuCommandBuffer.EndRenderPass(...)` | `GpuCommandBuffer.EndRenderPass() : void` | function |
| `Moss_GDKResumeGPU` | `GpuDevice.GdkResume(...)` | `GpuDevice.GdkResume() : void` | function |
| `Moss_GDKSuspendGPU` | `GpuDevice.GdkSuspend(...)` | `GpuDevice.GdkSuspend() : void` | function |
| `Moss_GenerateMipmapsForGPUTexture` | `GpuCommandBuffer.GenerateMipmapsForTexture(...)` | `GpuCommandBuffer.GenerateMipmapsForTexture(GpuTexture texture) : void` | function |
| `Moss_GetGPUDriver` | `GPU.GetDriver(...)` | `GPU.GetDriver(uint driverIndex) : string?` | function |
| `Moss_GetCompiledGPUBackendType` | `GPU.GetCompiledBackendType(...)` | `GPU.GetCompiledBackendType() : GpuBackendType` | function |
| `Moss_GetGPUDeviceBackendType` | `GpuDevice.GetBackendType(...)` | `GpuDevice.GetBackendType() : GpuBackendType` | function |
| `Moss_GPUBackendGetName` | `GPU.BackendGetName(...)` | `GPU.BackendGetName(GpuBackendType backend) : string?` | function |
| `Moss_GPUBackendGetShaderFormats` | `GPU.BackendGetShaderFormats(...)` | `GPU.BackendGetShaderFormats(GpuBackendType backend) : uint` | function |
| `Moss_GPUBackendIsCompiled` | `GPU.BackendIsCompiled(...)` | `GPU.BackendIsCompiled(GpuBackendType backend) : bool` | function |
| `Moss_GPUBackendIsSupportedOnPlatform` | `GPU.BackendIsSupportedOnPlatform(...)` | `GPU.BackendIsSupportedOnPlatform(GpuBackendType backend) : bool` | function |
| `Moss_GetGPUBackendInfo` | `GPU.GetBackendInfo(...)` | `GPU.GetBackendInfo(GpuBackendType backend, GpuBackendInfo outInfo) : bool` | function |
| `Moss_RegisterNativeGPUBackend` | `GPU.RegisterNativeBackend(...)` | `GPU.RegisterNativeBackend(GpuBackendType backend, ReadOnlySpan<byte> nativeTable) : bool` | function |
| `Moss_HasNativeGPUBackend` | `GPU.HasNativeBackend(...)` | `GPU.HasNativeBackend(GpuBackendType backend) : bool` | function |
| `Moss_GetGPUShaderFormats` | `GpuDevice.GetShaderFormats(...)` | `GpuDevice.GetShaderFormats() : uint` | function |
| `Moss_GetGPUTextureFormatFromPixelFormat` | `GPU.GetTextureFormatFromPixelFormat(...)` | `GPU.GetTextureFormatFromPixelFormat(PixelFormat pixelFormat) : TextureFormat` | function |
| `Moss_GetNumGPUDrivers` | `GPU.GetNumDrivers(...)` | `GPU.GetNumDrivers() : uint` | function |
| `Moss_GetPixelFormatFromGPUTextureFormat` | `GPU.GetPixelFormatFromTextureFormat(...)` | `GPU.GetPixelFormatFromTextureFormat(TextureFormat textureFormat) : PixelFormat` | function |
| `Moss_GPUSupportsProperties` | `GpuDevice.SupportsProperties(...)` | `GpuDevice.SupportsProperties() : bool` | function |
| `Moss_GPUSupportsShaderFormats` | `GpuDevice.SupportsShaderFormats(...)` | `GpuDevice.SupportsShaderFormats(uint shaderFormatMask) : bool` | function |
| `Moss_GPUTextureFormatTexelBlockSize` | `GPU.TextureFormatTexelBlockSize(...)` | `GPU.TextureFormatTexelBlockSize(TextureFormat format) : uint` | function |
| `Moss_GPUTextureSupportsFormat` | `GpuDevice.TextureSupportsFormat(...)` | `GpuDevice.TextureSupportsFormat(TextureFormat format, TextureUsage usage) : bool` | function |
| `Moss_GPUTextureSupportsSampleCount` | `GpuDevice.TextureSupportsSampleCount(...)` | `GpuDevice.TextureSupportsSampleCount(TextureFormat format, uint sampleCount) : bool` | function |
| `Moss_InsertGPUDebugLabel` | `GpuCommandBuffer.InsertDebugLabel(...)` | `GpuCommandBuffer.InsertDebugLabel(string label) : void` | function |
| `Moss_MapGPUTransferBuffer` | `GpuTransferBuffer.Map(...)` | `GpuTransferBuffer.Map() : Span<byte>` | function |
| `Moss_PopGPUDebugGroup` | `GpuCommandBuffer.PopDebugGroup(...)` | `GpuCommandBuffer.PopDebugGroup() : void` | function |
| `Moss_PushGPUComputeUniformData` | `GpuCommandBuffer.PushComputeUniformData(...)` | `GpuCommandBuffer.PushComputeUniformData(uint slot, ReadOnlySpan<byte> data, uint size) : void` | function |
| `Moss_PushGPUDebugGroup` | `GpuCommandBuffer.PushDebugGroup(...)` | `GpuCommandBuffer.PushDebugGroup(string label) : void` | function |
| `Moss_PushGPUFragmentUniformData` | `GpuCommandBuffer.PushFragmentUniformData(...)` | `GpuCommandBuffer.PushFragmentUniformData(uint slot, ReadOnlySpan<byte> data, uint size) : void` | function |
| `Moss_PushGPUVertexUniformData` | `GpuCommandBuffer.PushVertexUniformData(...)` | `GpuCommandBuffer.PushVertexUniformData(uint slot, ReadOnlySpan<byte> data, uint size) : void` | function |
| `Moss_QueryGPUFence` | `GpuFence.Query(...)` | `GpuFence.Query() : bool` | function |
| `Moss_ReleaseGPUBuffer` | `GpuBuffer.Release(...)` | `GpuBuffer.Release() : void` | function |
| `Moss_ReleaseGPUFence` | `GpuFence.Release(...)` | `GpuFence.Release() : void` | function |
| `Moss_ReleaseGPUSampler` | `GpuSampler.Release(...)` | `GpuSampler.Release() : void` | function |
| `Moss_ReleaseGPUShader` | `GpuShader.Release(...)` | `GpuShader.Release() : void` | function |
| `Moss_ReleaseGPUComputePipeline` | `GpuDevice.ReleaseComputePipeline(...)` | `GpuDevice.ReleaseComputePipeline(ComputePipelineState pipeline) : void` | function |
| `Moss_ReleaseGPUTexture` | `GpuTexture.Release(...)` | `GpuTexture.Release() : void` | function |
| `Moss_ReleaseGPUTextureView` | `GpuTextureView.Release(...)` | `GpuTextureView.Release() : void` | function |
| `Moss_ReleaseGPUTransferBuffer` | `GpuTransferBuffer.Release(...)` | `GpuTransferBuffer.Release() : void` | function |
| `Moss_ReleaseGPUFramebuffer` | `Framebuffer.Release(...)` | `Framebuffer.Release() : void` | function |
| `Moss_ReleaseGPUResourceSetLayout` | `ResourceSetLayout.Release(...)` | `ResourceSetLayout.Release() : void` | function |
| `Moss_ReleaseGPUResourceSet` | `ResourceSet.Release(...)` | `ResourceSet.Release() : void` | function |
| `Moss_ReleaseGPUQueryPool` | `GpuQueryPool.Release(...)` | `GpuQueryPool.Release() : void` | function |
| `Moss_SetGPUAllowedFramesInFlight` | `GpuDevice.SetAllowedFramesInFlight(...)` | `GpuDevice.SetAllowedFramesInFlight(uint framesInFlight) : void` | function |
| `Moss_SetGPUBlendConstants` | `GpuCommandBuffer.SetBlendConstants(...)` | `GpuCommandBuffer.SetBlendConstants(float r, float g, float b, float a) : void` | function |
| `Moss_SetGPUBufferName` | `GpuBuffer.SetName(...)` | `GpuBuffer.SetName(string name) : void` | function |
| `Moss_SetGPUScissor` (overload 1) | `GpuCommandBuffer.SetScissor(...)` | `GpuCommandBuffer.SetScissor(Rect scissor) : void` | function |
| `Moss_SetGPUStencilReference` | `GpuCommandBuffer.SetStencilReference(...)` | `GpuCommandBuffer.SetStencilReference(uint reference) : void` | function |
| `Moss_SetGPUTextureName` | `GpuTexture.SetName(...)` | `GpuTexture.SetName(string name) : void` | function |
| `Moss_SetGPUViewport` | `GpuCommandBuffer.SetViewport(...)` | `GpuCommandBuffer.SetViewport(GpuViewport viewport) : void` | function |
| `Moss_SetGPUScissor` (overload 2) | `GpuCommandBuffer.SetScissor(...)` | `GpuCommandBuffer.SetScissor(Rect scissors) : void` | function |
| `Moss_SubmitGPUCommandBuffer` | `GpuCommandBuffer.Submit(...)` | `GpuCommandBuffer.Submit() : void` | function |
| `Moss_SubmitGPUCommandBufferAndAcquireFence` | `GpuCommandBuffer.SubmitAndAcquireFence(...)` | `GpuCommandBuffer.SubmitAndAcquireFence() : GpuFence` | function |
| `Moss_BindGPUComputePipeline` | `GpuCommandBuffer.BindComputePipeline(...)` | `GpuCommandBuffer.BindComputePipeline(ComputePipelineState pipeline) : void` | function |
| `Moss_BarrierGPUResources` | `GpuCommandBuffer.BarrierResources(...)` | `GpuCommandBuffer.BarrierResources(GpuBarrierInfo barrierInfo) : void` | function |
| `Moss_ResetGPUQueryPool` | `GpuCommandBuffer.ResetQueryPool(...)` | `GpuCommandBuffer.ResetQueryPool(GpuQueryPool queryPool, uint firstQuery, uint queryCount) : void` | function |
| `Moss_WriteGPUTimestamp` | `GpuCommandBuffer.WriteTimestamp(...)` | `GpuCommandBuffer.WriteTimestamp(GpuQueryPool queryPool, uint queryIndex) : void` | function |
| `Moss_BeginGPUQuery` | `GpuCommandBuffer.BeginQuery(...)` | `GpuCommandBuffer.BeginQuery(GpuQueryPool queryPool, uint queryIndex) : void` | function |
| `Moss_EndGPUQuery` | `GpuCommandBuffer.EndQuery(...)` | `GpuCommandBuffer.EndQuery(GpuQueryPool queryPool, uint queryIndex) : void` | function |
| `Moss_GetGPUQueryResults` | `GpuQueryPool.GetQueryResults(...)` | `GpuQueryPool.GetQueryResults(uint firstQuery, uint queryCount, Span<byte> data, ulong dataSize, ulong stride, GpuQueryResultFlags flags) : bool` | function |
| `Moss_TransitionGPUBuffer` | `GpuCommandBuffer.Transition(...)` | `GpuCommandBuffer.Transition(GpuBuffer buffer, ResourceState oldState, ResourceState newState) : void` | function |
| `Moss_TransitionGPUTexture` | `GpuCommandBuffer.TransitionTexture(...)` | `GpuCommandBuffer.TransitionTexture(GpuTexture texture, ResourceState oldState, ResourceState newState) : void` | function |
| `Moss_UnmapGPUTransferBuffer` | `GpuTransferBuffer.Unmap(...)` | `GpuTransferBuffer.Unmap() : void` | function |
| `Moss_UploadToGPUBuffer` | `GpuBuffer.UploadTo(...)` | `GpuBuffer.UploadTo(ReadOnlySpan<byte> srcData, ulong size, ulong dstOffset) : void` | function |
| `Moss_UploadToGPUTexture` | `GpuTexture.UploadTo(...)` | `GpuTexture.UploadTo(GpuTextureTransferInfo transferInfo) : void` | function |
| `Moss_WaitForGPUFences` | `GpuDevice.WaitForFences(...)` | `GpuDevice.WaitForFences(ReadOnlySpan<GpuFence*> fences, bool waitAll, ulong timeoutNs) : void` | function |
| `Moss_WaitForGPUIdle` | `GpuDevice.WaitForIdle(...)` | `GpuDevice.WaitForIdle() : void` | function |
| `Moss_WindowSupportsGPUPresentMode` | `Window.SupportsPresentMode(...)` | `Window.SupportsPresentMode(GpuPresentMode presentMode) : bool` | function |
| `Moss_GPUBlitInfo` | `new GpuBlitInfo(...)` | `new GpuBlitInfo()` | constructor |
| `Moss_GPUBlitRegion` | `new GpuBlitRegion(...)` | `new GpuBlitRegion()` | constructor |
| `Moss_GPUBufferBinding` | `new GpuBufferBinding(...)` | `new GpuBufferBinding()` | constructor |
| `Moss_GPUBufferCreateInfo` | `new GpuBufferCreateInfo(...)` | `new GpuBufferCreateInfo()` | constructor |
| `Moss_GPUBufferLocation` | `new GpuBufferLocation(...)` | `new GpuBufferLocation()` | constructor |
| `Moss_GPUBufferRegion` | `new GpuBufferRegion(...)` | `new GpuBufferRegion()` | constructor |
| `Moss_GPUColorTargetBlendState` | `new GpuColorTargetBlendState(...)` | `new GpuColorTargetBlendState()` | constructor |
| `Moss_GPUColorTargetDescription` | `new GpuColorTargetDescription(...)` | `new GpuColorTargetDescription()` | constructor |
| `Moss_GPUColorTargetInfo` | `new GpuColorTargetInfo(...)` | `new GpuColorTargetInfo()` | constructor |
| `Moss_GPUDepthStencilState` | `new GpuDepthStencilState(...)` | `new GpuDepthStencilState()` | constructor |
| `Moss_GPUDepthStencilTargetInfo` | `new GpuDepthStencilTargetInfo(...)` | `new GpuDepthStencilTargetInfo()` | constructor |
| `Moss_GPUIndexedIndirectDrawCommand` | `new GpuIndexedIndirectDrawCommand(...)` | `new GpuIndexedIndirectDrawCommand()` | constructor |
| `Moss_GPUIndirectDispatchCommand` | `new GpuIndirectDispatchCommand(...)` | `new GpuIndirectDispatchCommand()` | constructor |
| `Moss_GPUIndirectDrawCommand` | `new GpuIndirectDrawCommand(...)` | `new GpuIndirectDrawCommand()` | constructor |
| `Moss_GPUMultisampleState` | `new GpuMultisampleState(...)` | `new GpuMultisampleState()` | constructor |
| `Moss_GPURasterizerState` | `new GpuRasterizerState(...)` | `new GpuRasterizerState()` | constructor |
| `Moss_GPUSamplerCreateInfo` | `new GpuSamplerCreateInfo(...)` | `new GpuSamplerCreateInfo()` | constructor |
| `Moss_GPUShaderCreateInfo` | `new GpuShaderCreateInfo(...)` | `new GpuShaderCreateInfo()` | constructor |
| `Moss_GPUStencilOpState` | `new GpuStencilOpState(...)` | `new GpuStencilOpState()` | constructor |
| `Moss_GPUTextureCreateInfo` | `new GpuTextureCreateInfo(...)` | `new GpuTextureCreateInfo()` | constructor |
| `Moss_GPUTextureLocation` | `new GpuTextureLocation(...)` | `new GpuTextureLocation()` | constructor |
| `Moss_GPUTextureRegion` | `new GpuTextureRegion(...)` | `new GpuTextureRegion()` | constructor |
| `Moss_GPUTextureSamplerBinding` | `new GpuTextureSamplerBinding(...)` | `new GpuTextureSamplerBinding()` | constructor |
| `Moss_GPUTextureTransferInfo` | `new GpuTextureTransferInfo(...)` | `new GpuTextureTransferInfo()` | constructor |
| `Moss_GPUTransferBufferCreateInfo` | `new GpuTransferBufferCreateInfo(...)` | `new GpuTransferBufferCreateInfo()` | constructor |
| `Moss_GPUTransferBufferLocation` | `new GpuTransferBufferLocation(...)` | `new GpuTransferBufferLocation()` | constructor |
| `Moss_GPUVertexAttribute` | `new GpuVertexAttribute(...)` | `new GpuVertexAttribute()` | constructor |
| `Moss_GPUVertexBufferDescription` | `new GpuVertexBufferDescription(...)` | `new GpuVertexBufferDescription()` | constructor |
| `Moss_GPUVertexInputState` | `new GpuVertexInputState(...)` | `new GpuVertexInputState()` | constructor |
| `Moss_GPUViewport` | `new GpuViewport(...)` | `new GpuViewport()` | constructor |
| `Moss_BindGPUComputeStorageBuffers` | `GpuCommandBuffer.BindComputeStorageBuffers(...)` | `GpuCommandBuffer.BindComputeStorageBuffers(uint firstSlot, ReadOnlySpan<GpuStorageBufferReadWriteBinding> bindings) : void` | function |
| `Moss_BindGPUComputeStorageTextures` | `GpuCommandBuffer.BindComputeStorageTextures(...)` | `GpuCommandBuffer.BindComputeStorageTextures(uint firstSlot, ReadOnlySpan<GpuStorageTextureReadWriteBinding> bindings) : void` | function |
| `Moss_BindGPUFragmentStorageBuffers` | `GpuCommandBuffer.BindFragmentStorageBuffers(...)` | `GpuCommandBuffer.BindFragmentStorageBuffers(uint firstSlot, ReadOnlySpan<GpuStorageBufferReadWriteBinding> bindings) : void` | function |
| `Moss_BindGPUFragmentStorageTextures` | `GpuCommandBuffer.BindFragmentStorageTextures(...)` | `GpuCommandBuffer.BindFragmentStorageTextures(uint firstSlot, ReadOnlySpan<GpuStorageTextureReadWriteBinding> bindings) : void` | function |
| `Moss_BindGPUVertexStorageBuffers` | `GpuCommandBuffer.BindVertexStorageBuffers(...)` | `GpuCommandBuffer.BindVertexStorageBuffers(uint firstSlot, ReadOnlySpan<GpuStorageBufferReadWriteBinding> bindings) : void` | function |
| `Moss_BindGPUVertexStorageTextures` | `GpuCommandBuffer.BindVertexStorageTextures(...)` | `GpuCommandBuffer.BindVertexStorageTextures(uint firstSlot, ReadOnlySpan<GpuStorageTextureReadWriteBinding> bindings) : void` | function |
| `Moss_GetGPUDeviceDriver` | `GpuDevice.GetDriver(...)` | `GpuDevice.GetDriver() : string?` | function |
| `Moss_GetGPUDeviceProperties` | `GpuDevice.GetProperties(...)` | `GpuDevice.GetProperties() : GpuDeviceProperties` | function |
| `Moss_ReleaseWindowFromGPUDevice` | `GpuDevice.ReleaseWindowFrom(...)` | `GpuDevice.ReleaseWindowFrom(Window window) : void` | function |
| `Moss_ClaimWindowForGPUDevice` | `GpuDevice.ClaimWindowFor(...)` | `GpuDevice.ClaimWindowFor(Window window) : bool` | function |
| `Moss_UpdateGPUResourceSet` | `ResourceSet.Update(...)` | `ResourceSet.Update(ReadOnlySpan<GpuResourceBinding> bindings) : void` | function |
| `Moss_CreateGPUDevice` | `GpuDevice.Create(...)` | `GpuDevice.Create(GpuDeviceDesc descriptor) : GpuDevice` | function |
| `Moss_CreateGPUDeviceWithProperties` | `GpuDevice.CreateWithProperties(...)` | `GpuDevice.CreateWithProperties(GpuDeviceDesc descriptor, GpuDeviceProperties requestedProperties) : GpuDevice` | function |
| `Moss_GPUBufferCreate` | `GpuDevice.CreateBuffer(...)` | `GpuDevice.CreateBuffer(GpuBufferDescriptor descriptor) : GpuBuffer` | function |
| `Moss_GPUBufferUpload` | `GpuBuffer.Upload(...)` | `public void GpuBuffer.Upload(ReadOnlySpan<byte> data, ulong offset = 0)` | function |
| `Moss_GPUBufferMap` | `GpuBuffer.Map(...)` | `GpuBuffer.Map() : Span<byte>` | function |
| `Moss_GPUBufferUnmap` | `GpuBuffer.Unmap(...)` | `GpuBuffer.Unmap() : void` | function |
| `Moss_GPUBufferDestroy` | `GpuBuffer.Dispose(...)` | `GpuBuffer.Dispose() : void` | function |
| `Moss_AcquireGPUSwapchainTexture` | `GpuTexture.AcquireSwapchain(...)` | `GpuTexture.AcquireSwapchain(Window window) : bool` | function |
| `Moss_AcquireGPUSwapchainTextureStatus` | `GpuTexture.AcquireSwapchainStatus(...)` | `GpuTexture.AcquireSwapchainStatus(Window window) : GpuSwapchainResult` | function |
| `Moss_GetGPUSwapchainTextureFormat` | `GpuDevice.GetSwapchainTextureFormat(...)` | `GpuDevice.GetSwapchainTextureFormat(Window window) : TextureFormat` | function |
| `Moss_SetGPUSwapchainParameters` | `GpuDevice.SetSwapchainParameters(...)` | `GpuDevice.SetSwapchainParameters(Window window, uint width, uint height, GpuPresentMode presentMode) : bool` | function |
| `Moss_ResizeGPUSwapchain` | `GpuDevice.ResizeSwapchain(...)` | `GpuDevice.ResizeSwapchain(Window window, uint width, uint height) : GpuSwapchainResult` | function |
| `Moss_WaitAndAcquireGPUSwapchainTexture` | `GpuTexture.WaitAndAcquireSwapchain(...)` | `GpuTexture.WaitAndAcquireSwapchain(Window window, ulong timeoutNs) : bool` | function |
| `Moss_WaitAndAcquireGPUSwapchainTextureStatus` | `GpuTexture.WaitAndAcquireSwapchainStatus(...)` | `GpuTexture.WaitAndAcquireSwapchainStatus(Window window, ulong timeoutNs) : GpuSwapchainResult` | function |
| `Moss_WaitForGPUSwapchain` | `GpuDevice.WaitForSwapchain(...)` | `GpuDevice.WaitForSwapchain(Window window, ulong timeoutNs) : bool` | function |
| `Moss_WindowSupportsGPUSwapchainComposition` | `Window.SupportsSwapchainComposition(...)` | `Window.SupportsSwapchainComposition() : bool` | function |
| `Moss_PresentGPUSwapchain` | `GpuDevice.PresentSwapchain(...)` | `GpuDevice.PresentSwapchain(Window window, GpuTexture texture) : bool` | function |
| `Moss_PresentGPUSwapchainStatus` | `GpuDevice.PresentSwapchainStatus(...)` | `GpuDevice.PresentSwapchainStatus(Window window, GpuTexture texture) : GpuSwapchainResult` | function |
| `Moss_GPUClear` | `GpuCommandBuffer.Clear(...)` | `GpuCommandBuffer.Clear(float r, float g, float b, float a) : void` | function |
| `Moss_GPUSamplerCreate` | `GpuDevice.CreateSampler(...)` | `GpuDevice.CreateSampler(GpuSamplerDesc desc) : GpuSampler` | function |
| `Moss_GPUSamplerDestroy` | `GpuSampler.Dispose(...)` | `GpuSampler.Dispose() : void` | function |
| `Moss_TextureCreate` | `GpuDevice.CreateTexture(...)` | `GpuDevice.CreateTexture(TextureDesc desc) : Texture` | function |
| `Moss_TextureDestroy` | `Texture.Dispose(...)` | `Texture.Dispose() : void` | function |
| `Moss_TextureUpload` | `Texture.Upload(...)` | `Texture.Upload(TextureUploadDesc desc) : void` | function |
| `Moss_TextureGenerateMips` | `Texture.GenerateMips(...)` | `Texture.GenerateMips() : void` | function |
| `Moss_GPUGetExternalFeatureMask` | `GPU.GetExternalFeatureMask(...)` | `GPU.GetExternalFeatureMask() : uint` | function |
| `Moss_GPUExternalFeatureAvailable` | `GPU.ExternalFeatureAvailable(...)` | `GPU.ExternalFeatureAvailable(GpuExternalFeature feature) : bool` | function |
| `Moss_GPUGetExternalFeatures` | `GPU.GetExternalFeatures(...)` | `GPU.GetExternalFeatures(GpuExternalFeatures outFeatures) : void` | function |
| `Moss_GPUExternalFeatureName` | `GPU.ExternalFeatureName(...)` | `GPU.ExternalFeatureName(GpuExternalFeature feature) : string?` | function |
| `Moss_FSR1BuildEasuConstants` | `GPU.Fsr1BuildEasuConstants(...)` | `GPU.Fsr1BuildEasuConstants(uint outConstants16, float inputViewportWidth, float inputViewportHeight, float inputWidth, float inputHeight, float outputWidth, float outputHeight) : void` | function |
| `Moss_FSR2GetUpscaleRatio` | `GPU.Fsr2GetUpscaleRatio(...)` | `GPU.Fsr2GetUpscaleRatio(FSR2QualityMode quality) : float` | function |
| `Moss_FSR2GetRenderResolution` | `GPU.Fsr2GetRenderResolution(...)` | `GPU.Fsr2GetRenderResolution(uint displayWidth, uint displayHeight, FSR2QualityMode quality, FSR2RenderResolution outResolution) : bool` | function |
| `Moss_FSR2GetJitterPhaseCount` | `GPU.Fsr2GetJitterPhaseCount(...)` | `GPU.Fsr2GetJitterPhaseCount(int renderWidth, int displayWidth) : int` | function |
| `Moss_FSR2GetJitterOffset` | `GPU.Fsr2GetJitterOffset(...)` | `GPU.Fsr2GetJitterOffset(out float outX, out float outY, int index, int phaseCount) : bool` | function |
| `Moss_MeshGenerateVertexRemap` | `GPU.MeshGenerateVertexRemap(...)` | `GPU.MeshGenerateVertexRemap(out uint destination, ReadOnlySpan<Uint32T> indices, ReadOnlySpan<byte> vertices, nuint vertexCount, nuint vertexSize) : nuint` | function |
| `Moss_MeshOptimizeVertexCache` | `GPU.MeshOptimizeVertexCache(...)` | `GPU.MeshOptimizeVertexCache(out uint destination, ReadOnlySpan<Uint32T> indices, nuint vertexCount) : bool` | function |
| `Moss_MeshOptimizeOverdraw` | `GPU.MeshOptimizeOverdraw(...)` | `GPU.MeshOptimizeOverdraw(out uint destination, ReadOnlySpan<Uint32T> indices, ReadOnlySpan<Float> vertexPositions, nuint vertexPositionsStride, float threshold) : bool` | function |
| `Moss_MeshOptimizeVertexFetch` | `GPU.MeshOptimizeVertexFetch(...)` | `GPU.MeshOptimizeVertexFetch(Span<byte> destination, out uint indices, nuint indexCount, ReadOnlySpan<byte> vertices, nuint vertexCount, nuint vertexSize) : nuint` | function |
| `Moss_SMOLVEncode` | `GPU.SmolvEncode(...)` | `GPU.SmolvEncode(ReadOnlySpan<byte> spirvData, nuint spirvSize, Span<byte> outSmolvData, out nuint inoutSmolvSize, uint flags) : bool` | function |
| `Moss_SMOLVGetDecodedSize` | `GPU.SmolvGetDecodedSize(...)` | `GPU.SmolvGetDecodedSize(ReadOnlySpan<byte> smolvData, nuint smolvSize) : nuint` | function |
| `Moss_SMOLVDecode` | `GPU.SmolvDecode(...)` | `GPU.SmolvDecode(ReadOnlySpan<byte> smolvData, nuint smolvSize, Span<byte> outSpirvData, nuint spirvSize, uint flags) : bool` | function |
| `Moss_GPUVulkanMemoryAllocatorAvailable` | `GPU.MemoryAllocatorAvailable(...)` | `GPU.MemoryAllocatorAvailable() : bool` | function |
| `Moss_GPUClothSolverCreate` | `GpuDevice.CreateClothSolver(...)` | `GpuDevice.CreateClothSolver(GpuClothSolverDesc desc) : GpuClothSolver` | function |
| `Moss_GPUClothSolverDestroy` | `GpuClothSolver.Dispose(...)` | `GpuClothSolver.Dispose() : void` | function |
| `Moss_GPUClothSolverDispatch` | `GpuCommandBuffer.ClothSolverDispatch(...)` | `GpuCommandBuffer.ClothSolverDispatch(GpuClothSolver solver, GpuClothDispatchDesc desc) : bool` | function |
| `Moss_GPUCommandBufferBegin` | `GpuDevice.CreateCommandBuffer(...)` | `GpuDevice.CreateCommandBuffer(CommandQueue queue) : GpuCommandBuffer` | function |
| `Moss_GPUCommandBufferEnd` | `GpuCommandBuffer.End(...)` | `GpuCommandBuffer.End() : void` | function |
| `Moss_GPUCommandBufferSubmit` | `GpuCommandBuffer.Submit(...)` | `GpuCommandBuffer.Submit() : void` | function |
| `Moss_CmdBeginRenderPass` | `GpuCommandBuffer.CmdBeginRenderPass(...)` | `GpuCommandBuffer.CmdBeginRenderPass(Framebuffer fb) : void` | function |
| `Moss_CmdEndRenderPass` | `GpuCommandBuffer.CmdEndRenderPass(...)` | `GpuCommandBuffer.CmdEndRenderPass() : void` | function |
| `Moss_CmdBindPipeline` | `GpuCommandBuffer.CmdBindPipeline(...)` | `GpuCommandBuffer.CmdBindPipeline(PipelineState pipeline) : void` | function |
| `Moss_CmdBindResourceSet` | `GpuCommandBuffer.CmdBindResourceSet(...)` | `GpuCommandBuffer.CmdBindResourceSet(uint setIndex, ResourceSet set) : void` | function |
| `Moss_CmdBindVertexBuffer` | `GpuCommandBuffer.CmdBindVertex(...)` | `GpuCommandBuffer.CmdBindVertex(GpuBuffer buffer, ulong offset) : void` | function |
| `Moss_CmdBindIndexBuffer` | `GpuCommandBuffer.CmdBindIndex(...)` | `GpuCommandBuffer.CmdBindIndex(GpuBuffer buffer, ulong offset) : void` | function |
| `Moss_CmdDraw` | `GpuCommandBuffer.CmdDraw(...)` | `GpuCommandBuffer.CmdDraw(uint vertexCount, uint firstVertex) : void` | function |
| `Moss_CmdDrawIndexed` | `GpuCommandBuffer.CmdDrawIndexed(...)` | `GpuCommandBuffer.CmdDrawIndexed(uint indexCount, uint firstIndex, int vertexOffset) : void` | function |
| `Moss_CmdDispatch` | `GpuCommandBuffer.CmdDispatch(...)` | `GpuCommandBuffer.CmdDispatch(uint x, uint y, uint z) : void` | function |
| `Moss_RGPassReadTexture` | `RGPass.ReadTexture(...)` | `RGPass.ReadTexture(RGTexture texture, ResourceState state) : RGTexture` | function |
| `Moss_RGPassWriteTexture` | `RGPass.WriteTexture(...)` | `RGPass.WriteTexture(RGTexture texture, ResourceState state) : RGTexture` | function |
| `Moss_GPUDeviceSupportsAsyncCompute` | `GpuDevice.SupportsAsyncCompute(...)` | `GpuDevice.SupportsAsyncCompute() : int` | function |
| `Moss_TextureSetResidency` | `Texture.SetResidency(...)` | `Texture.SetResidency(ResidencyState state) : void` | function |
| `Moss_ShaderCreate` | `GpuDevice.CreateShader(...)` | `GpuDevice.CreateShader(ShaderDesc desc) : Shader` | function |
| `Moss_ShaderDestroy` | `Shader.Dispose(...)` | `Shader.Dispose() : void` | function |
| `Moss_BindlessRegisterTexture` | `GpuDevice.BindlessRegisterTexture(...)` | `GpuDevice.BindlessRegisterTexture(Span<byte> texture) : BindlessHandle` | function |
| `Moss_BindlessRegisterBuffer` | `GpuBuffer.BindlessRegister(...)` | `GpuBuffer.BindlessRegister() : BindlessHandle` | function |
| `Moss_BindlessUnregister` | `GpuDevice.BindlessUnregister(...)` | `GpuDevice.BindlessUnregister(BindlessHandle handle) : void` | function |
| `Moss_PipelineCreate` | `GpuDevice.CreatePipeline(...)` | `GpuDevice.CreatePipeline(PipelineDesc desc) : PipelineState` | function |
| `Moss_PipelineDestroy` | `GpuDevice.PipelineDestroy(...)` | `GpuDevice.PipelineDestroy(PipelineState pipeline) : void` | function |
| `Moss_WindowResizeVK` | `GPU.WindowResizeVk(...)` | `GPU.WindowResizeVk(VkPhysicalDevice physicalDevice, VkDevice device, VkSwapchainKHR swapchain, VkExtentDIMENSION2D swapchainExtent, StdVector<VkFramebuffer> swapchainFramebuffers, StdVector<VkImageView> swapchainImageViews, VkImage depthImage, VkDeviceMemory depthImageMemory, VkImageView depthImageView) : void` | function |
| `Moss_GPUFatalError` | `GPU.FatalError(...)` | `GPU.FatalError(MossResult result) : void` | function |
| `Moss_SetGPUViewports` | `GpuCommandBuffer.SetViewports(...)` | `GpuCommandBuffer.SetViewports(ReadOnlySpan<GpuViewport> viewports) : void` | function |
| `Moss_SetGPUScissors` | `GpuCommandBuffer.SetScissors(...)` | `GpuCommandBuffer.SetScissors(ReadOnlySpan<Rect> scissors) : void` | function |
| `Moss_ClearGPUTexture` | `GpuCommandBuffer.ClearTexture(...)` | `GpuCommandBuffer.ClearTexture(GpuTexture texture, float color) : void` | function |
| `Moss_ClearGPUDepthStencilTexture` | `GpuCommandBuffer.ClearDepthStencilTexture(...)` | `GpuCommandBuffer.ClearDepthStencilTexture(GpuTexture texture, float depth, byte stencil) : void` | function |
| `Moss_DrawGPUIndexedPrimitivesIndirectCount` | `GpuCommandBuffer.DrawIndexedPrimitivesIndirectCount(...)` | `GpuCommandBuffer.DrawIndexedPrimitivesIndirectCount(GpuBuffer indirectBuffer, ulong offset, GpuBuffer countBuffer, ulong countOffset, uint maxDrawCount, uint stride) : void` | function |
| `Moss_DrawGPUPrimitivesIndirectCount` | `GpuCommandBuffer.DrawPrimitivesIndirectCount(...)` | `GpuCommandBuffer.DrawPrimitivesIndirectCount(GpuBuffer indirectBuffer, ulong offset, GpuBuffer countBuffer, ulong countOffset, uint maxDrawCount, uint stride) : void` | function |
| `Moss_SetGPUDepthBias` | `GpuCommandBuffer.SetDepthBias(...)` | `GpuCommandBuffer.SetDepthBias(float constantFactor, float clamp, float slopeFactor) : void` | function |
| `Moss_CreateGPUSemaphore` | `GpuDevice.CreateSemaphore(...)` | `GpuDevice.CreateSemaphore() : GpuSemaphore` | function |
| `Moss_ReleaseGPUSemaphore` | `GpuSemaphore.Release(...)` | `GpuSemaphore.Release() : void` | function |
| `Moss_SignalGPUSemaphoreValue` | `GpuSemaphore.SignalValue(...)` | `GpuSemaphore.SignalValue(ulong value) : void` | function |
| `Moss_WaitGPUSemaphoreValue` | `GpuSemaphore.WaitValue(...)` | `GpuSemaphore.WaitValue(ulong value, ulong timeoutNs) : bool` | function |
| `Moss_SubmitGPUCommandBufferWithSemaphores` | `GpuCommandBuffer.SubmitWithSemaphores(...)` | `GpuCommandBuffer.SubmitWithSemaphores(GpuSemaphore waitSemaphores, ReadOnlySpan<Uint64T> waitValues, GpuSemaphore signalSemaphores, ReadOnlySpan<Uint64T> signalValues) : void` | function |
| `Moss_GetGPUBindlessCapacity` | `GpuDevice.GetBindlessCapacity(...)` | `GpuDevice.GetBindlessCapacity(ShaderResourceType type) : uint` | function |
| `Moss_CreateGPUPipelineCache` | `GpuDevice.CreatePipelineCache(...)` | `GpuDevice.CreatePipelineCache(ReadOnlySpan<byte> initialData, nuint initialSize) : GpuPipelineCache` | function |
| `Moss_GetGPUPipelineCacheData` | `GpuPipelineCache.GetData(...)` | `GpuPipelineCache.GetData(Span<byte> outData, nuint outSize) : nuint` | function |
| `Moss_ReleaseGPUPipelineCache` | `GpuPipelineCache.Release(...)` | `GpuPipelineCache.Release() : void` | function |

### `Moss_GUI.h` → `Moss.GUI`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `CreateContext` | `GUI.CreateContext(...)` | `GUI.CreateContext(FontAtlas sharedFontAtlas) : GuiContext` | function |
| `DestroyContext` | `GuiContext.Dispose(...)` | `GuiContext.Dispose() : void` | function |
| `GetCurrentContext` | `GUI.GetCurrentContext(...)` | `GUI.GetCurrentContext() : GuiContext` | function |
| `SetCurrentContext` | `GuiContext.SetCurrent(...)` | `GuiContext.SetCurrent() : void` | function |
| `BeginWindow` | `GuiContext.BeginWindow(...)` | `GuiContext.BeginWindow(string name, out bool pOpen, GuiWindowFlags flags) : bool` | function |
| `EndWindow` | `GuiContext.EndWindow(...)` | `GuiContext.EndWindow() : void` | function |
| `BeginWindowChild` (overload 1) | `GuiContext.BeginWindowChild(...)` | `GuiContext.BeginWindowChild(string strId, Float2 size, GuiChildFlags childFlags, GuiWindowFlags windowFlags) : bool` | function |
| `BeginWindowChild` (overload 2) | `GuiContext.BeginWindowChild(...)` | `GuiContext.BeginWindowChild(uint id, Float2 size, GuiChildFlags childFlags, GuiWindowFlags windowFlags) : bool` | function |
| `EndWindowChild` | `GuiContext.EndWindowChild(...)` | `GuiContext.EndWindowChild() : void` | function |
| `BeginContainer` | `GuiContext.BeginContainer(...)` | `GuiContext.BeginContainer(Rect rect) : void` | function |
| `EndContainer` | `GuiContext.EndContainer(...)` | `GuiContext.EndContainer() : void` | function |
| `BeginVContainer` | `GuiContext.BeginVContainer(...)` | `GuiContext.BeginVContainer(Rect rect, float spacing) : void` | function |
| `EndVContainer` | `GuiContext.EndVContainer(...)` | `GuiContext.EndVContainer() : void` | function |
| `BeginHContainer` | `GuiContext.BeginHContainer(...)` | `GuiContext.BeginHContainer(Rect rect, float spacing) : void` | function |
| `EndHContainer` | `GuiContext.EndHContainer(...)` | `GuiContext.EndHContainer() : void` | function |
| `BeginCenterContainer` | `GuiContext.BeginCenterContainer(...)` | `GuiContext.BeginCenterContainer(Rect rect) : void` | function |
| `EndCenterContainer` | `GuiContext.EndCenterContainer(...)` | `GuiContext.EndCenterContainer() : void` | function |
| `BeginMarginContainer` | `GuiContext.BeginMarginContainer(...)` | `GuiContext.BeginMarginContainer(float leftMargin, float rightMargin, float topMargin, float bottomMargin) : void` | function |
| `EndMarginContainer` | `GuiContext.EndMarginContainer(...)` | `GuiContext.EndMarginContainer() : void` | function |
| `BeginVScrollContainer` | `GuiContext.BeginVScrollContainer(...)` | `GuiContext.BeginVScrollContainer() : void` | function |
| `EndVScrollContainer` | `GuiContext.EndVScrollContainer(...)` | `GuiContext.EndVScrollContainer() : void` | function |
| `BeginHScrollContainer` | `GuiContext.BeginHScrollContainer(...)` | `GuiContext.BeginHScrollContainer() : void` | function |
| `EndHScrollContainer` | `GuiContext.EndHScrollContainer(...)` | `GuiContext.EndHScrollContainer() : void` | function |
| `BeginGridContainer` | `GuiContext.BeginGridContainer(...)` | `GuiContext.BeginGridContainer() : void` | function |
| `EndGridContainer` | `GuiContext.EndGridContainer(...)` | `GuiContext.EndGridContainer() : void` | function |
| `ColorRect` | `GuiContext.ColorRect(...)` | `GuiContext.ColorRect(Color color, Float2 size) : void` | function |
| `Text` | `GuiContext.Text(...)` | `GuiContext.Text(string fmt, ... args) : void` | function |
| `RichText` | `GuiContext.RichText(...)` | `GuiContext.RichText(string label) : void` | function |
| `ProgressBar` | `GuiContext.ProgressBar(...)` | `GuiContext.ProgressBar() : void` | function |
| `TextLink` | `GuiContext.TextLink(...)` | `GuiContext.TextLink(string label) : bool` | function |
| `TextLinkOpenURL` | `GuiContext.TextLinkOpenUrl(...)` | `GuiContext.TextLinkOpenUrl(string label, string url) : void` | function |
| `Button` | `GuiContext.Button(...)` | `GuiContext.Button(string label, Rect rectPtr, Mat44 vpMatrix, float defaultWidth, float defaultHeight) : bool` | function |
| `ToggleSwitch` | `GuiContext.ToggleSwitch(...)` | `GuiContext.ToggleSwitch(bool active) : bool` | function |
| `RadioButton` | `GuiContext.RadioButton(...)` | `GuiContext.RadioButton(string label, bool active) : bool` | function |
| `Checkbox` | `GuiContext.Checkbox(...)` | `GuiContext.Checkbox(string label, out bool value, Rect rectPtr, Mat44 vpMatrix, float defaultSize) : bool` | function |
| `SliderFloat` | `GuiContext.SliderFloat(...)` | `GuiContext.SliderFloat(float minValue, float maxValue, float value) : void` | function |
| `SliderDouble` | `GuiContext.SliderDouble(...)` | `GuiContext.SliderDouble(double minValue, double maxValue, double value) : void` | function |
| `SliderInt` | `GuiContext.SliderInt(...)` | `GuiContext.SliderInt(int minValue, int maxValue, int value) : void` | function |
| `SliderFloat2` | `GuiContext.SliderFloat2(...)` | `GuiContext.SliderFloat2(Float2 minValue, Float2 maxValue, Float2 value) : void` | function |
| `SliderDouble2` | `GuiContext.SliderDouble2(...)` | `GuiContext.SliderDouble2(Double2 minValue, Double2 maxValue, Double2 value) : void` | function |
| `SliderInt2` | `GuiContext.SliderInt2(...)` | `GuiContext.SliderInt2(Int2 minValue, Int2 maxValue, Int2 value) : void` | function |
| `SliderFloat3` | `GuiContext.SliderFloat3(...)` | `GuiContext.SliderFloat3(Float3 minValue, Float3 maxValue, Float3 value) : void` | function |
| `SliderDouble3` | `GuiContext.SliderDouble3(...)` | `GuiContext.SliderDouble3(Double3 minValue, Double3 maxValue, Double3 value) : void` | function |
| `SliderInt3` | `GuiContext.SliderInt3(...)` | `GuiContext.SliderInt3(Int3 minValue, Int3 maxValue, Int3 value) : void` | function |
| `SliderFloat4` | `GuiContext.SliderFloat4(...)` | `GuiContext.SliderFloat4(Float4 minValue, Float4 maxValue, Float4 value) : void` | function |
| `SliderDouble4` | `GuiContext.SliderDouble4(...)` | `GuiContext.SliderDouble4(Double4 minValue, Double4 maxValue, Double4 value) : void` | function |
| `SliderInt4` | `GuiContext.SliderInt4(...)` | `GuiContext.SliderInt4(Int4 minValue, Int4 maxValue, Int4 value) : void` | function |
| `DragFloat` | `GuiContext.DragFloat(...)` | `GuiContext.DragFloat(string label, out float value, float vSpeed, float vMin, float vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragFloat2` | `GuiContext.DragFloat2(...)` | `GuiContext.DragFloat2(string label, Float2 value, float vSpeed, float vMin, float vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragFloat3` | `GuiContext.DragFloat3(...)` | `GuiContext.DragFloat3(string label, Float3 value, float vSpeed, float vMin, float vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragFloat4` | `GuiContext.DragFloat4(...)` | `GuiContext.DragFloat4(string label, Float4 value, float vSpeed, float vMin, float vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragFloatRange2` | `GuiContext.DragFloatRange2(...)` | `GuiContext.DragFloatRange2(string label, out float vCurrentMin, out float vCurrentMax, float vSpeed, float vMin, float vMax, string format, string formatMax, GuiSliderFlags flags) : bool` | function |
| `DragInt` | `GuiContext.DragInt(...)` | `GuiContext.DragInt(string label, out int v, float vSpeed, int vMin, int vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragInt2` | `GuiContext.DragInt2(...)` | `GuiContext.DragInt2(string label, int v, float vSpeed, int vMin, int vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragInt3` | `GuiContext.DragInt3(...)` | `GuiContext.DragInt3(string label, int v, float vSpeed, int vMin, int vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragInt4` | `GuiContext.DragInt4(...)` | `GuiContext.DragInt4(string label, int v, float vSpeed, int vMin, int vMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragIntRange2` | `GuiContext.DragIntRange2(...)` | `GuiContext.DragIntRange2(string label, out int vCurrentMin, out int vCurrentMax, float vSpeed, int vMin, int vMax, string format, string formatMax, GuiSliderFlags flags) : bool` | function |
| `DragScalar` | `GuiContext.DragScalar(...)` | `GuiContext.DragScalar(string label, GuiDataType dataType, Span<byte> pData, float vSpeed, ReadOnlySpan<byte> pMin, ReadOnlySpan<byte> pMax, string format, GuiSliderFlags flags) : bool` | function |
| `DragScalarN` | `GuiContext.DragScalarN(...)` | `GuiContext.DragScalarN(string label, GuiDataType dataType, Span<byte> pData, int components, float vSpeed, ReadOnlySpan<byte> pMin, ReadOnlySpan<byte> pMax, string format, GuiSliderFlags flags) : bool` | function |
| `InputText` | `GuiContext.InputText(...)` | `GuiContext.InputText(string label, Span<byte> buf, nuint bufSize, GuiInputTextFlags flags, GuiInputTextCallback callback) : bool` | function |
| `InputTextMultiline` | `GuiContext.InputTextMultiline(...)` | `GuiContext.InputTextMultiline(string label, Span<byte> buf, nuint bufSize, Float2 size, GuiInputTextFlags flags, GuiInputTextCallback callback) : bool` | function |
| `InputTextWithHint` | `GuiContext.InputTextWithHint(...)` | `GuiContext.InputTextWithHint(string label, string hint, Span<byte> buf, nuint bufSize, GuiInputTextFlags flags, GuiInputTextCallback callback) : bool` | function |
| `InputFloat` | `GuiContext.InputFloat(...)` | `GuiContext.InputFloat(string label, out float v, float step, float stepFast, string format, GuiInputTextFlags flags) : bool` | function |
| `InputFloat2` | `GuiContext.InputFloat2(...)` | `GuiContext.InputFloat2(string label, float v, string format, GuiInputTextFlags flags) : bool` | function |
| `InputFloat3` | `GuiContext.InputFloat3(...)` | `GuiContext.InputFloat3(string label, float v, string format, GuiInputTextFlags flags) : bool` | function |
| `InputFloat4` | `GuiContext.InputFloat4(...)` | `GuiContext.InputFloat4(string label, float v, string format, GuiInputTextFlags flags) : bool` | function |
| `InputInt` | `GuiContext.InputInt(...)` | `GuiContext.InputInt(string label, out int v, int step, int stepFast, GuiInputTextFlags flags) : bool` | function |
| `InputInt2` | `GuiContext.InputInt2(...)` | `GuiContext.InputInt2(string label, int v, GuiInputTextFlags flags) : bool` | function |
| `InputInt3` | `GuiContext.InputInt3(...)` | `GuiContext.InputInt3(string label, int v, GuiInputTextFlags flags) : bool` | function |
| `InputInt4` | `GuiContext.InputInt4(...)` | `GuiContext.InputInt4(string label, int v, GuiInputTextFlags flags) : bool` | function |
| `InputDouble` | `GuiContext.InputDouble(...)` | `GuiContext.InputDouble(string label, out double v, double step, double stepFast, string format, GuiInputTextFlags flags) : bool` | function |
| `InputScalar` | `GuiContext.InputScalar(...)` | `GuiContext.InputScalar(string label, GuiDataType dataType, Span<byte> pData, ReadOnlySpan<byte> pStep, ReadOnlySpan<byte> pStepFast, string format, GuiInputTextFlags flags) : bool` | function |
| `InputScalarN` | `GuiContext.InputScalarN(...)` | `GuiContext.InputScalarN(string label, GuiDataType dataType, Span<byte> pData, int components, ReadOnlySpan<byte> pStep, ReadOnlySpan<byte> pStepFast, string format, GuiInputTextFlags flags) : bool` | function |

### `Moss_Network.h` → `Moss.Network`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_Init_Network` | `Network.Initialize(...)` | `Network.Initialize() : void` | function |
| `enet_initialize_with_callbacks` | `NetworkCallbacks.InitializeWith(...)` | `NetworkCallbacks.InitializeWith() : int` | function |
| `Moss_TerminateNetwork` | `Network.Shutdown(...)` | `Network.Shutdown() : void` | function |
| `enet_list_clear` | `NetworkList.Clear(...)` | `NetworkList.Clear() : void` | function |
| `enet_list_insert` | `Network.ListInsert(...)` | `Network.ListInsert(E netListIterator, Span<byte> value) : NetworkListIterator` | function |
| `enet_list_remove` | `Network.ListRemove(...)` | `Network.ListRemove(E netListIterator) : Span<byte>` | function |
| `enet_list_move` | `Network.ListMove(...)` | `Network.ListMove(E netListIterator, Span<byte> value, Span<byte> value) : NetworkListIterator` | function |
| `enet_list_size` | `NetworkList.Size(...)` | `NetworkList.Size() : nuint` | function |
| `enet_time_get` | `Network.TimeGet(...)` | `Network.TimeGet() : uint` | function |
| `enet_time_set` | `Network.TimeSet(...)` | `Network.TimeSet(U int32T) : void` | function |
| `enet_socket_create` | `Network.CreateSocket(...)` | `Network.CreateSocket(NetworkAddressType adressType, NetworkSocketType socketType) : NetworkSocket` | function |
| `enet_socket_bind` | `NetworkAddress.SocketBind(...)` | `NetworkAddress.SocketBind(E netSocket) : int` | function |
| `enet_socket_get_address` | `NetworkAddress.SocketGet(...)` | `NetworkAddress.SocketGet(E netSocket) : int` | function |
| `enet_socket_listen` | `Network.SocketListen(...)` | `Network.SocketListen(E netSocket, I nt) : int` | function |
| `enet_socket_accept` | `NetworkAddress.SocketAccept(...)` | `NetworkAddress.SocketAccept(E netSocket) : NetworkSocket` | function |
| `enet_socket_connect` | `NetworkAddress.SocketConnect(...)` | `NetworkAddress.SocketConnect(E netSocket) : int` | function |
| `enet_socket_send` | `NetworkAddress.SocketSend(...)` | `NetworkAddress.SocketSend(E netSocket, NetworkBuffer value, S izeT) : int` | function |
| `enet_socket_receive` | `NetworkAddress.SocketReceive(...)` | `NetworkAddress.SocketReceive(E netSocket, NetworkBuffer value, S izeT) : int` | function |
| `enet_socket_wait` | `Network.SocketWait(...)` | `Network.SocketWait(E netSocket, out uint value, U int32T) : int` | function |
| `enet_socket_set_option` | `Network.SocketSetOption(...)` | `Network.SocketSetOption(E netSocket, E netSocketOption, I nt) : int` | function |
| `enet_socket_get_option` | `Network.SocketGetOption(...)` | `Network.SocketGetOption(E netSocket, E netSocketOption, out int value) : int` | function |
| `enet_socket_shutdown` | `Network.SocketShutdown(...)` | `Network.SocketShutdown(E netSocket, E netSocketShutdown) : int` | function |
| `enet_socket_destroy` | `Network.SocketDestroy(...)` | `Network.SocketDestroy(E netSocket) : void` | function |
| `enet_socketset_select` | `NetworkSocketSet.SocketsetSelect(...)` | `NetworkSocketSet.SocketsetSelect(E netSocket, NetworkSocketSet value, U int32T) : int` | function |
| `enet_address_set_host_ip` | `NetworkAddress.SetHostIp(...)` | `NetworkAddress.SetHostIp(string hostName) : int` | function |
| `enet_address_set_host` | `NetworkAddress.SetHost(...)` | `NetworkAddress.SetHost(NetworkAddressType type, string hostName) : int` | function |
| `enet_address_get_host_ip` | `NetworkAddress.GetHostIp(...)` | `NetworkAddress.GetHostIp(Span<byte> hostName, nuint nameLength) : int` | function |
| `enet_address_get_host` | `NetworkAddress.GetHost(...)` | `NetworkAddress.GetHost(Span<byte> hostName, nuint nameLength) : int` | function |
| `enet_address_equal_host` | `NetworkAddress.EqualHost(...)` | `NetworkAddress.EqualHost(NetworkAddress secondAddress) : int` | function |
| `enet_address_is_broadcast` | `NetworkAddress.IsBroadcast(...)` | `NetworkAddress.IsBroadcast() : int` | function |
| `enet_address_build_any` | `NetworkAddress.BuildAny(...)` | `NetworkAddress.BuildAny(NetworkAddressType type) : void` | function |
| `enet_address_build_loopback` | `NetworkAddress.BuildLoopback(...)` | `NetworkAddress.BuildLoopback(NetworkAddressType type) : void` | function |
| `enet_address_convert_ipv6` | `NetworkAddress.ConvertIpv6(...)` | `NetworkAddress.ConvertIpv6() : void` | function |
| `enet_crc32` | `NetworkBuffer.Crc32(...)` | `NetworkBuffer.Crc32(S izeT) : uint` | function |
| `enet_host_create` | `NetworkAddress.CreateHost(...)` | `NetworkAddress.CreateHost(NetworkAddressType type, S izeT, S izeT, U int32T, U int32T) : NetworkHost` | function |
| `enet_host_destroy` | `NetworkHost.Dispose(...)` | `NetworkHost.Dispose() : void` | function |
| `enet_host_check_events` | `NetworkHost.CheckEvents(...)` | `NetworkHost.CheckEvents(NetworkEvent value) : int` | function |
| `enet_host_service` | `NetworkHost.Service(...)` | `NetworkHost.Service(NetworkEvent value, U int32T) : int` | function |
| `enet_host_flush` (overload 1) | `NetworkHost.Flush(...)` | `NetworkHost.Flush() : void` | function |
| `enet_host_broadcast` (overload 1) | `NetworkHost.Broadcast(...)` | `NetworkHost.Broadcast(U int8T, NetworkPacket value) : void` | function |
| `enet_host_compress` | `NetworkHost.Compress(...)` | `NetworkHost.Compress(NetworkCompressor value) : void` | function |
| `enet_host_compress_with_range_coder` | `NetworkHost.CompressWithRangeCoder(...)` | `NetworkHost.CompressWithRangeCoder() : int` | function |
| `enet_host_channel_limit` | `NetworkHost.ChannelLimit(...)` | `NetworkHost.ChannelLimit(S izeT) : void` | function |
| `enet_host_bandwidth_limit` | `NetworkHost.BandwidthLimit(...)` | `NetworkHost.BandwidthLimit(U int32T, U int32T) : void` | function |
| `enet_host_bandwidth_throttle` | `NetworkHost.BandwidthThrottle(...)` | `NetworkHost.BandwidthThrottle() : void` | function |
| `enet_host_random_seed` | `Network.HostRandomSeed(...)` | `Network.HostRandomSeed() : uint` | function |
| `enet_host_random` | `NetworkHost.Random(...)` | `NetworkHost.Random() : uint` | function |
| `enet_host_connect` (overload 1) | `NetworkHost.Connect(...)` | `NetworkHost.Connect(NetworkAddress value, S izeT, U int32T) : NetworkPeer` | function |
| `enet_peer_send` | `NetworkPeer.Send(...)` | `NetworkPeer.Send(U int8T, NetworkPacket value) : int` | function |
| `enet_peer_ping` (overload 1) | `NetworkPeer.Ping(...)` | `NetworkPeer.Ping() : void` | function |
| `enet_peer_ping_interval` | `NetworkPeer.PingInterval(...)` | `NetworkPeer.PingInterval(U int32T) : void` | function |
| `enet_peer_timeout` (overload 1) | `NetworkPeer.Timeout(...)` | `NetworkPeer.Timeout(U int32T, U int32T, U int32T) : void` | function |
| `enet_peer_reset` (overload 1) | `NetworkPeer.Reset(...)` | `NetworkPeer.Reset() : void` | function |
| `enet_peer_disconnect` (overload 1) | `NetworkPeer.Disconnect(...)` | `NetworkPeer.Disconnect(U int32T) : void` | function |
| `enet_peer_disconnect_now` (overload 1) | `NetworkPeer.DisconnectNow(...)` | `NetworkPeer.DisconnectNow(U int32T) : void` | function |
| `enet_peer_disconnect_later` | `NetworkPeer.DisconnectLater(...)` | `NetworkPeer.DisconnectLater(U int32T) : void` | function |
| `enet_peer_throttle_configure` | `NetworkPeer.ThrottleConfigure(...)` | `NetworkPeer.ThrottleConfigure(U int32T, U int32T, U int32T) : void` | function |
| `enet_peer_throttle` | `NetworkPeer.Throttle(...)` | `NetworkPeer.Throttle(U int32T) : int` | function |
| `enet_peer_reset_queues` | `NetworkPeer.ResetQueues(...)` | `NetworkPeer.ResetQueues() : void` | function |
| `enet_peer_has_outgoing_commands` | `NetworkPeer.HasOutgoingCommands(...)` | `NetworkPeer.HasOutgoingCommands() : int` | function |
| `enet_peer_setup_outgoing_command` | `NetworkOutgoingCommand.PeerSetup(...)` | `NetworkOutgoingCommand.PeerSetup(NetworkPeer value) : void` | function |
| `enet_peer_queue_outgoing_command` | `NetworkPeer.QueueOutgoingCommand(...)` | `NetworkPeer.QueueOutgoingCommand(NetworkProtocol value, NetworkPacket value, U int32T, U int16T) : NetworkOutgoingCommand` | function |
| `enet_peer_queue_incoming_command` | `NetworkPeer.QueueIncomingCommand(...)` | `NetworkPeer.QueueIncomingCommand(NetworkProtocol value, ReadOnlySpan<byte> value, S izeT, U int32T, U int32T) : NetworkIncomingCommand` | function |
| `enet_peer_queue_acknowledgement` | `NetworkPeer.QueueAcknowledgement(...)` | `NetworkPeer.QueueAcknowledgement(NetworkProtocol value, U int16T) : NetworkAcknowledgement` | function |
| `enet_peer_dispatch_incoming_unreliable_commands` | `NetworkIncomingCommand.PeerDispatchIncomingUnreliableCommands(...)` | `NetworkIncomingCommand.PeerDispatchIncomingUnreliableCommands(NetworkPeer value, NetworkChannel value) : void` | function |
| `enet_peer_dispatch_incoming_reliable_commands` | `NetworkIncomingCommand.PeerDispatchIncomingReliableCommands(...)` | `NetworkIncomingCommand.PeerDispatchIncomingReliableCommands(NetworkPeer value, NetworkChannel value) : void` | function |
| `enet_peer_on_connect` | `NetworkPeer.OnConnect(...)` | `NetworkPeer.OnConnect() : void` | function |
| `enet_peer_on_disconnect` | `NetworkPeer.OnDisconnect(...)` | `NetworkPeer.OnDisconnect() : void` | function |
| `enet_packet_create` | `Network.CreatePacket(...)` | `Network.CreatePacket(ReadOnlySpan<byte> value, S izeT, U int32T) : NetworkPacket` | function |
| `enet_packet_destroy` | `NetworkPacket.Dispose(...)` | `NetworkPacket.Dispose() : void` | function |
| `enet_packet_resize` | `NetworkPacket.Resize(...)` | `NetworkPacket.Resize(S izeT) : int` | function |
| `enet_peer_receive` | `NetworkPeer.Receive(...)` | `NetworkPeer.Receive(Span<byte> channelId) : NetworkPacket` | function |
| `enet_range_coder_create` | `Network.CreateRangeCoder(...)` | `Network.CreateRangeCoder() : Span<byte>` | function |
| `enet_range_coder_destroy` | `Network.RangeCoderDestroy(...)` | `Network.RangeCoderDestroy(Span<byte> value) : void` | function |
| `enet_range_coder_compress` | `NetworkBuffer.RangeCoderCompress(...)` | `NetworkBuffer.RangeCoderCompress(Span<byte> value, S izeT, S izeT, Span<byte> value, S izeT) : nuint` | function |
| `enet_range_coder_decompress` | `Network.RangeCoderDecompress(...)` | `Network.RangeCoderDecompress(Span<byte> value, ReadOnlySpan<byte> value, S izeT, Span<byte> value, S izeT) : nuint` | function |
| `enet_protocol_command_size` | `Network.ProtocolCommandSize(...)` | `Network.ProtocolCommandSize(U int8T) : nuint` | function |
| `enet_packet_get_data` | `NetworkPacket.GetData(...)` | `NetworkPacket.GetData() : Span<byte>` | function |
| `enet_packet_get_user_data` | `NetworkPacket.GetUserData(...)` | `NetworkPacket.GetUserData() : Span<byte>` | function |
| `enet_packet_set_user_data` | `NetworkPacket.SetUserData(...)` | `NetworkPacket.SetUserData() : void` | function |
| `enet_packet_get_length` | `NetworkPacket.GetLength(...)` | `NetworkPacket.GetLength() : int` | function |
| `enet_packet_set_acknowledge_callback` | `NetworkPacket.SetAcknowledgeCallback(...)` | `NetworkPacket.SetAcknowledgeCallback(E netPacketAcknowledgedCallback) : void` | function |
| `enet_packet_set_free_callback` | `NetworkPacket.SetFreeCallback(...)` | `NetworkPacket.SetFreeCallback(E netPacketFreeCallback) : void` | function |
| `enet_packet_check_references` | `NetworkPacket.CheckReferences(...)` | `NetworkPacket.CheckReferences() : int` | function |
| `enet_packet_dispose` | `NetworkPacket.Dispose(...)` | `NetworkPacket.Dispose() : void` | function |
| `enet_host_get_peers_count` | `NetworkHost.GetPeersCount(...)` | `NetworkHost.GetPeersCount() : uint` | function |
| `enet_host_get_packets_sent` | `NetworkHost.GetPacketsSent(...)` | `NetworkHost.GetPacketsSent() : uint` | function |
| `enet_host_get_packets_received` | `NetworkHost.GetPacketsReceived(...)` | `NetworkHost.GetPacketsReceived() : uint` | function |
| `enet_host_get_bytes_sent` | `NetworkHost.GetBytesSent(...)` | `NetworkHost.GetBytesSent() : uint` | function |
| `enet_host_get_bytes_received` | `NetworkHost.GetBytesReceived(...)` | `NetworkHost.GetBytesReceived() : uint` | function |
| `enet_host_set_max_duplicate_peers` | `NetworkHost.SetMaxDuplicatePeers(...)` | `NetworkHost.SetMaxDuplicatePeers(U int16T) : void` | function |
| `enet_host_set_intercept_callback` | `NetworkHost.SetInterceptCallback(...)` | `NetworkHost.SetInterceptCallback(E netInterceptCallback) : void` | function |
| `enet_host_set_checksum_callback` | `NetworkHost.SetChecksumCallback(...)` | `NetworkHost.SetChecksumCallback(E netChecksumCallback) : void` | function |
| `enet_peer_get_id` | `NetworkPeer.GetId(...)` | `NetworkPeer.GetId() : uint` | function |
| `enet_peer_get_ip` | `NetworkPeer.GetIp(...)` | `NetworkPeer.GetIp(Span<byte> value, S izeT) : int` | function |
| `enet_peer_get_port` | `NetworkPeer.GetPort(...)` | `NetworkPeer.GetPort() : ushort` | function |
| `enet_peer_get_mtu` | `NetworkPeer.GetMtu(...)` | `NetworkPeer.GetMtu() : uint` | function |
| `enet_peer_get_state` | `NetworkPeer.GetState(...)` | `NetworkPeer.GetState() : NetworkPeerState` | function |
| `enet_peer_get_rtt` | `NetworkPeer.GetRtt(...)` | `NetworkPeer.GetRtt() : uint` | function |
| `enet_peer_get_last_rtt` | `NetworkPeer.GetLastRtt(...)` | `NetworkPeer.GetLastRtt() : uint` | function |
| `enet_peer_get_lastsendtime` | `NetworkPeer.GetLastsendtime(...)` | `NetworkPeer.GetLastsendtime() : uint` | function |
| `enet_peer_get_lastreceivetime` | `NetworkPeer.GetLastreceivetime(...)` | `NetworkPeer.GetLastreceivetime() : uint` | function |
| `enet_peer_get_packets_throttle` | `NetworkPeer.GetPacketsThrottle(...)` | `NetworkPeer.GetPacketsThrottle() : float` | function |
| `enet_peer_get_data` | `NetworkPeer.GetData(...)` | `NetworkPeer.GetData() : Span<byte>` | function |
| `enet_peer_set_data` | `NetworkPeer.SetData(...)` | `NetworkPeer.SetData(ReadOnlySpan<byte> value) : void` | function |
| `enet_host_create_with_transport` (overload 1) | `NetworkAddress.HostCreateWithTransport(...)` | `NetworkAddress.HostCreateWithTransport(nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth, NetworkTransportType transport) : NetworkHost` | function |
| `enet_host_connect` (overload 2) | `NetworkHost.Connect(...)` | `NetworkHost.Connect(NetworkAddress address, nuint channelCount, uint data) : NetworkPeer` | function |
| `enet_host_connect_ex` (overload 1) | `NetworkHost.ConnectEx(...)` | `NetworkHost.ConnectEx(string endpoint, nuint channelCount, uint data) : NetworkPeer` | function |
| `enet_host_transport_caps` (overload 1) | `NetworkHost.TransportCaps(...)` | `NetworkHost.TransportCaps() : uint` | function |
| `enet_transport_handle_event` | `NetworkTransportEvent.TransportHandle(...)` | `NetworkTransportEvent.TransportHandle(NetworkHost host) : void` | function |
| `enet_peer_reset` (overload 2) | `NetworkPeer.Reset(...)` | `NetworkPeer.Reset() : void` | function |
| `enet_peer_timeout` (overload 2) | `NetworkPeer.Timeout(...)` | `NetworkPeer.Timeout() : void` | function |
| `enet_peer_ping` (overload 2) | `NetworkPeer.Ping(...)` | `NetworkPeer.Ping() : void` | function |
| `enet_packet_encode` | `NetPacket.Encode(...)` | `NetPacket.Encode(Uint8 outBuffer, nuint bufferSize) : nuint` | function |
| `enet_packet_decode` | `Uint8.PacketDecode(...)` | `Uint8.PacketDecode(NetPacket outPacket, nuint bufferSize) : bool` | function |
| `enet_fragment_send` | `NetworkPeer.FragmentSend(...)` | `NetworkPeer.FragmentSend(Uint8 channelId, ReadOnlySpan<Uint8> data) : void` | function |
| `enet_fragment_receive` | `NetworkPeer.FragmentReceive(...)` | `NetworkPeer.FragmentReceive(NetPacket fragment, NetPacket outPacket) : bool` | function |
| `enet_bandwidth_update` | `NetworkHost.BandwidthUpdate(...)` | `NetworkHost.BandwidthUpdate(float deltaTime) : void` | function |
| `enet_peer_send_window` | `NetworkPeer.SendWindow(...)` | `NetworkPeer.SendWindow() : nuint` | function |
| `enet_peer_create_channel` | `NetworkPeer.CreateChannel(...)` | `NetworkPeer.CreateChannel(bool reliable, bool ordered) : int` | function |
| `enet_peer_reset_channels` | `NetworkPeer.ResetChannels(...)` | `NetworkPeer.ResetChannels() : void` | function |
| `enet_peer_disconnect` (overload 2) | `NetworkPeer.Disconnect(...)` | `NetworkPeer.Disconnect(Uint32TT reason) : void` | function |
| `enet_peer_disconnect_now` (overload 2) | `NetworkPeer.DisconnectNow(...)` | `NetworkPeer.DisconnectNow() : void` | function |
| `enet_peer_get_stats` | `NetworkPeer.GetStats(...)` | `NetworkPeer.GetStats(out float rtt, out float packetLoss, Uint32TT sent, Uint32TT received) : void` | function |
| `enet_host_flush` (overload 2) | `NetworkHost.Flush(...)` | `NetworkHost.Flush() : void` | function |
| `enet_host_broadcast` (overload 2) | `NetworkHost.Broadcast(...)` | `NetworkHost.Broadcast(Uint8 channelId, ReadOnlySpan<Uint8> data) : void` | function |
| `enet_host_set_protocol_id` | `NetworkHost.SetProtocolId(...)` | `NetworkHost.SetProtocolId(Uint16TT protocolId) : void` | function |
| `enet_host_get_protocol_id` | `NetworkHost.GetProtocolId(...)` | `NetworkHost.GetProtocolId() : Uint16TT` | function |
| `enet_host_create_with_transport` (overload 2) | `NetworkAddress.HostCreateWithTransport(...)` | `NetworkAddress.HostCreateWithTransport(nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth, NetworkTransportType transport) : NetworkHost` | function |
| `enet_host_connect` (overload 3) | `NetworkHost.Connect(...)` | `NetworkHost.Connect(NetworkAddress value, S izeT, U int32T) : NetworkPeer` | function |
| `enet_host_connect_ex` (overload 2) | `NetworkHost.ConnectEx(...)` | `NetworkHost.ConnectEx(string endpoint, S izeT, U int32T) : NetworkPeer` | function |
| `enet_host_transport_caps` (overload 2) | `NetworkHost.TransportCaps(...)` | `NetworkHost.TransportCaps() : uint` | function |
| `enet_http_server_start` | `Network.HttpServerStart(...)` | `Network.HttpServerStart(string bindAddress, NetworkUint16T port) : NetworkHttpServer` | function |
| `enet_http_server_stop` | `NetworkHttpServer.Stop(...)` | `NetworkHttpServer.Stop() : void` | function |
| `enet_http_register_endpoint` | `NetworkHttpServer.HttpRegisterEndpoint(...)` | `NetworkHttpServer.HttpRegisterEndpoint(string path, NetworkHttpEndpointHandler handler) : void` | function |
| `enet_host_create_secure` | `NetworkAddress.HostCreateSecure(...)` | `NetworkAddress.HostCreateSecure(nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth, NetworkTransportType transport, NetworkTransportSecurity security) : NetworkHost` | function |
| `enet_http_request_async` | `Network.HttpRequestAsync(...)` | `Network.HttpRequestAsync(NetworkHttpRequestDesc desc, NetworkHttpResponseCallback cb) : NetworkHttpRequest` | function |
| `enet_http_request_cancel` | `NetworkHttpRequest.Cancel(...)` | `NetworkHttpRequest.Cancel() : void` | function |
| `enet_http_request_release` | `NetworkHttpRequest.Release(...)` | `NetworkHttpRequest.Release() : void` | function |
| `enet_host_set_connect_validator` | `NetworkHost.SetConnectValidator(...)` | `NetworkHost.SetConnectValidator(NetworkConnectValidator validator) : void` | function |
| `enet_transport_send_stream` | `Network.TransportSendStream(...)` | `Network.TransportSendStream(... args) : int` | function |
| `enet_transport_receive_stream` | `Network.TransportReceiveStream(...)` | `Network.TransportReceiveStream(... args) : int` | function |
| `enet_transport_ws_close` | `NetworkTransportPeer.TransportWsClose(...)` | `NetworkTransportPeer.TransportWsClose(int code) : void` | function |
| `enet_transport_webrtc_signal` | `NetworkWebRTCSignal.TransportWebRtc(...)` | `NetworkWebRTCSignal.TransportWebRtc(NetworkTransport value, NetworkTransportPeer value) : void` | function |
| `enet_host_set_webrtc_signal_callback` | `NetworkHost.SetWebRtcSignalCallback(...)` | `NetworkHost.SetWebRtcSignalCallback(E netWebRTCSignalCallback, Span<byte> value) : void` | function |
| `enet_webrtc_handle_signal` | `NetworkWebRTCSignal.WebRtcHandle(...)` | `NetworkWebRTCSignal.WebRtcHandle(NetworkHost value, NetworkPeer value) : int` | function |

### `Moss_Physics.h` → `Moss.Physics`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `CollideShapeResult_FreeMembers` | `CollideShapeResult.FreeMembers(...)` | `CollideShapeResult.FreeMembers() : void` | function |
| `CollisionEstimationResult_FreeMembers` | `CollisionEstimationResult.FreeMembers(...)` | `CollisionEstimationResult.FreeMembers() : void` | function |
| `BroadPhaseLayerInterfaceMask_Create` | `Physics.CreateBroadPhaseLayerInterfaceMask(...)` | `Physics.CreateBroadPhaseLayerInterfaceMask(uint numBroadPhaseLayers) : BroadPhaseLayerInterface` | function |
| `BroadPhaseLayerInterfaceMask_ConfigureLayer` | `BroadPhaseLayerInterface.MaskConfigureLayer(...)` | `BroadPhaseLayerInterface.MaskConfigureLayer(BroadPhaseLayer broadPhaseLayer, uint groupsToInclude, uint groupsToExclude) : void` | function |
| `BroadPhaseLayerInterfaceTable_Create` | `Physics.CreateBroadPhaseLayerInterfaceTable(...)` | `Physics.CreateBroadPhaseLayerInterfaceTable(uint numObjectLayers, uint numBroadPhaseLayers) : BroadPhaseLayerInterface` | function |
| `BroadPhaseLayerInterfaceTable_MapObjectToBroadPhaseLayer` | `BroadPhaseLayerInterface.TableMapObjectToBroadPhaseLayer(...)` | `BroadPhaseLayerInterface.TableMapObjectToBroadPhaseLayer(ObjectLayer objectLayer, BroadPhaseLayer broadPhaseLayer) : void` | function |
| `ObjectLayerPairFilterMask_Create` | `Physics.CreateObjectLayerPairFilterMask(...)` | `Physics.CreateObjectLayerPairFilterMask() : ObjectLayerPairFilter` | function |
| `ObjectLayerPairFilterMask_GetObjectLayer` | `Physics.ObjectLayerPairFilterMaskGetObjectLayer(...)` | `Physics.ObjectLayerPairFilterMaskGetObjectLayer(uint group, uint mask) : ObjectLayer` | function |
| `ObjectLayerPairFilterMask_GetGroup` | `Physics.ObjectLayerPairFilterMaskGetGroup(...)` | `Physics.ObjectLayerPairFilterMaskGetGroup(ObjectLayer layer) : uint` | function |
| `ObjectLayerPairFilterMask_GetMask` | `Physics.ObjectLayerPairFilterMaskGetMask(...)` | `Physics.ObjectLayerPairFilterMaskGetMask(ObjectLayer layer) : uint` | function |
| `ObjectLayerPairFilterTable_Create` | `Physics.CreateObjectLayerPairFilterTable(...)` | `Physics.CreateObjectLayerPairFilterTable(uint numObjectLayers) : ObjectLayerPairFilter` | function |
| `ObjectLayerPairFilterTable_DisableCollision` | `ObjectLayerPairFilter.TableDisableCollision(...)` | `ObjectLayerPairFilter.TableDisableCollision(ObjectLayer layer1, ObjectLayer layer2) : void` | function |
| `ObjectLayerPairFilterTable_EnableCollision` | `ObjectLayerPairFilter.TableEnableCollision(...)` | `ObjectLayerPairFilter.TableEnableCollision(ObjectLayer layer1, ObjectLayer layer2) : void` | function |
| `ObjectLayerPairFilterTable_ShouldCollide` | `ObjectLayerPairFilter.TableShouldCollide(...)` | `ObjectLayerPairFilter.TableShouldCollide(ObjectLayer layer1, ObjectLayer layer2) : bool` | function |
| `ObjectVsBroadPhaseLayerFilterMask_Create` | `BroadPhaseLayerInterface.CreateObjectVsBroadPhaseLayerFilterMask(...)` | `BroadPhaseLayerInterface.CreateObjectVsBroadPhaseLayerFilterMask() : ObjectVsBroadPhaseLayerFilter` | function |
| `ObjectVsBroadPhaseLayerFilterTable_Create` | `BroadPhaseLayerInterface.CreateObjectVsBroadPhaseLayerFilterTable(...)` | `BroadPhaseLayerInterface.CreateObjectVsBroadPhaseLayerFilterTable(uint numBroadPhaseLayers, ObjectLayerPairFilter objectLayerPairFilter, uint numObjectLayers) : ObjectVsBroadPhaseLayerFilter` | function |
| `DrawSettings_InitDefault` | `DrawSettings.InitDefault(...)` | `DrawSettings.InitDefault() : void` | function |
| `ContactManifold_GetWorldSpaceNormal` | `ContactManifold.GetWorldSpaceNormal(...)` | `ContactManifold.GetWorldSpaceNormal(Vec3 result) : void` | function |
| `ContactManifold_GetPenetrationDepth` | `ContactManifold.GetPenetrationDepth(...)` | `ContactManifold.GetPenetrationDepth() : float` | function |
| `ContactManifold_GetSubShapeID1` | `ContactManifold.GetSubShapeId1(...)` | `ContactManifold.GetSubShapeId1() : SubShapeId` | function |
| `ContactManifold_GetSubShapeID2` | `ContactManifold.GetSubShapeId2(...)` | `ContactManifold.GetSubShapeId2() : SubShapeId` | function |
| `ContactManifold_GetPointCount` | `ContactManifold.GetPointCount(...)` | `ContactManifold.GetPointCount() : uint` | function |
| `ContactManifold_GetWorldSpaceContactPointOn1` | `ContactManifold.GetWorldSpaceContactPointOn1(...)` | `ContactManifold.GetWorldSpaceContactPointOn1(uint index, Vec3 result) : void` | function |
| `ContactManifold_GetWorldSpaceContactPointOn2` | `ContactManifold.GetWorldSpaceContactPointOn2(...)` | `ContactManifold.GetWorldSpaceContactPointOn2(uint index, Vec3 result) : void` | function |
| `CollisionDispatch_CollideShapeVsShape` | `CollideShapeCollectorCallback.CollisionDispatchCollideShapeVsShape(...)` | `CollideShapeCollectorCallback.CollisionDispatchCollideShapeVsShape(Shape shape1, Shape shape2, Vec3 scale1, Vec3 scale2, Mat44 centerOfMassTransform1, Mat44 centerOfMassTransform2, CollideShapeSettings collideShapeSettings, ShapeFilter shapeFilter) : bool` | function |
| `CollisionDispatch_CastShapeVsShapeLocalSpace` | `CastShapeCollectorCallback.CollisionDispatchCastShapeVsShapeLocalSpace(...)` | `CastShapeCollectorCallback.CollisionDispatchCastShapeVsShapeLocalSpace(Vec3 direction, Shape shape1, Shape shape2, Vec3 scale1InShape2LocalSpace, Vec3 scale2, Mat44 centerOfMassTransform1InShape2LocalSpace, Mat44 centerOfMassWorldTransform2, ShapeCastSettings shapeCastSettings, ShapeFilter shapeFilter) : bool` | function |
| `CollisionDispatch_CastShapeVsShapeWorldSpace` | `CastShapeCollectorCallback.CollisionDispatchCastShapeVsShapeWorldSpace(...)` | `CastShapeCollectorCallback.CollisionDispatchCastShapeVsShapeWorldSpace(Vec3 direction, Shape shape1, Shape shape2, Vec3 scale1, Vec3 inScale2, Mat44 centerOfMassWorldTransform1, Mat44 centerOfMassWorldTransform2, ShapeCastSettings shapeCastSettings, ShapeFilter shapeFilter) : bool` | function |
| `Skeleton_Create` | `Physics.CreateSkeleton(...)` | `Physics.CreateSkeleton() : Skeleton` | function |
| `Skeleton_Destroy` | `Skeleton.Dispose(...)` | `Skeleton.Dispose() : void` | function |
| `Skeleton_AddJoint` | `Skeleton.AddJoint(...)` | `Skeleton.AddJoint(string name) : uint` | function |
| `Skeleton_AddJoint2` | `Skeleton.AddJoint2(...)` | `Skeleton.AddJoint2(string name, int parentIndex) : uint` | function |
| `Skeleton_AddJoint3` | `Skeleton.AddJoint3(...)` | `Skeleton.AddJoint3(string name, string parentName) : uint` | function |
| `Skeleton_GetJointCount` | `Skeleton.GetJointCount(...)` | `Skeleton.GetJointCount() : int` | function |
| `Skeleton_GetJoint` | `SkeletonJoint.SkeletonGet(...)` | `SkeletonJoint.SkeletonGet(Skeleton skeleton, int index) : void` | function |
| `Skeleton_GetJointIndex` | `Skeleton.GetJointIndex(...)` | `Skeleton.GetJointIndex(string name) : int` | function |
| `Skeleton_CalculateParentJointIndices` | `Skeleton.CalculateParentJointIndices(...)` | `Skeleton.CalculateParentJointIndices() : void` | function |
| `Skeleton_AreJointsCorrectlyOrdered` | `Skeleton.AreJointsCorrectlyOrdered(...)` | `Skeleton.AreJointsCorrectlyOrdered() : bool` | function |
| `Indexify` | `Physics.Indexify(...)` | `Physics.Indexify(TriangleList inTriangles, VertexList outVertices, IndexedTriangleList outTriangles, float inVertexWeldDistance) : void` | function |
| `Deindexify` | `Physics.Deindexify(...)` | `Physics.Deindexify(VertexList inVertices, IndexedTriangleList inTriangles, TriangleList outTriangles) : void` | function |

### `Moss_Platform.h` → `Moss.Platform`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_CreateWindow` | `Platform.CreateWindow(...)` | `public static Window Platform.CreateWindow(WindowOptions options)` | function |
| `Moss_TerminateWindow` | `Window.Dispose(...)` | `Window.Dispose() : void` | function |
| `Moss_CreateMessageBox` | `Window.ShowMessageBox(...)` | `Window.ShowMessageBox(string title, string message, MessageBoxFlags flags) : bool` | function |
| `Moss_ShouldWindowClose` | `Window.ShouldClose` | `public bool Window.ShouldClose { get; }` | function |
| `Moss_PollEvents` | `Platform.PollEvents(...)` | `Platform.PollEvents() : void` | function |
| `Moss_GetWindowWidth` | `Platform.GetWindowWidth(...)` | `Platform.GetWindowWidth() : int` | function |
| `Moss_GetWindowHeight` | `Platform.GetWindowHeight(...)` | `Platform.GetWindowHeight() : int` | function |
| `Moss_SetWindowTitle` | `Window.Title` | `public string Window.Title { get; set; }` | function |
| `Moss_SetWindowIcon` | `Window.SetIcon(...)` | `Window.SetIcon(Image image) : void` | function |
| `Moss_CloseWindow` | `Window.Dispose(...)` | `Window.Dispose() : void` | function |
| `Moss_MonitorGetPrimary` | `Platform.Monitors.MonitorGetPrimary(...)` | `Platform.Monitors.MonitorGetPrimary() : Monitor` | function |
| `Moss_MonitorGetSecondary` | `Platform.Monitors.MonitorGetSecondary(...)` | `Platform.Monitors.MonitorGetSecondary() : Monitor` | function |
| `Moss_MonitorGetPhysicalSize` | `Monitor.GetPhysicalSize(...)` | `Monitor.GetPhysicalSize(out int widthMm, out int heightMm) : void` | function |
| `Moss_MonitorGetContentScale` | `Monitor.GetContentScale(...)` | `Monitor.GetContentScale(out float xscale, out float yscale) : void` | function |
| `Moss_MonitorGetPosition` | `Monitor.GetPosition(...)` | `Monitor.GetPosition(out int x, out int y) : void` | function |
| `Moss_MonitorGetName` | `Monitor.GetName(...)` | `Monitor.GetName() : string?` | function |
| `Moss_MonitorSetGammaRamp` | `GammaRamp.MonitorSet(...)` | `GammaRamp.MonitorSet(Monitor monitor) : void` | function |
| `Moss_MonitorGetGammaRamp` | `Monitor.GetGammaRamp(...)` | `Monitor.GetGammaRamp() : GammaRamp` | function |
| `Moss_MonitorSetGamma` | `Monitor.SetGamma(...)` | `Monitor.SetGamma(float gamma) : void` | function |
| `Moss_IsKeyPressed` | `Platform.Input.IsKeyPressed(...)` | `Platform.Input.IsKeyPressed(Keyboard key) : bool` | function |
| `Moss_IsReleased` | `Platform.IsReleased(...)` | `Platform.IsReleased(Keyboard key) : bool` | function |
| `Moss_IsKeyJustPressed` | `Platform.Input.IsKeyJustPressed(...)` | `Platform.Input.IsKeyJustPressed(Keyboard key) : bool` | function |
| `Moss_IsKeyJustReleased` | `Platform.Input.IsKeyJustReleased(...)` | `Platform.Input.IsKeyJustReleased(Keyboard key) : bool` | function |
| `Moss_InputGetKey` | `Platform.Input.InputGetKey(...)` | `Platform.Input.InputGetKey() : Keyboard` | function |
| `Moss_IsMousePressed` | `Platform.Input.IsMousePressed(...)` | `Platform.Input.IsMousePressed(Mouse button) : bool` | function |
| `Moss_IsMouseReleased` | `Platform.Input.IsMouseReleased(...)` | `Platform.Input.IsMouseReleased(Mouse button) : bool` | function |
| `Moss_IsMouseJustPressed` | `Platform.Input.IsMouseJustPressed(...)` | `Platform.Input.IsMouseJustPressed(Mouse button) : bool` | function |
| `Moss_IsMouseJustReleased` | `Platform.Input.IsMouseJustReleased(...)` | `Platform.Input.IsMouseJustReleased(Mouse button) : bool` | function |
| `Moss_InputGetMouseButton` | `Platform.Input.InputGetMouseButton(...)` | `Platform.Input.InputGetMouseButton() : Mouse` | function |
| `Moss_GetMousePosition` | `Platform.Input.GetMousePosition(...)` | `public static Point Platform.Input.GetMousePosition()` | function |
| `Moss_SetMousePosition` | `Platform.Input.SetMousePosition(...)` | `Platform.Input.SetMousePosition(int x, int y) : void` | function |
| `Moss_SetMouseVisible` | `Platform.Input.SetMouseVisible(...)` | `Platform.Input.SetMouseVisible(bool visible) : void` | function |
| `Moss_GetNumGamepads` | `Platform.GetNumGamepads(...)` | `Platform.GetNumGamepads() : int` | function |
| `Moss_OpenGamepad` | `Platform.Gamepads.OpenGamepad(...)` | `Platform.Gamepads.OpenGamepad(GamepadId id) : Gamepad` | function |
| `Moss_CloseGamepad` | `Gamepad.Dispose(...)` | `Gamepad.Dispose() : void` | function |
| `Moss_GamepadConnected` | `Gamepad.Connected(...)` | `Gamepad.Connected() : bool` | function |
| `Moss_UpdateGamepads` | `Platform.UpdateGamepads(...)` | `Platform.UpdateGamepads() : void` | function |
| `Moss_IsGamepadButtonPressed` | `Gamepad.IsButtonPressed(...)` | `Gamepad.IsButtonPressed(GamepadButton button) : bool` | function |
| `Moss_IsGamepadButtonJustPressed` | `Gamepad.IsButtonJustPressed(...)` | `Gamepad.IsButtonJustPressed(GamepadButton button) : bool` | function |
| `Moss_IsGamepadButtonJustReleased` | `Gamepad.IsButtonJustReleased(...)` | `Gamepad.IsButtonJustReleased(GamepadButton button) : bool` | function |
| `Moss_GetGamepadAxis` | `Gamepad.GetAxis(...)` | `Gamepad.GetAxis(GamepadAxis axis) : float` | function |
| `Moss_GetGamepadOrientation` | `Gamepad.GetOrientation(...)` | `Gamepad.GetOrientation(out float w, out float x, out float y, out float z) : bool` | function |
| `Moss_GetGamepadPlayerSpaceGyro` | `Gamepad.GetPlayerSpaceGyro(...)` | `Gamepad.GetPlayerSpaceGyro(out float outX, out float outY, float yawRelaxFactor) : void` | function |
| `Moss_GetGamepadWorldSpaceGyro` | `Gamepad.GetWorldSpaceGyro(...)` | `Gamepad.GetWorldSpaceGyro(out float outX, out float outY, float sideReductionThreshold) : void` | function |
| `Moss_ResetGamepadCalibration` | `Gamepad.ResetCalibration(...)` | `Gamepad.ResetCalibration() : void` | function |
| `Moss_RumbleGamepad` | `Gamepad.Rumble(...)` | `Gamepad.Rumble(ushort low, ushort high, uint durationMs) : bool` | function |
| `Moss_RumbleGamepadTriggers` | `Gamepad.RumbleTriggers(...)` | `Gamepad.RumbleTriggers(ushort left, ushort right, uint durationMs) : bool` | function |
| `Moss_SetGamepadLED` | `Gamepad.SetLed(...)` | `Gamepad.SetLed(byte r, byte g, byte b) : bool` | function |
| `Moss_GetGamepadName` | `Gamepad.GetName(...)` | `Gamepad.GetName() : string?` | function |
| `Moss_GetGamepadID` | `Gamepad.GetId(...)` | `Gamepad.GetId() : GamepadId` | function |
| `Moss_GetGamepadPlayerIndex` | `Gamepad.GetPlayerIndex(...)` | `Gamepad.GetPlayerIndex() : int` | function |
| `Moss_GetGamepadPowerInfo` | `Gamepad.GetPowerInfo(...)` | `Gamepad.GetPowerInfo(out int percent) : PowerState` | function |
| `Moss_GetNumGamepadTouchpads` | `Gamepad.GetNumTouchpads(...)` | `Gamepad.GetNumTouchpads() : int` | function |
| `Moss_GetNumGamepadTouchpadFingers` | `Gamepad.GetNumTouchpadFingers(...)` | `Gamepad.GetNumTouchpadFingers() : int` | function |
| `Moss_GetGamepadTouchpadFinger` | `Gamepad.GetTouchpadFinger(...)` | `Gamepad.GetTouchpadFinger(int pad, int finger, out bool down, out float x, out float y, out float pressure) : bool` | function |
| `Moss_GetGamepadMapping` | `Gamepad.GetMapping(...)` | `Gamepad.GetMapping() : string?` | function |
| `Moss_SetGamepadMapping` | `Gamepad.SetMapping(...)` | `Gamepad.SetMapping(string mapping) : bool` | function |
| `Moss_ReloadGamepadMappings` | `Platform.Gamepads.ReloadGamepadMappings(...)` | `Platform.Gamepads.ReloadGamepadMappings() : void` | function |
| `Moss_InputGetGamepadButton` | `Platform.Gamepads.InputGetGamepadButton(...)` | `Platform.Gamepads.InputGetGamepadButton() : GamepadButton` | function |
| `Moss_InputGetGamepadAxis` | `Platform.Gamepads.InputGetGamepadAxis(...)` | `Platform.Gamepads.InputGetGamepadAxis() : GamepadAxis` | function |
| `Moss_GetPenDeviceType` | `Platform.Input.GetPenDeviceType(...)` | `Platform.Input.GetPenDeviceType(PenId instanceId) : PenDeviceType` | function |
| `Moss_GetTouchDeviceName` | `Platform.Input.GetTouchDeviceName(...)` | `Platform.Input.GetTouchDeviceName(TouchId touchId) : string?` | function |
| `Moss_GetTouchDevices` | `Platform.Input.GetTouchDevices(...)` | `Platform.Input.GetTouchDevices(out int count) : TouchId` | function |
| `Moss_GetTouchDeviceType` | `Platform.Input.GetTouchDeviceType(...)` | `Platform.Input.GetTouchDeviceType(TouchId touchId) : TouchDeviceType` | function |
| `Moss_GetTouchFingers` | `Platform.Input.GetTouchFingers(...)` | `Platform.Input.GetTouchFingers(TouchId touchId, out int count) : Finger` | function |
| `Moss_GamepadHasSensor` | `Gamepad.HasSensor(...)` | `Gamepad.HasSensor(GamepadSensorType sensor) : bool` | function |
| `Moss_SetGamepadSensorEnabled` | `Gamepad.SetSensorEnabled(...)` | `Gamepad.SetSensorEnabled(GamepadSensorType sensor, bool enabled) : bool` | function |
| `Moss_GetGamepadSensorData` | `GamepadSensorData.Get(...)` | `GamepadSensorData.Get(Gamepad gp, GamepadSensorType sensor) : bool` | function |
| `Moss_OpenHaptic` | `Platform.Haptics.OpenHaptic(...)` | `Platform.Haptics.OpenHaptic(HapticId id) : Haptic` | function |
| `Moss_CloseHaptic` | `Haptic.Dispose(...)` | `Haptic.Dispose() : void` | function |
| `Moss_CreateHapticEffect` | `Haptic.CreateEffect(...)` | `Haptic.CreateEffect() : HapticId` | function |
| `Moss_DestroyHapticEffect` | `Haptic.DestroyEffect(...)` | `Haptic.DestroyEffect() : void` | function |
| `Moss_GetHapticEffectStatus` | `Haptic.GetEffectStatus(...)` | `Haptic.GetEffectStatus() : bool` | function |
| `Moss_GetHapticFeatures` | `Haptic.GetFeatures(...)` | `Haptic.GetFeatures() : uint` | function |
| `Moss_GetHapticFromID` | `Haptic.GetFromId(...)` | `Haptic.GetFromId() : Haptic` | function |
| `Moss_GetHapticID` | `Haptic.GetId(...)` | `Haptic.GetId() : HapticId` | function |
| `Moss_GetHapticName` | `Haptic.GetName(...)` | `Haptic.GetName() : string?` | function |
| `Moss_GetHapticNameForID` | `Haptic.GetNameForId(...)` | `Haptic.GetNameForId() : string?` | function |
| `Moss_GetHaptics` | `Haptic.GetHaptics(...)` | `Haptic.GetHaptics() : HapticId` | function |
| `Moss_GetMaxHapticEffects` | `Haptic.GetMaxEffects(...)` | `Haptic.GetMaxEffects() : int` | function |
| `Moss_GetMaxHapticEffectsPlaying` | `Haptic.GetMaxEffectsPlaying(...)` | `Haptic.GetMaxEffectsPlaying() : int` | function |
| `Moss_GetNumHapticAxes` | `Haptic.GetNumAxes(...)` | `Haptic.GetNumAxes() : int` | function |
| `Moss_HapticEffectSupported` | `Haptic.EffectSupported(...)` | `Haptic.EffectSupported() : bool` | function |
| `Moss_HapticRumbleSupported` | `Haptic.RumbleSupported(...)` | `Haptic.RumbleSupported() : bool` | function |
| `Moss_InitHapticRumble` | `Haptic.InitRumble(...)` | `Haptic.InitRumble() : bool` | function |
| `Moss_IsJoystickHaptic` | `Gamepad.IsJoystickHaptic(...)` | `Gamepad.IsJoystickHaptic() : bool` | function |
| `Moss_IsMouseHaptic` | `Platform.Haptics.IsMouseHaptic(...)` | `Platform.Haptics.IsMouseHaptic() : bool` | function |
| `Moss_OpenHapticFromJoystick` | `Gamepad.OpenHapticFromJoystick(...)` | `Gamepad.OpenHapticFromJoystick() : Haptic` | function |
| `Moss_OpenHapticFromMouse` | `Platform.Haptics.OpenHapticFromMouse(...)` | `Platform.Haptics.OpenHapticFromMouse() : Haptic` | function |
| `Moss_PauseHaptic` | `Haptic.Pause(...)` | `Haptic.Pause() : bool` | function |
| `Moss_PlayHapticRumble` | `Haptic.PlayRumble(...)` | `Haptic.PlayRumble(float strength, uint length) : bool` | function |
| `Moss_ResumeHaptic` | `Haptic.Resume(...)` | `Haptic.Resume() : bool` | function |
| `Moss_RunHapticEffect` | `Haptic.RunEffect(...)` | `Haptic.RunEffect(uint iterations) : bool` | function |
| `Moss_SetHapticAutocenter` | `Haptic.SetAutocenter(...)` | `Haptic.SetAutocenter(int center) : bool` | function |
| `Moss_SetHapticGain` | `Haptic.SetGain(...)` | `Haptic.SetGain(int gain) : bool` | function |
| `Moss_StopHapticEffect` | `Haptic.StopEffect(...)` | `Haptic.StopEffect(HapticEffectId effect) : bool` | function |
| `Moss_StopHapticEffects` | `Haptic.StopEffects(...)` | `Haptic.StopEffects() : bool` | function |
| `Moss_StopHapticRumble` | `Haptic.StopRumble(...)` | `Haptic.StopRumble() : bool` | function |
| `Moss_UpdateHapticEffect` | `HapticEffect.Update(...)` | `HapticEffect.Update(Haptic haptic, HapticEffectId effect) : bool` | function |
| `Moss_GetAvailableCPUCores` | `Platform.GetAvailableCpuCores(...)` | `Platform.GetAvailableCpuCores() : int` | function |
| `Moss_GetCPUCacheLineSize` | `Platform.GetCpuCacheLineSize(...)` | `Platform.GetCpuCacheLineSize() : int` | function |
| `Moss_GetSystemRAM` | `Platform.GetSystemRam(...)` | `Platform.GetSystemRam() : int` | function |
| `Moss_OpenURL` | `Platform.OpenUrl(...)` | `Platform.OpenUrl(string url) : bool` | function |
| `Moss_GetLocale` | `Platform.GetLocale(...)` | `Platform.GetLocale() : Locale` | function |
| `Moss_GetPowerInfo` | `Platform.GetPowerInfo(...)` | `Platform.GetPowerInfo(out int seconds, out int percent) : PowerState` | function |
| `Moss_IsProcessRunningByName` | `Platform.IsProcessRunningByName(...)` | `Platform.IsProcessRunningByName(string executablePath) : bool` | function |
| `Moss_LoadDynamicLibrary` | `Platform.LoadDynamicLibrary(...)` | `Platform.LoadDynamicLibrary(string libPath) : Span<byte>` | function |
| `Moss_GetLibrarySymbol` | `Platform.GetLibrarySymbol(...)` | `Platform.GetLibrarySymbol(Span<byte> handle, string symbolName) : Span<byte>` | function |
| `Moss_UnloadDynamicLibrary` | `Platform.UnloadDynamicLibrary(...)` | `Platform.UnloadDynamicLibrary(Span<byte> handle) : void` | function |
| `Moss_SetFramebufferResizeCallback` | `Platform.SetFramebufferResizeCallback(...)` | `Platform.SetFramebufferResizeCallback(FramebufferResizeCallback callback) : void` | function |
| `Moss_SetWindowSizeCallback` | `Platform.SetWindowSizeCallback(...)` | `Platform.SetWindowSizeCallback(WindowSizeCallback callback) : void` | function |
| `Moss_SetWindowResizeCallback` | `Platform.SetWindowResizeCallback(...)` | `Platform.SetWindowResizeCallback(WindowResizeCallback callback) : void` | function |
| `Moss_SetWindowPositionCallback` | `Platform.SetWindowPositionCallback(...)` | `Platform.SetWindowPositionCallback(WindowPositionCallback callback) : void` | function |
| `Moss_SetWindowFocusCallback` | `Platform.SetWindowFocusCallback(...)` | `Platform.SetWindowFocusCallback(WindowFocusCallback callback) : void` | function |
| `Moss_SetWindowContentScaleCallback` | `Platform.SetWindowContentScaleCallback(...)` | `Platform.SetWindowContentScaleCallback(WindowContentScaleCallback callback) : void` | function |
| `Moss_SetMonitorCallback` | `Platform.Monitors.SetMonitorCallback(...)` | `Platform.Monitors.SetMonitorCallback(MonitorCallback callback) : void` | function |
| `Moss_GetCameras` | `Platform.GetCameras(...)` | `Platform.GetCameras(out int count) : CameraId` | function |
| `Moss_GetCameraName` | `Platform.Cameras.GetCameraName(...)` | `Platform.Cameras.GetCameraName(CameraId cameraId) : string?` | function |
| `Moss_GetCameraPosition` | `Platform.Cameras.GetCameraPosition(...)` | `Platform.Cameras.GetCameraPosition(CameraId cameraId) : CameraPosition` | function |
| `Moss_GetCurrentCameraDriver` | `Platform.Cameras.GetCurrentCameraDriver(...)` | `Platform.Cameras.GetCurrentCameraDriver() : string?` | function |
| `Moss_GetNumCameraDrivers` | `Platform.Cameras.GetNumCameraDrivers(...)` | `Platform.Cameras.GetNumCameraDrivers() : int` | function |
| `Moss_GetCameraSupportedFormats` | `Platform.Cameras.GetCameraSupportedFormats(...)` | `Platform.Cameras.GetCameraSupportedFormats(CameraId cameraId, out int count) : CameraSpec` | function |
| `Moss_AcquireCameraFrame` | `Capture.AcquireCameraFrame(...)` | `Capture.AcquireCameraFrame(out ulong timestampNs) : Surface` | function |
| `Moss_ReleaseCameraFrame` | `Capture.ReleaseCameraFrame(...)` | `Capture.ReleaseCameraFrame(Surface frame) : void` | function |
| `Moss_GetCameraFormat` | `Capture.GetCameraFormat(...)` | `Capture.GetCameraFormat(CameraSpec outSpec) : bool` | function |
| `Moss_GetCameraPermissionState` | `Capture.GetCameraPermissionState(...)` | `Capture.GetCameraPermissionState() : CameraPermissionState` | function |
| `Moss_GetCameraProperties` (overload 1) | `Capture.GetCameraProperties(...)` | `Capture.GetCameraProperties() : PropertiesId` | function |
| `Moss_CloseCamera` | `Capture.CloseCamera(...)` | `Capture.CloseCamera() : void` | function |
| `Moss_GetCameraID` | `Capture.GetCameraId(...)` | `Capture.GetCameraId() : CameraId` | function |
| `Moss_GetCameraProperties` (overload 2) | `Capture.GetCameraProperties(...)` | `Capture.GetCameraProperties() : PropertiesId` | function |
| `Moss_OpenCapture` | `CameraSpec.OpenCapture(...)` | `CameraSpec.OpenCapture(CameraId id) : Capture` | function |
| `Moss_CopyFile` | `Platform.Storage.CopyFile(...)` | `Platform.Storage.CopyFile(string srcPath, string dstPath, bool overwrite) : bool` | function |
| `Moss_CreateDirectory` | `Platform.Storage.CreateDirectory(...)` | `Platform.Storage.CreateDirectory(string path, bool recursive) : bool` | function |
| `Moss_RemovePath` | `Platform.Storage.RemovePath(...)` | `Platform.Storage.RemovePath(string path, bool recursive) : bool` | function |
| `Moss_RenamePath` | `Platform.Storage.RenamePath(...)` | `Platform.Storage.RenamePath(string oldPath, string newPath, bool overwrite) : bool` | function |
| `Moss_GetPathInfo` | `Platform.Storage.GetPathInfo(...)` | `Platform.Storage.GetPathInfo(string path, PathInfo outInfo) : bool` | function |
| `Moss_GetCurrentDirectory` | `Platform.Storage.GetCurrentDirectory(...)` | `Platform.Storage.GetCurrentDirectory(Span<byte> outPath, int maxLen) : bool` | function |
| `Moss_GetBasePath` | `Platform.Storage.GetBasePath(...)` | `Platform.Storage.GetBasePath(Span<byte> outPath, int maxLen) : bool` | function |
| `Moss_GetUserFolder` | `Platform.GetUserFolder(...)` | `Platform.GetUserFolder(UserFolder folder, Span<byte> outPath, int maxLen) : bool` | function |
| `Moss_GetPrefPath` | `Platform.Storage.GetPrefPath(...)` | `Platform.Storage.GetPrefPath(string orgName, string appName, Span<byte> outPath, int maxLen) : bool` | function |
| `Moss_EnumerateDirectory` | `Platform.Storage.EnumerateDirectory(...)` | `Platform.Storage.EnumerateDirectory(string path, bool recursive, DirectoryIterateFn callback) : bool` | function |
| `Moss_GlobDirectory` | `Platform.Storage.GlobDirectory(...)` | `Platform.Storage.GlobDirectory(string pattern, DirectoryIterateFn callback) : bool` | function |
| `Moss_ShowFileDialogWithProperties` | `DialogFileFilter.ShowFileDialogWithProperties(...)` | `DialogFileFilter.ShowFileDialogWithProperties(DialogFileCallback callback, Window window, int nfilters, string defaultLocation) : void` | function |
| `Moss_ShowOpenFolderDialog` | `DialogFileFilter.ShowOpenFolderDialog(...)` | `DialogFileFilter.ShowOpenFolderDialog(DialogFileCallback callback, Window window, int nfilters, string defaultLocation, bool allowMany) : void` | function |
| `Moss_ShowOpenFileDialog` | `Window.ShowOpenFileDialog(...)` | `Window.ShowOpenFileDialog(DialogFileCallback callback, string defaultLocation, bool allowMany) : void` | function |
| `Moss_ShowSaveFileDialog` | `Platform.Dialogs.ShowSaveFileDialog(...)` | `Platform.Dialogs.ShowSaveFileDialog(FileDialogType type, DialogFileCallback callback, PropertiesId props) : void` | function |
| `Moss_CloseStorage` | `Storage.Dispose(...)` | `Storage.Dispose() : bool` | function |
| `Moss_CopyStorageFile` | `Storage.CopyFile(...)` | `Storage.CopyFile(string oldpath, string newpath) : bool` | function |
| `Moss_CreateStorageDirectory` | `Storage.CreateDirectory(...)` | `Storage.CreateDirectory(string path) : bool` | function |
| `Moss_EnumerateStorageDirectory` | `Storage.EnumerateDirectory(...)` | `Storage.EnumerateDirectory(string path, EnumerateDirectoryCallback callback) : bool` | function |
| `Moss_GetStorageFileSize` | `Storage.GetFileSize(...)` | `Storage.GetFileSize(string path, Uint64 length) : bool` | function |
| `Moss_GetStorageSpaceRemaining` | `Storage.GetSpaceRemaining(...)` | `Storage.GetSpaceRemaining() : ulong` | function |
| `Moss_GetStoragePathInfo` | `Storage.GetPathInfo(...)` | `Storage.GetPathInfo(string path, PathInfo info) : bool` | function |
| `Moss_GlobStorageDirectory` | `Storage.GlobDirectory(...)` | `Storage.GlobDirectory(string path, string pattern, GlobFlags flags, out int count) : string?` | function |
| `Moss_OpenFileStorage` | `Platform.Storage.OpenFileStorage(...)` | `Platform.Storage.OpenFileStorage(string path) : Storage` | function |
| `Moss_OpenStorage` | `Storage.Open(...)` | `Storage.Open() : Storage` | function |
| `Moss_OpenTitleStorage` | `Platform.Storage.OpenTitleStorage(...)` | `Platform.Storage.OpenTitleStorage(string override, PropertiesId props) : Storage` | function |
| `Moss_OpenUserStorage` | `Platform.Storage.OpenUserStorage(...)` | `Platform.Storage.OpenUserStorage(string org, string app, PropertiesId props) : Storage` | function |
| `Moss_ReadStorageFile` | `Storage.ReadFile(...)` | `Storage.ReadFile(string path, Span<byte> destination, Uint64 length) : bool` | function |
| `Moss_RemoveStoragePath` | `Storage.RemovePath(...)` | `Storage.RemovePath(string path) : bool` | function |
| `Moss_RenameStoragePath` | `Storage.RenamePath(...)` | `Storage.RenamePath(string oldpath, string newpath) : bool` | function |
| `Moss_StorageReady` | `Storage.Ready(...)` | `Storage.Ready() : bool` | function |
| `Moss_WriteStorageFile` | `Storage.WriteFile(...)` | `Storage.WriteFile(string path, ReadOnlySpan<byte> source, Uint64 length) : bool` | function |
| `Moss_CreateEmbeddedWindow` | `Platform.CreateEmbeddedWindow(...)` | `Platform.CreateEmbeddedWindow() : Window` | function |
| `Moss_Terminate_EmbeddedWindow` | `Platform.TerminateEmbeddedWindow(...)` | `Platform.TerminateEmbeddedWindow() : void` | function |
| `Moss_EmbeddedWindow_OnOrientationChanged` | `Platform.EmbeddedWindowOnOrientationChanged(...)` | `Platform.EmbeddedWindowOnOrientationChanged(bool focused) : void` | function |
| `Moss_EmbeddedWindow_OnResume` | `Platform.EmbeddedWindowOnResume(...)` | `Platform.EmbeddedWindowOnResume() : void` | function |
| `Moss_EmbeddedWindow_OnPause` | `Platform.EmbeddedWindowOnPause(...)` | `Platform.EmbeddedWindowOnPause() : void` | function |
| `Moss_MakeContextCurrent` | `Window.MakeContextCurrent(...)` | `Window.MakeContextCurrent() : void` | function |
| `Moss_SwapBuffers` | `Platform.SwapBuffers(...)` | `Platform.SwapBuffers() : void` | function |
| `Moss_SwapBuffersInterval` | `Platform.SwapBuffersInterval(...)` | `Platform.SwapBuffersInterval(int interval) : void` | function |
| `Moss_GetProcAddress` | `Platform.GetProcAddress(...)` | `Platform.GetProcAddress(string procname) : Span<byte>` | function |
| `Moss_CreateWindowSurface` | `Window.CreateVulkanSurface(...)` | `Window.CreateVulkanSurface(VkInstance instance, VkAllocationCallbacks? allocator = null) : VkSurfaceKHR` | function |
| `Moss_VulkanSupported` | `Platform.Supported(...)` | `Platform.Supported() : int` | function |
| `Moss_GetRequiredInstanceExtensions` | `Platform.GetRequiredInstanceExtensions(...)` | `Platform.GetRequiredInstanceExtensions(out uint count) : string?` | function |
| `Moss_GetInstanceProcAddress` | `Platform.GetInstanceProcAddress(...)` | `Platform.GetInstanceProcAddress(VkInstance instance, string procname) : Span<byte>` | function |
| `Moss_GetPhysicalDevicePresentationSupport` | `Window.GetPhysicalDevicePresentationSupport(...)` | `Window.GetPhysicalDevicePresentationSupport(VkPhysicalDevice device, uint queuefamily) : int` | function |
| `Moss_Metal_CreateView` | `Window.CreateView(...)` | `Window.CreateView() : MetalView` | function |
| `Moss_Metal_DestroyView` | `Platform.DestroyView(...)` | `Platform.DestroyView(MetalView view) : void` | function |
| `Moss_Metal_GetLayer` | `Platform.GetLayer(...)` | `Platform.GetLayer(MetalView view) : Span<byte>` | function |
| `Moss_Metal_Resize` | `Platform.Resize(...)` | `Platform.Resize(MetalView handle, uint width, uint height) : void` | function |

### `Moss_Renderer.h` → `Moss.Renderer`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_RendererCreateMaterial` | `Renderer.CreateMaterial(...)` | `Renderer.CreateMaterial(MaterialDesc desc) : Material` | function |
| `Moss_RendererDestroyMaterial` | `Renderer.DestroyMaterial(...)` | `Renderer.DestroyMaterial(Material material) : void` | function |
| `Moss_MaterialSetUniform` | `Material.SetUniform(...)` | `Material.SetUniform(string name, ReadOnlySpan<byte> data, uint size) : int` | function |
| `Moss_MaterialSetTexture` | `Material.SetTexture(...)` | `Material.SetTexture(MaterialTextureType type, Texture texture, GpuSampler sampler) : int` | function |
| `Moss_RendererCreatePipeline` | `Renderer.CreatePipeline(...)` | `Renderer.CreatePipeline(PipelineDesc desc) : PipelineState` | function |
| `Moss_PipelineBind` | `Renderer.PipelineBind(...)` | `Renderer.PipelineBind(PipelineState pipeline) : void` | function |
| `Moss_PipelineUnbind` | `Renderer.PipelineUnbind(...)` | `Renderer.PipelineUnbind(PipelineState pipeline) : void` | function |
| `Moss_PipelineSetUniform` | `Renderer.PipelineSetUniform(...)` | `Renderer.PipelineSetUniform(PipelineState pipeline, string name, ReadOnlySpan<byte> data, uint size) : void` | function |
| `Moss_PipelineFlush` | `Renderer.PipelineFlush(...)` | `Renderer.PipelineFlush(PipelineState pipeline) : void` | function |
| `Moss_PipelineValidate` | `Renderer.PipelineValidate(...)` | `Renderer.PipelineValidate(PipelineState pipeline) : int` | function |
| `Moss_RendererCreateResourceSet` | `Renderer.CreateResourceSet(...)` | `Renderer.CreateResourceSet(ResourceSetLayoutDesc desc) : ResourceSet` | function |
| `Moss_RendererDestroyResourceSet` | `Renderer.DestroyResourceSet(...)` | `Renderer.DestroyResourceSet(ResourceSet set) : void` | function |
| `Moss_ResourceSetBindUniformBuffer` | `ResourceSet.BindUniformBuffer(...)` | `ResourceSet.BindUniformBuffer(uint binding, GpuBuffer buffer) : void` | function |
| `Moss_ResourceSetBindUniformBufferRange` | `ResourceSet.BindUniformBufferRange(...)` | `ResourceSet.BindUniformBufferRange(uint binding, GpuBuffer buffer, nuint offset, nuint size) : void` | function |
| `Moss_ResourceSetBindStorageBuffer` | `ResourceSet.BindStorageBuffer(...)` | `ResourceSet.BindStorageBuffer(uint binding, GpuBuffer buffer) : void` | function |
| `Moss_ResourceSetBindTexture` | `ResourceSet.BindTexture(...)` | `ResourceSet.BindTexture(uint binding, Texture texture) : void` | function |
| `Moss_ResourceSetBindSampler` | `ResourceSet.BindSampler(...)` | `ResourceSet.BindSampler(uint binding, GpuSampler sampler) : void` | function |
| `Moss_ResourceSetBind` | `Renderer.ResourceSetBind(...)` | `Renderer.ResourceSetBind(PipelineState pipeline, uint setIndex, ResourceSet set) : void` | function |
| `Moss_RendererCreateFramebuffer` | `Renderer.CreateFramebuffer(...)` | `Renderer.CreateFramebuffer(FramebufferDesc desc) : Framebuffer` | function |
| `Moss_RendererDestroyFramebuffer` | `Renderer.DestroyFramebuffer(...)` | `Renderer.DestroyFramebuffer(Framebuffer framebuffer) : void` | function |
| `Moss_FramebufferResize` | `Framebuffer.Resize(...)` | `Framebuffer.Resize(uint width, uint height) : void` | function |
| `Moss_PostProcessExecute` | `Renderer.PostProcessExecute(...)` | `Renderer.PostProcessExecute(PostProcessPass pass) : void` | function |
| `Moss_FramebufferBegin` | `Renderer.FramebufferBegin(...)` | `Renderer.FramebufferBegin(Framebuffer framebuffer) : void` | function |
| `Moss_FramebufferEnd` | `Renderer.FramebufferEnd(...)` | `Renderer.FramebufferEnd(Framebuffer framebuffer) : void` | function |
| `Moss_TextureBarrier` | `Renderer.TextureBarrier(...)` | `Renderer.TextureBarrier(Texture texture, ResourceState oldState, ResourceState newState) : void` | function |
| `Moss_RendererGetBackbuffer` | `Renderer.GetBackbuffer(...)` | `Renderer.GetBackbuffer() : Framebuffer` | function |
| `Moss_RenderSubViewport` | `SubViewport.Render(...)` | `SubViewport.Render(GpuCommandBuffer cmd, SubViewportRecordFn record) : void` | function |
| `Moss_RendererCreate` | `Renderer.Create(...)` | `Renderer.Create(RendererDesc desc) : Renderer` | function |
| `Moss_CreateRenderer` | `Renderer.CreateRenderer(...)` | `Renderer.CreateRenderer(Window window) : Renderer` | function |
| `Moss_RendererDestroy` | `Renderer.Dispose(...)` | `Renderer.Dispose() : void` | function |
| `Moss_TerminateRenderer` | `Renderer.Dispose(...)` | `Renderer.Dispose() : void` | function |
| `Moss_RendererBeginFrame` | `Renderer.BeginFrame(...)` | `Renderer.BeginFrame() : void` | function |
| `Moss_RendererBeginFrameEx` | `Renderer.BeginFrameEx(...)` | `Renderer.BeginFrameEx(RenderFrameDesc desc) : void` | function |
| `Moss_RendererEndFrame` | `Renderer.EndFrame(...)` | `Renderer.EndFrame() : void` | function |
| `Moss_RendererGetGPUDevice` | `Renderer.GetGpuDevice(...)` | `Renderer.GetGpuDevice() : GpuDevice` | function |
| `Moss_MeshLoadFBX` | `Renderer.MeshLoadFbx(...)` | `Renderer.MeshLoadFbx(string filePath) : Mesh` | function |
| `Moss_MeshLoadOBJ` | `Renderer.MeshLoadObj(...)` | `Renderer.MeshLoadObj(string filePath) : Mesh` | function |
| `Moss_MeshLoadGLB` | `Renderer.MeshLoadGlb(...)` | `Renderer.MeshLoadGlb(string filePath) : Mesh` | function |
| `Moss_ModelLoadFBX` | `Renderer.ModelLoadFbx(...)` | `Renderer.ModelLoadFbx(string filePath) : Model` | function |
| `Moss_ModelLoadOBJ` | `Renderer.ModelLoadObj(...)` | `Renderer.ModelLoadObj(string filePath) : Model` | function |
| `Moss_ModelLoadGLB` | `Renderer.ModelLoadGlb(...)` | `Renderer.ModelLoadGlb(string filePath) : Model` | function |
| `Moss_ModelLoad` | `Renderer.ModelLoad(...)` | `Renderer.ModelLoad(string filePath) : Model` | function |
| `Moss_ModelGetLastLoadError` | `Renderer.ModelGetLastLoadError(...)` | `Renderer.ModelGetLastLoadError() : ModelLoadError` | function |
| `Moss_ModelGetLastLoadErrorMessage` | `Renderer.ModelGetLastLoadErrorMessage(...)` | `Renderer.ModelGetLastLoadErrorMessage() : string?` | function |
| `Moss_ModelFindBone` | `Model.FindBone(...)` | `Model.FindBone(string name) : int` | function |
| `Moss_ModelFindMorphTarget` | `Model.FindMorphTarget(...)` | `Model.FindMorphTarget(string name) : int` | function |
| `Moss_ModelSetBoneTransform` | `Model.SetBoneTransform(...)` | `Model.SetBoneTransform(uint boneIndex, ReadOnlySpan<float> transform16) : bool` | function |
| `Moss_ModelSetMorphWeight` | `Model.SetMorphWeight(...)` | `Model.SetMorphWeight(uint morphIndex, float weight) : bool` | function |
| `Moss_RendererDrawMesh` | `Renderer.DrawMesh(...)` | `Renderer.DrawMesh(DrawMeshDesc desc) : void` | function |
| `Moss_RendererDrawModelDesc` | `Renderer.DrawModelDesc(...)` | `Renderer.DrawModelDesc(DrawModelDesc desc) : void` | function |
| `Moss_RendererDrawModel` | `Renderer.DrawModel(...)` | `Renderer.DrawModel(Model model, ReadOnlySpan<float> transform) : void` | function |
| `Moss_RendererCreateFont` | `Renderer.CreateFont(...)` | `Renderer.CreateFont(FontDesc desc) : Font` | function |
| `Moss_RendererDestroyFont` | `Renderer.DestroyFont(...)` | `Renderer.DestroyFont(Font font) : void` | function |
| `Moss_FontLoad` | `Renderer.FontLoad(...)` | `Renderer.FontLoad(string path, float pixelSize) : Font` | function |
| `Moss_FontCreateFromMemory` | `Renderer.FontCreateFromMemory(...)` | `Renderer.FontCreateFromMemory(ReadOnlySpan<byte> data, nuint dataSize, float pixelSize) : Font` | function |
| `Moss_FontDestroy` | `Font.Dispose(...)` | `Font.Dispose() : void` | function |
| `Moss_FontGetMetrics` | `Font.GetMetrics(...)` | `Font.GetMetrics() : FontMetrics` | function |
| `Moss_FontGetGlyph` | `Font.GetGlyph(...)` | `Font.GetGlyph(uint codepoint) : FontGlyph` | function |
| `Moss_FontGetAtlasPixels` | `Font.GetAtlasPixels(...)` | `Font.GetAtlasPixels(out uint width, out uint height, out uint stride) : UnsignedChar` | function |
| `Moss_FontGetAtlasTexture` | `Font.GetAtlasTexture(...)` | `Font.GetAtlasTexture() : Texture` | function |
| `Moss_FontUploadAtlas` | `Renderer.FontUploadAtlas(...)` | `Renderer.FontUploadAtlas(Font font) : bool` | function |
| `Moss_FontMeasureText` | `Font.MeasureText(...)` | `Font.MeasureText(string text, float scale) : TextMeasure` | function |
| `Moss_FontBuildTextQuads` | `Renderer.FontBuildTextQuads(...)` | `Renderer.FontBuildTextQuads(DrawTextDesc desc, FontVertex outVertices, nuint vertexCapacity) : nuint` | function |
| `Moss_RendererDrawText` | `Renderer.DrawText(...)` | `Renderer.DrawText(DrawTextDesc desc) : void` | function |
| `Moss_RendererGetFontBackendMask` | `Renderer.GetFontBackendMask(...)` | `Renderer.GetFontBackendMask() : uint` | function |
| `Moss_RendererBackendSupportsFonts` | `Renderer.BackendSupportsFonts(...)` | `Renderer.BackendSupportsFonts(GpuBackendType backend) : bool` | function |
| `Moss_RendererDrawSprite` | `Renderer.DrawSprite(...)` | `Renderer.DrawSprite(SpriteDesc desc) : void` | function |
| `Moss_RendererCreateSpriteBatch` | `Renderer.CreateSpriteBatch(...)` | `Renderer.CreateSpriteBatch(uint capacity) : SpriteBatch` | function |
| `Moss_RendererDestroySpriteBatch` | `Renderer.DestroySpriteBatch(...)` | `Renderer.DestroySpriteBatch(SpriteBatch batch) : void` | function |
| `Moss_SpriteBatchClear` | `SpriteBatch.Clear(...)` | `SpriteBatch.Clear() : void` | function |
| `Moss_SpriteBatchAdd` | `SpriteBatch.Add(...)` | `SpriteBatch.Add(SpriteDesc desc) : int` | function |
| `Moss_RendererDrawSpriteBatch` | `Renderer.DrawSpriteBatch(...)` | `Renderer.DrawSpriteBatch(SpriteBatch batch) : void` | function |
| `Moss_RendererCreateDebugDrawList` | `Renderer.CreateDebugDrawList(...)` | `Renderer.CreateDebugDrawList(uint capacity) : DebugDrawList` | function |
| `Moss_RendererDestroyDebugDrawList` | `Renderer.DestroyDebugDrawList(...)` | `Renderer.DestroyDebugDrawList(DebugDrawList list) : void` | function |
| `Moss_DebugDrawListClear` | `DebugDrawList.Clear(...)` | `DebugDrawList.Clear() : void` | function |
| `Moss_DebugDrawLine` | `DebugDrawList.DebugDrawLine(...)` | `DebugDrawList.DebugDrawLine(DebugLineDesc desc) : int` | function |
| `Moss_DebugDrawBox` | `DebugDrawList.DebugDrawBox(...)` | `DebugDrawList.DebugDrawBox(DebugBoxDesc desc) : int` | function |
| `Moss_DebugDrawSphere` | `DebugDrawList.DebugDrawSphere(...)` | `DebugDrawList.DebugDrawSphere(DebugSphereDesc desc) : int` | function |
| `Moss_DebugDrawNavmesh` | `DebugDrawList.DebugDrawNavmesh(...)` | `DebugDrawList.DebugDrawNavmesh(DebugNavmeshDesc desc) : int` | function |
| `Moss_DebugDrawPhysicsContact` | `DebugDrawList.DebugDrawPhysicsContact(...)` | `DebugDrawList.DebugDrawPhysicsContact(DebugPhysicsContactDesc desc) : int` | function |
| `Moss_RendererDrawDebugList` | `Renderer.DrawDebugList(...)` | `Renderer.DrawDebugList(DebugDrawList list) : void` | function |
| `Moss_CameraCreate2D` | `Renderer.CameraCreateDimension2D(...)` | `Renderer.CameraCreateDimension2D(CameraDIMENSION2DDesc desc) : Camera` | function |
| `Moss_CameraCreate3D` | `Renderer.CameraCreateDimension3D(...)` | `Renderer.CameraCreateDimension3D(CameraDIMENSION3DDesc desc) : Camera` | function |
| `Moss_CameraCreateEditor` | `Renderer.CameraCreateEditor(...)` | `Renderer.CameraCreateEditor(EditorCameraDesc desc) : Camera` | function |
| `Moss_CameraDestroy` | `Camera.Dispose(...)` | `Camera.Dispose() : void` | function |
| `Moss_RendererSetCamera` | `Renderer.SetCamera(...)` | `Renderer.SetCamera(Camera camera) : void` | function |
| `Moss_RendererCreateShadowMap` | `Renderer.CreateShadowMap(...)` | `Renderer.CreateShadowMap(ShadowMapDesc desc) : ShadowMap` | function |
| `Moss_RendererDestroyShadowMap` | `Renderer.DestroyShadowMap(...)` | `Renderer.DestroyShadowMap(ShadowMap shadowMap) : void` | function |
| `Moss_RendererBeginShadowPass` | `Renderer.BeginShadowPass(...)` | `Renderer.BeginShadowPass(ShadowMap shadowMap) : void` | function |
| `Moss_RendererEndShadowPass` | `Renderer.EndShadowPass(...)` | `Renderer.EndShadowPass(ShadowMap shadowMap) : void` | function |
| `Moss_MeshDraw` | `Mesh.Draw(...)` | `Mesh.Draw() : void` | function |
| `Moss_ModelDraw` | `Model.Draw(...)` | `Model.Draw() : void` | function |
| `Moss_MeshRemove` | `Mesh.Dispose(...)` | `Mesh.Dispose() : void` | function |
| `Moss_ModelRemove` | `Model.Dispose(...)` | `Model.Dispose() : void` | function |
| `Moss_ResourceSetCreate` | `Renderer.CreateResourceSet(...)` | `Renderer.CreateResourceSet(ResourceSetLayoutDesc desc) : ResourceSet` | function |
| `Moss_ResourceSetDestroy` | `ResourceSet.Dispose(...)` | `ResourceSet.Dispose() : void` | function |
| `Moss_FramebufferCreate` | `Renderer.CreateFramebuffer(...)` | `Renderer.CreateFramebuffer(FramebufferDesc desc) : Framebuffer` | function |
| `Moss_FramebufferDestroy` | `Framebuffer.Dispose(...)` | `Framebuffer.Dispose() : void` | function |
| `Moss_RendererSetUpscaler` | `Renderer.SetUpscaler(...)` | `Renderer.SetUpscaler(Upscaler upscaler) : void` | function |
| `Moss_RendererSetUpscalerDesc` | `Renderer.SetUpscalerDesc(...)` | `Renderer.SetUpscalerDesc(RendererUpscalerDesc desc) : void` | function |
| `Moss_GPUGetPhysicalDeviceName` | `Renderer.GetPhysicalDeviceName(...)` | `Renderer.GetPhysicalDeviceName() : string?` | function |
| `Moss_GPUAPIGetName` | `Renderer.GpuapiGetName(...)` | `Renderer.GpuapiGetName() : string?` | function |
| `Moss_GPUAPIGetVersion` | `Renderer.GpuapiGetVersion(...)` | `Renderer.GpuapiGetVersion() : string?` | function |
| `Moss_RendererDrawLine2D` | `Renderer.DrawLineDimension2D(...)` | `Renderer.DrawLineDimension2D(Vec2 from, Vec2 to, Color color, float thickness) : void` | function |
| `Moss_RendererDrawLines2D` | `Renderer.DrawLinesDimension2D(...)` | `Renderer.DrawLinesDimension2D(TArray<Vec2> positions, Color color, float thickness) : void` | function |
| `Moss_RendererDrawBox2D` | `Renderer.DrawBoxDimension2D(...)` | `Renderer.DrawBoxDimension2D(ReadOnlySpan<Vec2> center, float rotation, Color color) : void` | function |
| `Moss_RendererDrawCircle2D` | `Renderer.DrawCircleDimension2D(...)` | `Renderer.DrawCircleDimension2D(Vec2 center, float radius, Color color) : void` | function |
| `Moss_RendererDrawCylinder2D` | `Renderer.DrawCylinderDimension2D(...)` | `Renderer.DrawCylinderDimension2D(RMat44 matrix, float halfHeight, float radius, Color color) : void` | function |
| `Moss_RendererDrawTriangle2D` | `Renderer.DrawTriangleDimension2D(...)` | `Renderer.DrawTriangleDimension2D(Vec2 p0, Vec2 p1, Vec2 p2, Color color) : void` | function |
| `Moss_RendererDrawArrow2D` | `Renderer.DrawArrowDimension2D(...)` | `Renderer.DrawArrowDimension2D(Vec2 from, Vec2 to, Color color, float headSize) : void` | function |
| `Moss_RendererDrawMarker2D` | `Renderer.DrawMarkerDimension2D(...)` | `Renderer.DrawMarkerDimension2D(Vec2 position, Color color, float size) : void` | function |
| `Moss_RendererDrawBone2D` | `Renderer.DrawBoneDimension2D(...)` | `Renderer.DrawBoneDimension2D(RMat44 transform, float length, Color color) : void` | function |
| `Moss_RendererDrawCoordinateSystem2D` | `Renderer.DrawCoordinateSystemDimension2D(...)` | `Renderer.DrawCoordinateSystemDimension2D(RMat44 matrix, float size) : void` | function |
| `Moss_RendererDrawCapsule2D` | `Renderer.DrawCapsuleDimension2D(...)` | `Renderer.DrawCapsuleDimension2D(RMat44 matrix, float halfHeightOfCylinder, float radius, Color color) : void` | function |
| `Moss_RendererDrawConvex2D` | `Renderer.DrawConvexDimension2D(...)` | `Renderer.DrawConvexDimension2D(ReadOnlySpan<Vec2> points, Color color) : void` | function |
| `Moss_RendererDrawConvexHull2D` | `Renderer.DrawConvexHullDimension2D(...)` | `Renderer.DrawConvexHullDimension2D(ReadOnlySpan<Vec2> points, Color color) : void` | function |
| `Moss_RendererDrawPolygonShape2D` | `Renderer.DrawPolygonShapeDimension2D(...)` | `Renderer.DrawPolygonShapeDimension2D(ReadOnlySpan<Vec2> points, Color color) : void` | function |
| `Moss_RendererDrawGizmo2D` | `Renderer.DrawGizmoDimension2D(...)` | `Renderer.DrawGizmoDimension2D(RMat44 transform, float size) : void` | function |
| `Moss_RendererDrawLineBatch2D` | `Renderer.DrawLineBatchDimension2D(...)` | `Renderer.DrawLineBatchDimension2D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawBoxBatch2D` | `Renderer.DrawBoxBatchDimension2D(...)` | `Renderer.DrawBoxBatchDimension2D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawCircleBatch2D` | `Renderer.DrawCircleBatchDimension2D(...)` | `Renderer.DrawCircleBatchDimension2D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawArrowBatch2D` | `Renderer.DrawArrowBatchDimension2D(...)` | `Renderer.DrawArrowBatchDimension2D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawMarkerBatch2D` | `Renderer.DrawMarkerBatchDimension2D(...)` | `Renderer.DrawMarkerBatchDimension2D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawBoxInstanced2D` | `Renderer.DrawBoxInstancedDimension2D(...)` | `Renderer.DrawBoxInstancedDimension2D(Vec2 size, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawCircleInstanced2D` | `Renderer.DrawCircleInstancedDimension2D(...)` | `Renderer.DrawCircleInstancedDimension2D(float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawCapsuleInstanced2D` | `Renderer.DrawCapsuleInstancedDimension2D(...)` | `Renderer.DrawCapsuleInstancedDimension2D(float halfHeightOfCylinder, float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawMarkerInstanced2D` | `Renderer.DrawMarkerInstancedDimension2D(...)` | `Renderer.DrawMarkerInstancedDimension2D(float size, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawGizmoInstanced2D` | `Renderer.DrawGizmoInstancedDimension2D(...)` | `Renderer.DrawGizmoInstancedDimension2D(float size, RMat44 transform, uint count) : void` | function |
| `Moss_RendererDrawLine3D` | `Renderer.DrawLineDimension3D(...)` | `Renderer.DrawLineDimension3D(Vec3 from, Vec3 to, Color color, float thickness) : void` | function |
| `Moss_RendererDrawLines3D` | `Renderer.DrawLinesDimension3D(...)` | `Renderer.DrawLinesDimension3D(TArray<Vec3> positions, Color color, float thickness) : void` | function |
| `Moss_RendererDrawBox3D` | `Renderer.DrawBoxDimension3D(...)` | `Renderer.DrawBoxDimension3D(ReadOnlySpan<Vec3> center, RMat44 rotation, Color color) : void` | function |
| `Moss_RendererDrawSphere3D` | `Renderer.DrawSphereDimension3D(...)` | `Renderer.DrawSphereDimension3D(Vec3 center, float radius, Color color) : void` | function |
| `Moss_RendererDrawCylinder3D` | `Renderer.DrawCylinderDimension3D(...)` | `Renderer.DrawCylinderDimension3D(Vec3 base, Vec3 top, float radius, Color color) : void` | function |
| `Moss_RendererDrawCone3D` | `Renderer.DrawConeDimension3D(...)` | `Renderer.DrawConeDimension3D(Vec3 base, Vec3 tip, float radius, Color color) : void` | function |
| `Moss_RendererDrawTriangle3D` | `Renderer.DrawTriangleDimension3D(...)` | `Renderer.DrawTriangleDimension3D(Vec3 p0, Vec3 p1, Vec3 p2, Color color) : void` | function |
| `Moss_RendererDrawArrow3D` | `Renderer.DrawArrowDimension3D(...)` | `Renderer.DrawArrowDimension3D(Vec3 from, Vec3 to, Color color, float headSize) : void` | function |
| `Moss_RendererDrawMarker3D` | `Renderer.DrawMarkerDimension3D(...)` | `Renderer.DrawMarkerDimension3D(Vec3 position, Color color, float size) : void` | function |
| `Moss_RendererDrawPlane3D` | `Renderer.DrawPlaneDimension3D(...)` | `Renderer.DrawPlaneDimension3D(Vec3 center, ReadOnlySpan<Vec3> normal, Color color) : void` | function |
| `Moss_RendererDrawBone3D` | `Renderer.DrawBoneDimension3D(...)` | `Renderer.DrawBoneDimension3D(RMat44 transform, float length, Color color) : void` | function |
| `Moss_RendererDrawCoordinateSystem3D` | `Renderer.DrawCoordinateSystemDimension3D(...)` | `Renderer.DrawCoordinateSystemDimension3D(RMat44 matrix, float size) : void` | function |
| `Moss_RendererDrawCapsule3D` | `Renderer.DrawCapsuleDimension3D(...)` | `Renderer.DrawCapsuleDimension3D(RMat44 matrix, float halfHeightOfCylinder, float radius, Color color) : void` | function |
| `Moss_RendererDrawConvex3D` | `Renderer.DrawConvexDimension3D(...)` | `Renderer.DrawConvexDimension3D(ReadOnlySpan<Vec3> points, Color color) : void` | function |
| `Moss_RendererDrawConvexHull3D` | `Renderer.DrawConvexHullDimension3D(...)` | `Renderer.DrawConvexHullDimension3D(ReadOnlySpan<Vec3> points, Color color) : void` | function |
| `Moss_RendererDrawPolygonShape3D` | `Renderer.DrawPolygonShapeDimension3D(...)` | `Renderer.DrawPolygonShapeDimension3D(ReadOnlySpan<Vec3> points, Color color) : void` | function |
| `Moss_RendererDrawGizmo3D` | `Renderer.DrawGizmoDimension3D(...)` | `Renderer.DrawGizmoDimension3D(RMat44 transform, float size) : void` | function |
| `Moss_RendererDrawLineBatch3D` | `Renderer.DrawLineBatchDimension3D(...)` | `Renderer.DrawLineBatchDimension3D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawBoxBatch3D` | `Renderer.DrawBoxBatchDimension3D(...)` | `Renderer.DrawBoxBatchDimension3D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawSphereBatch3D` | `Renderer.DrawSphereBatchDimension3D(...)` | `Renderer.DrawSphereBatchDimension3D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawArrowBatch3D` | `Renderer.DrawArrowBatchDimension3D(...)` | `Renderer.DrawArrowBatchDimension3D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawMarkerBatch3D` | `Renderer.DrawMarkerBatchDimension3D(...)` | `Renderer.DrawMarkerBatchDimension3D(RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawBoxInstanced3D` | `Renderer.DrawBoxInstancedDimension3D(...)` | `Renderer.DrawBoxInstancedDimension3D(Vec3 size, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawSphereInstanced3D` | `Renderer.DrawSphereInstancedDimension3D(...)` | `Renderer.DrawSphereInstancedDimension3D(float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawCapsuleInstanced3D` | `Renderer.DrawCapsuleInstancedDimension3D(...)` | `Renderer.DrawCapsuleInstancedDimension3D(float halfHeightOfCylinder, float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawCylinderInstanced3D` | `Renderer.DrawCylinderInstancedDimension3D(...)` | `Renderer.DrawCylinderInstancedDimension3D(float halfHeight, float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawConeInstanced3D` | `Renderer.DrawConeInstancedDimension3D(...)` | `Renderer.DrawConeInstancedDimension3D(float height, float radius, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawMarkerInstanced3D` | `Renderer.DrawMarkerInstancedDimension3D(...)` | `Renderer.DrawMarkerInstancedDimension3D(float size, RMat44 transform, Color colour, uint count) : void` | function |
| `Moss_RendererDrawGizmoInstanced3D` | `Renderer.DrawGizmoInstancedDimension3D(...)` | `Renderer.DrawGizmoInstancedDimension3D(float size, RMat44 transform, uint count) : void` | function |
| `Moss_VulkanGetDevice` | `Renderer.GetDevice(...)` | `Renderer.GetDevice() : VkDevice` | function |
| `Moss_VulkanGetDescriptorPool` | `Renderer.GetDescriptorPool(...)` | `Renderer.GetDescriptorPool() : VkDescriptorPool` | function |
| `Moss_VulkanGetDescriptorSetLayoutTexture` | `Renderer.GetDescriptorSetLayoutTexture(...)` | `Renderer.GetDescriptorSetLayoutTexture() : VkDescriptorSetLayout` | function |
| `Moss_VulkanGetTextureSamplerRepeat` | `Renderer.GetTextureSamplerRepeat(...)` | `Renderer.GetTextureSamplerRepeat() : VkSampler` | function |
| `Moss_VulkanGetTextureSamplerShadow` | `Renderer.GetTextureSamplerShadow(...)` | `Renderer.GetTextureSamplerShadow() : VkSampler` | function |
| `Moss_VulkanGetRenderPassShadow` | `Renderer.GetRenderPassShadow(...)` | `Renderer.GetRenderPassShadow() : VkRenderPass` | function |
| `Moss_VulkanGetRenderPass` | `Renderer.GetRenderPass(...)` | `Renderer.GetRenderPass() : VkRenderPass` | function |
| `Moss_VulkanGetPipelineLayout` | `Renderer.GetPipelineLayout(...)` | `Renderer.GetPipelineLayout() : VkPipelineLayout` | function |
| `Moss_VulkanGetCommandBuffer` | `Renderer.GetCommandBuffer(...)` | `Renderer.GetCommandBuffer() : VkCommandBuffer` | function |
| `Moss_VulkanStartTempCommandBuffer` | `Renderer.StartTempCommandBuffer(...)` | `Renderer.StartTempCommandBuffer() : VkCommandBuffer` | function |
| `Moss_VulkanEndTempCommandBuffer` | `Renderer.EndTempCommandBuffer(...)` | `Renderer.EndTempCommandBuffer(VkCommandBuffer commandBuffer) : void` | function |
| `Moss_VulkanAllocateMemory` | `Renderer.AllocateMemory(...)` | `Renderer.AllocateMemory(VkDeviceSize size, uint memoryTypeBits, VkMemoryPropertyFlags properties, VkDeviceMemory outMemory) : void` | function |
| `Moss_VulkanFreeMemory` | `Renderer.FreeMemory(...)` | `Renderer.FreeMemory(VkDeviceMemory memory, VkDeviceSize size) : void` | function |
| `Moss_VulkanCreateBuffer` | `Renderer.CreateBuffer(...)` | `Renderer.CreateBuffer(VkDeviceSize size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, BufferVK outBuffer) : void` | function |
| `Moss_VulkanCopyBuffer` | `Renderer.CopyBuffer(...)` | `Renderer.CopyBuffer(VkBuffer src, VkBuffer dst, VkDeviceSize size) : void` | function |
| `Moss_VulkanCreateDeviceLocalBuffer` | `Renderer.CreateDeviceLocalBuffer(...)` | `Renderer.CreateDeviceLocalBuffer(ReadOnlySpan<byte> data, VkDeviceSize size, VkBufferUsageFlags usage, BufferVK outBuffer) : void` | function |
| `Moss_VulkanFreeBuffer` | `Renderer.FreeBuffer(...)` | `Renderer.FreeBuffer(BufferVK buffer) : void` | function |
| `Moss_VulkanCreateConstantBuffer` | `Renderer.CreateConstantBuffer(...)` | `Renderer.CreateConstantBuffer(VkDeviceSize bufferSize) : ConstantBufferVK` | function |
| `Moss_VulkanCreateImage` | `Renderer.CreateImage(...)` | `Renderer.CreateImage(uint width, uint height, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, VkImage outImage, VkDeviceMemory outMemory) : void` | function |
| `Moss_VulkanDestroyImage` | `Renderer.DestroyImage(...)` | `Renderer.DestroyImage(VkImage image, VkDeviceMemory memory) : void` | function |
| `Moss_VulkanCreateImageView` | `Renderer.CreateImageView(...)` | `Renderer.CreateImageView(VkImage image, VkFormat format, VkImageAspectFlags aspectFlags) : VkImageView` | function |
| `Moss_VulkanFindDepthFormat` | `Renderer.FindDepthFormat(...)` | `Renderer.FindDepthFormat() : VkFormat` | function |
| `Moss_Vulkan_EncodeSPIRV` | `Renderer.EncodeSpirv(...)` | `Renderer.EncodeSpirv(string path) : void` | function |
| `Moss_Vulkan_DecodeSPIRV` | `Renderer.DecodeSpirv(...)` | `Renderer.DecodeSpirv(string path) : void` | function |
| `Moss_DX12GetDevice` | `Renderer.Dx12GetDevice(...)` | `Renderer.Dx12GetDevice() : IdDIMENSION3D1DIMENSION2Device` | function |
| `Moss_DX12GetRootSignature` | `Renderer.Dx12GetRootSignature(...)` | `Renderer.Dx12GetRootSignature() : IdDIMENSION3D12RootSignature` | function |
| `Moss_DX12GetCommandList` | `Renderer.Dx12GetCommandList(...)` | `Renderer.Dx12GetCommandList() : IdDIMENSION3D12GraphicsCommandList` | function |
| `Moss_DX12GetUploadQueue` | `Renderer.Dx12GetUploadQueue(...)` | `Renderer.Dx12GetUploadQueue() : CommandQueueDX12` | function |
| `Moss_DX12GetDSVHeap` | `Renderer.Dx12GetDsvHeap(...)` | `Renderer.Dx12GetDsvHeap() : DescriptorHeapDX12` | function |
| `Moss_DX12GetSRVHeap` | `Renderer.Dx12GetSrvHeap(...)` | `Renderer.Dx12GetSrvHeap() : DescriptorHeapDX12` | function |
| `Moss_DX_CompileHLSL` | `Renderer.CompileHlsl(...)` | `Renderer.CompileHlsl(string inputPath, string outputPath, string entryPoint, string target) : bool` | function |
| `Moss_DX_EnableDebugLayer` | `Renderer.EnableDebugLayer(...)` | `Renderer.EnableDebugLayer(bool enable) : void` | function |
| `Moss_DX_GetShaderModel` | `Renderer.GetShaderModel(...)` | `Renderer.GetShaderModel() : uint` | function |
| `Moss_MetalGetView` | `Renderer.GetView(...)` | `Renderer.GetView() : MTKView` | function |
| `Moss_MetalGetDevice` | `Renderer.GetDevice(...)` | `Renderer.GetDevice() : Span<byte>` | function |
| `Moss_MetalGetRenderEncoder` | `Renderer.GetRenderEncoder(...)` | `Renderer.GetRenderEncoder() : Span<byte>` | function |
| `Moss_Metal_CompileMSL` | `Renderer.CompileMsl(...)` | `Renderer.CompileMsl(string inputPath, string outputPath) : bool` | function |
| `Moss_Metal_SupportsFamily` | `Renderer.SupportsFamily(...)` | `Renderer.SupportsFamily(uint family) : bool` | function |

### `Moss_stdinc.h` → `Moss.Core`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Allocate` | `Core.Allocator.Allocate` | `public delegate* Allocate` | callback |
| `Reallocate` | `Core.Allocator.Reallocate` | `public delegate* Reallocate` | callback |
| `Free` | `Core.Allocator.Free` | `public delegate* Free` | callback |
| `AlignedAllocate` | `Core.Allocator.AlignedAllocate` | `public delegate* AlignedAllocate` | callback |
| `AlignedFree` | `Core.Allocator.AlignedFree` | `public delegate* AlignedFree` | callback |
| `RegisterDefaultAllocator` | `Core.RegisterDefaultAllocator(...)` | `Core.RegisterDefaultAllocator() : void` | function |
| `Allocate` | `Core.Allocate(...)` | `Core.Allocate(nuint inSize) : Span<byte>` | function |
| `Reallocate` | `Core.Reallocate(...)` | `Core.Reallocate(Span<byte> inBlock, nuint inOldSize, nuint inNewSize) : Span<byte>` | function |
| `Free` | `Core.Free(...)` | `Core.Free(Span<byte> inBlock) : void` | function |
| `AlignedAllocate` | `Core.AlignedAllocate(...)` | `Core.AlignedAllocate(nuint inSize, nuint inAlignment) : Span<byte>` | function |
| `AlignedFree` | `Core.AlignedFree(...)` | `Core.AlignedFree(Span<byte> inBlock) : void` | function |
| `Moss_rand` | `Core.Rand(...)` | `Core.Rand(Int32 n) : Int32` | function |
| `Moss_randf` | `Core.Randf(...)` | `Core.Randf() : float` | function |
| `Moss_randi` | `Core.Randi(...)` | `Core.Randi() : int` | function |
| `Moss_srand` | `Core.Srand(...)` | `Core.Srand(Int64 seed) : void` | function |
| `Moss_randi_range` | `Core.RandiRange(...)` | `Core.RandiRange(Int32 from, Int32 to) : Int32` | function |
| `Moss_randf_range` | `Core.RandfRange(...)` | `Core.RandfRange(float from, float to) : float` | function |
| `Moss_rand_gaussian` | `Core.RandGaussian(...)` | `Core.RandGaussian(float mean, float stddev) : float` | function |
| `Moss_rand_unit` | `Core.RandUnit(...)` | `Core.RandUnit() : float` | function |
| `Moss_GetTicks` | `Core.GetTicks(...)` | `Core.GetTicks() : Time` | function |
| `Moss_GetSeconds` | `Core.GetSeconds(...)` | `Core.GetSeconds(Time time) : double` | function |
| `Moss_GetMilliseconds` | `Core.GetMilliseconds(...)` | `Core.GetMilliseconds(Time ticks) : float` | function |
| `Moss_GetMillisecondsAndReset` | `Core.GetMillisecondsAndReset(...)` | `Core.GetMillisecondsAndReset(Time ticks) : float` | function |
| `Moss_Yield` | `Core.Yield(...)` | `Core.Yield() : void` | function |
| `Moss_Delay` | `Core.Delay(...)` | `Core.Delay(double delay) : double` | function |
| `Moss_LocalTime` | `Core.LocalTime(...)` | `Core.LocalTime() : string?` | function |
| `Moss_TimeStamp` | `Core.TimeStamp(...)` | `Core.TimeStamp() : string?` | function |
| `Moss_TimeNow` | `Core.TimeNow(...)` | `Core.TimeNow() : string?` | function |
| `Moss_CTimeNow` | `Core.CTimeNow(...)` | `Core.CTimeNow() : string?` | function |
| `Moss_TimeStampET` | `Core.TimeStampEt(...)` | `Core.TimeStampEt() : string?` | function |
| `Moss_TimeStampCT` | `Core.TimeStampCt(...)` | `Core.TimeStampCt() : string?` | function |
| `Moss_TimeStampMT` | `Core.TimeStampMt(...)` | `Core.TimeStampMt() : string?` | function |
| `Moss_TimeStampPT` | `Core.TimeStampPt(...)` | `Core.TimeStampPt() : string?` | function |
| `Moss_FormatTime` | `Core.FormatTime(...)` | `Core.FormatTime(string fmt) : string?` | function |

### `Moss_XR.h` → `Moss.XR`

| Native symbol (traceability) | Public API projection | Target-language signature | Kind |
|---|---|---|---|
| `Moss_XR_Initialize` | `XR.Initialize(...)` | `XR.Initialize() : bool` | function |
| `Moss_XR_Shutdown` | `XR.Shutdown(...)` | `XR.Shutdown() : void` | function |
| `Moss_XR_GetCapabilities` | `XR.GetCapabilities(...)` | `XR.GetCapabilities() : XrCapabilities` | function |
| `Moss_XR_EnableValidationLayers` | `XR.EnableValidationLayers(...)` | `XR.EnableValidationLayers() : bool` | function |
| `Moss_XR_CreateSession` | `XR.CreateSession(...)` | `XR.CreateSession() : XrSession` | function |
| `Moss_XR_DestroySession` | `XrSession.Dispose(...)` | `XrSession.Dispose() : void` | function |
| `Moss_XR_GetSessionState` | `XrSession.GetState(...)` | `XrSession.GetState() : XrSessionState` | function |
| `Moss_XR_BeginFrame` | `XrSession.BeginFrame(...)` | `XrSession.BeginFrame() : bool` | function |
| `Moss_XR_EndFrame` | `XrSession.EndFrame(...)` | `XrSession.EndFrame() : void` | function |
| `Moss_XR_GetPredictedDisplayTime` | `XrSession.GetPredictedDisplayTime(...)` | `XrSession.GetPredictedDisplayTime() : XrTime` | function |
| `Moss_XR_GetDeltaSeconds` | `XR.GetDeltaSeconds(...)` | `XR.GetDeltaSeconds() : float` | function |
| `Moss_XR_HandleEvents` | `XrSession.HandleEvents(...)` | `XrSession.HandleEvents() : void` | function |
| `Moss_XR_GetViewCount` | `XrSession.GetViewCount(...)` | `XrSession.GetViewCount() : uint` | function |
| `Moss_XR_GetView` | `XrSession.GetView(...)` | `XrSession.GetView(uint index, XrView outView) : bool` | function |
| `Moss_XR_CreateActionSet` | `XR.CreateActionSet(...)` | `XR.CreateActionSet(string name) : XrActionSet` | function |
| `Moss_XR_DestroyActionSet` | `XrActionSet.Dispose(...)` | `XrActionSet.Dispose() : void` | function |
| `Moss_XR_CreateAction` | `XrActionSet.CreateAction(...)` | `XrActionSet.CreateAction(string name, XrActionType type) : XrAction` | function |
| `Moss_XR_DestroyAction` | `XrAction.Dispose(...)` | `XrAction.Dispose() : void` | function |
| `Moss_XR_AttachActionSet` | `XrActionSet.Attach(...)` | `XrActionSet.Attach(XrSession session) : void` | function |
| `Moss_XR_SyncActions` | `XrSession.SyncActions(...)` | `XrSession.SyncActions() : void` | function |
| `Moss_XR_GetActionBoolean` | `XrAction.GetBoolean(...)` | `XrAction.GetBoolean() : bool` | function |
| `Moss_XR_GetActionFloat` | `XrAction.GetFloat(...)` | `XrAction.GetFloat() : float` | function |
| `Moss_XR_GetActionPose` | `XrAction.GetPose(...)` | `XrAction.GetPose(XrPose outPose) : bool` | function |
| `Moss_XR_StopHaptic` | `XrAction.StopHaptic(...)` | `XrAction.StopHaptic() : void` | function |
| `Moss_XR_CreateHand` | `XR.CreateHand(...)` | `XR.CreateHand() : XrHandModifier` | function |
| `Moss_XR_GetHandJointPose` | `XrHandModifier.GetHandJointPose(...)` | `XrHandModifier.GetHandJointPose(XrHandJoint joint, XrPose outPose) : bool` | function |
| `Moss_XR_DestroyHand` | `XrHandModifier.DestroyHand(...)` | `XrHandModifier.DestroyHand() : void` | function |
| `Moss_XR_GetHandPose` | `XrHandModifier.GetHandPose(...)` | `XrHandModifier.GetHandPose(XrPose outPose) : bool` | function |
| `Moss_XR_CreateBody` | `XR.CreateBody(...)` | `XR.CreateBody() : XrBodyModifier` | function |
| `Moss_XR_GetBodyJointPose` | `XrBodyModifier.GetBodyJointPose(...)` | `XrBodyModifier.GetBodyJointPose(XrBodyJoint joint, XrPose outPose) : bool` | function |
| `Moss_XR_DestroyBody` | `XrBodyModifier.DestroyBody(...)` | `XrBodyModifier.DestroyBody() : void` | function |
| `Moss_XR_CreateFace` | `XR.CreateFace(...)` | `XR.CreateFace() : XrFaceModifier` | function |
| `Moss_XR_DestroyFace` | `XrFaceModifier.DestroyFace(...)` | `XrFaceModifier.DestroyFace() : void` | function |
| `Moss_XR_UpdateBodyTracking` | `XrSession.UpdateBodyTracking(...)` | `XrSession.UpdateBodyTracking() : void` | function |
| `Moss_XR_UpdateHandTracking` | `XrSession.UpdateHandTracking(...)` | `XrSession.UpdateHandTracking() : void` | function |
| `Moss_XR_EnablePassthrough` | `XR.EnablePassthrough(...)` | `XR.EnablePassthrough(bool enable) : bool` | function |
| `Moss_XR_EnableFoveatedRendering` | `XR.EnableFoveatedRendering(...)` | `XR.EnableFoveatedRendering(bool enable) : bool` | function |
| `Moss_XR_GetBackendName` | `XR.GetBackendName(...)` | `XR.GetBackendName() : string?` | function |
| `Moss_XR_GetBackendVersion` | `XR.GetBackendVersion(...)` | `XR.GetBackendVersion() : uint` | function |
| `Moss_XR_BeginDebugLabel` | `XR.BeginDebugLabel(...)` | `XR.BeginDebugLabel(string label) : void` | function |
| `Moss_XR_EndDebugLabel` | `XR.EndDebugLabel(...)` | `XR.EndDebugLabel() : void` | function |


## 8. Native header and feature coverage

The public binding must cover the complete Moss header tree, not only the exported function index. Many Core, Geometry, Physics, Renderer, Resources, Skeleton, TriangleSplitter and Variants APIs are class members, templates, inline methods, or declarations not represented by the top-level free-function catalogue.

## Complete header inventory

Each link opens the corresponding source file in `include/Moss/`. The rightmost count shows declarations beginning with `MOSS_API` in that header; zero does not mean the header has no public API (it may expose class members, templates, inline methods, typedefs, or preprocessor declarations).

### Core (40 headers)

Foundational C++ utilities.

| Header | Export-marked declarations |
|---|---:|
| [`Core/BinaryHeap.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/BinaryHeap.h) | 0 |
| [`Core/ByteBuffer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/ByteBuffer.h) | 0 |
| [`Core/FPControlWord.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FPControlWord.h) | 0 |
| [`Core/FPException.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FPException.h) | 0 |
| [`Core/FPFlushDenormals.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FPFlushDenormals.h) | 0 |
| [`Core/Factory.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Factory.h) | 0 |
| [`Core/FixedSizeFreeList.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FixedSizeFreeList.h) | 0 |
| [`Core/FreeBlockAlloc.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FreeBlockAlloc.h) | 0 |
| [`Core/HashCombine.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/HashCombine.h) | 0 |
| [`Core/HashTable.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/HashTable.h) | 0 |
| [`Core/InsertionSort.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/InsertionSort.h) | 0 |
| [`Core/JobSystem.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/JobSystem.h) | 0 |
| [`Core/JobSystemSingleThreaded.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/JobSystemSingleThreaded.h) | 0 |
| [`Core/JobSystemThreadPool.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/JobSystemThreadPool.h) | 0 |
| [`Core/JobSystemWithBarrier.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/JobSystemWithBarrier.h) | 0 |
| [`Core/LinearCurve.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/LinearCurve.h) | 0 |
| [`Core/LockFreeHashMap.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/LockFreeHashMap.h) | 0 |
| [`Core/Memory.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Memory.h) | 11 |
| [`Core/Mutex.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Mutex.h) | 0 |
| [`Core/MutexArray.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/MutexArray.h) | 0 |
| [`Core/NonCopyable.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/NonCopyable.h) | 0 |
| [`Core/Profiler.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Profiler.h) | 4 |
| [`Core/QuickSort.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/QuickSort.h) | 0 |
| [`Core/RTTI.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/RTTI.h) | 0 |
| [`Core/Reference.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Reference.h) | 0 |
| [`Core/Result.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Result.h) | 0 |
| [`Core/STLAlignedAllocator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/STLAlignedAllocator.h) | 0 |
| [`Core/STLAllocator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/STLAllocator.h) | 0 |
| [`Core/STLLocalAllocator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/STLLocalAllocator.h) | 0 |
| [`Core/STLTempAllocator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/STLTempAllocator.h) | 0 |
| [`Core/ScopeExit.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/ScopeExit.h) | 0 |
| [`Core/Semaphore.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Semaphore.h) | 0 |
| [`Core/StreamIn.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StreamIn.h) | 0 |
| [`Core/StreamOut.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StreamOut.h) | 0 |
| [`Core/StreamUtils.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StreamUtils.h) | 0 |
| [`Core/StreamWrapper.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StreamWrapper.h) | 0 |
| [`Core/StridedPtr.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StridedPtr.h) | 0 |
| [`Core/StringTools.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/StringTools.h) | 6 |
| [`Core/TempAllocator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/TempAllocator.h) | 0 |
| [`Core/TickCounter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/TickCounter.h) | 0 |

### Geometry (9 headers)

Geometry algorithms and primitives.

| Header | Export-marked declarations |
|---|---:|
| [`Geometry/ClipPoly.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/ClipPoly.h) | 0 |
| [`Geometry/ClosestPoint.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/ClosestPoint.h) | 0 |
| [`Geometry/ConvexHullBuilder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/ConvexHullBuilder.h) | 0 |
| [`Geometry/ConvexSupport.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/ConvexSupport.h) | 0 |
| [`Geometry/EPAConvexHullBuilder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/EPAConvexHullBuilder.h) | 0 |
| [`Geometry/EPAPenetrationDepth.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/EPAPenetrationDepth.h) | 0 |
| [`Geometry/GJKClosestPoint.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/GJKClosestPoint.h) | 0 |
| [`Geometry/RayCylinder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/RayCylinder.h) | 0 |
| [`Geometry/Triangle.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Geometry/Triangle.h) | 0 |

### Physics (69 headers)

Simulation, body, shape, collision, constraint and query implementation headers.

| Header | Export-marked declarations |
|---|---:|
| [`Physics/AABBTree/AABBTreeBuilder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/AABBTreeBuilder.h) | 0 |
| [`Physics/AABBTree/AABBTreeToBuffer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/AABBTreeToBuffer.h) | 0 |
| [`Physics/AABBTree/NodeCodec/NodeCodecQuadTreeHalfFloat.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/NodeCodec/NodeCodecQuadTreeHalfFloat.h) | 0 |
| [`Physics/AABBTree/NodeCodec/NodeCodecQuadTreeHalfFloat2D.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/NodeCodec/NodeCodecQuadTreeHalfFloat2D.h) | 0 |
| [`Physics/AABBTree/TriangleCodec/TriangleCodecIndexed8BitPackSOA4Flags.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/TriangleCodec/TriangleCodecIndexed8BitPackSOA4Flags.h) | 0 |
| [`Physics/AABBTree/TriangleCodec/TriangleCodecIndexed8BitPackSOA4Flags2D.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/AABBTree/TriangleCodec/TriangleCodecIndexed8BitPackSOA4Flags2D.h) | 0 |
| [`Physics/Body/Body.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Body/Body.h) | 1 |
| [`Physics/Body/BodyID.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Body/BodyID.h) | 0 |
| [`Physics/Character/Character.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Character/Character.h) | 0 |
| [`Physics/Cloth/Cloth.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Cloth/Cloth.h) | 0 |
| [`Physics/Collision/ActiveEdges.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ActiveEdges.h) | 0 |
| [`Physics/Collision/BroadPhase/BroadPhase.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/BroadPhase/BroadPhase.h) | 0 |
| [`Physics/Collision/BroadPhase/BroadPhaseLayerInterfaceMask.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/BroadPhase/BroadPhaseLayerInterfaceMask.h) | 0 |
| [`Physics/Collision/BroadPhase/QuadTree.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/BroadPhase/QuadTree.h) | 0 |
| [`Physics/Collision/CastConvexVsTriangles.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CastConvexVsTriangles.h) | 0 |
| [`Physics/Collision/CastResult.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CastResult.h) | 0 |
| [`Physics/Collision/CastSphereVsTriangles.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CastSphereVsTriangles.h) | 0 |
| [`Physics/Collision/CollideConvexVsTriangles.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideConvexVsTriangles.h) | 0 |
| [`Physics/Collision/CollidePointResult.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollidePointResult.h) | 0 |
| [`Physics/Collision/CollideShape.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideShape.h) | 0 |
| [`Physics/Collision/CollideShapeVsShapePerLeaf.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideShapeVsShapePerLeaf.h) | 0 |
| [`Physics/Collision/CollideSoftBodyVertexIterator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideSoftBodyVertexIterator.h) | 0 |
| [`Physics/Collision/CollideSoftBodyVerticesVsTriangles.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideSoftBodyVerticesVsTriangles.h) | 0 |
| [`Physics/Collision/CollideSphereVsTriangles.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollideSphereVsTriangles.h) | 0 |
| [`Physics/Collision/CollisionCollector.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollisionCollector.h) | 0 |
| [`Physics/Collision/CollisionCollectorImpl.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollisionCollectorImpl.h) | 0 |
| [`Physics/Collision/CollisionDispatch.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollisionDispatch.h) | 0 |
| [`Physics/Collision/CollisionGroup.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/CollisionGroup.h) | 0 |
| [`Physics/Collision/ContactListener.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ContactListener.h) | 0 |
| [`Physics/Collision/EstimateCollisionResponse.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/EstimateCollisionResponse.h) | 1 |
| [`Physics/Collision/GroupFilter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/GroupFilter.h) | 0 |
| [`Physics/Collision/GroupFilterTable.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/GroupFilterTable.h) | 0 |
| [`Physics/Collision/InternalEdgeRemovingCollector.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/InternalEdgeRemovingCollector.h) | 0 |
| [`Physics/Collision/ManifoldBetweenTwoFaces.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ManifoldBetweenTwoFaces.h) | 2 |
| [`Physics/Collision/NarrowPhaseQuery.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/NarrowPhaseQuery.h) | 0 |
| [`Physics/Collision/NarrowPhaseStats.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/NarrowPhaseStats.h) | 0 |
| [`Physics/Collision/ObjectLayer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ObjectLayer.h) | 0 |
| [`Physics/Collision/ObjectLayerPairFilterMask.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ObjectLayerPairFilterMask.h) | 0 |
| [`Physics/Collision/ObjectLayerPairFilterTable.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ObjectLayerPairFilterTable.h) | 0 |
| [`Physics/Collision/PhysicsMaterial.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/PhysicsMaterial.h) | 0 |
| [`Physics/Collision/PhysicsMaterialSimple.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/PhysicsMaterialSimple.h) | 0 |
| [`Physics/Collision/RayCast.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/RayCast.h) | 0 |
| [`Physics/Collision/Shape/CompoundShapeVisitors.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/CompoundShapeVisitors.h) | 0 |
| [`Physics/Collision/Shape/ConvexHullShape.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/ConvexHullShape.h) | 0 |
| [`Physics/Collision/Shape/GetTrianglesContext.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/GetTrianglesContext.h) | 0 |
| [`Physics/Collision/Shape/PolyhedronSubmergedVolumeCalculator.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/PolyhedronSubmergedVolumeCalculator.h) | 0 |
| [`Physics/Collision/Shape/Shapes.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/Shapes.h) | 0 |
| [`Physics/Collision/Shape/SubShapeID.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/SubShapeID.h) | 0 |
| [`Physics/Collision/Shape/SubShapeIDPair.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/Shape/SubShapeIDPair.h) | 0 |
| [`Physics/Collision/ShapeCast.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ShapeCast.h) | 0 |
| [`Physics/Collision/ShapeFilter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/ShapeFilter.h) | 0 |
| [`Physics/Collision/SimShapeFilter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/SimShapeFilter.h) | 0 |
| [`Physics/Collision/SimShapeFilterWrapper.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/SimShapeFilterWrapper.h) | 0 |
| [`Physics/Collision/SortReverseAndStore.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/SortReverseAndStore.h) | 0 |
| [`Physics/Collision/TransformedShape.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Collision/TransformedShape.h) | 0 |
| [`Physics/Constraints/ConstraintManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Constraints/ConstraintManager.h) | 0 |
| [`Physics/Constraints/Constraints.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Constraints/Constraints.h) | 0 |
| [`Physics/Hair/Hair.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Hair/Hair.h) | 0 |
| [`Physics/IslandBuilder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/IslandBuilder.h) | 0 |
| [`Physics/LargeIslandSplitter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/LargeIslandSplitter.h) | 0 |
| [`Physics/PhysicsLock.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/PhysicsLock.h) | 0 |
| [`Physics/PhysicsScene.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/PhysicsScene.h) | 0 |
| [`Physics/PhysicsSystem.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/PhysicsSystem.h) | 0 |
| [`Physics/PhysicsUpdateContext.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/PhysicsUpdateContext.h) | 0 |
| [`Physics/Ragdoll/Ragdoll.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Ragdoll/Ragdoll.h) | 0 |
| [`Physics/SoftBody/Softbody.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/SoftBody/Softbody.h) | 0 |
| [`Physics/StateRecorder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/StateRecorder.h) | 0 |
| [`Physics/StateRecorderImpl.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/StateRecorderImpl.h) | 0 |
| [`Physics/Vehicle/Vehicle.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Vehicle/Vehicle.h) | 0 |

### Renderer (65 headers)

Renderer resources and graphics backend headers.

| Header | Export-marked declarations |
|---|---:|
| [`Renderer/DX12/CommandQueueDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/CommandQueueDX12.h) | 0 |
| [`Renderer/DX12/ConstantBufferDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/ConstantBufferDX12.h) | 0 |
| [`Renderer/DX12/DescriptorHeapDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/DescriptorHeapDX12.h) | 0 |
| [`Renderer/DX12/LightingDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/LightingDX12.h) | 0 |
| [`Renderer/DX12/PipelineStateDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/PipelineStateDX12.h) | 0 |
| [`Renderer/DX12/RenderInstancesDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/RenderInstancesDX12.h) | 0 |
| [`Renderer/DX12/RenderPrimitiveDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/RenderPrimitiveDX12.h) | 0 |
| [`Renderer/DX12/RendererDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/RendererDX12.h) | 0 |
| [`Renderer/DX12/ShaderDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/ShaderDX12.h) | 0 |
| [`Renderer/DX12/TextureDX12.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DX12/TextureDX12.h) | 0 |
| [`Renderer/DebugRenderer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DebugRenderer.h) | 0 |
| [`Renderer/DebugRendererPlayback.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DebugRendererPlayback.h) | 0 |
| [`Renderer/DebugRendererRecorder.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DebugRendererRecorder.h) | 0 |
| [`Renderer/DebugRendererSimple.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/DebugRendererSimple.h) | 0 |
| [`Renderer/FogVolume.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/FogVolume.h) | 0 |
| [`Renderer/Font.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Font.h) | 0 |
| [`Renderer/GL/FontGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/FontGL.h) | 0 |
| [`Renderer/GL/LightingGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/LightingGL.h) | 0 |
| [`Renderer/GL/MeshGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/MeshGL.h) | 0 |
| [`Renderer/GL/PipelineStateGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/PipelineStateGL.h) | 0 |
| [`Renderer/GL/PostProcess.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/PostProcess.h) | 0 |
| [`Renderer/GL/ReflectionProbe.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/ReflectionProbe.h) | 0 |
| [`Renderer/GL/Renderer_GL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/Renderer_GL.h) | 0 |
| [`Renderer/GL/ShaderGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/ShaderGL.h) | 0 |
| [`Renderer/GL/StorageBufferGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/StorageBufferGL.h) | 0 |
| [`Renderer/GL/SubViewportGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/SubViewportGL.h) | 0 |
| [`Renderer/GL/SurfaceGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/SurfaceGL.h) | 0 |
| [`Renderer/GL/TextureGL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/TextureGL.h) | 0 |
| [`Renderer/GL/glad.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/GL/glad.h) | 0 |
| [`Renderer/MTL/PipelineStateMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/PipelineStateMTL.h) | 0 |
| [`Renderer/MTL/RenderInstancesMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/RenderInstancesMTL.h) | 0 |
| [`Renderer/MTL/RenderPrimitiveMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/RenderPrimitiveMTL.h) | 0 |
| [`Renderer/MTL/RendererMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/RendererMTL.h) | 0 |
| [`Renderer/MTL/ShaderMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/ShaderMTL.h) | 0 |
| [`Renderer/MTL/TextureMTL.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/MTL/TextureMTL.h) | 0 |
| [`Renderer/Managers/DecalManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/DecalManager.h) | 0 |
| [`Renderer/Managers/FontManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/FontManager.h) | 0 |
| [`Renderer/Managers/LightingManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/LightingManager.h) | 0 |
| [`Renderer/Managers/MeshManager3D.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/MeshManager3D.h) | 0 |
| [`Renderer/Managers/ParticleEffectManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/ParticleEffectManager.h) | 0 |
| [`Renderer/Managers/SurfaceManager.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Managers/SurfaceManager.h) | 0 |
| [`Renderer/Material2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Material2.h) | 0 |
| [`Renderer/Material3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Material3.h) | 0 |
| [`Renderer/Mesh.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Mesh.h) | 0 |
| [`Renderer/Model.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Model.h) | 0 |
| [`Renderer/PipelineState.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/PipelineState.h) | 0 |
| [`Renderer/Renderer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Renderer.h) | 0 |
| [`Renderer/Shader.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Shader.h) | 0 |
| [`Renderer/SkyBox.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/SkyBox.h) | 0 |
| [`Renderer/StorageBuffer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/StorageBuffer.h) | 0 |
| [`Renderer/SubViewport.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/SubViewport.h) | 0 |
| [`Renderer/Surface.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Surface.h) | 0 |
| [`Renderer/Texture.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/Texture.h) | 0 |
| [`Renderer/VK/ConstantBufferVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/ConstantBufferVK.h) | 0 |
| [`Renderer/VK/LightingManagerVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/LightingManagerVK.h) | 0 |
| [`Renderer/VK/MeshVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/MeshVK.h) | 0 |
| [`Renderer/VK/PipelineStateVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/PipelineStateVK.h) | 0 |
| [`Renderer/VK/RenderInstancesVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/RenderInstancesVK.h) | 0 |
| [`Renderer/VK/RenderPrimitiveVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/RenderPrimitiveVK.h) | 0 |
| [`Renderer/VK/RendererVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/RendererVK.h) | 0 |
| [`Renderer/VK/Renderer_VK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/Renderer_VK.h) | 4 |
| [`Renderer/VK/ShaderVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/ShaderVK.h) | 0 |
| [`Renderer/VK/StorageBufferVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/StorageBufferVK.h) | 0 |
| [`Renderer/VK/SubViewportVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/SubViewportVK.h) | 0 |
| [`Renderer/VK/TextureVK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Renderer/VK/TextureVK.h) | 0 |

### Resources (21 headers)

File/resource and serialization headers.

| Header | Export-marked declarations |
|---|---:|
| [`Resources/BezierCurve.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/BezierCurve.h) | 0 |
| [`Resources/Config.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/Config.h) | 0 |
| [`Resources/Gradient.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/Gradient.h) | 0 |
| [`Resources/Json.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/Json.h) | 0 |
| [`Resources/ObjectStream/GetPrimitiveTypeOfType.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/GetPrimitiveTypeOfType.h) | 0 |
| [`Resources/ObjectStream/ObjectStream.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStream.h) | 4 |
| [`Resources/ObjectStream/ObjectStreamBinaryIn.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamBinaryIn.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamBinaryOut.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamBinaryOut.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamIn.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamIn.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamOut.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamOut.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamTextIn.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamTextIn.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamTextOut.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamTextOut.h) | 0 |
| [`Resources/ObjectStream/ObjectStreamTypes.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/ObjectStreamTypes.h) | 0 |
| [`Resources/ObjectStream/SerializableAttribute.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/SerializableAttribute.h) | 0 |
| [`Resources/ObjectStream/SerializableAttributeEnum.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/SerializableAttributeEnum.h) | 0 |
| [`Resources/ObjectStream/SerializableAttributeTyped.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/SerializableAttributeTyped.h) | 0 |
| [`Resources/ObjectStream/SerializableObject.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/SerializableObject.h) | 0 |
| [`Resources/ObjectStream/TypeDeclarations.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/ObjectStream/TypeDeclarations.h) | 0 |
| [`Resources/PCK.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/PCK.h) | 0 |
| [`Resources/Timer.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/Timer.h) | 0 |
| [`Resources/Tween.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Resources/Tween.h) | 0 |

### Skeleton (1 headers)

Skeleton and animation declarations.

| Header | Export-marked declarations |
|---|---:|
| [`Skeleton/Skeleton.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Skeleton/Skeleton.h) | 0 |

### TriangleSplitter (1 headers)

Triangle splitting utilities.

| Header | Export-marked declarations |
|---|---:|
| [`TriangleSplitter/TriangleSplitter.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/TriangleSplitter/TriangleSplitter.h) | 0 |

### Variants (53 headers)

Math, geometry and container value types.

| Header | Export-marked declarations |
|---|---:|
| [`Variants/AABB2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/AABB2.h) | 0 |
| [`Variants/AABB3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/AABB3.h) | 0 |
| [`Variants/Basis.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Basis.h) | 0 |
| [`Variants/Color.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Color.h) | 0 |
| [`Variants/Math/EigenValueSymmetric.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/EigenValueSymmetric.h) | 0 |
| [`Variants/Math/GaussianElimination.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/GaussianElimination.h) | 0 |
| [`Variants/Math/HalfFloat.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/HalfFloat.h) | 0 |
| [`Variants/Math/MathTypes.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/MathTypes.h) | 0 |
| [`Variants/Math/Real.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/Real.h) | 0 |
| [`Variants/Math/Trigonometry.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Math/Trigonometry.h) | 0 |
| [`Variants/Matrix/DMat44.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/DMat44.h) | 0 |
| [`Variants/Matrix/DMat4x3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/DMat4x3.h) | 0 |
| [`Variants/Matrix/DynMatrix.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/DynMatrix.h) | 0 |
| [`Variants/Matrix/Mat22.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat22.h) | 0 |
| [`Variants/Matrix/Mat2x3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat2x3.h) | 0 |
| [`Variants/Matrix/Mat2x4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat2x4.h) | 0 |
| [`Variants/Matrix/Mat33.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat33.h) | 0 |
| [`Variants/Matrix/Mat3x2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat3x2.h) | 0 |
| [`Variants/Matrix/Mat3x4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat3x4.h) | 0 |
| [`Variants/Matrix/Mat44.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat44.h) | 0 |
| [`Variants/Matrix/Mat4x2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat4x2.h) | 0 |
| [`Variants/Matrix/Mat4x3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat4x3.h) | 0 |
| [`Variants/Matrix/TMatrix.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/TMatrix.h) | 0 |
| [`Variants/OBB2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/OBB2.h) | 0 |
| [`Variants/OBB3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/OBB3.h) | 0 |
| [`Variants/Plane.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Plane.h) | 0 |
| [`Variants/Quat.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Quat.h) | 0 |
| [`Variants/Rect.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Rect.h) | 0 |
| [`Variants/TArray.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/TArray.h) | 0 |
| [`Variants/TMap.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/TMap.h) | 0 |
| [`Variants/TSet.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/TSet.h) | 0 |
| [`Variants/TStaticArray.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/TStaticArray.h) | 0 |
| [`Variants/Vector/BVec16.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/BVec16.h) | 0 |
| [`Variants/Vector/DVec2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec2.h) | 0 |
| [`Variants/Vector/DVec3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec3.h) | 0 |
| [`Variants/Vector/DVec4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec4.h) | 0 |
| [`Variants/Vector/Double2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Double2.h) | 0 |
| [`Variants/Vector/Double3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Double3.h) | 0 |
| [`Variants/Vector/Double4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Double4.h) | 0 |
| [`Variants/Vector/Float2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Float2.h) | 0 |
| [`Variants/Vector/Float3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Float3.h) | 0 |
| [`Variants/Vector/Float4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Float4.h) | 0 |
| [`Variants/Vector/Int2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Int2.h) | 0 |
| [`Variants/Vector/Int3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Int3.h) | 0 |
| [`Variants/Vector/Int4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Int4.h) | 0 |
| [`Variants/Vector/TVec.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/TVec.h) | 0 |
| [`Variants/Vector/UVec4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/UVec4.h) | 0 |
| [`Variants/Vector/Vec2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec2.h) | 1 |
| [`Variants/Vector/Vec3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec3.h) | 1 |
| [`Variants/Vector/Vec4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec4.h) | 0 |
| [`Variants/Vector/iVec2.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec2.h) | 1 |
| [`Variants/Vector/iVec3.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec3.h) | 1 |
| [`Variants/Vector/iVec4.h`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec4.h) | 1 |

### Top-level subsystem headers (11 headers)

| Header | Export-marked declarations |
|---|---:|
| [`Moss.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss.h)` | 0 |
| [`Moss_Audio.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Audio.h)` | 113 |
| [`Moss_GPU.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_GPU.h)` | 241 |
| [`Moss_GUI.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_GUI.h)` | 73 |
| [`Moss_Navigation.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Navigation.h)` | 0 |
| [`Moss_Network.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Network.h)` | 146 |
| [`Moss_Physics.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Physics.h)` | 39 |
| [`Moss_Platform.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Platform.h)` | 181 |
| [`Moss_Renderer.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_Renderer.h)` | 195 |
| [`Moss_XR.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_XR.h)` | 41 |
| [`Moss_stdinc.h](https://github.com/TxbiG/Moss/blob/main/include/Moss/Moss_stdinc.h)` | 34 |

## Inline implementation header inventory (19 files)

These files provide inline/template definitions or companion implementation details. They are included here because the public API surface cannot be understood from declarations alone when these files contain required definitions.

| Inline header |
|---|
| [`Core/FixedSizeFreeList.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/FixedSizeFreeList.inl) |
| [`Core/JobSystem.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/JobSystem.inl) |
| [`Core/LockFreeHashMap.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/LockFreeHashMap.inl) |
| [`Core/Profiler.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Core/Profiler.inl) |
| [`Physics/Body/Body.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Physics/Body/Body.inl) |
| [`Variants/Matrix/DMat44.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/DMat44.inl) |
| [`Variants/Matrix/Mat44.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Matrix/Mat44.inl) |
| [`Variants/Quat.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Quat.inl) |
| [`Variants/Vector/BVec16.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/BVec16.inl) |
| [`Variants/Vector/DVec2.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec2.inl) |
| [`Variants/Vector/DVec3.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec3.inl) |
| [`Variants/Vector/DVec4.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/DVec4.inl) |
| [`Variants/Vector/UVec4.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/UVec4.inl) |
| [`Variants/Vector/Vec2.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec2.inl) |
| [`Variants/Vector/Vec3.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec3.inl) |
| [`Variants/Vector/Vec4.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/Vec4.inl) |
| [`Variants/Vector/iVec2.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec2.inl) |
| [`Variants/Vector/iVec3.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec3.inl) |
| [`Variants/Vector/iVec4.inl`](https://github.com/TxbiG/Moss/blob/main/include/Moss/Variants/Vector/iVec4.inl) |

## 9. Native integration details to preserve

This is a language-level API specification, not a mechanical ABI mirror. When implementing the projection, keep these source-level distinctions intact:

- Convert pointer-plus-count inputs into a single slice/span/list/array/table/sequence; do not expose a redundant count parameter when the collection already carries its length.
- Convert output pointer pairs (for example, mouse coordinates) into a point/tuple/value object. Do not expose C-style `int*` output arguments.
- Resource factories return owned language objects and translate native allocation/backend failure into the language's normal error model. Destroy/terminate functions become deterministic resource disposal (`Dispose`, `Drop`, `close`, or context-manager release), not ordinary public `destroy_*` calls.
- GUI calls should be scoped to a `GuiContext` instance in every managed binding. They must not silently depend on mutable global “current context” state when the public API can retain an explicit context.
- Vulkan handles, callback pointers, allocator callbacks and backend-specific opaque handles stay behind advanced interop APIs unless the user explicitly opts into native interop. They must not be confused with Moss-owned managed resources.
- Where the native declaration lacks information required by the intended safe API (for example a pointer/count pair, output values, or a resource owner), resolve the native declaration or binding-side contract before implementing that projection; do not guess buffer length or object lifetime.

## 10. Build and distribution contract

- Ship matching native binaries and binding artifacts for each supported OS/architecture combination.
- Keep ABI declarations internal; public API users should not need to know calling conventions, struct packing, pointer widths or native library search paths.
- Make backend and optional dependency feature flags visible through capability queries or build metadata.
- Test a clean build, import/load, initialization/shutdown, object lifetime, callback detach, error translation and resource ownership in CI for every supported platform.
- Keep the generated language API index in sync with the native CMoss export catalogue whenever public native declarations change.

## 11. Compatibility and versioning

Treat the binding's public API as a separately versioned language library. A native API addition does not become a public binding API until the type mapping, ownership, failure semantics and example are specified. Avoid leaking raw `Moss_*` function names into application code. Deprecate public methods using the target language's normal annotation mechanism and provide a migration note in the API Reference itself when a breaking change is unavoidable.

---

**Canonical authority:** Moss C++/CMoss headers and the CMoss API Reference supplied for this documentation set. The native symbol column preserves traceability; the public API column is the intended language-specific design.
