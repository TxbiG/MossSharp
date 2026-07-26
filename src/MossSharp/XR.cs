using System;
using MossSharp.Native.XR;

namespace Moss.XR
{
    public enum XrRendererBackend : byte { OpenGL, OpenGLES, Vulkan, DirectX12, Metal }
    public enum XrHandedness : byte { Left, Right }
    public enum XrActionType : byte { Boolean, Float, Vec2, Pose, Haptic }

    public readonly record struct XrPose(float X, float Y, float Z, float Qx, float Qy, float Qz, float Qw);

    public readonly record struct XrInitSettings(XrRendererBackend Renderer, IntPtr GraphicsDevice, IntPtr GraphicsContext, IntPtr GraphicsBinding);

    public static class XrSystem
    {
        public static void Initialize(XrInitSettings settings)
        {
            var info = new MossXRInitInfo
            {
                Renderer = (RendererBackend)settings.Renderer,
                GraphicsDevice = settings.GraphicsDevice,
                GraphicsContext = settings.GraphicsContext,
                GraphicsBinding = settings.GraphicsBinding
            };

            if (!MossXRNative.Moss_XR_Initialize(in info))
            {
                throw new InvalidOperationException("Failed to initialize XR subsystem.");
            }
        }

        public static void Shutdown() => MossXRNative.Moss_XR_Shutdown();
        public static string BackendName => System.Runtime.InteropServices.Marshal.PtrToStringUTF8(MossXRNative.Moss_XR_GetBackendName()) ?? string.Empty;
        public static uint BackendVersion => MossXRNative.Moss_XR_GetBackendVersion();
    }

    public sealed class XrSession : IDisposable
    {
        public XrSession(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; private set; }
        public uint ViewCount => MossXRNative.Moss_XR_GetViewCount(Handle);

        public static XrSession Create()
        {
            IntPtr handle = MossXRNative.Moss_XR_CreateSession();
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create XR session.");
            }

            return new XrSession(handle);
        }

        public bool BeginFrame() => MossXRNative.Moss_XR_BeginFrame(Handle);
        public void EndFrame() => MossXRNative.Moss_XR_EndFrame(Handle);
        public void SyncActions() => MossXRNative.Moss_XR_SyncActions(Handle);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossXRNative.Moss_XR_DestroySession(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }

    public sealed class XrActionSet : IDisposable
    {
        public XrActionSet(IntPtr handle) => Handle = handle;
        public IntPtr Handle { get; private set; }

        public static XrActionSet Create(string name) => new(MossXRNative.Moss_XR_CreateActionSet(name));
        public XrAction CreateAction(string name, XrActionType type) => new(MossXRNative.Moss_XR_CreateAction(Handle, name, (MossXRActionType)type));
        public bool SuggestSimpleControllerBindings() => MossXRNative.Moss_XR_SuggestSimpleControllerBindings(Handle);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossXRNative.Moss_XR_DestroyActionSet(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }

    public sealed class XrAction : IDisposable
    {
        public XrAction(IntPtr handle) => Handle = handle;
        public IntPtr Handle { get; private set; }

        public bool GetBoolean() => MossXRNative.Moss_XR_GetActionBoolean(Handle);
        public bool GetBoolean(XrHandedness hand) => MossXRNative.Moss_XR_GetActionBooleanForHand(Handle, (MossXRHandedness)hand);
        public float GetFloat() => MossXRNative.Moss_XR_GetActionFloat(Handle);
        public float GetFloat(XrHandedness hand) => MossXRNative.Moss_XR_GetActionFloatForHand(Handle, (MossXRHandedness)hand);
        public void PlayHaptic(XrHandedness hand, float amplitude, float durationSeconds) => MossXRNative.Moss_XR_PlayHapticForHand(Handle, (MossXRHandedness)hand, amplitude, durationSeconds);
        public void StopHaptic(XrHandedness hand) => MossXRNative.Moss_XR_StopHapticForHand(Handle, (MossXRHandedness)hand);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossXRNative.Moss_XR_DestroyAction(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }
}