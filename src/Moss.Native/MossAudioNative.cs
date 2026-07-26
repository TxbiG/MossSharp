using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.Audio
{
    internal static class MossAudioNative
    {
        internal enum AudioEffectType : int { Lowpass = 0, Highpass = 1, Echo = 2, Flange = 4, Distortion = 8, Normalize = 16, ParamEq = 32, PitchShifter = 64, Chorus = 128, Compressor = 256, Reverb = 512, Delay = 1024, Doppler = 2048, Panning = 4096, DistanceAttenuation = 8192 }
        internal enum DistanceModel : int { Linear, Inverse, Exponential }
        internal enum AudioLoadType : int { FullyLoaded, Streaming }
        internal enum MossMicrophoneSampleFormat : int { F32 }

        [StructLayout(LayoutKind.Sequential)] internal struct Vec2 { public float X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct Vec3 { public float X, Y, Z; }
        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioAssetDesc
        {
            public IntPtr Path;
            public AudioLoadType LoadType;
        }
        [StructLayout(LayoutKind.Sequential)] internal struct MossMicrophoneLevels { public float Rms, Peak, SmoothedVolume, VoiceActivity; }
        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioRayHit2D
        {
            [MarshalAs(UnmanagedType.I1)] public bool Hit;
            public float Fraction;
            public Vec2 Position;
            public Vec2 Normal;
            public float Absorption;
            public float Transmission;
            public uint MaterialId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioRayHit3D
        {
            [MarshalAs(UnmanagedType.I1)] public bool Hit;
            public float Fraction;
            public Vec3 Position;
            public Vec3 Normal;
            public float Absorption;
            public float Transmission;
            public uint MaterialId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioRayTrace2DDesc
        {
            public IntPtr Physics;
            [MarshalAs(UnmanagedType.FunctionPtr)] public MossAudioRaycast2DCallback Raycast;
            public IntPtr UserData;
            public Vec2 ListenerPosition;
            public Vec2 SourcePosition;
            public float MaxDistance;
            public uint ReflectionRays;
            public float DirectOcclusionStrength;
            public float ReflectionStrength;
            public float AirAbsorption;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioRayTrace3DDesc
        {
            public IntPtr Physics;
            [MarshalAs(UnmanagedType.FunctionPtr)] public MossAudioRaycast3DCallback Raycast;
            public IntPtr UserData;
            public Vec3 ListenerPosition;
            public Vec3 SourcePosition;
            public float MaxDistance;
            public uint ReflectionRays;
            public float DirectOcclusionStrength;
            public float ReflectionStrength;
            public float AirAbsorption;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MossAudioRayTraceResult
        {
            [MarshalAs(UnmanagedType.I1)] public bool Audible;
            [MarshalAs(UnmanagedType.I1)] public bool Occluded;
            public float Distance;
            public float Attenuation;
            public float Occlusion;
            public float TransmissionGain;
            public float Lowpass;
            public float ReflectionGain;
            public float ReflectionDelaySeconds;
            public float DelaySeconds;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct MossMicrophoneDesc
        {
            public uint DeviceIndex, SampleRate, Channels, BufferFrames, RingBufferFrames;
            public MossMicrophoneSampleFormat Format;
            [MarshalAs(UnmanagedType.I1)] public bool StartImmediately;
            [MarshalAs(UnmanagedType.I1)] public bool EnableVoiceMetrics;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MicrophoneCallback(IntPtr buffer, int frames, IntPtr userData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void AudioStreamCallback(IntPtr buffer, int frames, IntPtr userData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal delegate bool MossAudioRaycast2DCallback(IntPtr physics, in Vec2 origin, in Vec2 direction, float distance, out MossAudioRayHit2D hit, IntPtr userData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal delegate bool MossAudioRaycast3DCallback(IntPtr physics, in Vec3 origin, in Vec3 direction, float distance, out MossAudioRayHit3D hit, IntPtr userData);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_Init_Audio();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_Terminate_Audio();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioUpdate(float deltaTime);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioLoadWav();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioLoadOgg([MarshalAs(UnmanagedType.LPUTF8Str)] string filename, AudioLoadType type);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioLoadMP3([MarshalAs(UnmanagedType.LPUTF8Str)] string filename);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCaptureMicrophone(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCaptureMossMicrophone(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_AudioAssetLoad(IntPtr manager, in MossAudioAssetDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCreateEffect(AudioEffectType type);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Moss_AudioCreateEffect")] internal static extern void Moss_AudioSetEffectParameter(IntPtr effect, [MarshalAs(UnmanagedType.LPUTF8Str)] string paramName, float value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveEffect(IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_AudioCreateChannel(uint channel);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Audio_RemoveChannel(uint channel);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_AudioGetMasterChannel();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioSetChannelVolume(uint channel, float volume);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioSetChannelMute(uint channel, [MarshalAs(UnmanagedType.I1)] bool mute);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioAddChannelEffect(uint channel, IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveChannelEffect(uint channel, IntPtr effect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveAllChannelEffects(uint channel);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioStreamCreate();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamPlay(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamStop(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetVolume(IntPtr stream, float volume);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetPitch(IntPtr stream, float pitch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetLoop(IntPtr stream, [MarshalAs(UnmanagedType.I1)] bool loop);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetPlaybackRate(IntPtr stream, float rate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetPan(IntPtr stream, float pan);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamRemove(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStreamSetCallback(IntPtr stream, AudioStreamCallback callback, IntPtr userData);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioStream2DCreate();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DPlay(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DStop(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DSetVolume(IntPtr stream, float volume);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DSetPitch(IntPtr stream, float pitch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DSetLoop(IntPtr stream, [MarshalAs(UnmanagedType.I1)] bool loop);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DSetPlaybackRate(IntPtr stream, float rate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DSetPan(IntPtr stream, float pan);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream2DRemove(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioStream3DCreate();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DPlay(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DStop(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetVolume(IntPtr stream, float volume);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetPitch(IntPtr stream, float pitch);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetLoop(IntPtr stream, [MarshalAs(UnmanagedType.I1)] bool loop);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetPlaybackRate(IntPtr stream, float rate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetPan(IntPtr stream, float pan);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetPosition(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetVelocity(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetMaxDistance(IntPtr stream);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DSetDistanceModel(IntPtr stream, DistanceModel model);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioStream3DRemove(IntPtr stream);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCreateAudioListener2D();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCreateAudioListener3D();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCreateRayAudioListener2D();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_AudioCreateRayAudioListener3D();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveAudioListener2D(IntPtr listener);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveAudioListener3D(IntPtr listener);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveRayAudioListener2D(IntPtr listener);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRemoveRayAudioListener3D(IntPtr listener);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioActivateAudioListener2D(IntPtr listener, [MarshalAs(UnmanagedType.I1)] bool activate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioActivateAudioListener3D(IntPtr listener, [MarshalAs(UnmanagedType.I1)] bool activate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioActivateRayAudioListener2D(IntPtr listener, [MarshalAs(UnmanagedType.I1)] bool activate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioActivateRayAudioListener3D(IntPtr listener, [MarshalAs(UnmanagedType.I1)] bool activate);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioListenerSetOrientation(IntPtr listener, in Vec3 forward, in Vec3 up);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRayListener2DSetPhysics(IntPtr listener, IntPtr physics, MossAudioRaycast2DCallback raycast, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRayListener3DSetPhysics(IntPtr listener, IntPtr physics, MossAudioRaycast3DCallback raycast, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRayListener2DSetTraceSettings(IntPtr listener, float maxDistance, uint reflectionRays);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioRayListener3DSetTraceSettings(IntPtr listener, float maxDistance, uint reflectionRays);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioRayTrace2D(in MossAudioRayTrace2DDesc desc, out MossAudioRayTraceResult result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioRayTrace3D(in MossAudioRayTrace3DDesc desc, out MossAudioRayTraceResult result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioRayTraceFromListener2D(IntPtr listener, in Vec2 sourcePosition, out MossAudioRayTraceResult result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioRayTraceFromListener3D(IntPtr listener, in Vec3 sourcePosition, out MossAudioRayTraceResult result);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_AudioRayTraceComputeGain(in MossAudioRayTraceResult result);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsSpeakerDeviceReady();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioSpeakerOpen();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioSpeakerPause();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioSpeakerResume();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioSpeakerIsPaused();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioSelectSpeakerDevice(int id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GetCurrentSpeakerDeviceID();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetSpeakerDeviceName(int id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_ListSpeakerDevices();

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_MicrophoneGetDeviceCount();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MicrophoneGetDeviceName(uint deviceIndex);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_MicrophoneOpen(in MossMicrophoneDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MicrophoneClose(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_MicrophoneStart(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MicrophoneStop(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_MicrophoneRead(IntPtr mic, [Out] float[] outSamples, uint maxFrames);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MicrophoneSetGain(IntPtr mic, float gain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_MicrophoneGetSampleRate(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_MicrophoneGetChannels(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_MicrophoneSetCallback(IntPtr mic, MicrophoneCallback callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern MossMicrophoneLevels Moss_MicrophoneGetLevels(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_MicrophoneGetLevelRMS(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_MicrophoneGetLevelPeak(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_MicrophoneGetSmoothedVolume(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_MicrophoneGetVoiceActivity(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_IsMicrophoneDeviceReady();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_AudioMicrophoneOpen();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioMicrophoneClose();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioMicrophonePlay();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioMicrophoneStop();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_AudioMicrophoneID();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_AudioSelectMicrophoneDevice(int id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GetMicrophoneDeviceName(int index);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_ListMicrophoneDevices();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioMicrophoneSetGain(IntPtr mic, float gain);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_AudioMicrophoneGetSampleRate(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_AudioMicrophoneGetChannels(IntPtr mic);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_AudioMicrophoneSetCallback(IntPtr mic, MicrophoneCallback callback, IntPtr userData);
    }
}
