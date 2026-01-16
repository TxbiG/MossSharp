using System;
using System.Runtime.InteropServices;

using AudioEffect = IntPtr;
using AudioStream = IntPtr;
using AudioStream2D = IntPtr;
using AudioStream3D = IntPtr;
using AudioListener2D = IntPtr;
using AudioListener3D = IntPtr;
using RayAudioListener2D = IntPtr;
using RayAudioListener3D = IntPtr;
using Moss_AudioSource = IntPtr;
using Microphone = IntPtr;
using ChannelID = UInt32;


public enum AudioEffectType : int {
            LOWPASS = 0,
            HIGHPASS = 1 << 0,
            ECHO = 1 << 1,
            FLANGE = 1 << 2,
            DISTORTION = 1 << 3,
            NORMALIZE = 1 << 4,
            PARAMEQ = 1 << 5,
            PITCHSHIFTER = 1 << 6,
            CHORUS = 1 << 7,
            COMPRESSOR = 1 << 8,
            REVERB = 1 << 9,
            DELAY = 1 << 10,
            DOPPLER = 1 << 11,
            PANNING = 1 << 12,
            DISTANCE_ATTENUATION = 1 << 13
}

public enum DistanceModel : int{
            LINEAR,
            INVERSE,
            EXPONENTIAL
        }

public enum AudioLoadType : int{
            FULLY_LOADED,
            STREAMING
}



[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MicrophoneCallback(IntPtr buffer, int samples, IntPtr userData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void AudioStreamCallback(IntPtr buffer, int frames, IntPtr userData);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern int Moss_Init_Audio();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_Terminate_Audio();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioUpdate(float deltaTime);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_AudioSource Moss_AudioLoadWav();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_AudioSource Moss_AudioLoadOgg(string filename, AudioLoadType type);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_AudioSource Moss_AudioLoadMP3(string filename);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern Moss_AudioSource Moss_AudioCaptureMicrophone(Microphone mic);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern AudioEffect Moss_AudioCreateEffect(AudioEffectType type);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioSetEffectParameter(AudioEffect effect, string paramName, float value);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioRemoveEffect(AudioEffect effect);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern ChannelID Moss_AudioCreateChannel(ChannelID channel);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Audio_RemoveChannel(ChannelID channel);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern ChannelID Moss_AudioGetMasterChannel();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioSetChannelVolume(ChannelID channel, float volume);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioSetChannelMute(ChannelID channel, byte mute);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioAddChannelEffect(ChannelID channel, AudioEffect effect);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioRemoveChannelEffect(ChannelID channel, AudioEffect effect);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioRemoveAllChannelEffects(ChannelID channel);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern AudioStream Moss_AudioStreamCreate();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamPlay(AudioStream stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamStop(AudioStream stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamSetVolume(AudioStream stream, float volume);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamSetPitch(AudioStream stream, float pitch);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamSetLoop(AudioStream stream, byte loop);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamRemove(AudioStream stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStreamSetCallback(AudioStream stream, AudioStreamCallback callback, IntPtr userData);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern AudioStream3D Moss_AudioStream3DCreate();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DPlay(AudioStream3D stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DStop(AudioStream3D stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DSetVolume(AudioStream3D stream, float volume);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DSetPitch(AudioStream3D stream, float pitch);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DSetLoop(AudioStream3D stream, byte loop);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DSetDistanceModel(AudioStream3D stream,  DistanceModel model);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioStream3DRemove(AudioStream3D stream);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern AudioListener3D Moss_AudioCreateAudioListener3D();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioActivateAudioListener3D( AudioListener3D listener, byte activate);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioListenerSetOrientation( AudioListener3D listener, IntPtr forward, IntPtr up);

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern int Moss_AudioMicrophoneOpen();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioMicrophoneClose();

[DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
public static extern void Moss_AudioMicrophoneSetCallback( Microphone mic, MicrophoneCallback callback, IntPtr userData);
