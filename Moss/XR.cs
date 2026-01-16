using System;
using System.Runtime.InteropServices;


using MossXR_Context      = IntPtr;
using MossXR_Session      = IntPtr;
using MossXR_Swapchain    = IntPtr;
using MossXR_Space        = IntPtr;
using MossXR_Action       = IntPtr;
using MossXR_ActionSet    = IntPtr;
using MossXR_Layer        = IntPtr;
using MossXR_Anchor       = IntPtr;
using MossXR_Origin       = IntPtr;
using MossXR_HandModifier = IntPtr;
using MossXR_BodyModifier = IntPtr;
using MossXR_FaceModifier = IntPtr;
using MossXR_Time = Int64;


public enum MossXR_SessionState : byte{
            Unknown,
            Ready,
            Synchronized,
            Visible,
            Focused,
            Stopping,
            Exiting,
            LossPending
}

public enum MossXR_ActionType : byte {
            Boolean,
            Float,
            Vec2,
            Pose,
            Haptic
}

public enum MossXR_ViewType : byte {
            Mono,
            Stereo
}

public enum MossXR_Handedness : byte {
            Left,
            Right
}

public enum Moss_XRImageLayout : int {
            COLOR_ATTACHMENT,
            SHADER_READ
}

public enum Moss_XREventType : int {
            SESSION_STATE_CHANGED,
            USER_PRESENCE_CHANGED,
            REFERENCE_SPACE_CHANGED,
            INSTANCE_LOSS_PENDING,
            INTERACTION_PROFILE_CHANGED,
            VISIBILITY_CHANGED
}

public enum XRBodyJoint : int {
            XR_BODY_ROOT,
            XR_BODY_SPINE,
            XR_BODY_CHEST,
            XR_BODY_NECK,
            XR_BODY_HEAD,
            XR_BODY_LEFT_SHOULDER,
            XR_BODY_LEFT_ELBOW,
            XR_BODY_LEFT_HAND,
            XR_BODY_RIGHT_SHOULDER,
            XR_BODY_RIGHT_ELBOW,
            XR_BODY_RIGHT_HAND,
            XR_BODY_LEFT_HIP,
            XR_BODY_LEFT_KNEE,
            XR_BODY_LEFT_FOOT,
            XR_BODY_RIGHT_HIP,
            XR_BODY_RIGHT_KNEE,
            XR_BODY_RIGHT_FOOT,
            XR_BODY_JOINT_COUNT
}

public enum XRHandJoint : int {
            XR_HAND_WRIST,
            XR_HAND_THUMB_METACARPAL,
            XR_HAND_THUMB_PROXIMAL,
            XR_HAND_THUMB_DISTAL,
            XR_HAND_THUMB_TIP,
            XR_HAND_INDEX_METACARPAL,
            XR_HAND_INDEX_PROXIMAL,
            XR_HAND_INDEX_INTERMEDIATE,
            XR_HAND_INDEX_DISTAL,
            XR_HAND_INDEX_TIP,
            XR_HAND_MIDDLE_METACARPAL,
            XR_HAND_MIDDLE_PROXIMAL,
            XR_HAND_MIDDLE_INTERMEDIATE,
            XR_HAND_MIDDLE_DISTAL,
            XR_HAND_MIDDLE_TIP,
            XR_HAND_RING_METACARPAL,
            XR_HAND_RING_PROXIMAL,
            XR_HAND_RING_INTERMEDIATE,
            XR_HAND_RING_DISTAL,
            XR_HAND_RING_TIP,
            XR_HAND_PINKY_METACARPAL,
            XR_HAND_PINKY_PROXIMAL,
            XR_HAND_PINKY_INTERMEDIATE,
            XR_HAND_PINKY_DISTAL,
            XR_HAND_PINKY_TIP,
            XR_HAND_JOINT_COUNT
}


[StructLayout(LayoutKind.Sequential)]
public struct MossXR_Pose {
            public Vec3 position;
            public Quat orientation;
}

