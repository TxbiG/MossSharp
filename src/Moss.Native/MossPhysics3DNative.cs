using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Physics3D
{
    [Flags]
    internal enum PhysicsUpdateError : uint
    {
        None = 0,
        ManifoldCacheFull = 1 << 0,
        BodyPairCacheFull = 1 << 1,
        ContactConstraintsFull = 1 << 2
    }

    internal enum ShapeType
    {
        Convex = 0,
        Compound = 1,
        Decorated = 2,
        Mesh = 3,
        HeightField = 4,
        SoftBody = 5,
        User1 = 6,
        User2 = 7,
        User3 = 8,
        User4 = 9
    }

    internal enum ShapeSubType
    {
        Sphere = 0,
        Box = 1,
        Triangle = 2,
        Capsule = 3,
        TaperedCapsule = 4,
        Cylinder = 5,
        ConvexHull = 6,
        StaticCompound = 7,
        MutableCompound = 8,
        RotatedTranslated = 9,
        Scaled = 10,
        OffsetCenterOfMass = 11,
        Mesh = 12,
        HeightField = 13,
        SoftBody = 14
    }

    internal enum ActiveEdgeMode : byte
    {
        CollideOnlyWithActive,
        CollideWithAll
    }

    internal enum CollectFacesMode : byte
    {
        CollectFaces,
        NoFaces
    }

    internal enum BackFaceMode : byte
    {
        IgnoreBackFaces,
        CollideWithBackFaces
    }

    internal enum BodyManagerShapeColor
    {
        InstanceColor,
        ShapeTypeColor,
        MotionTypeColor,
        SleepColor,
        IslandColor,
        MaterialColor
    }

    internal enum SoftBodyConstraintColor
    {
        ConstraintType,
        ConstraintGroup,
        ConstraintOrder
    }

    [Flags]
    internal enum MossPhysicsDebugDrawFlags : uint
    {
        None = 0,
        Bodies = 1u << 0,
        Sensors = 1u << 1,
        Contacts = 1u << 2,
        Constraints = 1u << 3,
        All = 0xFFFFFFFFu
    }

    internal enum MossPhysicsFeatureSupport : uint
    {
        Unsupported = 0,
        Partial = 1,
        Supported = 2,
        Experimental = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Vec3
    {
        public float X;
        public float Y;
        public float Z;
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
    internal unsafe struct Mat44
    {
        public fixed float M[16];
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PhysicsMaterial
    {
        public float StaticFriction;
        public float DynamicFriction;
        public float Restitution;
        public float Density;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollideSettingsBase
    {
        public ActiveEdgeMode ActiveEdgeMode;
        public CollectFacesMode CollectFacesMode;
        public float CollisionTolerance;
        public float PenetrationTolerance;
        public Vec3 ActiveEdgeMovementDirection;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollideShapeSettings
    {
        public CollideSettingsBase Base;
        public float MaxSeparationDistance;
        public BackFaceMode BackFaceMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShapeCastSettings
    {
        public CollideSettingsBase Base;
        public BackFaceMode BackFaceModeTriangles;
        public BackFaceMode BackFaceModeConvex;
        [MarshalAs(UnmanagedType.I1)] public bool UseShrunkenShapeAndConvexRadius;
        [MarshalAs(UnmanagedType.I1)] public bool ReturnDeepestPoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RayCastSettings
    {
        public BackFaceMode BackFaceModeTriangles;
        public BackFaceMode BackFaceModeConvex;
        [MarshalAs(UnmanagedType.I1)] public bool TreatConvexAsSolid;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SubShapeIDPair
    {
        public uint Body1ID;
        public uint SubShapeID1;
        public uint Body2ID;
        public uint SubShapeID2;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BroadPhaseCastResult
    {
        public uint BodyID;
        public float Fraction;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RayCastResult
    {
        public uint BodyID;
        public float Fraction;
        public uint SubShapeID2;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollidePointResult
    {
        public uint BodyID;
        public uint SubShapeID2;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollideShapeResult
    {
        public Vec3 ContactPointOn1;
        public Vec3 ContactPointOn2;
        public Vec3 PenetrationAxis;
        public float PenetrationDepth;
        public uint SubShapeID1;
        public uint SubShapeID2;
        public uint BodyID2;
        public uint Shape1FaceCount;
        public IntPtr Shape1Faces;
        public uint Shape2FaceCount;
        public IntPtr Shape2Faces;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShapeCastResult
    {
        public Vec3 ContactPointOn1;
        public Vec3 ContactPointOn2;
        public Vec3 PenetrationAxis;
        public float PenetrationDepth;
        public uint SubShapeID1;
        public uint SubShapeID2;
        public uint BodyID2;
        public float Fraction;
        [MarshalAs(UnmanagedType.I1)] public bool IsBackFaceHit;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DrawSettings
    {
        [MarshalAs(UnmanagedType.I1)] public bool DrawGetSupportFunction;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSupportDirection;
        [MarshalAs(UnmanagedType.I1)] public bool DrawGetSupportingFace;
        [MarshalAs(UnmanagedType.I1)] public bool DrawShape;
        [MarshalAs(UnmanagedType.I1)] public bool DrawShapeWireframe;
        public BodyManagerShapeColor DrawShapeColor;
        [MarshalAs(UnmanagedType.I1)] public bool DrawBoundingBox;
        [MarshalAs(UnmanagedType.I1)] public bool DrawCenterOfMassTransform;
        [MarshalAs(UnmanagedType.I1)] public bool DrawWorldTransform;
        [MarshalAs(UnmanagedType.I1)] public bool DrawVelocity;
        [MarshalAs(UnmanagedType.I1)] public bool DrawMassAndInertia;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSleepStats;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyVertices;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyVertexVelocities;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyEdgeConstraints;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyBendConstraints;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyVolumeConstraints;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodySkinConstraints;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyLRAConstraints;
        [MarshalAs(UnmanagedType.I1)] public bool DrawSoftBodyPredictedBounds;
        public SoftBodyConstraintColor DrawSoftBodyConstraintColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollisionGroup
    {
        public IntPtr GroupFilter;
        public uint GroupID;
        public uint SubGroupID;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollisionEstimationResultImpulse
    {
        public float ContactImpulse;
        public float FrictionImpulse1;
        public float FrictionImpulse2;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollisionEstimationResult
    {
        public Vec3 LinearVelocity1;
        public Vec3 AngularVelocity1;
        public Vec3 LinearVelocity2;
        public Vec3 AngularVelocity2;
        public Vec3 Tangent1;
        public Vec3 Tangent2;
        public uint ImpulseCount;
        public IntPtr Impulses;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct JobSystemThreadPoolConfig
    {
        public uint MaxJobs;
        public uint MaxBarriers;
        public int NumThreads;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PhysicsStepListenerContext
    {
        public float DeltaTime;
        [MarshalAs(UnmanagedType.I1)] public bool IsFirstStep;
        [MarshalAs(UnmanagedType.I1)] public bool IsLastStep;
        public IntPtr PhysicsSystem;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SkeletonJoint
    {
        public IntPtr Name;
        public IntPtr ParentName;
        public int ParentJointIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsLimitations
    {
        public MossPhysicsFeatureSupport SoftBodyVsSoftBody;
        public MossPhysicsFeatureSupport RagdollConstraintPresets;
        public MossPhysicsFeatureSupport CollisionMassInertiaPrecision;
        public MossPhysicsFeatureSupport SensorUpdateCallbacks;
        public IntPtr Notes;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsDebugBodyDesc
    {
        public Vec3 Center;
        public Vec3 Size;
        public Color Color;
        [MarshalAs(UnmanagedType.I1)] public bool IsSensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsDebugContactDesc
    {
        public Vec3 Position;
        public Vec3 Normal;
        public float Impulse;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsShapeSerializeDesc
    {
        public IntPtr Type;
        public Vec3 Center;
        public Vec3 Size;
        public float Radius;
        public float Height;
        public float Mass;
        [MarshalAs(UnmanagedType.I1)] public bool Sensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsSceneSerializeDesc
    {
        public IntPtr Shapes;
        public uint ShapeCount;
        public IntPtr Contacts;
        public uint ContactCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsSerializedSceneInfo
    {
        public uint ShapeCount;
        public uint ContactCount;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void CastRayResultCallback(IntPtr context, in RayCastResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void RayCastBodyResultCallback(IntPtr context, in BroadPhaseCastResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void CollideShapeBodyResultCallback(IntPtr context, uint bodyId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void CollidePointResultCallback(IntPtr context, in CollidePointResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void CollideShapeResultCallback(IntPtr context, in CollideShapeResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void CastShapeResultCallback(IntPtr context, in ShapeCastResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float CastRayCollectorCallback(IntPtr context, in RayCastResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float RayCastBodyCollectorCallback(IntPtr context, in BroadPhaseCastResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float CollideShapeBodyCollectorCallback(IntPtr context, uint bodyId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float CollidePointCollectorCallback(IntPtr context, in CollidePointResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float CollideShapeCollectorCallback(IntPtr context, in CollideShapeResult result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate float CastShapeCollectorCallback(IntPtr context, in ShapeCastResult result);

    internal static unsafe class MossPhysics3DNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void CollideShapeResult_FreeMembers(ref CollideShapeResult result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void CollisionEstimationResult_FreeMembers(ref CollisionEstimationResult result);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr BroadPhaseLayerInterfaceMask_Create(uint numBroadPhaseLayers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BroadPhaseLayerInterfaceMask_ConfigureLayer(IntPtr bpInterface, byte broadPhaseLayer, uint groupsToInclude, uint groupsToExclude);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr BroadPhaseLayerInterfaceTable_Create(uint numObjectLayers, uint numBroadPhaseLayers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BroadPhaseLayerInterfaceTable_MapObjectToBroadPhaseLayer(IntPtr bpInterface, uint objectLayer, byte broadPhaseLayer);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr ObjectLayerPairFilterMask_Create();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ObjectLayerPairFilterMask_GetObjectLayer(uint group, uint mask);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ObjectLayerPairFilterMask_GetGroup(uint layer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ObjectLayerPairFilterMask_GetMask(uint layer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr ObjectLayerPairFilterTable_Create(uint numObjectLayers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ObjectLayerPairFilterTable_DisableCollision(IntPtr objectFilter, uint layer1, uint layer2);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ObjectLayerPairFilterTable_EnableCollision(IntPtr objectFilter, uint layer1, uint layer2);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool ObjectLayerPairFilterTable_ShouldCollide(IntPtr objectFilter, uint layer1, uint layer2);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr ObjectVsBroadPhaseLayerFilterMask_Create(IntPtr broadPhaseLayerInterface);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr ObjectVsBroadPhaseLayerFilterTable_Create(IntPtr broadPhaseLayerInterface, uint numBroadPhaseLayers, IntPtr objectLayerPairFilter, uint numObjectLayers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void DrawSettings_InitDefault(out DrawSettings settings);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ContactManifold_GetWorldSpaceNormal(IntPtr manifold, out Vec3 result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float ContactManifold_GetPenetrationDepth(IntPtr manifold);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ContactManifold_GetSubShapeID1(IntPtr manifold);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ContactManifold_GetSubShapeID2(IntPtr manifold);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint ContactManifold_GetPointCount(IntPtr manifold);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ContactManifold_GetWorldSpaceContactPointOn1(IntPtr manifold, uint index, out Vec3 result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ContactManifold_GetWorldSpaceContactPointOn2(IntPtr manifold, uint index, out Vec3 result);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool CollisionDispatch_CollideShapeVsShape(IntPtr shape1, IntPtr shape2, in Vec3 scale1, in Vec3 scale2, in Mat44 centerOfMassTransform1, in Mat44 centerOfMassTransform2, in CollideShapeSettings collideShapeSettings, CollideShapeCollectorCallback callback, IntPtr userData, IntPtr shapeFilter);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool CollisionDispatch_CastShapeVsShapeLocalSpace(in Vec3 direction, IntPtr shape1, IntPtr shape2, in Vec3 scale1InShape2LocalSpace, in Vec3 scale2, ref Mat44 centerOfMassTransform1InShape2LocalSpace, ref Mat44 centerOfMassWorldTransform2, in ShapeCastSettings shapeCastSettings, CastShapeCollectorCallback callback, IntPtr userData, IntPtr shapeFilter);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool CollisionDispatch_CastShapeVsShapeWorldSpace(in Vec3 direction, IntPtr shape1, IntPtr shape2, in Vec3 scale1, in Vec3 scale2, in Mat44 centerOfMassWorldTransform1, in Mat44 centerOfMassWorldTransform2, in ShapeCastSettings shapeCastSettings, CastShapeCollectorCallback callback, IntPtr userData, IntPtr shapeFilter);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Skeleton_Create();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Skeleton_Destroy(IntPtr skeleton);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Skeleton_AddJoint(IntPtr skeleton, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Skeleton_AddJoint2(IntPtr skeleton, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int parentIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Skeleton_AddJoint3(IntPtr skeleton, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string parentName);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Skeleton_GetJointCount(IntPtr skeleton);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Skeleton_GetJoint(IntPtr skeleton, int index, out SkeletonJoint joint);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Skeleton_GetJointIndex(IntPtr skeleton, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Skeleton_CalculateParentJointIndices(IntPtr skeleton);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Skeleton_AreJointsCorrectlyOrdered(IntPtr skeleton);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PhysicsGetLimitations(out MossPhysicsLimitations limitations);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_PhysicsDebugDrawBodies(IntPtr list, MossPhysicsDebugBodyDesc* bodies, uint bodyCount, MossPhysicsDebugDrawFlags flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_PhysicsDebugDrawContacts(IntPtr list, MossPhysicsDebugContactDesc* contacts, uint contactCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_PhysicsShapeSerialize(in MossPhysicsShapeSerializeDesc shape, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_PhysicsSceneSerialize(in MossPhysicsSceneSerializeDesc scene, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_PhysicsSceneDeserializeInfo([MarshalAs(UnmanagedType.LPUTF8Str)] string serializedScene, out MossPhysicsSerializedSceneInfo info);
    }
}
