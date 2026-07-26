using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Components
{
    internal enum MossComponentKind : uint
    {
        Terrain = 1,
        OpenWorldTerrain = 2,
        ParticleEffect = 3,
        Tilemap = 4,
        Gridmap = 5,
        Landscape = 6,
        OpenSea = 7
    }

    internal enum MossComponentParticleSimulation : uint
    {
        Cpu = 0,
        Gpu = 1
    }

    [StructLayout(LayoutKind.Sequential)] internal struct Float2 { public float X; public float Y; }
    [StructLayout(LayoutKind.Sequential)] internal struct Float3 { public float X; public float Y; public float Z; }
    [StructLayout(LayoutKind.Sequential)] internal struct Color { public float R; public float G; public float B; public float A; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossComponentResourceBinding
    {
        public IntPtr PrimaryTexture;
        public IntPtr SecondaryTexture;
        public IntPtr Material;
        public IntPtr Mesh;
        public IntPtr VertexBuffer;
        public IntPtr IndexBuffer;
        public IntPtr InstanceBuffer;
        public uint AtlasColumns;
        public uint AtlasRows;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossTerrainDesc
    {
        public uint Width;
        public uint Height;
        public uint ChunkSize;
        public float CellSize;
        public float MaxHeight;
        public IntPtr HeightData;
        public uint HeightCount;
        public IntPtr HeightmapPath;
        public MossComponentResourceBinding Resources;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossOpenWorldTerrainDesc
    {
        public MossTerrainDesc Terrain;
        public uint StreamingRadiusChunks;
        public uint LodCount;
        [MarshalAs(UnmanagedType.I1)] public bool EnablePhysics;
        [MarshalAs(UnmanagedType.I1)] public bool EnableFoliage;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossParticleEmitterDesc
    {
        public MossComponentParticleSimulation Simulation;
        public uint Capacity;
        public float SpawnRate;
        public float LifetimeSeconds;
        public Float3 PositionMin;
        public Float3 PositionMax;
        public Float3 VelocityMin;
        public Float3 VelocityMax;
        public float StartSize;
        public float EndSize;
        public Color StartColor;
        public Color EndColor;
        public MossComponentResourceBinding Resources;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossTilemapDesc
    {
        public uint Width;
        public uint Height;
        public uint LayerCount;
        public Float2 TileSize;
        public IntPtr Tiles;
        public uint TileCount;
        public MossComponentResourceBinding Resources;
    }

    [StructLayout(LayoutKind.Sequential)] internal struct MossGridmapCellDesc { public int X; public int Y; public int Z; public uint MeshId; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGridmapDesc
    {
        public uint Width;
        public uint Height;
        public uint Depth;
        public Float3 CellSize;
        public IntPtr Cells;
        public uint CellCount;
        public MossComponentResourceBinding Resources;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossLandscapeDesc
    {
        public MossTerrainDesc Terrain;
        public float FoliageDensity;
        public float WindStrength;
        [MarshalAs(UnmanagedType.I1)] public bool EnableOpenWorldStreaming;
        public MossComponentResourceBinding FoliageResources;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossOpenSeaDesc
    {
        public float Width;
        public float Depth;
        public float PatchSize;
        public float WaveAmplitude;
        public float WaveSpeed;
        public Float3 WindDirection;
        public MossComponentResourceBinding Resources;
    }

    internal static class MossComponentsNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_TerrainCreate(in MossTerrainDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerrainDestroy(IntPtr terrain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerrainUpdate(IntPtr terrain, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerrainRender(IntPtr terrain, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_TerrainSerialize(IntPtr terrain, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenWorldTerrainCreate(in MossOpenWorldTerrainDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenWorldTerrainDestroy(IntPtr terrain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenWorldTerrainUpdate(IntPtr terrain, in Float3 cameraPosition, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenWorldTerrainRender(IntPtr terrain, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_OpenWorldTerrainSerialize(IntPtr terrain, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_ParticleEffectCreate(in MossParticleEmitterDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ParticleEffectDestroy(IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ParticleEffectUpdate(IntPtr effect, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_ParticleEffectRender(IntPtr effect, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_ParticleEffectGetAliveCount(IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_ParticleEffectSerialize(IntPtr effect, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_TilemapCreate(in MossTilemapDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TilemapDestroy(IntPtr tilemap);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TilemapRender(IntPtr tilemap, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_TilemapGetTile(IntPtr tilemap, uint x, uint y, uint layer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_TilemapSetTile(IntPtr tilemap, uint x, uint y, uint layer, uint tileId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_TilemapSerialize(IntPtr tilemap, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GridmapCreate(in MossGridmapDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GridmapDestroy(IntPtr gridmap);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GridmapRender(IntPtr gridmap, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GridmapSetCell(IntPtr gridmap, in MossGridmapCellDesc cell);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GridmapRemoveCell(IntPtr gridmap, int x, int y, int z);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GridmapGetCell(IntPtr gridmap, int x, int y, int z, out MossGridmapCellDesc outCell);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_GridmapSerialize(IntPtr gridmap, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_LandscapeCreate(in MossLandscapeDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_LandscapeDestroy(IntPtr landscape);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_LandscapeUpdate(IntPtr landscape, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_LandscapeRender(IntPtr landscape, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_LandscapeSerialize(IntPtr landscape, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_OpenSeaCreate(in MossOpenSeaDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenSeaDestroy(IntPtr sea);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenSeaUpdate(IntPtr sea, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_OpenSeaRender(IntPtr sea, IntPtr renderer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_OpenSeaSerialize(IntPtr sea, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_ComponentSettingsSerialize(MossComponentKind kind, IntPtr component, IntPtr buffer, nuint bufferSize);
    }
}
