using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Core
{
    internal static class MossCoreNative
    {
        [StructLayout(LayoutKind.Sequential)] internal struct Color { public float R, G, B, A; }
        [StructLayout(LayoutKind.Sequential)] internal struct Float2 { public float X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct Float3 { public float X, Y, Z; }
        [StructLayout(LayoutKind.Sequential)] internal struct NavVec2 { public float X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct NavVec3 { public float X, Y, Z; }
        [StructLayout(LayoutKind.Sequential)] internal struct NavPolyRef { public ulong Value; }
        [StructLayout(LayoutKind.Sequential)] internal struct NavCompressedTileRef { public ulong Value; }
        [StructLayout(LayoutKind.Sequential)] internal struct NavObstacleRef { public ulong Value; }

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUDevice(IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUDeviceWithProperties(IntPtr desc, IntPtr requestedProperties);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_DestroyGPUDevice(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AcquireGPUCommandBuffer(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CancelGPUCommandBuffer(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPURenderPass(IntPtr cmd, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPUComputePass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_BeginGPUCopyPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUCommandBufferEnd(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUCommandBufferSubmit(IntPtr device, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUCommandBufferBegin(IntPtr device, int queue);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBeginRenderPass(IntPtr cmd, IntPtr framebuffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdEndRenderPass(IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindPipeline(IntPtr cmd, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindResourceSet(IntPtr cmd, uint setIndex, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindVertexBuffer(IntPtr cmd, IntPtr buffer, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdBindIndexBuffer(IntPtr cmd, IntPtr buffer, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDraw(IntPtr cmd, uint vertexCount, uint firstVertex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDrawIndexed(IntPtr cmd, uint indexCount, uint firstIndex, int vertexOffset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_CmdDispatch(IntPtr cmd, uint x, uint y, uint z);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUClear(IntPtr cmd, float r, float g, float b, float a);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUBuffer(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUBufferCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferUpload(IntPtr device, IntPtr buffer, IntPtr data, ulong size, ulong offset);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUBufferMap(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferUnmap(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUBufferDestroy(IntPtr device, IntPtr buffer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUSampler(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GPUSamplerCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUSamplerDestroy(IntPtr device, IntPtr sampler);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_TextureCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureDestroy(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureUpload(IntPtr device, IntPtr texture, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureGenerateMips(IntPtr device, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTexture(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTextureView(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUTransferBuffer(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUFramebuffer(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUResourceSetLayout(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUResourceSet(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUShader(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_CreateGPUComputePipeline(IntPtr device, IntPtr createInfo);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ShaderCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ShaderDestroy(IntPtr device, IntPtr shader);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_PipelineCreate(IntPtr device, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PipelineDestroy(IntPtr device, IntPtr pipeline);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AcquireGPUSwapchainTexture(IntPtr device, IntPtr window, out IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_AcquireGPUSwapchainTextureStatus(IntPtr device, IntPtr window, out IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetGPUSwapchainTextureFormat(IntPtr device, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_SetGPUSwapchainParameters(IntPtr device, IntPtr window, uint width, uint height, int presentMode);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_ResizeGPUSwapchain(IntPtr device, IntPtr window, uint width, uint height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WaitForGPUSwapchain(IntPtr device, IntPtr window, ulong timeoutNs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_WindowSupportsGPUSwapchainComposition(IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_PresentGPUSwapchain(IntPtr device, IntPtr window, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_PresentGPUSwapchainStatus(IntPtr device, IntPtr window, IntPtr texture);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGPUDeviceDriver(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetGPUDeviceProperties(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_ClaimWindowForGPUDevice(IntPtr device, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ReleaseWindowFromGPUDevice(IntPtr device, IntPtr window);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GPUDeviceSupportsAsyncCompute(IntPtr device);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TextureSetResidency(IntPtr texture, int state);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GPUFatalError(int result);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUICreate(IntPtr renderer, IntPtr gpuDevice);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIDestroy(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetCurrent(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetCurrent();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetStyle(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetDisplaySize(IntPtr gui, float width, float height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetInput(IntPtr gui, IntPtr input);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasCreate(IntPtr gui, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIFontAtlasDestroy(IntPtr gui, IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasGetTexture(IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasGetPixels(IntPtr atlas, out uint width, out uint height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetFontAtlas(IntPtr gui, IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetKeyboardFocus(IntPtr gui, ulong id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ulong Moss_GUIGetKeyboardFocus(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUINewFrame(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIEndFrame(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetDrawData(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIRender(IntPtr gui, IntPtr renderer, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern UIntPtr Moss_GUIStyleSerialize(IntPtr style, IntPtr buffer, UIntPtr bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GUIStyleDeserialize(IntPtr style, [MarshalAs(UnmanagedType.LPUTF8Str)] string text);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavMeshCreate(IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavMeshDestroy(IntPtr mesh);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshBakeFromTriangles(IntPtr mesh, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshBakeFromTriangles2D(IntPtr mesh, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshDeserialize(IntPtr mesh, IntPtr data, int dataSize, [MarshalAs(UnmanagedType.I1)] bool copyData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshQueryNearestPoint(IntPtr mesh, in NavVec3 point, in NavVec3 halfExtents, out NavVec3 outPoint, out NavPolyRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavMeshFindPath(IntPtr mesh, in NavVec3 start, in NavVec3 end, [Out] NavVec3[] outPoints, int maxPoints);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshQueryNearestPoint2D(IntPtr mesh, in NavVec2 point, in NavVec2 halfExtents, float queryHeight, float planeHeight, out NavVec2 outPoint, out NavPolyRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavMeshFindPath2D(IntPtr mesh, in NavVec2 start, in NavVec2 end, float planeHeight, [Out] NavVec2[] outPoints, int maxPoints);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshInitTileCache(IntPtr mesh, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddTileCacheData(IntPtr mesh, IntPtr data, int dataSize, [MarshalAs(UnmanagedType.I1)] bool copyData, out NavCompressedTileRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshRemoveTileCacheData(IntPtr mesh, NavCompressedTileRef tileRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddObstacle(IntPtr mesh, IntPtr obstacle, out NavObstacleRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddObstacle2D(IntPtr mesh, in NavVec2 position, float radius, float height, float planeHeight, out NavObstacleRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshRemoveObstacle(IntPtr mesh, NavObstacleRef obstacle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshUpdateObstacles(IntPtr mesh, float dt, [MarshalAs(UnmanagedType.I1)] out bool upToDate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavMeshDebugDraw(IntPtr mesh, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavMeshGetLastError(IntPtr mesh);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavCrowdCreate(IntPtr mesh, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdDestroy(IntPtr crowd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavCrowdAddAgent(IntPtr crowd, IntPtr desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavCrowdAddAgent2D(IntPtr crowd, IntPtr desc, in NavVec2 position, float planeHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdRemoveAgent(IntPtr crowd, int agentIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdRequestMoveTarget(IntPtr crowd, int agentIndex, in NavVec3 target);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdRequestMoveTarget2D(IntPtr crowd, int agentIndex, in NavVec2 target, float planeHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdGetAgentPosition(IntPtr crowd, int agentIndex, out NavVec3 position);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdGetAgentPosition2D(IntPtr crowd, int agentIndex, out NavVec2 position);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdUpdate(IntPtr crowd, float dt);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavCrowdGetLastError(IntPtr crowd);
    }
}

