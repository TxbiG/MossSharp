using System;
using MossSharp.Native.GPU;

namespace Moss.GPU
{
    public enum GpuBackend : byte { Default, OpenGL, OpenGLES, Vulkan, DirectX12, Metal, Fallback }
    public enum ExternalGpuFeature : uint { Fsr1 = 1u << 0, Fsr2 = 1u << 1, MeshOptimizer = 1u << 2, Smolv = 1u << 3, VulkanMemoryAllocator = 1u << 4 }
    public enum Fsr2QualityMode : uint { Quality = 1, Balanced = 2, Performance = 3, UltraPerformance = 4 }

    public readonly record struct Fsr2RenderResolution(uint Width, uint Height);

    public static class GpuSystem
    {
        public static GpuBackend CompiledBackend => (GpuBackend)MossGPUNative.Moss_GetCompiledGPUBackendType();
        public static uint ExternalFeatureMask => MossGPUNative.Moss_GPUGetExternalFeatureMask();
        public static bool IsExternalFeatureAvailable(ExternalGpuFeature feature) => MossGPUNative.Moss_GPUExternalFeatureAvailable((GPUExternalFeature)feature);
        public static bool VulkanMemoryAllocatorAvailable => MossGPUNative.Moss_GPUVulkanMemoryAllocatorAvailable();
        public static float GetFsr2UpscaleRatio(Fsr2QualityMode quality) => MossGPUNative.Moss_FSR2GetUpscaleRatio((MossSharp.Native.GPU.FSR2QualityMode)quality);

        public static bool TryGetFsr2RenderResolution(uint displayWidth, uint displayHeight, Fsr2QualityMode quality, out Fsr2RenderResolution resolution)
        {
            bool ok = MossGPUNative.Moss_FSR2GetRenderResolution(displayWidth, displayHeight, (MossSharp.Native.GPU.FSR2QualityMode)quality, out var nativeResolution);
            resolution = new Fsr2RenderResolution(nativeResolution.Width, nativeResolution.Height);
            return ok;
        }
    }
}