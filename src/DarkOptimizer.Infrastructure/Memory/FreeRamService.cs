using DarkOptimizer.Core.FreeRam;
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
            return ValueTask.FromResult(default(MemorySnapshot));
        }

        return ValueTask.FromResult(new MemorySnapshot(
            status.ullTotalPhys,
            status.ullAvailPhys,
            status.ullTotalPageFile,
            status.ullAvailPageFile,
            DateTimeOffset.UtcNow));
    }

    public ValueTask<FreeRamRunResult> ReduceProcessWorkingSetsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var before = GetMemorySnapshotAsync(cancellationToken).Result;
        var sw = Stopwatch.StartNew();

        var ok = MemoryNativeMethods.SetProcessWorkingSetSizeEx(
            MemoryNativeMethods.GetCurrentProcess(),
            (nint)(-1),
            (nint)(-1),
            QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE);

        sw.Stop();
        var after = GetMemorySnapshotAsync(cancellationToken).Result;
        var reclaimed = (long)after.AvailablePhysicalBytes - (long)before.AvailablePhysicalBytes;

        return ValueTask.FromResult(new FreeRamRunResult(
            "ReduceProcessWorkingSets",
            ok,
            reclaimed,
            sw.Elapsed,
            ok ? "Working set trim invoked." : "SetProcessWorkingSetSizeEx failed."));
    }

    public ValueTask<FreeRamRunResult> PurgeStandbyListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ExecuteMemoryListCommand(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryPurgeStandbyList,
            "PurgeStandbyList",
            cancellationToken);
    }

    public async ValueTask<FreeRamRunResult> CombinedAggressiveTrimAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var sw = Stopwatch.StartNew();
        var a = await ReduceProcessWorkingSetsAsync(cancellationToken).ConfigureAwait(false);
        var b = await ExecuteMemoryListCommand(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryEmptyWorkingSets,
            "EmptyWorkingSets",
            cancellationToken).ConfigureAwait(false);
        var c = await ExecuteMemoryListCommand(
            MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND.MemoryPurgeLowPriorityStandbyList,
            "PurgeLowPriorityStandbyList",
            cancellationToken).ConfigureAwait(false);

        sw.Stop();
        return new FreeRamRunResult(
            "CombinedAggressiveTrim",
            a.Succeeded && b.Succeeded && c.Succeeded,
            a.ReclaimedBytes + b.ReclaimedBytes + c.ReclaimedBytes,
            sw.Elapsed,
            $"{a.Message} {b.Message} {c.Message}");
    }

    private ValueTask<FreeRamRunResult> ExecuteMemoryListCommand(
        MemoryNativeMethods.SYSTEM_MEMORY_LIST_COMMAND command,
        string strategy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var before = GetMemorySnapshotAsync(cancellationToken).Result;
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
            var after = GetMemorySnapshotAsync(cancellationToken).Result;
            var reclaimed = (long)after.AvailablePhysicalBytes - (long)before.AvailablePhysicalBytes;

            return ValueTask.FromResult(new FreeRamRunResult(
                strategy,
                status == STATUS_SUCCESS,
                reclaimed,
                sw.Elapsed,
                status == STATUS_SUCCESS ? "Command completed." : $"NtSetSystemInformation status: {status}"));
        }
    }
}
