namespace DarkOptimizer.Core.FreeRam;

public interface IFreeRamService
{
    ValueTask<MemorySnapshot> GetMemorySnapshotAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> ReduceProcessWorkingSetsAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> EmptyWorkingSetsAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> PurgeStandbyListAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> PurgeModifiedPageListAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> PurgeLowPriorityStandbyListAsync(CancellationToken cancellationToken);
    ValueTask<FreeRamRunResult> CombinedAggressiveTrimAsync(CancellationToken cancellationToken);
}
