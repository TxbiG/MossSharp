using System;
using MossSharp.Native.Network;

namespace Moss.Network
{
    public static class NetworkSystem
    {
        public static void Initialize()
        {
            int result = MossNetworkNative.Moss_Init_Network();
            if (result != 0)
            {
                throw new InvalidOperationException("Failed to initialize Moss Network.");
            }
        }

        public static void Shutdown() => MossNetworkNative.Moss_TerminateNetwork();
        public static uint Time => MossNetworkNative.enet_time_get();
        public static string LastError => System.Runtime.InteropServices.Marshal.PtrToStringUTF8(MossNetworkNative.Moss_NetworkGetLastError()) ?? string.Empty;
    }

    public sealed class NetworkHost : IDisposable
    {
        public NetworkHost(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; private set; }
        public uint Peers => MossNetworkNative.enet_host_get_peers_count(Handle);
        public uint PacketsSent => MossNetworkNative.enet_host_get_packets_sent(Handle);
        public uint PacketsReceived => MossNetworkNative.enet_host_get_packets_received(Handle);

        public void Flush() => MossNetworkNative.enet_host_flush(Handle);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossNetworkNative.enet_host_destroy(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
    }
}