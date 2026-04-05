using System.Runtime.InteropServices;

namespace DarkOptimizer.Infrastructure.Memory;

internal static partial class MemoryNativeMethods
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetProcessWorkingSetSizeEx(nint process, nint minimumWorkingSetSize, nint maximumWorkingSetSize, uint flags);

    [LibraryImport("kernel32.dll")]
    internal static partial nint GetCurrentProcess();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

    [LibraryImport("ntdll.dll")]
    internal static partial int NtSetSystemInformation(SYSTEM_INFORMATION_CLASS systemInformationClass, nint systemInformation, uint systemInformationLength);

    [StructLayout(LayoutKind.Sequential)]
    internal struct MemoryStatusEx
    {
        internal uint dwLength;
        internal uint dwMemoryLoad;
        internal ulong ullTotalPhys;
        internal ulong ullAvailPhys;
        internal ulong ullTotalPageFile;
        internal ulong ullAvailPageFile;
        internal ulong ullTotalVirtual;
        internal ulong ullAvailVirtual;
        internal ulong ullAvailExtendedVirtual;
    }

    internal enum SYSTEM_INFORMATION_CLASS : int
    {
        SystemMemoryListInformation = 80
    }

    internal enum SYSTEM_MEMORY_LIST_COMMAND : int
    {
        MemoryCaptureAccessedBits = 0,
        MemoryCaptureAndResetAccessedBits = 1,
        MemoryEmptyWorkingSets = 2,
        MemoryFlushModifiedList = 3,
        MemoryPurgeStandbyList = 4,
        MemoryPurgeLowPriorityStandbyList = 5,
        MemoryCommandMax = 6
    }
}
