using DarkOptimizer.Core.FreeRam;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DarkOptimizer.Infrastructure.Memory;

public sealed class FreeRamService : IFreeRamService
{
    private const uint QUOTA_LIMITS_HARDWS_MIN_DISABLE = 0x00000002;
    private const uint QUOTA_LIMITS_HARDWS_MAX_DISABLE = 0x00000004;
    private const int STATUS_SUCCESS = 0;

    public ValueTask<MemorySnapshot> GetMemorySnapshotAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        MemoryNativeMethods.MemoryStatusEx status = new()
        {
            dwLength = (uint)Marshal.SizeOf<MemoryNativeMethods.MemoryStatusEx>()
        };

        if (!MemoryNativeMethods.GlobalMemoryStatusEx(ref status))
        {
            var error = Marshal.GetLastWin32Error();
            throw new MemorySnapshotUnavailableException(
                $"No se pudo leer el estado de memoria del sistema (GlobalMemoryStatusEx). Win32 error: {error}.",
                new Win32Exception(error));
        }

        return ValueTask.FromResult(new MemorySnapshot(
            status.ullTotalPhys,
            status.ullAvailPhys,
            status.ullTotalPageFile,
            status.ullAvailPageFile,
            DateTimeOffset.UtcNow));
    }

    public async ValueTask<FreeRamRunResult> ReduceProcessWorkingSetsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var before = await GetMemorySnapshotAsync(cancellationToken).ConfigureAwait(false);
        var sw = Stopwatch.StartNew();

        var ok = MemoryNativeMethods.SetProcessWorkingSetSizeEx(
            MemoryNativeMethods.GetCurrentProcess(),
            (nint)(-1),
            (nint)(-1),
            QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE);

        sw.Stop();
        var after = await GetMemorySnapshotAsync(cancellationToken).ConfigureAwait(false);
        var reclaimed = (long)after.AvailablePhysicalBytes - (long)before.AvailablePhysicalBytes;

        return new FreeRamRunResult(
            "ReduceProcessWorkingSets",
            ok,
            reclaimed,
            sw.Elapsed,
            ok ? "Working set trim invoked." : "SetProcessWorkingSetSizeEx failed.");
    }

    public ValueTask<FreeRamRunResult> EmptyWorkingSetsAsync(CancellationToken cancellationToken)
        => ExecuteMemoryListCommandAsync(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryEmptyWorkingSets,
            "EmptyWorkingSets",
            cancellationToken);

    public ValueTask<FreeRamRunResult> PurgeStandbyListAsync(CancellationToken cancellationToken)
        => ExecuteMemoryListCommandAsync(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryPurgeStandbyList,
            "PurgeStandbyList",
            cancellationToken);

    public ValueTask<FreeRamRunResult> PurgeModifiedPageListAsync(CancellationToken cancellationToken)
        => ExecuteMemoryListCommandAsync(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryFlushModifiedList,
            "PurgeModifiedPageList",
            cancellationToken);

    public ValueTask<FreeRamRunResult> PurgeLowPriorityStandbyListAsync(CancellationToken cancellationToken)
        => ExecuteMemoryListCommandAsync(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryPurgeLowPriorityStandbyList,
            "PurgeLowPriorityStandbyList",
            cancellationToken);

    public async ValueTask<FreeRamRunResult> CombinedAggressiveTrimAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var sw = Stopwatch.StartNew();

        var workingSet = await ReduceProcessWorkingSetsAsync(cancellationToken).ConfigureAwait(false);
        var emptyWorkingSets = await EmptyWorkingSetsAsync(cancellationToken).ConfigureAwait(false);
        var standby = await PurgeStandbyListAsync(cancellationToken).ConfigureAwait(false);
        var modified = await PurgeModifiedPageListAsync(cancellationToken).ConfigureAwait(false);
        var lowPriority = await PurgeLowPriorityStandbyListAsync(cancellationToken).ConfigureAwait(false);

        sw.Stop();

        return new FreeRamRunResult(
            "CombinedAggressiveTrim",
            workingSet.Succeeded && emptyWorkingSets.Succeeded && standby.Succeeded && modified.Succeeded && lowPriority.Succeeded,
            workingSet.ReclaimedBytes + emptyWorkingSets.ReclaimedBytes + standby.ReclaimedBytes + modified.ReclaimedBytes + lowPriority.ReclaimedBytes,
            sw.Elapsed,
            $"{workingSet.Message} {emptyWorkingSets.Message} {standby.Message} {modified.Message} {lowPriority.Message}");
    }

    private async ValueTask<FreeRamRunResult> ExecuteMemoryListCommandAsync(
        MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND command,
        string strategy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var before = await GetMemorySnapshotAsync(cancellationToken).ConfigureAwait(false);
        var sw = Stopwatch.StartNew();

        var value = (int)command;
        var size = (uint)sizeof(int);

        unsafe
        {
            var status = MemoryNativeMethods.NtSetSystemInformation(
                MemoryNativeMethods.SYSTEM_INFORMATION_CLASS.SystemMemoryListInformation,
                (nint)(&value),
                size);

            sw.Stop();

            var after = await GetMemorySnapshotAsync(cancellationToken).ConfigureAwait(false);
            var reclaimed = (long)after.AvailablePhysicalBytes - (long)before.AvailablePhysicalBytes;

            return new FreeRamRunResult(
                strategy,
                status == STATUS_SUCCESS,
                reclaimed,
                sw.Elapsed,
                status == STATUS_SUCCESS ? "Command completed." : $"NtSetSystemInformation status: {status}");
        }
    }
}
