namespace DarkOptimizer.Core.FreeRam;

public readonly record struct FreeRamRunResult(
    string Strategy,
    bool Succeeded,
    long ReclaimedBytes,
    TimeSpan Duration,
    string Message);
