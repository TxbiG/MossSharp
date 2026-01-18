namespace MossSharp.Native {
    internal static class MossNativeLibrary {
#if WINDOWS
        public const string Name = "moss.dll";
#elif LINUX
        public const string Name = "libmoss.so";
#elif MACOS
        public const string Name = "libmoss.dylib";
#else
        public const string Name = "moss";
#endif
    }
}
