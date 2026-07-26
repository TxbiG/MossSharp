using System;
using System.Runtime.InteropServices;
using MossSharp.Native.Renderer;

namespace Moss.Renderer
{
    public enum Upscaler { None, Fsr1, Fsr2 }
    public enum Fsr2QualityMode : uint { Quality = 1, Balanced = 2, Performance = 3, UltraPerformance = 4 }

    public readonly record struct RendererHandle(IntPtr Value)
    {
        public bool IsNull => Value == IntPtr.Zero;
        public static implicit operator IntPtr(RendererHandle handle) => handle.Value;
    }

    public sealed class RendererContext : IDisposable
    {
        public RendererContext(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Renderer handle cannot be null.", nameof(handle));
            }

            Handle = handle;
        }

        public IntPtr Handle { get; private set; }
        public bool IsDisposed => Handle == IntPtr.Zero;

        public static RendererContext Create(IntPtr window)
        {
            IntPtr handle = MossRendererNative.Moss_CreateRenderer(window);
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create renderer.");
            }

            return new RendererContext(handle);
        }

        public void BeginFrame() => MossRendererNative.Moss_RendererBeginFrame(Handle);
        public void EndFrame() => MossRendererNative.Moss_RendererEndFrame(Handle);
        public IntPtr GpuDevice => MossRendererNative.Moss_RendererGetGPUDevice(Handle);

        public void SetUpscaler(Upscaler upscaler) => MossRendererNative.Moss_RendererSetUpscaler(Handle, (MossUpscaler)upscaler);

        public void SetUpscaler(Upscaler upscaler, Fsr2QualityMode quality, bool sharpening = false, float sharpness = 0.0f)
        {
            var desc = new MossRendererUpscalerDesc
            {
                Upscaler = (MossUpscaler)upscaler,
                Fsr2Quality = (MossFSR2QualityMode)quality,
                EnableSharpening = sharpening,
                Sharpness = sharpness
            };
            MossRendererNative.Moss_RendererSetUpscalerDesc(Handle, in desc);
        }

        public IntPtr LoadFont(string path, float pixelSize) => MossRendererNative.Moss_FontLoad(path, pixelSize);
        public void DrawText(IntPtr font, string text, float x, float y)
        {
            IntPtr nativeText = Marshal.StringToCoTaskMemUTF8(text);
            try
            {
                var desc = new MossDrawTextDesc
                {
                    Font = font,
                    Text = nativeText,
                    Position = new Vec2 { X = x, Y = y },
                    Color = new Color { R = 1, G = 1, B = 1, A = 1 },
                    Scale = 1.0f,
                    Visibility = uint.MaxValue
                };
                MossRendererNative.Moss_RendererDrawText(Handle, in desc);
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativeText);
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossRendererNative.Moss_RendererDestroy(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }
}