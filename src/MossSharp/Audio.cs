using System;
using System.Runtime.InteropServices;
using MossSharp.Native.Audio;

namespace Moss.Audio
{
    public enum AudioLoadType { FullyLoaded, Streaming }
    public enum DistanceModel { Linear, Inverse, Exponential }
    public delegate void AudioStreamCallback(IntPtr buffer, int frames, IntPtr userData);

    public readonly record struct AudioAssetHandle(nuint Value)
    {
        public bool IsNull => Value == 0;
    }

    public static class AudioSystem
    {
        private static bool initialized;

        public static bool IsInitialized => initialized;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            int result = MossAudioNative.Moss_Init_Audio();
            if (result != 0)
            {
                throw new InvalidOperationException("Failed to initialize Moss Audio.");
            }

            initialized = true;
        }

        public static void Shutdown()
        {
            if (!initialized)
            {
                return;
            }

            MossAudioNative.Moss_Terminate_Audio();
            initialized = false;
        }

        public static void Update(float deltaTime) => MossAudioNative.Moss_AudioUpdate(deltaTime);

        public static AudioAssetHandle LoadAsset(IntPtr assetManager, string path, AudioLoadType loadType = AudioLoadType.FullyLoaded)
        {
            IntPtr nativePath = Marshal.StringToCoTaskMemUTF8(path);
            try
            {
                var desc = new MossAudioNative.MossAudioAssetDesc
                {
                    Path = nativePath,
                    LoadType = (MossAudioNative.AudioLoadType)loadType
                };
                return new AudioAssetHandle(MossAudioNative.Moss_AudioAssetLoad(assetManager, in desc));
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativePath);
            }
        }
    }

    public sealed class AudioStream : IDisposable
    {
        private MossAudioNative.AudioStreamCallback? callback;

        private AudioStream(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; private set; }
        public bool IsDisposed => Handle == IntPtr.Zero;

        public static AudioStream Create()
        {
            IntPtr handle = MossAudioNative.Moss_AudioStreamCreate();
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create audio stream.");
            }

            return new AudioStream(handle);
        }

        public void Play()
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamPlay(Handle);
        }

        public void Stop()
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamStop(Handle);
        }

        public void SetVolume(float volume)
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamSetVolume(Handle, volume);
        }

        public void SetPitch(float pitch)
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamSetPitch(Handle, pitch);
        }

        public void SetLoop(bool loop)
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamSetLoop(Handle, loop);
        }

        public void SetPlaybackRate(float rate)
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamSetPlaybackRate(Handle, rate);
        }

        public void SetPan(float pan)
        {
            EnsureValid();
            MossAudioNative.Moss_AudioStreamSetPan(Handle, pan);
        }

        public void SetCallback(AudioStreamCallback streamCallback)
        {
            EnsureValid();
            callback = (buffer, frames, userData) => streamCallback(buffer, frames, userData);
            MossAudioNative.Moss_AudioStreamSetCallback(Handle, callback, IntPtr.Zero);
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MossAudioNative.Moss_AudioStreamRemove(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private void EnsureValid()
        {
            if (Handle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(nameof(AudioStream));
            }
        }
    }
}