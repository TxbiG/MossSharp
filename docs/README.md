# MossSharp Documentation

Welcome to MossSharp! This is a C# binding for the [Moss Framework](https://github.com/TxbiG/MossFramework), a powerful cross-platform game development and multimedia framework.

## Quick Links
- [Getting Started](./GETTING_STARTED.md) — Set up and create your first project
- [Architecture](./ARCHITECTURE.md) — Understand the structure and design
- [API Reference](./API_REFERENCE.md) — Detailed API documentation
- [Examples](./EXAMPLES.md) — Code examples for common tasks

## What is MossSharp?

MossSharp provides .NET bindings for the Moss Framework, enabling C# developers to build high-performance 2D/3D games and multimedia applications. It bridges the power of the native Moss Framework with the simplicity and productivity of the C# language.

### Key Features
- **Cross-platform Support**: Windows, Linux, macOS, Android, and iOS
- **Game Development**: 2D and 3D rendering, physics, audio, networking
- **Multimedia**: Graphics rendering, particle systems, UI framework
- **VR/AR Support**: XR (Extended Reality) subsystem for immersive experiences
- **Performance**: Native code execution with managed C# wrapper
- **NuGet Distribution**: Easy dependency management via NuGet packages

## Repository Structure

```
MossSharp/
├── src/
│   ├── MossSharp/          # Main C# binding implementation
│   ├── Moss.Native/        # Native interop definitions
│   └── Moss.Native.Debug/  # Debug native bindings
├── docs/                   # Documentation (this folder)
├── MossSharp.csproj        # Project configuration
├── README.md               # Quick overview
└── LICENSE.txt             # MIT License
```

## Project Information

- **Language**: C# (.NET 9.0+)
- **License**: MIT
- **Target Frameworks**: .NET 9.0, .NET 10.0
- **Version**: 2.20.1

## Supported Platforms

MossSharp provides native binaries for:
- **Windows**: x86, x64
- **Linux**: x64, ARM64
- **macOS**: ARM (Apple Silicon), x64
- **Android**: ARM, ARM64, x64
- **iOS**: ARM

## Major Modules

### Audio
Handle sound playback, spatial audio, and real-time audio streaming.
- Audio streams and channels
- 2D and 3D audio listeners
- Audio effects and processing
- Microphone input and speaker output

### Platform
Manage window creation, input handling, system resources, and platform-specific features.
- Window and monitor management
- Input system (keyboard, mouse, gamepad)
- Threading and processor utilities
- Video capture capabilities

### Physics
Simulate realistic physical interactions in 2D and 3D environments.
- Collision detection
- Rigid body dynamics
- Soft body simulation
- Character controllers
- Vehicle physics

### Rendering
Render 2D sprites, 3D geometry, particles, and post-processing effects.
- Multiple graphics APIs (OpenGL, Vulkan, DirectX 12)
- Textures, shaders, and meshes
- Particle systems (CPU and GPU)
- Lighting and shadows
- UI rendering

### GUI (User Interface)
Build interactive user interfaces with containers, controls, and layouts.
- Container types (horizontal, vertical, grid)
- Control widgets (buttons, sliders, text input)
- Layout management
- Event handling

### Networking
Implement multiplayer and networked features.
- Client/server architecture
- Remote Procedure Calls (RPC)
- Network synchronization

### XR (Virtual/Augmented Reality)
Develop immersive VR and AR experiences.
- XR device tracking
- Hand and controller input
- Spatial interaction

### Utility
Common utilities and helper functions.
- Variant types (vectors, matrices, quaternions)
- Collections and data structures
- Noise generation
- Tweening and animations

## Getting Help

- **Documentation**: See [Getting Started](./GETTING_STARTED.md)
- **Examples**: Check out [Examples](./EXAMPLES.md) for common patterns
- **Issues**: Report bugs on [GitHub Issues](https://github.com/TxbiG/MossSharp/issues)
- **Original Framework**: Visit [MossFramework](https://github.com/TxbiG/MossFramework) for more information

## License

MossSharp is licensed under the [MIT License](../LICENSE.txt). You are free to use this binding in commercial and personal projects.
