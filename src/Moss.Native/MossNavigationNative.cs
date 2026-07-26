using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Navigation
{
    internal readonly record struct MossNavPolyRef(uint Value);
    internal readonly record struct MossNavObstacleRef(uint Value);
    internal readonly record struct MossNavCompressedTileRef(uint Value);

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavVec2
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavVec3
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavMeshDesc
    {
        public MossNavVec3 Origin;
        public float TileWidth;
        public float TileHeight;
        public int MaxTiles;
        public int MaxPolys;
        public int MaxQueryNodes;
        public MossNavVec3 QueryHalfExtents;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavBakeDesc
    {
        public IntPtr Vertices;
        public int VertexCount;
        public int VertexStrideBytes;
        public IntPtr Indices;
        public int IndexCount;
        public float CellSize;
        public float CellHeight;
        public float AgentHeight;
        public float AgentRadius;
        public float AgentMaxClimb;
        public float AgentMaxSlope;
        public float RegionMinSize;
        public float RegionMergeSize;
        public float EdgeMaxLen;
        public float EdgeMaxError;
        public int VertsPerPoly;
        public float DetailSampleDist;
        public float DetailSampleMaxError;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavBake2DDesc
    {
        public IntPtr Vertices;
        public int VertexCount;
        public int VertexStrideBytes;
        public IntPtr Indices;
        public int IndexCount;
        public float PlaneHeight;
        public float CellSize;
        public float CellHeight;
        public float AgentHeight;
        public float AgentRadius;
        public float AgentMaxClimb;
        public float AgentMaxSlope;
        public float RegionMinSize;
        public float RegionMergeSize;
        public float EdgeMaxLen;
        public float EdgeMaxError;
        public int VertsPerPoly;
        public float DetailSampleDist;
        public float DetailSampleMaxError;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavTileCacheDesc
    {
        public MossNavVec3 Origin;
        public float CellSize;
        public float CellHeight;
        public int Width;
        public int Height;
        public float WalkableHeight;
        public float WalkableRadius;
        public float WalkableClimb;
        public float MaxSimplificationError;
        public int MaxTiles;
        public int MaxObstacles;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavObstacleDesc
    {
        public MossNavVec3 Position;
        public float Radius;
        public float Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavCrowdDesc
    {
        public int MaxAgents;
        public float MaxAgentRadius;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNavAgentDesc
    {
        public MossNavVec3 Position;
        public float Radius;
        public float Height;
        public float MaxAcceleration;
        public float MaxSpeed;
        public float CollisionQueryRange;
        public float PathOptimizationRange;
        public float SeparationWeight;
        public byte UpdateFlags;
        public byte ObstacleAvoidanceType;
        public byte QueryFilterType;
        public IntPtr UserData;
    }

    internal static class MossNavigationNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavMeshCreate(in MossNavMeshDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavMeshDestroy(IntPtr mesh);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshBakeFromTriangles(IntPtr mesh, in MossNavBakeDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshBakeFromTriangles2D(IntPtr mesh, in MossNavBake2DDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshDeserialize(IntPtr mesh, IntPtr data, int dataSize, [MarshalAs(UnmanagedType.I1)] bool copyData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshGetTileData(IntPtr mesh, int tileIndex, out IntPtr outData, out int outDataSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavMeshSerializeTileState(IntPtr mesh, int tileIndex, IntPtr outData, int outCapacity);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshDeserializeTileState(IntPtr mesh, int tileIndex, IntPtr data, int dataSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshQueryNearestPoint(IntPtr mesh, in MossNavVec3 point, in MossNavVec3 halfExtents, out MossNavVec3 outPoint, out MossNavPolyRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavMeshFindPath(IntPtr mesh, in MossNavVec3 start, in MossNavVec3 end, IntPtr outPoints, int maxPoints);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshQueryNearestPoint2D(IntPtr mesh, in MossNavVec2 point, in MossNavVec2 halfExtents, float queryHeight, float planeHeight, out MossNavVec2 outPoint, out MossNavPolyRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavMeshFindPath2D(IntPtr mesh, in MossNavVec2 start, in MossNavVec2 end, float planeHeight, IntPtr outPoints, int maxPoints);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshInitTileCache(IntPtr mesh, in MossNavTileCacheDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddTileCacheData(IntPtr mesh, IntPtr data, int dataSize, [MarshalAs(UnmanagedType.I1)] bool copyData, out MossNavCompressedTileRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshGetTileCacheData(IntPtr mesh, MossNavCompressedTileRef tileRef, out IntPtr outData, out int outDataSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshRemoveTileCacheData(IntPtr mesh, MossNavCompressedTileRef tileRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddObstacle(IntPtr mesh, in MossNavObstacleDesc obstacle, out MossNavObstacleRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshAddObstacle2D(IntPtr mesh, in MossNavVec2 position, float radius, float height, float planeHeight, out MossNavObstacleRef outRef);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshRemoveObstacle(IntPtr mesh, MossNavObstacleRef obstacle);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavMeshUpdateObstacles(IntPtr mesh, float dt, [MarshalAs(UnmanagedType.I1)] out bool outUpToDate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavMeshDebugDraw(IntPtr mesh, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavMeshGetLastError(IntPtr mesh);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavCrowdCreate(IntPtr mesh, in MossNavCrowdDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdDestroy(IntPtr crowd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavCrowdAddAgent(IntPtr crowd, in MossNavAgentDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NavCrowdAddAgent2D(IntPtr crowd, in MossNavAgentDesc desc, in MossNavVec2 position, float planeHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdRemoveAgent(IntPtr crowd, int agentIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdRequestMoveTarget(IntPtr crowd, int agentIndex, in MossNavVec3 target);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdRequestMoveTarget2D(IntPtr crowd, int agentIndex, in MossNavVec2 target, float planeHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdGetAgentPosition(IntPtr crowd, int agentIndex, out MossNavVec3 outPosition);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NavCrowdGetAgentPosition2D(IntPtr crowd, int agentIndex, out MossNavVec2 outPosition);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NavCrowdUpdate(IntPtr crowd, float dt);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NavCrowdGetLastError(IntPtr crowd);
    }
}
