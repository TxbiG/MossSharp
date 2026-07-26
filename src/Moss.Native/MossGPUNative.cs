using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.GPU
{
    internal enum GPUIndexType : byte { UInt16, UInt32 }
    internal enum CommandQueue : byte { Graphics, Compute, Transfer }
    internal enum GPUPresentMode : byte { VSync, Immediate, Mailbox }
    internal enum GPUSwapchainResult : byte { Success, Timeout, NotReady, Suboptimal, OutOfDate, Lost, Unsupported, Error }
    internal enum PixelFormat { Unknown, R8, RG8, RGB8, RGBA8, BGRA8, SRGB8, SRGBA8, Depth16, Depth24, Depth32F, Depth24Stencil8, Depth32FStencil8 }
    internal enum TextureType { Texture2D, Texture2DArray, Texture3D, TextureCube, TextureCubeArray }
    internal enum TextureFormat { R8, RG8, RGB8, RGBA8, R8Snorm, RG8Snorm, RGB8Snorm, RGBA8Snorm, R16F, RG16F, RGB16F, RGBA16F, R32F, RG32F, RGB32F, RGBA32F, R8UI, RG8UI, RGBA8UI, R16UI, RG16UI, RGBA16UI, R32UI, RG32UI, RGBA32UI, Depth16, Depth24, Depth32F, Depth24Stencil8, Depth32FStencil8, DXT1, DXT3, DXT5, BC4, BC5, BC6H, BC7, SRGB8, SRGBA8, Unknown = -1 }
    internal enum TextureAddressMode { Clamp, Wrap, Mirror }
    internal enum TextureFilter { Nearest, Linear, Point, Anisotropic, LinearMipPoint, PointMipLinear, MinLinearMagPointMipLinear, MinLinearMagPointMipPoint, MinPointMagLinearMipLinear, MinPointMagLinearMipPoint }
    internal enum ResourceState { Undefined, Present, Common, ShaderRead, ShaderWrite, RenderTarget, DepthRead, DepthWrite, TransferSrc, TransferDst, VertexBuffer, IndexBuffer, IndirectArgument }
    internal enum TextureUsage : uint { None = 0, Sampled = 1u << 0, Storage = 1u << 1, ColorTarget = 1u << 2, DepthTarget = 1u << 3, TransferSrc = 1u << 4, TransferDst = 1u << 5 }
    internal enum GPUBufferUsage : uint { None = 0, Vertex = 1u << 0, Index = 1u << 1, Uniform = 1u << 2, Storage = 1u << 3, Indirect = 1u << 4, TransferSrc = 1u << 5, TransferDst = 1u << 6 }
    internal enum ShaderStage : uint { None = 0, Vertex = 1u << 0, Fragment = 1u << 1, Compute = 1u << 2, Geometry = 1u << 3, TessControl = 1u << 4, TessEval = 1u << 5, Mesh = 1u << 6, Task = 1u << 7 }
    internal enum GPUShaderFormat : uint { None = 0, Glsl = 1u << 0, GlslEs = 1u << 1, SpirV = 1u << 2, Dxbc = 1u << 3, Dxil = 1u << 4, Msl = 1u << 5, Metallib = 1u << 6 }
    internal enum ShaderResourceType { UniformBuffer, StorageBuffer, SampledTexture, StorageTexture, Sampler, CombinedTextureSampler, InputAttachment }
    internal enum GPUTextureViewType { Default, Texture2D, Texture2DArray, Texture3D, TextureCube, TextureCubeArray }
    internal enum GPUQueryType { Timestamp, Occlusion, PipelineStatistics }
    internal enum GPUQueryResultFlags : uint { None = 0, Wait = 1u << 0, WithAvailability = 1u << 1, Result64 = 1u << 2 }
    internal enum ResidencyState { Resident, Evicted, Streaming }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUBackendInfo
    {
        public GPUBackendType Type;
        public IntPtr Name;
        public uint ShaderFormats;
        public uint Flags;
    }
    [StructLayout(LayoutKind.Sequential)] internal struct MossRect { public int X; public int Y; public int W; public int H; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUViewport { public float X; public float Y; public float W; public float H; public float MinDepth; public float MaxDepth; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUBufferDesc { public ulong Size; public GPUBufferUsage Usage; public int CpuVisible; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUBufferBinding { public IntPtr Buffer; public ulong Offset; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUBufferRegion { public ulong Offset; public ulong Size; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUBlitRegion { public uint SrcMipLevel; public uint SrcBaseLayer; public uint SrcLayerCount; public uint DstMipLevel; public uint DstBaseLayer; public uint DstLayerCount; public uint SrcX; public uint SrcY; public uint SrcZ; public uint DstX; public uint DstY; public uint DstZ; public uint Width; public uint Height; public uint Depth; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureRegion { public uint MipLevel; public uint BaseLayer; public uint LayerCount; public uint X; public uint Y; public uint Z; public uint Width; public uint Height; public uint Depth; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureTransferInfo { public IntPtr Data; public uint RowPitch; public uint SlicePitch; public GPUTextureRegion Region; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUStorageBufferReadWriteBinding { public IntPtr Buffer; public ulong Offset; public ulong Size; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUStorageTextureReadWriteBinding { public IntPtr Texture; public uint MipLevel; public uint Layer; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureSamplerBinding { public IntPtr Texture; public IntPtr Sampler; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureBinding { public IntPtr TextureView; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureCreateInfo { public TextureType Type; public TextureFormat Format; public TextureUsage Usage; public uint Width; public uint Height; public uint Depth; public uint Layers; public uint MipLevels; public uint SampleCount; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTextureViewCreateInfo { public IntPtr Texture; public GPUTextureViewType Type; public TextureFormat Format; public TextureUsage Usage; public uint BaseMipLevel; public uint MipLevelCount; public uint BaseLayer; public uint LayerCount; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUSamplerCreateInfo { public TextureFilter MinFilter; public TextureFilter MagFilter; public TextureFilter MipFilter; public TextureAddressMode AddressU; public TextureAddressMode AddressV; public TextureAddressMode AddressW; public float MipLodBias; public float MinLod; public float MaxLod; [MarshalAs(UnmanagedType.I1)] public bool EnableAnisotropy; public float MaxAnisotropy; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUShaderCreateInfo { public ShaderStage Stage; public IntPtr Bytecode; public nuint BytecodeSize; public GPUShaderFormat Format; public IntPtr EntryPoint; public IntPtr DebugName; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUComputePipelineCreateInfo { public IntPtr ComputeShader; public IntPtr SetLayouts; public uint SetLayoutCount; public IntPtr DebugName; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUTransferBufferCreateInfo { public ulong Size; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUQueryPoolCreateInfo { public GPUQueryType Type; public uint QueryCount; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUResourceSetLayoutBinding { public uint Binding; public ShaderResourceType Type; public uint Count; public ShaderStage StageMask; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUResourceSetLayoutCreateInfo { public uint SetIndex; public IntPtr Bindings; public uint BindingCount; [MarshalAs(UnmanagedType.I1)] public bool Bindless; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUResourceBinding { public uint Binding; public ShaderResourceType Type; public ShaderStage StageMask; public GPUBufferBinding UniformBuffer; public GPUStorageBufferReadWriteBinding StorageBuffer; public GPUTextureBinding SampledTexture; public GPUStorageTextureReadWriteBinding StorageTexture; public IntPtr Sampler; public GPUTextureSamplerBinding TextureSampler; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUResourceSetCreateInfo { public IntPtr Layout; public IntPtr Bindings; public uint BindingCount; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUBarrierInfo { public IntPtr TextureBarriers; public uint TextureBarrierCount; public IntPtr BufferBarriers; public uint BufferBarrierCount; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUDeviceProperties { public IntPtr DeviceName; public IntPtr VendorName; public uint VendorId; public uint DeviceId; public ulong DedicatedVideoMemory; public ulong SharedSystemMemory; [MarshalAs(UnmanagedType.I1)] public bool SupportsCompute; [MarshalAs(UnmanagedType.I1)] public bool SupportsMeshShaders; [MarshalAs(UnmanagedType.I1)] public bool SupportsBindless; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUDeviceDesc { public IntPtr Window; public uint BackbufferWidth; public uint BackbufferHeight; public GPUPresentMode PresentMode; [MarshalAs(UnmanagedType.I1)] public bool EnableValidation; [MarshalAs(UnmanagedType.I1)] public bool EnableDebugMarkers; }
    [StructLayout(LayoutKind.Sequential)] internal struct TextureDesc { public TextureType Type; public TextureFormat Format; public TextureUsage Usage; public uint Width; public uint Height; public uint Depth; public uint Layers; public uint MipLevels; public int GenerateMips; }
    [StructLayout(LayoutKind.Sequential)] internal struct TextureUploadDesc { public IntPtr Data; public uint Width; public uint Height; public uint Depth; public uint MipLevel; public uint BaseLayer; public uint LayerCount; public uint RowPitch; public uint SlicePitch; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUSamplerDesc { public TextureFilter MinFilter; public TextureFilter MagFilter; public TextureFilter MipFilter; public TextureAddressMode AddressU; public TextureAddressMode AddressV; public TextureAddressMode AddressW; public float MipLodBias; public float MaxAnisotropy; }
    [StructLayout(LayoutKind.Sequential)] internal struct TextureAssetDesc { public IntPtr Path; public TextureFormat Format; [MarshalAs(UnmanagedType.I1)] public bool GenerateMips; }
    [StructLayout(LayoutKind.Sequential)] internal struct ShaderAssetDesc { public IntPtr Path; public ShaderStage Stage; public IntPtr EntryPoint; }
    [StructLayout(LayoutKind.Sequential)] internal struct ShaderDesc { public ShaderStage Stage; public IntPtr Path; public IntPtr EntryPoint; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUClothSolverDesc { public IntPtr Pipeline; [MarshalAs(UnmanagedType.I1)] public bool TakePipelineOwnership; public GPUShaderCreateInfo Shader; public GPUComputePipelineCreateInfo PipelineInfo; public uint GroupSize; }
    [StructLayout(LayoutKind.Sequential)] internal struct GPUClothDispatchDesc { public IntPtr Positions; public IntPtr PreviousPositions; public IntPtr Velocities; public IntPtr Constraints; public ulong PositionsSize; public ulong PreviousPositionsSize; public ulong VelocitiesSize; public ulong ConstraintsSize; public uint ParticleCount; public uint ConstraintCount; public uint Iterations; public float DeltaTime; public float Damping; public float Stiffness; }

    internal enum GPUBackendType : byte
    {
        Default = 0,
        OpenGL = 1,
        OpenGLES = 2,
        Vulkan = 3,
        DirectX12 = 4,
        Metal = 5,
        Fallback = 6
    }

    [Flags]
    internal enum GPUExternalFeature : uint
    {
        Fsr1 = 1u << 0,
        Fsr2 = 1u << 1,
        MeshOptimizer = 1u << 2,
        Smolv = 1u << 3,
        VulkanMemoryAllocator = 1u << 4
    }

    internal enum FSR2QualityMode : uint
    {
        Quality = 1,
        Balanced = 2,
        Performance = 3,
        UltraPerformance = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUExternalFeatures
    {
        [MarshalAs(UnmanagedType.I1)] public bool Fsr1;
        [MarshalAs(UnmanagedType.I1)] public bool Fsr2;
        [MarshalAs(UnmanagedType.I1)] public bool MeshOptimizer;
        [MarshalAs(UnmanagedType.I1)] public bool Smolv;
        [MarshalAs(UnmanagedType.I1)] public bool VulkanMemoryAllocator;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FSR2RenderResolution
    {
        public uint Width;
        public uint Height;
    }
    internal static unsafe class MossGPUNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUDevice(in GPUDeviceDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUDeviceWithProperties(in GPUDeviceDesc desc, in GPUDeviceProperties requestedProperties);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DestroyGPUDevice(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGPUDeviceDriver(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGPUDeviceProperties(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetNumGPUDrivers();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGPUDriver(uint driverIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GetGPUShaderFormats(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUSupportsProperties(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUSupportsShaderFormats(IntPtr device, uint shaderFormatMask);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WindowSupportsGPUPresentMode(IntPtr window, GPUPresentMode presentMode);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ClaimWindowForGPUDevice(IntPtr device, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseWindowFromGPUDevice(IntPtr device, IntPtr window);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUBuffer(IntPtr device, in GPUBufferDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUBufferCreate(IntPtr device, in GPUBufferDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUBuffer(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferDestroy(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferUpload(IntPtr device, IntPtr buffer, IntPtr data, ulong size, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUBufferMap(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferUnmap(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UploadToGPUBuffer(IntPtr device, IntPtr dstBuffer, IntPtr srcData, ulong size, ulong dstOffset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DownloadFromGPUBuffer(IntPtr device, IntPtr srcBuffer, IntPtr dstData, ulong size, ulong srcOffset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUBufferName(IntPtr device, IntPtr buffer, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTexture(IntPtr device, in GPUTextureCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTextureView(IntPtr device, in GPUTextureViewCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUTexture(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUTextureView(IntPtr device, IntPtr textureView);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_TextureCreate(IntPtr device, in TextureDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureDestroy(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureUpload(IntPtr device, IntPtr texture, in TextureUploadDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureGenerateMips(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UploadToGPUTexture(IntPtr device, IntPtr dstTexture, in GPUTextureTransferInfo transferInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DownloadFromGPUTexture(IntPtr device, IntPtr srcTexture, in GPUTextureRegion srcRegion, IntPtr dstData, uint dstRowPitch, uint dstSlicePitch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUTextureName(IntPtr device, IntPtr texture, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureSetResidency(IntPtr texture, ResidencyState state);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern TextureFormat Moss_GetGPUTextureFormatFromPixelFormat(PixelFormat pixelFormat);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern PixelFormat Moss_GetPixelFormatFromGPUTextureFormat(TextureFormat textureFormat);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GPUTextureFormatTexelBlockSize(TextureFormat format);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_CalculateGPUTextureFormatSize(TextureFormat format, uint width, uint height, uint depth);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUTextureSupportsFormat(IntPtr device, TextureFormat format, TextureUsage usage);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUTextureSupportsSampleCount(IntPtr device, TextureFormat format, uint sampleCount);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUSampler(IntPtr device, in GPUSamplerCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUSamplerCreate(IntPtr device, in GPUSamplerDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUSampler(IntPtr device, IntPtr sampler);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUSamplerDestroy(IntPtr device, IntPtr sampler);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUShader(IntPtr device, in GPUShaderCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUShader(IntPtr device, IntPtr shader);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ShaderCreate(IntPtr device, in ShaderDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShaderDestroy(IntPtr device, IntPtr shader);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUComputePipeline(IntPtr device, in GPUComputePipelineCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUComputePipeline(IntPtr device, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_PipelineCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineDestroy(IntPtr device, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AcquireGPUCommandBuffer(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUCommandBufferBegin(IntPtr device, CommandQueue queue);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUCommandBufferEnd(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUCommandBufferSubmit(IntPtr device, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SubmitGPUCommandBuffer(IntPtr device, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_SubmitGPUCommandBufferAndAcquireFence(IntPtr device, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CancelGPUCommandBuffer(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_WaitForGPUIdle(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_WaitForGPUFences(IntPtr device, IntPtr fences, uint fenceCount, [MarshalAs(UnmanagedType.I1)] bool waitAll, ulong timeoutNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_QueryGPUFence(IntPtr device, IntPtr fence);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUFence(IntPtr device, IntPtr fence);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPUComputePass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EndGPUComputePass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPUCopyPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EndGPUCopyPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPURenderPass(IntPtr cmd, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EndGPURenderPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBeginRenderPass(IntPtr cmd, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdEndRenderPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindPipeline(IntPtr cmd, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindResourceSet(IntPtr cmd, uint setIndex, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindVertexBuffer(IntPtr cmd, IntPtr buffer, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindIndexBuffer(IntPtr cmd, IntPtr buffer, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDraw(IntPtr cmd, uint vertexCount, uint firstVertex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDrawIndexed(IntPtr cmd, uint indexCount, uint firstIndex, int vertexOffset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDispatch(IntPtr cmd, uint x, uint y, uint z);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputePipeline(IntPtr cmd, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUPipeline(IntPtr cmd, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUResourceSet(IntPtr cmd, uint setIndex, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUIndexBuffer(IntPtr cmd, IntPtr buffer, ulong offset, GPUIndexType indexType);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexBuffers(IntPtr cmd, uint firstBinding, GPUBufferBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexUniformBuffers(IntPtr cmd, uint firstSlot, GPUBufferBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUFragmentUniformBuffers(IntPtr cmd, uint firstSlot, GPUBufferBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputeUniformBuffers(IntPtr cmd, uint firstSlot, GPUBufferBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexTextures(IntPtr cmd, uint firstSlot, IntPtr textures, uint textureCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUFragmentTextures(IntPtr cmd, uint firstSlot, IntPtr textures, uint textureCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputeTextures(IntPtr cmd, uint firstSlot, IntPtr textures, uint textureCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexSamplers(IntPtr cmd, uint firstSlot, IntPtr samplers, uint samplerCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUFragmentSamplers(IntPtr cmd, uint firstSlot, IntPtr samplers, uint samplerCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputeSamplers(IntPtr cmd, uint firstSlot, IntPtr samplers, uint count);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexStorageBuffers(IntPtr cmd, uint firstSlot, GPUStorageBufferReadWriteBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUFragmentStorageBuffers(IntPtr cmd, uint firstSlot, GPUStorageBufferReadWriteBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputeStorageBuffers(IntPtr cmd, uint firstSlot, GPUStorageBufferReadWriteBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUVertexStorageTextures(IntPtr cmd, uint firstSlot, GPUStorageTextureReadWriteBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUFragmentStorageTextures(IntPtr cmd, uint firstSlot, GPUStorageTextureReadWriteBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindGPUComputeStorageTextures(IntPtr cmd, uint firstSlot, GPUStorageTextureReadWriteBinding* bindings, uint bindingCount);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DrawGPUPrimitives(IntPtr cmd, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DrawGPUIndexedPrimitives(IntPtr cmd, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DrawGPUPrimitivesIndirect(IntPtr cmd, IntPtr indirectBuffer, ulong offset, uint drawCount, uint stride);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DrawGPUIndexedPrimitivesIndirect(IntPtr cmd, IntPtr indirectBuffer, ulong offset, uint drawCount, uint stride);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DispatchGPUCompute(IntPtr cmd, uint groupCountX, uint groupCountY, uint groupCountZ);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DispatchGPUComputeIndirect(IntPtr cmd, IntPtr indirectBuffer, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CopyGPUBufferToBuffer(IntPtr cmd, IntPtr src, IntPtr dst, in GPUBufferRegion srcRegion, in GPUBufferRegion dstRegion);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CopyGPUBufferToTexture(IntPtr cmd, IntPtr src, IntPtr dst, in GPUBufferRegion srcRegion, in GPUTextureRegion dstRegion);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CopyGPUTextureToBuffer(IntPtr cmd, IntPtr src, IntPtr dst, in GPUTextureRegion srcRegion, in GPUBufferRegion dstRegion);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CopyGPUTextureToTexture(IntPtr cmd, IntPtr src, IntPtr dst, in GPUTextureRegion srcRegion, in GPUTextureRegion dstRegion);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BlitGPUTexture(IntPtr cmd, IntPtr src, IntPtr dst, in GPUBlitRegion region);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTransferBuffer(IntPtr device, in GPUTransferBufferCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MapGPUTransferBuffer(IntPtr device, IntPtr transferBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UnmapGPUTransferBuffer(IntPtr device, IntPtr transferBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUTransferBuffer(IntPtr device, IntPtr transferBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUQueryPool(IntPtr device, in GPUQueryPoolCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUQueryPool(IntPtr device, IntPtr queryPool);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResetGPUQueryPool(IntPtr cmd, IntPtr queryPool, uint firstQuery, uint queryCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_WriteGPUTimestamp(IntPtr cmd, IntPtr queryPool, uint queryIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPUQuery(IntPtr cmd, IntPtr queryPool, uint queryIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_EndGPUQuery(IntPtr cmd, IntPtr queryPool, uint queryIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetGPUQueryResults(IntPtr device, IntPtr queryPool, uint firstQuery, uint queryCount, IntPtr data, ulong dataSize, ulong stride, GPUQueryResultFlags flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUResourceSetLayout(IntPtr device, in GPUResourceSetLayoutCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUResourceSet(IntPtr device, in GPUResourceSetCreateInfo createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUResourceSetLayout(IntPtr device, IntPtr layout);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUResourceSet(IntPtr device, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_UpdateGPUResourceSet(IntPtr device, IntPtr set, GPUResourceBinding* bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUFramebuffer(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseGPUFramebuffer(IntPtr device, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BarrierGPUResources(IntPtr cmd, in GPUBarrierInfo barrierInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TransitionGPUBuffer(IntPtr cmd, IntPtr buffer, ResourceState oldState, ResourceState newState);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TransitionGPUTexture(IntPtr cmd, IntPtr texture, ResourceState oldState, ResourceState newState);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GenerateMipmapsForGPUTexture(IntPtr cmd, IntPtr texture);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AcquireGPUSwapchainTexture(IntPtr device, IntPtr window, out IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUSwapchainResult Moss_AcquireGPUSwapchainTextureStatus(IntPtr device, IntPtr window, out IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern TextureFormat Moss_GetGPUSwapchainTextureFormat(IntPtr device, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetGPUSwapchainParameters(IntPtr device, IntPtr window, uint width, uint height, GPUPresentMode presentMode);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUSwapchainResult Moss_ResizeGPUSwapchain(IntPtr device, IntPtr window, uint width, uint height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WaitAndAcquireGPUSwapchainTexture(IntPtr device, IntPtr window, out IntPtr texture, ulong timeoutNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUSwapchainResult Moss_WaitAndAcquireGPUSwapchainTextureStatus(IntPtr device, IntPtr window, out IntPtr texture, ulong timeoutNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WaitForGPUSwapchain(IntPtr device, IntPtr window, ulong timeoutNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WindowSupportsGPUSwapchainComposition(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_PresentGPUSwapchain(IntPtr device, IntPtr window, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUSwapchainResult Moss_PresentGPUSwapchainStatus(IntPtr device, IntPtr window, IntPtr texture);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUClear(IntPtr cmd, float r, float g, float b, float a);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUAllowedFramesInFlight(IntPtr device, uint framesInFlight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUBlendConstants(IntPtr cmd, float r, float g, float b, float a);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUScissor(IntPtr cmd, in MossRect scissor);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUStencilReference(IntPtr cmd, uint reference);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SetGPUViewport(IntPtr cmd, in GPUViewport viewport);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_InsertGPUDebugLabel(IntPtr cmd, [MarshalAs(UnmanagedType.LPUTF8Str)] string label);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PushGPUDebugGroup(IntPtr cmd, [MarshalAs(UnmanagedType.LPUTF8Str)] string label);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PopGPUDebugGroup(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PushGPUComputeUniformData(IntPtr cmd, uint slot, IntPtr data, uint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PushGPUFragmentUniformData(IntPtr cmd, uint slot, IntPtr data, uint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PushGPUVertexUniformData(IntPtr cmd, uint slot, IntPtr data, uint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GDKResumeGPU(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GDKSuspendGPU(IntPtr device);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUClothSolverCreate(IntPtr device, in GPUClothSolverDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUClothSolverDestroy(IntPtr device, IntPtr solver);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUClothSolverDispatch(IntPtr cmd, IntPtr solver, in GPUClothDispatchDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_GPUAssetLoadTexture(IntPtr manager, IntPtr device, in TextureAssetDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_GPUAssetLoadShader(IntPtr manager, IntPtr device, in ShaderAssetDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RGPassReadTexture(IntPtr pass, IntPtr texture, ResourceState state);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RGPassWriteTexture(IntPtr pass, IntPtr texture, ResourceState state);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GPUDeviceSupportsAsyncCompute(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_BindlessRegisterTexture(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_BindlessRegisterBuffer(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BindlessUnregister(IntPtr device, uint handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUFatalError(IntPtr result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUBackendType Moss_GetCompiledGPUBackendType();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern GPUBackendType Moss_GetGPUDeviceBackendType(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUBackendGetName(GPUBackendType backend);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GPUBackendGetShaderFormats(GPUBackendType backend);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUBackendIsCompiled(GPUBackendType backend);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUBackendIsSupportedOnPlatform(GPUBackendType backend);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GetGPUBackendInfo(GPUBackendType backend, out GPUBackendInfo info);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GPUGetExternalFeatureMask();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUExternalFeatureAvailable(GPUExternalFeature feature);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUGetExternalFeatures(out GPUExternalFeatures features);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUExternalFeatureName(GPUExternalFeature feature);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FSR1BuildEasuConstants(uint* outConstants16, float inputViewportWidth, float inputViewportHeight, float inputWidth, float inputHeight, float outputWidth, float outputHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_FSR2GetUpscaleRatio(FSR2QualityMode quality);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_FSR2GetRenderResolution(uint displayWidth, uint displayHeight, FSR2QualityMode quality, out FSR2RenderResolution resolution);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_FSR2GetJitterPhaseCount(int renderWidth, int displayWidth);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_FSR2GetJitterOffset(out float x, out float y, int index, int phaseCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_MeshGenerateVertexRemap(uint* destination, uint* indices, nuint indexCount, IntPtr vertices, nuint vertexCount, nuint vertexSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_MeshOptimizeVertexCache(uint* destination, uint* indices, nuint indexCount, nuint vertexCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_MeshOptimizeOverdraw(uint* destination, uint* indices, nuint indexCount, float* vertexPositions, nuint vertexCount, nuint vertexPositionsStride, float threshold);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_MeshOptimizeVertexFetch(IntPtr destination, uint* indices, nuint indexCount, IntPtr vertices, nuint vertexCount, nuint vertexSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SMOLVEncode(IntPtr spirvData, nuint spirvSize, IntPtr outSmolvData, ref nuint inoutSmolvSize, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_SMOLVGetDecodedSize(IntPtr smolvData, nuint smolvSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SMOLVDecode(IntPtr smolvData, nuint smolvSize, IntPtr outSpirvData, nuint spirvSize, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GPUVulkanMemoryAllocatorAvailable();
    }
}
