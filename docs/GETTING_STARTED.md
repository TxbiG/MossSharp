# Getting Started with MossSharp

This guide will walk you through setting up MossSharp and creating your first project.

## Prerequisites

- **.NET 9.0 or higher** installed on your system
- A C# IDE (Visual Studio, Visual Studio Code, Rider, etc.)
- Basic knowledge of C# programming

## Installation

### Via NuGet Package Manager

```bash
dotnet add package MossSharp
```

### Via Package Manager Console (Visual Studio)

```powershell
Install-Package MossSharp
```

### Via NuGet.Config

Add the following to your `.csproj` file if using a private feed:

```xml
<ItemGroup>
  <PackageReference Include="MossSharp" Version="2.20.1" />
</ItemGroup>
```

## Creating Your First Project

### Step 1: Create a New Console Application

```bash
dotnet new console -n MyMossGame
cd MyMossGame
```

### Step 2: Add MossSharp Dependency

```bash
dotnet add package MossSharp
```

### Step 3: Write Your First Program

Open `Program.cs` and replace its contents:

```csharp
using Moss.Platform;
using Moss.Audio;

var builder = new PlatformBuilder();
var app = builder.Build();

// Initialize subsystems
AudioSystem.Initialize();

try
{
    // Your application logic here
    Console.WriteLine("MossSharp initialized successfully!");
    
    // Update loop
    while (app.IsRunning)
    {
        app.Update();
        AudioSystem.Update(0.016f); // ~60 FPS
    }
}
finally
{
    // Cleanup
    AudioSystem.Shutdown();
}
```

### Step 4: Build and Run

```bash
dotnet build
dotnet run
```

## Common Patterns

### Initializing Subsystems

Most Moss subsystems follow a similar initialization pattern:

```csharp
// Audio
AudioSystem.Initialize();
// ... use audio ...
AudioSystem.Shutdown();

// Platform (handled by PlatformBuilder)
var platform = new PlatformBuilder().Build();
```

### Creating and Using Audio Streams

```csharp
using Moss.Audio;

AudioSystem.Initialize();

// Create an audio stream
var stream = AudioStream.CreateStream();

// Play audio
stream.Play();

// Stop when done
stream.Stop();
stream.Dispose();

AudioSystem.Shutdown();
```

### XR Initialization

```csharp
using Moss.XR;

var settings = new XRInitSettings
{
    Renderer = "OpenGL", // or "Vulkan", "DirectX12"
    GraphicsDevice = IntPtr.Zero,
    GraphicsContext = IntPtr.Zero
};

XRSystem.Initialize(settings);
```

## Project Configuration

### Recommended .csproj Settings

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MossSharp" Version="2.20.1" />
  </ItemGroup>
</Project>
```

## Platform-Specific Setup

### Windows
No additional setup required. Native binaries are included in the NuGet package.

### Linux
Ensure development libraries are installed:
```bash
sudo apt-get install libgl1-mesa-dev libvulkan-dev
```

### macOS
Xcode command-line tools are required:
```bash
xcode-select --install
```

### Android
Requires Android NDK and SDK. See [Moss Framework Android Guide](https://github.com/TxbiG/MossFramework).

### iOS
Requires Xcode and deployment target iOS 12+.

## Troubleshooting

### Native Library Not Found

**Problem**: `DllNotFoundException` at runtime

**Solution**:
1. Ensure NuGet package is properly installed
2. Check that your platform is supported
3. Verify runtime identifier (RID) matches your system

### Build Errors

**Problem**: `error CS1591: Missing XML comment`

**Solution**: Add this to your .csproj:
```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CS1591</NoWarn>
</PropertyGroup>
```

## Next Steps

- Read the [Architecture](./ARCHITECTURE.md) guide to understand the structure
- Check out [Examples](./EXAMPLES.md) for real-world code samples
- Review the [API Reference](./API_REFERENCE.md) for detailed documentation
- Explore the [Moss Framework](https://github.com/TxbiG/MossFramework) repository for deeper knowledge

## Getting Help

- **Documentation**: See the docs folder
- **Issues**: Report bugs on [GitHub](https://github.com/TxbiG/MossSharp/issues)
- **Discussions**: Join community discussions
