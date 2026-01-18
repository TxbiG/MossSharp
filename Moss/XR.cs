


namespace Moss.XR
{
            public static class XRSystem
            {
                public static void Initialize(XRInitSettings settings)
                {
                    MossXR_InitInfo info = new MossXR_InitInfo
                    {
                        renderer = settings.Renderer,
                        graphicsDevice = settings.GraphicsDevice,
                        graphicsContext = settings.GraphicsContext
                    };
            
                    if (!MossXRNative.Moss_XR_Initialize(ref info))
                        throw new InvalidOperationException("Failed to initialize XR subsystem.");
                }
            }
}
