using System;

namespace Moss
{
    public readonly record struct NativeHandle(IntPtr Value)
    {
        public bool IsNull => Value == IntPtr.Zero;
        public static NativeHandle Null => new(IntPtr.Zero);
        public static implicit operator IntPtr(NativeHandle handle) => handle.Value;
        public static explicit operator NativeHandle(IntPtr value) => new(value);
    }

    public static class MossRuntime
    {
        public static void InitializeAudio() => Audio.AudioSystem.Initialize();
        public static void ShutdownAudio() => Audio.AudioSystem.Shutdown();
        public static void UpdateAudio(float deltaTime) => Audio.AudioSystem.Update(deltaTime);
    }
}