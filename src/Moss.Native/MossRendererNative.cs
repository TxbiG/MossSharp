using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Renderer
{
    internal enum MaterialTextureType
    {
        Color,
        Roughness,
        Metalness,
        Normal,
        Occlusion,
        Emission,
        Height,
        AlphaMask,
        Packed,
        Max
    }

    internal enum PresentMode
    {
        Immediate = 0,
        Mailbox = 1,
        VSync = 2,
        Adaptive = 3
    }

    internal enum ResourceState
    {
        Undefined = 0,
        Common = 1,
        RenderTarget = 2,
        DepthWrite = 3,
        DepthRead = 4,
        ShaderResource = 5,
        UnorderedAccess = 6,
        CopySource = 7,
        CopyDest = 8,
        Present = 9
    }

    internal enum TextureFormat
    {
        Unknown = 0,
        R8 = 1,
        RG8 = 2,
        RGB8 = 3,
        RGBA8 = 4,
        BGRA8 = 5,
        RGBA16F = 6,
        RGBA32F = 7,
        Depth16 = 8,
        Depth24Stencil8 = 9,
        Depth32F = 10
    }

    internal enum MossModelLoadError
    {
        None = 0,
        FileNotFound = 1,
        UnsupportedFormat = 2,
        ParseFailed = 3,
        OutOfMemory = 4,
        Unknown = 5
    }

    internal enum MossFSR2QualityMode : uint
    {
        Quality = 1,
        Balanced = 2,
        Performance = 3,
        UltraPerformance = 4
    }
    internal enum MossUpscaler
    {
        None,
        Fsr1,
        Fsr2
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Color
    {
        public float R;
        public float G;
        public float B;
        public float A;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Vec2
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Vec3
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Mat44
    {
        public fixed float M[16];
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossRendererUpscalerDesc
    {
        public MossUpscaler Upscaler;
        public MossFSR2QualityMode Fsr2Quality;
        [MarshalAs(UnmanagedType.I1)] public bool EnableSharpening;
        public float Sharpness;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MossRendererDesc
    {
        public IntPtr Window;
        public IntPtr GpuDevice;
        public Color ClearColor;
        public uint BackbufferWidth;
        public uint BackbufferHeight;
        public uint VirtualWidth;
        public uint VirtualHeight;
        public PresentMode PresentMode;
        [MarshalAs(UnmanagedType.I1)] public bool EnableDebug;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossRenderFrameDesc
    {
        public IntPtr Camera;
        public float DeltaTime;
        public float WorldScale;
        public Color ClearColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDrawMeshDesc
    {
        public IntPtr Mesh;
        public IntPtr Pipeline;
        public IntPtr ResourceSet;
        public IntPtr Transform;
        public uint Visibility;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDrawModelDesc
    {
        public IntPtr Model;
        public IntPtr Pipeline;
        public IntPtr ResourceSet;
        public IntPtr Transform;
        public uint Visibility;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossMaterialTextureDesc
    {
        public MaterialTextureType Type;
        public IntPtr Texture;
        public IntPtr Sampler;
        public uint Binding;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossMaterialUniformDesc
    {
        public IntPtr Name;
        public IntPtr Data;
        public uint Size;
        public uint Binding;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossShaderVariantDesc
    {
        public IntPtr Name;
        public IntPtr VertexShader;
        public IntPtr FragmentShader;
        public IntPtr ComputeShader;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossMaterialDesc
    {
        public IntPtr Name;
        public IntPtr Textures;
        public uint TextureCount;
        public IntPtr Uniforms;
        public uint UniformCount;
        public IntPtr Variants;
        public uint VariantCount;
        public IntPtr Pipeline;
        public IntPtr ResourceSet;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPipelineCacheDesc
    {
        public uint MaxPipelines;
        [MarshalAs(UnmanagedType.I1)] public bool AllowHotReload;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossSpriteDesc
    {
        public IntPtr Texture;
        public IntPtr Material;
        public Vec2 Position;
        public Vec2 Size;
        public Vec2 Origin;
        public Vec2 UvMin;
        public Vec2 UvMax;
        public Color Color;
        public float Rotation;
        public float Depth;
        public uint Visibility;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDebugLineDesc
    {
        public Vec3 From;
        public Vec3 To;
        public Color Color;
        public float Thickness;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDebugBoxDesc
    {
        public Vec3 Center;
        public Vec3 Size;
        public IntPtr Transform;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDebugSphereDesc
    {
        public Vec3 Center;
        public float Radius;
        public Color Color;
        public uint Segments;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDebugNavmeshDesc
    {
        public IntPtr Vertices;
        public uint VertexCount;
        public IntPtr Indices;
        public uint IndexCount;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDebugPhysicsContactDesc
    {
        public Vec3 Position;
        public Vec3 Normal;
        public float Impulse;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossCamera2DDesc
    {
        public Vec2 Position;
        public Vec2 Offset;
        public float Zoom;
        public float Rotation;
        public float ViewportWidth;
        public float ViewportHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossCamera3DDesc
    {
        public Vec3 Position;
        public Vec3 Target;
        public Vec3 Up;
        public float FovDegrees;
        public float AspectRatio;
        public float NearPlane;
        public float FarPlane;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossEditorCameraDesc
    {
        public MossCamera3DDesc Camera;
        public float MoveSpeed;
        public float LookSensitivity;
        [MarshalAs(UnmanagedType.I1)] public bool OrbitMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossShadowMapDesc
    {
        public uint Width;
        public uint Height;
        public TextureFormat DepthFormat;
        [MarshalAs(UnmanagedType.I1)] public bool EnableCascades;
        public uint CascadeCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPostProcessPass
    {
        public IntPtr Pipeline;
        public IntPtr ResourceSet;
        public IntPtr Target;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Viewport
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float MinDepth;
        public float MaxDepth;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SubViewport
    {
        public Viewport Viewport;
        public IntPtr Target;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossMeshAssetDesc
    {
        public IntPtr Path;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossFontAssetDesc
    {
        public IntPtr Path;
        public float PixelSize;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MossFontGlyph
    {
        public uint Codepoint;
        public float X0;
        public float Y0;
        public float X1;
        public float Y1;
        public float U0;
        public float V0;
        public float U1;
        public float V1;
        public float XAdvance;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossFontDesc
    {
        public IntPtr Path;
        public IntPtr Data;
        public nuint DataSize;
        public float PixelSize;
        public uint AtlasWidth;
        public uint AtlasHeight;
        public uint FirstCodepoint;
        public uint CodepointCount;
        [MarshalAs(UnmanagedType.I1)] public bool UploadToGpu;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossFontMetrics
    {
        public float PixelSize;
        public float Ascent;
        public float Descent;
        public float LineGap;
        public float LineHeight;
        public float Baseline;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossTextMeasure
    {
        public float Width;
        public float Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossDrawTextDesc
    {
        public IntPtr Font;
        public IntPtr Text;
        public Vec2 Position;
        public Color Color;
        public float Scale;
        public float Depth;
        public uint Visibility;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossFontVertex
    {
        public Vec2 Position;
        public Vec2 Uv;
        public Color Color;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void MossSubViewportRecordFn(IntPtr commandBuffer, IntPtr userData);

    internal static unsafe class MossRendererNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreate(in MossRendererDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateRenderer(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroy(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerminateRenderer(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererBeginFrame(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererBeginFrameEx(IntPtr renderer, in MossRenderFrameDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererEndFrame(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererGetGPUDevice(IntPtr renderer);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateMaterial(IntPtr renderer, in MossMaterialDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyMaterial(IntPtr renderer, IntPtr material);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_MaterialSetUniform(IntPtr material, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr data, uint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_MaterialSetTexture(IntPtr material, MaterialTextureType type, IntPtr texture, IntPtr sampler);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreatePipeline(IntPtr renderer, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineBind(IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineUnbind(IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineSetUniform(IntPtr pipeline, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr data, uint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineFlush(IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_PipelineValidate(IntPtr pipeline);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateResourceSet(IntPtr renderer, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyResourceSet(IntPtr renderer, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBindUniformBuffer(IntPtr set, uint binding, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBindUniformBufferRange(IntPtr set, uint binding, IntPtr buffer, nuint offset, nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBindStorageBuffer(IntPtr set, uint binding, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBindTexture(IntPtr set, uint binding, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBindSampler(IntPtr set, uint binding, IntPtr sampler);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetBind(IntPtr renderer, IntPtr pipeline, uint setIndex, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ResourceSetCreate(IntPtr renderer, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ResourceSetDestroy(IntPtr set);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateFramebuffer(IntPtr renderer, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyFramebuffer(IntPtr renderer, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FramebufferResize(IntPtr framebuffer, uint width, uint height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PostProcessExecute(IntPtr renderer, in MossPostProcessPass pass);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FramebufferBegin(IntPtr renderer, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FramebufferEnd(IntPtr renderer, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureBarrier(IntPtr renderer, IntPtr texture, ResourceState oldState, ResourceState newState);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererGetBackbuffer(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RenderSubViewport(IntPtr commandBuffer, in SubViewport subViewport, MossSubViewportRecordFn record, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FramebufferCreate(IntPtr renderer, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FramebufferDestroy(IntPtr framebuffer);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MeshLoadFBX([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MeshLoadOBJ([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MeshLoadGLB([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ModelLoadFBX([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ModelLoadOBJ([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ModelLoadGLB([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ModelLoad([MarshalAs(UnmanagedType.LPUTF8Str)] string filePath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossModelLoadError Moss_ModelGetLastLoadError();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ModelGetLastLoadErrorMessage();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_ModelFindBone(IntPtr model, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_ModelFindMorphTarget(IntPtr model, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Moss_ModelSetBoneTransform")] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ModelSetBoneTransformPtr(IntPtr model, uint boneIndex, float* transform16);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Moss_ModelSetMorphWeight")] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ModelSetMorphWeight(IntPtr model, uint morphIndex, float weight);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_RendererAssetLoadMesh(IntPtr manager, IntPtr renderer, in MossMeshAssetDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererAssetGetMesh(IntPtr manager, nuint handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_RendererAssetLoadFont(IntPtr manager, IntPtr renderer, in MossFontAssetDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererAssetGetFont(IntPtr manager, nuint handle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateFont(IntPtr renderer, in MossFontDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyFont(IntPtr renderer, IntPtr font);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontLoad([MarshalAs(UnmanagedType.LPUTF8Str)] string path, float pixelSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontCreateFromMemory(IntPtr data, nuint dataSize, float pixelSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_FontDestroy(IntPtr font);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontGetMetrics(IntPtr font);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontGetGlyph(IntPtr font, uint codepoint);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontGetAtlasPixels(IntPtr font, out uint width, out uint height, out uint stride);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_FontGetAtlasTexture(IntPtr font);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_FontUploadAtlas(IntPtr renderer, IntPtr font);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossTextMeasure Moss_FontMeasureText(IntPtr font, [MarshalAs(UnmanagedType.LPUTF8Str)] string text, float scale);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_FontBuildTextQuads(in MossDrawTextDesc desc, MossFontVertex* outVertices, nuint vertexCapacity);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawText(IntPtr renderer, in MossDrawTextDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_RendererGetFontBackendMask();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_RendererBackendSupportsFonts(byte backend);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawMesh(IntPtr renderer, in MossDrawMeshDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawModelDesc(IntPtr renderer, in MossDrawModelDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawModel(IntPtr renderer, IntPtr model, float* transform);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawSprite(IntPtr renderer, in MossSpriteDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateSpriteBatch(IntPtr renderer, uint capacity);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroySpriteBatch(IntPtr renderer, IntPtr batch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_SpriteBatchClear(IntPtr batch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_SpriteBatchAdd(IntPtr batch, in MossSpriteDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawSpriteBatch(IntPtr renderer, IntPtr batch);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateDebugDrawList(IntPtr renderer, uint capacity);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyDebugDrawList(IntPtr renderer, IntPtr list);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DebugDrawListClear(IntPtr list);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_DebugDrawLine(IntPtr list, in MossDebugLineDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_DebugDrawBox(IntPtr list, in MossDebugBoxDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_DebugDrawSphere(IntPtr list, in MossDebugSphereDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_DebugDrawNavmesh(IntPtr list, in MossDebugNavmeshDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_DebugDrawPhysicsContact(IntPtr list, in MossDebugPhysicsContactDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawDebugList(IntPtr renderer, IntPtr list);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CameraCreate2D(in MossCamera2DDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CameraCreate3D(in MossCamera3DDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CameraCreateEditor(in MossEditorCameraDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CameraDestroy(IntPtr camera);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererSetCamera(IntPtr renderer, IntPtr camera);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_RendererCreateShadowMap(IntPtr renderer, in MossShadowMapDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDestroyShadowMap(IntPtr renderer, IntPtr shadowMap);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererBeginShadowPass(IntPtr renderer, IntPtr shadowMap);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererEndShadowPass(IntPtr renderer, IntPtr shadowMap);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MeshDraw(IntPtr mesh);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ModelDraw(IntPtr model);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MeshRemove(IntPtr mesh);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ModelRemove(IntPtr model);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUGetPhysicalDeviceName();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUAPIGetName();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUAPIGetVersion();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererSetUpscaler(IntPtr renderer, MossUpscaler upscaler);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererSetUpscalerDesc(IntPtr renderer, in MossRendererUpscalerDesc desc);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawLines2D(IntPtr renderer, IntPtr positions, Color color, float thickness);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCylinder2D(IntPtr renderer, in Mat44 matrix, float halfHeight, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawBone2D(IntPtr renderer, in Mat44 transform, float length, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCoordinateSystem2D(IntPtr renderer, in Mat44 matrix, float size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCapsule2D(IntPtr renderer, in Mat44 matrix, float halfHeightOfCylinder, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawGizmo2D(IntPtr renderer, in Mat44 transform, float size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawLines3D(IntPtr renderer, IntPtr positions, Color color, float thickness);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawBone3D(IntPtr renderer, in Mat44 transform, float length, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCoordinateSystem3D(IntPtr renderer, in Mat44 matrix, float size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCapsule3D(IntPtr renderer, in Mat44 matrix, float halfHeightOfCylinder, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawGizmo3D(IntPtr renderer, in Mat44 transform, float size);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawLine2D(IntPtr renderer, in Vec2 from, in Vec2 to, Color color, float thickness);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawBox2D(IntPtr renderer, in Vec2 center, in Vec2 size, float rotation, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCircle2D(IntPtr renderer, in Vec2 center, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawTriangle2D(IntPtr renderer, in Vec2 p0, in Vec2 p1, in Vec2 p2, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawArrow2D(IntPtr renderer, in Vec2 from, in Vec2 to, Color color, float headSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawMarker2D(IntPtr renderer, in Vec2 position, Color color, float size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawConvex2D(IntPtr renderer, Vec2* points, uint pointCount, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawConvexHull2D(IntPtr renderer, Vec2* points, uint pointCount, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawPolygonShape2D(IntPtr renderer, Vec2* points, uint pointCount, Color color);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawLine3D(IntPtr renderer, in Vec3 from, in Vec3 to, Color color, float thickness);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawBox3D(IntPtr renderer, in Vec3 center, in Vec3 size, in Mat44 rotation, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawSphere3D(IntPtr renderer, in Vec3 center, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCylinder3D(IntPtr renderer, in Vec3 @base, in Vec3 top, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawCone3D(IntPtr renderer, in Vec3 @base, in Vec3 tip, float radius, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawTriangle3D(IntPtr renderer, in Vec3 p0, in Vec3 p1, in Vec3 p2, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawArrow3D(IntPtr renderer, in Vec3 from, in Vec3 to, Color color, float headSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawMarker3D(IntPtr renderer, in Vec3 position, Color color, float size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawPlane3D(IntPtr renderer, in Vec3 center, in Vec3 normal, float size, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawConvex3D(IntPtr renderer, Vec3* points, uint pointCount, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawConvexHull3D(IntPtr renderer, Vec3* points, uint pointCount, Color color);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_RendererDrawPolygonShape3D(IntPtr renderer, Vec3* points, uint pointCount, Color color);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetDevice(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetCommandBuffer(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetDescriptorPool(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetDescriptorSetLayoutTexture(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetTextureSamplerRepeat(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetTextureSamplerShadow(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetRenderPassShadow(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetRenderPass(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanGetPipelineLayout(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanStartTempCommandBuffer(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanEndTempCommandBuffer(IntPtr renderer, IntPtr commandBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanAllocateMemory(IntPtr renderer, ulong size, uint memoryTypeBits, uint properties, out ulong memory);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanFreeMemory(IntPtr renderer, ulong memory, ulong size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanCreateBuffer(IntPtr renderer, ulong size, uint usage, uint properties, IntPtr outBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanCopyBuffer(IntPtr renderer, ulong src, ulong dst, ulong size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanCreateDeviceLocalBuffer(IntPtr renderer, IntPtr data, ulong size, uint usage, IntPtr outBuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanFreeBuffer(IntPtr renderer, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanCreateConstantBuffer(IntPtr renderer, ulong bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanCreateImage(IntPtr renderer, uint width, uint height, int format, int tiling, uint usage, uint properties, out ulong image, out ulong memory);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_VulkanDestroyImage(IntPtr renderer, ulong image, ulong memory);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_VulkanCreateImageView(IntPtr renderer, ulong image, int format, uint aspectFlags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_VulkanFindDepthFormat(IntPtr renderer);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetDevice(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetRootSignature(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetCommandList(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetUploadQueue(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetDSVHeap(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_DX12GetSRVHeap(IntPtr renderer);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MetalGetView(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MetalGetDevice(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MetalGetRenderEncoder(IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Vulkan_EncodeSPIRV([MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Vulkan_DecodeSPIRV([MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_DX_CompileHLSL([MarshalAs(UnmanagedType.LPUTF8Str)] string inputPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string outputPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string entryPoint, [MarshalAs(UnmanagedType.LPUTF8Str)] string target);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DX_EnableDebugLayer([MarshalAs(UnmanagedType.I1)] bool enable);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_DX_GetShaderModel();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_Metal_CompileMSL([MarshalAs(UnmanagedType.LPUTF8Str)] string inputPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string outputPath);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_Metal_SupportsFamily(uint family);
    }
}
