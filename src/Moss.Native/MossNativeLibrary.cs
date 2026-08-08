namespace MossSharp.Native {
    internal static class MossNativeLibrary {
        /// <summary>
        /// Bare library name without platform-specific extensions.
        /// The .NET runtime resolver automatically maps this to:
        /// - "moss.dll" on Windows
        /// - "libmoss.so" on Linux
        /// - "libmoss.dylib" on macOS
        /// </summary>
        public const string Name = "moss";
    }
}
