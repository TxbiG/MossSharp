using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.XR
{
    internal static class MossXRNative
    {
        internal enum MossXRSessionState : byte { Unknown, Ready, Synchronized, Visible, Focused, Stopping, Exiting, LossPending }
        internal enum MossXRActionType : byte { Boolean, Float, Vec2, Pose, Haptic }
        internal enum MossXRHandedness : byte { Left, Right }
        internal enum MossXRViewType : byte { Mono, Stereo }
        internal enum RendererBackend : byte { OpenGL, OpenGLES, Vulkan, DirectX12, Metal }
        internal enum XRBodyJoint : int { Root, Spine, Chest, Neck, Head, LeftShoulder, LeftElbow, LeftHand, RightShoulder, RightElbow, RightHand, LeftHip, LeftKnee, LeftFoot, RightHip, RightKnee, RightFoot, Count }
        internal enum XRHandJoint : int { Wrist, ThumbMetacarpal, ThumbProximal, ThumbDistal, ThumbTip, IndexMetacarpal, IndexProximal, IndexIntermediate, IndexDistal, IndexTip, MiddleMetacarpal, MiddleProximal, MiddleIntermediate, MiddleDistal, MiddleTip, RingMetacarpal, RingProximal, RingIntermediate, RingDistal, RingTip, PinkyMetacarpal, PinkyProximal, PinkyIntermediate, PinkyDistal, PinkyTip, Count }

        [StructLayout(LayoutKind.Sequential)] internal struct Vec2 { public float X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct Vec3 { public float X, Y, Z; }
        [StructLayout(LayoutKind.Sequential)] internal struct Quat { public float X, Y, Z, W; }
        [StructLayout(LayoutKind.Sequential)] internal unsafe struct Mat44 { public fixed float Values[16]; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRPose { public Vec3 Position; public Quat Orientation; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRFov { public float Left, Right, Up, Down; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRView { public MossXRPose Pose; public MossXRFov Fov; public Mat44 View; public Mat44 Projection; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRCapabilities { public uint ViewCount; public MossXRViewType ViewType; [MarshalAs(UnmanagedType.I1)] public bool HandTracking, EyeTracking, BodyTracking, FaceTracking, Passthrough, Anchors, DepthLayers; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRActionBindingDesc { public IntPtr Action; public IntPtr Path; }
        [StructLayout(LayoutKind.Sequential)] internal struct MossXRInitInfo { public RendererBackend Renderer; public IntPtr GraphicsDevice, GraphicsContext, GraphicsBinding; }

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_Initialize(in MossXRInitInfo info);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_Shutdown();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_GetCapabilities();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_EnableValidationLayers();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateSession();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroySession(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossXRSessionState Moss_XR_GetSessionState(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_BeginFrame(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_EndFrame(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern long Moss_XR_GetPredictedDisplayTime(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_XR_GetDeltaSeconds();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_HandleEvents(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_XR_GetViewCount(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetView(IntPtr session, uint index, out MossXRView view);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateSwapchain(IntPtr session, uint width, uint height, int format);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroySwapchain(IntPtr swapchain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_AcquireSwapchainImage(IntPtr swapchain, out uint imageIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_ReleaseSwapchainImage(IntPtr swapchain, uint imageIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_XR_GetSwapchainImageCount(IntPtr swapchain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_GetSwapchainNativeImage(IntPtr swapchain, uint imageIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_XR_GetSwapchainWidth(IntPtr swapchain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_XR_GetSwapchainHeight(IntPtr swapchain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateAnchor(in MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyAnchor(IntPtr anchor);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_LocateAnchor(IntPtr anchor, long time, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateProjectionLayer(IntPtr swapchain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateQuadLayer(IntPtr swapchain, in MossXRPose pose, Vec2 size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyLayer(IntPtr layer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_SubmitLayers(IntPtr session, uint layerCount, IntPtr layers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateActionSet([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyActionSet(IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateAction(IntPtr set, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, MossXRActionType type);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyAction(IntPtr action);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_AttachActionSet(IntPtr session, IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_SuggestActionBindings([MarshalAs(UnmanagedType.LPUTF8Str)] string interactionProfilePath, IntPtr bindings, uint bindingCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_SuggestSimpleControllerBindings(IntPtr set);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ulong Moss_XR_GetHandPath(MossXRHandedness hand);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_SyncActions(IntPtr session);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetActionBoolean(IntPtr action);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetActionBooleanForHand(IntPtr action, MossXRHandedness hand);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_XR_GetActionFloat(IntPtr action);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_XR_GetActionFloatForHand(IntPtr action, MossXRHandedness hand);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetActionPose(IntPtr action, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetActionPoseForHand(IntPtr action, MossXRHandedness hand, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_PlayHaptic(IntPtr action, ulong hand, float amplitude, float durationSeconds);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_PlayHapticForHand(IntPtr action, MossXRHandedness hand, float amplitude, float durationSeconds);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_StopHaptic(IntPtr action);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_StopHapticForHand(IntPtr action, MossXRHandedness hand);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateHand();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetHandJointPose(IntPtr hand, XRHandJoint joint, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyHand(IntPtr hand);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetHandPose(IntPtr hand, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateBody();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_GetBodyJointPose(IntPtr body, XRBodyJoint joint, out MossXRPose pose);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyBody(IntPtr body);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_CreateFace();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_DestroyFace(IntPtr face);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_EnablePassthrough([MarshalAs(UnmanagedType.I1)] bool enable);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_XR_EnableFoveatedRendering([MarshalAs(UnmanagedType.I1)] bool enable);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_XR_GetBackendName();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_XR_GetBackendVersion();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_BeginDebugLabel([MarshalAs(UnmanagedType.LPUTF8Str)] string label);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_XR_EndDebugLabel();
    }
}
