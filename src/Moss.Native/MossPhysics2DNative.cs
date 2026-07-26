using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Physics2D
{
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

        public static Vec3 From2D(Vec2 value, float z = 0.0f) => new() { X = value.X, Y = value.Y, Z = z };
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
    internal struct MossPhysicsLimitations
    {
        public MossPhysicsFeatureSupport SoftBodyVsSoftBody;
        public MossPhysicsFeatureSupport RagdollConstraintPresets;
        public MossPhysicsFeatureSupport CollisionMassInertiaPrecision;
        public MossPhysicsFeatureSupport SensorUpdateCallbacks;
        public IntPtr Notes;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsDebugBody2DDesc
    {
        public Vec2 Center;
        public Vec2 Size;
        public Color Color;
        [MarshalAs(UnmanagedType.I1)] public bool IsSensor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossPhysicsDebugContact2DDesc
    {
        public Vec2 Position;
        public Vec2 Normal;
        public float Impulse;
        public Color Color;
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
    internal struct MossPhysicsShapeSerialize2DDesc
    {
        public IntPtr Type;
        public Vec2 Center;
        public Vec2 Size;
        public float Radius;
        public float Mass;
        [MarshalAs(UnmanagedType.I1)] public bool Sensor;
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

    internal static unsafe class MossPhysics2DNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_PhysicsGetLimitations(out MossPhysicsLimitations limitations);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_PhysicsDebugDrawBodies(IntPtr list, MossPhysicsDebugBodyDesc* bodies, uint bodyCount, MossPhysicsDebugDrawFlags flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_PhysicsDebugDrawContacts(IntPtr list, MossPhysicsDebugContactDesc* contacts, uint contactCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_PhysicsShapeSerialize(in MossPhysicsShapeSerializeDesc shape, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_PhysicsSceneSerialize(in MossPhysicsSceneSerializeDesc scene, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_PhysicsSceneDeserializeInfo([MarshalAs(UnmanagedType.LPUTF8Str)] string serializedScene, out MossPhysicsSerializedSceneInfo info);

        internal static MossPhysicsDebugBodyDesc To3D(in MossPhysicsDebugBody2DDesc body, float z = 0.0f) => new()
        {
            Center = Vec3.From2D(body.Center, z),
            Size = Vec3.From2D(body.Size, 0.0f),
            Color = body.Color,
            IsSensor = body.IsSensor
        };

        internal static MossPhysicsDebugContactDesc To3D(in MossPhysicsDebugContact2DDesc contact, float z = 0.0f) => new()
        {
            Position = Vec3.From2D(contact.Position, z),
            Normal = Vec3.From2D(contact.Normal, 0.0f),
            Impulse = contact.Impulse,
            Color = contact.Color
        };

        internal static MossPhysicsShapeSerializeDesc To3D(in MossPhysicsShapeSerialize2DDesc shape) => new()
        {
            Type = shape.Type,
            Center = Vec3.From2D(shape.Center),
            Size = Vec3.From2D(shape.Size, 0.0f),
            Radius = shape.Radius,
            Height = 0.0f,
            Mass = shape.Mass,
            Sensor = shape.Sensor
        };
    }
}
