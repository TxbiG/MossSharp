
namespace Moss.Audio
{

    using AudioStreamHandle = IntPtr;
  
    public static class AudioSystem
    {
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
                return;

            int result = Native.Audio.MossAudioNative.Moss_Init_Audio();
            if (result != 0)
                throw new InvalidOperationException("Failed to initialize Moss Audio");

            initialized = true;
        }

        public static void Shutdown()
        {
            if (!initialized)
                return;

            Native.Audio.MossAudioNative.Moss_Terminate_Audio();
            initialized = false;
        }

        public static void Update(float deltaTime)
        {
            Native.Audio.MossAudioNative.Moss_AudioUpdate(deltaTime);
        }
    }

    public sealed class AudioStream : IDisposable {
        internal IntPtr Handle { get; private set; }

        internal AudioStream(IntPtr handle) {
            Handle = handle;
        }

        public enum DistanceModel {
            Linear,
            Inverse,
            Exponential
        }


        public static AudioStream CreateStream() {
            IntPtr handle = Native.Audio.MossAudioNative.Moss_AudioStreamCreate();
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create audio stream");
        
            return new AudioStream(handle);
        }

        public void Play() {
            EnsureValid();
            Native.Audio.MossAudioNative.Moss_AudioStreamPlay(Handle);
        }

        public void Stop() {
            EnsureValid();
            Native.Audio.MossAudioNative.Moss_AudioStreamStop(Handle);
        }

        public void Dispose() {
            if (Handle != IntPtr.Zero)
            {
                Native.Audio.MossAudioNative.Moss_AudioStreamRemove(Handle);
                Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private void EnsureValid() {
            if (Handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(AudioStream));
        }

        private MossAudioNative.AudioStreamCallback _callback;

        public void SetCallback(Action<float[]> onAudio) {
            _callback = (buffer, frames, userData) =>
            {
                // marshal data
            };
        
            Native.Audio.MossAudioNative.Moss_AudioStreamSetCallback(Handle, _callback, IntPtr.Zero);
        }
    }
}
