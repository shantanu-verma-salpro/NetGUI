
using Win32.Graphics.Direct3D;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi.Common;
using static ImGuiNET.DxResources;
using static Win32.Graphics.Direct3D11.Apis;
using Win32;
using static Win32.Apis;
using Win32.Graphics.Dxgi;


namespace ImGuiNET{
    public static class DxManager{
           public static unsafe bool CreateDeviceD3D(IntPtr hWnd)
        {
            var sd = new SwapChainDescription
            {
                BufferCount = 3,
                BufferDesc = new ModeDescription
                {
                    Width = (uint)0,
                    Height = (uint)0,
                    Format = Format.R8G8B8A8Unorm,
                    RefreshRate = new Rational(60, 1)
                },
                Flags = SwapChainFlags.AllowModeSwitch,
                BufferUsage = Win32.Graphics.Dxgi.Usage.RenderTargetOutput,
                OutputWindow = hWnd,
                SampleDesc = new SampleDescription(1, 0),
                Windowed = true,
                SwapEffect = SwapEffect.Discard
            };

            CreateDeviceFlags creationFlags = CreateDeviceFlags.BgraSupport;
            ReadOnlySpan<FeatureLevel> featureLevels =
                [
                        FeatureLevel.Level_11_1,
                        FeatureLevel.Level_11_0,
                        FeatureLevel.Level_10_1

                ];

            FeatureLevel featureLevel = 0;

            var res = D3D11CreateDeviceAndSwapChain(
                null,
                DriverType.Hardware,
                IntPtr.Zero,
                creationFlags,
                featureLevels.GetPointer(),
                (uint)featureLevels.Length,
                D3D11_SDK_VERSION,
                &sd,
                SwapChain.GetAddressOf(),
                Device.GetAddressOf(),
                &featureLevel,
                DeviceContext.GetAddressOf()
            );

            if (res.Failure)
            {
                Console.WriteLine("Unable to initialize DirectX device and swap chain.");
                return false;
            }

            ComPtr<ID3D11Texture2D> pBackBuffer = default;
            SwapChain.Get()->GetBuffer(0, __uuidof<ID3D11Texture2D>(), pBackBuffer.GetVoidAddressOf());
            Device.Get()->CreateRenderTargetView((ID3D11Resource*)pBackBuffer.Get(), null, RenderTargetView.GetAddressOf());
            pBackBuffer.Dispose();
            return true;
        }

        public unsafe static void CleanupDeviceD3D()
        {
            CleanupRenderTarget();
            SwapChain.Dispose();
            DeviceContext.Dispose();
            Device.Dispose();
        }

        public static unsafe void CreateRenderTarget(uint g_ResizeWidth, uint g_ResizeHeight)
        {
            ComPtr<ID3D11Texture2D> pBackBuffer = default;
            SwapChain.Get()->ResizeBuffers(0, g_ResizeWidth, g_ResizeHeight, Format.Unknown, 0);
            SwapChain.Get()->GetBuffer(0, __uuidof<ID3D11Texture2D>(), pBackBuffer.GetVoidAddressOf());
            Device.Get()->CreateRenderTargetView((ID3D11Resource*)pBackBuffer.Get(), null, RenderTargetView.GetAddressOf());
            pBackBuffer.Dispose();
        }

        public static unsafe void CleanupRenderTarget()
        {
            RenderTargetView.Dispose();
        }
    }
}