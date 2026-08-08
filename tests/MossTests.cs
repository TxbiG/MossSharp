using System.Runtime.InteropServices;
using Xunit;

namespace MossSharp.Tests;

/// <summary>
/// Tests that P/Invoke methods can be called at runtime.
/// These tests verify that native Moss DLLs are correctly located and accessible.
/// DllNotFoundException and EntryPointNotFoundException are caught at call time, not compile time.
/// </summary>
public class MossTests
{
    /// <summary>
    /// P/Invoke declaration for a simple Moss function.
    /// Update this with actual Moss exports once the API is defined.
    /// </summary>
    [DllImport("Moss", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint MossGetVersion();

    /// <summary>
    /// Test that the Moss native library can be loaded and a function called.
    /// This test will fail with DllNotFoundException if the native binary is missing.
    /// </summary>
    [Fact]
    public void TestMossLibraryLoaded()
    {
        // This call will throw DllNotFoundException if Moss DLL/so/dylib is not found
        // It will throw EntryPointNotFoundException if the export is missing
        var result = MossGetVersion();

        // If we got here, the native library loaded successfully
        Assert.True(result >= 0, "MossGetVersion should return a version number");
    }

    /// <summary>
    /// Test basic interop with native code.
    /// This ensures the P/Invoke marshalling is correct.
    /// </summary>
    [Fact]
    public void TestMossInterop()
    {
        try
        {
            // Attempt to call a native function
            var version = MossGetVersion();
            Assert.NotEqual(0u, version);
        }
        catch (DllNotFoundException ex)
        {
            Assert.Fail($"Moss native library not found: {ex.Message}");
        }
        catch (EntryPointNotFoundException ex)
        {
            Assert.Fail($"Moss export 'MossGetVersion' not found: {ex.Message}");
        }
    }
}
