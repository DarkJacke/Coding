using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

namespace DarkOptimizer.App;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        WindowsAppSdkBootstrapper.Initialize();

        Application.Start(static _ =>
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(dispatcherQueue));
            _ = new App();
        });
    }
}

internal static class WindowsAppSdkBootstrapper
{
    private const int APPMODEL_ERROR_NO_PACKAGE = 15700;
    private const uint WINDOWS_APPSDK_VERSION_1_7 = 0x00010007;

    private static bool _initialized;
    private static nint _libraryHandle;

    public static void Initialize()
    {
        if (_initialized || IsPackaged())
        {
            return;
        }

        if (!NativeLibrary.TryLoad("Microsoft.WindowsAppRuntime.Bootstrap.dll", out _libraryHandle))
        {
            throw new InvalidOperationException(
                "Windows App SDK bootstrapper DLL not found. Publish with <WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>.");
        }

        var initializePointer = NativeLibrary.GetExport(_libraryHandle, "MddBootstrapInitialize2");
        var initialize = Marshal.GetDelegateForFunctionPointer<MddBootstrapInitialize2Delegate>(initializePointer);

        var hr = initialize(WINDOWS_APPSDK_VERSION_1_7, null, 0UL, 0U);
        if (hr < 0)
        {
            throw new InvalidOperationException($"MddBootstrapInitialize2 failed with HRESULT 0x{hr:X8}.");
        }

        _initialized = true;
        AppDomain.CurrentDomain.ProcessExit += static (_, _) => Shutdown();
    }

    private static bool IsPackaged()
    {
        uint length = 0;
        var result = GetCurrentPackageFullName(ref length, null);
        return result != APPMODEL_ERROR_NO_PACKAGE;
    }

    private static void Shutdown()
    {
        if (!_initialized || _libraryHandle == 0)
        {
            return;
        }

        var shutdownPointer = NativeLibrary.GetExport(_libraryHandle, "MddBootstrapShutdown");
        var shutdown = Marshal.GetDelegateForFunctionPointer<MddBootstrapShutdownDelegate>(shutdownPointer);
        shutdown();

        NativeLibrary.Free(_libraryHandle);
        _libraryHandle = 0;
        _initialized = false;
    }

    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GetCurrentPackageFullName(ref uint packageFullNameLength, string? packageFullName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    private delegate int MddBootstrapInitialize2Delegate(uint majorMinorVersion, string? versionTag, ulong minVersion, uint options);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void MddBootstrapShutdownDelegate();
}
