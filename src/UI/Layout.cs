using System;
using System.Runtime.InteropServices;
using System.Numerics;
using static ImGuiNET.Window;
using static ImGuiNET.WinMacros;
using static ImGuiNET.Pinvoke;
using static ImGuiNET.WinController;
using static ImGuiNET.WinResources;
using static ImGuiNET.DxResources;
using static ImGuiNET.DxController;
using static ImGuiNET.WinConstants;
using static ImGuiNET.WinStructs;
using static ImGuiNET.DxManager;


namespace ImGuiNET
{
    public class UserInterface
    {
        private const uint WM_SIZE = 0x0005;
        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint WM_DESTROY = 0x0002;
        private const uint SIZE_MINIMIZED = 1;
        private const uint SC_KEYMENU = 0xF100;
        public static uint g_ResizeWidth = 0, g_ResizeHeight = 0;
        public static bool SwapChainOccluded = false;

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            if (HandleInputs(hWnd, msg, wParam, lParam) != 0)
                return IntPtr.Zero;

            switch (msg)
            {
                case WM_SIZE:
                    if (wParam.ToUInt32() == SIZE_MINIMIZED)
                        return IntPtr.Zero;

                    g_ResizeWidth = LOWORD(lParam.ToInt32());
                    g_ResizeHeight = HIWORD(lParam.ToInt32());
                    return IntPtr.Zero;

                case WM_SYSCOMMAND:
                    if ((wParam.ToUInt32() & 0xFFF0) == SC_KEYMENU)
                        return IntPtr.Zero;
                    break;

                case WM_DESTROY:
                    Environment.Exit(0);
                    return IntPtr.Zero;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public virtual void Render() { }

        public unsafe void Run(string title, int width, int height)
        {
            SetProcessDPIAware();
            IntPtr titlePtr = Marshal.StringToHGlobalUni(title);
            if (!CreateWindow(titlePtr, width, height, Marshal.GetFunctionPointerForDelegate<WndProc>(CustomWndProc)))
                return;

            if (!CreateDeviceD3D(windowHandle))
            {
               
                return;
            }

            ShowWindow(windowHandle, (int)SW.ShowDefault);
            UpdateWindow(windowHandle);

            var ctx = ImGui.CreateContext();
            ImGui.SetCurrentContext(ctx);


            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.Fonts.AddFontFromFileTTF("c:\\Windows\\Fonts\\segoeui.ttf", 18.0f);
            ImGui.StyleColorsDark();

            InitWin32Controller();
            InitDxController(Device, DeviceContext);

            Vector4 clearColor = new Vector4(0.45f, 0.55f, 0.60f, 1.00f);
            bool done = false;

            float* clearColorWithAlpha = stackalloc float[4]
            {
                clearColor.X * clearColor.W,
                clearColor.Y * clearColor.W,
                clearColor.Z * clearColor.W,
                clearColor.W
            };

            while (!done)
            {
                while (PeekMessage(out WindowMessage msg, IntPtr.Zero, 0U, 0U, 0x0001))
                {
                    TranslateMessage(ref msg);
                    DispatchMessageW(ref msg);
                    if (msg.message == 0x0012) 
                        done = true;
                }

                if (done)
                    break;

                if (g_ResizeWidth != 0 && g_ResizeHeight != 0)
                {
                    CleanupRenderTarget();
                    CreateRenderTarget(g_ResizeWidth, g_ResizeHeight);
                    g_ResizeWidth = g_ResizeHeight = 0;
                }

                DxNewFrame();
                WindowNewFrame();
                ImGui.NewFrame();
                Render();
                ImGui.Render();
                DeviceContext.Get()->OMSetRenderTargets(1, RenderTargetView.GetAddressOf(), null);
                DeviceContext.Get()->ClearRenderTargetView(RenderTargetView, clearColorWithAlpha);
                RenderDrawData(ImGui.GetDrawData());

                var hr = SwapChain.Get()->Present(1, 0);
            }

            DxShutdown();
            ImGui.DestroyContext();
            CleanupDeviceD3D();
            Shutdown();
        }
    }
}
