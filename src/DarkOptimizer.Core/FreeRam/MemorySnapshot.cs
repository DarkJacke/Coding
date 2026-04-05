namespace DarkOptimizer.Core.FreeRam;

public readonly record struct MemorySnapshot(
    ulong TotalPhysicalBytes,
    ulong AvailablePhysicalBytes,
    ulong TotalPageFileBytes,
    ulong AvailablePageFileBytes,
    DateTimeOffset CollectedAtUtc)
{
    public double UsedPercent => TotalPhysicalBytes == 0
        ? 0
        : ((double)(TotalPhysicalBytes - AvailablePhysicalBytes) / TotalPhysicalBytes) * 100d;
}