[StructLayout(LayoutKind.Sequential)]
public struct MossXR_Fov {
            public float left;
            public float right;
            public float up;
            public float down;
        }

[StructLayout(LayoutKind.Sequential)]
public struct MossXR_View {
            public MossXR_Pose pose;
            public MossXR_Fov fov;
            public Mat44 view;
            public Mat44 projection;
}

[StructLayout(LayoutKind.Sequential)]
public struct MossXR_Capabilities {
            public uint viewCount;
            public MossXR_ViewType viewType;

            public byte handTracking;
            public byte eyeTracking;
            public byte bodyTracking;
            public byte faceTracking;

            public byte passthrough;
            public byte anchors;
            public byte depthLayers;
}

[StructLayout(LayoutKind.Sequential)]
public struct MossXR_InitInfo {
            public int renderer;
            public IntPtr graphicsDevice;
            public IntPtr graphicsContext;
}

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_Initialize(ref MossXR_InitInfo info);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_Shutdown();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Session Moss_XR_CreateSession();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroySession(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_SessionState Moss_XR_GetSessionState(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_BeginFrame(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_EndFrame(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Time Moss_XR_GetPredictedDisplayTime(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern float Moss_XR_GetDeltaSeconds();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern uint Moss_XR_GetViewCount(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetView(MossXR_Session session, uint index, ref MossXR_View outView);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Swapchain Moss_XR_CreateSwapchain(MossXR_Session session, uint width, uint height, int format);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroySwapchain(MossXR_Swapchain swapchain);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern IntPtr Moss_XR_AcquireSwapchainImage(MossXR_Swapchain swapchain, out uint imageIndex);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_ReleaseSwapchainImage(MossXR_Swapchain swapchain, uint imageIndex);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_ActionSet Moss_XR_CreateActionSet(string name);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyActionSet(MossXR_ActionSet set);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Action Moss_XR_CreateAction(MossXR_ActionSet set, string name, MossXR_ActionType type);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyAction(MossXR_Action action);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_AttachActionSet(MossXR_Session session, MossXR_ActionSet set);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_SyncActions(MossXR_Session session);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetActionBoolean(MossXR_Action action);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern float Moss_XR_GetActionFloat(MossXR_Action action);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetActionPose(MossXR_Action action, ref MossXR_Pose outPose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_PlayHaptic(MossXR_Action action, float amplitude, float durationSeconds);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_StopHaptic(MossXR_Action action);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_HandModifier Moss_XR_CreateHand();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetHandJointPose(MossXR_HandModifier hand, XRHandJoint joint, ref MossXR_Pose outPose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetHandPose(MossXR_HandModifier hand, ref MossXR_Pose outPose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyHand(MossXR_HandModifier hand);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_BodyModifier Moss_XR_CreateBody();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_GetBodyJointPose(MossXR_BodyModifier body, XRBodyJoint joint, ref MossXR_Pose outPose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyBody(MossXR_BodyModifier body);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_FaceModifier Moss_XR_CreateFace();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyFace(MossXR_FaceModifier face);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Anchor Moss_XR_CreateAnchor(ref MossXR_Pose pose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyAnchor(MossXR_Anchor anchor);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_LocateAnchor(MossXR_Anchor anchor, MossXR_Time time, ref MossXR_Pose outPose);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Layer Moss_XR_CreateProjectionLayer(MossXR_Swapchain swapchain);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern MossXR_Layer Moss_XR_CreateQuadLayer(MossXR_Swapchain swapchain, ref MossXR_Pose pose, ref Vec2 size);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_DestroyLayer(MossXR_Layer layer);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_SubmitLayers(MossXR_Session session, uint layerCount, IntPtr layers);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_EnablePassthrough(byte enable);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern byte Moss_XR_EnableFoveatedRendering(byte enable);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern IntPtr Moss_XR_GetCapabilities();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern string Moss_XR_GetBackendName();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern uint Moss_XR_GetBackendVersion();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_BeginDebugLabel(string label);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_XR_EndDebugLabel();
