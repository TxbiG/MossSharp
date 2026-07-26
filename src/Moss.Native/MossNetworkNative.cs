using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Network
{
    internal enum ENetSocketType
    {
        Stream = 1,
        Datagram = 2
    }

    internal enum ENetSocketShutdown
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2
    }

    internal enum ENetAddressType
    {
        Any = 0,
        IPv4 = 1,
        IPv6 = 2
    }

    [Flags]
    internal enum ENetSocketWait : uint
    {
        None = 0,
        Send = 1 << 0,
        Receive = 1 << 1,
        Interrupt = 1 << 2
    }

    internal enum ENetSocketOption
    {
        NonBlock = 1,
        Broadcast = 2,
        ReceiveBuffer = 3,
        SendBuffer = 4,
        ReuseAddress = 5,
        ReceiveTimeout = 6,
        SendTimeout = 7,
        Error = 8,
        NoDelay = 9,
        TimeToLive = 10,
        IPv6Only = 11
    }

    [Flags]
    internal enum ENetPacketFlag : uint
    {
        Reliable = 1 << 0,
        Unsequenced = 1 << 1,
        NoAllocate = 1 << 2,
        UnreliableFragment = 1 << 3,
        Sent = 1 << 8
    }

    internal enum ENetPeerState : byte
    {
        Disconnected = 0,
        Connecting = 1,
        AcknowledgingConnect = 2,
        ConnectionPending = 3,
        ConnectionSucceeded = 4,
        Connected = 5,
        DisconnectLater = 6,
        Disconnecting = 7,
        AcknowledgingDisconnect = 8,
        Zombie = 9
    }

    internal enum ENetEventType
    {
        None = 0,
        Connect = 1,
        Disconnect = 2,
        Receive = 3
    }

    internal enum ENetTransportType
    {
        Udp = 0,
        Tcp = 1,
        WebSocket = 2,
        WebRtc = 3
    }

    [Flags]
    internal enum ENetTransportCaps : uint
    {
        None = 0,
        Reliable = 1 << 0,
        Ordered = 1 << 1,
        Unreliable = 1 << 2,
        Fragmentation = 1 << 3,
        Encryption = 1 << 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ENetAddress
    {
        public ENetAddressType Type;
        public ushort Port;
        public fixed ushort Host[8];
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetBuffer
    {
        public IntPtr Data;
        public nuint DataLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetPacket
    {
        public nuint ReferenceCount;
        public uint Flags;
        public IntPtr Data;
        public nuint DataLength;
        public IntPtr FreeCallback;
        public IntPtr UserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetEvent
    {
        public ENetEventType Type;
        public IntPtr Peer;
        public byte ChannelId;
        public uint Data;
        public IntPtr Packet;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetCallbacks
    {
        public IntPtr Malloc;
        public IntPtr Free;
        public IntPtr NoMemory;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossNetworkHostDesc
    {
        public IntPtr BindAddress;
        public ushort Port;
        public nuint MaxPeers;
        public nuint Channels;
        public uint IncomingBandwidth;
        public uint OutgoingBandwidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetTransportConfig
    {
        public ENetTransportType Type;
        public IntPtr Endpoint;
        public ushort Port;
        public uint Flags;
        public IntPtr UserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetTransportPeer
    {
        public uint Id;
        public IntPtr UserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetTransportEvent
    {
        public int Type;
        public ENetTransportPeer Peer;
        public IntPtr Data;
        public nuint DataLength;
        public uint Data32;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetPacketHeader
    {
        public ushort ProtocolId;
        public ushort Sequence;
        public ushort Ack;
        public uint AckBits;
        public byte ChannelId;
        public byte Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetPacket
    {
        public NetPacketHeader Header;
        public IntPtr Data;
        public nuint Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetHttpTLSConfig
    {
        public IntPtr CertificatePath;
        public IntPtr PrivateKeyPath;
        [MarshalAs(UnmanagedType.I1)] public bool VerifyPeer;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetHttpHeader
    {
        public IntPtr Name;
        public IntPtr Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetHttpRequestDesc
    {
        public int Method;
        public IntPtr Url;
        public IntPtr Headers;
        public nuint HeaderCount;
        public IntPtr Body;
        public nuint BodyLength;
        public ENetHttpTLSConfig Tls;
        public IntPtr UserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENetHttpResponse
    {
        public int StatusCode;
        public IntPtr Headers;
        public nuint HeaderCount;
        public IntPtr Body;
        public nuint BodyLength;
        public int Error;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void ENetPacketAcknowledgedCallback(IntPtr packet);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void ENetPacketFreeCallback(IntPtr packet);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint ENetChecksumCallback(IntPtr buffers, nuint bufferCount);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int ENetInterceptCallback(IntPtr host, ref ENetEvent @event);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void ENetHttpEndpointHandler(IntPtr request, IntPtr response, IntPtr userData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void ENetHttpResponseCallback(in ENetHttpResponse response, IntPtr userData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int ENetConnectValidator(IntPtr peer, IntPtr data, nuint dataLength);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void ENetWebRTCSignalCallback(IntPtr host, IntPtr peer, IntPtr signal, IntPtr userData);

    internal static unsafe class MossNetworkNative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_Init_Network();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_initialize_with_callbacks(in ENetCallbacks inits);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_TerminateNetwork();

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_time_get();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_time_set(uint timeBase);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nint enet_socket_create(ENetAddressType addressType, ENetSocketType socketType);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_bind(nint socket, in ENetAddress address);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_get_address(nint socket, out ENetAddress address);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_listen(nint socket, int backlog);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nint enet_socket_accept(nint socket, out ENetAddress address);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_connect(nint socket, in ENetAddress address);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_send(nint socket, in ENetAddress address, ENetBuffer* buffers, nuint bufferCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_receive(nint socket, out ENetAddress address, ENetBuffer* buffers, nuint bufferCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_wait(nint socket, ref uint condition, uint timeout);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_set_option(nint socket, ENetSocketOption option, int value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_get_option(nint socket, ENetSocketOption option, out int value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socket_shutdown(nint socket, ENetSocketShutdown how);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_socket_destroy(nint socket);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_socketset_select(nint maxSocket, IntPtr readSet, IntPtr writeSet, uint timeout);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_set_host_ip(ref ENetAddress address, [MarshalAs(UnmanagedType.LPUTF8Str)] string hostName);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_set_host(ref ENetAddress address, ENetAddressType type, [MarshalAs(UnmanagedType.LPUTF8Str)] string hostName);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_get_host_ip(in ENetAddress address, IntPtr hostName, nuint nameLength);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_get_host(in ENetAddress address, IntPtr hostName, nuint nameLength);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_equal_host(in ENetAddress firstAddress, in ENetAddress secondAddress);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_address_is_broadcast(in ENetAddress address);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_address_build_any(out ENetAddress address, ENetAddressType type);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_address_build_loopback(out ENetAddress address, ENetAddressType type);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_address_convert_ipv6(ref ENetAddress address);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_crc32(ENetBuffer* buffers, nuint bufferCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_host_create(ENetAddressType type, in ENetAddress address, nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_destroy(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_host_check_events(IntPtr host, out ENetEvent @event);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_host_service(IntPtr host, out ENetEvent @event, uint timeout);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_flush(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_broadcast(IntPtr host, byte channelId, IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_channel_limit(IntPtr host, nuint channelLimit);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_bandwidth_limit(IntPtr host, uint incomingBandwidth, uint outgoingBandwidth);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_bandwidth_throttle(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_compress(IntPtr host, IntPtr compressor);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_host_compress_with_range_coder(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_host_create_secure(in ENetAddress address, nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth, ENetTransportType transport, IntPtr security);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_connect_validator(IntPtr host, ENetConnectValidator validator);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_webrtc_signal_callback(IntPtr host, ENetWebRTCSignalCallback callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_random_seed();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_random(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_host_connect(IntPtr host, in ENetAddress address, nuint channelCount, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_connect_ex")] internal static extern IntPtr enet_host_connect_ex(IntPtr host, [MarshalAs(UnmanagedType.LPUTF8Str)] string endpoint, nuint channelCount, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_transport_caps(IntPtr host);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_peer_send(IntPtr peer, byte channelId, IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_ping(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_ping_interval(IntPtr peer, uint pingInterval);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_timeout")] internal static extern void enet_peer_timeout_full(IntPtr peer, uint timeoutLimit, uint timeoutMinimum, uint timeoutMaximum);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_reset(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_disconnect(IntPtr peer, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_disconnect_now(IntPtr peer, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_disconnect_later(IntPtr peer, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_throttle_configure(IntPtr peer, uint interval, uint acceleration, uint deceleration);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_peer_throttle(IntPtr peer, uint rtt);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_peer_has_outgoing_commands(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_reset_queues(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_setup_outgoing_command(IntPtr peer, IntPtr command);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_peer_queue_outgoing_command(IntPtr peer, IntPtr command, IntPtr packet, uint offset, ushort length);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_peer_queue_incoming_command(IntPtr peer, IntPtr command, IntPtr data, nuint dataLength, uint flags, uint fragmentCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_peer_queue_acknowledgement(IntPtr peer, IntPtr command, ushort sentTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_dispatch_incoming_unreliable_commands(IntPtr peer, IntPtr channel, IntPtr command);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_dispatch_incoming_reliable_commands(IntPtr peer, IntPtr channel, IntPtr command);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_on_connect(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_on_disconnect(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_peer_receive(IntPtr peer, out byte channelId);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_packet_create(IntPtr data, nuint dataLength, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_packet_destroy(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_packet_resize(IntPtr packet, nuint dataLength);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_packet_get_data(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_packet_get_user_data(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_packet_set_user_data(IntPtr packet, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_packet_get_length(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_packet_set_acknowledge_callback(IntPtr packet, ENetPacketAcknowledgedCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_packet_set_free_callback(IntPtr packet, ENetPacketFreeCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_packet_check_references(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_packet_dispose(IntPtr packet);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_get_peers_count(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_get_packets_sent(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_get_packets_received(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_get_bytes_sent(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_host_get_bytes_received(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_max_duplicate_peers(IntPtr host, ushort duplicatePeers);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_intercept_callback(IntPtr host, ENetInterceptCallback callback);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_checksum_callback(IntPtr host, ENetChecksumCallback callback);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_id(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_peer_get_ip(IntPtr peer, IntPtr buffer, nuint bufferLength);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ushort enet_peer_get_port(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_mtu(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ENetPeerState enet_peer_get_state(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_rtt(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_last_rtt(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_lastsendtime(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint enet_peer_get_lastreceivetime(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float enet_peer_get_packets_throttle(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_peer_get_data(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_set_data(IntPtr peer, IntPtr data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_get_stats(IntPtr peer, out float rtt, out float packetLoss, out uint sent, out uint received);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_packet_encode(in NetPacket packet, IntPtr outBuffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool enet_packet_decode(out NetPacket packet, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_fragment_send(IntPtr peer, byte channelId, IntPtr data, nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool enet_fragment_receive(IntPtr peer, in NetPacket fragment, out NetPacket packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_bandwidth_update(IntPtr host, float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_peer_send_window(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_peer_create_channel(IntPtr peer, [MarshalAs(UnmanagedType.I1)] bool reliable, [MarshalAs(UnmanagedType.I1)] bool ordered);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_peer_reset_channels(IntPtr peer);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_host_set_protocol_id(IntPtr host, ushort protocolId);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern ushort enet_host_get_protocol_id(IntPtr host);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_range_coder_create();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_range_coder_destroy(IntPtr context);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_range_coder_compress(IntPtr context, ENetBuffer* buffers, nuint bufferCount, nuint inLimit, byte* outData, nuint outLimit);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_range_coder_decompress(IntPtr context, byte* inData, nuint inLimit, byte* outData, nuint outLimit);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_protocol_command_size(byte commandNumber);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_list_insert(IntPtr position, IntPtr data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_list_remove(IntPtr position);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_list_move(IntPtr position, IntPtr dataFirst, IntPtr dataLast);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint enet_list_size(IntPtr list);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_list_clear(IntPtr list);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkGetLastError();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkCreateHost(in MossNetworkHostDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkCreateClient(nuint channels, uint incomingBandwidth, uint outgoingBandwidth);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NetworkDestroyHost(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkConnect(IntPtr host, [MarshalAs(UnmanagedType.LPUTF8Str)] string address, ushort port, nuint channels, uint userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NetworkDisconnect(IntPtr peer, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NetworkDisconnectNow(IntPtr peer, uint data);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NetworkPoll(IntPtr host, out ENetEvent @event, uint timeoutMs);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NetworkFlush(IntPtr host);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NetworkSendReliable(IntPtr peer, byte channel, IntPtr data, nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_NetworkSendUnreliable(IntPtr peer, byte channel, IntPtr data, nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkPacketCreate(IntPtr data, nuint size, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_NetworkPacketDestroy(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_NetworkPacketData(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_NetworkPacketSize(IntPtr packet);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NetworkWriteU16(IntPtr buffer, nuint capacity, ref nuint offset, ushort value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NetworkWriteU32(IntPtr buffer, nuint capacity, ref nuint offset, uint value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NetworkReadU16(IntPtr buffer, nuint size, ref nuint offset, out ushort value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_NetworkReadU32(IntPtr buffer, nuint size, ref nuint offset, out uint value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_host_create_with_transport(in ENetAddress address, nuint peerCount, nuint channelLimit, uint incomingBandwidth, uint outgoingBandwidth, ENetTransportType transport);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_transport_handle_event(IntPtr host, in ENetTransportEvent transportEvent);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_transport_ws_close(IntPtr peer, int code);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_transport_webrtc_signal(IntPtr transport, IntPtr peer, IntPtr signal);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int enet_webrtc_handle_signal(IntPtr host, IntPtr peer, IntPtr signal);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_http_server_start([MarshalAs(UnmanagedType.LPUTF8Str)] string bindHost, ushort port);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_http_server_stop(IntPtr server);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_http_register_endpoint(IntPtr server, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, ENetHttpEndpointHandler handler, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr enet_http_request_async(in ENetHttpRequestDesc desc, ENetHttpResponseCallback callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_http_request_cancel(IntPtr request);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void enet_http_request_release(IntPtr request);
    }
}
